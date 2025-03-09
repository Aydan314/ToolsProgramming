using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[System.Serializable]
public struct Asset
{
    public float defaultRotation;
    public GameObject assetObject;
}

[System.Serializable]
public struct AssetBasePack
{
    public string name;
    public float gridSize;
    public List<Asset> assets;
    public AssetBaseBrush brush;
}
