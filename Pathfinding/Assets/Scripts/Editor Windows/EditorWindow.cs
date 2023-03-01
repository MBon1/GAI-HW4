using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class EditorWindow : MonoBehaviour
{
    protected GameObject target;

    protected float expandedHeight = 20;
    protected float collapseHeight = 40;

    protected void Awake()
    {
        expandedHeight = this.gameObject.GetComponent<RectTransform>().rect.height;
        Debug.Log("HEIGHT: " + expandedHeight);
    }

    public abstract void SetTargetProperty();
    public abstract void SetTargetObject(GameObject obj);

    protected void SetAllWidgetsInteractability(bool interactability)
    {
        EditorWidget[] widgets = this.gameObject.GetComponentsInChildren<EditorWidget>();

        foreach(EditorWidget widget in widgets)
        {
            widget.interactable = interactability;
        }
    }

    protected abstract void SetAllWidgetsActiveness(bool active);

    [ContextMenu("Collapse")]
    public void Collapse()
    {
        SetAllWidgetsInteractability(false);
        SetAllWidgetsActiveness(false);
        SetHeight(collapseHeight);
    }

    [ContextMenu("Expand")]
    public void Expand()
    {
        SetHeight(expandedHeight);
        SetAllWidgetsActiveness(true);
        SetAllWidgetsInteractability(true);
    }

    protected void SetHeight(float height)
    {
        this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(this.gameObject.GetComponent<RectTransform>().rect.width, height);
    }

    [ContextMenu("Toggle Size")]
    public void ToggleWindowSize()
    {
        if (this.gameObject.GetComponent<RectTransform>().rect.height == collapseHeight)
            Expand();
        else
            Collapse();
    }
}
