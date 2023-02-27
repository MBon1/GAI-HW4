using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public bool isWayPointMap = false;
    public float hWeight = 1.0f;
    public bool useEuclidean = true;

    public int rows { get; protected set; }
    public int columns { get; protected set; }
    public MapNode[,] map { get; protected set; }
    public Dictionary<Vector3Int, MapNode> nodeByTile { get; protected set; } = new Dictionary<Vector3Int, MapNode>();
    public Dictionary<Vector3, MapNode> nodeByPosition { get; protected set; } = new Dictionary<Vector3, MapNode>();
    public Dictionary<MapNode, Vector2Int> nodeMapLookUp { get; protected set; } = new Dictionary<MapNode, Vector2Int>();

    // Pathfinding Variables
    public int tilesPerNode { get; protected set; } = 1;

    /* Creates a Map.
     * 
     *    Takes: int (map width)
     *           int (map height)
     *           int (number of tiles per node in this map)
     * Modifies: rows
     *           columns
     *           map
     *           tilesPerNode
     *  Returns: Map
     *  Expects: NONE
     */
    public Map(int width, int height, int _tilesPerNode)
    {
        rows = (int)Mathf.Ceil((float)height / _tilesPerNode);
        columns = (int)Mathf.Ceil((float)width / _tilesPerNode);

        map = new MapNode[rows, columns];

        tilesPerNode = _tilesPerNode;
    }

    /* Set a cell at the given row and column of the map with the given node.
     * Does not check if cell requested exists in the map.
     * 
     *    Takes: int
     *           int
     *           MapNode
     * Modifies: map
     *           nodeByTile
     *           nodeByPosition
     *           nodeByMapLookUp
     *  Returns: NONE
     *  Expects: NONE
     */
    public void SetCell(int row, int col, MapNode node)
    {
        RemoveCell(row, col);

        map[row, col] = node;

        foreach (Vector3Int tile in node.tiles)
        {
            nodeByTile.Add(tile, node);
        }

        nodeByPosition.Add(node.position, node);

        nodeMapLookUp.Add(node, new Vector2Int(row, col));
    }

    /* Remove a cell at the given row and column of the map.
     * 
     *    Takes: int
     *           int
     * Modifies: map
     *           nodeByTile
     *           nodeByPosition
     *           nodeByMapLookUp
     *  Returns: NONE
     *  Expects: NONE
     */
    public void RemoveCell(int row, int col)
    {
        if (map[row, col] != null)
        {
            MapNode mapNode = map[row, col];

            foreach (Vector3Int tile in mapNode.tiles)
            {
                nodeByTile.Remove(tile);
            }

            nodeByPosition.Remove(mapNode.position);

            nodeMapLookUp.Remove(mapNode);

            map[row, col] = null;
        }
    }

    /* Returns the neighboring nodes of a requested node.
     * If the node does not exist in the map, an empty list is returned.
     * 
     *    Takes: MapNode
     * Modifies: NONE
     *  Returns: List<MapNode> (list of neighboring nodes to the given node)
     *  Expects: NONE
     */
    public List<MapNode> getNeighbors(MapNode node)
    {
        if (!nodeMapLookUp.ContainsKey(node))
        {
            return new List<MapNode>();
        }

        Vector2Int posInMap = nodeMapLookUp[node];
        return getNeighbors(posInMap.x, posInMap.y);
    }

    /* Returns the neighboring nodes of a requested cell.
     * If the cell is outside of the map, an empty list is returned.
     * 
     *    Takes: int
     *           int
     * Modifies: NONE
     *  Returns: List<MapNode> (list of neighboring nodes to the given cell)
     *  Expects: NONE
     */
    public List<MapNode> getNeighbors(int row, int col)
    {
        List<MapNode> neighbors = new List<MapNode>();

        if (!isWayPointMap)
        {
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
        }
        else
        {
            foreach (MapNode node in nodeMapLookUp.Keys)
            {
                if (map[row, col] == node)
                {
                    continue;
                }

                // ASSIGNMENT : Do ray casting to determine neighbors
                // If ray cast does NOT hit something, add node to neighbors 
            }
        }

        return neighbors;
    }

    /* Sets h weight value for map and all of the nodes in the map.
     * 
     *    Takes: NONE
     * Modifies: hWeight
     *           map nodes' hWeight
     *  Returns: NONE
     *  Expects: Not currenty running an alogirthm.
     */
    public void SetNeighbors()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (map[i, j] != null)
                {
                    map[i, j].neighbors = getNeighbors(i, j);
                }
            }
        }
    }


    /* Sets h weight value for map and all of the nodes in the map.
     * 
     *    Takes: NONE
     * Modifies: hWeight
     *           map nodes' hWeight
     *  Returns: NONE
     *  Expects: Not currenty running an alogirthm.
     */
    public void SetHWeight(float _hWeight)
    {
        hWeight = _hWeight;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (map[i,j] != null)
                {
                    map[i, j].hWeight = hWeight;
                }
            }
        }
    }

     /* Sets useEuclidean value for map and all of the nodes in the map. 
      * If false is passed through, map and nodes will use Manhattan calcultaion.
      * 
      *    Takes: NONE
      * Modifies: useEuclidean
      *           map nodes' useEuclidean
      *  Returns: NONE
      *  Expects: Not currenty running an alogirthm.
      */
    public void SetUseEuclidean(bool _useEuclidean)
    {
        useEuclidean = _useEuclidean;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (map[i, j] != null)
                {
                    map[i, j].useEuclidean = useEuclidean;
                }
            }
        }
    }

    /* Removes all non way point nodes.
     * 
     *    Takes: NONE
     * Modifies: map
     *           nodeByTile
     *           nodeByPosition
     *           nodeByMapLookUp
     *  Returns: NONE
     *  Expects: NONE
     */
    public void RemoveNonWayPoints()
    {
        if (!isWayPointMap)
        {
            return;
        }

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (map[i, j] != null && !map[i, j].isWayPoint)
                {
                    RemoveCell(i, j);
                }
            }
        }
    }

    public void ResetPathFinding()
    {
        foreach(MapNode node in map)
        {
            node.ResetTraverseData();
        }
    }
}
