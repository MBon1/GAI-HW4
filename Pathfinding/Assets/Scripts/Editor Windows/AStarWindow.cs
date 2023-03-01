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

    [SerializeField] NumericInputField hWeight;

    [SerializeField] TileMapLoader mapLoader;

    Vector3Int position = new Vector3Int(-1, -1, 0);


    public override void SetTargetObject(GameObject obj)
    {
        this.target = obj;
        SetTargetProperty();
    }

    protected override void SetAllWidgetsActiveness(bool active)
    {
        g.gameObject.SetActive(active);
        h.gameObject.SetActive(active);
        f.gameObject.SetActive(active);

        hWeight.gameObject.transform.parent.parent.gameObject.SetActive(active);
    }

    public override void SetTargetProperty()
    {
        //Time.timeScale = GetTimeScaleValue();
        if (mapLoader.map != null)
        {
            mapLoader.map.hWeight = hWeight.value;
        }
    }

    public void SetTargetPosition(Vector3Int pos)
    {
        position = pos;
        DisplayValues();
    }

    public void DisplayValues()
    {
        if (mapLoader.map != null && 
            position.x >= 0 && position.x < mapLoader.map.rows &&
            position.y >= 0 && position.y < mapLoader.map.columns)
        {
            pos.text = "(" + position.x + " , " + position.y + ")";
            MapNode node = mapLoader.map.map[position.x, position.y];
            g.SetValue(node.g, true);
            h.SetValue(node.h, true);
            f.SetValue(node.f, true);
        }
        else
        {
            pos.text = "???";
            g.SetValue(0, true);
            h.SetValue(0, true);
            f.SetValue(0, true);
        }

        if (mapLoader.map != null)
        {
            hWeight.SetValue(mapLoader.map.hWeight, false);
        }
    }

    public new void ToggleWindowSize()
    {
        pos.gameObject.SetActive(this.gameObject.GetComponent<RectTransform>().rect.height == collapseHeight);
        base.ToggleWindowSize();
    }
}
