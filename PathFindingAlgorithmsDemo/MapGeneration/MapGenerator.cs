using System;

namespace PathFindingAlgorithmsDemo.MapGeneration
{
    //source: https://github.com/klaytonkowalski/example-cellular-automata/blob/master/example/main.script

    public class MapGenerator
    {
        private bool _useSeed;

        public MapGenerator(int width, int height)
        {
            Width = width;
            Height = height;
            Seed = "";
            Map = new bool[Width, Height];
            _useSeed = false;
        }

        public int Width { get; }
        public int Height { get; }
        public string Seed { get; private set; }
        public bool[,] Map { get; }

        public MapGenerator SetSeed(string seed)
        {
            _useSeed = true;
            Seed = seed;

            return this;
        }

        public MapGenerator GenerateNoise(int density = 50)
        {
            density = density < 0 ? 0 : density > 100 ? 100 : density;

            var r = new Random(_useSeed ? Seed.GetHashCode() : 0);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Map[x, y] = r.Next(0, 100) < density;
                }
            }

            return this;
        }

        private bool[,] GetMapCopy()
        {
            var copy = new bool[Width, Height];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    copy[i, j] = Map[i, j];
                }
            }

            return copy;
        }

        public MapGenerator PerformCellularAutomata(int iterations)
        {
            iterations = iterations < 0 ? 0 : iterations;

            for (int i = 0; i < iterations; i++)
            {
                var mapIteration = GetMapCopy();

                for (int j = 0; j < Height; j++)
                {
                    for (int k = 0; k < Width; k++)
                    {
                        var neighborWallCount = 0;
                        var isBorder = false;

                        for (int y = j - 1; y <= j + 1; y++)
                        {
                            for (int x = k - 1; x <= k + 1; x++)
                            {
                                if (y >= 0 && x >= 0 && y < Height && x < Width)
                                {
                                    if (y != j || x != k)
                                    {
                                        if (mapIteration[x, y] == true)
                                        {
                                            neighborWallCount += 1;
                                        }
                                    }
                                    
                                }
                                else
                                {
                                    isBorder = true;
                                }
                            }
                        }

                        Map[k, j] = neighborWallCount > 4 || isBorder;
                    }
                }
            }

            return this;
        }
    }
}
