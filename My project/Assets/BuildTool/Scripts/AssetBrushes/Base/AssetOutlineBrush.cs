using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.VersionControl;
using UnityEngine;

[CreateAssetMenu(fileName = "Outline Brush", menuName = "Scriptable Objects/BuildTool/Brushes/Outline Brush")]

public class AssetOutlineBrush : AssetBaseBrush
{
    public override void Build(List<BuildNodeData> Selection, SpawnableObjectPack assetPack)
    {
        GameObject root = new GameObject();
        root.name = assetPack.name + " Brush";
        root.transform.position = Selection[0].position;

        BuildNodeData buildNode = Selection[0];
        Selection = ForceWindingOrderClockwise(Selection);
        
        int i = 0;

        // Loop through shapes nodes and connect points up with placed assets //
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
        // Detect if directions are perpendicular //
        Vector3 nextNodeDir = (buildNode.position - buildNode.GetNext().position).normalized;
        Vector3 prevNodeDir = (buildNode.position - buildNode.GetPrev().position).normalized;

        return nextNodeDir != (prevNodeDir * -1.0f);
    }

    public float CalculateAssetRotation(BuildNodeData buildNode)
    {
        Vector3 nextNodeDir = (buildNode.position - buildNode.GetNext().position).normalized;

        // Give asset correct rotation based on previous node direction //

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
    public void BuildAssetsBetween(BuildNodeData node, SpawnableObjectPack assetPack, GameObject rootObject)
    {
        Vector3 end = node.GetNext().position;
        Vector3 start = node.position;

        float distance = (end - start).magnitude;

        distance = assetPack.gridSize * Mathf.Round(distance / assetPack.gridSize);

        Vector3 step = (end - start) / distance;

        if (assetPack.GetCornerObjects().Count > 0)
        {
            SpawnableObject asset = assetPack.spawnableObjects[0];

            // Places a corner asset if it is valid //
            if (node.GetPrev() != node && DetectNodeAtCorner(node))
            {
                asset = assetPack.GetCornerObjects()[0];
                if (Random.Range(0,100) / 100.0f < assetPack.spreadDensity) PlaceAsset(asset.objectPrefab, rootObject, start, CalculateAssetCornerRotation(node) + asset.defaultRotation);
            }
            // Otherwise places a default asset //
            else
            {
                if (Random.Range(0, 100) / 100.0f < assetPack.spreadDensity) PlaceAsset(asset.objectPrefab, rootObject, start, CalculateAssetRotation(node) + asset.defaultRotation);
            }
        }
        if (assetPack.spawnableObjects.Count > 0)
        {
            SpawnableObject asset = assetPack.GetDefaultObjects()[0];

            for (float i = assetPack.gridSize; i < distance; i += assetPack.gridSize)
            {
                if (Random.Range(0, 100) / 100.0f < assetPack.spreadDensity) PlaceAsset(asset.objectPrefab, rootObject, start + (i * step), CalculateAssetRotation(node) + asset.defaultRotation);
            }
        }
    }
}
