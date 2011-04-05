using System.IO;
using System.Xml;

namespace NContrib.Extensions {

    public static class XmlExtensions {

        public static string GetNodeValue(this XmlNode node, string xpath, string fallback = null) {
            var selectedNode = node.SelectSingleNode(xpath);
            return selectedNode == null ? fallback : selectedNode.InnerText;
        }

        public static bool HasNode(this XmlNode node, string xpath) {
            return node.SelectSingleNode(xpath) != null;
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
