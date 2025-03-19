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
    public bool windingOrderAntiClockwise = true;

    public BuildNodeData GetPrev()
    {
        if (windingOrderAntiClockwise) return prev;
        else return next;
    } 

    public BuildNodeData GetNext()
    {
        if (windingOrderAntiClockwise) return next;
        else return prev;
    }

}

[ExecuteInEditMode]
public class BuildNode : MonoBehaviour
{
    LineRenderer lineRenderer;
    Renderer renderer;
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
        SceneView.duringSceneGui += OnSceneGUI;

        lineRenderer = GetComponent<LineRenderer>();
        renderer = GetComponent<Renderer>();

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
        
        node.nodeData.next = GetNodeData();
        nodeData.prev = node.GetNodeData();
        node.connected = true;

        nodeData.position = gameObject.transform.position;
    }

    public void ConnectToStart(BuildNode startNode)
    {
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
        renderer.sharedMaterial = colour;
    }
    public Material GetMaterial()
    {
        return renderer.sharedMaterial;
    }

    public BuildNodeData GetNodeData()
    {
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
