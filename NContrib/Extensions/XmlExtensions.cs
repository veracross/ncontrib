using System.Globalization;
using System.IO;
using System.Xml;

namespace NContrib.Extensions {

    public static class XmlExtensions {

        public static string GetAttributeValue(this XmlNode node, string attributeName) {
            return node.GetAttributeValue(attributeName, (string)null);
        }

        public static T GetAttributeValue<T>(this XmlNode node, string attributeName, T fallback = default(T), CultureInfo cultureInfo = null) {

            if (node == null) return fallback;
            if (node.Attributes == null) return fallback;
            if (node.Attributes.Count == 0) return fallback;

            var attr = node.Attributes[attributeName];

            if (attr == null) return fallback;

            var value = attr.InnerText;

            if (value.IsBlank()) return fallback;
            
            return value.ConvertTo<T>(cultureInfo);
        }

        /// <summary>
        /// Gets a string value from the given <paramref name="xpath"/>
        /// </summary>
        /// <param name="node"></param>
        /// <param name="xpath"></param>
        /// <param name="namespaceManager"></param>
        /// <returns></returns>
        public static string GetNodeValue(this XmlNode node, string xpath = null, XmlNamespaceManager namespaceManager = null) {
            return node.GetNodeValue(xpath, (string)null, namespaceManager: namespaceManager);
        }

        /// <summary>
        /// Gets a value from the given <paramref name="xpath"/> and converts it to type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="xpath"></param>
        /// <param name="fallback"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="namespaceManager"></param>
        /// <returns></returns>
        public static T GetNodeValue<T>(this XmlNode node, string xpath = null, T fallback = default(T), CultureInfo cultureInfo = null, XmlNamespaceManager namespaceManager = null) {
            var selectedNode = xpath.IsBlank() ? node : node.SelectSingleNode(xpath, namespaceManager);
            if (selectedNode == null) return fallback;

            var value = selectedNode.InnerText;
            
            return value.IsBlank() ? fallback : value.ConvertTo<T>(cultureInfo);
        }

        /// <summary>Indicates if any nodes match the given xpath</summary>
        /// <param name="node"></param>
        /// <param name="xpath"></param>
        /// <param name="namespaceManager"></param>
        /// <returns></returns>
        public static bool NodesExist(this XmlNode node, string xpath, XmlNamespaceManager namespaceManager = null) {
            if (node.SelectSingleNode(xpath, namespaceManager) == null) return false;

            var nodes = node.SelectNodes(xpath, namespaceManager);
            return nodes != null && nodes.Count >= 0;
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
