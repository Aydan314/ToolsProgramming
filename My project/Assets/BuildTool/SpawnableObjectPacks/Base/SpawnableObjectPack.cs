using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnableObjectPack", menuName = "Scriptable Objects/BuildTool/SpawnableObjectPack")]
public class SpawnableObjectPack : ScriptableObject
{
    public AssetBaseBrush brush;
    public List<SpawnableObject> spawnableObjects;
    public float gridSize;
    public float spreadDensity;
}
