using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class TileMapLoader : MonoBehaviour
{
    public string mapFileName = "";
    public Maps mapFile = Maps.NONE;
    public bool isWayPointMap = false;
    public bool useEuclidean = true;

    public Tilemap tileMap;
    public Tilemap nontraversableTileMap;

    public TileData boundaryTile;
    public TileData unpassableTile;
    public TileData walkableTile;

    Dictionary<Tile, TileData> tileLookUp = new Dictionary<Tile, TileData>();
    Dictionary<char, TileData> tileKeyLookUp = new Dictionary<char, TileData>();

    public int tilesPerNode = 1;
    public Map map { get; private set; } = null;

    [Space(10)]
    [SerializeField] PathFindingMouseController mouseController;
    [SerializeField] AStarWindow editorWindow;
    [SerializeField] LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        StoreTileData(boundaryTile);
        StoreTileData(unpassableTile);
        StoreTileData(walkableTile);
    }

    /* Record tile data into look up tables.
     * 
     *    Takes: TileData
     * Modifies: tileLookUp
     *           tileKeyLookUp
     *  Returns: NONE
     *  Expects: NONE
     */
    private void StoreTileData(TileData tileData)
    {
        tileLookUp.Add(tileData.tile, tileData);
        tileKeyLookUp.Add(tileData.key, tileData);
    }

    /* Loads a map from a selected file. 
     * 
     *    Takes: NONE
     * Modifies: tileMap
     *           map
     *  Returns: NONE
     *  Expects: NONE
     */
    [ContextMenu("Load Map from File")]
    public void LoadTileMap()
    {
        // Get the map to load
        string mapName = mapFileName;
        if (mapFile == Maps.AR0011SR)
            mapName = "AR0011SR";
        if (mapFile == Maps.arena2)
            mapName = "arena2";
        if (mapFile == Maps.hrt201n)
            mapName = "hrt201n";
        if (mapFile == Maps.lak104d)
            mapName = "lak104d";
        if (mapFile == Maps.test)
            mapName = "test";

        string mapFilePath = "Assets/Maps/" + mapName + ".map";

        if (!File.Exists(mapFilePath))
        {
            Debug.LogError("FILE DOES NOT EXIST: " + mapFilePath);
            return;
        }
        else
        {
            Debug.Log(mapFilePath);
        }

        string height = "height ";
        string width = "width ";

        int rows = 0;
        int columns = 0;

        StreamReader reader = new StreamReader(mapFilePath);
        string line = reader.ReadLine();
        while (line != null && !line.Equals("map"))
        {
            line = line.ToLower();
            //Debug.Log(line);
            if (line.Contains(height))
            {
                string heightVal = line.Substring(line.IndexOf(height) + height.Length);
                //Debug.Log(heightVal);
                if (!int.TryParse(heightVal, out rows))
                {
                    Debug.LogError("INVALID HEIGHT VALUE: " + heightVal);
                    return;
                }
            }

            if (line.Contains(width))
            {
                string widthVal = line.Substring(line.IndexOf(width) + width.Length);
                //Debug.Log(widthVal);
                if (!int.TryParse(widthVal, out columns))
                {
                    Debug.LogError("INVALID WIDTH VALUE: " + widthVal);
                    return;
                }
            }

            line = reader.ReadLine();
        }

        // Reset Maps
        map = new Map(columns, rows, tilesPerNode, isWayPointMap, lineRenderer);
        tileMap.ClearAllTiles();
        nontraversableTileMap.ClearAllTiles();
        lineRenderer.positionCount = 0;

        // Add new tiles
        for (int i = 0; i < rows; i++)
        {
            line = reader.ReadLine();

            for (int j = 0; j < columns; j++)
            {
                Vector3Int tilePos = new Vector3Int(i, j, 0);   // If Tilemap is not translated and rotated : Vector3Int(j, -i, 0)
                if (tileKeyLookUp.ContainsKey(line[j]))
                {
                    //Debug.Log(line[j]);
                    Tile tile = tileKeyLookUp[line[j]].tile;
                    tileMap.SetTile(tilePos, tile);
                    // If tile is not traversable, add to nontraversable tilemap
                    if (tile != walkableTile.tile)
                        nontraversableTileMap.SetTile(tilePos, tile);
                }
                else
                {
                    Debug.Log("UNKNOWN KEY : " + line[j]);
                }

                MapNode node = new MapNode(tileMap, walkableTile.tile, map.tilesPerNode, tilePos);
                node.useEuclidean = useEuclidean;
                if (i % tilesPerNode == 0 && j % tilesPerNode == 0)
                {
                    Vector2Int mapPos = new Vector2Int(Mathf.FloorToInt(i / (float)tilesPerNode), Mathf.FloorToInt(j / (float)tilesPerNode));
                    map.SetCell(mapPos.x, mapPos.y, node);
                    //Debug.Log("Node (" + mapPos.x + ", " + mapPos.y + ") @ (" + tilePos.x + ", " + tilePos.y + ") : ");
                }
            }
        }
        
        // If waypoint, determine which nodes are way points
        if (map.isWayPointMap)
        {
            // COMPLETE (hungj2) : SET WHICH NODES ARE WAY POINTS

            // If a node is a waypoint, set map node to way point color
            // mapNode.SetNodeColor(TraverseColor.WayPoint);

            for (int r = 1; r < this.map.rows - 1; r++)
            {
                for (int c = 1; c < this.map.columns - 1; c++)
                {
                    MapNode curr = this.map.map[r, c];

                    if (!curr.IsTraversable())
                    {
                        continue;
                    }

                    MapNode e = this.map.map[r + 1, c];
                    MapNode ne = this.map.map[r + 1, c - 1];
                    MapNode n = this.map.map[r, c - 1];
                    MapNode nw = this.map.map[r - 1, c - 1];
                    MapNode w = this.map.map[r - 1, c];
                    MapNode sw = this.map.map[r - 1, c + 1];
                    MapNode s = this.map.map[r, c + 1];
                    MapNode se = this.map.map[r + 1, c + 1];

                    if ((!ne.IsTraversable() && n.IsTraversable() && e.IsTraversable())
                        || (!nw.IsTraversable() && n.IsTraversable() && w.IsTraversable())
                        || (!sw.IsTraversable() && s.IsTraversable() && w.IsTraversable())
                        || (!se.IsTraversable() && s.IsTraversable() && e.IsTraversable()))
                    {
                        curr.SetNodeColor(MapNode.TraverseColor.WayPoint);
                        curr.isWayPoint = true;
                        map.wayPoints.Add(curr);
                    }
                    else
                    {
                        // No need to set the color of a tile that's going to get nuked...
                        curr.isWayPoint = false;
                    }
                }
            }

            // Remove all non-way-points
            //map.RemoveNonWayPoints();
        }

        // Set Neighbors
        map.SetNeighbors();

        // Set h weight
        map.SetHWeight(map.hWeight);
        editorWindow.SetDefaults();

        mouseController.Reset();
    }

    // Old function to check if a node is traversable
    public bool IsTraversable(Vector3Int position)
    {
        Tile tile = tileMap.GetTile<Tile>(position);

        if (tile == null || tile == boundaryTile.tile || tile == unpassableTile.tile)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    [System.Serializable]
    public struct TileData
    {
        public char key;
        public Tile tile;
        //public WalkableAreas[] contactPoints;
    }

    public enum Maps
    {
        AR0011SR,
        arena2,
        hrt201n,
        lak104d,
        test,
        NONE
    }
}
