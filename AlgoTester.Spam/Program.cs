using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;
using static AlgoTester.Helpers.ConsoleHelper;

namespace AlgoTester.Spam
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var messagesCounts = ReadInts(3);
            
            var normalMessages = messagesCounts[0];
            var spamMessages = messagesCounts[1];
            var classifyMessages = messagesCounts[2];
            
            var normalWords = ReadItems(normalMessages, str => str.Split(' ').AsEnumerable()).ToArray();
            var spamWords = ReadItems(spamMessages, str => str.Split(' ').AsEnumerable()).ToArray();
            
            var normalWordsProbability = normalWords.GroupBy(w => w)
                .ToDictionary(g => g.Key, g => (double)g.Count() / normalWords.Length);
            
            var spamWordsProbability = spamWords.GroupBy(w => w)
                .ToDictionary(g => g.Key, g => (double)g.Count() / spamWords.Length);

            for (int i = 0; i < classifyMessages; i++)
            {
                var message = ReadLine().Split(' ');
                
                var normalProbability = message.Select(w => normalWordsProbability.TryGetValue(w, out var probability) ? probability : 0)
                    .Aggregate((double)normalMessages/(normalMessages+spamMessages), (p, w) => p * w);
                
                var spamProbability = message.Select(w => spamWordsProbability.TryGetValue(w, out var probability) ? probability : 0)
                    .Aggregate((double)spamMessages/(normalMessages+spamMessages), (p, w) => p * w);
                
                WriteLine(spamProbability/(spamProbability + normalProbability));
            }
        }
    }
}