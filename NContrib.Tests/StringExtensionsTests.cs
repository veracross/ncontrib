using NContrib.Extensions;
using NUnit.Framework;

namespace NContrib.Tests {

    [TestFixture]
    public class StringExtensionsTests {

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
