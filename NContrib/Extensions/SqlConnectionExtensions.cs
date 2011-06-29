using System.Data;
using System.Data.SqlClient;

namespace NContrib.Extensions {

    public static class SqlConnectionExtensions {

        public static FluidSqlDatabase ToFluidSqlDatabase(this SqlConnection cn) {
            return new FluidSqlDatabase(cn);
        }

        /// <summary>
        /// Creates a <see cref="SqlCommand"/> using the given connection
        /// </summary>
        /// <param name="cn"></param>
        /// <param name="command"></param>
        /// <param name="commandType">Defaults to <see cref="CommandType.Text"/></param>
        /// <returns></returns>
        public static SqlCommand CreateCommand(this SqlConnection cn, string command, CommandType commandType = CommandType.Text) {
            return new SqlCommand(command, cn) { CommandType = commandType };
        }

        /// <summary>
        /// Executes a Text non-query (update, delete, etc) against the given connection and returns nothing.
        /// </summary>
        /// <param name="cn"></param>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public static void ExecuteNonQuery(this SqlConnection cn, string command, object parameters = null) {
            using (var cmd = cn.CreateCommand(command)) {
                cmd.AddParameters(parameters);
                cmd.ExecuteNonQuery();
            }
        }

        public static void ExecuteProcedureNonQuery(this SqlConnection cn, string procedureName, object parameters = null) {
            using (var cmd = cn.CreateCommand(procedureName, CommandType.StoredProcedure)) {
                cmd.AddParameters(parameters);
                cmd.ExecuteNonQuery();
            }
        }

        public static SqlDataReader ExecuteProcedureReader(this SqlConnection cn, string procedureName, object parameters = null) {
            return cn.ExecuteReader(procedureName, CommandType.StoredProcedure, parameters);
        }

        public static SqlDataReader ExecuteQueryReader(this SqlConnection cn, string query, object parameters = null) {
            return cn.ExecuteReader(query, CommandType.StoredProcedure, parameters);
        }

        public static SqlDataReader ExecuteReader(this SqlConnection cn, string command, CommandType commandType, object parameters = null) {
            using (var cmd = cn.CreateCommand(command, commandType)) {
                cmd.AddParameters(parameters);
                return cmd.ExecuteReader();
            }
        }

        public static T ExecuteScalar<T>(this SqlConnection cn, string commandText, CommandType commandType = CommandType.Text, object parameters = null) {
            using (var cmd = cn.CreateCommand(commandText, commandType)) {
                cmd.AddParameters(parameters);

                var value = cmd.ExecuteScalar();

                if (typeof(T) == typeof(bool)) {
                    var s = value as string;

                    if (s == "0") value = "False";
                    if (s == "1") value = "True";
                }

                return value.ConvertTo<T>();
            }
        }
    }
}
