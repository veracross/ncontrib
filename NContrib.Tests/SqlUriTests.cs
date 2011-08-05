using System.Data.SqlClient;
using System.Diagnostics;
using System.Web;
using NUnit.Framework;

namespace NContrib.Tests {

    [TestFixture]
    public class SqlUriTests {

        [Test]
        public void ParseSqlServerUri_UrlEncodedParameters_Parses() {

            var uri = string.Format("mssqls://{0}:{1}@{2}/{3}",
                                    HttpUtility.UrlEncode("us3rn@:name"),
                                    HttpUtility.UrlEncode("p@ssw@£$;"),
                                    HttpUtility.UrlEncode("srv-x"),
                                    HttpUtility.UrlEncode("database"));

            Trace.WriteLine(uri);

            var cs = SqlUri.Parse(uri);
            Trace.WriteLine(cs);
            var cn = new SqlConnectionStringBuilder(cs);

            Assert.AreEqual("us3rn@:name", cn.UserID);
            Assert.AreEqual("p@ssw@£$;", cn.Password);
            Assert.AreEqual("srv-x", cn.DataSource);
            Assert.AreEqual("database", cn.InitialCatalog);
            Assert.AreEqual(false, cn.IntegratedSecurity);
            Assert.AreEqual(true, cn.Encrypt);
        }

        [Test]
        public void ParseSqlServerUri_EncryptUsernamePasswordServerDatabases_Parses() {
            const string uri = "mssqls://username:password@server/database";
            var cs = SqlUri.Parse(uri);
            Trace.WriteLine(cs);
            var cn = new SqlConnectionStringBuilder(cs);

            Assert.AreEqual("username", cn.UserID);
            Assert.AreEqual("password", cn.Password);
            Assert.AreEqual("server", cn.DataSource);
            Assert.AreEqual("database", cn.InitialCatalog);
            Assert.AreEqual(false, cn.IntegratedSecurity);
            Assert.AreEqual(true, cn.Encrypt);
        }

        [Test]
        public void ParseSqlServerUri_UsernamePasswordServerDatabases_Parses() {
            const string uri = "mssql://username:password@server/database";
            var cs = SqlUri.Parse(uri);
            Trace.WriteLine(cs);
            var cn = new SqlConnectionStringBuilder(cs);

            Assert.AreEqual("username", cn.UserID);
            Assert.AreEqual("password", cn.Password);
            Assert.AreEqual("server", cn.DataSource);
            Assert.AreEqual("database", cn.InitialCatalog);
            Assert.AreEqual(false, cn.IntegratedSecurity);
            Assert.AreEqual(false, cn.Encrypt);
        }

        [Test]
        public void ParseSqlServerUri_UsernameServerDatabases_Parses() {
            const string uri = "mssql://username@server/database";
            var cs = SqlUri.Parse(uri);
            Trace.WriteLine(cs);
            var cn = new SqlConnectionStringBuilder(cs);

            Assert.AreEqual("username", cn.UserID);
            Assert.IsEmpty(cn.Password);
            Assert.AreEqual("server", cn.DataSource);
            Assert.AreEqual("database", cn.InitialCatalog);
            Assert.AreEqual(false, cn.IntegratedSecurity);
            Assert.AreEqual(false, cn.Encrypt);
        }

        [Test]
        public void ParseSqlServerUri_EncryptServerDatabaseTrusted_Parses() {
            const string uri = "mssqls://server/database";
            var cs = SqlUri.Parse(uri);
            Trace.WriteLine(cs);
            var cn = new SqlConnectionStringBuilder(cs);

            Assert.AreEqual(true, cn.Encrypt);
            Assert.AreEqual("server", cn.DataSource);
            Assert.AreEqual("database", cn.InitialCatalog);
            Assert.AreEqual(true, cn.IntegratedSecurity);
        }

        [Test]
        public void ParseSqlServerUri_ServerNoDatabaseTrusted_Parses() {
            const string uri = "mssqls://server";
            var cs = SqlUri.Parse(uri);
            Trace.WriteLine(cs);
            var cn = new SqlConnectionStringBuilder(cs);

            Assert.AreEqual(true, cn.Encrypt);
            Assert.AreEqual("server", cn.DataSource);
            Assert.IsEmpty(cn.InitialCatalog);
            Assert.AreEqual(true, cn.IntegratedSecurity);
        }
    }
}
