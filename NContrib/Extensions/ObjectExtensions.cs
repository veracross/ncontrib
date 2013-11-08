using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

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
            if (value.GetType() == type || type.IsInstanceOfType(value))
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

            // if the destination type is an array and the source type is a string
            // then we probably have a delimited string. attempt to split the string
            // by commas, semi-colons, and pipes, and converting each element to the
            // destination array's element type
            // ex) "1,2,3,4".ConvertTo(typeof(int[], null) => [1, 2, 3, 4]
            if (type.IsArray && value is string)
            {
                var stringValues = (value as string).Split(new[] {',', ';', '|'});
                var elementType = type.GetElementType();
                var arr = Array.CreateInstance(elementType, stringValues.Length);

                Enumerable.Range(0, stringValues.Length)
                          .ToList()
                          .ForEach(i => arr.SetValue(stringValues[i].ConvertTo(elementType, cultureInfo), i));

                return arr;
            }

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
        /// Serializes an object to a formatted XML string
        /// </summary>
        /// <param name="o"></param>
        /// <param name="formatting"></param>
        /// <returns></returns>
        public static string SerializeToXml(this object o, Formatting formatting = Formatting.Indented) {
            using (var sw = new StringWriter())
            using (var tw = new XmlTextWriter(sw) { Formatting = formatting }) {
                new System.Xml.Serialization.XmlSerializer(o.GetType()).Serialize(tw, o);
                return sw.ToString();
            }
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

        /// <summary>
        /// Attempts to convert the object to type T. If successful, assigns the value using the given assigner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="assign"></param>
        /// <returns></returns>
        public static bool TryConvertTo<T>(this object o, Action<T> assign) {
            return o.TryConvertTo(thisObject => thisObject.ConvertTo<T>(), assign);
        }

        /// <summary>
        /// Attemtps to convert the object to type T using the given converter. If successful, the value
        /// is assisgned using the given assigner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="converter"></param>
        /// <param name="assign"></param>
        /// <returns></returns>
        public static bool TryConvertTo<T>(this object o, Converter<object, T> converter, Action<T> assign) {
            
            try {
                assign(converter(o));
                return true;
            }
            catch(Exception) {
                return false;
            }
        }
    }
}
