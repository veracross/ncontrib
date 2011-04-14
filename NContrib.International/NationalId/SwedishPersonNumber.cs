using System;
using System.Globalization;
using System.Text.RegularExpressions;
using NContrib.Extensions;

namespace NContrib.International.NationalId {

    public class SwedishPersonNumber {

        public DateTime DateOfBirth { get; private set; }

        public string Löpnummer { get; private set; }

        public int CheckDigit { get; private set; }

        public Gender Gender { get; private set; }

        public static DateTime NowInSweden {
            get { return DateTime.UtcNow.ToTimeZone("Central European Standard Time"); }
        }

        private SwedishPersonNumber() {}

        private string Format(string dateFormat) {
            return DateOfBirth.ToString(dateFormat, CultureInfo.InvariantCulture) + CenturyIndicator(DateOfBirth) + Löpnummer + CheckDigit;
        }

        public override string ToString() {
            return Format("yyyyMMdd");
        }

        public string ToShortString() {
            return Format("yyMMdd");
        }

        public static SwedishPersonNumber Parse(string personnummer) {
            
            if (!IsValidFormat(personnummer))
                throw new ArgumentException("Invalid personnummer format", personnummer);

            // Including the century is optional
            var offset = personnummer.Length == 13 ? 2 : 0;

            var m = personnummer.Substring(offset + 2, 2).ConvertTo<int>();
            var d = personnummer.Substring(offset + 4, 2).ConvertTo<int>();

            DateTime dob;

            if (offset == 0) {
                // century has not been included, so we'll figure it out based on the separator

                var y = personnummer.Substring(offset, 2).ConvertTo<int>();
                var centuryIndicator = personnummer[6];

                var currentCentury = NowInSweden.Year.ToString().Left(2);
                dob = new DateTime((currentCentury + y).ConvertTo<int>(), m, d);

                if (dob > NowInSweden)
                    dob = dob.AddYears(-100);

                if (centuryIndicator == '+')
                    dob = dob.AddYears(-100);
            }
            else {
                // century was included, making our job easy
                dob = new DateTime(personnummer.Substring(0, 4).ConvertTo<int>(), m, d);
            }

            return new SwedishPersonNumber {
                DateOfBirth = dob,
                Löpnummer = personnummer.Substring(offset + 7, 3),
                CheckDigit = personnummer.Substring(offset + 10, 1).ConvertTo<int>(),
                Gender = GetGender(personnummer),
            };
        }

        public bool IsChecksumValid() {
            return IsChecksumValid(ToString());
        }

        public static bool IsChecksumValid(string personnummer) {
            var scrubbed = Regex.Replace(personnummer, @"\D", "");
            return Luhn.IsValid(scrubbed.Length == 12 ? scrubbed.Substring(2) : scrubbed);
        }

        public static Gender GetGender(int löpnummer) {
            return löpnummer % 2 == 0 ? Gender.Female : Gender.Male;
        }

        public static Gender GetGender(string personnummer) {
            return GetGender(personnummer.DigitAt(personnummer.Length - 2));
        }

        public static char CenturyIndicator(DateTime dob) {
            return dob.AddYears(100) < NowInSweden ? '+' : '-';
        }

        public static bool IsValidFormat(string personnummer) {
            return Regex.IsMatch(personnummer, RegexLibrary.NationalId.SwedishPersonNumber);
        }

        public static implicit operator SwedishPersonNumber(string personnummer) {
            return Parse(personnummer);
        }
    }
}
