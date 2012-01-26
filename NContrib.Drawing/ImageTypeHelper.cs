using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NContrib.Drawing {

    public static class ImageTypeHelper {

        /// <summary>
        /// Gets the appropriate <see cref="ImageCodecInfo"/> based on a mime type
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        public static ImageCodecInfo GetImageEncoderFromMimeType(string mimeType) {

            if (!Regex.IsMatch(mimeType, RegexLibrary.MimeType))
                throw new ArgumentException(mimeType + " does not appear to be a valid MIME type", mimeType);

            mimeType = mimeType.ToLower();

            // create a mapping of irregular mime types to what is standard and used in ImageCodecs
            var irregular = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "image/pjpeg", "image/jpeg" },
                { "image/jpg",   "image/jpeg" },
                { "image/jpe",   "image/jpeg" },
                { "image/x-png", "image/png" },
            };

            irregular.TryGetValue(mimeType, out mimeType);

            var encoders = ImageCodecInfo.GetImageEncoders().Where(ic => ic.MimeType == mimeType).ToArray();

            if (encoders.Length == 0)
                return null;

            if (encoders.Length > 1)
                throw new InvalidOperationException("Found more than one encoder for " + mimeType);

            return encoders[0];
        }

        /// <summary>
        /// Get the appropriate <see cref="ImageFormat"/> for the specified mimeType using 
        /// <see cref="GetImageEncoderFromMimeType"/> as a helper.
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public static ImageFormat GetImageFormatFromMimeType(string mimeType) {
            return new ImageFormat(GetImageEncoderFromMimeType(mimeType).FormatID);
        }

        /// <summary>
        /// Detects if the image in the given stream is
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsImageType(Stream stream, ImageFormat type) {
            
            if (type == ImageFormat.Png)
                return IsPng(stream);
            if (type == ImageFormat.Bmp)
                return IsBmp(stream);
            if (type == ImageFormat.Gif)
                return IsGif(stream);
            if (type == ImageFormat.Jpeg)
                return IsJpeg(stream);

            throw new NotSupportedException("Detection for image type '" + type + "' is not supported");
            
        }

        /// <summary>
        /// Looks for the PNG 8-byte leading signature at the current position in the stream.
        /// Rewinds the stream to its original position when finished.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static bool IsPng(Stream stream) {
            var br = new BinaryReader(stream);
            var signature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            var firstEight = br.ReadBytes(signature.Length);

            if (stream.CanSeek)
                stream.Seek(-8, SeekOrigin.Current);

            return signature.SequenceEqual(firstEight);
        }

        /// <summary>
        /// Looks for the BMP 2-byte leading signature at the current position in the stream.
        /// Rewinds the stream to its original position when finished.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static bool IsBmp(Stream stream) {
            var magic = new[] { stream.ReadByte(), stream.ReadByte() };

            if (stream.CanSeek)
                stream.Seek(-2, SeekOrigin.Current);

            return magic.SequenceEqual(new[] { 0x42, 0x4D });
        }

        /// <summary>
        /// Looks for the GIF 6-byte leading signature and version the current position in the stream.
        /// Rewinds the stream to its original position when finished.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static bool IsGif(Stream stream) {
            var br = new BinaryReader(stream);
            var magic1 = new byte[] { 0x47, 0x49, 0x46 };
            var signature = br.ReadBytes(3);
            var version = br.ReadChars(3);

            var versionCheck = char.IsNumber(version[0]) && char.IsNumber(version[1]) && char.IsLetter(version[2]);

            if (br.BaseStream.CanSeek)
                br.BaseStream.Seek(-6, SeekOrigin.Current);

            return magic1.SequenceEqual(signature) && versionCheck;
        }

        /// <summary>
        /// Looks for the JPEG 2-bytes leading signature at the current position in the stream
        /// Rewinds the stream to its original position when finished.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static bool IsJpeg(Stream stream) {
            var header = new[] { stream.ReadByte(), stream.ReadByte() };
            stream.Seek(-2, SeekOrigin.Current);

            return header.SequenceEqual(new[] { 0xFF, 0xD8 });
        }
    }
}
