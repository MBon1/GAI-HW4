using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewFormationManager : MonoBehaviour
{
    public static NewFormationManager FM;
    public GameObject agentPrefab;
    public int numAgents = 12;
    public List<GameObject> allAgents = new List<GameObject>();
    public Vector3 goalPos = Vector3.zero;
    public Movement_3 movement_3;
    public bool outsourceMovement = false;

    [Header("Target Positions")]
    public GameObject agentTargetPrefab;
    public GameObject agentVisibleTargetPrefab;
    public Dictionary<GameObject, GameObject> agentTargetPositions = new Dictionary<GameObject, GameObject>();

    [Header("Formation Pattern")]
    public Formation formation = Formation.Line;
    Formation lastFormation = Formation.None;
    public float agentWidth = 1;
    public float agentAttatchmentDistance = 0.5f;
    public bool usingTwoLevel = false;

    [Header("Visualization")]
    public Text formationText;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 pos = this.transform.position;
        for (int i = 0; i < numAgents; i++)
        {
            allAgents.Add(Instantiate(agentPrefab, pos, Quaternion.identity));
            allAgents[i].name = "Agent " + i;
            /*if (!usingTwoLevel)
            {
                agentTargetPositions.Add(allAgents[i], Instantiate(agentTargetPrefab, pos, Quaternion.identity));
            }
            else
            {
                agentTargetPositions.Add(allAgents[i], Instantiate(agentVisibleTargetPrefab, pos, Quaternion.identity));
            }*/
            agentTargetPositions.Add(allAgents[i], Instantiate(agentVisibleTargetPrefab, pos, Quaternion.identity));

            agentTargetPositions[allAgents[i]].name = "Target " + i;

            Movement_3 movement = allAgents[i].GetComponent<Movement_3>();
            movement.targets.Add(agentTargetPositions[allAgents[i]]);
            if (usingTwoLevel)
            {
                movement.movement = Movement_3.MovementOperation.Arrive;
                movement.obstacleAvoidance = Movement_3.ObstacleAvoidanceOperation.RayCasting;
            }
        }

        if (numAgents > 0 && outsourceMovement)
            movement_3.outsource = allAgents[0].GetComponent<Movement_3>();
        else
            movement_3.outsource = null;

        FM = this;

        UpdateFormation(true);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFormation();
    }

    /*void OnTriggerEnter2D(Collider2D Other) {
        formation = Formation.Line;
    }

    void OnTriggerExit2D(Collider2D Other) {
        formation = Formation.Circle;
    }*/

    public void RemoveAgent(GameObject agent)
    {
        int index = allAgents.IndexOf(agent);
        if (allAgents.Remove(agent))
        {
            Destroy(agentTargetPositions[agent]);
            Destroy(agent);
            if (allAgents.Count > 0 && outsourceMovement)
            {
                movement_3.outsource = allAgents[0].GetComponent<Movement_3>();
            }
            else
            {
                movement_3.outsource = null;
            }
        }
    }

    void UpdateFormation(bool ignoreDetatchment = false)
    {
        SetGoalPos();
        if (formation == Formation.Circle)
        {
            UpdateAgentsCircle(ignoreDetatchment);
        }
        if (formation == Formation.Line)
        {
            UpdateAgentsLine(ignoreDetatchment);
        }
        if (formation == Formation.Wedge)
        {
            UpdateAgentsWedge(ignoreDetatchment);
        }
        if (formation == Formation.ThreeWideLine)
        {
            UpdateAgentsThreeWideLine(ignoreDetatchment);
        }

        if (formation != lastFormation)
        {
            lastFormation = formation;

            if (formation == Formation.Circle)
            {
                WriteFormation("Defensive Circle");
            }
            if (formation == Formation.Line)
            {
                WriteFormation("Line");
            }
            if (formation == Formation.Wedge)
            {
                WriteFormation("Wedge");
            }
            if (formation == Formation.ThreeWideLine)
            {
                WriteFormation("Three Wide Line");
            }
        }
    }

    void SetGoalPos()
    {
        SetGoalPos(this.transform.position);
    }

    void SetGoalPos(Vector3 pos)
    {
        goalPos = pos;
    }


    void UpdateAgentsCircle(bool ignoreDetatchment = false)
    {
        if (allAgents.Count == 1)
        {
            allAgents[0].transform.position = this.transform.position;
            allAgents[0].transform.rotation = this.transform.rotation;
            return;
        }

        float numSlots = allAgents.Count;
        float radius = agentWidth / Mathf.Sin(Mathf.PI / numSlots);
        float angleStep = 360.0f / allAgents.Count;
        float currentAngle = 90.0f + this.transform.rotation.eulerAngles.z;

        Vector3 pos = transform.position;
        
        for (int i = 0; i < allAgents.Count; i++)
        {
            float x = pos.x + radius * Mathf.Cos(Mathf.Deg2Rad * currentAngle);
            float y = pos.y + radius * Mathf.Sin(Mathf.Deg2Rad * currentAngle);
            Vector2 newPos = new Vector2(x, y);

            agentTargetPositions[allAgents[i]].transform.position = newPos;

            // Check distance between agent's position and its taret's position
            Movement_3 movement = allAgents[i].GetComponent<Movement_3>();
            bool notDetatched = !movement.Detatch(agentAttatchmentDistance);
            if (ignoreDetatchment || notDetatched)
            {
                allAgents[i].transform.position = newPos;
                allAgents[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentAngle - 90));
            }

            currentAngle += angleStep;
        }
    }

    void UpdateAgentsLine(bool ignoreDetatchment = false)
    {
        if (allAgents.Count == 0)
        {
            return;
        }

        float spacing = 2.0f;
        //float relativePos = ((float)(allAgents.Count - 1) * spacing) / 2.0f;
        float relativePos = 1;
        if (usingTwoLevel)
        {
            relativePos = -spacing;
        }
        float forwardAngle = this.transform.rotation.eulerAngles.z + 90.0f;

        Vector3 pos = transform.position;

        for (int i = 0; i < allAgents.Count; i++)
        {
            float x = pos.x + (Mathf.Cos(Mathf.Deg2Rad * forwardAngle) * relativePos);
            float y = pos.y + (Mathf.Sin(Mathf.Deg2Rad * forwardAngle) * relativePos);
            Vector2 newPos = new Vector2(x, y);

            agentTargetPositions[allAgents[i]].transform.position = newPos;

            // Check distance between agent's position and its taret's position
            Movement_3 movement = allAgents[i].GetComponent<Movement_3>();
            if (usingTwoLevel)
            {
                movement.movement = Movement_3.MovementOperation.Arrive;
                movement.obstacleAvoidance = Movement_3.ObstacleAvoidanceOperation.RayCasting;
            }

            bool notDetatched = !movement.Detatch(agentAttatchmentDistance);
            if (!usingTwoLevel && (ignoreDetatchment || notDetatched))
            {
                allAgents[i].transform.position = newPos;
                allAgents[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, forwardAngle - 90));
            }

            relativePos -= 2.0f;
        }
    }

    void UpdateAgentsWedge(bool ignoreDetatchment = false)
    {
        if (allAgents.Count == 0)
        {
            return;
        }

        float relativePos = -5;
        float forwardAngle = this.transform.rotation.eulerAngles.z + 90.0f;

        Vector3 pos = transform.position;
        float x;
        float y;
        float odd_x = pos.x, odd_y = pos.y, even_x = pos.x, even_y = pos.y;

        for (int i = 0; i < allAgents.Count; i++)
        {
            Movement_3 movement = allAgents[i].GetComponent<Movement_3>();
            if (usingTwoLevel)
            {
                movement.movement = Movement_3.MovementOperation.Arrive;
                movement.obstacleAvoidance = Movement_3.ObstacleAvoidanceOperation.RayCasting;
            }

            if (i == 0)
            {
                x = pos.x + (Mathf.Cos(Mathf.Deg2Rad * forwardAngle) * relativePos);
                y = pos.y + (Mathf.Sin(Mathf.Deg2Rad * forwardAngle) * relativePos);
                even_x = x;
                even_y = y;
                odd_x = x;
                odd_y = y;
                relativePos = -2;
            }
            else if (i % 2 ==0)
            {
                x = even_x + (Mathf.Cos((Mathf.Deg2Rad * forwardAngle) + 45) * relativePos);
                y = even_y + (Mathf.Sin((Mathf.Deg2Rad * forwardAngle) + 45) * relativePos);
                even_x = x;
                even_y = y;
            }
            else
            {
                x = odd_x + (Mathf.Cos((Mathf.Deg2Rad * forwardAngle) - 45) * relativePos);
                y = odd_y + (Mathf.Sin((Mathf.Deg2Rad * forwardAngle) - 45) * relativePos);
                odd_x = x;
                odd_y = y;
            }
            Vector2 newPos = new Vector2(x, y);

            agentTargetPositions[allAgents[i]].transform.position = newPos;

            // Check distance between agent's position and its taret's position
            if (ignoreDetatchment)
            {
                allAgents[i].transform.position = newPos;
                allAgents[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, forwardAngle - 90));
            }
        }
    }

    void UpdateAgentsThreeWideLine(bool ignoreDetatchment = false)
    {
        if (allAgents.Count == 0)
        {
            return;
        }

        float spacing = 2.0f;
        //float relativePos = ((float)(allAgents.Count - 1) * spacing) / 2.0f;
        float relativePos = 1;
        if (usingTwoLevel)
        {
            relativePos = -spacing;
        }
        float forwardAngle = this.transform.rotation.eulerAngles.z + 90.0f;

        Vector3 pos = transform.position;
        float x = pos.x, y = pos.y, center_x = pos.x, center_y = pos.y;
        for (int i = 0; i < allAgents.Count; i++)
        {
            if (i % 3 == 0 && i != 0)
            {
                relativePos -= 2.0f;
            }
            if (i % 3 == 0)
            {
                x = pos.x + (Mathf.Cos(Mathf.Deg2Rad * forwardAngle) * relativePos);
                y = pos.y + (Mathf.Sin(Mathf.Deg2Rad * forwardAngle) * relativePos);
                center_x = x;
                center_y = y;
            }else if(i % 3 == 1)
            {
                x = center_x + (Mathf.Cos(Mathf.Deg2Rad * forwardAngle -90) * 2);
                y = center_y + (Mathf.Sin(Mathf.Deg2Rad * forwardAngle -90)  * 2);
            }
            else
            {
                x = center_x + (Mathf.Cos(Mathf.Deg2Rad * forwardAngle +90) * 2);
                y = center_y + (Mathf.Sin(Mathf.Deg2Rad * forwardAngle +90) * 2);
            }
            Vector2 newPos = new Vector2(x, y);

            agentTargetPositions[allAgents[i]].transform.position = newPos;

            // Check distance between agent's position and its taret's position
            Movement_3 movement = allAgents[i].GetComponent<Movement_3>();
            if (usingTwoLevel)
            {
                movement.movement = Movement_3.MovementOperation.Arrive;
                movement.obstacleAvoidance = Movement_3.ObstacleAvoidanceOperation.RayCasting;
            }

            bool notDetatched = !movement.Detatch(agentAttatchmentDistance);
            if (!usingTwoLevel && (ignoreDetatchment || notDetatched))
            {
                allAgents[i].transform.position = newPos;
                allAgents[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, forwardAngle - 90));
            }
        }
    }

    // Display the name of the given behavior
    void WriteFormation(string behavior)
    {
        if (formationText != null)
            formationText.text = behavior;
    }
}




public enum Formation
{
    None, 
    Circle,
    Line,
    Wedge,
    ThreeWideLine
}
