namespace AlgoTester.Helpers
{
    public static class GraphsHelper
    {
        public static List<T> GetBfsShortestPath<T>(this IEnumerable<T> items, Func<T, T, bool> isConnectedPredicate,
            T start, T end)
        {
            var queue = new Queue<T>();
            var visited = new HashSet<T>();
            var parents = new List<ParentChildrenPair<T>>();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current.Equals(end))
                {
                    break;
                }

                var childrenItems = items.Where(x => !visited.Contains(x) && isConnectedPredicate(current, x));

                foreach (var item in childrenItems)
                {
                    queue.Enqueue(item);
                    visited.Add(item);

                    parents.Add(new ParentChildrenPair<T>(current, item));

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
        
        public static List<T> GetBfsShortestPath<T>(Func<T, IEnumerable<T>> getNeighbourItems,
            T start, T end)
        {
            var queue = new Queue<T>();
            var visited = new HashSet<T>();
            var parents = new List<ParentChildrenPair<T>>();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current.Equals(end))
                {
                    break;
                }

                var childrenItems = getNeighbourItems(current).Where(x => !visited.Contains(x));

                foreach (var item in childrenItems)
                {
                    queue.Enqueue(item);
                    visited.Add(item);

                    parents.Add(new ParentChildrenPair<T>(current, item));

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
        
        public static List<T> GetBfsShortestPath<T>(Func<T, IEnumerable<T>> getNeighbourItems,
            IEnumerable<T> start, IEnumerable<T> end)
        {
            var queue = new Queue<T>(start);
            var visited = new HashSet<T>(start);
            var parents = new List<ParentChildrenPair<T>>();
            
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (end.Contains(current))
                {
                    break;
                }

                var childrenItems = getNeighbourItems(current).Where(x => !visited.Contains(x));

                foreach (var item in childrenItems)
                {
                    queue.Enqueue(item);
                    visited.Add(item);

                    parents.Add(new ParentChildrenPair<T>(current, item));

                    if (end.Contains(item))
                    {
                        break;
                    }
                }
            }

            var path = new List<T>();

            if (!parents.Any(parentPair => end.Contains(parentPair.Child)))
            {
                if (end.Contains(visited.Last()))
                {
                    return path;
                }

                throw new InvalidOperationException("No path found");
            }

            var currentItem = visited.Last();

            path.Add(currentItem);
            
            while (!start.Contains(currentItem))
            {
                currentItem = parents.First(parentPair => parentPair.Child.Equals(currentItem)).Parent;
                
                path.Add(currentItem);
            }

            return path;
        }
        
        public static bool HasPathBfs<T>(this IEnumerable<T> items, 
            Func<T,T, bool> isConnectedPredicate, 
            Func<T, bool> isStartItem,
            Func<T,bool> isLastItem)
        {
            var queue = new Queue<T>();
            var visited = new HashSet<T>();
            var lastItems = new HashSet<T>();

            foreach (var item in items)
            {
                if (isStartItem(item))
                {
                    queue.Enqueue(item);
                    visited.Add(item);
                }
                
                if (isLastItem(item))
                {
                    lastItems.Add(item);
                }
            }

            if (!lastItems.Any())
            {
                return false;
            }

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (lastItems.Contains(current))
                {
                    return true;
                }

                var childrenItems = items.Where(x => !visited.Contains(x) && isConnectedPredicate(current, x));

                foreach (var item in childrenItems)
                {
                    queue.Enqueue(item);
                    visited.Add(item);

                    if (lastItems.Contains(item))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool CanReachAllItems<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> getNeighbourItems, T start)
        {
            var queue = new Queue<T>();
            var visited = new HashSet<T>();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                var childrenItems = getNeighbourItems(current).Where(x => !visited.Contains(x));

                foreach (var item in childrenItems)
                {
                    queue.Enqueue(item);
                    visited.Add(item);
                }
            }
            
            return visited.Count == items.Count();
        }
    }

    public struct ParentChildrenPair<T>
    {
        public T Parent { get; }
        
        public T Child { get; }
        
        public ParentChildrenPair(T parent, T child)
        {
            Parent = parent;
            Child = child;
        }
    }
}