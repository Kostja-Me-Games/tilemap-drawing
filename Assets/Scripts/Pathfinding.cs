using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Pathfinding
{
    private Tilemap tilemap;
    [SerializeField] private Dictionary<Vector3Int, Node> nodes;
    private paintbrush pb;
    public Pathfinding(Tilemap tilemap)
    {
        this.tilemap = tilemap;
        nodes = new Dictionary<Vector3Int, Node>();
        pb = GameObject.Find("Grid").GetComponent<paintbrush>();

    }

    public List<Vector3Int> FindPath(Vector3Int startPosition, Vector3Int destinationPosition)
    {
        if (!IsWalkable(destinationPosition))
        {
            // Destination is unwalkable, return an empty list
            return new List<Vector3Int>();
        }

        // Create nodes for each tile in the tilemap
        CreateNodes(startPosition);

        Node startNode = nodes[startPosition];
        Node destinationNode = nodes[destinationPosition];

        // Initialize the open and closed sets
        HashSet<Node> openSet = new HashSet<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // Get the node with the lowest cost in the open set
            Node currentNode = GetNodeWithLowestCost(openSet);

            // Remove the current node from the open set and add it to the closed set
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == destinationNode)
            {
                // Destination reached, construct and return the path
                return ConstructPath(startNode, destinationNode);
            }

            // Process the neighbors of the current node
            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (closedSet.Contains(neighbor) || !IsWalkable(neighbor.position))
                {
                    // Skip unwalkable or already processed nodes
                    continue;
                }

                int newCost = currentNode.gCost + GetDistance(currentNode, neighbor);

                if (newCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    // Update neighbor's costs and set the current node as its parent
                    neighbor.gCost = newCost;
                    neighbor.hCost = GetDistance(neighbor, destinationNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        // Add the neighbor to the open set if it's not already in it
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        // No path found, return an empty list
        return new List<Vector3Int>();
    }

    private void CreateNodes(Vector3Int startPosition)
    {
        nodes.Clear();
        Node startingNode = new Node(startPosition);
        nodes.Add(startPosition, startingNode);
        foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
        {
            if (!IsWalkable(position))
            {
                continue;
            }

            Node node = new Node(position);
            nodes.Add(position, node);
        }

        Debug.Log(nodes);
        // iterate over nodes and print key and value
        //foreach (KeyValuePair<Vector3Int, Node> kvp in nodes)
        //{
        //    Debug.Log("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
        //}
    }

    private bool IsWalkable(Vector3Int position)
    {
        TileBase tile = tilemap.GetTile(position);
        return tile == pb.tiles["empty"] || pb.tiles["urinium"];
    }

    private Node GetNodeWithLowestCost(HashSet<Node> nodeSet)
    {
        Node lowestCostNode = null;
        foreach (Node node in nodeSet)
        {
            if (lowestCostNode == null || node.fCost < lowestCostNode.fCost)
            {
                lowestCostNode = node;
            }
        }
        return lowestCostNode;
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        Vector3Int[] neighborOffsets =
        {
            new Vector3Int(-1, 0, 0),  // Left
            new Vector3Int(1, 0, 0),   // Right
            new Vector3Int(0, -1, 0),  // Down
            new Vector3Int(0, 1, 0)    // Up
        };

        foreach (Vector3Int offset in neighborOffsets)
        {
            Vector3Int neighborPosition = node.position + offset;
            if (nodes.ContainsKey(neighborPosition))
            {
                neighbors.Add(nodes[neighborPosition]);
            }
        }

        return neighbors;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        return Mathf.Abs(nodeA.position.x - nodeB.position.x) + Mathf.Abs(nodeA.position.y - nodeB.position.y);
    }

    private List<Vector3Int> ConstructPath(Node startNode, Node destinationNode)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Node currentNode = destinationNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }

        path.Reverse(); // Reverse the path to get it from start to destination
        return path;
    }

    private class Node
    {
        public Vector3Int position;
        public int gCost; // Cost from the start node
        public int hCost; // Cost to the destination node
        public int fCost { get { return gCost + hCost; } }
        public Node parent;

        public Node(Vector3Int position)
        {
            this.position = position;
        }
    }
}
