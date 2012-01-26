using System;

namespace NContrib.Culture {

    public class EnglishTextGenerator : ITextGenerator {

        // Single-digit and small number names
        private static readonly string[] SmallNumbers = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };

        // Tens number names from twenty upwards
        private static readonly string[] Tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

        // Scale number names for use during recombination
        private static readonly string[] ScaleNumbers = { "", "Thousand", "Million", "Billion" };

        public string NumberToWords(decimal number) {
            var whole = Math.Truncate(number);
            var remain = number - whole;
            var pct = Math.Truncate(remain * 100);

            return NumberToWords((int)whole) + " and " + pct + "/100";
        }

        public string NumberToWords(int number) {

            if (number == 0)
                return SmallNumbers[0];

            var digitGroups = new int[4];
            var positive = Math.Abs(number);

            for (var i = 0; i < 4; i++) {
                digitGroups[i] = positive % 1000;
                positive /= 1000;
            }

            // Convert each three-digit group to words
            var groupText = new string[4];

            for (var i = 0; i < 4; i++)
                groupText[i] = ThreeDigitGroupToWords(digitGroups[i]);

            // Recombine the three-digit groups
            var combined = groupText[0];

            // Determine whether an 'and' is needed
            var appendAnd = (digitGroups[0] > 0) && (digitGroups[0] < 100);

            // Process the remaining groups in turn, smallest to largest
            for (var i = 1; i < 4; i++) {
                // Only add non-zero items
                if (digitGroups[i] == 0) continue;

                // Build the string to add as a prefix
                var prefix = groupText[i] + " " + ScaleNumbers[i];

                if (combined.Length != 0) {
                    prefix += appendAnd ? " and " : ", ";
                }

                // Opportunity to add 'and' is ended
                appendAnd = false;

                // Add the three-digit group to the combined string
                combined = prefix + combined;
            }

            // Negative rule
            if (number < 0)
                combined = "Negative " + combined;

            return combined;
        }

        private static string ThreeDigitGroupToWords(int threeDigits) {
            // Initialise the return text
            var groupText = "";

            // Determine the hundreds and the remainder
            var hundreds = threeDigits / 100;
            var tensUnits = threeDigits % 100;

            // Hundreds rules
            if (hundreds != 0) {
                groupText += SmallNumbers[hundreds] + " Hundred";

                if (tensUnits != 0)
                    groupText += " and ";
            }

            // Determine the tens and units
            var tens = tensUnits / 10;
            int units = tensUnits % 10;

            // Tens rules
            if (tens >= 2) {
                groupText += Tens[tens];
                if (units != 0)
                    groupText += " " + SmallNumbers[units];
                
            }
            else if (tensUnits != 0)
                groupText += SmallNumbers[tensUnits];

            return groupText;
        }
    }
}
