using static System.Console;
using static AlgoTester.Helpers.ConsoleHelper;

namespace AlgoTester.HardChoice
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var input = ReadInts(2);

            var start = input[0];
            var end = input[1];

            double startVolume = GetVolumeOfTriangle(start);
            double endVolume = GetVolumeOfTriangle(end);

            WriteLine((endVolume - startVolume));
        }

        private static double GetVolumeOfTriangle(int start)
        {
            var side = start * Math.Sqrt(2);

            var radius = side / (2 * Math.Sqrt(3)/2);
            
            var height = Math.Sqrt(Math.Pow(start,2) - Math.Pow(radius, 2));

            var area = Math.Pow(side,2) * Math.Sqrt(3)/4;
            
            return 1/3d * area * height;
        }
    }

    public struct Point
    {
        public double X { get; }
        public double Y { get; }
        
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}