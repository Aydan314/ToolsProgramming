using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnableObjectPack", menuName = "Scriptable Objects/BuildTool/SpawnableObjectPack")]
public class SpawnableObjectPack : ScriptableObject
{
    public AssetBaseBrush brush;
    public List<SpawnableObject> spawnableObjects;
    public float gridSize = 1.0f;

    [Range(0f, 1f)]
    public float spreadDensity = 1.0f;

    public List<SpawnableObject> GetDefaultObjects()
    {
        List<SpawnableObject> defaultObjects = new List<SpawnableObject>();

        foreach (SpawnableObject obj in spawnableObjects)
        {
            if (!obj.isCornerObject) defaultObjects.Add(obj);
        }

        return defaultObjects;
    }

    public List<SpawnableObject> GetCornerObjects()
    {
        List<SpawnableObject> cornerObjects = new List<SpawnableObject>();

        foreach (SpawnableObject obj in spawnableObjects)
        {
            if (obj.isCornerObject) cornerObjects.Add(obj);
        }

        return cornerObjects;
    }
}
