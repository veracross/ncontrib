using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using NContrib.Extensions;
using NContrib.International;
using NUnit.Framework;

namespace NContrib.Tests.International {

    [TestFixture]
    public class CurrencyTest {

        [Test]
        public void MinorUnit_Currency_ExpectedSize() {
            Assert.AreEqual(0, Currency.GetById("JPY").MinorUnit, "JPY");
            Assert.AreEqual(2, Currency.GetById("CHF").MinorUnit, "CHF");
            Assert.AreEqual(3, Currency.GetById("KWD").MinorUnit, "KWD");
        }

        [Test]
        public void AmountToImpliedDecimal_SameAmountDifferentCurrencies_CorrectIntegerLength() {
            const decimal amount = 123.40m;

            Assert.AreEqual(123400, Currency.GetById("KWD").AmountToImpliedDecimal(amount), "KWD");
            Assert.AreEqual(12340, Currency.GetById("DKK").AmountToImpliedDecimal(amount), "DKK");
            Assert.AreEqual(123, Currency.GetById("JPY").AmountToImpliedDecimal(amount), "JPY");
        }

        [Test]
        public void GetById_InvalidId_ThrowsException() {
            Assert.Throws<CurrencyIdentifierFormatException>(() => Currency.GetById(""));
            Assert.Throws<CurrencyIdentifierFormatException>(() => Currency.GetById("1"));
            Assert.Throws<CurrencyIdentifierFormatException>(() => Currency.GetById("LL"));
            Assert.Throws<CurrencyIdentifierFormatException>(() => Currency.GetById("LL9"));
            Assert.Throws<CurrencyIdentifierFormatException>(() => Currency.GetById("1B3"));
        }

        [Test]
        public void GetById_ValidAlphaAndNumericIds_ReturnCurrencies() {
            Currency twd = "901";
            Assert.AreEqual("TWD", twd.Code);
            Assert.AreEqual("GBP", ((Currency) "GBP").Code);
            Assert.AreEqual("SEK", ((Currency) "752").Code);
            Assert.AreEqual("DKK", Currency.GetById("DKK").Code);
        }

        [Test]
        public void BuiltInCurrencies_Should_Have_Valid_Format_Locales() {
            var currencies = Currency.BuiltInCurrencies.Where(c => c.FormatCulture.IsNotBlank());

            foreach (var c in currencies) {

                // aside from the euro, gibraltar pound, and st helena pound, and the caribbean,
                // the first two chars of the currency code should match the country code on the format locale
                if (c.Code.NotIn("EUR", "GIP", "SHP") && c.FormatCulture != "en-029")
                    Assert.AreEqual(c.Code.Left(2), c.FormatCulture.Right(2), "Testing for " + c.EnglishName);

                // ensure the locale exists by creating it
                var ci = CultureInfo.GetCultureInfo(c.FormatCulture);

                Assert.NotNull(ci);

                Trace.WriteLine("{\"" + c.Code + "\", \"" + 1234.56m.ToString("C", ci) + "\"},");
            }
        }

        [Test]
        public void FormatAmount_SameAmountDifferentCurrencies_FormatsAccordingToCurrencysLocale() {
            const decimal amount = 1234.56m;

            var expected = new Dictionary<string, string> {
                {"AED", "د.إ.‏ 1,234.56"},
                {"AFN", "؋1٬234٫56"},
                {"ALL", "1.234,56Lek"},
                {"AMD", "1,234.56 դր."},
                {"ARS", "$ 1.234,56"},
                {"AUD", "$1,234.56"},
                {"AZN", "1 234,56 man."},
                {"BAM", "1.234,56 KM"},
                {"BBD", "$1,234.56"},
                {"BDT", "৳ 1,234.56"},
                {"BGN", "1 234,56 лв."},
                {"BHD", "د.ب.‏ 1,234.560"},
                {"BND", "$1.235"},
                {"BOB", "$b 1.234,56"},
                {"BRL", "R$ 1.234,56"},
                {"BYR", "1 234,56 р."},
                {"BZD", "BZ$1,234.56"},
                {"CAD", "$1,234.56"},
                {"CHF", "Fr. 1'234.56"},
                {"CLP", "$ 1.234,56"},
                {"CNY", "¥1,234.56"},
                {"COP", "$ 1.234,56"},
                {"CRC", "₡1.234,56"},
                {"CZK", "1 234,56 Kč"},
                {"DKK", "kr. 1.234,56"},
                {"DOP", "RD$1,234.56"},
                {"DZD", "1,234.56 DZD"},
                {"EGP", "ج.م.‏ 1,234.56"},
                {"ETB", "ETB1,234.56"},
                {"EUR", "1.234,56 €"},
                {"FJD", "$1,234.56"},
                {"GBP", "£1,234.56"},
                {"GEL", "1 234,56 Lari"},
                {"GIP", "£1,234.56"},
                {"GYD", "$1,234.56"},
                {"HKD", "HK$1,234.56"},
                {"HNL", "L. 1,234.56"},
                {"HRK", "1.234,56 kn"},
                {"HUF", "1 234,56 Ft"},
                {"IDR", "Rp1.235"},
                {"ILS", "₪ 1,234.56"},
                {"INR", "Rs. 1,234.56"},
                {"IQD", "د.ع.‏ 1,234.56"},
                {"IRR", "ريال 1,234/56"},
                {"ISK", "1.235 kr."},
                {"JMD", "J$1,234.56"},
                {"JOD", "د.ا.‏ 1,234.560"},
                {"JPY", "¥1,235"},
                {"KES", "S1,234.56"},
                {"KGS", "1 234-56 сом"},
                {"KHR", "1,234.56៛"},
                {"KRW", "₩1,235"},
                {"KWD", "د.ك.‏ 1,234.560"},
                {"KYD", "$1,234.56"},
                {"KZT", "Т1 234-56"},
                {"LAK", "1,234.56₭"},
                {"LBP", "ل.ل.‏ 1,234.56"},
                {"LKR", "රු. 1,234.56"},
                {"LTL", "1.234,56 Lt"},
                {"LVL", "Ls 1 234,56"},
                //{"LYD", "د.ل.‏ 1,234.56"},
                {"MAD", "د.م.‏ 1,234.56"},
                {"MKD", "1.234,56 ден."},
                {"MNT", "1 234,56₮"},
                {"MOP", "MOP1,234.56"},
                {"MVR", "1,234.56 ރ."},
                {"MXN", "$1,234.56"},
                {"MYR", "RM1,234.56"},
                {"NGN", "N 1,234.56"},
                {"NIO", "C$ 1,234.56"},
                {"NOK", "kr 1 234,56"},
                {"NPR", "रु1,234.56"},
                {"NZD", "$1,234.56"},
                {"OMR", "ر.ع.‏ 1,234.560"},
                {"PAB", "B/. 1,234.56"},
                {"PEN", "S/. 1,234.56"},
                {"PHP", "Php1,234.56"},
                {"PKR", "Rs1,234.56"},
                {"PLN", "1 234,56 zł"},
                {"PYG", "Gs 1.234,56"},
                {"QAR", "ر.ق.‏ 1,234.56"},
                {"RON", "1.234,56 lei"},
                {"RSD", "1.234,56 Din."},
                {"RUB", "1 234,56р."},
                {"RWF", "RWF 1 234,56"},
                {"SAR", "ر.س.‏ 1,234.56"},
                {"SEK", "1.234,56 kr"},
                {"SGD", "$1,234.56"},
                {"SHP", "£1,234.56"},
                {"SYP", "ل.س.‏ 1,234.56"},
                {"THB", "฿1,234.56"},
                {"TND", "د.ت.‏ 1,234.560"},
                {"TRY", "1.234,56 TL"},
                {"TTD", "TT$1,234.56"},
                {"TWD", "NT$1,234.56"},
                {"UAH", "1 234,56₴"},
                {"USD", "$1,234.56"},
                {"UYU", "$U 1.234,56"},
                {"UZS", "1 235 so'm"},
                {"VEF", "Bs. F. 1.234,56"},
                {"VND", "1.234,56 ₫"},
                {"YER", "ر.ي.‏ 1,234.56"},
                {"ZAR", "R 1 234,56"},
                {"ZWL", "Z$1,234.56"},
            };

            foreach (var k in expected) {
                var currency = Currency.GetById(k.Key);
                
                // the framework uses non-breaking spaces for grouping-separators and
                // that seems to have not carried over when i made the dictionary above
                // so just replace non-breaking spaces with regular spaces
                var actual = currency.FormatAmount(amount).Replace('\u00A0', ' ');

                Assert.AreEqual(k.Value.Replace('\u00A0', ' '), actual, currency.Code);
            }
        }
    }
}
