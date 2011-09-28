using System;

namespace NContrib {

    /// <summary>
    /// Handles cases where code is unreachable but the compiler doesn't know it
    /// </summary>
    /// <example>
    /// Consider a function with argument "arg"
    /// 
    /// if (arg != "v1" && arg != "v2) throw new ArgumentException("arg");
    /// if (arg == "v1") return "r1";
    /// if (arg == "v2") return "r2";
    /// // unreachable code
    /// </example>
    public class RuhRohException : Exception { }
}
