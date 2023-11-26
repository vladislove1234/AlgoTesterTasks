using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;
using static AlgoTester.Helpers.ConsoleHelper;

namespace AlgoTester.Lab
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var pointsCount = ReadInt();
            
            var points = ReadItems(pointsCount, str => str.Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToArray())
                .Select(x => new Point(double.Parse(x[0]), double.Parse(x[1]), double.Parse(x[2])))
                .ToArray();

            var A = new double[3,3];

            A[0, 0] = points.Sum(p => p.X * p.X);
            A[0, 1] = A[1,0] = points.Sum(p => p.X * p.Y);
            A[1, 1] = points.Sum(p => p.Y * p.Y);
            A[0,2] = A[2,0] = points.Sum(p => p.X);
            A[2,1] = A[1,2] = points.Sum(p => p.Y);
            A[2, 2] = pointsCount;
            
            var B = new double[3];
            
            B[0] = points.Sum(p => p.X * p.Z);
            B[1] = points.Sum(p => p.Y * p.Z);
            B[2] = points.Sum(p => p.Z);

            var X = SolveSystem(A, B);
            
            WriteLine($"{X[0]} {X[1]} {X[2]}");
        }
        
        static double[] SolveSystem(double[,] A, double[] b)
        {
            int n = A.GetLength(0);
            double[] result = new double[n];
            
            for (int i = 0; i < n; i++)
            {
                for (int k = i + 1; k < n; k++)
                {
                    double factor = A[k, i] / A[i, i];
                    b[k] -= factor * b[i];
                    for (int j = i; j < n; j++)
                    {
                        A[k, j] -= factor * A[i, j];
                    }
                }
            }
            
            for (int i = n - 1; i >= 0; i--)
            {
                double sum = 0;
                for (int j = i + 1; j < n; j++)
                {
                    sum += A[i, j] * result[j];
                }
                result[i] = (b[i] - sum) / A[i, i];
            }

            return result;
        }
    }
    
    public struct Point
    {
        public double X { get; }
        public double Y { get; }
        
        public double Z { get; }

        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}