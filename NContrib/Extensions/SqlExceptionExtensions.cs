using System;
using System.Data.SqlClient;
using System.Linq;

namespace NContrib.Extensions
{
    public static class SqlExceptionExtensions
    {
        /// <summary>
        /// List of non-error messages that were lumped in with the errors
        /// These are print messages, or anything with a class/severity of 10 or lower
        /// </summary>
        /// <returns></returns>
        public static string[] InfoMessages(this SqlException ex)
        {
            return ex.Errors.Cast<SqlError>()
                     .Where(e => e.Class <= 10)
                     .OrderBy(e => e.LineNumber)
                     .Select(e => e.Message)
                     .ToArray();
        }

        /// <summary>
        /// List of actual error messages. Anything with a class/severity of 11 or higher
        /// Excludes informational messages. Those can be retrieved from <see cref="InfoMessages"/>
        /// </summary>
        /// <returns></returns>
        public static string[] ErrorMessages(this SqlException ex)
        {
            return ex.Errors.Cast<SqlError>()
                     .Where(e => e.Class >= 11)
                     .OrderBy(e => e.LineNumber)
                     .Select(e => e.Message)
                     .ToArray();
        }

        /// <summary>
        /// I
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string Describe(this SqlException ex)
        {
            var errors = ex.Errors.Cast<SqlError>().Where(e => e.Class >= 11).Select(FormatError).ToArray();
            return string.Join(Environment.NewLine, errors);
        }

        private static string FormatError(SqlError err)
        {
            var message = err.Message;

            // print statements
            if (err.Class == 0)
                return message;

            // errors that can be traced to a spot in a procedure/trigger/module
            if (!string.IsNullOrEmpty(err.Procedure))
                return message + " (" + err.Procedure + ":line " + err.LineNumber + ")";

            // generic errors
            return string.Format("{0} (Msg {1}, Level {2}, State {3}, Line {4})",
                                 err.Message, err.Number, err.Class, err.State, err.LineNumber);
        }
    }
}
