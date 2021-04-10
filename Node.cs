using System.Collections.Generic;

namespace AstarPF
{
    public class Node
    {
        static public int WielkoscX = 0;
        static public int WielkoscY = 0;

        public Vector2 polozenie;
        public bool moznaChodzic;

        public int gKoszt;
        public int hKoszt;
        public Node rodzic;

        public Node(Vector2 _polozenie, bool _moznaChodzic)
        {
            polozenie = _polozenie;
            moznaChodzic = _moznaChodzic;
        }

        public int fKoszt { get { return gKoszt + hKoszt; } }

        public static List<Node> ZdobadziSasiadow(Node node, List<List<Node>> grid)
        {
            List<Node> nody = new List<Node>();

            if (node.polozenie.x < WielkoscX - 1)
            {
                nody.Add(grid[node.polozenie.x + 1][node.polozenie.y]);
            }
            if (node.polozenie.x > 0)
            {
                nody.Add(grid[node.polozenie.x - 1][node.polozenie.y]);
            }
            if (node.polozenie.y < WielkoscY - 1)
            {
                nody.Add(grid[node.polozenie.x][node.polozenie.y + 1]);
            }
            if (node.polozenie.y > 0)
            {
                nody.Add(grid[node.polozenie.x][node.polozenie.y - 1]);
            }

            return nody;
        }
    }
}
