using System.Web.Script.Serialization;

namespace NContrib {

    /// <summary>
    /// Serializes and deserializes objects to/from JSON
    /// </summary>
    public static class Json {

        /// <summary>Serializes a <see cref="System.Object"/> to JSON</summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns></returns>
        public static string ToJson(this object obj) {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        /// <summary>Serialize a <see cref="System.Object"/> to JSON with the specificied recursion depth limit</summary>
        /// <param name="obj">Object to serialize</param>
        /// <param name="recursionDepth"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, int recursionDepth) {
            var serializer = new JavaScriptSerializer {
                RecursionLimit = recursionDepth,
            };
            return serializer.Serialize(obj);
        }

        /// <summary>
        /// Deserializes JSON to <see cref="System.Object"/> of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T Parse<T>(string s) {
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(s);
        }

        /// <summary>Deserializes JSON to <see cref="System.Object"/></summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static object Parse(string s) {
            var serializer = new JavaScriptSerializer();
            return serializer.DeserializeObject(s);
        }
    }
}
