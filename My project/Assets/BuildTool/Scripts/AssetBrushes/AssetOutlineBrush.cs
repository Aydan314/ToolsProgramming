using System.Collections.Generic;
using UnityEngine;

public class AssetOutlineBrush : AssetBaseBrush
{
    public override void Build(List<GameObject> Selection, AssetBasePack assetPack)
    {
        Debug.Log("Outline Called using " + assetPack.name);

        for (int i = 1; i < Selection.Count; i++)
        {
            GameObject marker = Selection[i];
            BuildNode buildNode = marker.GetComponent<BuildNode>();
            BuildAssetsBetween(buildNode.startPos, buildNode.endPos, assetPack);
        }
    }
}
