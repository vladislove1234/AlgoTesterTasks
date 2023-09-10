namespace AlgoTester.Helpers
{
    public static class GraphsHelper
    {
        public static List<T> GetBfsShortestPath<T>(this IEnumerable<T> items, Func<T, T, bool> isConnectedPredicate,
            T start, T end)
        {
            var queue = new Queue<T>();
            var visited = new HashSet<T>();
            var parents = new List<(T Parent, T Child)>();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current.Equals(end))
                {
                    break;
                }

                var childrenItems = items.Where(x => !visited.Contains(x) && isConnectedPredicate(current, x))
                    .ToList();

                foreach (var item in childrenItems)
                {
                    queue.Enqueue(item);
                    visited.Add(item);

                    parents.Add((current, item));

                    if (item.Equals(end))
                    {
                        break;
                    }
                }
            }

            var path = new List<T>();

            if (!parents.Any(parentPair => parentPair.Child.Equals(end)))
            {
                if (visited.Contains(end))
                {
                    return path;
                }

                throw new InvalidOperationException("No path found");
            }

            var currentItem = end;

            while (!currentItem.Equals(start))
            {
                path.Add(currentItem);

                currentItem = parents.First(parentPair => parentPair.Child.Equals(currentItem)).Parent;
            }

            return path;
        }
    }
}