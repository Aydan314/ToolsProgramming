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
    public List<AssetBasePack> assetBasePacks = new List<AssetBasePack>();

    private void Start()
    {
        
    }

    public AssetBasePack GetActiveAssetPack()
    {
        return assetBasePacks[0];
    }

    public void Build(List<GameObject> footPrint)
    {
        GetActiveAssetPack().brush.Build(footPrint, GetActiveAssetPack());
    }
}
