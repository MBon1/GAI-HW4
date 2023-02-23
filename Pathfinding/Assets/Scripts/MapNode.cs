using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapNode
{
    public List<Vector3Int> tiles { get; private set; } = new List<Vector3Int>();
    public Vector3 position { get; private set; } = Vector3.zero;

    public float g { get; private set; } = 0;  // Distance from starting node
    public float h { get; private set; } = 0;  // Distance from end node
    public float f { get; private set; } = 0;  // G cost + H cost

    public Node parent = null;

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

    /* Sets the G, H, and F values of this node for path finding.
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
        SetGHF(Vector3.Distance(position, startPosition), Vector3.Distance(position, endPosition));
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
        h = _h;
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

    /* Zeroes out G, H, and F values. Nulls out parent value.
     * 
     *    Takes: NONE
     * Modifies: g
     *           h
     *           f
     *           parent
     *  Returns: NONE
     *  Expects: NONE
     */
    public void ResetTraverseData()
    {
        SetGHF(0, 0);
        parent = null;
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
}
