using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;

[ExecuteInEditMode]
public class AssetPackManager : MonoBehaviour
{
    [SerializeField]
    public List<SpawnableObjectPack> assetBasePacks = new List<SpawnableObjectPack>();

    public SpawnableObjectPack GetActiveAssetPack()
    {
        return assetBasePacks[0];
    }

    public void Build(List<BuildNodeData> footPrint)
    {
        GetActiveAssetPack().brush.Build(footPrint, GetActiveAssetPack());
    }
}
