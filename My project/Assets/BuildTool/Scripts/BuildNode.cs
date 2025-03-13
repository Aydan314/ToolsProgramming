using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
[ExecuteInEditMode]
public class BuildNode : MonoBehaviour
{
    LineRenderer lineRenderer;
    Renderer renderer;
    bool connected = false;
    public BuildNode prevNode;
    public BuildNode nextNode;
    public Vector3 selectionDrawPos;
    public Vector3 nodePos;

    [SerializeField]
    public Color drawColour;
    [SerializeField]
    public Color startNodeColour;
    [SerializeField]
    public Color fullSelectionColour;
    public bool isStartNode = false;
    public bool windingOrderAntiClockwise = true;
    
    void Start()
    {
        OnEnable();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;

        lineRenderer = GetComponent<LineRenderer>();
        renderer = GetComponent<Renderer>();

        SetColour(drawColour);

        if (!lineRenderer)
        {
            Debug.LogError("!! No Line Renderer Attached !!");
        }

        lineRenderer.startColor = drawColour;
        lineRenderer.endColor = drawColour;

        nodePos = gameObject.transform.position;
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
            lineRenderer.SetPosition(0, nodePos);
            lineRenderer.SetPosition(1, nextNode.nodePos);
        }
        else if (this != null)
        {
            lineRenderer.SetPosition(0, nodePos);
            lineRenderer.SetPosition(1, selectionDrawPos);
        }
    }
    public void ConnectTo(BuildNode node)
    {
        prevNode = node;
        node.nextNode = this;
        node.connected = true;
    }

    public void ConnectToStart(BuildNode startNode)
    {
        nextNode = startNode;
        startNode.prevNode = this;
        connected = true;
    }

    public void Disconnect()
    {
        connected = false;
        nextNode = null;
    }
    public void SetAsStartNode()
    {
        SetColour(startNodeColour);
        isStartNode = true;
    }
    public void SetColour(Color colour)
    {
        renderer.material.color = colour;
    }
    public Color GetColour()
    {
        return renderer.material.color;
    }

    public BuildNode GetPrevNode()
    {
        if (!windingOrderAntiClockwise) return nextNode;
        return prevNode;
    }

    public BuildNode GetNextNode()
    {
        if (!windingOrderAntiClockwise) return prevNode;
        return nextNode;
    }
}
