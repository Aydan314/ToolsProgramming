using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.VersionControl;
using UnityEngine;

public class AssetOutlineBrush : AssetBaseBrush
{
    public override void Build(List<BuildNodeData> Selection, AssetBasePack assetPack)
    {
        GameObject root = new GameObject();
        root.name = assetPack.name + " Brush";

        BuildNodeData buildNode = Selection[0];

        if (DetectWindingOrderClockwise(Selection))
        {
            Debug.Log("Clockwise");
            foreach (BuildNodeData node in Selection)
            {
                node.windingOrderClockwise = false;
            }
        }
        else
        {
            Debug.Log("AntiClockwise");
        }

        int i = 0;

        while (buildNode != null)
        {
            BuildAssetsBetween(buildNode, assetPack, root);
            buildNode = buildNode.GetNext();

            if (buildNode == Selection[0]) break;

            if (i > 100)
            {
                Debug.LogError("!! Max Selection Iteration Reached !!");
                break;
            }
            i++;
        }
    }

    public bool DetectNodeAtCorner(BuildNodeData buildNode)
    {
        Vector3 nextNodeDir = (buildNode.position - buildNode.GetNext().position).normalized;
        Vector3 prevNodeDir = (buildNode.position - buildNode.GetPrev().position).normalized;

        return nextNodeDir != (prevNodeDir * -1.0f);
    }

    public float CalculateAssetRotation(BuildNodeData buildNode)
    {
        Vector3 nextNodeDir = (buildNode.position - buildNode.GetNext().position).normalized;

        if (nextNodeDir.z > 0) return 270.0f;
        else if (nextNodeDir.x < 0) return 180.0f;
        else if (nextNodeDir.z < 0) return 90.0f;

        return 0.0f;
    }

    public float CalculateAssetCornerRotation(BuildNodeData buildNode)
    {
        Vector3 prevNodeDir = (buildNode.position - buildNode.GetPrev().position).normalized;
        Vector3 nextNodeDir = (buildNode.position - buildNode.GetNext().position).normalized;

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
    public void BuildAssetsBetween(BuildNodeData node, AssetBasePack assetPack, GameObject rootObject)
    {
        Vector3 end = node.GetNext().position;
        Vector3 start = node.position;

        float distance = (end - start).magnitude;

        distance = assetPack.gridSize * Mathf.Round(distance / assetPack.gridSize);

        Vector3 step = (end - start) / distance;

        if (assetPack.cornerAssets.Count > 0)
        {
            Asset asset = assetPack.assets[0];

            // Places a corner asset if it is valid //
            if (node.GetPrev() != node && DetectNodeAtCorner(node))
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
