using System;
using System.Collections.Generic;
using System.Linq;

namespace NContrib.Extensions {

    public static class IEnumerableExtensions {

        private static readonly Random Randomizer = new Random();

        /// <summary>
        /// Converts the elements to a list and executes the given action for each element.
        /// Returns the original element list unchanged
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elements"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<T> Action<T>(this IEnumerable<T> elements, Action<T> action) {
            elements.ToList().ForEach(action);
            return elements;
        }

        /// <summary>
        /// Break a list of items into chunks of a specific size
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize) {

            if (source == null)
                throw new ArgumentNullException("source");

            if (chunksize <= 0)
                throw new ArgumentException("Chunk size must be greater than zero.", "chunksize");

            while (source.Any()) {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize);
            }
        }

        /// <summary>
        /// Finds all elements in a list of the given type and casts them to that.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindAndCast<T>(this IEnumerable<object> source)
        {
            return source.Where(item => item is T).Cast<T>();
        }

        public static T Random<T>(this IEnumerable<T> source) {
            
            if (source == null)
                throw new ArgumentNullException("source");

            var arr = source.ToArray();

            if (arr.Length == 1)
                return arr[0];

            var index = Randomizer.Next(0, arr.Length);

            return arr[index];
        }

        /// <summary>
        /// Flattens an enumerable KeyValuePair into a Dictionary
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Dictionary<T1, T2> ToDictionary<T1, T2>(this IEnumerable<KeyValuePair<T1, T2>> source) {
            return source.ToDictionary(x1 => x1.Key, x2 => x2.Value);
        }
    }
}
