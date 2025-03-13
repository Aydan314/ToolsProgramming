using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.VersionControl;
using UnityEngine;

public class AssetOutlineBrush : AssetBaseBrush
{
    public override void Build(List<BuildNode> Selection, AssetBasePack assetPack)
    {
        GameObject root = new GameObject();
        root.name = assetPack.name + " Brush";

        BuildNode buildNode = Selection[0];

        if (DetectWindingOrderClockwise(Selection))
        {
            foreach(BuildNode node in Selection)
            {
                node.windingOrderAntiClockwise = false;
            }
        }

        int i = 0;

        while (buildNode != null)
        {
            BuildAssetsBetween(buildNode, assetPack, root);
            buildNode = buildNode.GetNextNode();

            if (buildNode == Selection[0]) break;

            if (i > 100) 
            {
                Debug.LogError("!! Max Selection Iteration Reached !!");
                break;
            }
            i++;
        }
    }

    public bool DetectWindingOrderClockwise(List<BuildNode> selection)
    {
        BuildNode startNode = selection[0];

        Vector3 nextNodeDir = (startNode.nodePos - startNode.nextNode.nodePos).normalized;
        Vector3 prevNodeDir = (startNode.nodePos - startNode.prevNode.nodePos).normalized;

        return nextNodeDir.x != prevNodeDir.z;
    }

    public bool DetectNodeAtCorner(BuildNode buildNode)
    {
        Vector3 nextNodeDir = (buildNode.nodePos - buildNode.nextNode.nodePos).normalized;
        Vector3 prevNodeDir = (buildNode.nodePos - buildNode.prevNode.nodePos).normalized;

        return nextNodeDir != (prevNodeDir * -1.0f);
    }

    public float CalculateAssetRotation(BuildNode buildNode)
    {
        Vector3 nextNodeDir = (buildNode.nodePos - buildNode.GetNextNode().nodePos).normalized;

        if (nextNodeDir.z > 0) return 270.0f;
        else if (nextNodeDir.x < 0) return 180.0f;
        else if (nextNodeDir.z < 0) return 90.0f;

        return 0.0f;
    }

    public float CalculateAssetCornerRotation(BuildNode buildNode)
    {
        Vector3 prevNodeDir = (buildNode.nodePos - buildNode.GetPrevNode().nodePos).normalized;
        Vector3 nextNodeDir = (buildNode.nodePos - buildNode.GetNextNode().nodePos).normalized;

        // Anti clockwise winding order //
        if (prevNodeDir.x < 0 && nextNodeDir.z > 0) return 270.0f;
        else if (prevNodeDir.z < 0 && nextNodeDir.x < 0) return 180.0f;
        else if (prevNodeDir.x > 0 && nextNodeDir.z < 0) return 90.0f;

        // Clockwise winding order //
        else if (prevNodeDir.z < 0 && nextNodeDir.x > 0) return 90.0f;
        else if (prevNodeDir.x < 0 && nextNodeDir.z < 0) return 180.0f;
        else if (prevNodeDir.z > 0 && nextNodeDir.x < 0) return 270.0f;

        return 0.0f;
    }
    public void BuildAssetsBetween(BuildNode node, AssetBasePack assetPack, GameObject rootObject)
    {
        Vector3 end = node.GetNextNode().transform.position;
        Vector3 start = node.gameObject.transform.position;

        float distance = (end - start).magnitude;

        distance = assetPack.gridSize * Mathf.Round(distance / assetPack.gridSize);

        Vector3 step = (end - start) / distance;

        if (assetPack.cornerAssets.Count > 0)
        {
            Asset asset = assetPack.assets[0];

            // Places a corner asset if it is valid //
            if (node.prevNode != node && DetectNodeAtCorner(node))
            {
                asset = assetPack.cornerAssets[0];
                PlaceAsset(asset.assetObject, rootObject, start, CalculateAssetCornerRotation(node) + asset.defaultRotation);
            }
            // Otherwise places a default asset //
            else
            {
                PlaceAsset(asset.assetObject, rootObject, start, CalculateAssetRotation(node) + asset.defaultRotation);
            }
        }
        if (assetPack.assets.Count > 0)
        {
            Asset asset = assetPack.assets[0];
            
            for (float i = assetPack.gridSize; i < distance; i += assetPack.gridSize)
            {
                PlaceAsset(asset.assetObject, rootObject, start + (i * step), CalculateAssetRotation(node) + asset.defaultRotation);
            }
        }
    }
}
