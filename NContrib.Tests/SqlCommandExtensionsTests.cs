using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using NContrib.Extensions;
using NUnit.Framework;

namespace NContrib.Tests {

    [TestFixture]
    public class SqlCommandExtensionsTests {

        [Test]
        public void AddParameters_DictionaryObject_AddsAllParameters() {

            var parameters = new Dictionary<string, object> {
                {"id", 1},
                {"name", "Reginald"},
                {"date_of_birth", new DateTime(1985, 10, 08)}
            };

            var cmd = new SqlCommand();
            cmd.AddParameters(parameters);

            var commandParameters = cmd.Parameters.Cast<SqlParameter>().ToDictionary(p => p.ParameterName, p => p.Value);
            var commandParameterKeys = commandParameters.Select(p => p.Key).ToArray();

            var missing = parameters.Where(p => p.Key.NotIn(commandParameterKeys));

            Assert.AreEqual(0, missing.Count());

            foreach (var p in parameters)
                Assert.AreEqual(p.Value, cmd.Parameters[p.Key].Value);
        }

        [Test]
        public void AddParameters_AnonymousType_AddsAllPropertiesAsParameters() {
            var parameters = new {Id = 1, Name = "Reginald", DateOfBirth = new DateTime(1985, 10, 08)};

            var cmd = new SqlCommand();
            cmd.AddParameters(parameters, s => s.ToSnakeCase().ToLower());

            var commandParameters = cmd.Parameters.Cast<SqlParameter>().ToDictionary(p => p.ParameterName, p => p.Value);
            var commandParameterKeys = commandParameters.Select(p => p.Key).ToArray();

            var expectedKeys = new[] {"id", "name", "date_of_birth"};

            expectedKeys.Action(k => Assert.That(commandParameterKeys.Contains(k)));

            Assert.AreEqual(1, cmd.Parameters["id"].Value);
            Assert.AreEqual("Reginald", cmd.Parameters["name"].Value);
            Assert.AreEqual(new DateTime(1985, 10, 08), cmd.Parameters["date_of_birth"].Value);
        }
    }
}
