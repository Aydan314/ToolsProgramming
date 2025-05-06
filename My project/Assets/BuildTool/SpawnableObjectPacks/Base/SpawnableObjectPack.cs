using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnableObjectPack", menuName = "Scriptable Objects/BuildTool/SpawnableObjectPack")]
[Serializable]
public class SpawnableObjectPack : ScriptableObject
{
    public AssetBaseBrush brush;
    public List<SpawnableObject> spawnableObjects;
    public float gridSize = 1.0f;
    public float spreadDensity = 1.0f;

    public bool CheckErrors()
    {
        // Check that the data in the pack is all valid //
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
        // Lists all objects that arent corners or outline //
        List<SpawnableObject> defaultObjects = new List<SpawnableObject>();

        foreach (SpawnableObject obj in spawnableObjects)
        {
            if (!obj.isOutsetCornerObject && !obj.isInsetCornerObject && !obj.isOutlineObject) defaultObjects.Add(obj);
        }

        return defaultObjects;
    }

    public List<SpawnableObject> GetOutlineObjects()
    {
        // Lists all objects that are outline and not corners //
        List<SpawnableObject> outlineObjects = new List<SpawnableObject>();

        foreach (SpawnableObject obj in spawnableObjects)
        {
            if (obj.isOutlineObject && !obj.isOutsetCornerObject && !obj.isInsetCornerObject) outlineObjects.Add(obj);
        }

        return outlineObjects;
    }

    public List<SpawnableObject> GetOutsetCornerObjects()
    {
        // Lists all objects that are corners //
        List<SpawnableObject> cornerObjects = new List<SpawnableObject>();

        foreach (SpawnableObject obj in spawnableObjects)
        {
            if (obj.isOutsetCornerObject) cornerObjects.Add(obj);
        }

        return cornerObjects;
    }

    public List<SpawnableObject> GetInsetCornerObjects()
    {
        // Lists all objects that are corners //
        List<SpawnableObject> cornerObjects = new List<SpawnableObject>();

        foreach (SpawnableObject obj in spawnableObjects)
        {
            if (obj.isInsetCornerObject) cornerObjects.Add(obj);
        }

        return cornerObjects;
    }

    public SpawnableObject PickRandomFromObjects(List<SpawnableObject> objects)
    {
        return objects[UnityEngine.Random.Range(0, objects.Count)];
    }
}
