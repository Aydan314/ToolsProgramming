using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using Unity.VisualScripting;
using UnityEngine.Rendering;

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
        BuildNodeData startNode = selection[0];

        Vector3 nextNodeDir = (startNode.position - startNode.next.position).normalized;
        Vector3 prevNodeDir = (startNode.position - startNode.prev.position).normalized;

        Debug.Log(nextNodeDir + " " + prevNodeDir);

        return nextNodeDir.x != prevNodeDir.z;
    }
}
