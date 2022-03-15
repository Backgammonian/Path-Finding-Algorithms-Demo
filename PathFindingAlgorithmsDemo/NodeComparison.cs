using System;

namespace PathFindingAlgorithmsDemo
{
    public class NodeComparison
    {
        private readonly Node _end;

        public NodeComparison(Node end)
        {
            _end = end;

            HeuristicComparison = (lhs, rhs) =>
            {
                var lhsCost = lhs.Cost + Node.GetEuclideanHeuristicCost(lhs, _end);
                var rhsCost = rhs.Cost + Node.GetEuclideanHeuristicCost(rhs, _end);

                return lhsCost.CompareTo(rhsCost);
            };
        }

        public Comparison<Node> HeuristicComparison { get; }
    }
}
