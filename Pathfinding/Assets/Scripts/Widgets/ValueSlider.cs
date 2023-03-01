using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueSlider : EditorWidget
{
    Slider slider;
    [SerializeField] Text sliderText;

    public override bool interactable
    {
        get { return slider.interactable; }   // get method
        set { slider.interactable = value; }  // set method
    }

    // Start is called before the first frame update
    private void Awake()
    {
        slider = this.GetComponent<Slider>();
        SetValue();
    }

    public override void SetValue()
    {
        SetValue(slider.value, true);
    }

    public override void SetValue(float val, bool setProperty)
    {
        value = Mathf.Clamp(val, slider.minValue, slider.maxValue);
        sliderText.text = value.ToString("0.00");
        slider.value = value;

        if (setProperty)
        {
            SetProperty();
        }
    }

    public float minValue()
    {
        return slider.minValue;
    }

    public float maxValue()
    {
        return slider.maxValue;
    }
}
