using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NContrib.Extensions {

    public static class IEnumerableExtensions {

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
