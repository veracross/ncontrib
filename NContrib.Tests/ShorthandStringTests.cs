using System;
using System.Collections.Generic;
using NContrib.Extensions;
using NUnit.Framework;

namespace NContrib.Tests {

    [TestFixture]
    public class ShorthandStringTests {

        [Test]
        public void Pct_Object_Formats() {

            var s = "Hello, {Name}. The time is now {Now:yyyy-MM-dd HH:mm}";
            var o = new {Name = "Mike", Now = new DateTime(2011, 06, 29, 16, 06, 00)};
            

            Assert.AreEqual("Hello, Mike. The time is now 2011-06-29 16:06", s.S() % o);
            Assert.AreEqual("Hi, Mike", "Hi, {0}".S() % "Mike");
        }

        [Test]
        public void Pct_Object_FormatsWithLocale() {

            var d = new Dictionary<string, object> {
                {"Amount", 20.45m},
                {"Currency", "EUR"},
                {"Date", new DateTime(2011, 10, 31)}
            };

            Assert.AreEqual("You owe 20,45 EUR by 2011-10-31", "You owe {Amount:N} {Currency} by {Date:d}".S("sv-SE") % d, "sv-SE");
            Assert.AreEqual("You owe 20.45 EUR by 31/10/2011", "You owe {Amount:N} {Currency} by {Date:d}".S("en-GB") % d, "en-GB");
            Assert.AreEqual("You owe 20.45 EUR by 10/31/2011", "You owe {Amount:N} {Currency} by {Date:d}".S("en-US") % d, "en-US");
        }

        [Test]
        public void Ast_String_Repeats() {

            Assert.AreEqual("----------", "-".S() * 10);
        }

        [Test]
        public void Amp_String_RegexMatch() {

            Assert.IsTrue("123443554".S() & @"^\d+$", "All numbers");
            Assert.IsTrue("iõhäqåhäö".S() & @"^\p{Ll}+$", "All lower-case letters");
        }
    }
}
