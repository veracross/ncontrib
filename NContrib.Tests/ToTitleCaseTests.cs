using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NContrib.Extensions;
using NUnit.Framework;

namespace NContrib.Tests {

    [TestFixture]
    public class ToTitleCaseTests {

        private IList<Tuple<string, string>> _expectations;

        [TestFixtureSetUp]
        public void Setup() {

            var temp = new List<Tuple<string, string>>();

            using (var stream = new StreamReader("fodder/TitleCase.txt")) {
                string line;

                while ((line = stream.ReadLine()) != null) {
                    if (string.IsNullOrEmpty(line)) continue;
                    if (line.StartsWith("#")) continue;

                    temp.Add(new Tuple<string, string>(line, stream.ReadLine()));
                }
            }

            _expectations = temp;
        }

        [Test]
        public void ToTitleCase_TitleCaseTextSourceFiles_TitleCases() {

            var successes = 0;

            foreach (var pair in _expectations) {
                Console.WriteLine("Input:   " + pair.Item1);
                Console.WriteLine("Expect:  " + pair.Item2);

                var result = pair.Item1.ToTitleCase();
                var success = result == pair.Item2;

                if (success)
                    successes++;

                Console.WriteLine("{0}: {1}", success ? "Success" : "FAILURE", result);
                Console.WriteLine();
            }

            Console.WriteLine("Successes: {0}. Failures: {1}", successes, _expectations.Count - successes);
        }

        [Test]
        public void ToTitleCase_TextWithExceptions_ExceptionsHonoured() {

            string[] specials = {"USA", "JPMorgan", "HSBC", "UBS", "PNC", "CNB", "ING", "BBVA", "CIT", "RBC", "TD", "TCF"};

            Assert.AreEqual("HSBC Bank, USA, N.A.", "HSBC BANK, USA, N.A.".ToTitleCase(specials));
            Assert.AreEqual("JPMorgan Chase Bank", "JPMORGAN CHASE BANK".ToTitleCase(specials));
            Assert.AreEqual("ING Direct", "ING DIRECT".ToTitleCase(specials));
            Assert.AreEqual("Sterling Bank", "STERLING BANK".ToTitleCase(specials), "Make sure specials don't interfere");
            Assert.AreEqual("OpenCable Officially Becomes tru2way",
                            "OpenCable officially becomes tru2way".ToTitleCase(new[] {"tru2way"}));
        }

        [Test]
        public void ToTitleCase_Abbreviations_RetainCase() {
            Assert.AreEqual("The Connecticut Bank & Trust Company", "THE CONNECTICUT BANK & TRUST COMPANY".ToTitleCase());
            Assert.AreEqual("HSBC Bank", "HSBC BANK".ToTitleCase());
            Assert.AreEqual("Connector for the FSB", "CONNECTOR FOR THE FSB".ToTitleCase());
            Assert.AreEqual("AT&T Q&A Session", "AT&T Q&A SESSION".ToTitleCase());
        }
    }
}
