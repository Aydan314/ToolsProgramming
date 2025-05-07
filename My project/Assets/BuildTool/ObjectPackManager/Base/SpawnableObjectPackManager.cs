#if UNITY_EDITOR

using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;

[CreateAssetMenu(fileName = "SpawnableObjectPackManager", menuName = "Scriptable Objects/BuildTool/SpawnableObjectPackManager")]
public class SpawnableObjectPackManager : ScriptableObject
{
    [SerializeField]
    public List<SpawnableObjectPack> assetBasePacks = new List<SpawnableObjectPack>();
    public SpawnableObjectPack selectedPack;

    private void OnEnable()
    {
        if (assetBasePacks.Count > 0) selectedPack = assetBasePacks[0];
    }

    public SpawnableObjectPack GetActiveAssetPack()
    {
        return selectedPack;
    }

    public void Build(List<BuildNodeData> footPrint)
    {
        GetActiveAssetPack().brush.Build(footPrint, GetActiveAssetPack());
    }
}

#endif
