using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFindingMouseController : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;

    [Header("Path Finding Positions")]
    [SerializeField] Vector3Int startPosition = new Vector3Int();
    [SerializeField] Vector3Int endPosition = new Vector3Int();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))  // Perform A*
        {

        }
        else if (Input.GetMouseButtonDown(0))    // Set Start Position
        {
            startPosition = GetTilePosition();
        } 
        else if (Input.GetMouseButtonDown(1))     // Set End Position
        {
            endPosition = GetTilePosition();
        }

    }

    Vector3Int GetTilePosition()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return tilemap.WorldToCell(worldPoint);
    }
}
