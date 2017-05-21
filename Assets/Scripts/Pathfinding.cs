using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    Grid grid;
    public Transform seeker, target;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    //to find shorted path between points
    void findPath(Vector3 startPoint, Vector3 targetPoint)
    {
        Node startNode = grid.nodeFromWorldPoint(startPoint);
        Node targetNode = grid.nodeFromWorldPoint(targetPoint);
        List<Node> openSet = new List<Node>(); //set of nodes to be evaluated
        HashSet<Node> closedSet = new HashSet<Node>(); //set of nodes that have already been calculated
        openSet.Add(startNode);
        while(openSet.Count > 0)
        {
            Node currentNode = openSet[0]; // node with lowest fcost
            //to find node with lowest fcost in the openset
            for (int i=1; i< openSet.Count; i++)
            {
                openSet[i].finfFcost();
                if(openSet[i].fcost < currentNode.fcost || openSet[i].fcost == currentNode.fcost && openSet[i].hcost < currentNode.hcost)
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            if(currentNode == targetNode) //found path
            {
                retracePath(startNode, targetNode);
                return;
            }
            foreach (Node neighbour in grid.getNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }
                int newMove = currentNode.gcost + getDistance(currentNode, neighbour);
                if(newMove < neighbour.gcost || !openSet.Contains(neighbour)) // new path is shorted than the old path
                {
                    neighbour.gcost = newMove;
                    neighbour.hcost = getDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;
                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

    }

    int getDistance(Node a, Node b)
    {
        int distx = Mathf.Abs(a.gridX - b.gridX);
        int disty = Mathf.Abs(a.gridY - b.gridY);
        if (distx > disty)
            return 14 * disty + 10 * (distx - disty);
        else
            return 14 * distx + 10 * (disty - distx);
    }

    void retracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node curNode = end;
        while (curNode != start)
        {
            path.Add(curNode);
            curNode = curNode.parent;
        }
        path.Reverse();
        grid.path = path;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        findPath(seeker.position, target.position);
	}
}
