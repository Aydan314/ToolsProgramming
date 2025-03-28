
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[System.Serializable]
public struct SpawnableData
{
    public float defaultRotation;
    public GameObject assetObject;
}

[System.Serializable]
public struct AssetBasePack
{
    public string name;
    [Tooltip("Size in Metres of the grid the assets in the pack should snap to")]
    public float gridSize;
    [Header("Pack Assets:")]
    public List<SpawnableData> assets;
    public List<SpawnableData> cornerAssets;
    [Header("Pack Brush:")]
    public AssetBaseBrush brush;
    [Header("Spread Density")]
    [Range(0, 1)]
    public float spreadDensity;
}
