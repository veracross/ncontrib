using System;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using NContrib.Extensions;

namespace NContrib {

    public class ShorthandString {

        public string Format { get; protected set; }

        public IFormatProvider FormatProvider { get; protected set; }

        public ShorthandString(string s)
            : this(s, CultureInfo.CurrentCulture) { }

        public ShorthandString(string s, string cultureName)
            : this(s, CultureInfo.GetCultureInfo(cultureName)) {}

        public ShorthandString(string s, IFormatProvider formatProvider) {
            Format = s;
            FormatProvider = formatProvider;
        }

        public static string operator %(ShorthandString s, object[] args) {
            return string.Format(s.FormatProvider, s.Format, args);
        }

        public static string operator %(ShorthandString s, IDictionary values) {
            return s.Format.Inject(values, s.FormatProvider);
        }

        public static string operator %(ShorthandString s, object values) {
            return s.Format.Inject(values, s.FormatProvider);
        }

        public static string operator %(ShorthandString s, string singleValue) {
            return string.Format(s.FormatProvider, s.Format, singleValue);
        }

        public static string operator *(ShorthandString s, int times) {
            return s.Format.Repeat(times);
        }

        public static bool operator &(ShorthandString s, string pattern) {
            return Regex.Match(s.Format, pattern).Success;
        }
    }
}
