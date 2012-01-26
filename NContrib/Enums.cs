using NContrib.Extensions;

namespace NContrib {
    
    public enum TextTransform {
        None,
        Upper,
        Lower
    }

    /// <summary>
    /// Two methods for wrapping text. One only hard-breaks a word when the word can't fit on a line
    /// even by itself. The other always breaks no matter what.
    /// </summary>
    /// <seealso cref="StringExtensions.Wrap(string, int, TextWrapMethod, string, string)"/>
    public enum TextWrapMethod {
        /// <summary>Only hard-breaks when the word can't fit on a line even by itself</summary>
        HardBreakWhenNecessary,

        /// <summary>Always hard-break. Fills each line with as many chars as can fit and then hard-breaks.</summary>
        HardBreakAlways
    }
}
