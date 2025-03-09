using System.Collections.Generic;
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

            BuildAssetsBetween(buildNode.startPos, buildNode.endPos, assetPack, root);
        }
    }
    public void BuildAssetsBetween(Vector3 start, Vector3 end, AssetBasePack assetPack, GameObject rootObject)
    {
        Asset asset = assetPack.assets[0];

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

        rotation += asset.defaultRotation;
        
        for (float i = assetPack.gridSize; i < distance; i += assetPack.gridSize)
        {
            GameObject newObject = Instantiate(asset.assetObject);
            newObject.transform.position = start + (i * step);
            newObject.transform.rotation = Quaternion.Euler(0, rotation, 0);
            newObject.transform.parent = rootObject.transform;

        }
    }
}
