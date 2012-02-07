using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace NContrib.Drawing.Extensions {

    public static class ImageExtensions {

        /// <summary>
        /// Scales an image to within the specified dimensions, disregarding aspect ratio
        /// </summary>
        /// <param name="source"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static Image GetScaledImage(this Image source, int width, int height) {
            return source.GetScaledImage(new Size(width, height));
        }

        /// <summary>
        /// Scales an image to within the specified dimensions, disregarding aspect ratio.
        /// To preserve aspect ratio, generate the size parameter using the Constrain, Scale, and Expand methods
        /// </summary>
        /// <param name="source"></param>
        /// <param name="size">New size to scale to. Consider using the Constrain, Scale, and Expand methods to generate this value</param>
        /// <returns></returns>
        public static Image GetScaledImage(this Image source, Size size) {
            var b = new Bitmap(size.Width, size.Height);

            using (var g = Graphics.FromImage(b)) {
                g.SetGraphicsDefaults();
                g.DrawImage(source, 0, 0, size.Width, size.Height);
            }

            return b;
        }

        /// <summary>
        /// Crops an image to make it square
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Image GetSquareImage(this Image image) {
            int length, x = 0, y = 0;

            if (image.Width > image.Height) {
                length = image.Height;
                x = (image.Width - length) / 2;
            }
            else {
                length = image.Width;
                y = (image.Size.Height - length) / 2;
            }

            var b = new Bitmap(length, length);

            using (var g = Graphics.FromImage(b)) {
                g.SetGraphicsDefaults();
                g.DrawImage(image, new Rectangle(0, 0, length, length), x, y, length, length, GraphicsUnit.Pixel);
            }

            return b;
        }

        private static void SetGraphicsDefaults(this Graphics g) {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
        }

        /// <summary>
        /// Saves an Image as a JPEG image to the given path using a 1 - 100 compression level
        /// </summary>
        /// <param name="img"></param>
        /// <param name="fileName"></param>
        /// <param name="compression"></param>
        public static void SaveJpegWithCompressionSetting(this Image img, string fileName, int compression) {
            var stream = File.Open(fileName, FileMode.Create);
            img.SaveJpegWithCompressionSetting(stream, compression);
            stream.Close();
        }

        /// <summary>
        /// Saves an Image as a JPEG to the given stream using a 1 - 100 compression level
        /// </summary>
        /// <param name="img"></param>
        /// <param name="outputStream"></param>
        /// <param name="compression"></param>
        public static void SaveJpegWithCompressionSetting(this Image img, Stream outputStream, long compression) {
            var eps = new EncoderParameters(1);
            eps.Param[0] = new EncoderParameter(Encoder.Quality, compression);
            var ici = ImageTypeHelper.GetImageEncoderFromMimeType("image/jpeg");
            img.Save(outputStream, ici, eps);
        }
    }
}
