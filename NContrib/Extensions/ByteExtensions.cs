using System;

namespace NContrib.Extensions {

    public static class ByteExtensions {

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
