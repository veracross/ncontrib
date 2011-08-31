using System;
using System.Linq;
using NContrib.Extensions;

namespace NContrib {

    /// <summary>
    /// Generates and validates Luhn numbers
    /// http://en.wikipedia.org/wiki/Luhn_algorithm
    /// </summary>
    public static class Luhn {

        /// <summary>Generates a number that passes Luhn validation</summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Generate(int length) {
            return Generate(length, null);
        }

        /// <summary>Checks to see if this number as a string passes a Luhn check</summary>
        /// <param name="number">Number as a string to check</param>
        /// <returns>True if the number passes a Luhn check. False if it does not.</returns>
        public static bool IsValid(string number) {
            var total = 0;

            for (var i = number.Length; i > 0; i--) {
                var multiplier = 1 + ((number.Length - i) % 2);
                var digit = int.Parse(number.Substring(i - 1, 1));
                var sum = digit * multiplier;

                if (sum > 9)
                    sum -= 9;

                total += sum;
            }

            return (total % 10 == 0);
        }

        /// <summary>
        /// Generates a number that passes Luhn validation.
        /// Number starts with the specified seed value
        /// </summary>
        /// <param name="length">Total length of the number to generate</param>
        /// <param name="seed">Optional numbers with which to seed the sequence</param>
        /// <returns></returns>
        public static string Generate(int length, string seed) {
            int pos = 0, sum = 0;
            var digits = new int[length];
            var r = new Random();

            if (seed.IsNotBlank() && seed.Length >= length)
                throw new ArgumentException("Seed length must be less than the requested string length");

            if (!seed.IsEmpty()) {
                for (var i = 0; i < seed.Length; i++)
                    digits[i] = int.Parse(seed.Substring(i, 1));

                pos += seed.Length;
            }

            while (pos < length - 1)
                digits[pos++] = r.Next(0, 10);

            var lenOffset = (length + 1) % 2;

            for (pos = 0; pos < length - 1; pos++) {
                if ((pos + lenOffset) % 2 > 0) {
                    var t = digits[pos] * 2;
                    if (t > 9)
                        t -= 9;
                    sum += t;
                }
                else
                    sum += digits[pos];
            }

            digits[length - 1] = (10 - (sum % 10)) % 10;

            return String.Concat(digits.Select(d => d.ToString()).ToArray());
        }
    }
}
