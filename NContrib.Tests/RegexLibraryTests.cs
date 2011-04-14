using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace NContrib.Tests {

    [TestFixture]
    public class RegexLibraryTests {

        [Test]
        public void MimeType_ListOfKnownTypes_MatchesAll() {
            var path = Path.GetFullPath("fodder/mime-type-list.txt");
            var lines = File.ReadAllLines(path);

            var re = new Regex(RegexLibrary.MimeType, RegexOptions.Compiled);

            foreach (var type in lines)
                Assert.IsTrue(re.Match(type).Success, "Testing " + type);
        }

        [Test]
        public void DanishCprNumber_ValidCprNumbers_MatchesAll() {
            var valid = new[] {
                "211062-5629",
            };

            var re = new Regex(RegexLibrary.NationalId.DanishCprNumber, RegexOptions.Compiled);

            foreach (var cpr in valid)
                Assert.IsTrue(re.Match(cpr).Success, "Testing " + cpr);
        }

        [Test]
        public void SwedishPersonNumber_ValidPersonNUmbers_MatchesAll() {
            var valid = new[] {
                "8112189876",
                "811218-9876",
                "811218-9876",
                "811218+9876",
                "198112189876",
                "19811218-9876",
                "18811218+9876",
                "18811218-9876",
                "8005124469",
                "800512-4469",
                "19800512-4469",
                "198005124469",
            };

            var re = new Regex(RegexLibrary.NationalId.SwedishPersonNumber, RegexOptions.Compiled);

            foreach (var pn in valid)
                Assert.IsTrue(re.Match(pn).Success, "Testing " + pn);
        }
    }
}
