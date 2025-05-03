using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.VersionControl;
using UnityEngine;

[CreateAssetMenu(fileName = "Outline And Fill Brush", menuName = "Scriptable Objects/BuildTool/Brushes/Outline And Fill Brush")]
public class AssetOutlineAndFillBrush : AssetBaseBrush
{
    public override void Build(List<BuildNodeData> Selection, SpawnableObjectPack assetPack)
    {
        GameObject root = new GameObject();
        root.name = assetPack.name + " Brush";
        root.transform.position = Selection[0].position;

        SpawnableObject asset = assetPack.GetDefaultObjects()[0];
        BuildNodeData startNode = Selection[0];

        Selection = ForceWindingOrderClockwise(Selection);
        int i = 0;


        BuildNodeData buildNode = startNode;

        // Loop through shapes nodes and connect points up with placed assets //
        while (buildNode != null)
        {
            BuildAssetsBetween(buildNode, assetPack, root);
            buildNode = buildNode.GetNext();

            if (buildNode == startNode) break;

            if (i > 100)
            {
                Debug.LogError("!! Max Selection Iteration Reached !!");
                break;
            }
            i++;
        }


        List<Rect> footprintShape = ConvertSelectionToRects(Selection);
        i = 0;

        // Iterate through each extracted rect and fill //
        foreach (Rect rect in footprintShape)
        {

            for (float x = rect.x; x < (rect.x + rect.width); x += assetPack.gridSize)
            {
                for (float y = rect.y; y < (rect.y + rect.height); y += assetPack.gridSize)
                {
                    if (Random.Range(0, 100) / 100.0f < assetPack.spreadDensity) PlaceAsset(asset.objectPrefab, root, new Vector3(x, startNode.position.y, y), asset.defaultRotation);
                }
            }
            i++;
        }



    }

    
}
