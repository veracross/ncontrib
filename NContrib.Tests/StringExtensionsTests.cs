using NContrib.Extensions;
using NUnit.Framework;

namespace NContrib.Tests {

    [TestFixture]
    public class StringExtensionsTests {

        [Test]
        public void IsBlank_BlankText_Detected() {
            string nullval = null;
            const string empty = "";
            const string whitespace = " \t\n";
            const string space = " ";

            Assert.IsTrue(nullval.IsBlank());
            Assert.IsTrue(empty.IsBlank());
            Assert.IsTrue(whitespace.IsBlank());
            Assert.IsTrue(space.IsBlank());
        }

        [Test]
        public void IsBlank_NonBlankText_Detected() {
            Assert.IsFalse(" f".IsBlank());
            Assert.IsFalse("f".IsBlank());
            Assert.IsFalse("\ntext\n".IsBlank());
        }

        [Test]
        public void IsNotBlank_BlankText_Detected() {
            string nullval = null;
            const string empty = "";
            const string whitespace = " \t\n";
            const string space = " ";

            Assert.IsFalse(nullval.IsNotBlank());
            Assert.IsFalse(empty.IsNotBlank());
            Assert.IsFalse(whitespace.IsNotBlank());
            Assert.IsFalse(space.IsNotBlank());
        }

        [Test]
        public void IsNotBlank_NonBlankText_Detected() {
            Assert.IsTrue(" f".IsNotBlank());
            Assert.IsTrue("f".IsNotBlank());
            Assert.IsTrue("\ntext\n".IsNotBlank());
        }

        [Test]
        public void FromIndexOf_ValidRegexPatterns_Pass() {
            Assert.AreEqual("Malmö", "S-21144 Malmö".FromIndexOf(@"\d+\s*"));
            Assert.AreEqual("01921", "Boxford, MA 01921".FromIndexOf(@",\s*\p{Lu}{2}\s*"));
        }

        [Test]
        public void Right_String_ReturnsRightCharacters() {
            const string s = "No highs no lows must be Bose";

            Assert.AreEqual("Bose", s.Right(4));
            Assert.AreEqual("e", s.Right(1));
        }

        [Test]
        public void SnakeCase_CamelCaseText_Snakeifies() {
            Assert.AreEqual("Transaction_ID", "TransactionID".SnakeCase());
            Assert.AreEqual("First_Name", "FirstName".SnakeCase());
            Assert.AreEqual("CPR_Number", "CPRNumber".SnakeCase());
            Assert.AreEqual("Reference_ID_Number", "ReferenceIDNumber".SnakeCase());
            Assert.AreEqual("Local_ATM_Transaction", "LocalATMTransaction".SnakeCase());
            Assert.AreEqual("Person_SSN", "PersonSSN".SnakeCase());
            Assert.AreEqual("Person_Ssn", "PersonSsn".SnakeCase());
            Assert.AreEqual("BMWCCA_Member_ID", "BMWCCAMemberID".SnakeCase());
            Assert.AreEqual("BMWCCA_Member_Id", "BMWCCAMemberId".SnakeCase());
            Assert.AreEqual("ID_Number", "IDNumber".SnakeCase());
            Assert.AreEqual("Id_Number", "IdNumber".SnakeCase());
            Assert.AreEqual("ELeg_Id_Expiration", "ELegIdExpiration".SnakeCase());
        }

        [Test]
        public void UntilIndexOf_ValidRegexPatterns_Pass() {
            Assert.AreEqual("S-21144", "S-21144 Malmö".UntilIndexOf(@"\d+", true));
            Assert.AreEqual("S", "S-21144 Malmö".UntilIndexOf(@"\-\d+"));
        }
    }
}
