using System.Globalization;
using System.IO;
using System.Xml;

namespace NContrib.Extensions {

    public static class XmlExtensions {

        /// <summary>
        /// Gets a string value from the given <paramref name="xpath"/>
        /// </summary>
        /// <param name="node"></param>
        /// <param name="xpath"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public static string GetNodeValue(this XmlNode node, string xpath = null, string fallback = null) {
            var selectedNode = xpath.IsBlank() ? node : node.SelectSingleNode(xpath);
            return selectedNode == null ? fallback : selectedNode.InnerText;
        }

        /// <summary>
        /// Gets a value from the given <paramref name="xpath"/> and converts it to type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="xpath"></param>
        /// <param name="fallback"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static T GetNodeValue<T>(this XmlNode node, string xpath = null, T fallback = default(T), CultureInfo cultureInfo = null) {
            var value = node.GetNodeValue(xpath);
            return value == null ? fallback : value.ConvertTo<T>(cultureInfo);
        }

        /// <summary>Indicates if any nodes match the given xpath</summary>
        /// <param name="node"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static bool NodesExist(this XmlNode node, string xpath) {
            var n = node.SelectNodes(xpath);
            return n != null && n.Count > 0;
        }

        /// <summary>
        /// Create XmlReader
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static XmlReader ToXmlReader(this XmlNode node) {
            var sr = new StringReader(node.WriteToString());
            return new XmlTextReader(sr);
        }

        /// <summary>
        /// Returns an XML node as a string with specified formatting
        /// </summary>
        /// <param name="node">XmlNode</param>
        /// <param name="formatting">Either Indented or None</param>
        /// <returns>XML node as a string</returns>
        public static string WriteToString(this XmlNode node, Formatting formatting = Formatting.Indented) {
            using (var sw = new StringWriter())
            using (var xw = new XmlTextWriter(sw) { Formatting = formatting }) {
                node.WriteTo(xw);
                return sw.ToString();
            }
        }
    }
}
