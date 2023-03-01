using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    public bool isRunning { get; private set; } = false;


    public IEnumerator AStarCoroutine(Map map, MapNode start, MapNode goal, float waitTime = 0.5f)
    {
        isRunning = true;

        List<MapNode> openList = new List<MapNode>();
        List<MapNode> closedList = new List<MapNode>();

        openList.Add(start);  // start.f is initialized to be 0.0f
        start.SetGHF(0.0f, Vector3.Distance(start.position, goal.position));

        // Check if there are open lists and if the start and end nodes are traversable
        while (openList.Count > 0 && start.IsTraversable() && goal.IsTraversable())
        {
            MapNode q = openList[0];
            float minF = openList[0].f;

            foreach (MapNode node in openList)
            {
                float currF = node.f;

                if (currF < minF)
                {
                    q = node;
                    minF = currF;
                }
            }

            openList.Remove(q);
            closedList.Add(q);

            if (q.Equals(goal))
            {
                //q.SetNodeColor(MapNode.TraverseColor.Start); // should add color for path
                while (q != null)
                {
                    q.SetNodeColor(MapNode.TraverseColor.Start); // should add color path
                    q = q.parent;
                }
                isRunning = false;

                // COMPLETE (staplp2) : Draw final path

                Debug.Log("PATH FOUND!");
                yield break;
            }

            List<MapNode> neighbors = map.getNeighbors(q);
            foreach (MapNode successor in neighbors)
            {
                if (!successor.IsTraversable() || closedList.Contains(successor)) // If the start / end position is on a non-traversable tile. Need to consider this.
                {
                    continue;
                }

                /*if (successor.Equals(start))
                {
                    continue;
                }*/
                float newG, newH;
                if (successor.useEuclidean)
                {
                    newG = q.g + Mathf.Abs(Vector3.Distance(successor.position, q.position));
                    newH = Mathf.Abs(Vector3.Distance(goal.position, successor.position));
                }
                else
                {
                    newG = q.g + Mathf.Abs(ManhattanDistance(successor.position, q.position));
                    newH = Mathf.Abs(ManhattanDistance(goal.position, successor.position));
                }
                
                float newF = newG + (newH * successor.hWeight);

                //Vector3 succPos = successor.position;  // uhhh should this be a Vector3Int...?

                //List<MapNode> bothLists = openList;
                //bothLists.AddRange(closedList);

                bool isSkippable = false;
                if (openList.Contains(successor))
                {
                    foreach (MapNode node in openList)
                    {
                        if (node.Equals(successor) && (node.f < newF))
                        {
                            isSkippable = true;
                            break;
                        }
                    }
                }

                if (!isSkippable)
                {
                    successor.SetGHF(newG, newH);
                    if (!openList.Contains(successor))
                    {
                        openList.Add(successor);
                    }
                    successor.parent = q;
                }
            }

            // COMPLETE (hungj2) : Update Node Colors

            foreach (MapNode closedNode in closedList) {
                closedNode.SetNodeColor(MapNode.TraverseColor.Closed);
            }

            foreach (MapNode openNode in openList) {
                openNode.SetNodeColor(MapNode.TraverseColor.Open);
            }

            yield return new WaitForSeconds(waitTime);
        }

        isRunning = false;

        Debug.Log("NO PATH FOUND!");
    }


    private float ManhattanDistance(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    //Justin's latest updated implementation
    /*public IEnumerator AStarCoroutine(Map map, MapNode start, MapNode goal, float waitTime = 0.5f)
    {
        isRunning = true;

        List<MapNode> openList = new List<MapNode>();
        List<MapNode> closedList = new List<MapNode>();

        openList.Add(start);  // start.f is initialized to be 0.0f

        while (openList.Count > 0)
        {
            MapNode q = openList[0];
            float minF = openList[0].f;

            foreach (MapNode node in openList)
            {
                float currF = node.f;

                if (currF < minF)
                {
                    q = node;
                    minF = currF;
                }
            }

            openList.Remove(q);

            foreach (MapNode successor in map.getNeighbors(q))
            {
                if (!successor.IsTraversable()) // If the start / end position is on a non-traversable tile. Need to consider this.
                {
                    continue;
                }

                *//*if (successor.Equals(start))
                {
                    continue;
                }*//*

                if (successor.Equals(goal))
                {
                    successor.SetNodeColor(MapNode.TraverseColor.Start); // should add color for path
                    *//*while (q.parent != null)
                    {
                        q.SetNodeColor(MapNode.TraverseColor.Start); // should add color path
                        q = q.parent;
                    }*//*
                    isRunning = false;

                    // COMPLETE (staplp2) : Draw final path

                    Debug.Log("PATH FOUND!");
                    yield break;
                }

                float newG = q.g + Mathf.Abs(Vector3.Distance(successor.position, q.position));
                float newH = Mathf.Abs(Vector3.Distance(goal.position, successor.position));
                float newF = newG + newH;

                Vector3 succPos = successor.position;  // uhhh should this be a Vector3Int...?

                List<MapNode> bothLists = openList;
                //bothLists.AddRange(closedList);

                bool isSkippable = false;

                foreach (MapNode node in bothLists)
                {
                    if (node.position.Equals(succPos) && (node.f < newF))
                    {
                        isSkippable = true;
                        break;
                    }
                }

                if (!isSkippable)
                {
                    successor.SetGHF(newG, newH);
                    openList.Add(successor);
                    successor.parent = q;
                }
            }

            closedList.Add(q);

            // COMPLETE (hungj2) : Update Node Colors

            foreach (MapNode closedNode in closedList)
            {
                closedNode.SetNodeColor(MapNode.TraverseColor.Closed);
            }

            foreach (MapNode openNode in openList)
            {
                openNode.SetNodeColor(MapNode.TraverseColor.Open);
            }

            yield return new WaitForSeconds(waitTime);
        }

        isRunning = false;

        Debug.Log("NO PATH FOUND!");
    }*/


    /*public List<MapNode> AStarAlgorithm(Map map, MapNode start, MapNode goal)
    {
        List<MapNode> openList = new List<MapNode>();
        List<MapNode> closedList = new List<MapNode>();

        openList.Add(start);  // start.f is initialized to be 0.0f

        while (openList.Count > 0)
        {
            MapNode q = openList[0];
            float minF = openList[0].f;

            foreach (MapNode node in openList)
            {
                float currF = node.f;

                if (currF < minF)
                {
                    q = node;
                    minF = currF;
                }
            }

            openList.Remove(q);
            
            foreach (MapNode successor in map.getNeighbors(q))
            {
                if (successor.Equals(goal))
                {
                    return closedList;
                }

                float newG = q.g + Mathf.Abs(Vector3.Distance(successor.position, q.position));
                float newH = Mathf.Abs(Vector3.Distance(goal.position, successor.position));

                successor.SetGHF(newG, newH);

                Vector3 succPos = successor.position;  // uhhh should this be a Vector3Int...?

                List<MapNode> bothLists = openList;
                bothLists.AddRange(closedList);

                bool isSkippable = false;

                foreach (MapNode node in bothLists)
                {
                    if (node.position.Equals(succPos) && (node.f < successor.f))
                    {
                        isSkippable = true;
                        break;
                    }
                }

                if (!isSkippable)
                {
                    openList.Add(successor);
                }
            }

            closedList.Add(q);
        }

        return closedList;
    }*/




    // NOTE FROM JUSTIN: I'm a poopy dumb dumb and all of the code below was for nothing.
    //                   You can probably tell that it's based on Wikipedia's explanation.


    // public float HeuristicFunction(string type, MapNode start, MapNode goal)
    // {
    //     float distance = 0.0f;

    //     if (type.Equals("Euclidian"))
    //     {
    //         // TO DO: FILL OUT
    //     }
    //     else if (type.Equals("Manhattan"))
    //     {
    //         // TO DO: FILL OUT
    //     }

    //     return distance;
    // }

    // public List<MapNode> ReconstructPath(Dictionary<MapNode, MapNode> cameFrom, ref MapNode current)
    // {
    //     List<MapNode> total_path = new List<MapNode>();

    //     total_path.Add(current);

    //     foreach (MapNode current in cameFrom.Keys)
    //     {
    //         current = cameFrom[current];
    //         total_path.Insert(0, current);
    //     }

    //     return total_path;
    // }

    // public List<MapNode> AStarAlgorithm(string type, MapNode start, MapNode goal)
    // {
    //     List<MapNode> openSet = new List<MapNode>();
    //     List<MapNode> closedSet = new List<MapNode>();

    //     openList.Add(start);

    //     Dictionary<MapNode, float> gScore = new Dictionary<MapNode, float>();
    //     Dictionary<MapNode, float> fScore = new Dictionary<MapNode, float>();

    //     // TO DO: INITIALIZE THE MAPS

    //     gScore[start] = 0;
    //     fScore[start] = HeuristicFunction(type, start, goal);

    //     // NOTE: This is when I realized that this isn't how the data structure works.

    //     return new List<Node>();  // FAILURE
    // }




    // NOTE FROM JUSTIN: The code below is Philip's original implementation.


    /*public void AStarAlgorithm(Node startNode, Node finishNode)
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
    }*/
}
