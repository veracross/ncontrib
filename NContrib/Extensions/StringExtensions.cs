using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NContrib.Extensions {

    public static class StringExtensions {

        /// <summary>
        /// Converts a string to camel case using spaces, dashes, and underscores as breaking points for a new capital letter
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Camelize(this string s) {
            if (Regex.IsMatch(s, @"^\p{Lu}+$"))
                return s.ToLower();

            return char.ToLower(s[0]) + Regex.Replace(s.Substring(1), @"[\s\-_](\w)", m => m.Groups[1].Value.ToUpper());
        }

        /// <summary>Gets a digit inside of the string at a specified index.</summary>
        /// <example>Given the string 122240861, the routing number for Schwab, "122240861".DigitAt(6) returns 8</example>
        /// <param name="input"></param>
        /// <param name="index">Character index to look for a digit</param>
        /// <returns><see cref="Int32"/></returns>
        /// <exception cref="ArgumentException">Thrown when the char is not a digit</exception>
        /// <exception cref="IndexOutOfRangeException" />
        public static int DigitAt(this string input, int index) {
            ushort c = input[index];

            if (c < '0' || c > '9')
                throw new ArgumentException(string.Format("The value at index {0} is '{1}' which is not a digit.", index, input[index]));

            return c - 48;
        }

        /// <summary>Returns part of a string starting at the index where the search string was found</summary>
        /// <param name="s"></param>
        /// <param name="searchPattern"></param>
        /// <param name="includeSearchString"></param>
        /// <returns></returns>
        /// <example>"Boston, MA".FromIndexOf(", ", false) => "MA"</example>
        public static string FromIndexOf(this string s, string searchPattern, bool includeSearchString = false) {
            var m = Regex.Match(s, searchPattern);

            if (!m.Success)
                return s;

            var offset = m.Index + (includeSearchString ? 0 : m.Value.Length);
            return s.Substring(offset, s.Length - offset);
        }

        /// <summary>Returns part of a string starting at the index where the search char was found</summary>
        /// <param name="s"></param>
        /// <param name="search"></param>
        /// <param name="includeSearchChar"></param>
        /// <returns></returns>
        /// <example>"$123,456.78".FromIndexOf('.', false) => "78"</example>
        public static string FromIndexOf(this string s, char search, bool includeSearchChar = false) {
            var index = s.IndexOf(search);
            var offset = index + (includeSearchChar ? 0 : 1);
            return index == -1 ? s : s.Substring(offset, s.Length - offset);
        }

        /// <summary>Concatenate these strings using the specified delimiter/glue/separator</summary>
        /// <param name="strings"></param>
        /// <param name="delimiter"></param>
        /// <returns>String.Empty if the sequence contains no elements</returns>
        public static string Join(this IEnumerable<string> strings, string delimiter) {
            return strings.Any()
                ? string.Join(delimiter, strings as string[] ?? strings.ToArray())
                : string.Empty;
        }

        /// <summary>
        /// Concatenate the elements in this sequence using the specified delimiter
        /// </summary>
        /// <param name="strings"></param>
        /// <param name="delimiter"></param>
        /// <returns>string.Empty if the sequence contains no elements</returns>
        public static string Join(this IEnumerable<string> strings, char delimiter) {
            return strings.Join(delimiter.ToString());
        }

        /// <summary>
        /// Concatenate these strings using the specified delimiter until the last element,
        /// then use another delimiter. Used for easily making natural language lists such as Apples, Eggs, Bread, and Milk
        /// </summary>
        /// <param name="strings"></param>
        /// <param name="delimiter"></param>
        /// <param name="lastDelimiter"></param>
        /// <returns></returns>
        public static string Join(this IEnumerable<string> strings, string delimiter, string lastDelimiter) {
            return strings.Any()
                ? string.Join(delimiter, strings as string[] ?? strings.ToArray(), 0, strings.Count() - 1)
                    + lastDelimiter + strings.ElementAt(strings.Count() - 1)
                : string.Empty;
        }


        /// <summary>Tests to find if the string is null or empty-string (zero-length)</summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string input) {
            return String.IsNullOrEmpty(input);
        }

        /// <summary>Tests to find if the string is neither null null nor empty-string (zero-length)</summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string input) {
            return !input.IsEmpty();
        }

        /// <summary>Tests to find if the string is null, empty (zero-length), or contains only whitespace</summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsBlank(this string input) {
            return String.IsNullOrEmpty(input) || input.ToCharArray().All(c => char.IsWhiteSpace(c));
        }

        /// <summary>Tests to find if the string is not null, not empty (zero-length), and contains more than whitespace</summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNotBlank(this string input) {
            return !input.IsBlank();
        }

        /// <summary>
        /// Returns the right <paramref name="length"/> characters of the given string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Left(this string input, int length) {
            return input.Substring(0, length);
        }

        /// <summary>Returns a string Dictionary by parsing a list of delimited key/value pairs</summary>
        /// <param name="input"></param>
        /// <param name="pairSeparator">Regex pattern that separates the pairs of key/values</param>
        /// <param name="keyValueSeparator">Regex pattern that separates the a key from its value. Ex) '='</param>
        /// <param name="comparer">Comparer to use when creating the dictionary.</param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseDictionary(this string input,
            string pairSeparator, string keyValueSeparator, IEqualityComparer<string> comparer) {

            var pattern = string.Format(@"^([^{0}]+)\s*{0}\s*(.*?)$", keyValueSeparator);

            return Regex.Split(input, pairSeparator)
                    .Where(s => s.Trim().Length > 0)
                    .Select(s => {
                        var m = Regex.Match(s, pattern);

                        if (!m.Success)
                            throw new Exception(string.Format("No matches found in string: {0} with regex {1}", s, pattern));

                        return new[] { m.Groups[1].Value, m.Groups[2].Value };
                    })
                    .ToDictionary(x => x[0], x => x[1], comparer);
        }

        /// <summary>Returns a string Dictionary by parsing a list of delimited key/value pairs. Uses default comparer.</summary>
        /// <param name="input"></param>
        /// <param name="pairSeparator">Regex pattern that separates the pairs of key/values</param>
        /// <param name="keyValueSeparator">Regex pattern that separates the a key from its value. Ex) '='</param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseDictionary(this string input, string pairSeparator, string keyValueSeparator) {
            return input.ParseDictionary(pairSeparator, keyValueSeparator, EqualityComparer<string>.Default);
        }

        /// <summary>Repeats a character, turning it into a string</summary>
        /// <param name="input"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public static string Repeat(this char input, int times) {
            return new String(input, times);
        }

        /// <summary>
        /// Repeats a string <paramref name="input"/> <paramref name="times"/>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public static string Repeat(this string input, int times) {
            return new StringBuilder().Insert(0, input, times).ToString();
        }

        /// <summary>
        /// Returns the right <paramref name="length"/> characters of the given string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Right(this string input, int length) {
            return input.Substring(input.Length - length);
        }

        /// <summary>
        /// Returns a string in snake case. Often used for converting object names.
        /// Is smart about repeating capital letters
        /// </summary>
        /// <example>
        /// TransactionID => Transaction_ID
        /// FirstName => First_Name
        /// CPRNumber => CPR_Number
        /// ReferenceIDNumber => Reference_ID_Number
        /// </example>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SnakeCase(this string input) {
            input = input.Replace(' ', '_');
            input = Regex.Replace(input, @"(?<=\p{Lu}{2,}|\p{Ll})(\p{Lu})(?=\p{Ll})", "_$1");
            input = Regex.Replace(input, @"(?<=\p{Ll})(\p{Lu}{2,})", "_$1");
            input = Regex.Replace(input, "_{2,}", "_");
            return input;
        }

        /// <summary>
        /// Returns a string as its byte representation, hex encoded
        /// </summary>
        /// <param name="s"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static string ToHex(this string s, Encoding enc) {
            return enc.GetBytes(s).ToHex();
        }

        /// <summary>
        /// Returns a string as its <see cref="Encoding.UTF8"/> byte representation, hex encoded
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToHex(this string s) {
            return s.ToHex(Encoding.UTF8);
        }

        /// <summary>
        /// Attempts to convert the string to something (T) using the ConvertTo&lt;T&gt; method
        /// If exceptions are encountered, false is returned and the assigner is not called
        /// If no exceptions are found, assigner is called and true is returned
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">String to parse</param>
        /// <param name="assign">Assignment action delegate. Used to handle the converted result.</param>
        /// <returns><see cref="bool"/></returns>
        /// <example>"1234".TryParse&lt;int&gt;(r => myLocalIntVar = r)</example>
        public static bool TryConvert<T>(this string input, Action<T> assign) {
            return input.TryConvert(s => s.ConvertTo<T>(), assign);
        }

        /// <summary>
        /// Attempts to convert the string to something else using a Converter. If the conversion works
        /// (as in, there are no exceptions thrown), the assignment delegate is called and true is returned.
        /// If any exceptions are found when calling the converter, false is returned and the assigner is not called.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">String to parse</param>
        /// <param name="converter">Conversion delegate. String to T</param>
        /// <param name="assign">Assignment action delegate. Used to handle the converted result.</param>
        /// <returns><see cref="bool"/></returns>
        /// <example>"12".TryParse(s => int.Parse(s), r => myLocalIntVar = r)</example>
        public static bool TryConvert<T>(this string input, Converter<string, T> converter, Action<T> assign) {
            try {
                assign(converter(input));
                return true;
            }
            catch (Exception) {
                return false;
            }
        }

        /// <summary>
        /// Returns a string until the specified search string is found.
        /// The original string is returned if the search string is not found.
        /// The search string is optionally included in the result
        /// </summary>
        /// <param name="s"></param>
        /// <param name="searchPattern">String to search for</param>
        /// <param name="includeSearchString">Include the search string in the result</param>
        /// <returns></returns>
        /// <example>"Sentence one. Sentence Two.".UntilIndexOf(".", true) => "Sentence One."</example>
        /// <example>"Boston, MA".UntilIndexOf("," false) => "Boston"</example>
        public static string UntilIndexOf(this string s, string searchPattern, bool includeSearchString = false) {
            var m = Regex.Match(s, searchPattern);

            if (!m.Success)
                return s;

            var offset = m.Index + (includeSearchString ? m.Value.Length : 0);
            
            return s.Substring(0, offset);
        }

        /// <summary>
        /// Returns a string until the specified search char is found.
        /// The original string is returned if the search char is not found.
        /// The search string is optionally included in the result
        /// </summary>
        /// <param name="s"></param>
        /// <param name="search">String to search for</param>
        /// <param name="includeSearchString">Include the search string in the result</param>
        /// <returns></returns>
        /// <example>"Sentence one. Sentence Two.".UntilIndexOf('.', true) => "Sentence One."</example>
        public static string UntilIndexOf(this string s, char search, bool includeSearchString = false) {
            var index = s.IndexOf(search);
            return index <= 0 ? s : s.Substring(0, index + (includeSearchString ? 1 : 0));
        }

        /// <summary>
        /// Returns an array of words in the given string.
        /// Splits the given string using the regex \W (non-word) element. This is [^a-zA-Z_0-9]
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string[] Words(this string s) {
            return Regex.Split(s, @"(?s)\W+");
        }

        /// <summary>Returns an array of whitespace-separated elements in the given string. Similar to qw() in Perl</summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string[] W(this string s) {
            return Regex.Split(s, @"(?s)\s+");
        }
    }
}
