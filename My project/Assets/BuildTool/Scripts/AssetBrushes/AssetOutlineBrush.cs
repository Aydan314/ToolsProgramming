using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class AssetOutlineBrush : AssetBaseBrush
{
    public override void Build(List<BuildNode> Selection, AssetBasePack assetPack)
    {
        GameObject root = new GameObject();
        root.name = assetPack.name + " Brush";

        BuildNode buildNode = Selection[0];

        int i = 0;
       
        while (buildNode != null)
        {
            BuildAssetsBetween(buildNode, assetPack, root);
            buildNode = buildNode.nextNode;

            if (buildNode == Selection[0]) break;

            if (i > 100) 
            {
                Debug.LogError("!! Max Selection Iteration Reached !!");
                break;
            }
            i++;
        }
    }
    public void BuildAssetsBetween(BuildNode node, AssetBasePack assetPack, GameObject rootObject)
    {
        Vector3 end = node.nextNode.transform.position;
        Vector3 start = node.gameObject.transform.position;

        float distance = (end - start).magnitude;

        distance = assetPack.gridSize * Mathf.Round(distance / assetPack.gridSize);

        Vector3 step = (end - start) / distance;

        float rotation = 0;

        if (step.x > 0)
        {
            rotation = 0;
        }
        else if (step.x < 0)
        {
            rotation = 180;
        }
        else if (step.z > 0)
        {
            rotation = 270;
        }
        else if (step.z < 0)
        {
            rotation = 90;
        }

        if (assetPack.cornerAssets.Count > 0)
        {
            Asset asset = assetPack.assets[0];

            if (node.prevNode != node)
            {
                asset = assetPack.cornerAssets[0];
               
            }

            PlaceAsset(asset.assetObject, rootObject, start, rotation + asset.defaultRotation);
        }
        if (assetPack.assets.Count > 0)
        {
            Asset asset = assetPack.assets[0];
            
            for (float i = assetPack.gridSize; i < distance; i += assetPack.gridSize)
            {
                PlaceAsset(asset.assetObject, rootObject, start + (i * step), rotation + asset.defaultRotation);
            }
        }
    }
}
