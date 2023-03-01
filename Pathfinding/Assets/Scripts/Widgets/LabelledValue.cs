using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelledValue : EditorWidget
{
    [SerializeField] Text valeText;

    public override bool interactable
    {
        get { return this.gameObject.activeInHierarchy; }   // get method
        set { this.gameObject.SetActive(value); }  // set method
    }

    public override void SetValue()
    {
        valeText.text = value.ToString("0.00");
    }

    public override void SetValue(float val, bool setProperty)
    {
        value = val;
        valeText.text = value.ToString("0.00");
    }


}
