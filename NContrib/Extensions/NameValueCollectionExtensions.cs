using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace NContrib.Extensions {

    public static class NameValueCollectionExtensions {
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
            return collection.Keys.Cast<string>().ToDictionary(s => s, s => collection[s], equalityComparer);
        }

    }
}
