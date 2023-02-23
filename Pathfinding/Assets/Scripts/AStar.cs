using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        
    }

    public void AStarAlgorithm(Node startNode, Node finishNode)
    {
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        openList.Add(startNode);


        Node currentNode;
        while(openList.Count > 0)
        {
            currentNode = openList[0];
            float lowestF = openList[0].f;
            foreach(Node n in openList)
            {
                if(n.f < lowestF)
                {
                    currentNode = n;
                    lowestF = n.f;
                }
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if(currentNode.Equals(finishNode))
            {
                return;
            }

            foreach(Node neighbor in currentNode.getNeighbors())
            {
                if(!neighbor.isTraversable() || closedList.Contains(neighbor))
                {
                    continue;
                }
                if(!openList.Contains(neighbor) || false) //change false to check that new path is shorter than current
                {
                    neighbor.UpdateGHF();
                    neighbor.parent = currentNode;
                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }

        }
}


}
