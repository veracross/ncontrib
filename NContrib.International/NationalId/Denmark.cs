using System;
using System.Globalization;
using System.Text.RegularExpressions;
using NContrib.Extensions;

namespace NContrib.International.NationalId {

    public sealed class Denmark {

        private const string DateFormat = "ddMMyy";

        public string CprNumber { get; private set; }

        public string Sequence { get; private set; }

        public Gender Gender { get; private set; }

        public DateTime DateOfBirth { get; private set; }

        private Denmark() { }

        public static Denmark Parse(string cprNumber) {
            if (!IsValidFormat(cprNumber))
                throw new ArgumentException("Invalid CPR Number", "cprNumber");

            return new Denmark {
                DateOfBirth = ExtractDateComponent(cprNumber),
                Gender = ExtractGenederComponent(cprNumber),
                Sequence = ExtractSequenceComponent(cprNumber),
                CprNumber = cprNumber,
            };
        }

        public static bool IsValidFormat(string cprNumber) {

            // basic pattern checking. not bullet-proof because of the date component
            // but a fair first-pass check
            if (!Regex.IsMatch(cprNumber, @"^[0-3][0-9][0-1][0-9][0-9]{2}-[0-9]{4}$"))
                return false;

            // ensure the date is a valid date
            try {
                ExtractDateComponent(cprNumber);
            }
            catch (FormatException) {
                return false;
            }

            return true;
        }

        private static DateTime ExtractDateComponent(string cprNumber) {
            return DateTime.ParseExact(cprNumber.Left(6), DateFormat, CultureInfo.InvariantCulture);
        }

        private static string ExtractSequenceComponent(string cprNumber) {
            return cprNumber.Right(4);
        }

        private static Gender ExtractGenederComponent(string cprNumber) {
            return cprNumber.Right(1).ConvertTo<int>() % 2 == 0 ? Gender.Female : Gender.Male;
        }

        public override string ToString() {
            return CprNumber;
        }

        public static implicit operator Denmark(string cprNumber) {
            return Parse(cprNumber);
        }
    }
}
