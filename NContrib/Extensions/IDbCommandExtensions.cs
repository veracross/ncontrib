using System;
using System.Collections.Generic;
using System.Data;

namespace NContrib.Extensions {

    public static class IDbCommandExtensions {

        /// <summary>
        /// Executes the given <see cref="IDbCommand"/> and returns the requested columns
        /// of types TKey and TValue as a Dictionary&lt;TKey, TValue&gt;. Useful when selecting key/value
        /// pairs from a database.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="keyColumn"></param>
        /// <param name="valueColumn"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> ExecuteDictionary<TKey, TValue>(this IDbCommand cmd, int keyColumn = 0, int valueColumn = 1) {
            var d = new Dictionary<TKey, TValue>();

            using (var dr = cmd.ExecuteReader()) {
                while (dr.Read())
                    d.Add(dr.GetValue<TKey>(keyColumn), dr.GetValue<TValue>(valueColumn));
            }

            return d;
        }

        /// <summary>
        /// Executes the given <see cref="IDbCommand"/> and returns the requested columns
        /// of types TKey and TValue as a Dictionary&lt;TKey, TValue&gt;. Useful when selecting key/value
        /// pairs from a database.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="keyColumn"></param>
        /// <param name="valueColumn"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> ExecuteDictionary<TKey, TValue>(this IDbCommand cmd, string keyColumn, string valueColumn) {
            var d = new Dictionary<TKey, TValue>();

            using (var dr = cmd.ExecuteReader()) {
                while (dr.Read())
                    d.Add(dr.GetValue<TKey>(keyColumn), dr.GetValue<TValue>(valueColumn));
            }

            return d;
        }

        /// <summary>
        /// Exexcutes the given <see cref="IDbCommand"/> and returns an IEnumerable&lt;&T&gt;
        /// where T is each record transformed by the given Converter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static IEnumerable<T> ExecuteAndTransform<T>(this IDbCommand cmd, Converter<IDataReader, T> converter) {
            var temp = new List<T>();

            using (var dr = cmd.ExecuteReader()) {
                while (dr.Read())
                    temp.Add(converter(dr));
            }
            return temp;
        }

    }
}
