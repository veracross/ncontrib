using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NContrib.Extensions {

    public static class IDictionaryExtensions {

        /// <summary>
        /// Returns a formatted string showing the keys and values in this Dictionary for easy reading
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string Describe<T1, T2>(this IDictionary<T1, T2> dict) {
            if (dict == null)
                return "(null)";

            if (dict.Count == 0)
                return "(empty)";

            var padLen = dict.Keys.Select(k => k.ToString()).Max(x => x.Length);
            return dict.Select(x => x.Key.ToString().PadRight(padLen) + " => " + x.Value).Join("\n");
        }

        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) {
            return dict.GetValue(key, default(TValue));
        }

        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue fallback) {
            return dict.ContainsKey(key) ? dict[key] : fallback;
        }

        public static string ToHttpQuery(this IDictionary<string, string> dict) {
            return dict.ToHttpQuery(Encoding.UTF8);
        }

        public static string ToHttpQuery(this IDictionary<string, string> dict, Encoding enc) {
            return dict.Select(p => HttpUtility.UrlEncode(p.Key, enc) + "=" + HttpUtility.UrlEncode(p.Value, enc)).Join(", ");
        }

        public static bool TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Action<TValue> assign) {
            if (dict.ContainsKey(key)) {
                assign(dict[key]);
                return true;
            }
            return false;
        }

        public static bool TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue fallback, Action<TValue> assign) {
            if (dict.ContainsKey(key)) {
                assign(dict[key]);
                return true;
            }

            assign(fallback);
            return false;
        }
    }
}
