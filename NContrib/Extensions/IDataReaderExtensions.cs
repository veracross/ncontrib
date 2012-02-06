using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
        /// Returns a string array of the column names in this <see cref="IDataReader"/>
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static string[] GetColumnNames(this IDataReader dr) {
            // msbuild would *not* work without specifying type arguments. I have *no* idea why
            return Enumerable.Range(0, dr.FieldCount).Select(dr.GetName).ToArray();
        }

        public static IDictionary<string, TValue> GetRowAsDictionary<TValue>(this IDataReader dr, Func<string, string> fieldNameConverter = null) {
            var temp = new Dictionary<string, TValue>();
            
            if (fieldNameConverter == null)
                fieldNameConverter = s => s;

            for (var i = 0; i < dr.FieldCount; i++)
                temp.Add(fieldNameConverter(dr.GetName(i)), dr.GetValue(i).ConvertTo<TValue>());

            return temp;
        }

        /// <summary>
        /// Returns the value from the specified column and converts it using <see cref="ObjectExtensions.ConvertTo"/>
        /// When the column value is null, the default for type T is returned
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static T GetValue<T>(this IDataReader dr, string columnName) {
            return dr.GetValue(columnName, default(T));
        }

        /// <summary>
        /// Returns the value from the specified column and converts it using <see cref="ObjectExtensions.ConvertTo"/>
        /// When the column value is null, the specified fallback is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public static T GetValue<T>(this IDataReader dr, string columnName, T fallback) {
            return dr.GetValue(dr.GetOrdinal(columnName), fallback);
        }

        /// <summary>
        /// Returns the value from the specified column and converts it using <see cref="ObjectExtensions.ConvertTo"/>
        /// When the column value is null, the default for type T is returned
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public static T GetValue<T>(this IDataReader dr, int columnIndex) {
            return dr.GetValue(columnIndex, default(T));
        }

        /// <summary>
        /// Returns the value from the specified column and converts it using <see cref="ObjectExtensions.ConvertTo"/>
        /// When the column value is null, the specified fallback is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="columnIndex"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public static T GetValue<T>(this IDataReader dr, int columnIndex, T fallback) {
            return dr.IsDBNull(columnIndex) ? fallback : dr.GetValue(columnIndex).ConvertTo<T>();
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
