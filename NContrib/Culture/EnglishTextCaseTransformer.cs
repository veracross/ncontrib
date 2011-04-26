using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NContrib.Extensions;

namespace NContrib.Culture {

    /// <summary>
    /// .NET port of https://launchpad.net/titlecase.py
    /// </summary>
    public class EnglishTextCaseTransformer : ITextCaseTransformer {

        private const string Small = @"a|an|and|as|at|but|by|en|for|if|in|of|on|or|the|to|v\.?|via|vs\.?";

        private const string Punct = @"!""#$%&'‘()*+,\-./:;?@\[\\\]_`{|}~";

        // words that according to the New York Times Style Guide should be lower case in titles
        private static readonly Regex SmallWords = new Regex(@"(?i)^(" + Small + @")$");

        private static readonly Regex InlineFullstop = new Regex(@"(?i)\p{Lu}[.]\p{Lu}");

        private static readonly Regex UcElsewhere = new Regex(@"\p{P}*?\p{L}+\p{Lu}+?");

        private static readonly Regex CapFirst = new Regex(@"^[" + Punct + @"]*?([\p{L}])");

        private static readonly Regex SmallFirst = new Regex(@"(?i)^([" + Punct + @"]*)(" + Small + @")\b");

        private static readonly Regex SmallLast = new Regex(@"(?i)\b(" + Small + @")[" + Punct + @"]?$");

        private static readonly Regex Subphrase = new Regex(@"([:.;?!][ ])(" + Small + @")");

        // second character of the string is an apostrophe, after a D, O, or L (O'Leary)
        private static readonly Regex AposSecond = new Regex(@"(?i)^[dol]['‘]\p{L}+");

        // all upper-case letters, whitespace, and punctuation
        private static readonly Regex AllCaps = new Regex(@"^[\p{Lu}\s\W]+$");

        // Two upper case letters, each with a full-stop
        private static readonly Regex UcInitials = new Regex(@"^(?:\p{Lu}{1}\.{1}|\p{Lu}{1}\.{1}\p{Lu}{1})+$");

        private static readonly Regex MacMc = new Regex(@"^([Mm]a?c)(\w+)");


        public string ToTitleCase(string title, IEnumerable<string> specials = null) {

            var results = new List<string>();

            var words = Regex.Split(title, @"[\t ]");

            foreach (var readonlyWord in words) {

                var word = readonlyWord;

                if (specials != null) {
                    var special = specials.SingleOrDefault(sp => Regex.IsMatch(word, sp, RegexOptions.IgnoreCase));
                    if (special != null) {
                        results.Add(Regex.Replace(word, special, special, RegexOptions.IgnoreCase));
                        continue;
                    }                    
                }
                    
                if (AllCaps.IsMatch(word)) {
                    
                    if (specials != null && specials.Contains(word)) {
                        results.Add(word);
                        continue;
                    }

                    if (UcInitials.IsMatch(word)) {
                        results.Add(word);
                        continue;
                    }

                    if (word.Contains('&')) {
                        results.Add(word);
                        continue;
                    }

                    word = word.ToLower();
                }

                if (AposSecond.IsMatch(word)) {
                    var letters = word.ToCharArray();
                    letters[0] = Char.ToUpper(letters[0]);
                    letters[2] = Char.ToUpper(letters[2]);
                    results.Add(new String(letters));
                    continue;
                }

                if (InlineFullstop.IsMatch(word) || UcElsewhere.IsMatch(word)) {
                    results.Add(word);
                    continue;
                }

                if (SmallWords.IsMatch(word)) {
                    results.Add(word);
                    continue;
                }

                var macMcMatch = MacMc.Match(word);
                if (macMcMatch.Success) {
                    results.Add(BlindTitleCase(macMcMatch.Groups[0].Value) + BlindTitleCase(macMcMatch.Groups[1].Value));
                    continue;
                }

                if (word.Contains('/') && !word.Contains("//")) {
                    var slashed = word.Split('/')
                        .Select(w => CapFirst.Replace(w, m => m.Groups[0].Value.ToUpper()))
                        .ToArray();

                    results.Add(string.Join("/", slashed));

                    continue;
                }

                if (word.Contains('-')) {
                    var hyphenated = word.Split('-')
                        .Select(w => CapFirst.Replace(w, m => m.Groups[0].Value.ToUpper()))
                        .ToArray();

                    results.Add(string.Join("-", hyphenated));
                    continue;
                }

                results.Add(BlindTitleCase(word));
            }

            var result = string.Join(" ", results.ToArray());

            result = SmallFirst.Replace(result, m => m.Groups[1].Value + BlindTitleCase(m.Groups[2].Value));
            result = SmallLast.Replace(result, m => BlindTitleCase(m.Groups[0].Value));
            result = Subphrase.Replace(result, m => m.Groups[1] + BlindTitleCase(m.Groups[2].Value));

            return result;
        }

        private static string BlindTitleCase(string input) {

            bool found = false;
            var chars = input.ToCharArray();

            for (var i = 0; i < chars.Length; i++) {
                var c = chars[i];

                if (Char.IsDigit(c))
                    found = true;

                if (found) {
                    chars[i] = Char.ToLower(c);
                    continue;
                }

                if (Char.IsLetter(chars[i])) {
                    chars[i] = char.ToUpper(c);
                    found = true;
                }
            }

            return new String(chars);
        }
    }
}