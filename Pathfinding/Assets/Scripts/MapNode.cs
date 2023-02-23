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

    public Tilemap tilemap { get; private set; }
    public List<Tile> traversableTiles { get; private set; }


    public MapNode(Tilemap tilemap, List<Tile> walkableTiles, int tilesPerNode, Vector3Int initNode)
    {
        SetNode(tilemap, walkableTiles, tilesPerNode, initNode);
    }

    public MapNode(Tilemap tilemap, List<Tile> walkableTiles, int tilesPerNode, Vector3Int initNode, Vector3Int startPosition, Vector3Int endPosition)
    {
        SetNode(tilemap, walkableTiles, tilesPerNode, initNode);
        SetGHF(startPosition, endPosition);
    }

    private void SetNode(Tilemap _tilemap, List<Tile> walkableTiles, int tilesPerNode, Vector3Int initNode)
    {
        tilemap = _tilemap;
        traversableTiles = walkableTiles;

        for (int i = 0; i < tilesPerNode; i++)
        {
            for (int j = 0; j < tilesPerNode; j++)
            {
                position += new Vector3(initNode.x + i, initNode.y + j, initNode.z);
            }
        }

        position /= Mathf.Pow(tilesPerNode, 2);
    }

    public void SetGHF(Vector3Int startPosition, Vector3Int endPosition)
    {
        SetGHF(Vector3.Distance(position, startPosition), Vector3.Distance(position, endPosition));
    }

    public void SetGHF(float _g, float _h)
    {
        g = _g;
        h = _h;
        SetF();
    }

    private void SetF()
    {
        f = g + h;
    }

    public bool IsTraversable()
    {
        foreach (Vector3Int tile in tiles)
        {
            if (!traversableTiles.Contains(tilemap.GetTile<Tile>(tile)))
            {
                return false;
            }
        }

        return true;
    }
}
