using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using NContrib.Extensions;

namespace NContrib.Drawing.Extensions {

    public static class StreamExtensions {

        /// <summary>
        /// Tries to use the GNV fast image size checkers for JPEG, GIF, PNG, and BMP
        /// Uses .NET newing-up an Image object as a fallback
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        public static Size GetImageSize(this Stream s) {

            var mime = s.GetMime();

            if (mime.IsEmpty() || mime == "application/octet-stream")
                throw new InvalidDataException("Invalid or undetectable image format");

            var imageFormat = ImageTypeHelper.GetImageFormatFromMimeType(mime);

            if (imageFormat == null)
                throw new InvalidDataException("Could not detect image format from mime type: " + mime);

            if (imageFormat.Guid == ImageFormat.Jpeg.Guid)
                return s.GetJpegImageSize();

            if (imageFormat.Guid == ImageFormat.Gif.Guid)
                return s.GetGifImageSize();

            if (imageFormat.Guid == ImageFormat.Png.Guid)
                return s.GetPngImageSize();

            if (imageFormat.Guid == ImageFormat.Bmp.Guid)
                return s.GetBmpImageSize();

            using (var img = Image.FromStream(s)) {
                return img.Size;
            }
        }

        /// <summary>
        /// Reads the size of a PNG image from this stream at the current offset
        /// Rewinds the stream to where it started when finished
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Size GetPngImageSize(this Stream stream) {

            if (!ImageTypeHelper.IsPng(stream))
                throw new InvalidDataException("This is not a PNG stream");

            var br = new BinaryReader(stream);
            var position = br.BaseStream.Position;

            // jump over the 8 byte signature, the 4 byte chunk length indicator, and the 4 byte IHDR marker
            br.BaseStream.Seek(16, SeekOrigin.Begin);

            var size = new Size((int)br.ReadUInt32BE(), (int)br.ReadUInt32BE());

            if (br.BaseStream.CanSeek)
                br.BaseStream.Seek(position, SeekOrigin.Begin);

            return size;
        }

        /// <summary>
        /// Reads the size of a BMP image from this stream at the current offset
        /// Rewinds the stream to where it started when finished
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Size GetBmpImageSize(this Stream stream) {

            if (!ImageTypeHelper.IsBmp(stream))
                throw new InvalidDataException("This is not a BMP stream");

            var br = new BinaryReader(stream);

            // jump over:
            // 2: the magic number
            // 4: size of the file
            // 2: reserved
            // 2: reserved
            // 4: start of bmp data offset
            // 4: header size
            br.BaseStream.Seek(18, SeekOrigin.Begin);

            var size = new Size(br.ReadInt32(), br.ReadInt32());

            if (br.BaseStream.CanSeek)
                br.BaseStream.Seek(-26, SeekOrigin.Current);

            return size;
        }

        /// <summary>
        /// Reads the size of a GIF image from this stream at the current offset
        /// Rewinds the stream to where it started when finished
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Size GetGifImageSize(this Stream stream) {

            if (!ImageTypeHelper.IsGif(stream))
                throw new InvalidDataException("This is not a GIF stream");

            var br = new BinaryReader(stream);

            // jump over:
            // 3: GIF
            // 3: version (87a, 89a, etc)
            br.BaseStream.Seek(6, SeekOrigin.Current);

            var size = new Size(br.ReadInt16(), br.ReadInt16());

            if (br.BaseStream.CanSeek)
                br.BaseStream.Seek(-10, SeekOrigin.Current);

            return size;
        }

        /// <summary>
        /// Reads a JPEG image's dimensions and returns a <see cref="Size"/> object. Much faster
        /// than using the built in <see cref="Image"/> class which loads the entire image into memory
        /// Rewinds the stream after reading.
        /// </summary>
        /// <param name="stream"></param>
        /// <exception cref="InvalidDataException">Thrown when the Stream is not a JPEG image</exception>
        /// <returns></returns>
        public static Size GetJpegImageSize(this Stream stream) {

            if (!ImageTypeHelper.IsJpeg(stream))
                throw new InvalidDataException("This is not a JPEG stream");

            try {
                var br = new BinaryReader(stream);

                // skip the 0xFFD8 marker
                br.BaseStream.Seek(2, SeekOrigin.Current);

                // keep reading packets until we find one that contains Size info
                while (true) {
                    ushort b1 = 0x00;

                    while (b1 != 0xFF)
                        b1 = br.ReadByte();

                    while (b1 == 0xFF)
                        b1 = br.ReadByte();

                    if (b1 >= 0xC0 && b1 <= 0xC3) {
                        // this is the SOF (Start Of Frame) marker. skip 3 bytes
                        br.BaseStream.Seek(3, SeekOrigin.Current);

                        var h = br.ReadUInt16BE();
                        var w = br.ReadUInt16BE();

                        return new Size(w, h);
                    }
                    else {
                        // this isn't the SOF marker, skip to the next marker
                        br.BaseStream.Seek(br.ReadUInt16BE() - 2, SeekOrigin.Current);
                    }
                }
            }
            catch (EndOfStreamException ex) {
                throw new Exception("Hit the end of the stream without finding the dimensions. This file may be corrupt.", ex);
            }
            finally {
                if (stream != null && stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);
            }
        }
    }
}
