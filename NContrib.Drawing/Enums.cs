using System.Drawing;

namespace NContrib.Drawing {

    /// <summary>
    /// Controls how width and height parameters are treated when scaling
    /// </summary>
    public enum ScaleMethod {
        /// <summary>
        /// Treats both the width and height as maximums
        /// </summary>
        ToFit,

        /// <summary>
        /// Treats both the width and height as minimums, filling the requested <see cref="Size"/>
        /// </summary>
        /// <example>
        /// If you wanted to scale a 1024x768 (4:3) image to fill a 1280x800 (16:10) display,
        /// this option would yield a <see cref="Size"/> of 1280x960
        /// </example>
        ToFill,
    }

    /// <summary>
    /// Methods for scaling the Size of an image
    /// </summary>
    public enum ScaleType {
        /// <summary>
        /// Size can grow or shrink to satisfy the request
        /// </summary>
        ExpandOrShrink,

        /// <summary>
        /// Size can only be expanded; can't shrink
        /// </summary>
        Expand,

        /// <summary>
        /// Size can only shrink
        /// </summary>
        Shrink,
    }
}
