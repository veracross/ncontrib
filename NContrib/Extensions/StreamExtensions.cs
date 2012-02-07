using System;
using System.IO;

namespace NContrib.Extensions {
    
    public static class StreamExtensions {

        /// <summary>
        /// Use MIME magic to find the MIME type for this stream.
        /// </summary>
        /// <remarks>
        /// 256 bytes (or the total number of bytes in the stream if 256 are not available)
        /// are read from the current stream location to detect the MIME type.
        /// If the stream support seeking, the stream position will be moved back after
        /// the sample is taken.
        /// </remarks>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string GetMime(this Stream stream) {

            var buffer = new byte[ByteExtensions.MimeSampleSize];

            var readLength = stream.Length >= buffer.Length ? buffer.Length: (int)stream.Length;

            stream.Read(buffer, 0, readLength);

            if (stream.CanSeek)
                stream.Seek(readLength * -1, SeekOrigin.Current);

            return buffer.GetMime();
        }

        /// <summary>
        /// Reads the entire contents of a stream into a byte array
        /// </summary>
        /// <seealso>http://www.yoda.arachsys.com/csharp/readbinary.html</seealso>
        /// <param name="stream"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public static byte[] ReadAllBytes(this Stream stream, int bufferSize = 32768) {

            if (bufferSize < 1)
                bufferSize = 32768;

            var buffer = new byte[bufferSize];
            var read = 0;
            int chunk;

            while ( (chunk = stream.Read(buffer, read, buffer.Length - read)) > 0) {

                read += chunk;

                // if we read in the entire buffer length, see if there's anything left to read
                if (read != buffer.Length)
                    continue;

                var nextByte = stream.ReadByte();

                // end of stream. done.
                if (nextByte == -1)
                    return buffer;

                // not done. resize the buffer, add the byte we read, carry on
                var newBuffer = new byte[buffer.Length * 2];
                Array.Copy(buffer, newBuffer, buffer.Length);
                newBuffer[read] = (byte) nextByte;
                buffer = newBuffer;
                read++;
            }

            var ret = new byte[read];
            Array.Copy(buffer, ret, read);
            return ret;
        }
    }
}
