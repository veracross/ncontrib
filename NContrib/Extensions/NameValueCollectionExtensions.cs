using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace NContrib.Extensions {

    public static class NameValueCollectionExtensions {

        public static bool ContainsKey(this NameValueCollection collection, string key) {
            return collection.AllKeys.Any(k => k == key);
        }

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
            return Enumerable.Range(0, collection.Count)
                .ToDictionary(collection.GetKey, collection.Get, equalityComparer);
        }

    }
}
