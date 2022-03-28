using System.Collections.Generic;

namespace PathFindingAlgorithmsDemo
{
    public struct NewPathInfo
    {
        public NewPathInfo(List<Node> path)
        {
            Path = path;
        }

        public List<Node> Path { get; }
    }
}
