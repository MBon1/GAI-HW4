using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    List<Vector3Int> tilesInNode = new List<Vector3Int>();
    public Node parent { get; private set; } = null;

    public float g { get; private set; } = 0;  // Distance from starting node
    public float h { get; private set; } = 0;  // Distance from end node
    public float f { get; private set; } = 0;  // G cost + H cost

    Node (Vector3Int position, Vector3Int startPosition, Vector3Int endPosition, Node node = null)
    {
        g = Vector3Int.Distance(position, startPosition);
        h = Vector3Int.Distance(position, endPosition);
        SetF();

        parent = node;

        tilesInNode.Add(position);
    }

    public void UpdateGHF(float newG, float newH)
    {
        g = newG;
        h = newH;
        SetF();
    }

    private void SetF()
    {
        f = g + h;
    }

    public List<Vector3Int> GetTilesInNode()
    {
        return tilesInNode;
    }
}
