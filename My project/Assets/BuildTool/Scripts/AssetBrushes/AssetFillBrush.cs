using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.VersionControl;
using UnityEngine;
using static UnityEditor.Rendering.InspectorCurveEditor;

public class AssetFillBrush : AssetBaseBrush
{
    public override void Build(List<BuildNodeData> Selection, SpawnableObjectPack assetPack)
    {
        GameObject root = new GameObject();
        root.name = assetPack.name + " Brush";
        root.transform.position = Selection[0].position;

        SpawnableObject asset = assetPack.spawnableObjects[0];
        BuildNodeData startNode = Selection[0];

        Selection = ForceWindingOrderClockwise(Selection);

        List<Rect> footprintShape = ConvertSelectionToRects(Selection);

        int i = 0;
        // Iterate through each extracted rect and fill //
        foreach (Rect rect in footprintShape)
        {
            
            for (float x = rect.x; x < (rect.x + rect.width); x += assetPack.gridSize)
            {
                for (float y = rect.y; y < (rect.y + rect.height); y += assetPack.gridSize)
                {
                    PlaceAsset(asset.objectPrefab, root, new Vector3(x, startNode.position.y, y), asset.defaultRotation);
                }
            }
            i++;
        }

    }

    
}
