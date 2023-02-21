using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class TileMapLoader : MonoBehaviour
{
    public string mapFileName = "";
    public Maps map = Maps.NONE;

    public Tilemap tileMap;

    public TileData boundaryTile;
    public TileData unpassableTile;
    public TileData walkableTile;

    Dictionary<Tile, TileData> tileLookUp = new Dictionary<Tile, TileData>();
    Dictionary<char, TileData> tileKeyLookUp = new Dictionary<char, TileData>();

    // Start is called before the first frame update
    void Start()
    {
        StoreTileData(boundaryTile);
        StoreTileData(unpassableTile);
        StoreTileData(walkableTile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StoreTileData(TileData tileData)
    {
        tileLookUp.Add(tileData.tile, tileData);
        tileKeyLookUp.Add(tileData.key, tileData);
    }

    [ContextMenu("Load Map from File")]
    public void LoadTileMap()
    {
        string mapName = mapFileName;
        if (map == Maps.AR0011SR)
            mapName = "AR0011SR";
        if (map == Maps.arena2)
            mapName = "arena2";
        if (map == Maps.hrt201n)
            mapName = "hrt201n";
        if (map == Maps.lak104d)
            mapName = "lak104d";

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
            Debug.Log(line);
            if (line.Contains(height))
            {
                string heightVal = line.Substring(line.IndexOf(height) + height.Length);
                Debug.Log(heightVal);
                if (!int.TryParse(heightVal, out rows))
                {
                    Debug.LogError("INVALID HEIGHT VALUE: " + heightVal);
                    return;
                }
            }

            if (line.Contains(width))
            {
                string widthVal = line.Substring(line.IndexOf(width) + width.Length);
                Debug.Log(widthVal);
                if (!int.TryParse(widthVal, out columns))
                {
                    Debug.LogError("INVALID WIDTH VALUE: " + widthVal);
                    return;
                }
            }

            line = reader.ReadLine();
        }

        tileMap.ClearAllTiles();

        for (int i = 0; i < rows; i++)
        {
            line = reader.ReadLine();

            for (int j = 0; j < columns; j++)
            {
                if (tileKeyLookUp.ContainsKey(line[j]))
                {
                    //Debug.Log(line[j]);
                    Tile tile = tileKeyLookUp[line[j]].tile;
                    tileMap.SetTile(new Vector3Int(j, -i, 0), tile);
                }
                else
                {
                    Debug.Log("UNKNOWN KEY : " + line[j]);
                }
            }
        }
    }

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
        NONE
    }
}
