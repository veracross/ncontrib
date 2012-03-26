using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace NContrib.Extensions {

    public static class NameValueCollectionExtensions {

        /// <summary>
        /// Determines if the given key is present in the <see cref="NameValueCollection"/>
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ContainsKey(this NameValueCollection collection, string key) {
            return collection.AllKeys.Any(k => k == key);
        }

        /// <summary>
        /// If the <see cref="NameValueCollection"/> contains the given key, the value is converted
        /// to <see cref="T"/> and returned. If it's not present, the fallback value is returned
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="key"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public static T GetValue<T>(this NameValueCollection collection, string key, T fallback = default(T)) {
            return collection.ContainsKey(key) ? collection[key].ConvertTo<T>() : fallback;
        }

        /// <summary>
        /// Converts a NameValueCollection into a Dictionary&lt;string,string&gt;
        /// using a <see cref="StringComparer.InvariantCultureIgnoreCase"/> equality comparer.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDictionary(this NameValueCollection collection) {
            return collection.ToDictionary(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Converts a NameValueCollection into a Dictionary&lt;string,string&gt;
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="equalityComparer"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDictionary(this NameValueCollection collection, IEqualityComparer<string> equalityComparer) {
            return collection.ToDictionary(collection.GetKey, collection.Get, equalityComparer);
        }

        public static Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(this NameValueCollection collection, Func<int, TKey> keySelector, Func<int, TElement> elementSelector, IEqualityComparer<TKey> equalityComparer) {
            return Enumerable.Range(0, collection.Count)
                .ToDictionary(keySelector, elementSelector, equalityComparer);
        }
    }
}
