using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NContrib.International;
using NUnit.Framework;

namespace NContrib.Tests.International {

    [TestFixture]
    public class CountryTests {

        [Test]
        public void GetById_VariousValidIds_GetsExpectedCountry() {

            Assert.AreEqual("SE", Country.GetById("SE").CodeAlpha2);
            Assert.AreEqual("SE", Country.GetById("SWE").CodeAlpha2);
            Assert.AreEqual("SE", ((Country) "SE").CodeAlpha2);
            Assert.AreEqual("SE", Country.GetById(752).CodeAlpha2);
        }
    }
}
