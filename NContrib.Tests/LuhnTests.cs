using System.Diagnostics;
using NUnit.Framework;

namespace NContrib.Tests {

    [TestFixture]
    public class LuhnTests {

        [Test]
        public void IsValid_ValidLuhnNumber_Pass() {
            Assert.AreEqual(true, Luhn.IsValid("354957031609855"), "IMEI");
            Assert.AreEqual(true, Luhn.IsValid("6761631794313583"), "Meestro");
        }

        [Test]
        public void Generate_VisaCardNumber_Valid() {
            var value = Luhn.Generate(16, "4581");

            Trace.WriteLine("Generated " + value);

            Assert.AreEqual(16, value.Length);
            Assert.AreEqual(true, Luhn.IsValid(value));
            Assert.AreEqual("4581", value.Substring(0, 4));
        }
    }
}
