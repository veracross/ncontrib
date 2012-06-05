using System;
using System.Linq;

namespace NContrib.Extensions {

    public static class ArrayExtensions {

        private static readonly Random Randomizer = new Random();

        public static string Describe<T>(this T[] source) {

            if (source == null)
                return "(null)";

            if (source.Length == 0)
                return "(empty)";

            return "[" + source.Select(x => x.ToString()).Join(", ") + "]";
        }

        /// <summary>
        /// Uses Fisher-Yates shuffle on the array to shuffle the order
        /// </summary>
        /// <seealso cref="http://www.dotnetperls.com/fisher-yates-shuffle"/>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        public static void Shuffle<T>(this T[] source) {

            for (var i = source.Length; i > 1; i--) {

                var j = Randomizer.Next(i);
                var tmp = source[j];
                source[j] = source[i - 1];
                source[i - 1] = tmp;
            }
        }
    }
}
