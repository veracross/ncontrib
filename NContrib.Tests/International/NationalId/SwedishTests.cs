using System;
using NContrib.International;
using NContrib.International.NationalId;
using NUnit.Framework;

namespace NContrib.Tests.International.NationalId {

    [TestFixture]
    public class SwedishTests {

        [Test]
        public void Parse_ValidPersonnummer_ReturnsAllValidParts() {
            var pn = Swedish.Parse("811218-9876");
            Assert.AreEqual(Gender.Male, pn.Gender);
            Assert.AreEqual(new DateTime(1981, 12, 18), pn.DateOfBirth);
            Assert.AreEqual("987", pn.Löpnummer);
            Assert.AreEqual(6, pn.CheckDigit);
            Assert.IsTrue(pn.IsChecksumValid());
        }

        [Test]
        public void ToString_ValidPersonnummer_FormatsCorrectly() {
            var pn1 = Swedish.Parse("811218-9876");

            Assert.AreEqual("19811218-9876", pn1.ToString());
            Assert.AreEqual("811218-9876", pn1.ToShortString());

            var pn2 = Swedish.Parse("18811218-9876");

            Assert.AreEqual("18811218+9876", pn2.ToString());
            Assert.AreEqual("811218+9876", pn2.ToShortString());
        }
    }
}
