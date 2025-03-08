using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
[ExecuteInEditMode]
public class LineDraw : MonoBehaviour
{
    LineRenderer lineRenderer;
    Vector3 endPos;
    Vector3 startPos;
    bool connected = false;

    [SerializeField]
    public Color drawColour;
    void Start()
    {
        OnEnable();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;

        lineRenderer = GetComponent<LineRenderer>();

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

    // Update is called once per frame
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
}
