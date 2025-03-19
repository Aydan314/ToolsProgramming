using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.VersionControl;
using UnityEngine;
using static UnityEditor.Rendering.InspectorCurveEditor;

public struct Bounds
{
    public Vector3 max;
    public Vector3 min;
}

public class AssetSpreadBrush : AssetBaseBrush
{
    public override void Build(List<BuildNodeData> Selection, AssetBasePack assetPack)
    {
        GameObject root = new GameObject();
        root.name = assetPack.name + " Brush";

        Asset asset = assetPack.assets[0];
        BuildNodeData startNode = Selection[0];

        if (DetectWindingOrderClockwise(Selection))
        {
            Debug.Log("Clockwise");
            foreach (BuildNodeData node in Selection)
            {
                node.windingOrderAntiClockwise = false;
            }
        }
        else
        {
            Debug.Log("AntiClockwise");
        }

        List<Rect> footprintShape = ConvertSelectionToRects(Selection);

        int i = 0;
        foreach (Rect rect in footprintShape)
        {
            Debug.Log(rect);


            for (float x = rect.x; x < (rect.x + rect.width); x += assetPack.gridSize)
            {
                for (float y = rect.y; y < (rect.y + rect.height); y += assetPack.gridSize)
                {
                    PlaceAsset(asset.assetObject, root, new Vector3(x, 0, y), asset.defaultRotation);
                }
            }
            i++;
        }

    }

    public bool CompletesCycle(List<BuildNodeData> nodes)
    {
        foreach (BuildNodeData node in nodes)
        {
            Vector3 nextNodeDir = GetNextNodeDir(node);

            foreach(BuildNodeData checkNode in nodes)
            {
                if (checkNode != node)
                {
                    Debug.Log(nextNodeDir + " " + GetNextNodeDir(checkNode));
                    if (nextNodeDir == GetNextNodeDir(checkNode)) return false;
                }
            }
        }

        return true;
    }

    public bool PointLiesOnAnyLine(List<BuildNodeData> nodes, Vector3 point)
    {
        foreach (BuildNodeData node in nodes)
        {
            if (node.next != null)
            {
                Vector3 nodePos = node.position;
                Vector3 nextNodePos = node.next.position;

                Debug.Log("checking if position " + point + " is between" + nodePos + " and " + nextNodePos);

                if (point.x == nodePos.x && point.x == nextNodePos.x)
                {
                    if (point.z <= Mathf.Max(nodePos.z, nextNodePos.z) && point.z >= Mathf.Min(nodePos.z, nextNodePos.z)) return true;
                }
                if (point.z == nodePos.z && point.z == nextNodePos.z)
                {
                    if (point.x <= Mathf.Max(nodePos.x, nextNodePos.x) && point.x >= Mathf.Min(nodePos.x, nextNodePos.x)) return true;
                }

            }
        }
        return false;
    }

    public Vector3 GetNextNodeDir(BuildNodeData node)
    {
        return (node.position - node.next.position).normalized;
    }

    public Vector3 GenerateNewPoint(List<BuildNodeData> nodes)
    {
        Vector3 A = nodes[1].position;
        Vector3 B = nodes[2].position;
        Vector3 C = nodes[0].position;


        Debug.Log("creating point from " + A + " " + B + " " + C + " == " + (A + (B - A) + (C - A)));

        return A + (B - A) + (C - A);
    }

    public Rect GenerateRectFromPoints(List<Vector3> points)
    {
        Rect rect = new Rect();

        float xMin = float.MaxValue;
        float xMax = float.MinValue;
        
        float yMin = float.MaxValue;
        float yMax = float.MinValue;
        

        foreach (var point in points)
        {
            xMax = Mathf.Max(xMax, point.x);
            xMin = Mathf.Min(xMin, point.x);

            yMax = Mathf.Max(yMax, point.z);
            yMin = Mathf.Min(yMin, point.z);
        }

        Debug.Log("Min max :" + xMin + " " + yMin + "        " + xMax + " " + yMax);

        rect.xMin = xMin;
        rect.xMax = xMax;

        rect.yMin = yMin;
        rect.yMax = yMax;

        return rect;
    }

    public void DeleteNodesInvolved(List<BuildNodeData> nodes, List<BuildNodeData> selection)
    {
        foreach (var node in nodes)
        {
            selection.Remove(node);
        }
    }
    public List<Rect> ConvertSelectionToRects(List<BuildNodeData> Selection)
    {
        List<Rect> footprintShape = new List<Rect>();

        List<BuildNodeData> rectPointsList = new List<BuildNodeData>() { Selection[0] };

        BuildNodeData current = Selection[0];
        int i = 0;

        while (current != null && Selection.Count > 0) 
        {
            Debug.Log(i + " current: " + current.position);

            if (i >= 100)
            {
                Debug.LogError("!! Shape Selection is Too Complex !!");
                return new List<Rect>();
            }
            i++;

            if (rectPointsList.Count == 3)
            {
                Debug.Log(i + "Attempting to create rectangle");
                if (CompletesCycle(rectPointsList))
                {
                    Debug.Log(i + " cycle completes!");
                    Vector3 newPoint = GenerateNewPoint(rectPointsList);

                    if (PointLiesOnAnyLine(Selection,newPoint))
                    {
                        Debug.Log(i + " point lies on line");
                        Rect rect = GenerateRectFromPoints(new List<Vector3>()
                            {
                                rectPointsList[0].position,
                                rectPointsList[1].position,
                                rectPointsList[2].position,
                                newPoint
                            }
                        );

                        footprintShape.Add(rect);

                        Debug.Log("created new rect " + rect);

                        DeleteNodesInvolved(rectPointsList, Selection);

                        BuildNodeData newNode = new BuildNodeData();

                        newNode.position = newPoint;
                        newNode.prev = rectPointsList[0].prev;
                        newNode.next = rectPointsList[2].next;

                        Debug.Log(newNode.position + "<<<<<<<<<<<");

                        BuildNodeData result = Selection.Find(x => x.position == newPoint);
                        if (result != null)
                        {
                            rectPointsList[0].prev.next = result.next;
                            rectPointsList[2].next.prev = result.next;

                            rectPointsList.Clear();
                            rectPointsList.Add(result.next);

                            Selection.Remove(result);

                            current = result;
                        }
                        else
                        {
                            rectPointsList[0].prev.next = newNode;
                            rectPointsList[2].next.prev = newNode;

                            rectPointsList.Clear();
                            rectPointsList.Add(newNode);

                            Selection.Add(newNode);

                            current = newNode;
                        }

                    }
                    else
                    {
                        rectPointsList.Clear();
                        rectPointsList.Add(current);
                    }
                }
                else
                {
                    rectPointsList.Clear();
                    rectPointsList.Add(current);
                }
            }

            current = current.next;

            rectPointsList.Add(current);
        }


        return footprintShape;
    }
}
