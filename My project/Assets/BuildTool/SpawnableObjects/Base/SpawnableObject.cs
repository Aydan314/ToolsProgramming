#if UNITY_EDITOR

using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnableObject", menuName = "Scriptable Objects/BuildTool/SpawnableObject")]
public class SpawnableObject : ScriptableObject
{
    public GameObject objectPrefab;
    public float defaultRotation;
    public bool isOutsetCornerObject;
    public bool isInsetCornerObject;
    public bool isOutlineObject;
}

#endif
