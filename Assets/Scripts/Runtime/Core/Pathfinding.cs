

using LineGame.Runtime.Managers;
using LineGame.Settings;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LineGame.Runtime.Core
{
    public class Pathfinding
    {
        public static List<Vector2> FindPathTwoWay(Tile start, Tile end)
        {
            List<Vector2> path = FindPath(start, end,start.Ball);
            List<Vector2> pathReverse = FindPath(end, start,start.Ball);
            if(pathReverse == null) return path;
            pathReverse.Reverse();
            if(path == null) return pathReverse;
            return path.Count < pathReverse.Count ? path : pathReverse;
        }
        protected static List<Vector2> FindPath(Tile start, Tile end,Ball ball)
        {
            Node startNode = new Node(start);
            Node endNode = new Node(end);
            List<Node> toSearch = new List<Node>() { startNode };
            List<Node> processed = new List<Node>();    

            while(toSearch.Any())
            {
                Node current = GetClosestNode(toSearch);

                toSearch.Remove(current);
                processed.Add(current);

                if(current.Tile == endNode.Tile)
                {
                    endNode = current;
                    return CalculatePath(startNode, endNode);
                }

                foreach(Node neighbor in GetNeighbor(current.Tile))
                {
                    if (processed.Any(x => x.Tile == neighbor.Tile)) continue;
                    if (neighbor.Tile.Ball != null && neighbor.Tile != endNode.Tile && !ball.IsThrought) continue;
                    bool inSearch = toSearch.Any(x => x.Tile == neighbor.Tile);
                    float costToNeighbor = current.G + CalculateGCost(current, neighbor);
                    if(!inSearch || costToNeighbor < neighbor.G)
                    {
                        neighbor.G = costToNeighbor;
                        neighbor.Connection = current;
                        if(!inSearch)
                        {
                            neighbor.H = CalculateHCost(startNode, current, neighbor); //CalculateHCost(neighbor, endNode);
                            toSearch.Add(neighbor);
                        }
                    }
                }
            }
            return null;
        }
        protected static List<Vector2> CalculatePath(Node start,Node endNode)
        {
            List<Vector2> path = new List<Vector2>();
            Node current = endNode;
            path.Add(current.Tile.transform.position);
            while(current!= start)
            {
                if (current.Connection.Connection != null)
                {
                    Vector2 pointA = current.Tile.transform.position;
                    Vector2 poinC = current.Connection.Connection.Tile.transform.position;
                    if (pointA.x != poinC.x && pointA.y != poinC.y)
                    {
                        path.Add(current.Connection.Tile.transform.position);
                    }
                }
                current = current.Connection;
            }
            path.Add(current.Tile.transform.position);
            path.Reverse();
            return path;
        }
        protected static Node GetClosestNode(List<Node> nodes)
        { 
            Node closetNode = nodes[0];
            for(int i = 0; i < nodes.Count; i++)
            {
                if(nodes[i].F < closetNode.F || nodes[i].F == closetNode.F && nodes[i].H < closetNode.H)
                {
                    closetNode = nodes[i];
                }
            }
            return closetNode;
        }
        protected static List<Node> GetNeighbor(Tile tile)
        {
            List<Node> nodes = new List<Node>();
            foreach(Tile current in BoardManager.Instance.GetNeighbor(tile))
            {
                nodes.Add(new Node(current));
            }
            return nodes;
        }
        protected static float CalculateGCost(Node current, Node neighbor)
        {

            if (current.Connection == null) return 0;
            if ((neighbor.Tile.transform.position.x != current.Connection.Tile.transform.position.x)
                && (neighbor.Tile.transform.position.y != current.Connection.Tile.transform.position.y))
            {
                return 0;
            }
            return 0;
        }
        protected static float CalculateHCost(Node start,Node current, Node neighbor)
        {
           
            List<Vector2> path = new List<Vector2>();
            Node temp = neighbor;
            temp.Connection = current;
            while (temp != start)
            {
                if (temp.Connection.Connection != null)
                {
                    Vector2 pointA = temp.Tile.transform.position;
                    Vector2 poinC = temp.Connection.Connection.Tile.transform.position;
                    if (pointA.x != poinC.x && pointA.y != poinC.y)
                    {
                        path.Add(temp.Connection.Tile.transform.position);
                    }
                }
                temp = temp.Connection;
            }
            return path.Count;
        }
        protected class Node
        {
            public Tile Tile { get; protected set; }
            public Node Connection { get; set; }
            public float G { get; set; }
            public float H { get; set; }
            public float F => G + H;

            public Node(Tile tile)
            {
                Tile = tile;
                G = int.MaxValue;
            }
        }
    }
    
}
