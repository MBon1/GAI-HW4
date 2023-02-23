using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public int rows { get; private set; }
    public int columns { get; private set; }
    public MapNode[,] map { get; private set; }
    public Dictionary<Vector3Int, MapNode> nodeByTile { get; private set; } = new Dictionary<Vector3Int, MapNode>();
    public Dictionary<Vector3, MapNode> nodeByPosition { get; private set; } = new Dictionary<Vector3, MapNode>();
    public Dictionary<MapNode, Vector2Int> nodeMapLookUp { get; private set; } = new Dictionary<MapNode, Vector2Int>();

    // Pathfinding Variables
    public static int tilesPerNode = 1;


    public Map(int width, int height, int _tilesPerNode)
    {
        rows = (int)Mathf.Ceil((float)height / _tilesPerNode);
        columns = (int)Mathf.Ceil((float)width / _tilesPerNode);

        map = new MapNode[rows, columns];

        tilesPerNode = _tilesPerNode;
    }

    public void SetCell(int row, int col, MapNode node)
    {
        if (map[row, col] != null)
        {
            MapNode mapNode = map[row, col];

            foreach (Vector3Int tile in mapNode.tiles)
            {
                nodeByTile.Remove(tile);
            }

            nodeByPosition.Remove(mapNode.position);

            nodeMapLookUp.Remove(node);
        }

        map[row, col] = node;

        foreach (Vector3Int tile in node.tiles)
        {
            nodeByTile.Add(tile, node);
        }

        nodeByPosition.Add(node.position, node);

        nodeMapLookUp.Add(node, new Vector2Int(row, col));
    }

    public List<MapNode> getNeighbors(MapNode node)
    {
        if (!nodeMapLookUp.ContainsKey(node))
        {
            return new List<MapNode>();
        }

        Vector2Int posInMap = nodeMapLookUp[node];
        return getNeighbors(posInMap.x, posInMap.y);
    }

    public List<MapNode> getNeighbors(int row, int col)
    {
        List<MapNode> neighbors = new List<MapNode>();

        if (row >= rows || row < 0 ||
            col >= columns || col < 0)
        {
            return neighbors;
        }

        // Rows
        int minRow = row - 1;
        if (minRow >= 0)
        {
            neighbors.Add(map[minRow, col]);
        }

        int maxRow = row + 1;
        if (maxRow < rows)
        {
            neighbors.Add(map[maxRow, col]);
        }

        // Columns
        int minCol = row - 1;
        if (minCol >= 0)
        {
            neighbors.Add(map[row, minCol]);
        }

        int maxCol = row + 1;
        if (maxCol < columns)
        {
            neighbors.Add(map[row, maxCol]);
        }

        return neighbors;
    }
}
