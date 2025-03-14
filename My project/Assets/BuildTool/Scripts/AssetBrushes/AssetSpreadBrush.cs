using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.VersionControl;
using UnityEngine;

public struct Bounds
{
    public Vector3 max;
    public Vector3 min;
}

public class AssetSpreadBrush : AssetBaseBrush
{
    public override void Build(List<BuildNode> Selection, AssetBasePack assetPack)
    {
        GameObject root = new GameObject();
        root.name = assetPack.name + " Brush";

        Asset asset = assetPack.assets[0];

        Bounds bounds = FindMinMaxBounds(Selection);

        for (float x = bounds.min.x; x <= bounds.max.x; x+= assetPack.gridSize)
        {
            for (float y = bounds.min.z; y <= bounds.max.z; y+=assetPack.gridSize)
            {
                if (Random.Range(0, 1000) / 1000.0f < assetPack.spreadDensity)
                {
                    PlaceAsset(asset.assetObject, root, new Vector3(x, Selection[0].nodePos.y, y), 0);
                }
            }
        }

    }

    public List<List<Bounds>> CreateSelectionLayers(List<BuildNode> selection)
    {
        List<List<Bounds>> selectionLayers = new List<List<Bounds>>();

        Bounds bounds = FindMinMaxBounds(selection);



        return selectionLayers;
    }

    public Bounds FindMinMaxBounds(List<BuildNode> selection)
    {
        Vector3 maxBound = new Vector3(float.MinValue,float.MinValue,float.MinValue);
        Vector3 minBound = new Vector3(float.MaxValue,float.MaxValue,float.MaxValue);

        foreach (BuildNode node in selection)
        {
            maxBound.x = Mathf.Max(maxBound.x, node.nodePos.x);
            maxBound.y = Mathf.Max(maxBound.y, node.nodePos.y);
            maxBound.z = Mathf.Max(maxBound.z, node.nodePos.z);

            minBound.x = Mathf.Min(minBound.x, node.nodePos.x);
            minBound.y = Mathf.Min(minBound.y, node.nodePos.y);
            minBound.z = Mathf.Min(minBound.z, node.nodePos.z);
        }

        Bounds bounds = new Bounds();
        bounds.min = minBound;
        bounds.max = maxBound;

        return bounds;
    }
}
