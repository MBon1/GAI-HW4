using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class EditorWidget : MonoBehaviour
{
    public float value { get; protected set; }
    [SerializeField] EditorWindow editorWindow;

    public abstract bool interactable
    {
        get;    // get method
        set;    // set method
    }


    protected void SetProperty()
    {
        editorWindow.SetTargetProperty();
    }

    public abstract void SetValue();

    public abstract void SetValue(float val, bool setProperty);
}
