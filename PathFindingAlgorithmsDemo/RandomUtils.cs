using System;

namespace PathFindingAlgorithmsDemo
{
    public static class RandomUtils
    {
        private static readonly Random _r = new Random();
        private static readonly string _chars = "1234567890qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";

        public static int Next(int minValue, int maxValue)
        {
            return _r.Next(minValue, maxValue);
        }

        public static string GetRandomString(int length)
        {
            var result = "";

            for (int i = 0; i < length; i++)
            {
                result += _chars[Next(0, _chars.Length)];
            }

            return result;
        }
    }
}
