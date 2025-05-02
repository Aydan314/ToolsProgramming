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

        Debug.Log("outline and fill");

    }

    
}
