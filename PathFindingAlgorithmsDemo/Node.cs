using System;
using System.Numerics;
using System.Collections.Generic;

namespace PathFindingAlgorithmsDemo
{
    public class Node
    {
        static Node()
        {
            Size = 4;
        }

        public static int Size { get; }

        private static float GetEuclideanHeuristicCost(Node current, Node end)
        {
            var start = current == null ? new Vector2() : current.ToVector2();
            var finish = end == null ? new Vector2() : end.ToVector2();
            return (start - finish).Length();
        }

        public Node(NodeGrid grid, int x, int y, int weight)
        {
            Grid = grid;
            X = x;
            Y = y;
            Weight = weight;
            IsWalkable = true;

            HeuristicComparison = (lhs, rhs) =>
            {
                var lhsCost = lhs.Cost + GetEuclideanHeuristicCost(lhs, this);
                var rhsCost = rhs.Cost + GetEuclideanHeuristicCost(rhs, this);

                return lhsCost.CompareTo(rhsCost);
            };
        }

        public NodeGrid Grid { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Weight { get; private set; }
        public bool IsWalkable { get; private set; }
        public bool IsExpensive => Weight == NodeGrid.ExpensiveNodeWeight && IsWalkable;
        public int Cost { get; set; }
        public Node PreviousNode { get; set; }
        public Comparison<Node> HeuristicComparison { get; }

        public void SetToDefault()
        {
            IsWalkable = true;
            Weight = NodeGrid.DefaultNodeWeight;
        }

        public void SetToExpensive()
        {
            IsWalkable = true;
            Weight = NodeGrid.ExpensiveNodeWeight;
        }

        public void SetToWall()
        {
            IsWalkable = false;
            Weight = NodeGrid.DefaultNodeWeight;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        public List<Node> BacktrackToPath()
        {
            var current = this;
            var path = new List<Node>();

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
