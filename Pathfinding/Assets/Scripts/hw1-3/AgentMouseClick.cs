using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentMouseClick : MonoBehaviour
{
    public bool clickToDestroy = true;

    private void OnMouseDown()
    {
        NewFormationManager.FM.RemoveAgent(this.gameObject);
    }
}
