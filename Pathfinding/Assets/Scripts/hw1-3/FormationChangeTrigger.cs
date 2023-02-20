using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationChangeTrigger : MonoBehaviour
{
    public Formation enterFormation = Formation.Line;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Formation Manager"))
        {
            collision.gameObject.GetComponent<NewFormationManager>().formation = enterFormation;
        }
    }
}
