using System;

namespace NContrib.International {

    public class CurrencyIdentifierFormatException : Exception {
        public string CurrencyIdentifier { get; set; }

        public CurrencyIdentifierFormatException(string identifier)
            : base(identifier + " is not a valid currency identifier") {
            CurrencyIdentifier = identifier;
        }
    }

    public class UnknownCurrencyIdentifierException : Exception {
        public string CurrencyIdentifier { get; set; }

        public UnknownCurrencyIdentifierException(string identifier)
            : base(identifier + " is not a known currency identifier") {
            CurrencyIdentifier = identifier;
        }
    }
}
