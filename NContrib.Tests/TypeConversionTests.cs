using System;
using System.Data;
using System.Globalization;
using NContrib.Extensions;
using NUnit.Framework;

namespace NContrib.Tests {

    [TestFixture]
    public class TypeConversionTests {


        [Test]
        public void ConvertTo_EuroDecimals_Converts() {
            Assert.AreEqual(123.45m, "123,45".ConvertTo<decimal>(CultureInfo.GetCultureInfo("sv-SE")));
            Assert.AreEqual(123.45m, "123.45".ConvertTo<decimal>(CultureInfo.GetCultureInfo("en-GB")));
        }

        [Test]
        public void ConvertTo_BooleanNames_Converts() {
            Assert.AreEqual(false, "False".ConvertTo<bool>());
            Assert.AreEqual(true, "True".ConvertTo<bool>());
        }

        [Test]
        public void ConvertTo_InvalidBooleanNames_ExceptionThrown() {
            Assert.Throws<FormatException>(() => "0".ConvertTo<bool>());
            Assert.Throws<FormatException>(() => "1".ConvertTo<bool>());
            Assert.Throws<FormatException>(() => "no".ConvertTo<bool>());
            Assert.Throws<FormatException>(() => "yes".ConvertTo<bool>());
            Assert.Throws<FormatException>(() => "vrai".ConvertTo<bool>());
        }

        [Test]
        public void ConvertTo_DateTimeStrings_Converts() {
            var dt1 = new DateTime(2011, 03, 12, 14, 23, 50);
            Assert.AreEqual(dt1, "2011-03-12 14:23:50".ConvertTo<DateTime>(CultureInfo.GetCultureInfo("sv-SE")));
            Assert.AreEqual(dt1, "12/3/2011 14:23:50".ConvertTo<DateTime>(CultureInfo.GetCultureInfo("en-GB")));
            Assert.AreEqual(dt1, "3/12/2011 14:23:50".ConvertTo<DateTime>(CultureInfo.GetCultureInfo("en-US")));
        }

        [Test]
        public void ConvertTo_NullValueType_ReturnsInstances() {
            object temp = null;
            Assert.AreEqual(0, temp.ConvertTo<int>());
            Assert.AreEqual(false, temp.ConvertTo<bool>());
            Assert.AreEqual(0f, temp.ConvertTo<float>());
            Assert.AreEqual(0m, temp.ConvertTo<decimal>());
            Assert.AreEqual(0L, temp.ConvertTo<long>());
            Assert.AreEqual(0d, temp.ConvertTo<double>());
            Assert.AreEqual(0, temp.ConvertTo<byte>());
            Assert.AreEqual(0u, temp.ConvertTo<uint>());
            Assert.AreEqual(new Point(0, 0), temp.ConvertTo<Point>());
            Assert.AreEqual(Operators.None, temp.ConvertTo<Operators>());
        }

        [Test]
        public void ConvertTo_EnumName_Converts() {
            Assert.AreEqual(Operators.Telenor, "Telenor".ConvertTo<Operators>());
            Assert.AreEqual(CommandType.StoredProcedure, "StoredProcedure".ConvertTo<CommandType>());
        }

        /* Culture info isn't being used by reflection as expected
        
        [Test]
        public void ParseConvert_BooleanNames_Converts() {
            Assert.AreEqual(false, "False".ParseConvert(typeof(bool), CultureInfo.CurrentCulture));
            Assert.AreEqual(true, "True".ParseConvert(typeof(bool), CultureInfo.CurrentCulture));
        }

        [Test]
        public void ParseConvert_IntegerStrings_Converts() {
            Assert.AreEqual(12345, "12345".ParseConvert(typeof(int), CultureInfo.InvariantCulture), "Plain");
            Assert.AreEqual(345, "    345".ParseConvert(typeof(int), CultureInfo.InvariantCulture), "Whitespace");
        }

        [Test]
        public void ParseConvert_DecimalStrings_Converts() {
            Assert.AreEqual(123.45m, "123,45".ParseConvert(typeof(decimal), CultureInfo.GetCultureInfo("sv-SE")));
            Assert.AreEqual(12345.67m, "12 345,67".ParseConvert(typeof(decimal), CultureInfo.GetCultureInfo("sv-SE")));

            Assert.AreEqual(123.45m, "123.45".ParseConvert(typeof(decimal), CultureInfo.InvariantCulture));
            Assert.AreEqual(12345.67m, "12,345.67".ParseConvert(typeof(decimal), CultureInfo.InvariantCulture));
        }
        */
    }
}
