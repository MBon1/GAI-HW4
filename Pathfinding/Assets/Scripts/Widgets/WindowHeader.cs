using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowHeader : MonoBehaviour
{
    [SerializeField] EditorWindow editorWindow;
    [SerializeField] Toggle toggle;

    // Start is called before the first frame update
    void Start()
    {
        if (toggle.isOn)
        {
            editorWindow.Expand();
        }
        else
        {
            editorWindow.Collapse();
        }
    }
}
