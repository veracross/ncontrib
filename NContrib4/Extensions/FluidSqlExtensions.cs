using System.Collections.Generic;
using NContrib;

namespace NContrib4.Extensions {

    public static class FluidSqlExtensions {

        public static List<dynamic> ExecuteDynamic(this FluidSql fs) {

            return fs.ExecuteAndTransform(dr => (dynamic) new DynamicDataRecord(dr));

        }
    }
}
