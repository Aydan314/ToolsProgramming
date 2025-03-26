using System.Collections.Generic;
using UnityEngine;

public class AssetSpreadBrush : AssetBaseBrush
{
    public override void Build(List<BuildNodeData> Selection, AssetBasePack assetPack)
    {
        GameObject root = new GameObject();
        root.name = assetPack.name + " Brush";
        root.transform.position = Selection[0].position;

        Asset asset = assetPack.assets[0];
        BuildNodeData startNode = Selection[0];

        Selection = ForceWindingOrderClockwise(Selection);

        List<Rect> footprintShape = ConvertSelectionToRects(Selection);

        int i = 0;
        foreach (Rect rect in footprintShape)
        {
            
            for (float x = rect.x; x < (rect.x + rect.width); x += assetPack.gridSize)
            {
                for (float y = rect.y; y < (rect.y + rect.height); y += assetPack.gridSize)
                {
                    if ((Random.Range(0, 100) / 100.0f) < assetPack.spreadDensity)
                    {
                        PlaceAsset(asset.assetObject, root, new Vector3(x, startNode.position.y, y), asset.defaultRotation);
                    }
                }
            }
            i++;
        }

    }
}
