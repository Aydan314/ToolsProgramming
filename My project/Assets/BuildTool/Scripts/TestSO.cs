using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Hello/I/Love/Scriptable/Objects")]
class TestSO : ScriptableObject
{
    public GameObject prefab;
    public Vector3 size;


    public virtual void Interact()
    {

    }
}

class TestSOTwo : TestSO
{

}