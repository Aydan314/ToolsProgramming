using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnableObjectPack", menuName = "Scriptable Objects/BuildTool/SpawnableObjectPack")]
public class SpawnableObjectPack : ScriptableObject
{
    public AssetBaseBrush brush;
    public List<SpawnableObject> spawnableObjects;
    public float gridSize = 1.0f;
    public float spreadDensity = 1.0f;

    public bool CheckErrors()
    {
        if (gridSize <= 0f)
        {
            Debug.LogError("!! Grid Size cannot be 0 or less !!");
            return true;
        }
        else if (spawnableObjects == null)
        {
            Debug.LogError("!! Spawnable objects have not been set !!");
            return true;
        }
        else if (spawnableObjects.Count < 1)
        {
            Debug.LogError("!! No available objects to spawn !!");
            return true;
        }
        else if (brush == null)
        {
            Debug.LogError("!! Brush has not been selected !!");
            return true;
        }

        foreach (SpawnableObject obj in spawnableObjects)
        {
            if (obj.objectPrefab == null)
            {
                Debug.LogError("!! Object prefab is null !!");
                return true;
            }
        }

        return false;
    }

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
