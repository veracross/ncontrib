using System;
using System.Data;
using NContrib.Extensions;
using NUnit.Framework;

namespace NContrib.Tests {
    
    [TestFixture]
    public class IDataReaderExtensionsTests {

        [Test]
        public void GetValue_ValidColumnIdentifiers_ReturnsCorrectType() {

            var dt = new DataTable();
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("time", typeof(DateTime));
            dt.Columns.Add("amount", typeof(decimal));
            dt.Columns.Add("description", typeof(string));

            dt.Columns[0].Unique = true;
            dt.DefaultView.Sort = "id asc";

            dt.Rows.Add(1, new DateTime(2011, 03, 31, 18, 22, 13), 70.41f, "Kaffe");
            dt.Rows.Add(2, DateTime.Now, 204.54f, "Pizza");

            Assert.AreEqual(2, dt.Rows.Count);
            
            var dr = new DataTableReader(dt);

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
}
