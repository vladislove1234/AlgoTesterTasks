namespace AlgoTester.Helpers
{
    public static class ConsoleHelper
    {
        private const string DefaultSeparator = " ";
        
        public static int ReadInt()
        {
            var input = Console.ReadLine();
            
            return int.Parse(input);
        }
        
        public static int[] ReadInts(int count)
        {
            var input = Console.ReadLine();
            
            return input.Split(DefaultSeparator)
                .Take(count)
                .Select(int.Parse)
                .ToArray();
        }

        public static IEnumerable<T> ReadItems<T>(int count, Func<string, T> parser)
        {
            return Enumerable.Range(0, count).Select(_ => parser(Console.ReadLine()));
        }
        
        public static void ReadItems(int count, Action<string> delegateAction)
        {
            for (int i = 0; i < count; i++)
            {
                delegateAction(Console.ReadLine());   
            }
        }
    }
}