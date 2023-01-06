using System;
using System.Linq;

namespace ExtenDev.LINQPad.Extensions
{
    // TODO: Add XML comments to all members
    public static class ObjectExtensions
    {
        public static bool In<T>(this T item, params T[] matchValues)
        {
            return matchValues.Contains(item);
        }

        public static U Transform<T, U>(this T input, Func<T, U> mapper)
        {
            return mapper(input);
        }

        public static O MapWhen<I, O>(this I input, params (I InputValue, O Result)[] conditionalMappings)
        {
            foreach (var mapping in conditionalMappings)
            {
                if ((input == null && mapping.InputValue == null) || (input?.Equals(mapping.InputValue) ?? false))
                {
                    return mapping.Result;
                }
            }
            throw new ArgumentException("No matching conditional mapping value found", nameof(input));
        }

        public static O MapWhen<I, O>(this I input, O defaultResult, params (I InputValue, O Result)[] conditionalMappings)
        {
            foreach (var mapping in conditionalMappings)
            {
                if ((input == null && mapping.InputValue == null) || (input?.Equals(mapping.InputValue) ?? false))
                {
                    return mapping.Result;
                }
            }
            return defaultResult;
        }

        [System.Obsolete("When extension method cannot be used with a single conditional mapping. Resulting behavior is not intuitive.", true)]
        public static O MapWhen<I, O>(this I input, (I, O) singleMapping)
        {
            throw new ArgumentException("MapWhen extension method cannot be used with a single conditional mapping. Resulting behavior is not intuitive.", nameof(singleMapping));
        }

    }
}
