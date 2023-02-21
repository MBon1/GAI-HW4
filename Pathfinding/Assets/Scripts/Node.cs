using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public static Dictionary<Vector3, Node> nodesInMap;
    public static int nodeSize = 1;
    public static Vector3Int startPos;
    public static Vector3Int endPos;

    public Vector3Int position;
    List<Vector3Int> tilesInNode = new List<Vector3Int>();
    public Node parent = null;

    public float g { get; private set; } = 0;  // Distance from starting node
    public float h { get; private set; } = 0;  // Distance from end node
    public float f { get; private set; } = 0;  // G cost + H cost

    Node (Vector3Int position, Vector3Int startPosition, Vector3Int endPosition, Node node = null)
    {
        this.position = position;
        startPos = startPosition;
        endPos = endPosition;

        g = Vector3Int.Distance(position, startPosition);
        h = Vector3Int.Distance(position, endPosition);
        SetF();

        parent = node;

        tilesInNode.Add(position);

        nodesInMap.Add(position, this);
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


    // Get Neighboring Nodes;   Need dictionary to track all nodes by position
    public List<Node> GetNeighboringNodes()
    {
        List<Node> neighbors = new List<Node>();
        neighbors.Add(GetNodeAtPosition(new Vector3Int(position.x + nodeSize, position.y, position.z)));    // Right Neighbor
        neighbors.Add(GetNodeAtPosition(new Vector3Int(position.x - nodeSize, position.y, position.z)));    // Left Neighbor
        neighbors.Add(GetNodeAtPosition(new Vector3Int(position.x, position.y + nodeSize, position.z)));    // Lower Neighbor
        neighbors.Add(GetNodeAtPosition(new Vector3Int(position.x, position.y - nodeSize, position.z)));    // Upper Neighbor

        return neighbors;
    }

    public Node GetNodeAtPosition(Vector3Int pos)
    {
        if (nodesInMap.ContainsKey(pos))
        {
            return nodesInMap[pos];     // Maybe need to update some values?
        }
        else
        {
            Node node = new Node(pos, startPos, endPos, this);
            nodesInMap.Add(pos, node);
            return node;
        }
    }
}
