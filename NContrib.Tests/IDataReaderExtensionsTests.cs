using System;
using System.Data;
using NContrib.Extensions;
using NUnit.Framework;

namespace NContrib.Tests {
    
    [TestFixture]
    public class IDataReaderExtensionsTests {

        private DataTable _dt1;

        [TestFixtureSetUp]
        public void Setup() {
            _dt1 = new DataTable();
            _dt1.Columns.Add("id", typeof(int));
            _dt1.Columns.Add("time", typeof(DateTime));
            _dt1.Columns.Add("amount", typeof(decimal));
            _dt1.Columns.Add("description", typeof(string));

            _dt1.Columns[0].Unique = true;
            _dt1.DefaultView.Sort = "id asc";

            _dt1.Rows.Add(1, new DateTime(2011, 03, 31, 18, 22, 13), 70.41f, "Kaffe");
            _dt1.Rows.Add(2, DateTime.Now, 204.54f, "Pizza");

            Assert.AreEqual(2, _dt1.Rows.Count);
        }

        [Test]
        public void GetValue_ValidColumnIdentifiers_ReturnsCorrectType() {

            using (var dr = new DataTableReader(_dt1)) {
                dr.Read();
                Assert.AreEqual(1, dr.GetValue<int>("id"), "ID");
                Assert.AreEqual(1L, dr.GetValue<long>("id"), "ID as long");
                Assert.AreEqual(1, dr.GetValue<ushort>("id"), "ID as short");
                Assert.AreEqual(1, dr.GetValue<byte>("id"), "ID as byte");

                Assert.AreEqual(new DateTime(2011, 03, 31, 18, 22, 13), dr.GetValue<DateTime>("time"), "Time");
                Assert.AreEqual(70.41f, dr.GetValue<decimal>("amount"), "Amount");
                Assert.AreEqual("Kaffe", dr.GetValue<string>("description"), "Description");
            }
        }

        [Test]
        public void GetColumnAsArrayOf_Int_ReturnsIntArray() {
            var expected = new[] {1, 2};

            using (var dr = new DataTableReader(_dt1)) {
                var values = dr.GetColumnAsArrayOf<int>("id");
                Assert.AreEqual(expected, values);
            }
        }

        [Test]
        public void GetColumnNames_Dt1_GetsColumnNames() {
            var expected = new[] {"id", "time", "amount", "description"};

            using (var dr = new DataTableReader(_dt1)) {
                var names = dr.GetColumnNames();
                Assert.AreEqual(expected, names);
            }
        }
    }
}
