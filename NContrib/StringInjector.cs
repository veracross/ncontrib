using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace NContrib {

    /// <summary>
    /// Originally from:
    /// http://mo.notono.us/2008/07/c-stringinject-format-strings-by-key.html
    /// 
    /// Injects an object into a string using named replacements. Supports standard formatting.
    /// </summary>
    /// <example>"Hello, {Name}. Today is {Today:dd/mm/yyyy}".Inject(new { Name = "John", Today = DateTime.Now });</example>
    public static class StringInjector {

        /// <summary>
        /// Replaces keys in a string with the values of matching object properties using the CurrentCulture for formatting.
        /// <remarks>Uses <see cref="String.Format(IFormatProvider, string, object[])"/> internally; custom formats should match those used for that method.</remarks>
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo} and {foo:SomeFormat}.</param>
        /// <param name="injectionObject">The object whose properties should be injected in the string</param>
        /// <returns>A version of the formatString string with keys replaced by (formatted) key values.</returns>
        public static string Inject(this string formatString, object injectionObject) {
            return formatString.Inject(GetPropertyHash(injectionObject), null);
        }

        /// <summary>
        /// Replaces keys in a string with the values of matching object properties using the specified format provider
        /// <remarks>Uses <see cref="String.Format(IFormatProvider, string, object[])"/> internally; custom formats should match those used for that method.</remarks>
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo} and {foo:SomeFormat}.</param>
        /// <param name="injectionObject">The object whose properties should be injected in the string</param>
        /// <param name="formatProvider">Format provider used for default formatting</param>
        /// <returns>A version of the formatString string with keys replaced by (formatted) key values.</returns>
        public static string Inject(this string formatString, object injectionObject, IFormatProvider formatProvider) {
            return formatString.Inject(GetPropertyHash(injectionObject), formatProvider);
        }

        /// <summary>
        /// Replaces keys in a string with the values of matching dictionary entries using the CurrentCulture for formatting.
        /// <remarks>Uses <see cref="String.Format(IFormatProvider, string, object[])"/> internally; custom formats should match those used for that method.</remarks>
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo} and {foo:SomeFormat}.</param>
        /// <param name="dictionary">An <see cref="IDictionary{TKey,TValue}"/> with keys and values to inject into the string</param>
        /// <returns>A version of the formatString string with dictionary keys replaced by (formatted) key values.</returns>
        public static string Inject(this string formatString, IDictionary dictionary) {
            return formatString.Inject(new Hashtable(dictionary), null);
        }

        /// <summary>
        /// Replaces keys in a string with the values of matching dictionary entries using the specified format provider
        /// <remarks>Uses <see cref="String.Format(IFormatProvider, string, object[])"/> internally; custom formats should match those used for that method.</remarks>
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo} and {foo:SomeFormat}.</param>
        /// <param name="dictionary">An <see cref="IDictionary{TKey,TValue}"/> with keys and values to inject into the string</param>
        /// <param name="formatProvider"></param>
        /// <returns>A version of the formatString string with dictionary keys replaced by (formatted) key values.</returns>
        public static string Inject(this string formatString, IDictionary dictionary, IFormatProvider formatProvider) {
            return formatString.Inject(new Hashtable(dictionary), formatProvider);
        }

        /// <summary>
        /// Replaces keys in a string with the values of matching hashtable entries using the CurrentCulture for formatting.
        /// <remarks>Uses <see cref="String.Format(IFormatProvider, string, object[])"/> internally; custom formats should match those used for that method.</remarks>
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo} and {foo:SomeFormat}.</param>
        /// <param name="attributes">A <see cref="Hashtable"/> with keys and values to inject into the string</param>
        /// <returns>A version of the formatString string with hastable keys replaced by (formatted) key values.</returns>
        public static string Inject(this string formatString, Hashtable attributes) {
            var result = formatString;

            if (attributes == null || formatString == null)
                return result;

            return attributes.Keys.Cast<string>().Aggregate(result, (current, attributeKey) => current.InjectSingleValue(attributeKey, attributes[attributeKey], null));
        }

        /// <summary>
        /// Replaces keys in a string with the values of matching hashtable entries using the specified format provider.
        /// <remarks>Uses <see cref="String.Format(IFormatProvider, string, object[])"/> internally; custom formats should match those used for that method.</remarks>
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo} and {foo:SomeFormat}.</param>
        /// <param name="attributes">A <see cref="Hashtable"/> with keys and values to inject into the string</param>
        /// <param name="formatProvider"></param>
        /// <returns>A version of the formatString string with hastable keys replaced by (formatted) key values.</returns>
        public static string Inject(this string formatString, Hashtable attributes, IFormatProvider formatProvider) {
            var result = formatString;

            if (attributes == null || formatString == null)
                return result;

            return attributes.Keys.Cast<string>().Aggregate(result, (current, attributeKey) => current.InjectSingleValue(attributeKey, attributes[attributeKey], formatProvider));
        }

        /// <summary>
        /// Replaces all instances of a 'key' (e.g. {foo} or {foo:SomeFormat}) in a string with an optionally formatted value, and returns the result.
        /// </summary>
        /// <param name="formatString">The string containing the key; unformatted ({foo}), or formatted ({foo:SomeFormat})</param>
        /// <param name="key">The key name (foo)</param>
        /// <param name="replacementValue">The replacement value; if null is replaced with an empty string</param>
        /// <param name="formatProvider"></param>
        /// <returns>The input string with any instances of the key replaced with the replacement value</returns>
        public static string InjectSingleValue(this string formatString, string key, object replacementValue, IFormatProvider formatProvider) {

            var result = formatString;

            if (formatProvider == null)
                formatProvider = CultureInfo.CurrentCulture;

            //regex replacement of key with value, where the generic key format is:
            //Regex foo = new Regex("{(foo)(?:}|(?::(.[^}]*)}))");
            //for key = foo, matches {foo} and {foo:SomeFormat}
            var attributeRegex = new Regex(@"{(?<key>" + key + @")(?:}|(?::(?<format>.[^}]*)}))");

            //loop through matches, since each key may be used more than once (and with a different format string)
            foreach (Match m in attributeRegex.Matches(formatString)) {
                string replacement;

                //matched {foo:SomeFormat}
                if (m.Groups["format"].Value.Length > 0) {
                    //do a double string.Format - first to build the proper format string, and then to format the replacement value
                    var attributeFormatString = string.Format(CultureInfo.InvariantCulture, "{{0:{0}}}", m.Groups["format"].Value);
                    replacement = string.Format(formatProvider, attributeFormatString, replacementValue);
                }
                else {
                    //matched {foo}
                    replacement = (replacementValue ?? string.Empty).ToString();
                }

                //perform replacements, one match at a time
                result = result.Replace(m.ToString(), replacement);  //attributeRegex.Replace(result, replacement, 1);
            }
            return result;

        }

        /// <summary>
        /// Creates a HashTable based on current object state.
        /// <remarks>Copied from the MVCToolkit HtmlExtensionUtility class</remarks>
        /// </summary>
        /// <param name="properties">The object from which to get the properties</param>
        /// <returns>A <see cref="Hashtable"/> containing the object instance's property names and their values</returns>
        private static Hashtable GetPropertyHash(object properties) {

            if (properties == null)
                return null;

            var values = new Hashtable();
            var props = TypeDescriptor.GetProperties(properties);

            foreach (PropertyDescriptor prop in props)
                values.Add(prop.Name, prop.GetValue(properties));

            return values;
        }

    }
}
