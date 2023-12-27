using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;
using static AlgoTester.Helpers.ConsoleHelper;

namespace AlgoTester.Lidar
{
    public struct Point
    {
        public double X { get; set; }

        public double Y { get; set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        public static Point operator /(Point a, double scalar)
        {
            if (scalar == 0)
                throw new ArgumentException("Division by zero");

            return new Point(a.X / scalar, a.Y / scalar);
        }

        public static Point operator *(Point a, double scalar)
        {
            return new Point(a.X * scalar, a.Y * scalar);
        }

        public static Point operator *(Point a, Point b)
        {
            return new Point(a.X * b.X, a.Y * b.Y);
        }

        public Point Copy()
        {
            return new Point(this.X, this.Y);
        }
    }

    public struct Line : IEquatable<Line>
    {
        public Point StartPoint;

        public Point EndPoint;

        public Line(Point start, Point end)
        {
            StartPoint = start;
            EndPoint = end;
        }

        public override int GetHashCode()
        {
            return StartPoint.GetHashCode() + EndPoint.GetHashCode();
        }

        public bool Equals(Line other)
        {
            var aHash = GetHashCode();

            var bHash = other.GetHashCode();

            var isEqual = aHash == bHash;

            return isEqual;
        }

        public double DistanceFromPoint(Point P)
        {
            var AB = EndPoint - StartPoint;
            var AP = P - StartPoint;

            var projection = DotProduct(AP, AB);
            var abLengthSquared = DotProduct(AB, AB);

            var d = projection / abLengthSquared;

            if (d < 0) return Distance(P, StartPoint);

            if (d > 1)
            {
                return Distance(P, EndPoint);
            }

            var closestPoint = StartPoint + AB * d;

            return Distance(P, closestPoint);
        }

        private double Distance(Point p0, Point startPoint)
        {
            var dx = p0.X - startPoint.X;
            var dy = p0.Y - startPoint.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        private static double DotProduct(Point v1, Point v2)
        {
            var result = v1.X * v2.X + v1.Y * v2.Y;

            return result;
        }
    }

    public static class PolygonHelper
    {
        private static bool OnLine(Line l1, Point p)
        {
            if (p.X <= Math.Max(l1.StartPoint.X, l1.EndPoint.X)
                && p.X >= Math.Min(l1.StartPoint.X, l1.EndPoint.X)
                && p.Y <= Math.Max(l1.StartPoint.Y, l1.EndPoint.Y)
                && p.Y >= Math.Min(l1.StartPoint.Y, l1.EndPoint.Y))
                return true;

            return false;
        }

        private static int Direction(Point a, Point b, Point c)
        {
            var val = (b.Y - a.Y) * (c.X - b.X)
                      - (b.X - a.X) * (c.Y - b.Y);

            if (val == 0)
                return 0;

            if (val < 0)
                return 2;
            
            return 1;
        }

        private static bool IsIntersect(Line l1, Line l2)
        {
            var dir1 = Direction(l1.StartPoint, l1.EndPoint, l2.StartPoint);
            var dir2 = Direction(l1.StartPoint, l1.EndPoint, l2.EndPoint);
            var dir3 = Direction(l2.StartPoint, l2.EndPoint, l1.StartPoint);
            var dir4 = Direction(l2.StartPoint, l2.EndPoint, l1.EndPoint);
            
            if (dir1 != dir2 && dir3 != dir4)
                return true;
            
            if (dir1 == 0 && OnLine(l1, l2.StartPoint))
                return true;
            
            if (dir2 == 0 && OnLine(l1, l2.EndPoint))
                return true;
            
            if (dir3 == 0 && OnLine(l2, l1.StartPoint))
                return true;
            
            if (dir4 == 0 && OnLine(l2, l1.EndPoint))
                return true;

            return false;
        }

        public static bool CheckInside(Point[] poly, int n, Point p)
        {
            if (n < 3)
                return false;

            var pt = new Point(9999, p.Y);
            var exline = new Line(p, pt);
            var count = 0;
            var i = 0;
            do
            {
                var side = new Line(poly[i], poly[(i + 1) % n]);
                if (IsIntersect(side, exline))
                {
                    if (Direction(side.StartPoint, p, side.EndPoint) == 0)
                        return OnLine(side, p);
                    count++;
                }

                i = (i + 1) % n;
            } while (i != 0);
            
            return (count & 1) == 1;
        }
    }

    public static class Program
    {
        private const int ParticlesCount = 400;

        private static readonly Random _random = new Random();

        private static void Main()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var pointsCount = ReadInt();

            var coordinates = ReadInts(pointsCount * 2).ToArray();

            var polygonPoints = new Point[pointsCount];

            for (int i = 0; i < pointsCount; i++)
            {
                polygonPoints[i] = new Point(coordinates[i * 2], coordinates[i * 2 + 1]);
            }

            var polygonLines = new List<Line>(pointsCount)
            {
                new Line(polygonPoints[0], polygonPoints[pointsCount - 1])
            };

            for (int i = 0; i < polygonPoints.Length - 1; i++)
            {
                polygonLines.Add(new Line(polygonPoints[i], polygonPoints[i + 1]));
            }

            var input = ReadInts(2);

            var movements = input[0];
            var lidarRays = input[1];

            var deviations = ReadLine().Split(" ").Select(double.Parse).ToArray();

            var sL = deviations[0];
            var sX = deviations[1];
            var sY = deviations[2];

            var particles = new List<Point>(ParticlesCount);

            var startPositionInput = ReadInts(3);

            if (startPositionInput[0] == 1)
            {
                particles = Enumerable.Range(0, ParticlesCount)
                    .Select(x => new Point(startPositionInput[1], startPositionInput[2])).ToList();
            }
            else
            {
                particles = Enumerable.Range(0, ParticlesCount).Select(x => GetPointInBounds(polygonPoints)).ToList();
            }

            var lidarVectors = SenseLidar(lidarRays);

            particles = RearrangeParticles(particles, lidarVectors, polygonLines, sL);

            for (int i = 0; i < movements; i++)
            {
                particles = MoveParticles(particles, sX, sY);

                lidarVectors = SenseLidar(lidarRays);

                particles = RearrangeParticles(particles, lidarVectors, polygonLines, sL);
            }

            WriteLine(particles.Average(x => x.X).ToString("F7") + " " + particles.Average(x => x.Y).ToString("F7"));
        }

        private static List<Point> MoveParticles(List<Point> particles, double sX, double sY)
        {
            var movementInput = ReadInts(2);

            var movement = new Point(movementInput[0], movementInput[1]);

            return particles.Select(p =>
            {
                return new Point(p.X + GenerateNormalDistribution(movement.X, sX),
                    p.Y + GenerateNormalDistribution(movement.Y, sY));
            }).ToList();
        }

        private static List<Point> RearrangeParticles(List<Point> particles, List<Point> lidarVectors,
            List<Line> polygonLines, double sL)
        {
            var weights = new double[particles.Count];

            for (int i = 0; i < particles.Count; i++)
            {
                var particle = particles[i];

                var lidarPoints = lidarVectors.Select(x => x + particle).ToList();

                var lidarPointsDistances =
                    lidarPoints.Select(x => polygonLines.Min(l => l.DistanceFromPoint(x))).ToArray();

                double deviation = Math.Sqrt(Math.Pow(lidarPointsDistances.Sum(),2) / lidarPoints.Count);

                var a = (1 / (sL * Math.Sqrt(2 * Math.PI)));

                var c = -0.5d * Math.Pow(deviation / sL, 2);

                var b = Math.Pow(Math.E,c);

                weights[i] = a * b;
            }

            Normalize(weights);
            
            var rearrangedParticles = new List<Point>(ParticlesCount);

            var maxWeightIndex = 0;

            for (int i = 0; i < particles.Count; i++)
            {
                var copies = (int)weights[i] * ParticlesCount;

                if (weights[maxWeightIndex] < weights[i])
                    maxWeightIndex = i;

                rearrangedParticles.AddRange(Enumerable.Range(0, copies).Select(x => particles[i].Copy()));
            }

            if (rearrangedParticles.Count != particles.Count)
            {
                rearrangedParticles.AddRange(Enumerable.Range(0, particles.Count - rearrangedParticles.Count)
                    .Select(x => particles[maxWeightIndex].Copy()));
            }

            return rearrangedParticles;
        }

        private static void Normalize(double[] weights)
        {
            var sum = weights.Sum() <= 0 ? 1 : weights.Sum();

            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] /= sum;
            }
        }

        private static List<Point> SenseLidar(int lidarRays)
        {
            lidarRays /= 4;
            var lidarVectors = ReadLine().Split(" ").Select(double.Parse).ToArray();

            return lidarVectors.Where((x,i) => i % 4 == 0).Select((x, i) =>
                    new Point(Math.Cos(i * 2 *Math.PI / lidarRays), Math.Sin(i * 2 * Math.PI / lidarRays)) * x)
                .ToList();
        }

        private static Point GetPointInBounds(Point[] points)
        {
            var minX = points.Min(x => x.X);
            var minY = points.Min(x => x.Y);
            
            var maxX = points.Max(x => x.X);
            var maxY = points.Max(x => x.Y);

            var point = new Point(minX, minY);

            point.X += (maxX - minX) * _random.NextDouble();
            point.Y += (maxY - minY) * _random.NextDouble();

            if (!PolygonHelper.CheckInside(points, points.Length, point))
            {
                return GetPointInBounds(points);
            }

            return point;
        }
        
        public static double GenerateNormalDistribution(double mean, double stdDev)
        {
            var u1 = 1.0 - _random.NextDouble();
            var u2 = 1.0 - _random.NextDouble();
            
            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                Math.Sin(2.0 * Math.PI * u2);
            
            var randNormal =
                mean + stdDev * randStdNormal;

            return randNormal;
        }
    }
}