﻿using System.Collections.Generic;

namespace PathFindingAlgorithmsDemo.Algorithms
{
    //source: https://github.com/dbrizov/Unity-PathFindingAlgorithms/blob/master/Assets/Scripts/PathFinder.cs

    public static class GreedyBestFirstSearchPathfinder
    {
        public static List<Node> GreedyBestFirstSearchFindPath(this NodeGrid grid, ref HashSet<Node> visited)
        {
            grid.Start.PreviousNode = null;

            var frontier = new MinHeap<Node>(grid.End.HeuristicComparison);
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
                    if (!visited.Contains(neighbor) && neighbor.IsWalkable)
                    {
                        frontier.Add(neighbor);
                        visited.Add(neighbor);

                        neighbor.PreviousNode = current;
                    }
                }
            }

            return grid.End.BacktrackToPath();
        }

        public static List<Node> GreedyBestFirstSearchFindPath(this NodeGrid grid, ref HashSet<Node> visited, ref List<NewNodeInfo> visitedSteps, ref List<NewPathInfo> pathsSteps)
        {
            grid.Start.PreviousNode = null;

            var frontier = new MinHeap<Node>(grid.End.HeuristicComparison);
            frontier.Add(grid.Start);

            visited.Add(grid.Start);

            visitedSteps.Add(new NewNodeInfo(grid.Start));

            while (frontier.Count > 0)
            {
                var current = frontier.Remove();

                if (current == grid.End)
                {
                    break;
                }

                foreach (var neighbor in grid.GetNeighbors(current))
                {
                    if (!visited.Contains(neighbor) && neighbor.IsWalkable)
                    {
                        frontier.Add(neighbor);
                        visited.Add(neighbor);

                        visitedSteps.Add(new NewNodeInfo(neighbor));

                        neighbor.PreviousNode = current;

                        pathsSteps.Add(new NewPathInfo(current.BacktrackToPath()));
                    }
                }
            }

            return grid.End.BacktrackToPath();
        }
    }
}
