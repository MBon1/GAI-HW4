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


    public MapNode(int tilesPerNode, Vector3Int initNode)
    {
        SetNode(tilesPerNode, initNode);
    }

    public MapNode(int tilesPerNode, Vector3Int initNode, Vector3Int startPosition, Vector3Int endPosition)
    {
        SetNode(tilesPerNode, initNode);
        SetGHF(startPosition, endPosition);
    }

    private void SetNode(int tilesPerNode, Vector3Int initNode)
    {
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

    public void ResetTraverseData()
    {
        SetGHF(0, 0);
        parent = null;
    }

    public bool IsTraversable(Tilemap tilemap, Tile traversableTile)
    {
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
