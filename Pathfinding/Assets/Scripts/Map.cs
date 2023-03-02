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
    public List<MapNode> wayPoints = new List<MapNode>();

    public LineRenderer lineRenderer;

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
    public Map(int width, int height, int _tilesPerNode, bool useWayPoints, LineRenderer lr)
    {
        rows = (int)Mathf.Ceil((float)height / _tilesPerNode);
        columns = (int)Mathf.Ceil((float)width / _tilesPerNode);

        map = new MapNode[rows, columns];

        tilesPerNode = _tilesPerNode;

        isWayPointMap = useWayPoints;

        lineRenderer = lr;
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

        if (node.isWayPoint)
        {
            wayPoints.Add(node);
        }
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

            if (mapNode.isWayPoint)
            {
                wayPoints.Remove(mapNode);
            }
        }
    }

    /* Returns a vector to offset a node position to be at the center of the node.
     * 
     *    Takes: NONE
     * Modifies: NONE
     *  Returns: Vector3 (offset value)
     *  Expects: NONE
     */
    public Vector3 GetNodeOffset()
    {
        return new Vector3(0.5f, -0.5f, 0);
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

        return node.neighbors;
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

            if (!map[row, col].IsTraversable())
            {
                return neighbors;
            }

            // Cardinal Rows
            int minRow = row - 1;
            if (minRow >= 0 && minRow < row)
            {
                neighbors.Add(map[minRow, col]);
            }
            /*else
            {
                minRow = -1;
            }*/

            int maxRow = row + 1;
            if (maxRow >= 0 && maxRow < rows)
            {
                neighbors.Add(map[maxRow, col]);
            }
            /*else
            {
                maxRow = -1;
            }*/

            // Columns
            int minCol = col - 1;
            if (minCol >= 0 && minCol < col)
            {
                neighbors.Add(map[row, minCol]);
            }
            /*else
            {
                minCol = -1;
            }*/

            int maxCol = col + 1;
            if (maxCol >= 0 && maxCol < columns)
            {
                neighbors.Add(map[row, maxCol]);
            }
            /*else
            {
                maxCol = -1;
            }*/

            // Diagonals    minRow  maxRow  minCol  maxCol
            /*if (minRow > -1 && minCol > -1)
            {
                neighbors.Add(map[minRow, minCol]);
            }

            if (minRow > -1 && maxCol > -1)
            {
                neighbors.Add(map[minRow, maxCol]);
            }

            if (maxRow > -1 && minCol > -1)
            {
                neighbors.Add(map[maxRow, minCol]);
            }

            if (maxRow > -1 && maxCol > -1)
            {
                neighbors.Add(map[maxRow, maxCol]);
            }*/
        }
        else
        {
            foreach (MapNode candidate in wayPoints)
            {
                MapNode current = map[row, col];
                
                if (current.Equals(candidate))
                {
                    continue;
                }

                // Do ray casting to determine neighbors
                // If ray cast does NOT hit something, add node to neighbors

                Vector3 offset = GetNodeOffset();

                Vector3 posA = new Vector3(current.position.y, current.position.x * -1, current.position.z) + offset;
                Vector3 posB = new Vector3(candidate.position.y, candidate.position.x * -1, candidate.position.z) + offset; 

                Vector2 direction = new Vector2((posB.x - posA.x), (posB.y - posA.y));
                float distance = Vector3.Distance(posA, posB);

                RaycastHit2D hit = Physics2D.Raycast(posA, direction, distance);

                if (hit.collider == null)
                {
                    neighbors.Add(candidate);
                    Debug.DrawRay(posA, direction, Color.red, 1);
                }
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
        if (isWayPointMap)
        {
            foreach (MapNode node in wayPoints)
            {
                Vector2Int pos = nodeMapLookUp[node];
                node.neighbors = getNeighbors(pos.x, pos.y);
            }
        }
        else
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
        Debug.Log("New H Weight: " + hWeight);
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
            
            if (wayPoints.Contains(node) && node.isWayPoint == false)
            {
                wayPoints.Remove(node);
            }
        }
        lineRenderer.positionCount = 0;
    }

    public void AddPointToLineRenderer(Vector3 pos)
    {
        lineRenderer.positionCount++;

        Vector3 offset = GetNodeOffset();

        Vector3 newPos = new Vector3(pos.y, pos.x * -1, pos.z) + offset;

        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPos);
    }
}
