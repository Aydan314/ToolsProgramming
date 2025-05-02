using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BuildNodeData
{
    public BuildNodeData prev;
    public Vector3 position;
    public BuildNodeData next;
    public bool windingOrderClockwise = false;

    public BuildNodeData GetPrev()
    {
        // return previous node depending on order //
        if (windingOrderClockwise) return prev;
        else return next;
    } 

    public BuildNodeData GetNext()
    {
        // return next node depending on order //
        if (windingOrderClockwise) return next;
        else return prev;
    }

    public void SetNext(BuildNodeData nextNode)
    {
        // Set next node depending on order //
        if (windingOrderClockwise) next = nextNode;
        else prev = nextNode;
    }

    public void SetPrev(BuildNodeData prevNode)
    {
        // Set previous node depending on order //
        if (windingOrderClockwise) prev = prevNode;
        else next = prevNode;
    }
}

[ExecuteInEditMode]
public class BuildNode : MonoBehaviour
{
    LineRenderer lineRenderer;
    Renderer nodeRenderer;
    bool connected = false;

    public BuildNodeData nodeData;
    public Vector3 selectionDrawPos;

    public bool isStartNode = false;
    

    [SerializeField]
    public Material drawColour;
    [SerializeField]
    public Material startNodeColour;
    [SerializeField]
    public Material fullSelectionColour;
    
    
    void Start()
    {
        OnEnable();
    }

    private void OnEnable()
    {
        // Init values //

        SceneView.duringSceneGui += OnSceneGUI;

        lineRenderer = GetComponent<LineRenderer>();
        nodeRenderer = GetComponent<Renderer>();

        SetMaterial(drawColour);

        if (!lineRenderer)
        {
            Debug.LogError("!! No Line Renderer Attached !!");
        }

        lineRenderer.material = drawColour;

        
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        connected = false;
    }
    private void OnSceneGUI(SceneView sceneView)
    {
        // Draw connection lines between nodes //
        if (connected)
        {
            lineRenderer.SetPosition(0, GetNodeData().position);
            lineRenderer.SetPosition(1, nodeData.next.position);
        }
        else if (this != null)
        {
            lineRenderer.SetPosition(0, GetNodeData().position);
            lineRenderer.SetPosition(1, selectionDrawPos);
        }
    }
    public void ConnectTo(BuildNode node)
    {
        // Update connection values //
        node.nodeData.next = GetNodeData();
        nodeData.prev = node.GetNodeData();
        node.connected = true;

        nodeData.position = gameObject.transform.position;
    }

    public void ConnectToStart(BuildNode startNode)
    {
        // Update connection values to start //
        startNode.nodeData.prev = GetNodeData();
        nodeData.next = startNode.GetNodeData();
        
        connected = true;

        nodeData.position = gameObject.transform.position;
    }

    public void Disconnect()
    {
        connected = false;
        nodeData.next = null;
    }
    public void SetAsStartNode()
    {
        SetMaterial(startNodeColour);
        isStartNode = true;
    }
    public void SetMaterial(Material colour)
    {
        nodeRenderer.sharedMaterial = colour;
    }
    public Material GetMaterial()
    {
        return nodeRenderer.sharedMaterial;
    }

    public BuildNodeData GetNodeData()
    {
        // Return build node's data unless it has none, then create data //
        if (nodeData == null)
        {
            nodeData = new BuildNodeData();

            nodeData.position = gameObject.transform.position;
            nodeData.next = null;
            nodeData.prev = null;
        }

        return nodeData;
    }
}
