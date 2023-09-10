namespace AlgoTester.Helpers
{
    public static class ConsoleHelper
    {
        public static int ReadInt()
        {
            return int.Parse(Console.ReadLine());
        }

        public static IEnumerable<T> ReadItems<T>(int count, Func<string, T> parser)
        {
            return Enumerable.Range(0, count).Select(_ => parser(Console.ReadLine()));
        }
    }
}