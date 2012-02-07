using System;
using System.Drawing;

namespace NContrib.Drawing.Extensions {

    public static class SizeExtensions {

        /// <summary>
        /// Calculates a scaled image size, changing the size by a percentage
        /// </summary>
        /// <param name="sourceSize"></param>
        /// <param name="scalePercent">Scale percentage. 1 = 100% which yields no change
        /// .5 = 50% which would reduce the height and width by half.</param>
        /// <returns></returns>
        public static Size Scale(this Size sourceSize, float scalePercent) {

            if (Math.Abs(scalePercent - 1) < float.Epsilon)
                return sourceSize;

            var newHeight = (int)(sourceSize.Height * scalePercent);
            var newWidth = (int)(sourceSize.Width * scalePercent);

            return new Size(newWidth, newHeight);
        }

        /// <summary>
        /// Scales an image size, preserving aspect ratio.
        /// </summary>
        /// <param name="sourceSize"></param>
        /// <param name="width">Min or max allowed width. Min/Max controlled by the method parameter</param>
        /// <param name="height">Min or max allowed height. Min/Max controlled by the method parameter</param>
        /// <param name="method"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Size Scale(this Size sourceSize, int width, int height, ScaleMethod method, ScaleType type) {

            if (type == ScaleType.Expand && sourceSize.Height > height && sourceSize.Width > width)
                return sourceSize;

            if (type == ScaleType.Shrink && sourceSize.Height < height && sourceSize.Width < width)
                return sourceSize;

            if (height == 0)
                height = sourceSize.Height;

            if (width == 0)
                width = sourceSize.Width;

            // calculate the two possible scaling factors
            var wAr = (float)width / sourceSize.Width;
            var hAr = (float)height / sourceSize.Height;

            // pick the scaling factor based on the scale method
            var ar = method == ScaleMethod.ToFit ? Math.Min(wAr, hAr) : Math.Max(wAr, hAr);

            return sourceSize.Scale(ar);
        }

        /// <summary>
        /// Scales an image <see cref="Size"/>, but only up
        /// </summary>
        /// <param name="sourceSize"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static Size Expand(this Size sourceSize, int width, int height, ScaleMethod method) {
            return sourceSize.Scale(width, height, method, ScaleType.Expand);
        }

        /// <summary>
        /// Scales an image <see cref="Size"/>, but only down
        /// </summary>
        /// <param name="sourceSize"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static Size Shrink(this Size sourceSize, int width, int height, ScaleMethod method) {
            return sourceSize.Scale(width, height, method, ScaleType.Shrink);
        }

        /// <summary>
        /// Returns a Size that is a sub-section of the given source size and conforms to the specified aspect
        /// </summary>
        /// <param name="sourceSize"></param>
        /// <param name="aspect"></param>
        /// <example>If you pass in a 1024x576 Size with a requested aspect of 4/3, you'll get 768x576</example>
        /// <returns></returns>
        public static Size CropToAspect(this Size sourceSize, float aspect) {

            int width, height;
            var currentAspect = (float)sourceSize.Width / sourceSize.Height;

            if (Math.Abs(aspect - currentAspect) < float.Epsilon)
                return sourceSize;

            if (aspect > currentAspect) {
                width = sourceSize.Width;
                height = (int)(sourceSize.Width / aspect);
            }
            else {
                width = (int)(sourceSize.Height * aspect);
                height = sourceSize.Height;
            }

            return new Size(width, height);
        }

    }
}
