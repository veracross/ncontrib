using System;
using NContrib.Extensions;
using NUnit.Framework;

namespace NContrib.Tests {

    [TestFixture]
    public class DateTimeExtensionsTests {

        [Test]
        public void ToBeatTime_KnownTimes_CorrectBeatTime() {
            var dt = new DateTime(2011, 04, 05, 11, 37, 11);

            Assert.AreEqual(484, Math.Floor(dt.ToBeatTime()));
        }

        [Test]
        public void ToUnixTime_KnownTime_Converts() {
            Assert.AreEqual(1301957923L, new DateTime(2011, 04, 04, 22, 58, 43).ToUnixTime());
            Assert.AreEqual(1000000000L, new DateTime(2001, 09, 09, 01, 46, 40).ToUnixTime());
            Assert.AreEqual(1234567890L, new DateTime(2009, 02, 13, 23, 31, 30).ToUnixTime());
            Assert.AreEqual(3141592653L, new DateTime(2069, 07, 21, 00, 37, 33).ToUnixTime());
        }
    }
}
