using System.Collections.Generic;

namespace PathFindingAlgorithmsDemo
{
    public class NodeGrid
    {
        private readonly Node[] _nodes;

        public NodeGrid(int width, int height)
        {
            Width = width;
            Height = height;
            _nodes = new Node[Width * Height];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    var node = new Node(this, i, j, 1);
                    var index = GetNodeIndex(i, j);
                    _nodes[index] = node;
                }
            }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Node Start { get; set; }
        public Node End { get; set; }

        public Node this[int x, int y]
        {
            get => IsInBounds(x, y) ? _nodes[GetNodeIndex(x, y)] : _nodes[0];
            private set
            {
                if (IsInBounds(x, y))
                {
                    _nodes[GetNodeIndex(x, y)] = value;
                }
            }
        }

        private int GetNodeIndex(int x, int y)
        {
            return x + (y * Width);
        }

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public void ClearWalls()
        {
            for (int i = 0; i < _nodes.Length; i++)
            {
                _nodes[i].IsWalkable = true;
            }
        }

        public void ResetCosts()
        {
            foreach (var node in _nodes)
            {
                node.PreviousNode = null;
                node.Cost = 0;
            }
        }

        public void SetCosts(int value)
        {
            foreach (var node in _nodes)
            {
                node.Cost = value;
            }
        }

        public IEnumerable<Node> GetNeighbors(Node node)
        {
            if (IsInBounds(node.X, node.Y + 1))
            {
                yield return _nodes[GetNodeIndex(node.X, node.Y + 1)];
            }

            if (IsInBounds(node.X, node.Y - 1))
            {
                yield return _nodes[GetNodeIndex(node.X, node.Y - 1)];
            }

            if (IsInBounds(node.X + 1, node.Y))
            {
                yield return _nodes[GetNodeIndex(node.X + 1, node.Y)];
            }

            if (IsInBounds(node.X - 1, node.Y))
            {
                yield return _nodes[GetNodeIndex(node.X - 1, node.Y)];
            }
        }
    }
}
