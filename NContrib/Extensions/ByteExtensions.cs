using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;

namespace NContrib.Extensions {

    public static class ByteExtensions {

        public const int MimeSampleSize = 256;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pBC">A pointer to the IBindCtx interface. Can be set to NULL.</param>
        /// <param name="pwzUrl">A pointer to a string value that contains the URL of the data. Can be set to NULL if pBuffer contains the data to be sniffed.</param>
        /// <param name="pBuffer">A pointer to the buffer that contains the data to be sniffed. Can be set to NULL if pwzUrl contains a valid URL. </param>
        /// <param name="cbSize">An unsigned long integer value that contains the size of the buffer. </param>
        /// <param name="pwzMimeProposed">A pointer to a string value that contains the proposed MIME type. This value is authoritative if nothing else can be determined about type. Can be set to NULL.</param>
        /// <param name="dwMimeFlags"></param>
        /// <param name="ppwzMimeOut">The address of a string value that receives the suggested MIME type. </param>
        /// <param name="dwReserverd">Reserved. Must be set to 0.</param>
        /// <returns></returns>
        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static uint FindMimeFromData(
            uint pBC,
            [MarshalAs(UnmanagedType.LPStr)] string pwzUrl,
            [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
            uint cbSize,
            [MarshalAs(UnmanagedType.LPStr)] string pwzMimeProposed,
            uint dwMimeFlags,
            out uint ppwzMimeOut,
            uint dwReserverd
        );

        /// <summary>
        /// Use MIME magic to find the MIME type of this data block.
        /// This should be the first 256 bytes of a file
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetMime(this byte[] data) {
            try {
                uint mimeType;
                FindMimeFromData(0, null, data, MimeSampleSize, null, 0, out mimeType, 0);

                var mimePointer = new IntPtr(mimeType);
                var mime = Marshal.PtrToStringUni(mimePointer);
                Marshal.FreeCoTaskMem(mimePointer);

                return mime;
            }
            catch {
                return "application/octet-stream";
            }
        }

        /// <summary>
        /// Decompress a string using GZip and the specific encoding. Defaults to UTF-8
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encodingName"> </param>
        /// <returns></returns>
        public static string GZipDecompress(this byte[] data, string encodingName = "utf-8")
        {
            var enc = Encoding.GetEncoding(encodingName);
            
            using (var ms = new MemoryStream())
            using (var gz = new GZipStream(ms, CompressionMode.Decompress))
            {
                ms.Write(data, 0, data.Length);
                ms.Position = 0;
                var buffer = gz.ReadAllBytes();
                return enc.GetString(buffer);
            }
        }

        /// <summary>
        /// Returns a byte array as a hexadecimal string. No byte separators (such as spaces or dashes)
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHex(this byte[] bytes) {
            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}
