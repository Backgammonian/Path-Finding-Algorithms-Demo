﻿using System.Collections.Generic;

namespace PathFindingAlgorithmsDemo.Algorithms
{
    //source: https://github.com/dbrizov/Unity-PathFindingAlgorithms/blob/master/Assets/Scripts/PathFinder.cs

    public static class DijkstraPathfinder
    {
        public static List<Node> DijkstraFindPath(this NodeGrid grid, ref HashSet<Node> visited)
        {
            grid.SetCosts(int.MaxValue);
            grid.Start.Cost = 0;
            grid.Start.PreviousNode = null;

            visited.Add(grid.Start);

            var frontier = new MinHeap<Node>((lhs, rhs) => lhs.Cost.CompareTo(rhs.Cost));
            frontier.Add(grid.Start);

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
                        visited.Add(neighbor);
                        frontier.Add(neighbor);
                    }
                }
            }

            return grid.End.BacktrackToPath();
        }

        public static List<Node> DijkstraFindPath(this NodeGrid grid, ref HashSet<Node> visited, ref List<NewNodeInfo> visitedSteps, ref List<NewPathInfo> pathsSteps)
        {
            grid.SetCosts(int.MaxValue);
            grid.Start.Cost = 0;
            grid.Start.PreviousNode = null;

            visited.Add(grid.Start);

            visitedSteps.Add(new NewNodeInfo(grid.Start));

            var frontier = new MinHeap<Node>((lhs, rhs) => lhs.Cost.CompareTo(rhs.Cost));
            frontier.Add(grid.Start);

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

                        pathsSteps.Add(new NewPathInfo(current.BacktrackToPath()));
                    }

                    if (!visited.Contains(neighbor) && neighbor.IsWalkable)
                    {
                        visited.Add(neighbor);
                        frontier.Add(neighbor);

                        visitedSteps.Add(new NewNodeInfo(neighbor));
                    }
                }
            }

            return grid.End.BacktrackToPath();
        }
    }
}
