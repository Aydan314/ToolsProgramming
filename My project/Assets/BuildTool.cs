using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

[ExecuteInEditMode]
public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField]
    GameObject node;
    [SerializeField]
    float gridSize = 1;

    private Vector3 gridStart;
    GameObject prevNode = null;
    bool creatingGrid = false;
    bool toolActive = false;
    List<GameObject> footPrint;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnEnable();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        Debug.Log("- Press B to toggle build tool");
       
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    // Update is called once per frame
    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        Vector3 gridPos = new Vector3();

        if (creatingGrid)
        {
            if (toolActive)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                RaycastHit hit;


                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 prevPos = prevNode.transform.position;
                    gridPos.x = gridSize * Mathf.Round((hit.point.x - gridSize / 2.0f) / gridSize) + gridStart.x % gridSize;
                    gridPos.y = gridSize * Mathf.Round((hit.point.y - gridSize / 2.0f) / gridSize) + gridStart.y % gridSize;
                    gridPos.z = gridSize * Mathf.Round((hit.point.z - gridSize / 2.0f) / gridSize) + gridStart.z % gridSize;

                    if (gridPos.x != prevPos.x && gridPos.z != prevPos.z)
                    {
                        if (Mathf.Abs(gridPos.x - prevPos.x) > Mathf.Abs(gridPos.z - prevPos.z))
                        {
                            gridPos.z = prevPos.z;
                        }
                        else
                        {
                            gridPos.x = prevPos.x;
                        }
                    }



                }

                prevNode.GetComponent<LineDraw>().ConnectTo(gridPos);


                if (e.type == EventType.KeyDown)
                {
                    if (e.keyCode == KeyCode.Return)
                    {
                        creatingGrid = false;
                        foreach (var marker in footPrint)
                        {
                            DestroyImmediate(marker);
                        }
                    }
                    if (e.keyCode == KeyCode.Escape)
                    {
                        GameObject deleted = footPrint[footPrint.Count - 1];
                        footPrint.Remove(deleted);
                        DestroyImmediate(deleted);
                        if (footPrint.Count == 0)
                        {
                            creatingGrid = false;
                        }
                        else
                        {
                            prevNode = footPrint[footPrint.Count - 1];
                        }

                    }
                }
            }
        }

        if (e != null)
        {
            if (toolActive)
            {
                if (e.type == EventType.MouseDown)
                {
                    if (e.keyCode == KeyCode.Mouse0)
                    {
                        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit))
                        {
                            if (!creatingGrid)
                            {
                                gridStart = hit.point;
                                creatingGrid = true;
                                footPrint = new List<GameObject>();
                                GameObject newObject = Instantiate(node);

                                newObject.transform.position = gridStart;
                                prevNode = newObject;
                                footPrint.Add(newObject);
                            }
                            else
                            {
                                GameObject newObject = Instantiate(node);

                                newObject.transform.position = gridPos;
                                newObject.GetComponent<LineDraw>().ConnectTo(prevNode.transform.position);
                                prevNode = newObject;

                                footPrint.Add(newObject);
                            }


                        }


                    }
                }
            }
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.B)
                {
                    toolActive = !toolActive;

                    if (toolActive) Debug.Log("Build Tool Enabled");
                    else Debug.Log("Build Tool Disabled");
                }
            }
            
        }
      }
    
}

   