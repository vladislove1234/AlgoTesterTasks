using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AlgoTester.Helpers;
using static System.Console;
using static AlgoTester.Helpers.ConsoleHelper;

namespace AlgoTester.End
{
    public struct Column
    {
        public int X { get; }
        public int Y { get; }
        
        public double Radius { get; }

        public Column(int x, int y, double radius)
        {
            X = x;
            Y = y;

            Radius = radius;
        }
        
        public bool IsEnoughDistanceBetweenColumn(Column second, double requiredDistance)
        {
            var distance = Math.Sqrt(Math.Pow(X - second.X, 2) + Math.Pow(Y - second.Y, 2));

            return distance >= Radius + second.Radius + requiredDistance;
        }
    }
    
    public static class Program
    {
        public static void Main()
        {
            var fieldPoints = ReadInts(2);
            
            var startX = fieldPoints[0];
            var endX = fieldPoints[1];

            var radius = ReadInt();
            var columnsCount = ReadInt();

            var columns = ReadItems(columnsCount, str =>
            {
                var points = str.Split(" ")
                    .Take(2)
                    .Select(int.Parse)
                    .ToArray();

                return new Column(points[0], points[1], radius);
            }).ToArray();
            
            var maximumDiameter = BinarySearchHelper.FindMaxValue(
                diameter => CanFitBetweenColumns(diameter, columns, startX, endX),
                1,
                (endX - startX),
                0.0001);
            
            WriteLine(maximumDiameter);
        }

        private static bool CanFitBetweenColumns(double diameter, Column[] columns, int startX, int endX)
        {
            var result = GraphsHelper.HasPathBfs(columns, (c1, c2) => !c1.IsEnoughDistanceBetweenColumn(c2, diameter),
                (c) => c.X - c.Radius - diameter <= startX, (c) => c.X +c.Radius + diameter >= endX);

            return !result;
        }
    }
}