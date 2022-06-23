using LineGame.Runtime.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LineGame.Runtime.Core
{
    public class Pathfinding
    {

        
        public static List<Vector2> FindPath(Tile startTile, Tile endTile)
        {
            Node startNode = new Node(endTile);
            startNode.Cost = 0;
            List<Node> toSearch = new List<Node>() { startNode };
            List<Tile> toSearchTile = new List<Tile>();
            Dictionary<Tile, Node> processed = new Dictionary<Tile, Node>();
            
            while (toSearch.Any())
            {
                Node current = toSearch[0];
                for(int i = 0; i < toSearch.Count; i++)
                {
                    if (toSearch[i].Cost < current.Cost) current = toSearch[i];
                }
                foreach (var neighbor in GetNeighbour(current))
                {           
                    
                    if (processed.ContainsKey(neighbor.Tile)) continue;
                    if (neighbor.Tile.Ball != null && neighbor.Tile != startTile && !neighbor.Tile.Ball.IsThrought) continue;

                    int cost = current.Cost + GetCostMove(current,neighbor) ;
                    if(cost < neighbor.Cost )
                    {
                        neighbor.Cost = cost;
                        neighbor.Previous = current;
                        if (!toSearchTile.Contains(neighbor.Tile))
                        {
                            toSearchTile.Add(neighbor.Tile);
                            toSearch.Add(neighbor);
                        }
                    }

                }
                processed.Add(current.Tile,current);
                toSearch.Remove(current);

            }
            if (!processed.ContainsKey(startTile)) return null;
            Node endNode = processed[startTile];
            var path = new List<Vector2>();
            path.Add(endNode.Tile.transform.position);
            while (endNode.Tile != endTile)
            {
                if(endNode.Previous.Previous != null)
                {
                    Vector2 pointA = endNode.Tile.transform.position;
                    Vector2 poinC = endNode.Previous.Previous.Tile.transform.position;
                    if (pointA.x != poinC.x && pointA.y != poinC.y)
                    {
                        path.Add(endNode.Previous.Tile.transform.position);
                    }
                }
                endNode = endNode.Previous;
            }            
            path.Add(endNode.Tile.transform.position);
            return path;

        }
        protected static List<Node> GetNeighbour(Node node)
        {
            Vector2 nodeValue = node.Tile.transform.position;
            List<Node> neighbourNodes = new List<Node>();
            if (nodeValue.x - 1 >= 0)
            {
                Vector2 position = new Vector2((int)nodeValue.x - 1, (int)nodeValue.y);
                Tile neighbourTile = BoardManager.Instance.Tiles[position];
                neighbourNodes.Add(new Node(neighbourTile));
            }
            if (nodeValue.x + 1 < BoardManager.Instance.Size)
            {
                Vector2 position = new Vector2((int)nodeValue.x + 1, (int)nodeValue.y);
                Tile neighbourTile = BoardManager.Instance.Tiles[position];
                neighbourNodes.Add(new Node(neighbourTile));
            }
            if (nodeValue.y - 1 >= 0)
            {
                Vector2 position = new Vector2((int)nodeValue.x, (int)nodeValue.y - 1);
                Tile neighbourTile = BoardManager.Instance.Tiles[position];
                neighbourNodes.Add(new Node(neighbourTile));
            }
            if (nodeValue.y + 1 < BoardManager.Instance.Size)
            {
                Vector2 position = new Vector2((int)nodeValue.x, (int)nodeValue.y + 1);
                Tile neighbourTile = BoardManager.Instance.Tiles[position];
                neighbourNodes.Add(new Node(neighbourTile));
            }
            return neighbourNodes;
        }
        protected static int GetCostMove(Node start, Node end)
        {
            if (start.Previous == null) return 0;
            if ((end.Tile.transform.position.x != start.Previous.Tile.transform.position.x)
                && (end.Tile.transform.position.y != start.Previous.Tile.transform.position.y))
            {
                return 1;
            }
            return 0;
        }

    }
    public class Node
    {
        protected Tile _tile;
        public Tile Tile => _tile;

        public int Cost { get; set; }
        public Node Previous { get; set; }

        public Node(Tile tile)
        {
            _tile = tile;
            Cost = int.MaxValue;
        }
      
    }
}
