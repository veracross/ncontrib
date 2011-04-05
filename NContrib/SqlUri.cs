using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using NContrib.Extensions;

namespace NContrib {

    public static class SqlUri {

        /// <summary>
        /// Parses a SQL Server connection URI in the format
        /// mssql://username:password@server\instance/database
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ParseSqlServerUri(string s) {
            const string re = @"(?xi)
                ^
                (?<protocol>mssqls?)
                ://
                (?:   (?<username>[^:]+) :)?
                (?:   (?<password>[^@]+) )?
                (?: @ (?<server>[^/]+) ) /?
                (?:   (?<database>\w+)   )?
                $";

            var match = Regex.Match(s, re);

            if (!match.Success)
                throw new ArgumentException("Not recognised as a SQL Server connection string");

            Func<string, string> getVal = name => match.Groups[name].Value.IsBlank() ? null : Uri.UnescapeDataString(match.Groups[name].Value);

            var protocol = getVal("protocol").ToLower();

            if (protocol.NotIn("mssql", "mssqls"))
                throw new ArgumentException(string.Format("{0} is not a valid protocol", protocol));

            var csb = new SqlConnectionStringBuilder();

            csb.DataSource = getVal("server");

            if (getVal("protocol") == "mssqls")
                csb.Encrypt = true;

            if (getVal("username").IsBlank())
                csb.IntegratedSecurity = true;
            else {
                csb.UserID = getVal("username");
                csb.Password = getVal("password");
            }

            return csb.ConnectionString;
        }
    }
}