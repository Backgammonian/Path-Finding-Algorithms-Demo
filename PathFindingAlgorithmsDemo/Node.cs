﻿using System.Numerics;

namespace PathFindingAlgorithmsDemo
{
    public class Node
    {
        static Node()
        {
            Size = 8;
        }

        public static int Size { get; }

        public static float GetEuclideanHeuristicCost(Node current, Node end)
        {
            var start = current == null ? new Vector2() : current.ToVector2();
            var finish = end == null ? new Vector2() : end.ToVector2();
            return (start - finish).Length();
            //return (float)System.Math.Sqrt((start.X - finish.X) * (start.X - finish.X) + (start.Y - finish.Y) * (start.Y - finish.Y));
        }

        public Node(NodeGrid grid, int x, int y, int weight)
        {
            Grid = grid;
            X = x;
            Y = y;
            Weight = weight;
            IsWalkable = true;
        }

        public NodeGrid Grid { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Weight { get; private set; }
        public int Cost { get; set; }
        public bool IsWalkable { get; set; }
        public Node PreviousNode { get; set; }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }
    }
}
