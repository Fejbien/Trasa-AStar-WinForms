using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstarPF
{
    public class AStar
    {
        public static List<Node> UkonczonaSciezka = new List<Node>();

        public static void FindPath(Node startNode, Node koniecNode, List<List<Node>> grid)
        {
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node node = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fKoszt < node.fKoszt || openSet[i].fKoszt == node.fKoszt)
                    {
                        if (openSet[i].hKoszt < node.hKoszt)
                            node = openSet[i];
                    }
                }

                openSet.Remove(node);
                closedSet.Add(node);

                if (node == koniecNode)
                {
                    RetracePath(startNode, koniecNode);
                    return;
                }

                foreach (Node neighbour in Node.ZdobadziSasiadow(node, grid))
                {
                    if (!neighbour.moznaChodzic || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newCostToNeighbour = node.gKoszt + GetDistance(node, neighbour);
                    if (newCostToNeighbour < neighbour.gKoszt || !openSet.Contains(neighbour))
                    {
                        neighbour.gKoszt = newCostToNeighbour;
                        neighbour.hKoszt = GetDistance(neighbour, koniecNode);
                        neighbour.rodzic = node;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }

        static void RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.rodzic;
            }
            path.Reverse();

            UkonczonaSciezka = path;
        }

        static int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Math.Abs(nodeA.polozenie.x - nodeB.polozenie.x);
            int dstY = Math.Abs(nodeA.polozenie.y - nodeB.polozenie.y);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}
