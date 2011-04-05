using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NContrib.Extensions {

    public static class ArrayExtensions {

        public static string Describe<T>(this T[] source) {
            if (source == null)
                return "(null)";

            if (source.Length == 0)
                return "(empty)";

            return "[" + source.Select(x => x.ToString()).Join(", ") + "]";
        }

    }
}
