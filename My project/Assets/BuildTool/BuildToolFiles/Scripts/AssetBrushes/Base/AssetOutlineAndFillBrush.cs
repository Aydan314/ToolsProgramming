#if UNITY_EDITOR

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
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

        BuildNodeData startNode = Selection[0];

        // Create copy of selection for outline as fill destroys data //
        List<BuildNodeData> SelectionCopy = new List<BuildNodeData>();

        int index = 0;
        foreach(var node in Selection)
        {
            BuildNodeData newNode = new BuildNodeData();

            newNode.position = node.position;

            if (index != 0)
            {
                newNode.prev = (SelectionCopy[index - 1]);
                SelectionCopy[index - 1].next = (newNode);
            }
            if (index == Selection.Count - 1) 
            { 
                newNode.next = (SelectionCopy[0]);
                SelectionCopy[0].prev = (newNode);
            }

            SelectionCopy.Add(newNode);

            index++;
        }

        SelectionCopy = ForceWindingOrderClockwise(SelectionCopy);
        Selection = ForceWindingOrderClockwise(Selection);

        List<Rect> footprintShape = ConvertSelectionToRects(Selection);
        int i = 0;

        if (footprintShape.Count > 0)
        {
            // Iterate through each extracted rect and fill avoiding outline //
            foreach (Rect rect in footprintShape)
            {
                for (float x = rect.x; x < (rect.x + rect.width); x += assetPack.gridSize)
                {

                    for (float y = rect.y; y < (rect.y + rect.height); y += assetPack.gridSize)
                    {
                        bool intersectsOutline = false;

                        // Avoid outline intersection //
                        foreach (var node in SelectionCopy)
                        {
                            // Matches any x outline values //
                            if (x >= node.position.x && x <= node.GetPrev().position.x && y == node.position.z) { intersectsOutline = true; break; }
                            if (x >= node.position.x && x <= node.GetNext().position.x && y == node.position.z) { intersectsOutline = true; break; }

                            // Mathches any y outline values //
                            if (y >= node.position.z && y <= node.GetPrev().position.z && x == node.position.x) { intersectsOutline = true; break; }
                            if (y >= node.position.z && y <= node.GetNext().position.z && x == node.position.x) { intersectsOutline = true; break; }

                        }
                        //Debug.Log(intersectsOutline);

                        if (!intersectsOutline)
                        {
                            SpawnableObject asset = assetPack.PickRandomFromObjects(assetPack.GetDefaultObjects());
                            if (Random.Range(0, 100) / 100.0f < assetPack.spreadDensity) PlaceAsset(asset.objectPrefab, root, new Vector3(x, startNode.position.y, y), asset.defaultRotation);
                        }
                    }

                }
                i++;
            }

            i = 0;

            BuildNodeData startNodeCopy = SelectionCopy[0];
            BuildNodeData buildNode = startNodeCopy;

            // Loop through shapes nodes and connect points up with placed assets //
            while (buildNode != null)
            {
                BuildAssetsBetween(buildNode, assetPack, root);
                buildNode = buildNode.GetNext();

                if (buildNode == startNodeCopy) break;

                if (i > 100)
                {
                    Debug.LogError("!! Max Selection Iteration Reached !!");
                    break;
                }
                i++;
            }

        }


    }

    
}

#endif