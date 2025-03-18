using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.VersionControl;
using UnityEngine;
using static UnityEditor.Rendering.InspectorCurveEditor;

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

        List<Bounds> boundsList = CreateSlices(Selection, assetPack, bounds);

        foreach(Bounds b in boundsList)
        {
            Debug.Log(b.min + " " + b.max);
        }

        for (float z = bounds.min.z; z < bounds.max.z; z += assetPack.gridSize)
        {
            List<Bounds> buildBetween = FindBoundsAtZ(z, boundsList);

            buildBetween.OrderByDescending(b => b.min.x).ToList();

            Debug.Log(buildBetween.Count);

            for (int i = buildBetween.Count - 1; i > 0; i -= 2)
            {
                Vector3 min = buildBetween[i - 1].min;
                Vector3 max = buildBetween[i].min;

                min.z = z; max.z = z;

                Debug.Log(min + " aaaaaaaaaaa " + max + " " + i);

                BuildAssetsBetween(min, max, assetPack, root);
            }
        }

    }

    public void BuildAssetsBetween(Vector3 start, Vector3 end, AssetBasePack assetPack, GameObject rootObject)
    {
        Asset asset = assetPack.assets[0];

        float distance = (end - start).magnitude;

        distance = assetPack.gridSize * Mathf.Round(distance / assetPack.gridSize);

        Vector3 step = (end - start) / distance;

        for (float i = assetPack.gridSize; i < distance; i += assetPack.gridSize)
        {
            PlaceAsset(asset.assetObject, rootObject, start + (i * step), asset.defaultRotation);
        }
    }

    public List<Bounds> FindBoundsAtZ(float z, List<Bounds> slices)
    {
        List<Bounds> found = new List<Bounds>();

        foreach(Bounds bound in slices)
        {
            if (bound.min.z <= z && bound.max.z >= z)
            {
                found.Add(bound);
            }
        }

        return found;
    }

    public List<Bounds> CreateSlices(List<BuildNode> selection, AssetBasePack assetPack, Bounds MinMax)
    {
        

        List<Bounds> slices = new List<Bounds>();

        for (float z = MinMax.min.z; z <= MinMax.max.z; z += assetPack.gridSize)
        {
            foreach (BuildNode node in selection)
            {
                if (node.nodePos.z == z)
                {

                    bool alreadyFound = false;

                    foreach (Bounds bound in slices)
                    {
                        if (node.nodePos == bound.min || node.nodePos == bound.max) { alreadyFound = true; break; }
                    }

                    if (!alreadyFound)
                    {
                        Bounds slice = new Bounds();

                        slice.min = node.nodePos;

                        if (node.nextNode.nodePos.x == node.nodePos.x)
                        {
                            slice.max = node.nextNode.nodePos;
                        }
                        else
                        {
                            slice.max = node.prevNode.nodePos;
                        }

                        if (slice.max.z < slice.min.z)
                        {
                            Vector3 temp = slice.max;
                            slice.max = slice.min;
                            slice.min = temp;
                        }

                        slices.Add(slice);
                    }
                }
            }
        }

        return slices;
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
