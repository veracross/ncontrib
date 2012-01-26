using System;
using System.Data.SqlClient;
using NContrib.Extensions;

namespace NContrib {

    public static class SqlUri {

        /// <summary>
        /// Parses a SQL Server connection URI in the format
        /// mssql://username:password@server[/instance]/database
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string Parse(string uri) {

            var u = new Uri(uri);

            if (u.Scheme.ToLower().NotIn("mssql", "mssqls"))
                throw new ArgumentException(string.Format("{0} is not a valid protocol", u.Scheme));

            var csb = new SqlConnectionStringBuilder();
            csb.DataSource = u.Host;
            csb.Encrypt = u.Scheme.ToLower() == "mssqls";

            var pathParts = u.AbsolutePath.Split('/');
            if (pathParts.Length == 3) {
                csb.DataSource += "\\" + Uri.UnescapeDataString(pathParts[1]);
                csb.InitialCatalog = Uri.UnescapeDataString(pathParts[2]);
            }
            if (pathParts.Length == 2)
                csb.InitialCatalog = Uri.UnescapeDataString(pathParts[1]);

            if (u.UserInfo.IsBlank())
                csb.IntegratedSecurity = true;
            else {
                
                var userParts = u.UserInfo.Split(new[] {':'}, 2);
                csb.UserID = Uri.UnescapeDataString(userParts[0]);

                if (userParts.Length == 2)
                    csb.Password = Uri.UnescapeDataString(userParts[1]);
            }
            
            return csb.ConnectionString;
        }
    }
}
