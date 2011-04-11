using System.Collections.Generic;
using System.Linq;
using NContrib.Extensions;

namespace NContrib.International {

    public class CurrencyCollection : HashSet<Currency> {

        /// <summary>Get a currency by its numeric or alpha code</summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Currency this[string id] {
            get {
                if (!Currency.IsValidIdentifier(id))
                    throw new CurrencyIdentifierFormatException(id);


                var currency = id.IsDigits()
                           ? this.SingleOrDefault(c => c.NumericCode == id)
                           : this.SingleOrDefault(s => s.Code == id);

                if (currency == null)
                    throw new UnknownCurrencyIdentifierException(id);

                return currency;
            }
        }

        public void Add(string code, string numericCode, int minorUnit, string englishName, string formatCulture = null) {
            Add(new Currency(code, numericCode, minorUnit, englishName, formatCulture));
        }
    }
}