using NContrib.Extensions;
using NUnit.Framework;

namespace NContrib.Tests {

    [TestFixture]
    public class StringExtensionsTests {

        [Test]
        public void ToCamelCase_VariousText_CamelCaseAsExpected() {
            var fodder = new[] { "background-repeat-x", "background color", "transaction ID", "transaction_id", "x-pos", "class_pk" };

            var expectedLower = new[] {"backgroundRepeatX", "backgroundColor", "transactionId", "transactionId", "xPos", "classPk"};
            var expectedUpper = new[] {"BackgroundRepeatX", "BackgroundColor", "TransactionId", "TransactionId", "XPos", "ClassPk"};

            for (var i = 0; i < fodder.Length; i++) {
                Assert.AreEqual(expectedLower[i], fodder[i].ToCamelCase(TextTransform.Lower), "Lower Test");
                Assert.AreEqual(expectedUpper[i], fodder[i].ToCamelCase(TextTransform.Upper), "Upper Test");
            }
        }

        [Test]
        public void ContainsOnly_StringWithOnlyGivenCharacters_True() {
            Assert.IsTrue("4111-1111-1111-1111".ContainsOnly('1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-'));
            Assert.IsTrue("XL".ContainsOnly('S', 'M', 'L', 'X'));
        }

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
        public void Left_StringOfAnyLength_ReturnsExpected() {
            Assert.AreEqual("MacBook", "MacBook Pro".Left(7));

            Assert.AreEqual("MacBook", "MacBook".Left(7));
            Assert.AreEqual("MacBook", "MacBook".Left(100));
            Assert.AreEqual(null, ((string)null).Left(100));
        }

        [Test]
        public void RegexReplace_VariousTextInput_Replaced() {
            Assert.AreEqual("authorizenet", "authorize.net".RegexReplace(@"\P{L}", ""));
            Assert.AreEqual("GSM", "GSM900".RegexReplace(@"\d", ""));
        }


        [Test]
        public void ToPlural_VariousTextNoNumber_Pluralizes() {
            // normal
            Assert.AreEqual("cars", "car".ToPlural());

            // irregular
            Assert.AreEqual("criteria", "criterion".ToPlural());

            // irregular
            Assert.AreEqual("leaves", "leaf".ToPlural());

            // some -y words should end up as -ies
            Assert.AreEqual("entities", "entity".ToPlural());

            // makine sure -y isn't being blindly replaced with -ies
            Assert.AreEqual("holidays", "holiday".ToPlural());

            Assert.AreEqual("women", "woman".ToPlural());
            Assert.AreEqual("wolves", "wolf".ToPlural());
            Assert.AreEqual("loaves", "loaf".ToPlural());
            Assert.AreEqual("knives", "knife".ToPlural());
            Assert.AreEqual("roofs", "roof".ToPlural());
            Assert.AreEqual("dwarfs", "dwarf".ToPlural());
            Assert.AreEqual("cellos", "cello".ToPlural());
            Assert.AreEqual("memos", "memo".ToPlural());
            Assert.AreEqual("stereos", "stereo".ToPlural());
            Assert.AreEqual("mice", "mouse".ToPlural());
            Assert.AreEqual("cherries", "cherry".ToPlural());
            Assert.AreEqual("days", "day".ToPlural());
            Assert.AreEqual("pros", "pro".ToPlural());
            Assert.AreEqual("staffs", "staff".ToPlural());
        }

        [Test]
        public void Right_String_ReturnsRightCharacters() {
            const string s = "No highs no lows must be Bose";

            Assert.AreEqual("Bose", s.Right(4));
            Assert.AreEqual("e", s.Right(1));

            Assert.AreEqual("MacBook", "MacBook".Right(20));
            Assert.AreEqual("Pro", "MacBook Pro".Right(3));

            Assert.AreEqual(null, ((string)null).Left(100));
        }

        [Test]
        public void ToSingular_VariousTextNoNumber_Singularizes() {
            // irregular
            Assert.AreEqual("goose", "geese".ToSingular());

            // irregular
            Assert.AreEqual("leaf", "leaves".ToSingular());

            // normal
            Assert.AreEqual("car", "cars".ToSingular());

            // -ies to -y
            Assert.AreEqual("quality", "qualities".ToSingular());

            // -ys to -y
            Assert.AreEqual("holiday", "holiday".ToSingular());
        }

        [Test]
        public void ToSnakeCase_CamelCaseText_Snakeifies() {
            Assert.AreEqual("Transaction_ID", "TransactionID".ToSnakeCase());
            Assert.AreEqual("First_Name", "FirstName".ToSnakeCase());
            Assert.AreEqual("CPR_Number", "CPRNumber".ToSnakeCase());
            Assert.AreEqual("Reference_ID_Number", "ReferenceIDNumber".ToSnakeCase());
            Assert.AreEqual("Local_ATM_Transaction", "LocalATMTransaction".ToSnakeCase());
            Assert.AreEqual("Person_SSN", "PersonSSN".ToSnakeCase());
            Assert.AreEqual("Person_Ssn", "PersonSsn".ToSnakeCase());
            Assert.AreEqual("BMWCCA_Member_ID", "BMWCCAMemberID".ToSnakeCase());
            Assert.AreEqual("BMWCCA_Member_Id", "BMWCCAMemberId".ToSnakeCase());
            Assert.AreEqual("ID_Number", "IDNumber".ToSnakeCase());
            Assert.AreEqual("Id_Number", "IdNumber".ToSnakeCase());
            Assert.AreEqual("ELeg_Id_Expiration", "ELegIdExpiration".ToSnakeCase());
        }

        [Test]
        public void UntilIndexOf_ValidRegexPatterns_Pass() {
            Assert.AreEqual("S-21144", "S-21144 Malmö".UntilIndexOf(@"\d+", true));
            Assert.AreEqual("S", "S-21144 Malmö".UntilIndexOf(@"\-\d+"));
        }
    }
}
