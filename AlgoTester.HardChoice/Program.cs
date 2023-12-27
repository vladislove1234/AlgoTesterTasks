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

namespace AlgoTester.HardChoice
{
    public static class Program
    {
        private const int Iterations = 10000;
        
        static void Main(string[] args)
        {
            var input = Console.ReadLine().Split(' ').Select(double.Parse).ToArray();

            var start = input[0];
            var end = input[1];
            
            var probability = GetProbability(start, end);

            WriteLine(probability);
        }

        private static double GetProbability(double start, double end)
        {
            var range = (end-start)/Iterations;
            var probability = (IrwinHallPdf(end) + IrwinHallPdf(start)) / 2;

            for(int i = 1; i < Iterations; i++)
            {
                probability += IrwinHallPdf(start + i * range);
            }

            return probability * range;
        }

        private static double IrwinHallPdf(double x)
        {
            if (x < 1)
            {
                return 0.5d * Math.Pow(x,2);
            }

            if (x < 2)
            {
                return 0.5d * (-2 * Math.Pow(x, 2) + 6 * x - 3);
            }

            else
            {
                return 0.5d * Math.Pow(3 - x, 2);
            }
        }
    }
}