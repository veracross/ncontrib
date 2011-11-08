using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace NContrib.Extensions {

    public static class ObjectExtensions {

        /// <summary>Converts a value to type T</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(this object value, CultureInfo cultureInfo) {

            if (value == null || value is DBNull)
                return default(T);

            // if the types are the same or the value inherits from T, we're done
            if (value.GetType() == typeof(T) || value is T)
                return (T)value;

            return (T)ConvertTo(value, typeof(T), cultureInfo);
        }

        /// <summary>
        /// Converts a value to type T using <see cref="CultureInfo.CurrentCulture"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(this object value) {
            return value.ConvertTo<T>(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converts a value to the requested type.
        /// </summary>
        /// <example>
        /// When <paramref name="value"/> is null: For value types: a new intsance. Otherwise, null is returned.
        /// The original value is returned when it matches the destination type or the destination type is assignable from the value type. (value is T)
        /// When the <paramref name="value"/> type implements IConvertible, it is used to convert to <paramref name="type"/> if supported
        /// When a <see cref="TypeConverter"/> exists for <paramref name="type"/> that can convert from the type of <paramref name="value"/>, it is used.
        /// If <paramref name="type"/> is <see cref="String"/>, <paramref name="value"/>.ToString() is returned.
        /// If <paramref name="type"/> has Parse or TryParse methods, they are attempted
        /// Lastly, an InvalidCastException is thrown
        /// </example>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public static object ConvertTo(this object value, Type type) {
            return value.ConvertTo(type, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converts a value to the requested type.
        /// </summary>
        /// <example>
        /// When <paramref name="value"/> is null: For value types: a new insance. Otherwise, null is returned.
        /// The original value is returned when it matches the destination type or the destination type is assignable from the value type. (value is T)
        /// When the <paramref name="value"/> type implements IConvertible, it is used to convert to <paramref name="type"/> if supported
        /// When a <see cref="TypeConverter"/> exists for <paramref name="type"/> that can convert from the type of <paramref name="value"/>, it is used.
        /// If <paramref name="type"/> is <see cref="String"/>, <paramref name="value"/>.ToString() is returned.
        /// If <paramref name="type"/> has Parse or TryParse methods, they are attempted
        /// Lastly, an InvalidCastException is thrown
        /// </example>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public static object ConvertTo(this object value, Type type, CultureInfo cultureInfo) {
            
            // if the value is null, return a default instance for value types or null for non-value
            if (value == null || value is DBNull)
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            
            // if the types are the same or value inherits from the type, we're done
            if (value.GetType() == type || type.IsAssignableFrom(value.GetType()))
                return value;

            // if we're going to a bool and the value is 1 or 0, assume true/false
            if (type == typeof(bool)) {
                if (value as string == "1") return true;
                if (value as string == "0") return false;
            }

            // if IConvertible is implemented, see if that can do it for us
            if (value is IConvertible) {
                try { return Convert.ChangeType(value, type, cultureInfo); }
                catch (InvalidCastException) { }
            }

            // try a type converter
            var conv = TypeDescriptor.GetConverter(type);
            if (conv != null && conv.CanConvertFrom(value.GetType())) {
                try { return conv.ConvertFrom(null, cultureInfo, value); }
                catch (NotSupportedException) { }
                catch (NullReferenceException) { }
            }

            // for going to string, all we have left is using ToString()
            if (type == typeof(string))
                return value.ToString();

            // use enum parser for enums
            if (type.IsEnum)
                return Enum.Parse(type, value.ToString());

            throw new InvalidCastException(string.Format("It is not possible to convert from '{0}' to '{1}'",
                                                         value.GetType(), type));
        }

        /// <summary>
        /// Copies properties from an object that has matching property names and
        /// types that can be converted with <see cref="ConvertTo(object, Type)"/>
        /// </summary>
        /// <param name="o"></param>
        /// <param name="copyFrom"></param>
        public static void CopyPropertiesFrom(this object o, object copyFrom) {
            o.CopyPropertiesFrom(copyFrom, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Copies properties from an object that has matching property names and
        /// types that can be converted with <see cref="ConvertTo(object, Type)"/>
        /// </summary>
        /// <param name="o"></param>
        /// <param name="copyFrom"></param>
        /// <param name="cultureInfo">Culture info to use during type conversion</param>
        public static void CopyPropertiesFrom(this object o, object copyFrom, CultureInfo cultureInfo) {
            var q = from p in o.GetType().GetProperties()
                    join d in copyFrom.GetType().GetProperties() on p.Name equals d.Name
                    select new {
                        LocalProperty = p,
                        DefaultValue = d.GetValue(copyFrom, null).ConvertTo(p.PropertyType, cultureInfo),
                    };

            q.Action(x => x.LocalProperty.SetValue(o, x.DefaultValue, null));
        }

        /// <summary>
        /// Describes the state of an object in plain text
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Describe(this object obj) {
            var od = new ObjectDescriber();
            return od.Describe(obj);
        }

        /// <summary>
        /// Indicates if the given object exists in an array of similar objects using
        /// <see cref="IEnumerable{T}"/>.Contains using the default equality comparer
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="o"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool In<T1, T2>(this T1 o, params T2[] collection) where T1 : T2 {
            return collection.Contains(o);
        }

        /// <summary>
        /// Indicates if the given object exists in an array of similar objects using
        /// <see cref="IEnumerable{T}"/>.Contains using the given equality comparer
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="o"></param>
        /// <param name="collection"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static bool In<T1, T2>(this T1 o, IEqualityComparer<T2> comparer, params T2[] collection) where T1 : T2 {
            return collection.Contains(o, comparer);
        }

        /// <summary>
        /// Indicates if the given object does not exist in an array of similar objects using
        /// <see cref="IEnumerable{T}"/>.Contains using the default equality comparer
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="o"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool NotIn<T1, T2>(this T1 o, params T2[] collection) where T1 : T2 {
            return !collection.Contains(o);
        }

        /// <summary>
        /// Indicates if the given object does not exist in an array of similar objects using
        /// <see cref="IEnumerable{T}"/>.Contains using the default equality comparer
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="value"></param>
        /// <param name="collection"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static bool NotIn<T1, T2>(this T1 value, IEqualityComparer<T2> comparer, params T2[] collection) where T1 : T2 {
            return !collection.Contains(value, comparer);
        }

        /// <summary>
        /// Finds all properties in the object that have the <see cref="DefaultValueAttribute"/> attribute and
        /// sets their value to that specified in the DefaultValue attribute.
        /// </summary>
        /// <param name="o"></param>
        public static void SetDefaults(this object o) {
            if (o == null)
                return;

            o.GetType()
                .GetProperties()
                .Where(p => p.HasAttributeOfType<DefaultValueAttribute>())
                .Action(p => p.SetValue(o, p.GetSingleAttribute<DefaultValueAttribute>().Value, null));
        }

        /// <summary>
        /// First calls SetDefaults which does property initilisation with <see cref="DefaultValueAttribute"/>
        /// attributes. Then, uses the passed defaults object parameter to copy property values from that object
        /// to this object. Property names must match and types must be convertible using ConvertTo
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defaults"></param>
        public static void SetDefaults(this object o, object defaults) {
            o.SetDefaults();
            o.CopyPropertiesFrom(defaults);
        }
    }
}
