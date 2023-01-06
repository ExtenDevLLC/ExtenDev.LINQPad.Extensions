using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExtenDev.LINQPad.Extensions
{
    // TODO: Add XML comments to all members
    public static class CollectionExtensions
    {

        public static V GetOrAdd<K, V>(this Dictionary<K, V> dictionary, K key, Func<K, V> factory)
            where K : notnull
        {
            if (!dictionary.TryGetValue(key, out V? val))
            {
                val = factory(key);
                dictionary.Add(key, val);
            }

            return val;
        }

        public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dictionary, K key)
            where K : notnull
        {
            dictionary.TryGetValue(key, out var ret);
            return ret;
        }

        public static V? GetFirstValueOrDefault<K, V>(this IEnumerable<Dictionary<K, V>> dictionaries, K key)
            where K : notnull
        {
            foreach (var dict in dictionaries)
            {
                if (dict.TryGetValue(key, out V val)) return val;
            }
            return default(V);
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items) action(item);

            return items;
        }

        public static IEnumerable<T> EnumerateInReverse<T>(this IList<T> list)
        {
            for (int x = list.Count - 1; x >= 0; x--)
            {
                yield return list[x];
            }
        }

        public static IEnumerable<T> EnumerateInReverse<T>(this LinkedList<T> list)
        {
            var curNode = list.Last;
            
            while (curNode != null)
            {
                yield return curNode.Value;
                curNode = curNode.Previous;
            }
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> items, params T[] additionalItems)
        {
            return items.Concat((IEnumerable<T>)additionalItems);
        }

        public static IEnumerable<T> ConcatIf<T>(this IEnumerable<T> items, bool condition, params T[] additionalItems)
        {
            return condition ? items.Concat((IEnumerable<T>)additionalItems) : items;
        }

        public static IEnumerable<T> ConcatIf<T>(this IEnumerable<T> items, bool condition, IEnumerable<T> additionalItems)
        {
            return condition ? items.Concat(additionalItems) : items;
        }

        public static IEnumerable<T> ConcatIf<T>(this IEnumerable<T> items, bool condition, Func<IEnumerable<T>> additionalItems)
        {
            return condition ? items.Concat(additionalItems()) : items;
        }
    }
}
