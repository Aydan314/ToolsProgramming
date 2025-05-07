#if UNITY_EDITOR

using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using System.Runtime.InteropServices.WindowsRuntime;

[System.Serializable]
public class AssetBaseBrush : ScriptableObject
{
    public virtual void Build(List<BuildNodeData> Selection, SpawnableObjectPack assetPack)
    {
        Debug.Log("Base Brush Build Called Using " + assetPack.name);
    }

    public void PlaceAsset(GameObject asset, GameObject rootObject, Vector3 position, float angle)
    {
        // Add new gameobject to world and set its parent //
        GameObject newObject = Instantiate(asset);
        newObject.transform.position = position;
        newObject.transform.rotation = Quaternion.Euler(0, angle, 0);
        newObject.transform.parent = rootObject.transform;
    }
    public List<BuildNodeData> ForceWindingOrderClockwise(List<BuildNodeData> selection)
    {
        // List of directions in a square //

        List<Vector3> clockwiseDir = new List<Vector3>() { new Vector3(1, 0, 0), new Vector3(0, 0, -1), new Vector3(-1, 0, 0), new Vector3(0, 0, 1) };
        List<Vector3> antiClockwiseDir = new List<Vector3>() { new Vector3(-1, 0, 0), new Vector3(0, 0, -1), new Vector3(1, 0, 0), new Vector3(0, 0, 1) };
        Vector3 startDir = GetNextNodeDir(selection[0]);

        // Detects which position in the clockwise loop the shape begins //
        int clockwiseStartPos = -1;
        for (int i = 0; i < clockwiseDir.Count; i++)
        {
            if (startDir == clockwiseDir[i])
            {
                clockwiseStartPos = i;
                break;
            }
        }

        // Detects which position in the anticlockwise loop the shape begins //
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

        // Iterates through the shape and records the amount of anticlockwise and clockwise directions followed //
        foreach (BuildNodeData node in selection)
        {
            Vector3 nextNodeDir = GetNextNodeDir(node);
            if (nextNodeDir == clockwiseDir[(clockwiseStartPos + clockwiseCount) % clockwiseDir.Count]) clockwiseCount++;
            if (nextNodeDir == antiClockwiseDir[(antiClockwiseStartPos + antiClockwiseCount) % antiClockwiseDir.Count]) antiClockwiseCount++;
        }
       
        // If the shape follows more clockwise directions reverse winding direction //
        if (clockwiseCount > antiClockwiseCount)
        {
            foreach (BuildNodeData node in selection)
            {
                node.windingOrderClockwise = true;
            }
        }

        return selection;
    }

    public bool IsWindingOrderClockwise(List<BuildNodeData> selection)
    {
        // List of directions in a square //

        List<Vector3> clockwiseDir = new List<Vector3>() { new Vector3(1, 0, 0), new Vector3(0, 0, -1), new Vector3(-1, 0, 0), new Vector3(0, 0, 1) };
        List<Vector3> antiClockwiseDir = new List<Vector3>() { new Vector3(-1, 0, 0), new Vector3(0, 0, -1), new Vector3(1, 0, 0), new Vector3(0, 0, 1) };
        Vector3 startDir = GetNextNodeDir(selection[0]);

        // Detects which position in the clockwise loop the shape begins //
        int clockwiseStartPos = -1;
        for (int i = 0; i < clockwiseDir.Count; i++)
        {
            if (startDir == clockwiseDir[i])
            {
                clockwiseStartPos = i;
                break;
            }
        }

        // Detects which position in the anticlockwise loop the shape begins //
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

        // Iterates through the shape and records the amount of anticlockwise and clockwise directions followed //
        foreach (BuildNodeData node in selection)
        {
            Vector3 nextNodeDir = GetNextNodeDir(node);
            if (nextNodeDir == clockwiseDir[(clockwiseStartPos + clockwiseCount) % clockwiseDir.Count]) clockwiseCount++;
            if (nextNodeDir == antiClockwiseDir[(antiClockwiseStartPos + antiClockwiseCount) % antiClockwiseDir.Count]) antiClockwiseCount++;
        }

        // If the shape follows more clockwise directions reverse winding direction //
        
        return clockwiseCount > antiClockwiseCount;
    }

    public Vector3 GetNextNodeDir(BuildNodeData node)
    {
        return (node.position - node.GetNext().position).normalized;
    }

    public bool CompletesCycle(List<BuildNodeData> nodes)
    {
        // List of clockwise directions in a square //
        List<Vector3> clockwiseDir = new List<Vector3>() { new Vector3(1, 0, 0), new Vector3(0, 0, -1), new Vector3(-1, 0, 0), new Vector3(0, 0, 1) };

        int cycleStartPos = -1;

        Vector3 firstDir = GetNextNodeDir(nodes[0]);

        // Find where in the winding order the shape starts //
        for (int i = 0; i < clockwiseDir.Count; i++)
        {
            if (clockwiseDir[i] == firstDir)
            {
                cycleStartPos = i;
                break;
            }
        }

        if (cycleStartPos == -1) return false;

        bool cycleFound = true;

        // Detect if all 4 next node directions match the list starting at any direction //
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

            firstDir = GetNextNodeDir(nodes[0].prev);

            // Find prev node direction start in list //
            for (int i = 0; i < clockwiseDir.Count; i++)
            {
                if (clockwiseDir[i] == firstDir)
                {
                    cycleStartPos = i;
                    break;
                }
            }



            if (cycleStartPos == -1) return false;

            cycleFound = true;

            // Detect if all 4 prev node directions match the list //
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
            // Iterate throgh shape to see if given points axis matches and is between itself and its next node //
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
        // Given 3 points generate the 4th point that would complete the square //
        Vector3 A = nodes[1].position;
        Vector3 B = nodes[2].position;
        Vector3 C = nodes[0].position;

        return A + (B - A) + (C - A);
    }

    public Rect GenerateRectFromPoints(List<Vector3> points)
    {
        // Find min amd max bounds from list for rect and create //
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

        // Loop through shape until its empty //
        while (current != null && Selection.Count > 0)
        {
            // Prevent algorithm from running too long //
            if (i >= 100)
            {
                Debug.LogError("!! Shape Selection is Too Complex !!");
                return new List<Rect>();
            }
            i++;

            // Once 3 nodes have been exploded attempt to cut out a rect from the shape //
            if (rectPointsList.Count == 3)
            {
                // If the 3 nodes have the correct winding order, so arent on the outside //
                if (CompletesCycle(rectPointsList))
                {
                    Vector3 newPoint = GenerateNewPoint(rectPointsList);

                    // If the created rect will fit inside the shape //
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
                        // Divide out the new shape //
                        footprintShape.Add(rect);
                        
                        // remove points used to divide out rect //
                        DeleteNodesInvolved(rectPointsList, Selection);

                        BuildNodeData newNode = new BuildNodeData();

                        newNode.position = newPoint;

                        newNode.windingOrderClockwise = windingOrder;

                        newNode.SetPrev(rectPointsList[0].GetPrev());
                        newNode.SetNext(rectPointsList[2].GetNext());

                        // Add new point to the shape mesh and reconnect the points back up //

                        BuildNodeData result = Selection.Find(x => x.position == newPoint);
                        if (result != null)
                        {
                            // If the new point is an already existing point in the shape, remove it as it isnt needed //
                            rectPointsList[0].GetPrev().SetNext(result.GetNext());
                            rectPointsList[2].GetNext().SetPrev(result.GetNext());

                            rectPointsList.Clear();
                            rectPointsList.Add(result.GetNext());

                            Selection.Remove(result);

                            current = result;
                        }
                        else
                        {
                            // Otherwise add the new point to the mesh //
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
                        // Reset the explored nodes //
                        current = rectPointsList[0];
                        rectPointsList.Clear();

                    }
                }
                else
                {
                    // Reset the explored nodes //
                    current = rectPointsList[0];
                    rectPointsList.Clear();
                }
            }

            // Add current node to explored list //
            current = current.GetNext();

            rectPointsList.Add(current);
        }


        return footprintShape;
    }

    public bool DetectNodeAtCorner(BuildNodeData buildNode)
    {
        // Detect if directions are perpendicular //
        Vector3 nextNodeDir = (buildNode.position - buildNode.GetNext().position).normalized;
        Vector3 prevNodeDir = (buildNode.position - buildNode.GetPrev().position).normalized;

        return nextNodeDir != (prevNodeDir * -1.0f);
    }

    public float CalculateAssetRotation(BuildNodeData buildNode)
    {
        Vector3 nextNodeDir = (buildNode.position - buildNode.GetNext().position).normalized;

        // Give asset correct rotation based on previous node direction //

        if (nextNodeDir.z > 0) return 270.0f;
        else if (nextNodeDir.x < 0) return 180.0f;
        else if (nextNodeDir.z < 0) return 90.0f;

        return 0.0f;
    }

    public float CalculateAssetCornerRotation(BuildNodeData buildNode)
    {
        Vector3 prevNodeDir = (buildNode.position - buildNode.GetPrev().position).normalized;
        Vector3 nextNodeDir = (buildNode.position - buildNode.GetNext().position).normalized;

        // Anti clockwise winding order //
        if (prevNodeDir.x < 0 && nextNodeDir.z > 0) return 270.0f;
        else if (prevNodeDir.z < 0 && nextNodeDir.x < 0) return 180.0f;
        else if (prevNodeDir.x > 0 && nextNodeDir.z < 0) return 90.0f;

        // Clockwise winding order //
        if (prevNodeDir.z < 0 && nextNodeDir.x > 0) return 90.0f;
        else if (prevNodeDir.x < 0 && nextNodeDir.z < 0) return 180.0f;
        else if (prevNodeDir.z > 0 && nextNodeDir.x < 0) return 270.0f;

        return 0.0f;
    }

    public SpawnableObject SelectCornerAsset(BuildNodeData buildNode, SpawnableObjectPack assetPack)
    {
        List<SpawnableObject> insetCorners = assetPack.GetInsetCornerObjects();
        List<SpawnableObject> outsetCorners = assetPack.GetOutsetCornerObjects();
        List<SpawnableObject> defaults = assetPack.GetDefaultObjects();

        List<BuildNodeData> corner = new List<BuildNodeData>() { buildNode.GetPrev(), buildNode, buildNode.GetNext()};

        Vector3 prevNodeDir = (buildNode.position - buildNode.GetPrev().position).normalized;
        Vector3 nextNodeDir = (buildNode.position - buildNode.GetNext().position).normalized;

        // Anti clockwise winding order //
        if (!IsWindingOrderClockwise(corner))
        {
            if (insetCorners.Count > 0)
            {
                return assetPack.PickRandomFromObjects(assetPack.GetInsetCornerObjects());
            }
            else if (defaults.Count > 0) return assetPack.GetDefaultObjects()[0];
            else return null;
        }

        // Clockwise winding order //

        if (outsetCorners.Count > 0)
        {
            return assetPack.PickRandomFromObjects(assetPack.GetOutsetCornerObjects());
        }
        else if (defaults.Count > 0) return assetPack.GetDefaultObjects()[0];
        else return null;
    }

    public void BuildAssetsBetween(BuildNodeData node, SpawnableObjectPack assetPack, GameObject rootObject)
    {
        Vector3 end = node.GetNext().position;
        Vector3 start = node.position;

        float distance = (end - start).magnitude;

        distance = assetPack.gridSize * Mathf.Round(distance / assetPack.gridSize);

        Vector3 step = (end - start) / distance;

        if (assetPack.GetOutsetCornerObjects().Count > 0)
        {
            SpawnableObject asset = assetPack.PickRandomFromObjects(assetPack.spawnableObjects);

            // Places a corner asset if it is valid //
            if (node.GetPrev() != node && DetectNodeAtCorner(node))
            {
                SpawnableObject prefab = SelectCornerAsset(node, assetPack);

                if (Random.Range(0, 100) / 100.0f < assetPack.spreadDensity && prefab != null) PlaceAsset(prefab.objectPrefab, rootObject, start, CalculateAssetCornerRotation(node) + prefab.defaultRotation);
            }
            // Otherwise places a default asset //
            else
            {
                if (Random.Range(0, 100) / 100.0f < assetPack.spreadDensity) PlaceAsset(asset.objectPrefab, rootObject, start, CalculateAssetRotation(node) + asset.defaultRotation);
            }
        }
        if (assetPack.GetOutlineObjects().Count > 0)
        {

            for (float i = assetPack.gridSize; i < distance; i += assetPack.gridSize)
            {
                SpawnableObject asset = assetPack.PickRandomFromObjects(assetPack.GetOutlineObjects());
                if (Random.Range(0, 100) / 100.0f < assetPack.spreadDensity) PlaceAsset(asset.objectPrefab, rootObject, start + (i * step), CalculateAssetRotation(node) + asset.defaultRotation);
            }
        }
    }
}
#endif