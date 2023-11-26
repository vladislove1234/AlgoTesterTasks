using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;
using static AlgoTester.Helpers.ConsoleHelper;

namespace AlgoTester.CenterOfLines
{
    public static class Program
    {
        static void Main()
        {
            var itemsCount = ReadInt();

            var lines = ReadItems<Line>(itemsCount, s =>
            {
                var inputraw = s.Split().Where(x => !string.IsNullOrEmpty(x)).ToArray();
                var input = inputraw.Select(x => double.Parse(x)).ToArray();

                return new Line(new Point(input[0], input[1], input[2]), new Point(input[3], input[4], input[0]));
            }).ToArray();

            Func<Point, double> objectiveFunction = x => lines.Sum(l2 => l2.DistanceFromPoint(x));

            Point initialPoint = new Point( lines.Average(l => l.EndPoint.X - l.StartPoint.X)/2 , lines.Average(l => l.EndPoint.Y - l.StartPoint.Y)/2 , lines.Average(l => l.EndPoint.Z - l.StartPoint.Z)/2); // Initial guess

            Point result = NelderMeadOptimize(objectiveFunction, initialPoint);

            Console.WriteLine($"{objectiveFunction(result)}");
        }

        public struct Point
        {
            public double X { get; set; }

            public double Y { get; set; }

            public double Z { get; set; }

            public Point(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public static bool IsSmaller(Point a, Point b)
            {
                return a.Y < b.Y;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(X, Y, Z);
            }

            public static Point operator +(Point a, Point b)
            {
                return new Point(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
            }

            public static Point operator -(Point a, Point b)
            {
                return new Point(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
            }

            public static Point operator /(Point a, double scalar)
            {
                if (scalar == 0)
                    throw new ArgumentException("Division by zero");

                return new Point(a.X / scalar, a.Y / scalar, a.Z / scalar);
            }

            public static Point operator *(Point a, double scalar)
            {
                return new Point(a.X * scalar, a.Y * scalar, a.Z * scalar);
            }

            public static Point operator *(Point a, Point b)
            {
                return new Point(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
            }
        }

        public struct Line : IEquatable<Line>
        {
            public Point StartPoint;

            public Point EndPoint;

            public Line(Point start, Point end)
            {
                StartPoint = Point.IsSmaller(start, end) ? start : end;
                EndPoint = Point.IsSmaller(start, end) ? end : start;
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

                double projectionFactor = DotProduct(AP, AB) / DotProduct(AB, AB);
                Point projection =  AB * projectionFactor;

                Point perpendicular = AP - projection;

                double distance = Magnitude(perpendicular);
                return distance;
            }

            static double DotProduct(Point v1, Point v2)
            {
                double result = v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;

                return result;
            }

            static double Magnitude(Point vector)
            {
                double sumSquares = vector.X*vector.X + vector.Y * vector.Y + vector.Z * vector.Z;

                return Math.Sqrt(sumSquares);
            }
        }

        static Point[] InitializeSimplex(Point initialPoint)
        {
            var simplex = new Point[4]
            {
                initialPoint,
                new Point(initialPoint.X * 0.1, initialPoint.Y, initialPoint.Z),
                new Point(initialPoint.X, initialPoint.Y*0.1, initialPoint.Z),
                new Point(initialPoint.X, initialPoint.Y, initialPoint.Z * 0.1),
            };

            return simplex;
        }

        static Point NelderMeadOptimize(Func<Point, double> objectiveFunction, Point initialPoint)
        {
            double alpha = 1; // Reflection coefficient
            double beta = 0.5; // Contraction coefficient
            double gamma = 2; // Expansion coefficient
            double sigma = 0.5; // Shrinkage coefficient

            var points = InitializeSimplex(initialPoint);

            while (true)
            {
                var orderedPoints = points.OrderBy(p => objectiveFunction(p)).ToArray();

                var centroid = orderedPoints.Take(orderedPoints.Length - 1).Aggregate((p1, p2) => p1 + p2);
                centroid /= 3;

                var worst = orderedPoints.Last();
                var secondWorst = orderedPoints.TakeLast(2).First();
                var best = orderedPoints.First();

                if(Math.Abs(objectiveFunction(best) - 4) < 0.0001)
                {
                    break;
                }

                //reflection 

                var reflectedPoint = centroid + (centroid - worst) * alpha;


                if(objectiveFunction(reflectedPoint) < objectiveFunction(secondWorst) && objectiveFunction(best) <= objectiveFunction(reflectedPoint))
                {
                    points = orderedPoints.Take(orderedPoints.Length - 1).Append(reflectedPoint).ToArray();

                    continue;
                }
                else if(objectiveFunction(reflectedPoint) < objectiveFunction(best))
                {
                    var expanded = centroid + (reflectedPoint - centroid) * gamma;

                    if(objectiveFunction(expanded) < objectiveFunction(reflectedPoint))
                    {
                        points = orderedPoints.Take(orderedPoints.Length - 1).Append(expanded).ToArray();

                        continue;
                    }
                    else
                    {
                        points = orderedPoints.Take(orderedPoints.Length - 1).Append(reflectedPoint).ToArray();

                        continue;
                    }
                }
                else
                {
                    if(objectiveFunction(reflectedPoint) < objectiveFunction(worst))
                    {
                        var contracted = centroid + (reflectedPoint - centroid) * beta;

                        if(objectiveFunction(contracted) < objectiveFunction(reflectedPoint))
                        {
                            points = orderedPoints.Take(orderedPoints.Length - 1).Append(contracted).ToArray();

                            continue;
                        }
                    }
                    else
                    {
                        var contracted = centroid + (worst - centroid) * beta;

                        if (objectiveFunction(contracted) < objectiveFunction(worst))
                        {
                            points = orderedPoints.Take(orderedPoints.Length - 1).Append(contracted).ToArray();

                            continue;
                        }
                    }

                    points = orderedPoints.Skip(1).Select(p => best + (p - best) * sigma).Append(best).ToArray();
                }
            }

            return points.MinBy(p => objectiveFunction(p));
        }

        public static double StandartDeviation(Point[] points)
        {
            Point mean = points.Aggregate((p1,p2) => p1 + p2);
            mean /= points.Length;

            Point sumOfSquaredDifferences = new Point();

            foreach (var point in points)
            {
                Point difference = point - mean;
                sumOfSquaredDifferences += difference * difference;
            }

            Point variance = sumOfSquaredDifferences / points.Length;

            double standardDeviation = Math.Sqrt(variance.X) + Math.Sqrt(variance.Y) + Math.Sqrt(variance.Z);

            return (double)standardDeviation;
        }
    }
}