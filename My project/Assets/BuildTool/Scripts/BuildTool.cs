using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;

[ExecuteInEditMode]
public class BuildTool : MonoBehaviour
{
    [SerializeField]
    GameObject node;
    [SerializeField]
    float gridSize = 1;

    private Vector3 gridStart;
    GameObject prevNode = null;
    AssetPackManager assetPackManager;

    bool creatingGrid = false;
    bool toolActive = false;
    List<GameObject> footPrint;
    float inputCooldown = 0.0f;
    float clickCooldown = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnEnable();
    }

    // Add tools update function to be called //
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        assetPackManager = GetComponent<AssetPackManager>();

        if (assetPackManager == null)
        {
            Debug.LogError("Asset Pack Manager could not be found!");
        }

        Debug.Log("- Press B to toggle build tool");
       
    }

    // Remove tools update function from call //
    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    // Remove all nodes used for selection in world //

    private void DestroySelectionNodes()
    {
        foreach (var marker in footPrint)
        {
            DestroyImmediate(marker);
        }
    }
    private void SelectionComplete()
    {
        assetPackManager.Build(footPrint);
        DestroySelectionNodes();
        creatingGrid = false;
    }

    // Remove previous node in grid selection //
    private void RemovePreviousNode()
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

    // Create a new grid selection //
    private void BeginGridCreation(Vector3 point)
    {
        creatingGrid = true;
        footPrint = new List<GameObject>();

        GameObject newObject = Instantiate(node);

        gridStart = point;
        newObject.transform.position = gridStart;
        newObject.GetComponent<BuildNode>().SetAsStartNode();
        newObject.transform.parent = this.gameObject.transform;
        prevNode = newObject;

        footPrint.Add(newObject);
    }

    // Add a point aligned to the first point in the selection grid //
    private void AddPointToGrid(Vector3 gridPos)
    {
        if (gridPos != footPrint[0].transform.position)
        {
            GameObject newObject = Instantiate(node);

            newObject.transform.position = gridPos;
            newObject.GetComponent<BuildNode>().ConnectTo(prevNode.transform.position);
            newObject.transform.parent = this.gameObject.transform;

            prevNode.GetComponent<BuildNode>().prevNodePos = gridPos;

            prevNode = newObject;

            footPrint.Add(newObject);
        }
        // User has finished area selection as all nodes connect //
        else
        {
            prevNode.GetComponent<BuildNode>().ConnectTo(footPrint[0].transform.position);
            footPrint[0].GetComponent<BuildNode>().prevNodePos = prevNode.transform.position;
            SelectionComplete();
        }
    }

    // Loops through each node exept the start node and applys a colour //
    private void ApplyColourToGrid(Color color)
    {
        for (int i = 1; i < footPrint.Count; i++)
        {
            footPrint[i].GetComponent<BuildNode>().SetColour(color);
        }
    }

    // Ensures a given point is within a cell of the grid //
    private Vector3 SnapPointToGrid(Vector3 point)
    {
        Vector3 gridPos = new Vector3();

        gridPos.x = gridSize * Mathf.Round((point.x - gridSize / 2.0f) / gridSize) + gridStart.x % gridSize;
        gridPos.y = gridSize * Mathf.Round((point.y - gridSize / 2.0f) / gridSize) + gridStart.y % gridSize;
        gridPos.z = gridSize * Mathf.Round((point.z - gridSize / 2.0f) / gridSize) + gridStart.z % gridSize;

        return gridPos;
    }

    // Ensures a point shares atleast 1 axis with the previous point, making selection not diagonal //
    private Vector3 SnapPointToAxis(Vector3 point, Vector3 prevPos)
    {
        if (point.x != prevPos.x && point.z != prevPos.z)
        {
            if (Mathf.Abs(point.x - prevPos.x) > Mathf.Abs(point.z - prevPos.z))
            {
                point.z = prevPos.z;
            }
            else
            {
                point.x = prevPos.x;
            }
        }

        point.y = prevPos.y;

        return point;
    }

    // Update function for the Build Tool //
    private void OnSceneGUI(SceneView sceneView)
    {
        // Ensures the user can't input too quickly //
        if (inputCooldown > 0.0f)
        {
            inputCooldown -= Time.deltaTime;
        }
        else if (inputCooldown != 0.0f) inputCooldown = 0.0f;

        // Stops the user placing nodes too quickly //
        if (clickCooldown > 0.0f)
        {
            clickCooldown -= Time.deltaTime;
        }
        else if (clickCooldown != 0.0f) clickCooldown = 0.0f;


        Event e = Event.current;
        Vector3 gridPos = new Vector3();

        if (creatingGrid)
        {
            if (prevNode == null)
            {
                creatingGrid = false;
                DestroySelectionNodes();
            }
            if (toolActive)
            { 
                // Creates a line in the direction the user is going to place a node //

                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                RaycastHit hit;


                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 prevPos = prevNode.transform.position;

                    gridPos = SnapPointToGrid(hit.point);
                    gridPos = SnapPointToAxis(gridPos, prevPos);
                    
                    BuildNode node = footPrint[0].GetComponent<BuildNode>();

                    if (footPrint.Count > 1)
                    {
                        // Detects if full loop of selection will be made upon placing node //
                        if (gridPos == footPrint[0].transform.position)
                        {
                            node.SetColour(node.fullSelectionColour);
                            ApplyColourToGrid(node.fullSelectionColour);

                        }
                        // Resets node colour //
                        else if (node.GetColour() != node.startNodeColour)
                        {
                            node.SetColour(node.startNodeColour);
                            ApplyColourToGrid(node.drawColour);
                        }
                    }
                }

                prevNode.GetComponent<BuildNode>().ConnectTo(gridPos);


                if (e.type == EventType.KeyDown)
                {
                    // User can press enter to create selection even if its not full, e.g for a single wall //
                    if (e.keyCode == KeyCode.Return)
                    {
                        prevNode.GetComponent<BuildNode>().ConnectTo(prevNode.transform.position);
                        SelectionComplete();
                    }
                    // Deletes the previous node //
                    if (e.keyCode == KeyCode.Escape && inputCooldown == 0.0f)
                    {
                        RemovePreviousNode();
                        inputCooldown = 0.5f;
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
                    // Places a node into the scene //
                    if (e.keyCode == KeyCode.Mouse0 && clickCooldown == 0.0f)
                    {
                        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit))
                        {
                            if (!creatingGrid)
                            {
                                BeginGridCreation(hit.point);
                            }
                            else
                            {
                                AddPointToGrid(gridPos);
                            }

                        }
                        clickCooldown = 0.5f;

                    }
                }
            }
            // Toggles the tool on and off //
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.B && inputCooldown == 0.0f)
                {
                    toolActive = !toolActive;

                    if (toolActive) Debug.Log("Build Tool Enabled");
                    else Debug.Log("Build Tool Disabled");

                    inputCooldown = 0.5f;
                }
            }
            
        }
      }
    
}

   