using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using System.Runtime.InteropServices.WindowsRuntime;

[System.Serializable]
public class AssetBaseBrush : MonoBehaviour
{
    public virtual void Build(List<BuildNodeData> Selection, AssetBasePack assetPack)
    {
        Debug.Log("Base Brush Build Called Using " + assetPack.name);
    }

    public void PlaceAsset(GameObject asset, GameObject rootObject, Vector3 position, float angle)
    {
        GameObject newObject = Instantiate(asset);
        newObject.transform.position = position;
        newObject.transform.rotation = Quaternion.Euler(0, angle, 0);
        newObject.transform.parent = rootObject.transform;
    }
    public bool DetectWindingOrderClockwise(List<BuildNodeData> selection)
    {
        List<Vector3> clockwiseDir = new List<Vector3>() { new Vector3(1, 0, 0), new Vector3(0, 0, -1), new Vector3(-1, 0, 0), new Vector3(0, 0, 1) };
        List<Vector3> antiClockwiseDir = new List<Vector3>() { new Vector3(-1, 0, 0), new Vector3(0, 0, -1), new Vector3(1, 0, 0), new Vector3(0, 0, 1) };
        Vector3 startDir = GetNextNodeDir(selection[0]);

        int clockwiseStartPos = -1;
        for (int i = 0; i < clockwiseDir.Count; i++)
        {
            if (startDir == clockwiseDir[i])
            {
                clockwiseStartPos = i;
                break;
            }
        }

        int antiClockwiseStartPos = -1;
        for (int i = 0; i < antiClockwiseDir.Count; i++)
        {
            if (startDir == antiClockwiseDir[i])
            {
                antiClockwiseStartPos = i;
                break;
            }
        }

        int clockwiseCount = 0;
        int antiClockwiseCount = 0;

        foreach (BuildNodeData node in selection)
        {
            Vector3 nextNodeDir = GetNextNodeDir(node);
            if (nextNodeDir == clockwiseDir[(clockwiseStartPos + clockwiseCount) % clockwiseDir.Count]) clockwiseCount++;
            if (nextNodeDir == antiClockwiseDir[(antiClockwiseStartPos + antiClockwiseCount) % antiClockwiseDir.Count]) antiClockwiseCount++;
        }
        Debug.Log(clockwiseCount);

        return clockwiseCount > antiClockwiseCount;
    }

    public Vector3 GetNextNodeDir(BuildNodeData node)
    {
        return (node.position - node.GetNext().position).normalized;
    }
}
