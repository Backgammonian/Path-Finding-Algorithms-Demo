using System.Collections.Generic;

namespace PathFindingAlgorithmsDemo.Algorithms
{
    public static class Utils
    {
        public static List<Node> BacktrackToPath(Node end)
        {
            var current = end;
            List<Node> path = new List<Node>();

            while (current != null)
            {
                path.Add(current);
                current = current.PreviousNode;
            }

            path.Reverse();

            return path;
        }
    }
}
