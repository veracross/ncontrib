using System;
using System.Globalization;
using System.Text.RegularExpressions;
using NContrib.Extensions;

namespace NContrib.International.NationalId {

    public sealed class DanishCprNumber {

        private const string DateFormat = "ddMMyy";

        public string CprNumber { get; private set; }

        public string Sequence { get; private set; }

        public Gender Gender { get; private set; }

        public DateTime DateOfBirth { get; private set; }

        private DanishCprNumber() { }

        public static DanishCprNumber Parse(string cprNumber) {
            if (!IsValidFormat(cprNumber))
                throw new ArgumentException("Invalid CPR Number", "cprNumber");

            return new DanishCprNumber {
                DateOfBirth = GetDateOfBirth(cprNumber),
                Gender = GetGender(cprNumber),
                Sequence = GetSequence(cprNumber),
                CprNumber = cprNumber,
            };
        }

        public static bool IsValidFormat(string cprNumber) {

            // basic pattern checking. not bullet-proof because of the date component
            // but a fair first-pass check
            if (!Regex.IsMatch(cprNumber, RegexLibrary.NationalId.DanishCprNumber))
                return false;

            // ensure the date is a valid date
            try {
                GetDateOfBirth(cprNumber);
            }
            catch (FormatException) {
                return false;
            }

            return true;
        }

        public static DateTime GetDateOfBirth(string cprNumber) {
            return DateTime.ParseExact(cprNumber.Left(6), DateFormat, CultureInfo.InvariantCulture);
        }

        public static string GetSequence(string cprNumber) {
            return cprNumber.Right(4);
        }

        public static Gender GetGender(string cprNumber) {
            return cprNumber.Right(1).ConvertTo<int>() % 2 == 0 ? Gender.Female : Gender.Male;
        }

        public override string ToString() {
            return CprNumber;
        }

        public static implicit operator DanishCprNumber(string cprNumber) {
            return Parse(cprNumber);
        }
    }
}
