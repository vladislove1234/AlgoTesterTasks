using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;
using static AlgoTester.Helpers.ConsoleHelper;

namespace AlgoTester.CenterOfEarth
{
    public static class Program
    {
        public const double E = 1e-7;
        
        static void Main(string[] args)
        {
            var pointsCount = ReadInt();

            var points = ReadItems(pointsCount, str => str.Split(' '))
                .Select(x => new Point(double.Parse(x[0]), double.Parse(x[1])))
                .ToArray();

            Point result = GetGeometricalCenter(points);

            WriteLine(points.Sum(x => Distance(x, result)));
        }

        static Point GetGeometricalCenter(Point[] points)
        {
            Point current = new Point(points.Average(p => p.X), points.Average(p => p.Y));
            Point previous = current;

            do
            {
                double sumWeights = 0.0;
                double sumX = 0.0;
                double sumY = 0.0;

                foreach (Point point in points)
                {
                    double distance = Distance(current, point);
                    double weight = distance > E ? 1.0 / distance : 0.0;

                    sumWeights += weight;
                    sumX += weight * point.X;
                    sumY += weight * point.Y;
                }

                Point next = new Point(sumX / sumWeights, sumY / sumWeights);

                previous = current;
                current = next;
                
            } while (Distance(previous, current) >= E);

            return current;
        }

        static double Distance(Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
    
    class Point
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