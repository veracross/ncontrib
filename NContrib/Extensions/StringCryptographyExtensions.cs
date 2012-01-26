using System.Security.Cryptography;
using System.Text;

namespace NContrib.Extensions {
    
    public static class StringCryptographyExtensions {

        /// <summary>Creates an all lower-case hex MD5 hash of the input string using the default system encoding</summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MD5(this string input) {
            return input.MD5(Encoding.Default);
        }

        /// <summary>Creates an all lower-case hex MD5 hash of the input string using the specified encoding</summary>
        /// <param name="input"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static string MD5(this string input, Encoding enc) {
            var data = enc.GetBytes(input);

            using (var md5 = System.Security.Cryptography.MD5.Create()) {
                return md5.ComputeHash(data).ToHex().ToLower();
            }
        }

        /// <summary>
        /// Creates a lower-case SHA1 hash of the input string using the default system encoding
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SHA1(this string data) {
            return data.SHA1(Encoding.Default);
        }

        /// <summary>
        /// Creates a lower-case SHA1 hash of the input string using the specified encoding
        /// </summary>
        /// <param name="input"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static string SHA1(this string input, Encoding enc) {
            var data = enc.GetBytes(input);
            var sha = new SHA1CryptoServiceProvider();
            return sha.ComputeHash(data).ToHex().ToLower();
        }

        /// <summary>
        /// Creates an HMAC-MD5 fingerprint of the given data with the given key using default encoding
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string HMACMD5(this string data, string key) {
            return data.HMACMD5(key, Encoding.Default);
        }

        /// <summary>
        /// Creates an HMAC-MD5 fingerprint of the given data with the given key using the specified encoding
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static string HMACMD5(this string data, string key, Encoding enc) {
            var hmacKey = enc.GetBytes(key);
            var hmacData = enc.GetBytes(data);

            using (var hmacMd5 = new HMACMD5(hmacKey)) {
                return hmacMd5.ComputeHash(hmacData).ToHex().ToLower();
            }
        }
    }
}
