using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class AssetOutlineBrush : AssetBaseBrush
{
    public override void Build(List<GameObject> Selection, AssetBasePack assetPack)
    {
        GameObject root = new GameObject();
        root.name = assetPack.name + " Brush";

        for (int i = 0; i < Selection.Count; i++)
        {

            GameObject marker = Selection[i];
            BuildNode buildNode = marker.GetComponent<BuildNode>();

            BuildAssetsBetween(buildNode, assetPack, root);
        }
    }
    public void BuildAssetsBetween(BuildNode node, AssetBasePack assetPack, GameObject rootObject)
    {
        Vector3 start = node.startPos;
        Vector3 end = node.endPos;
        Vector3 prevNodePos = node.prevNodePos;

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

            if (prevNodePos != start)
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
