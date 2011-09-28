using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NContrib.Tests {

    [TestFixture]
    public class FluidSqlTests {

        [Test]
        public void CreateInsertCommand_Fields_CommandTextCreated() {

            var f = new Dictionary<string, object> {{"id", 1234}, {"last_name", "Rogers"}};

            var fs = new FluidSql(null);

            fs.CreateInsertCommand("Person", f);

            Assert.AreEqual("insert into Person (id, last_name) values(@id, @last_name)", fs.Command.CommandText);
        }
    }
}
