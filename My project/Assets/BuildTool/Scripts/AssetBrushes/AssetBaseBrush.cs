using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.VersionControl;

[System.Serializable]
public class AssetBaseBrush : MonoBehaviour
{
    public virtual void Build(List<GameObject> Selection, AssetBasePack assetPack)
    {
        Debug.Log("Base Brush Build Called Using " + assetPack.name);
    }
}
