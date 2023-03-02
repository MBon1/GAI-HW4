using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapNode
{
    public List<Vector3Int> tiles { get; private set; } = new List<Vector3Int>();
    public Vector3 position { get; private set; } = Vector3.zero;

    public bool isWayPoint = false;

    public float hWeight = 1.0f;

    public bool useEuclidean = true;

    public float g { get; private set; } = int.MaxValue;  // Distance from starting node
    public float h { get; private set; } = int.MaxValue;  // Distance from end node
    public float f { get; private set; } = int.MaxValue;  // G cost + H cost

    public List<MapNode> neighbors;                       // List of neighboring nodes

    public MapNode parent = null;

    private static Tilemap tilemap;
    private static Tile traversableTile;

    /* Creates a MapNode. Does NOT set pathfinding variables. MapNode can cover multiple tiles, recording
     * tilesPerNode by tilesPerNode tiles starting with initNode and adding the node to the right and below it.
     * 
     *    Takes: Tilemap (tile map node is representing)
     *           Tile (tile that can be traversed on)
     *           int (number of tiles this node will cover)
     *           Vector3Int (initial node for the MapNode to represent)
     * Modifies: tilemap
     *           traversableTile
     *           tiles
     *           position
     *  Returns: MapNode
     *  Expects: NONE
     */
    public MapNode(Tilemap _tilemap, Tile _traversableTile, int tilesPerNode, Vector3Int initNode)
    {
        SetNode(_tilemap, _traversableTile, tilesPerNode, initNode);
    }

    /* Creates a MapNode. DOES set pathfinding variables. MapNode can cover multiple tiles, recording
     * tilesPerNode by tilesPerNode tiles starting with initNode and adding the node to the right and below it.
     * 
     *    Takes: Tilemap (tile map node is representing)
     *           Tile (tile that can be traversed on)
     *           int (number of tiles this node will cover)
     *           Vector3Int (initial node for the MapNode to represent)
     *           Vector3Int (start position node of the path)
     *           Vector3Int (end position node of the path)
     * Modifies: tilemap
     *           traversableTile
     *           tiles
     *           position
     *  Returns: MapNode
     *  Expects: NONE
     */
    public MapNode(Tilemap _tilemap, Tile _traversableTile, int tilesPerNode, Vector3Int initNode, Vector3Int startPosition, Vector3Int endPosition)
    {
        SetNode(_tilemap, _traversableTile, tilesPerNode, initNode);
        SetGHF(startPosition, endPosition);
    }

    /* Sets MapNode variables. For MapNode creation.
     * 
     *    Takes: Tilemap (tile map node is representing)
     *           Tile (tile that can be traversed on)
     *           int (number of tiles this node will cover)
     *           Vector3Int (initial node for the MapNode to represent)
     * Modifies: tilemap
     *           traversableTile
     *           tiles
     *           position
     *  Returns: NONE
     *  Expects: NONE
     */
    private void SetNode(Tilemap _tilemap, Tile _traversableTile, int tilesPerNode, Vector3Int initNode)
    {
        tilemap = _tilemap;
        traversableTile = _traversableTile;

        for (int i = 0; i < tilesPerNode; i++)
        {
            for (int j = 0; j < tilesPerNode; j++)
            {
                Vector3Int tile = new Vector3Int(initNode.x + i, initNode.y + j, 0);
                tiles.Add(tile);
                position += tile;
            }
        }

        position /= Mathf.Pow(tilesPerNode, 2);
    }

    /* Sets the G, H, and F values of this node for path finding. Calculates using either Euclidean or Manhattan distance.
     * 
     *    Takes: Vector3Int (start position node of the path)
     *           Vector3Int (end position node of the path)
     * Modifies: g
     *           h
     *           f
     *  Returns: NONE
     *  Expects: NONE
     */
    public void SetGHF(Vector3Int startPosition, Vector3Int endPosition)
    {
        if (useEuclidean)
        {
            SetGHF(Vector3.Distance(position, startPosition), Vector3.Distance(position, endPosition));
        }
        else
        {
            SetGHF(ManhattanDistance(position, startPosition), ManhattanDistance(position, endPosition));
        }
    }

    /* Sets the G, H, and F values of this node for path finding.
     * 
     *    Takes: float (new g value)
     *           float (new h value)
     * Modifies: g
     *           h
     *           f
     *  Returns: NONE
     *  Expects: NONE
     */
    public void SetGHF(float _g, float _h)
    {
        g = _g;
        h = _h * hWeight;
        SetF();
    }

    /* Sets the F value of this node for path finding.
     * 
     *    Takes: NONE
     * Modifies: f
     *  Returns: NONE
     *  Expects: NONE
     */
    private void SetF()
    {
        f = g + h;
    }

    /* Sets G, H, and F values to max int value. 
     * Nulls out parent value. 
     * Sets color to None.
     * 
     *    Takes: NONE
     * Modifies: g
     *           h
     *           f
     *           parent
     *           color of tiles
     *  Returns: NONE
     *  Expects: NONE
     */
    public void ResetTraverseData()
    {
        g = int.MaxValue;
        h = int.MaxValue;
        f = int.MaxValue;
        parent = null;
        if (isWayPoint)
        {
            SetNodeColor(TraverseColor.WayPoint);
        }
        else
        {
            SetNodeColor(TraverseColor.None);
        }
    }

    /* Checks if the tile is traversable.
     * 
     *    Takes: NONE
     * Modifies: NONE
     *  Returns: bool
     *  Expects: NONE
     */
    public bool IsTraversable()
    {
        // Check if one of the tiles in the node is not traversable
        foreach(Vector3Int tile in tiles)
        {
            Tile t = tilemap.GetTile<Tile>(tile);
            if (t == null || t != traversableTile)
            {
                return false;
            }
        }
        return true;
    }

    public void SetNodeColor(TraverseColor color)
    {
        Color c;

        if (color == TraverseColor.Open)
        {
            c = new Color(0, 110f / 255f, 1, 1);    // Blue
        }
        else if (color == TraverseColor.Closed)
        {
            c = new Color(1, 12f / 255f, 0, 1);     // Red
        }
        else if (color == TraverseColor.Start)
        {
            c = new Color(247f / 255f, 176 / 255f, 17f / 255f, 1);    // Dark Yellow
        }
        else if (color == TraverseColor.End)
        {
            c = new Color(1, 0, 217f / 255f, 1);    // Magenta
        }
        else if (color == TraverseColor.WayPoint)
        {
            c = new Color(152f / 255f, 0, 1, 1);    // Purple
        }
        else
        {
            c = Color.white;
        }

        c.a = GetNodeColorAlpha();

        SetNodeColor(c);
    }

    /* Changes the color of all tiles represented by this node.
     * 
     *    Takes: Color
     * Modifies: colors of tiles
     *  Returns: NONE
     *  Expects: NONE
     */
    public void SetNodeColor(Color color)
    {
        foreach (Vector3Int tile in tiles)
        {
            tilemap.SetTileFlags(tile, TileFlags.None);
            tilemap.SetColor(tile, color);
        }
    }

    /* Changes the alpha of the color of all tiles represented by this node.
     * 
     *    Takes: float
     * Modifies: alpha of the colors of tiles
     *  Returns: NONE
     *  Expects: NONE
     */
    public void SetNodeAlpha(float alpha)
    {
        foreach (Vector3Int tile in tiles)
        {
            tilemap.SetTileFlags(tile, TileFlags.None);
            Color c = tilemap.GetColor(tile);
            tilemap.SetColor(tile, new Color(c.r, c.g, c.b, alpha));
        }
    }

    /* Returns the color of this node.
     * 
     *    Takes: NONE
     * Modifies: NONE
     *  Returns: Color
     *  Expects: Node represents at least 1 tile
     *           All tiles represented are the same color
     */
    public Color GetNodeColor()
    {
        return tilemap.GetColor(tiles[0]);
    }

    /* Returns the color of this node.
     * 
     *    Takes: NONE
     * Modifies: NONE
     *  Returns: Color
     *  Expects: Node represents at least 1 tile
     *           All tiles represented are the same color
     */
    public float GetNodeColorAlpha()
    {
        return tilemap.GetColor(tiles[0]).a;
    }

    /* Calculates the Manhattan distance between two points.
     * 
     *    Takes: Vector3 (point a)
     *           Vector3 (point b)
     * Modifies: NONE
     *  Returns: float (Manhattan distance)
     *  Expects: NONE
     */
    private float ManhattanDistance(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    public enum TraverseColor
    {
        Open,
        Closed, 
        Start,
        End,
        WayPoint, // For Way points
        None
    }
}
