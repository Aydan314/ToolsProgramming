using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.VersionControl;

[System.Serializable]
public class AssetBaseBrush : MonoBehaviour
{
    public virtual void Build(List<GameObject> Selection, AssetBasePack assetPack)
    {
        Debug.Log("Base Brush Build Called Using " + assetPack.name);
    }

    public void BuildAssetsBetween(Vector3 start, Vector3 end, AssetBasePack assetPack)
    {
        Asset asset = assetPack.assets[0];

        Vector3 step = (end - start) / assetPack.gridSize;

        float distance = (end - start).magnitude;
        float distanceStep = distance / assetPack.gridSize;

        for (float i = 0; i < distance; i += distanceStep)
        {
            GameObject newObject = Instantiate(asset.assetObject);
            newObject.transform.position = start + (i * step);
        }
    }
}
