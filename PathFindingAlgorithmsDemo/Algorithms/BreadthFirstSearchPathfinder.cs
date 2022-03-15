using System.Collections.Generic;

namespace PathFindingAlgorithmsDemo.Algorithms
{
    //source: https://github.com/dbrizov/Unity-PathFindingAlgorithms/blob/master/Assets/Scripts/PathFinder.cs

    public static class BreadthFirstSearchPathfinder
    {
        public static List<Node> BreadthFirstSearchFindPath(this NodeGrid grid, ref HashSet<Node> visited)
        {
            grid.Start.PreviousNode = null;

            visited.Add(grid.Start);

            var frontier = new Queue<Node>();
            frontier.Enqueue(grid.Start);

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                if (current == grid.End)
                {
                    break;
                }

                foreach (var neighbor in grid.GetNeighbors(current))
                {
                    if (!visited.Contains(neighbor) && neighbor.IsWalkable)
                    {
                        visited.Add(neighbor);
                        frontier.Enqueue(neighbor);

                        neighbor.PreviousNode = current;
                    }
                }
            }

            return Utils.BacktrackToPath(grid.End);
        }
    }
}
