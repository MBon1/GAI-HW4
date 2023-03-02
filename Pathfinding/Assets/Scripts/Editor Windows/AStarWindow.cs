using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class AStarWindow : EditorWindow
{
    [SerializeField] Text pos;

    [SerializeField] LabelledValue g;
    [SerializeField] LabelledValue h;
    [SerializeField] LabelledValue f;

    [SerializeField] LabelledValue timeScale;
    [SerializeField] LabelledValue pathFinding;

    [SerializeField] NumericInputField hWeight;

    [SerializeField] Dropdown heuristic;

    [SerializeField] TileMapLoader mapLoader;

    Vector2Int position = new Vector2Int(-1, -1);

    [Space(10)]
    MapNode currNode = null;
    float selectedAlpha = 0.75f;


    public override void SetTargetObject(GameObject obj)
    {
        this.target = obj;
        SetTargetProperty();
    }

    protected override void SetAllWidgetsActiveness(bool active)
    {
        pos.gameObject.SetActive(active);

        g.gameObject.SetActive(active);
        h.gameObject.SetActive(active);
        f.gameObject.SetActive(active);

        timeScale.gameObject.SetActive(active);
        pathFinding.gameObject.SetActive(active);

        hWeight.gameObject.transform.parent.parent.gameObject.SetActive(active);
        heuristic.gameObject.transform.parent.parent.gameObject.SetActive(active);
    }

    public override void SetTargetProperty()
    {
        //Time.timeScale = GetTimeScaleValue();
        if (mapLoader.map != null)
        {
            mapLoader.map.SetHWeight(hWeight.value);
        }
    }

    public void SetTargetPosition(Vector3Int pos)
    {
        if (currNode != null)
        {
            currNode.SetNodeAlpha(1);
        }

        if (mapLoader.map != null && mapLoader.map.nodeByTile.ContainsKey(pos))
        {
            MapNode node = mapLoader.map.nodeByTile[pos];
            position = mapLoader.map.nodeMapLookUp[node];

            currNode = node;
            currNode.SetNodeAlpha(selectedAlpha);
        }
        else
        {
            position = new Vector2Int(-1, -1);
            currNode = null;
        }
        DisplayValues();
    }

    public void DisplayValues()
    {
        if (mapLoader.map != null && 
            position.x >= 0 && position.x < mapLoader.map.rows &&
            position.y >= 0 && position.y < mapLoader.map.columns)
        {
            pos.text = "(" + position.x + " , " + position.y + ")";

            Color color = currNode.GetNodeColor();
            if (color.r == 1 && color.g == 1 && color.b == 1)
            {
                color = Color.black;
            }
            pos.color = new Color(color.r, color.g, color.b, 1);

            MapNode node = mapLoader.map.map[position.x, position.y];
            g.SetValue(node.g, true, "---", (node.g >= int.MaxValue));
            h.SetValue(node.h, true, "---", (node.h >= int.MaxValue));
            f.SetValue(node.f, true, "---", (node.f >= int.MaxValue));
        }
        else
        {
            pos.text = "???";
            pos.color = Color.black;
            g.SetValue(0, true, "---", true);
            h.SetValue(0, true, "---", true);
            f.SetValue(0, true, "---", true);
        }

        timeScale.SetValue(Time.timeScale, true);
    }

    public void SetDefaults()
    {
        if (mapLoader.map != null)
        {
            hWeight.SetValue(mapLoader.map.hWeight, false);
            SetAStarHeuristic();
            SetPathFindingMessage("...");
        }
    }

    public void SetAStarHeuristic()
    {
        if (mapLoader.map == null)
        {
            return;
        }

        if (heuristic.value == 0)
        {
            // Euclidean
            mapLoader.useEuclidean = true;
        }
        else
        {
            // Manhattan
            mapLoader.useEuclidean = false;
        }
        mapLoader.map.useEuclidean = mapLoader.useEuclidean;
    }

    public void SetPathFindingMessage(string text)
    {
        pathFinding.SetValue(text);
    }
}
