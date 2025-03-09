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

    [SerializeField]
    public Color drawColour;
    [SerializeField]
    public Color startNodeColour;
    [SerializeField]
    public Color fullSelectionColour;
    public bool isStartNode = false;
    public Vector3 endPos;
    public Vector3 startPos;
    void Start()
    {
        OnEnable();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;

        lineRenderer = GetComponent<LineRenderer>();
        renderer = GetComponent<Renderer>();

        if (!lineRenderer)
        {
            Debug.LogError("!! No Line Renderer Attached !!");
        }

        lineRenderer.startColor = drawColour;
        lineRenderer.endColor = drawColour;

        startPos = transform.position;
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
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
        }
    }
    public void ConnectTo(Vector3 pos)
    {
        endPos = pos;
        connected = true;
    }

    public void Disconnect()
    {
        endPos = new Vector3();
        connected = false;
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
}
