using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileSizeSetter : MonoBehaviour
{
    [SerializeField] Vector2Int cap = new Vector2Int(int.MinValue, int.MaxValue);
    InputField inputField;
    [SerializeField] int value = 2;

    // Start is called before the first frame update
    private void Awake()
    {
        inputField = this.GetComponent<InputField>();
        SetValue(value);
    }

    public void SetValue()
    {
        string inputText = inputField.text;

        if (!int.TryParse(inputText, out int newValue))
        {
            newValue = value;
        }

        if (float.TryParse(inputText, out float f))
        {
            newValue = Mathf.FloorToInt(f);
        }

        SetValue(newValue);
    }

    public void SetValue(int val)
    {
        value = Mathf.Clamp(val, cap.x, cap.y);
        inputField.text = value.ToString("0");

        SetTileSize(value);
    }

    public void SetTileSize(int size)
    {
        TileMapLoader._tilesPerNode = size;
    }
}
