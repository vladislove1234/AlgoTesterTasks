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

namespace AlgoTester.ToiletMan;

public static class Program
{
    private const int ErrorAnswer = -1;

    public static void Main()
    {
        var roomsPointers = ReadInts(2);

        var roomsCount = roomsPointers[0];
        var pointersCount = roomsPointers[1];

        var roomsPointersDict = new Dictionary<int, HashSet<int>>(pointersCount);

        ReadItems(pointersCount, str =>
        {
            var items = str.Split(" ").Select(int.Parse).ToArray();

            CreateOrAddToList(roomsPointersDict, items[0] - 1, items[1] - 1);
        });

        var roomsArray = Enumerable.Range(0, roomsCount).ToArray();

        for (var i = 0; i < roomsCount; i++)
        {
            if (roomsArray.CanReachAllItems(room => GetPointeredItems(room, roomsPointersDict) ,i))
            {
                WriteLine(i + 1);

                return;
            }
        }

        WriteLine(ErrorAnswer);
    }

    private static IEnumerable<int> GetPointeredItems(int rooms, Dictionary<int, HashSet<int>> roomPointers)
    {
        if (roomPointers.TryGetValue(rooms, out var value))
        {
            return value;
        }
        
        return Enumerable.Empty<int>();
    }

    private static void CreateOrAddToList(Dictionary<int, HashSet<int>> dictionary, int room, int pointer)
    {
        if (dictionary.TryGetValue(room, out var value))
        {
            value.Add(pointer);
        }
        else
        {
            dictionary.Add(room, new HashSet<int>{pointer});
        }
    }
}