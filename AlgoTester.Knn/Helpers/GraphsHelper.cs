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
        public static List<T> GetBfsShortestPath<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> getNeighbourItems,
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
            HashSet<T> start, HashSet<T> end)
        {
            var queue = new Queue<T>(start);
            var visited = new HashSet<T>(start);
            var parents = new List<Tuple<T,T>>();

            if(start.Overlaps(end))
            {
                return new List<T>()
                {
                    start.First()
                };
            }
            
            bool foundWay = false;
            
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                var childrenItems = getNeighbourItems(current).Where(x => !visited.Contains(x));

                foreach (var item in childrenItems)
                {
                    queue.Enqueue(item);
                    visited.Add(item);

                    parents.Add(new Tuple<T, T>(current, item));

                    if (end.Contains(item))
                    {
                        foundWay = true;
                        
                        queue.Clear();
                        
                        break;
                    }
                }
            }

            var path = new List<T>();
            
            if(!foundWay)
            {
                throw new InvalidOperationException("Couldn't found the way");
            }

            var currentPair = parents.Last();

            path.Add(currentPair.Item1);
            
            path.Add(currentPair.Item2);

            while (!start.Contains(currentPair.Item1))
            {
                currentPair = parents.First(x => x.Item2.Equals(currentPair.Item1));

                path.Add(currentPair.Item1);
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