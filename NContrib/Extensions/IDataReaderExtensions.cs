using System;
using System.Collections.Generic;
using System.Data;

namespace NContrib.Extensions {

    public static class IDataReaderExtensions {

        /// <summary>Returns an entire column as a T[]</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static T[] GetColumnAsArrayOf<T>(this IDataReader dr, string columnName) {
            var temp = new List<T>();

            while (dr.Read())
                temp.Add(dr.GetValue<T>(columnName));

            return temp.ToArray();
        }
        
        /// <summary>
        /// Reads through an <see cref="IDataReader"/> result set and transforms each row using
        /// the given <paramref name="converter"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static List<T> TransformAll<T>(this IDataReader dr, Converter<IDataReader, T> converter) {
            var temp = new List<T>();

            while (dr.Read())
                temp.Add(converter(dr));

            return temp;
        }
    }
}
