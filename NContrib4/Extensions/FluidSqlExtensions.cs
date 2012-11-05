using System.Collections.Generic;
using NContrib;

namespace NContrib4.Extensions {

    public static class FluidSqlExtensions {

        public static List<dynamic> ExecuteDynamic(this FluidSql fs, bool convertDbNull = true) {

            return fs.ExecuteAndTransform(dr => (dynamic) new DynamicDataRecord(dr, convertDbNull));

        }
    }
}
