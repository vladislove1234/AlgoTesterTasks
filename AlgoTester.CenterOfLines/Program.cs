using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;
using static AlgoTester.Helpers.ConsoleHelper;

namespace AlgoTester.CenterOfLines
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var countOfLines = ReadInt();

            var lines = ReadItems(countOfLines, s =>
            {
                var coordinates = s.Split(' ').Select(float.Parse).ToArray();

                return new Line(new Point(coordinates[0], coordinates[1], coordinates[2]),
                    new Point(coordinates[3], coordinates[4], coordinates[5]));
            });
        }
    }

    public struct Line
    {
        public Point Start { get; }
        public Point End { get; }

        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }
    }

    public struct Point
    {
        public float X { get; }

        public float Y { get; }

        public float Z { get; }
        
        public Point(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
    }
}