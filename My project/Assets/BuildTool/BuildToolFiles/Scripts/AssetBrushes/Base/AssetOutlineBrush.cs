#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.VersionControl;
using UnityEngine;

[CreateAssetMenu(fileName = "Outline Brush", menuName = "Scriptable Objects/BuildTool/Brushes/Outline Brush")]

public class AssetOutlineBrush : AssetBaseBrush
{
    public override void Build(List<BuildNodeData> Selection, SpawnableObjectPack assetPack)
    {
        GameObject root = new GameObject();
        root.name = assetPack.name + " Brush";
        root.transform.position = Selection[0].position;

        BuildNodeData buildNode = Selection[0];
        Selection = ForceWindingOrderClockwise(Selection);
        
        int i = 0;

        // Loop through shapes nodes and connect points up with placed assets //
        while (buildNode != null)
        {
            BuildAssetsBetween(buildNode, assetPack, root);
            buildNode = buildNode.GetNext();

            if (buildNode == Selection[0]) break;

            if (i > 100)
            {
                Debug.LogError("!! Max Selection Iteration Reached !!");
                break;
            }
            i++;
        }
    }
}
#endif