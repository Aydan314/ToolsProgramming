using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using System.Runtime.InteropServices.WindowsRuntime;

[System.Serializable]
public class AssetBaseBrush : MonoBehaviour
{
    public virtual void Build(List<BuildNodeData> Selection, AssetBasePack assetPack)
    {
        Debug.Log("Base Brush Build Called Using " + assetPack.name);
    }

    public void PlaceAsset(GameObject asset, GameObject rootObject, Vector3 position, float angle)
    {
        GameObject newObject = Instantiate(asset);
        newObject.transform.position = position;
        newObject.transform.rotation = Quaternion.Euler(0, angle, 0);
        newObject.transform.parent = rootObject.transform;
    }
    public List<BuildNodeData> ForceWindingOrderClockwise(List<BuildNodeData> selection)
    {
        List<Vector3> clockwiseDir = new List<Vector3>() { new Vector3(1, 0, 0), new Vector3(0, 0, -1), new Vector3(-1, 0, 0), new Vector3(0, 0, 1) };
        List<Vector3> antiClockwiseDir = new List<Vector3>() { new Vector3(-1, 0, 0), new Vector3(0, 0, -1), new Vector3(1, 0, 0), new Vector3(0, 0, 1) };
        Vector3 startDir = GetNextNodeDir(selection[0]);

        int clockwiseStartPos = -1;
        for (int i = 0; i < clockwiseDir.Count; i++)
        {
            if (startDir == clockwiseDir[i])
            {
                clockwiseStartPos = i;
                break;
            }
        }

        int antiClockwiseStartPos = -1;
        for (int i = 0; i < antiClockwiseDir.Count; i++)
        {
            if (startDir == antiClockwiseDir[i])
            {
                antiClockwiseStartPos = i;
                break;
            }
        }

        int clockwiseCount = 0;
        int antiClockwiseCount = 0;

        foreach (BuildNodeData node in selection)
        {
            Vector3 nextNodeDir = GetNextNodeDir(node);
            if (nextNodeDir == clockwiseDir[(clockwiseStartPos + clockwiseCount) % clockwiseDir.Count]) clockwiseCount++;
            if (nextNodeDir == antiClockwiseDir[(antiClockwiseStartPos + antiClockwiseCount) % antiClockwiseDir.Count]) antiClockwiseCount++;
        }
       
        if (clockwiseCount > antiClockwiseCount)
        {
            foreach (BuildNodeData node in selection)
            {
                node.windingOrderClockwise = true;
            }
        }

        return selection;
    }

    public Vector3 GetNextNodeDir(BuildNodeData node)
    {
        return (node.position - node.GetNext().position).normalized;
    }

    public bool CompletesCycle(List<BuildNodeData> nodes)
    {

        List<Vector3> clockwiseDir = new List<Vector3>() { new Vector3(1, 0, 0), new Vector3(0, 0, -1), new Vector3(-1, 0, 0), new Vector3(0, 0, 1) };

        int cycleStartPos = -1;
        int i = 0;

        Vector3 firstDir = GetNextNodeDir(nodes[0]);

        foreach (Vector3 dir in clockwiseDir)
        {
            if (dir == firstDir)
            {
                cycleStartPos = i;
                break;
            }
            i++;
        }

        if (cycleStartPos == -1) return false;

        bool cycleFound = true;

        for (int j = 0; j < 3; j++)
        {
            int listPos = (cycleStartPos + j) % clockwiseDir.Count;

            if (clockwiseDir[listPos] != GetNextNodeDir(nodes[j]))
            {
                cycleFound = false;
                break;
            }
        }

        if (!cycleFound)
        {
            cycleStartPos = -1;
            i = 0;

            firstDir = GetNextNodeDir(nodes[0].prev);

            foreach (Vector3 dir in clockwiseDir)
            {
                if (dir == firstDir)
                {
                    cycleStartPos = i;
                    break;
                }
                i++;
            }

            if (cycleStartPos == -1) return false;

            cycleFound = true;

            for (int j = 0; j < 3; j++)
            {
                int listPos = (cycleStartPos + j) % clockwiseDir.Count;

                if (clockwiseDir[listPos] != GetNextNodeDir(nodes[j]))
                {
                    cycleFound = false;
                    break;
                }
            }
        }

        return cycleFound;
    }

    public bool PointLiesOnAnyLine(List<BuildNodeData> nodes, Vector3 point)
    {
        foreach (BuildNodeData node in nodes)
        {
            if (node.GetNext() != null)
            {
                Vector3 nodePos = node.position;
                Vector3 nextNodePos = node.GetNext().position;

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

    public Vector3 GenerateNewPoint(List<BuildNodeData> nodes)
    {
        Vector3 A = nodes[1].position;
        Vector3 B = nodes[2].position;
        Vector3 C = nodes[0].position;

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
        bool windingOrder = Selection[0].windingOrderClockwise;

        List<Rect> footprintShape = new List<Rect>();

        List<BuildNodeData> rectPointsList = new List<BuildNodeData>() { Selection[0] };

        BuildNodeData current = Selection[0];
        int i = 0;

        while (current != null && Selection.Count > 0)
        {
            if (i >= 100)
            {
                Debug.LogError("!! Shape Selection is Too Complex !!");
                return new List<Rect>();
            }
            i++;

            if (rectPointsList.Count == 3)
            {
                if (CompletesCycle(rectPointsList))
                {
                    Vector3 newPoint = GenerateNewPoint(rectPointsList);

                    if (PointLiesOnAnyLine(Selection, newPoint))
                    {
                        Rect rect = GenerateRectFromPoints(new List<Vector3>()
                            {
                                rectPointsList[0].position,
                                rectPointsList[1].position,
                                rectPointsList[2].position,
                                newPoint
                            }
                        );

                        footprintShape.Add(rect);

                        DeleteNodesInvolved(rectPointsList, Selection);

                        BuildNodeData newNode = new BuildNodeData();

                        newNode.position = newPoint;

                        newNode.windingOrderClockwise = windingOrder;

                        newNode.SetPrev(rectPointsList[0].GetPrev());
                        newNode.SetNext(rectPointsList[2].GetNext());

                        BuildNodeData result = Selection.Find(x => x.position == newPoint);
                        if (result != null)
                        {
                            rectPointsList[0].GetPrev().SetNext(result.GetNext());
                            rectPointsList[2].GetNext().SetPrev(result.GetNext());

                            rectPointsList.Clear();
                            rectPointsList.Add(result.GetNext());

                            Selection.Remove(result);

                            current = result;
                        }
                        else
                        {
                            rectPointsList[0].GetPrev().SetNext(newNode);
                            rectPointsList[2].GetNext().SetPrev(newNode);

                            rectPointsList.Clear();
                            rectPointsList.Add(newNode);

                            Selection.Add(newNode);

                            current = newNode;
                        }

                    }
                    else
                    {
                        current = rectPointsList[0];
                        rectPointsList.Clear();


                    }
                }
                else
                {
                    current = rectPointsList[0];
                    rectPointsList.Clear();

                }
            }

            current = current.GetNext();

            rectPointsList.Add(current);
        }


        return footprintShape;
    }
}
