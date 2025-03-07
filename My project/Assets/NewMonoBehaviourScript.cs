using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
[ExecuteInEditMode]
public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField]
    GameObject gameObject;
    [SerializeField]
    int width = 4;
    [SerializeField]
    int depth = 4;
    [SerializeField]
    int stories = 1;

    private int currentFloor;
    private Vector3 buildingPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnEnable();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    // Update is called once per frame
    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        if (e != null)
        {
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.E)
                {
                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    RaycastHit hit;


                    if (Physics.Raycast(ray, out hit))
                    {
                      
                        buildingPos = hit.point;

                        Debug.Log("Generating Building at" + buildingPos + "...");

                        for (int i = 0; i < stories; i++)
                        {
                            GenerateFloor();
                        }

                        currentFloor = 0;
                    }

                    
                }
            }
        }
    }

    private void GenerateFloor()
    {
        List<List<int>> floorPlan = new List<List<int>>();

        for (int x = 0; x < width; x++)
        {
            List<int> line = new List<int>();

            for (int y = 0; y < depth; y++)
            {
                line.Add(1);
            }
            floorPlan.Add(line);
        }

        int posX = 0;
        int posY = 0;
        foreach (List<int> line in floorPlan)
        {
            foreach (int i in line)
            {
                GameObject newObject = Instantiate(gameObject);
                newObject.transform.position = new Vector3(buildingPos.x + posX, buildingPos.y + currentFloor, buildingPos.z + posY);
                posX++;
            }
            posY++;
            posX = 0;
        }

        currentFloor++;
    }
}
 