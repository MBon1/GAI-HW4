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

    AStar astar = new AStar();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (astar.isRunning)
        {
            return;
        }

        Vector3Int pos = GetTilePosition();

        if (Input.GetMouseButtonDown(0))    // Set Start Position
        {
            if (mapLoader.map != null && mapLoader.map.nodeByTile.ContainsKey(pos))
            {
                startPosition = pos;

                if (startNode != null)
                {
                    if (startNode.isWayPoint)
                    {
                        startNode.SetNodeColor(MapNode.TraverseColor.WayPoint);
                    }
                    else
                    {
                        startNode.SetNodeColor(MapNode.TraverseColor.None);
                        mapLoader.map.wayPoints.Remove(startNode);
                    }
                }

                startNode = mapLoader.map.nodeByTile[startPosition];
                
                startNode.SetNodeColor(MapNode.TraverseColor.Start);

                // If we're using waypoints, redetermine all way point neighbors
                if (mapLoader.map.isWayPointMap)
                {
                    mapLoader.map.SetNeighbors();
                }
            }
        } 
        else if (Input.GetMouseButtonDown(1))     // Set End Position
        {
            if (mapLoader.map != null && mapLoader.map.nodeByTile.ContainsKey(pos))
            {
                endPosition = pos;

                if (endNode != null)
                {
                    if (endNode.isWayPoint)
                    {
                        endNode.SetNodeColor(MapNode.TraverseColor.WayPoint);
                    }
                    else
                    {
                        endNode.SetNodeColor(MapNode.TraverseColor.None);
                        mapLoader.map.wayPoints.Remove(endNode);
                    }
                }

                endNode = mapLoader.map.nodeByTile[endPosition];
                endNode.SetNodeColor(MapNode.TraverseColor.End);

                // If we're using waypoints, redetermine all way point neighbors
                // If we're using waypoints, redetermine all way point neighbors
                if (mapLoader.map.isWayPointMap)
                {
                    mapLoader.map.SetNeighbors();
                }

                // Perform A*
                StartCoroutine(astar.AStarCoroutine(mapLoader.map, startNode, endNode));
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
