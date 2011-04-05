using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NContrib.Extensions;
using NUnit.Framework;

namespace NContrib.Tests {

    
    [TestFixture]
    public class ObjectExtensionsTests {

        [Test]
        public void In_IntIncollection_ReturnTrue() {
            var col = new[] {1, 2, 3, 4, 5};

            Assert.AreEqual(true, 2.In(col));
            Assert.AreEqual(true, 1.In(col));

            foreach (var i in col)
                Assert.AreEqual(true, i.In(col));

            Assert.AreEqual(true, 5.In(1, 3, 5, 6, 7), "Params");
        }

        [Test]
        public void In_IntNotInCollection_ReturnFalse() {
            var col = new[] { 1, 2, 3, 4, 5 };

            Assert.AreEqual(false, 0.In(col), "Testing 0");
            Assert.AreEqual(false, 9.In(col), "Testing 9");

            foreach (var i in col.Select(c => c + col.Max()))
                Assert.AreEqual(false, i.In(col), "Testing " + i);

            var odd = Enumerable.Range(1, 201).Where(i => i % 2 == 1).ToArray();

            for (var i = 0; i < odd.Length; i ++)
                Assert.AreEqual(i % 2 == 1, i.In(odd));
        }

        [Test]
        public void In_EnumInCollection_ReturnTrue() {
            var alsoDanish = new[] {Operators.Telenor, Operators.Tre, Operators.Telia};
            Assert.AreEqual(true, Operators.Telenor.In(alsoDanish));
            Assert.AreEqual(true, Operators.Tre.In(alsoDanish));
            Assert.AreEqual(true, Operators.Telia.In(alsoDanish));
        }

        [Test]
        public void In_StringInCollection_ReturnTrue() {
            var strings = new[] {"iPhone", "iPod", "Safari", "StarHub"};
            Assert.AreEqual(true, "iPhone".In(strings));
            Assert.AreEqual(true, "iphone".In(StringComparer.OrdinalIgnoreCase, strings));
            Assert.AreEqual(true, "ipod".In(StringComparer.OrdinalIgnoreCase, "ipod", "zune"));
            Assert.AreEqual(true, "StarHub".In("StarHub", "MobileOne", "SingTel"));
        }
    }
}
