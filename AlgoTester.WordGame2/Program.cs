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

namespace AlgoTester.WordGame2
{
    public static class Program
    {
        private const int WrongAnswer = -1;

        public static void Main()
        {
            try
            {
                var wordsCount = ReadInt();
                var words = ReadItems(wordsCount, str => str).ToList();

                var path = words.GetBfsShortestPath(CanGoToNextWord, words.First(), words.Last());
                
                WriteLine(path.Count);
            }
            catch
            {
                WriteLine(WrongAnswer);
            }
        }

        private static bool CanGoToNextWord(string currentWord, string nextWord)
        {
            return currentWord[currentWord.Length - 2] == nextWord[1];
        }
    }
}