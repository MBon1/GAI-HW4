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

    public Map(int width, int height, int tilesPerNode)
    {
        rows = (int)Mathf.Ceil((float)height / tilesPerNode);
        columns = (int)Mathf.Ceil((float)width / tilesPerNode);

        map = new MapNode[rows, columns];
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

    public List<MapNode> GetNeighboringNodes(MapNode node)
    {
        if (!nodeMapLookUp.ContainsKey(node))
        {
            return new List<MapNode>();
        }

        Vector2Int posInMap = nodeMapLookUp[node];
        return GetNeighboringNodes(posInMap.x, posInMap.y);
    }

    public List<MapNode> GetNeighboringNodes(int row, int col)
    {
        List<MapNode> neighbors = new List<MapNode>();

        if (row >= rows || row < 0 ||
            col >= columns || col < 0)
        {
            return neighbors;
        }

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

    public class MapNode
    {
        public List<Vector3Int> tiles { get; private set; } = new List<Vector3Int>();
        public Vector3 position { get; private set; } = Vector3.zero;

        public float g { get; private set; } = 0;  // Distance from starting node
        public float h { get; private set; } = 0;  // Distance from end node
        public float f { get; private set; } = 0;  // G cost + H cost

        public Node parent = null;

        public MapNode(int tilesPerNode, Vector3Int initNode)
        {
            SetNode(tilesPerNode, initNode);
        }

        public MapNode(int tilesPerNode, Vector3Int initNode, Vector3Int startPosition, Vector3Int endPosition)
        {
            SetNode(tilesPerNode, initNode);
            SetGHF(Vector3.Distance(position, startPosition), Vector3.Distance(position, endPosition));
        }

        private void SetNode(int tilesPerNode, Vector3Int initNode)
        {
            for (int i = 0; i < tilesPerNode; i++)
            {
                for (int j = 0; j < tilesPerNode; j++)
                {
                    position += new Vector3(initNode.x + i, initNode.y + j, initNode.z);
                }
            }

            position /= Mathf.Pow(tilesPerNode, 2);
        }

        public void SetGHF(float g, float h)
        {
            this.g = g;
            this.h = h;
            SetF();
        }

        private void SetF()
        {
            f = g + h;
        }
    }
}
