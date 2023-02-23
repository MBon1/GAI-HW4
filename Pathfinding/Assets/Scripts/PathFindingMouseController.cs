using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFindingMouseController : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] TileMapLoader mapLoader;

    [Header("Path Finding Positions")]
    [SerializeField] Vector3Int startPosition = new Vector3Int();
    MapNode startNode = null;

    [SerializeField] Vector3Int endPosition = new Vector3Int();
    MapNode endNode = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3Int pos = GetTilePosition();

        if (Input.GetKeyDown(KeyCode.KeypadEnter))  // Perform A*
        {

        }
        else if (Input.GetMouseButtonDown(0))    // Set Start Position
        {
            if (mapLoader.map != null && mapLoader.map.nodeByTile.ContainsKey(pos))
            {
                startPosition = pos;

                if (startNode != null)
                {
                    startNode.SetNodeColor(MapNode.TraverseColor.None);
                }

                startNode = mapLoader.map.nodeByTile[startPosition];
                startNode.SetNodeColor(MapNode.TraverseColor.Start);
            }
        } 
        else if (Input.GetMouseButtonDown(1))     // Set End Position
        {
            if (mapLoader.map != null && mapLoader.map.nodeByTile.ContainsKey(pos))
            {
                endPosition = pos;

                if (endNode != null)
                {
                    endNode.SetNodeColor(MapNode.TraverseColor.None);
                }

                endNode = mapLoader.map.nodeByTile[endPosition];
                endNode.SetNodeColor(MapNode.TraverseColor.End);
            }
        }

    }

    Vector3Int GetTilePosition()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return tilemap.WorldToCell(worldPoint);
    }

    public void Reset()
    {
        startNode = null;
        endNode = null;
    }
}
