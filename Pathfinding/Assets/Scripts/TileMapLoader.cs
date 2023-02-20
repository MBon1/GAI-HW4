using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class TileMapLoader : MonoBehaviour
{
    public string mapFileName = "";
    
    public  Tilemap tileMap;

    public TileData boundaryTile;
    public TileData unpassableTile;
    public TileData walkableTile;

    Dictionary<Tile, TileData> tileLookUp = new Dictionary<Tile, TileData>();
    Dictionary<char, TileData> tileKeyLookUp = new Dictionary<char, TileData>();

    // Start is called before the first frame update
    void Start()
    {
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
        string mapFilePath = "Assets/Maps/" + mapFileName + ".map";

        if (!File.Exists(mapFilePath))
        {
            Debug.LogError("FILE DOES NOT EXIST: " + mapFilePath);
            return;
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

            for (int i = 0; i < rows; i++)
            {
                line = reader.ReadLine();

                for (int j = 0; j < columns; j++)
                {
                    Tile tile = tileKeyLookUp[line[j]].tile;
                    tileMap.SetTile(new Vector3Int(i, j, 0), tile);
                }
            }
        }
    }

    [System.Serializable]
    public struct TileData
    {
        public char key;
        public Tile tile;
        //public WalkableAreas[] contactPoints;
    }
}
