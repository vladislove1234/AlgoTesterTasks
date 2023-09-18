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

namespace AlgoTester.Trilandia
{
    public static class Program
    {
        private const int ErrorAnswer = -1;
        private const string DefaultSeparator = " ";

        public static void Main()
        {
            try
            {
                var trianglesCount = ReadInt();

                var startEndPoints = ReadInts(2);

                var startPoint = startEndPoints[0] - 1;
                var endPoint = startEndPoints[1] - 1;
                
                var points = ReadItems(trianglesCount * 3, str =>
                {
                    var coordinates = str.Split(DefaultSeparator)
                        .Select(float.Parse)
                        .ToArray();

                    return new Point(coordinates[0], coordinates[1], coordinates[2]);
                }).ToArray();
                
                var triangles = GetTriangles(points).ToArray();
                
                var path = triangles.GetBfsShortestPath((a,b) => a.IsConnectedWithTriangle(b),
                    triangles[startPoint],
                    triangles[endPoint]);
                
                WriteLine(path.Count );
            }
            catch (Exception e)
            {
                WriteLine(ErrorAnswer);
            }
        }

        private static IEnumerable<Triangle> GetTriangles(Point[] points)
        {
            for(int i = 0; i < points.Length / 3; i++)
            {
                var first = points[i * 3];
                var second = points[i * 3 + 1];
                var third = points[i * 3 + 2];

                yield return new Triangle(
                    new Side(first, second),
                    new Side(second, third),
                    new Side(third, first)
                );
            }
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
            return HashCode.Combine(X.GetHashCode(), Y.GetHashCode(), Z.GetHashCode());
        }
    }
    
    public struct Side : IEquatable<Side>
    {
        public Point StartPoint;

        public Point EndPoint;

        public Side(Point start, Point end)
        {
            StartPoint = start;
            EndPoint = end;
        }

        public override int GetHashCode()
        {
            return StartPoint.GetHashCode() * EndPoint.GetHashCode();
        }

        public bool Equals(Side other)
        {
            var aHash = GetHashCode();

            var bHash = other.GetHashCode();

            var isEqual = aHash == bHash;

            return isEqual;
        }
    }

    public struct Triangle
    {
        public HashSet<Side> Sides;
        public Triangle(Side first, Side second, Side third)
        {
            Sides = new HashSet<Side>()
            {
                first, second, third
            };
        }

        public bool IsConnectedWithTriangle(Triangle other)
        {
            foreach (var side in Sides)
            {
                if (other.Sides.Contains(side))
                {
                    return true;
                }
            }

            return false;
        }
    }
}