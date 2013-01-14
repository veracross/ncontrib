using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace NContrib.Extensions
{
    public static class IDataRecordExtensions
    {
        public static T AutoMapType<T>(this IDataRecord dr)
        {
            return (T)dr.AutoMapType(typeof (T));
        }

        public static object AutoMapType(this IDataRecord dr, Type destType)
        {
            var props = destType.GetProperties().ToArray();

            var destObj = Activator.CreateInstance(destType);

            for (var i = 0; i < dr.FieldCount; i++)
            {
                var propName = dr.GetName(i).ToCamelCase(TextTransform.Upper);
                var destProp = props.FirstOrDefault(p => p.Name == propName);

                if (destProp == null)
                    continue;

                var value = dr.GetValue(i, convertDbNull: true);

                try
                {
                    value = value.ConvertTo(destProp.PropertyType);
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to convert field '" + dr.GetName(i) + "' to '" + destProp.PropertyType.Name + "': " + ex.Message, ex);
                }

                try
                {
                    destProp.SetValue(destObj, value, null);
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not set property '" + destProp.Name + "': " + ex.Message, ex);
                }
            }

            return destObj;
        }

        /// <summary>
        /// Returns a string array of the column names in this <see cref="IDataReader"/>
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static string[] GetColumnNames(this IDataRecord dr)
        {
            // msbuild would *not* work without specifying type arguments. I have *no* idea why
            return Enumerable.Range(0, dr.FieldCount).Select(dr.GetName).ToArray();
        }

        public static IDictionary<string, TValue> GetRowAsDictionary<TValue>(this IDataRecord dr, Func<string, string> fieldNameConverter = null)
        {
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
        public static T GetValue<T>(this IDataRecord dr, string columnName)
        {
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
        public static T GetValue<T>(this IDataRecord dr, string columnName, T fallback)
        {
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
        public static T GetValue<T>(this IDataRecord dr, int columnIndex)
        {
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
        public static T GetValue<T>(this IDataRecord dr, int columnIndex, T fallback)
        {
            return dr.IsDBNull(columnIndex) ? fallback : dr.GetValue(columnIndex).ConvertTo<T>();
        }

        public static object GetValue(this IDataRecord dr, int columnIndex, bool convertDbNull)
        {
            var value = dr.GetValue(columnIndex);

            if (!convertDbNull || !(value is DBNull))
                return value;

            var fieldType = dr.GetFieldType(columnIndex);

            if (fieldType.IsValueType)
                return Activator.CreateInstance(fieldType);

            if (fieldType.IsArray)
                return Array.CreateInstance(fieldType.GetElementType(), 0);

            var tc = new TypeConverter();
            return tc.ConvertTo(null, fieldType);
        }
    }
}
