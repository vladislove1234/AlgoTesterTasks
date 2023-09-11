namespace AlgoTester.Helpers
{
    public static class ConsoleHelper
    {
        public static int ReadInt()
        {
            var input = Console.ReadLine();
            
            return int.Parse(input);
        }

        public static IEnumerable<T> ReadItems<T>(int count, Func<string, T> parser)
        {
            return Enumerable.Range(0, count).Select(_ => parser(Console.ReadLine()));
        }
    }
}