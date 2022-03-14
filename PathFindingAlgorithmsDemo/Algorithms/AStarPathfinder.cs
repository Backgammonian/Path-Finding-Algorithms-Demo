using System.Collections.Generic;

namespace PathFindingAlgorithmsDemo.Algorithms
{
    public static class AStarPathfinder
    {
        public static List<Node> AStartFindPath(this NodeGrid grid, ref HashSet<Node> visited)
        {
            grid.SetCosts(int.MaxValue);
            grid.Start.Cost = 0;
            grid.Start.PreviousNode = null;

            var comparison = new NodeComparison(grid.End);

            var frontier = new MinHeap<Node>(comparison.HeuristicComparison);
            frontier.Add(grid.Start);

            visited.Add(grid.Start);

            while (frontier.Count > 0)
            {
                var current = frontier.Remove();

                if (current == grid.End)
                {
                    break;
                }

                foreach (var neighbor in grid.GetNeighbors(current))
                {
                    int newNeighborCost = current.Cost + neighbor.Weight;
                    if (newNeighborCost < neighbor.Cost)
                    {
                        neighbor.Cost = newNeighborCost;
                        neighbor.PreviousNode = current;
                    }

                    if (!visited.Contains(neighbor) && neighbor.IsWalkable)
                    {
                        frontier.Add(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }

            return Utils.BacktrackToPath(grid.End);
        }
    }
}
