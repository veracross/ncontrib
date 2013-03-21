using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using NContrib.Extensions;

namespace NContrib
{
    /// <summary>
    /// This formats a <see cref="SqlCommand"/> so that it's highly readable and probably executable,
    /// but it is not meant to generate SQL that gets executed. It's for describing a command.
    /// </summary>
    public static class SqlCommandFormatter
    {
        public static CultureInfo FormatCulture = CultureInfo.InvariantCulture;

        public static string FormatCommand(SqlCommand cmd)
        {
            string desc;

            var parameters =
                cmd.Parameters.Cast<SqlParameter>()
                   .Where(p => p.Direction != ParameterDirection.ReturnValue)
                   .ToDictionary(p => "@" + p.ParameterName, FormatSqlParameter);

            if (cmd.CommandType == CommandType.StoredProcedure)
            {
                desc = "exec " + cmd.CommandText;

                if (parameters.Any())
                    desc += " " + parameters.Select(p => p.Key + " = " + p.Value).Join(", ");

                return desc;
            }

            desc = cmd.CommandText;

            if (parameters.Any())
                desc = parameters.Aggregate(desc, (current, p) => current.Replace(p.Key, p.Value));

            return desc;
        }

        public static string FormatSqlParameter(SqlParameter p)
        {
            var formatted = FormatSqlValue(p.Value);

            if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
                formatted += " output";

            return formatted;
        }

        public static string FormatSqlValue(object value)
        {
            if (value == null)
                return "null";

            var type = value.GetType();
            var typeCode = Type.GetTypeCode(type);

            if (type.IsEnum)
            {
                value = value.ToString();
                typeCode = TypeCode.String;
            }

            if (type.IsArray && type.GetElementType() == typeof(byte))
                return "0x" + BitConverter.ToString((byte[]) value).Replace("-", "");

            if (typeCode == TypeCode.DateTime)
                return string.Format(FormatCulture, "'{0:yyyy-MM-dd HH:mm:ss.fff}'", value);

            if (typeCode == TypeCode.Boolean)
                return ((bool)value) ? "1" : "0";

            if (typeCode == TypeCode.String)
                return "'" + ((string)value).Replace("'", "''") + "'";

            if (typeCode == TypeCode.Double)
                return ((double)value).ToString(FormatCulture);

            if (typeCode == TypeCode.Decimal)
                return ((decimal)value).ToString(FormatCulture);

            return value.ToString();
        }
    }
}