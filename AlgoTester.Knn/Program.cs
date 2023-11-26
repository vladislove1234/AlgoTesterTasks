using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;
using static AlgoTester.Helpers.ConsoleHelper;

namespace AlgoTester.Knn
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var inputs = ReadInts(3);
            
            var classesCount = inputs[0];
            var neighboursCount = inputs[1];
            var classifiesCount = inputs[2];
            
            var outputString = string.Empty;

            var classesItems = new Dictionary<Postcard, int>();

            for (int i = 0; i < classesCount; i++)
            {
                var input = ReadLine().Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray();

                var postCard = new Postcard(int.Parse(input[0]), int.Parse(input[1]));

                classesItems[postCard] = i + 1;
            }
            
            for (int i = 0; i < classifiesCount; i++)
            {
                var input = ReadLine().Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray();

                var postCard = new Postcard(int.Parse(input[0]), int.Parse(input[1]));

                var closestClasses = classesItems.OrderBy(x => x.Key.DistanceTo(postCard));

                var classesPoints = new Dictionary<int, int>();
                
                var counter = 0;

                var lastDistance = -1.0d;
                
                foreach (var closestClass in closestClasses)
                {
                    if (counter >= neighboursCount && lastDistance != closestClass.Key.DistanceTo(postCard))
                    {
                        break;
                    }
                    
                    classesPoints[closestClass.Value] = classesPoints.TryGetValue(closestClass.Value, out var points) ? points + 1 : 1;
                    
                    lastDistance = closestClass.Key.DistanceTo(postCard);

                    counter++;
                }

                var closestClass1 = classesPoints.OrderByDescending(x => x.Value).ThenBy(x => x.Key).First().Key;

                classesItems[postCard] = closestClass1;

                outputString += closestClass1 + " ";
            }
            
            WriteLine(outputString);
        }

        public struct Postcard
        {
            public int Height { get; set; }
            public int Width { get; set; }
            
            public Postcard(int height, int width)
            {
                Height = height;
                Width = width;
            }

            public double DistanceTo(Postcard other)
            {
                return Math.Max(Math.Abs(other.Width - Width), Math.Abs(other.Height - Height));
            }

            public bool Equals(Postcard other)
            {
                return Height == other.Height && Width == other.Width;
            }

            public override int GetHashCode()
            {
                return Height.GetHashCode() ^ Width.GetHashCode();
            }
        }
    }
}