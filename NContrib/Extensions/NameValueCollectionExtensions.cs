using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace NContrib.Extensions {

    public static class NameValueCollectionExtensions {

        /// <summary>
        /// Converts a NameValueCollection into a Dictionary&lt;string,string&gt;
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDictionary(this NameValueCollection collection) {
            return collection.Keys.Cast<string>().ToDictionary(s => s, s => collection[s]);
        }

    }
}
