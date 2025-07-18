using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class HelperFunctions
{
    #region GameObject

    public static void ApplyRandomRotation(GameObject obj)
    {
        obj.transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
    }

    public static GameObject GetChild(GameObject parent, string name)
    {
        for(int i = 0; i < parent.transform.childCount; i++)
        {
            if (parent.transform.GetChild(i).gameObject.name == name) return parent.transform.GetChild(i).gameObject;
        }
        return null;
    }

    /// <summary>
    /// Recursively sets the layer of the given GameObject and all of its children.
    /// </summary>
    /// <param name="root">The root GameObject whose layer (and all descendants’) will be changed.</param>
    /// <param name="layer">The layer index to assign.</param>
    public static void SetLayerRecursively(GameObject root, int layer)
    {
        if (root == null) return;
        root.layer = layer;
        foreach (Transform child in root.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    #endregion

    #region Enum

    public static string GetEnumDescription(System.Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        if (fieldInfo == null)
            return value.ToString();

        var attribute = fieldInfo
            .GetCustomAttributes(typeof(DescriptionAttribute), false)
            .FirstOrDefault() as DescriptionAttribute;

        return attribute?.Description ?? value.ToString();
    }

    public static T GetRandomEnumValue<T>() where T : System.Enum
    {
        var values = System.Enum.GetValues(typeof(T));
        return (T)values.GetValue(new System.Random().Next(values.Length));
    }

    #endregion

    #region Math

    /// <summary>
    /// Modulo that handles negative values in a logical way.
    /// </summary>
    public static int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    public static float SmoothLerp(float start, float end, float t)
    {
        t = Mathf.Clamp01(t);
        t = t * t * (3f - 2f * t);
        return Mathf.Lerp(start, end, t);
    }

    public static Vector3 SmoothLerp(Vector3 start, Vector3 end, float t)
    {
        t = Mathf.Clamp01(t);
        t = t * t * (3f - 2f * t);
        return Vector3.Lerp(start, end, t);
    }

    /// <summary>
    /// Rasterizes a line between two points using Bresenham's line algorithm.
    /// Returns a list of all grid cells that should be filled, considering the specified line thickness.
    /// </summary>
    public static List<Vector2Int> RasterizeLine(Vector2 start, Vector2 end, int lineThickness)
    {
        List<Vector2Int> points = new List<Vector2Int>();

        float x0 = start.x;
        float y0 = start.y;
        float x1 = end.x;
        float y1 = end.y;

        float dx = Mathf.Abs(x1 - x0);
        float dy = Mathf.Abs(y1 - y0);
        float sx = x0 < x1 ? 1f : -1f;
        float sy = y0 < y1 ? 1f : -1f;
        float err = dx - dy;

        // Calculate half thickness
        float additionalWidthOnEachSide = ((lineThickness - 1f) / 2f);

        while (true)
        {
            // Add points around the main point to achieve the desired thickness
            for (float tx = -additionalWidthOnEachSide; tx <= additionalWidthOnEachSide; tx += 0.1f)
            {
                for (float ty = -additionalWidthOnEachSide; ty <= additionalWidthOnEachSide; ty += 0.1f)
                {
                    // Add point only if it's within the square around the thickness radius
                    if (Mathf.Abs(tx) + Mathf.Abs(ty) <= additionalWidthOnEachSide)
                    {
                        Vector2Int point = new Vector2Int(Mathf.RoundToInt(x0 + tx), Mathf.RoundToInt(y0 + ty));
                        if (!points.Contains(point))
                        {
                            points.Add(point);
                        }
                    }
                }
            }

            if (Mathf.Abs(x0 - x1) <= 1f && Mathf.Abs(y0 - y1) <= 1f)
                break;

            float e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }

        return points;
    }

    public static void SetAsMirrored(GameObject obj)
    {
        obj.transform.localScale = new Vector3(obj.transform.localScale.x * -1f, obj.transform.localScale.y, obj.transform.localScale.z);
    }

    /// <summary>
    /// Creates a list of points along a parabolic arc between start and end.
    /// </summary>
    /// <param name="start">Starting point of the arc.</param>
    /// <param name="end">Ending point of the arc.</param>
    /// <param name="height">Maximum height of the arc.</param>
    /// <param name="segments">Number of segments the arc is divided into.</param>
    /// <returns>A list of Vector3 points along the arc.</returns>
    public static List<Vector3> CreateArc(Vector3 start, Vector3 end, float height, int segments)
    {
        List<Vector3> arcPoints = new List<Vector3>();

        // Add the start point
        arcPoints.Add(start);

        // Calculate the arc
        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments; // Normalized parameter (0 to 1)

            // Linear interpolation between start and end
            Vector3 linearPoint = Vector3.Lerp(start, end, t);

            // Add vertical parabolic height
            float parabolicHeight = 4 * height * t * (1 - t); // Parabolic equation
            linearPoint.y += parabolicHeight;

            arcPoints.Add(linearPoint);
        }

        return arcPoints;
    }

    #endregion

    #region Random

    public static T GetWeightedRandomElement<T>(Dictionary<T, int> weightDictionary)
    {
        int probabilitySum = weightDictionary.Sum(x => x.Value);
        int rng = Random.Range(0, probabilitySum);
        int tmpSum = 0;
        foreach (KeyValuePair<T, int> kvp in weightDictionary)
        {
            tmpSum += kvp.Value;
            if (rng < tmpSum) return kvp.Key;
        }
        throw new System.Exception();
    }
    public static T GetWeightedRandomElement<T>(Dictionary<T, float> weightDictionary)
    {
        float probabilitySum = weightDictionary.Sum(x => x.Value);
        float rng = Random.Range(0, probabilitySum);
        float tmpSum = 0;
        foreach (KeyValuePair<T, float> kvp in weightDictionary)
        {
            tmpSum += kvp.Value;
            if (rng < tmpSum) return kvp.Key;
        }
        throw new System.Exception();
    }

    /// <summary>
    /// Returns a random number in a gaussian distribution. About 2/3 of generated numbers are within the standard deviation of the mean.
    /// </summary>
    public static float NextGaussian(float mean, float standard_deviation)
    {
        return mean + NextGaussian() * standard_deviation;
    }
    private static float NextGaussian()
    {
        float v1, v2, s;
        do
        {
            v1 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            v2 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            s = v1 * v1 + v2 * v2;
        } while (s >= 1.0f || s == 0f);
        s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);

        return v1 * s;
    }
    public static Vector2Int GetRandomNearPosition(Vector2Int pos, float standard_deviation)
    {
        float x = NextGaussian(pos.x, standard_deviation);
        float y = NextGaussian(pos.y, standard_deviation);

        return new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
    }

    public static Direction GetRandomSide() => GetSides().RandomElement();
    public static Direction GetRandomCorner() => GetCorners().RandomElement();

    #endregion

    #region Direction

    public static Direction GetNextDirection8(Direction dir)
    {
        return dir switch
        {
            Direction.N => Direction.NE,
            Direction.NE => Direction.E,
            Direction.E => Direction.SE,
            Direction.SE => Direction.S,
            Direction.S => Direction.SW,
            Direction.SW => Direction.W,
            Direction.W => Direction.NW,
            Direction.NW => Direction.N,
            _ => throw new System.Exception("Direction " + dir.ToString() + " not handled")
        };
    }
    public static Direction GetPreviousDirection8(Direction dir)
    {
        return dir switch
        {
            Direction.N => Direction.NW,
            Direction.NW => Direction.W,
            Direction.W => Direction.SW,
            Direction.SW => Direction.S,
            Direction.S => Direction.SE,
            Direction.SE => Direction.E,
            Direction.E => Direction.NE,
            Direction.NE => Direction.N,
            _ => throw new System.Exception("Direction " + dir.ToString() + " not handled")
        };
    }

    public static Direction GetNextSideDirection(Direction dir)
    {
        return dir switch
        {
            Direction.N => Direction.E,
            Direction.E => Direction.S,
            Direction.S => Direction.W,
            Direction.W => Direction.N,
            _ => throw new System.Exception("Direction " + dir.ToString() + " not handled")
        };
    }
    public static Direction GetPreviousSideDirection(Direction dir)
    {
        return dir switch
        {
            Direction.N => Direction.W,
            Direction.E => Direction.N,
            Direction.S => Direction.E,
            Direction.W => Direction.S,
            _ => throw new System.Exception("Direction " + dir.ToString() + " not handled")
        };
    }

    public static Direction GetAdjacentDirection(Vector2Int from, Vector2Int to)
    {
        if (to == from) return Direction.None;

        if (to == from + new Vector2Int(1, 0)) return Direction.E;
        if (to == from + new Vector2Int(-1, 0)) return Direction.W;
        if (to == from + new Vector2Int(0, 1)) return Direction.N;
        if (to == from + new Vector2Int(0, -1)) return Direction.S;

        if (to == from + new Vector2Int(1, 1)) return Direction.NE;
        if (to == from + new Vector2Int(-1, 1)) return Direction.NW;
        if (to == from + new Vector2Int(1, -1)) return Direction.SE;
        if (to == from + new Vector2Int(-1, -1)) return Direction.SW;

        throw new System.Exception("The two given coordinates are not equal or adjacent to each other.");
    }

    public static Direction GetOppositeDirection(Direction dir)
    {
        return dir switch
        {
            Direction.N => Direction.S,
            Direction.E => Direction.W,
            Direction.S => Direction.N,
            Direction.W => Direction.E,
            Direction.NE => Direction.SW,
            Direction.NW => Direction.SE,
            Direction.SW => Direction.NE,
            Direction.SE => Direction.NW,
            Direction.None => Direction.None,
            _ => throw new System.Exception("Direction " + dir.ToString() + " not handled")
        };
    }

    public static Direction GetDirection8FromAngle(float angle, float offset = 0)
    {
        // Normalize angle to the [0, 360) range
        float angleDegrees = angle + offset;
        angleDegrees %= 360f;
        if (angleDegrees < 0)
        {
            angleDegrees += 360f;
        }

        // Offset by 22.5 so that each 45° segment is centered
        double segmentOffset = (angleDegrees + 22.5) % 360f;

        // Determine which 45° segment the angle belongs in
        int segment = (int)(segmentOffset / 45.0);

        switch (segment)
        {
            case 0: return Direction.N;
            case 1: return Direction.NE;
            case 2: return Direction.E;
            case 3: return Direction.SE;
            case 4: return Direction.S;
            case 5: return Direction.SW;
            case 6: return Direction.W;
            case 7: return Direction.NW;
            default: return Direction.None;
        }
    }

    public static Direction GetDirection4FromAngle(float angle, float offset = 0)
    {
        // Normalize angle to the [0, 360) range
        float angleDegrees = angle + offset;
        angleDegrees %= 360f;
        if (angleDegrees < 0)
        {
            angleDegrees += 360f;
        }

        // Offset by 45 so that each 90° segment is centered
        double segmentOffset = (angleDegrees + 45) % 360f;

        // Determine which 90° segment the angle belongs in
        int segment = (int)(segmentOffset / 90.0);

        switch (segment)
        {
            case 0: return Direction.N;
            case 1: return Direction.E;
            case 2: return Direction.S;
            case 3: return Direction.W;
            default: return Direction.None;
        }
    }


    private static List<Direction> _Directions8 = new List<Direction>() { Direction.N, Direction.NE, Direction.E, Direction.SE, Direction.S, Direction.SW, Direction.W, Direction.NW };
    public static List<Direction> GetAllDirections8() => _Directions8;
    
    private static List<Direction> _Directions9 = new List<Direction>() { Direction.None, Direction.N, Direction.NE, Direction.E, Direction.SE, Direction.S, Direction.SW, Direction.W, Direction.NW };
    public static List<Direction> GetAllDirections9() => _Directions9;
    
    private static List<Direction> _Corners = new List<Direction>() { Direction.SW, Direction.SE, Direction.NE, Direction.NW };
    public static List<Direction> GetCorners() => _Corners;

    private static List<Direction> _Sides = new List<Direction>() { Direction.N, Direction.E, Direction.S, Direction.W };
    public static List<Direction> GetSides() => _Sides;
    public static bool IsCorner(Direction dir) => GetCorners().Contains(dir);
    public static bool IsSide(Direction dir) => GetSides().Contains(dir);

    public static Vector2Int GetCoordinatesInDirection(Vector2Int coordinates, Direction dir)
    {
        return coordinates + GetDirectionVectorInt(dir);
    }
    public static Vector2Int GetDirectionVectorInt(Direction dir, int distance = 1)
    {
        if (dir == Direction.N) return new Vector2Int(0, distance);
        if (dir == Direction.E) return new Vector2Int(distance, 0);
        if (dir == Direction.S) return new Vector2Int(0, -distance);
        if (dir == Direction.W) return new Vector2Int(-distance, 0);
        if (dir == Direction.NE) return new Vector2Int(distance, distance);
        if (dir == Direction.NW) return new Vector2Int(-distance, distance);
        if (dir == Direction.SE) return new Vector2Int(distance, -distance);
        if (dir == Direction.SW) return new Vector2Int(-distance, -distance);
        return new Vector2Int(0, 0);
    }

    public static Vector2 GetDirectionVectorFloat(Direction dir, float distance = 1f)
    {
        if (dir == Direction.N) return new Vector2(0, distance);
        if (dir == Direction.E) return new Vector2(distance, 0);
        if (dir == Direction.S) return new Vector2(0, -distance);
        if (dir == Direction.W) return new Vector2(-distance, 0);
        if (dir == Direction.NE) return new Vector2(distance, distance);
        if (dir == Direction.NW) return new Vector2(-distance, distance);
        if (dir == Direction.SE) return new Vector2(distance, -distance);
        if (dir == Direction.SW) return new Vector2(-distance, -distance);
        return new Vector2(0, 0);
    }

    /// <summary>
    /// Returns the corner directions that are relevant for a given direction.
    /// </summary>
    public static List<Direction> GetAffectedCorners(Direction dir)
    {
        if (dir == Direction.None) return new List<Direction> { Direction.NE, Direction.NW, Direction.SW, Direction.SE };
        if (dir == Direction.N) return new List<Direction> { Direction.NE, Direction.NW };
        if (dir == Direction.E) return new List<Direction> { Direction.NE, Direction.SE };
        if (dir == Direction.S) return new List<Direction> { Direction.SW, Direction.SE };
        if (dir == Direction.W) return new List<Direction> { Direction.SW, Direction.NW };
        if (dir == Direction.NW) return new List<Direction>() { Direction.NW };
        if (dir == Direction.NE) return new List<Direction>() { Direction.NE };
        if (dir == Direction.SE) return new List<Direction>() { Direction.SE };
        if (dir == Direction.SW) return new List<Direction>() { Direction.SW };
        throw new System.Exception("Direction " + dir.ToString() + " not handled");
    }
    public static bool DoAffectedCornersOverlap(Direction dir1, Direction dir2) => GetAffectedCorners(dir1).Intersect(GetAffectedCorners(dir2)).Any();

    public static List<Direction> GetAffectedSides(Direction dir)
    {
        if (dir == Direction.None) return new List<Direction> { Direction.N, Direction.E, Direction.S, Direction.W };
        if (dir == Direction.N) return new List<Direction> { Direction.N };
        if (dir == Direction.E) return new List<Direction> { Direction.E };
        if (dir == Direction.S) return new List<Direction> { Direction.S };
        if (dir == Direction.W) return new List<Direction> { Direction.W };
        if (dir == Direction.NW) return new List<Direction>() { Direction.N, Direction.W };
        if (dir == Direction.NE) return new List<Direction>() { Direction.N, Direction.E };
        if (dir == Direction.SE) return new List<Direction>() { Direction.S, Direction.E };
        if (dir == Direction.SW) return new List<Direction>() { Direction.S, Direction.W };
        throw new System.Exception("Direction " + dir.ToString() + " not handled");
    }

    public static List<Direction> GetAffectedDirections(Direction dir)
    {
        if (dir == Direction.None) return GetAllDirections8();
        if (dir == Direction.N) return new List<Direction> { Direction.NW, Direction.N, Direction.NE };
        if (dir == Direction.E) return new List<Direction> { Direction.NE, Direction.E, Direction.SE };
        if (dir == Direction.S) return new List<Direction> { Direction.SW, Direction.S, Direction.SE };
        if (dir == Direction.W) return new List<Direction> { Direction.NW, Direction.W, Direction.SW };
        if (dir == Direction.NW) return new List<Direction>() { Direction.NW, Direction.N, Direction.W };
        if (dir == Direction.NE) return new List<Direction>() { Direction.NE, Direction.N, Direction.E };
        if (dir == Direction.SE) return new List<Direction>() { Direction.SE, Direction.S, Direction.E };
        if (dir == Direction.SW) return new List<Direction>() { Direction.SW, Direction.S, Direction.W };
        throw new System.Exception("Direction " + dir.ToString() + " not handled");
    }

    public static Direction GetMirroredCorner(Direction dir, Direction axis)
    {
        if(axis == Direction.N || axis == Direction.S) // east,west stays the same
        {
            if (dir == Direction.NE) return Direction.SE;
            if (dir == Direction.NW) return Direction.SW;
            if (dir == Direction.SW) return Direction.NW;
            if (dir == Direction.SE) return Direction.NE;
        }
        if (axis == Direction.E || axis == Direction.W) // north,south stays the same
        {
            if (dir == Direction.NE) return Direction.NW;
            if (dir == Direction.NW) return Direction.NE;
            if (dir == Direction.SW) return Direction.SE;
            if (dir == Direction.SE) return Direction.SW;
        }
        throw new System.Exception("axis " + axis.ToString() + " not handled or direction " + dir.ToString() + " not handled");
    }

    /// <summary>
    /// Returns the heights for a flat surface based on its height.
    /// </summary>
    public static Dictionary<Direction, int> GetFlatHeights(int height)
    {
        Dictionary<Direction, int> heights = new Dictionary<Direction, int>();
        foreach (Direction dir in GetCorners()) heights.Add(dir, height);
        return heights;
    }

    /// <summary>
    /// Returns the heights for a sloped surface based on its upwards direction and base height.
    /// </summary>
    public static Dictionary<Direction, int> GetSlopeHeights(int baseHeight, Direction dir)
    {
        Dictionary<Direction, int> heights = new Dictionary<Direction, int>();
        foreach (Direction corner in GetAffectedCorners(dir))
        {
            heights.Add(corner, baseHeight + 1);
            heights.Add(GetOppositeDirection(corner), baseHeight);
        }
        return heights;
    }

    public static float GetDirectionAngle(Direction dir)
    {
        if (dir == Direction.N) return 0f;
        if (dir == Direction.NE) return 45f;
        if (dir == Direction.E) return 90f;
        if (dir == Direction.SE) return 135f;
        if (dir == Direction.S) return 180f;
        if (dir == Direction.SW) return 225f;
        if (dir == Direction.W) return 270f;
        if (dir == Direction.NW) return 315f;
        return 0f;
    }

    public static Quaternion Get2dRotationByDirection(Direction dir)
    {
        return Quaternion.Euler(0f, GetDirectionAngle(dir), 0f);
    }

    /// <summary>
    /// Returns the cell coordinates when going from source coordinates into a 2d direction on the same altitude level.
    /// </summary>
    public static Vector3Int GetAdjacentCellCoordinates(Vector3Int cellCoordinates, Direction dir)
    {
        Vector2Int dirVector = GetDirectionVectorInt(dir);
        return new Vector3Int(cellCoordinates.x + dirVector.x, cellCoordinates.y, cellCoordinates.z + dirVector.y);
    }

    /// <summary>
    /// Returns the global cell coordinates of the wall that is to the left/right/above/below a wall piece with the given source coordinates and side.
    /// <br/> dir refers which direction we want to search, whereas (N = Above, S = Below, W = Left, E = Right).
    /// </summary>
    public static Vector3Int GetAdjacentWallCellCoordinates(Vector3Int sourceCoordinates, Direction sourceSide, Direction dir)
    {
        if (dir == Direction.N) return new Vector3Int(sourceCoordinates.x, sourceCoordinates.y + 1, sourceCoordinates.z);
        if (dir == Direction.S) return new Vector3Int(sourceCoordinates.x, sourceCoordinates.y - 1, sourceCoordinates.z);

        Vector3Int offset = Vector3Int.zero;
        if (GetAffectedSides(dir).Contains(Direction.N)) offset = new Vector3Int(0, 1, 0);
        if (GetAffectedSides(dir).Contains(Direction.S)) offset = new Vector3Int(0, -1, 0);

        if (GetAffectedSides(dir).Contains(Direction.W))
        {
            return sourceSide switch
            {
                Direction.N => offset + new Vector3Int(sourceCoordinates.x + 1, sourceCoordinates.y, sourceCoordinates.z),
                Direction.S => offset + new Vector3Int(sourceCoordinates.x - 1, sourceCoordinates.y, sourceCoordinates.z),
                Direction.W => offset + new Vector3Int(sourceCoordinates.x, sourceCoordinates.y, sourceCoordinates.z + 1),
                Direction.E => offset + new Vector3Int(sourceCoordinates.x, sourceCoordinates.y, sourceCoordinates.z - 1),
                _ => throw new System.Exception("direction not handled")
            };
        }

        if (GetAffectedSides(dir).Contains(Direction.E))
        {
            return sourceSide switch
            {
                Direction.N => offset + new Vector3Int(sourceCoordinates.x - 1, sourceCoordinates.y, sourceCoordinates.z),
                Direction.S => offset + new Vector3Int(sourceCoordinates.x + 1, sourceCoordinates.y, sourceCoordinates.z),
                Direction.W => offset + new Vector3Int(sourceCoordinates.x, sourceCoordinates.y, sourceCoordinates.z - 1),
                Direction.E => offset + new Vector3Int(sourceCoordinates.x, sourceCoordinates.y, sourceCoordinates.z + 1),
                _ => throw new System.Exception("direction not handled")
            };
        }
        throw new System.Exception("direction not handled");
    }

    public static Vector2Int GetCornerCoordinates(Vector2Int dimensions, Direction corner)
    {
        if (corner == Direction.SW) return new Vector2Int(0, 0);
        if (corner == Direction.SE) return new Vector2Int(dimensions.x - 1, 0);
        if (corner == Direction.NE) return new Vector2Int(dimensions.x - 1, dimensions.y - 1);
        if (corner == Direction.NW) return new Vector2Int(0, dimensions.y - 1);
        throw new System.Exception($"{corner} is not a valid corner direction.");
    }

    /// <summary>
    /// Groups the given coordinates into clusters based on 4-directional connectivity.
    /// </summary>
    /// <param name="coordinates">A set of 2D coordinates.</param>
    /// <returns>A list of clusters, each cluster represented as a HashSet of connected coordinates.</returns>
    public static List<HashSet<Vector2Int>> GetConnectedClusters(HashSet<Vector2Int> coordinates)
    {
        var clusters = new List<HashSet<Vector2Int>>();
        var visited = new HashSet<Vector2Int>();

        // For each coordinate, if it's not yet visited, perform a BFS or DFS to find its cluster
        foreach (var coord in coordinates)
        {
            if (!visited.Contains(coord))
            {
                var cluster = new HashSet<Vector2Int>();
                var queue = new Queue<Vector2Int>();

                // Start BFS
                visited.Add(coord);
                queue.Enqueue(coord);

                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    cluster.Add(current);

                    // Check all 4 possible neighbors
                    foreach (var neighbor in GetNeighbors(current))
                    {
                        // If neighbor is part of the original set and not yet visited, add it
                        if (coordinates.Contains(neighbor) && !visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            queue.Enqueue(neighbor);
                        }
                    }
                }

                clusters.Add(cluster);
            }
        }

        return clusters;
    }

    /// <summary>
    /// Returns the 4 orthogonal neighbors for a given coordinate.
    /// </summary>
    private static IEnumerable<Vector2Int> GetNeighbors(Vector2Int coord)
    {
        yield return new Vector2Int(coord.x + 1, coord.y);
        yield return new Vector2Int(coord.x - 1, coord.y);
        yield return new Vector2Int(coord.x, coord.y + 1);
        yield return new Vector2Int(coord.x, coord.y - 1);
    }

    #endregion

    #region UI

    /// <summary>
    /// Destroys all children of a GameObject immediately.
    /// </summary>
    public static void DestroyAllChildredImmediately(GameObject obj, int skipElements = 0)
    {
        int numChildren = obj.transform.childCount;
        for (int i = skipElements; i < numChildren; i++) GameObject.DestroyImmediate(obj.transform.GetChild(skipElements).gameObject);
    }

    public static Sprite TextureToSprite(Texture tex) => TextureToSprite((Texture2D)tex);
    public static Sprite TextureToSprite(Texture2D tex)
    {
        if (tex == null) return null;
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }
    public static Sprite GetAssetPreviewSprite(string path)
    {
#if UNITY_EDITOR
        // Only executes in the Unity Editor
        UnityEngine.Object asset = Resources.Load(path);
        if (asset == null)
            throw new System.Exception($"Could not find asset with path {path}.");

        // The AssetPreview class is also editor-only
        Texture2D assetPreviewTexture = UnityEditor.AssetPreview.GetAssetPreview(asset);
        // if (assetPreviewTexture == null) 
        //    throw new System.Exception($"Could not create asset preview texture of {asset} ({path}).");

        return TextureToSprite(assetPreviewTexture);
#else
    // Always returns null in builds
    return null;
#endif
    }

    public static Sprite TextureToSprite(string resourcePath)
    {
        Texture2D texture = Resources.Load<Texture2D>(resourcePath);
        return TextureToSprite(texture);
    }

    /// <summary>
    /// Sets the Left, Right, Top and Bottom attribute of a RectTransform
    /// </summary>
    public static void SetRectTransformMargins(RectTransform rt, float left, float right, float top, float bottom)
    {
        rt.offsetMin = new Vector2(left, bottom);
        rt.offsetMax = new Vector2(-right, -top);
    }

    public static void SetLeft(RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }

    /// <summary>
    /// Unfocusses any focussed button/dropdown/toggle UI element so that keyboard inputs don't get 'absorbed' by the UI element.
    /// </summary>
    public static void UnfocusNonInputUiElements()
    {
        if (EventSystem.current.currentSelectedGameObject != null && (
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>() != null ||
            EventSystem.current.currentSelectedGameObject.GetComponent<TMP_Dropdown>() != null ||
            EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>() != null
            ))
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    /// <summary>
    /// Returns if any ui element is currently focussed.
    /// </summary>
    public static bool IsUiFocussed()
    {
        return EventSystem.current.currentSelectedGameObject != null;
    }

    /// <summary>
    /// Returns is the mouse is currently hovering over a UI element.
    /// </summary>
    public static bool IsMouseOverUi()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public static bool IsPointerOverUIWithComponent<T>() where T : UnityEngine.Component
    {
        EventSystem eventSystem = EventSystem.current;
        GraphicRaycaster raycaster = UnityEngine.Object.FindFirstObjectByType<GraphicRaycaster>();

        PointerEventData pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.GetComponent<T>() != null)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if the mouse is currently over a UI element, excluding certain UI objects
    /// and all their children.
    /// </summary>
    /// <param name="excludedUiElements">
    /// Optional list of UI GameObjects to ignore in the check (including any of their children).
    /// </param>
    /// <returns>
    /// True if mouse is over a UI element that is not excluded; false otherwise.
    /// </returns>
    public static bool IsMouseOverUiExcept(params GameObject[] excludedUiElements)
    {
        // Quick check: if pointer isn't over *any* UI elements, we can stop.
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            return false;
        }

        // Perform a UI raycast from the mouse pointer
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        // If no UI elements are hit, we can stop
        if (results.Count == 0)
        {
            return false;
        }

        // Check each UI element that was hit by the raycast
        foreach (RaycastResult result in results)
        {
            GameObject hitObject = result.gameObject;

            // If the hit object is not in the excluded list and not a child of an excluded object,
            // then we consider the mouse to be over a "meaningful" UI element.
            if (!IsExcluded(hitObject, excludedUiElements))
            {
                return true;
            }
        }

        // If we only hit excluded objects, return false
        return false;
    }

    /// <summary>
    /// Returns true if the given object is the same as one of the excluded objects
    /// or is a child of one of them.
    /// </summary>
    private static bool IsExcluded(GameObject candidate, GameObject[] excludedUiElements)
    {
        foreach (GameObject excluded in excludedUiElements)
        {
            if (excluded == null) continue;

            // If candidate is the excluded object itself or is a descendant
            if (candidate.transform == excluded.transform ||
                candidate.transform.IsChildOf(excluded.transform))
            {
                return true;
            }
        }
        return false;
    }

    #endregion

    #region Color

    public static Color GetColorFromRgb255(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }

    public static Color SmoothLerpColor(Color c1, Color c2, float t)
    {
        t = Mathf.Clamp01(t); // Ensure t is in the range [0, 1]
        return Color.Lerp(c1, c2, SmoothStep(t));
    }

    // SmoothStep function for smoother interpolation
    private static float SmoothStep(float t)
    {
        return t * t * (3f - 2f * t);
    }

    #endregion

    #region Raycast

    /// <summary>
    /// Sorts the given array of raycast hits by distance, whereas the first hit (closest to source position) is first.
    /// </summary>
    public static void OrderRaycastHitsByDistance(RaycastHit[] hits)
    {
        System.Array.Sort(hits, (a, b) => (a.distance.CompareTo(b.distance)));
    }

    #endregion

    #region Network

    public static string GetLocalIPv4()
    {
        string localIP = string.Empty;
        string hostName = Dns.GetHostName();       // Get the name of the host running the application
        IPHostEntry hostEntry = Dns.GetHostEntry(hostName);

        foreach (var ip in hostEntry.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break; // If you only want the first IPv4
            }
        }

        return localIP;
    }

    #endregion
}
