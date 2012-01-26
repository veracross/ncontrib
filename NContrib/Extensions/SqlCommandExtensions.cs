using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace NContrib.Extensions {

    public static class SqlCommandExtensions {

        /// <summary>
        /// Reflects the properties from the <paramref name="parameters"/> parameter and adds them all to the command with their values
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parameters"></param>
        /// <param name="convertParameterName"></param>
        /// <returns></returns>
        public static SqlCommand AddParameters(this SqlCommand cmd, object parameters, Converter<string, string> convertParameterName = null) {

            if (parameters == null)
                return cmd;

            var pdict = parameters.GetType().GetProperties()
                .ToDictionary(p => p.Name, p => p.GetValue(parameters, null));

            return pdict.Count() == 0 ? cmd : cmd.AddParameters(pdict, convertParameterName);
        }

        public static SqlCommand AddParameters<T>(this SqlCommand cmd, IDictionary<string, T> parameters, Converter<string, string> convertParameterName = null) {
            parameters.Action(p => cmd.Parameters.AddWithValue(convertParameterName == null ? p.Key : convertParameterName(p.Key), p.Value));
            return cmd;
        }

        /// <summary>
        /// Determine the likely command type based on the command text. One-word command is assumed to be a procedure, otherwise command text
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static CommandType DetectCommandType(string commandText) {
            return commandText.Contains(' ') ? CommandType.Text : CommandType.StoredProcedure;
        }
    }
}
