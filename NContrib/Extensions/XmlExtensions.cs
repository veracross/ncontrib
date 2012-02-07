using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Xsl;

namespace NContrib.Extensions {

    public static class XmlExtensions {

        /// <summary>
        /// Easily adds an attribute with a value to a node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attributeName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static XmlAttribute AddAttribute(this XmlNode node, string attributeName, object value) {
            var a = node.GetRootDocument().CreateAttribute(attributeName);
            a.Value = value == null ? string.Empty : value.ToString();
            node.Attributes.Append(a);
            return a;
        }

        /// <summary>
        /// Adds a child to any XmlNode object
        /// </summary>
        /// <param name="node">XmlNode to add to</param>
        /// <param name="xpath">Name of the element to create</param>
        /// <param name="value">Value to add to the element</param>
        /// <returns>Newly create XmlNode with the added value</returns>
        public static XmlNode AddChild(this XmlNode node, string xpath, object value = null) {

            var e = node.GetRootDocument().CreateElement(xpath);

            if (value != null)
                e.InnerText = value.ToString();

            node.AppendChild(e);

            return e;
        }

        /// <summary>
        /// Adds a new node called and automatically adds an ID attribute with IDs starting at 1
        /// </summary>
        /// <param name="node"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static XmlNode AddChildWithAutoIdNode(this XmlNode node, string xpath) {
            var id = 1;

            if (node.ChildExists(xpath))
                id = node.SelectNodes(xpath).Count + 1;

            return AddChildWithIdNode(node, xpath, id);
        }

        /// <summary>
        /// Creates a child node and adds a node called id with the specified id
        /// </summary>
        /// <param name="node"></param>
        /// <param name="xpath"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static XmlNode AddChildWithIdNode(this XmlNode node, string xpath, int id) {
            var newNode = node.AddChild(xpath);
            newNode.AddChild("id", id);

            return newNode;
        }

        /// <summary>
        /// Tells whether or not a child element with a certain name exits as
        /// a direct descendant of this XmlNode
        /// </summary>
        /// <param name="node">XmlNode to search</param>
        /// <param name="xpath">Element name to look for</param>
        /// <returns></returns>
        public static bool ChildExists(this XmlNode node, string xpath) {
            if (string.IsNullOrEmpty(xpath))
                return false;

            return node.SelectSingleNode(xpath) != null;
        }

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

        /// <summary>
        /// Returns the highest level XmlDocument object for this node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static XmlDocument GetRootDocument(this XmlNode node) {
            return node.OwnerDocument ?? (XmlDocument)node;
        }

        /// <summary>Indicates if any nodes match the given xpath</summary>
        /// <param name="node"></param>
        /// <param name="xpath"></param>
        /// <param name="namespaceManager"></param>
        /// <returns></returns>
        public static bool NodesExist(this XmlNode node, string xpath, XmlNamespaceManager namespaceManager = null) {
            if (node == null)
                return false;

            var nodes = node.SelectNodes(xpath, namespaceManager);
            return nodes != null && nodes.Count > 0;
        }

        /// <summary>
        /// Removes this single node
        /// </summary>
        /// <param name="node"></param>
        public static void Remove(this XmlNode node) {
            node.ParentNode.RemoveChild(node);
        }

        /// <summary>
        /// Removes all nodes in this XmlNodeList
        /// </summary>
        /// <param name="nodes"></param>
        public static void Remove(this XmlNodeList nodes) {
            nodes.Cast<XmlNode>().ToList().ForEach(n => n.Remove());
        }

        /// <summary>
        /// Removes the node/nodes specified by the xpath
        /// Will not throw an exception if no node/nodes are found
        /// </summary>
        /// <param name="node"></param>
        /// <param name="xpath"></param>
        public static void Remove(this XmlNode node, string xpath) {
            node.SelectNodes(xpath).Remove();
        }

        /// <summary>
        /// Sets a node's InnerText property to the data parameter encoded with Base64
        /// </summary>
        /// <param name="n"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static void SetInnerBase64(this XmlNode n, byte[] bytes) {
            n.InnerText = Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Transforms an XmlNode with the specified XSL string.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="xsl"></param>
        /// <param name="formatting"></param>
        /// <returns></returns>
        public static string TransformWithXsl(this XmlNode node, string xsl, Formatting formatting = Formatting.Indented) {

            using (var sw = new StringWriter())
            using (var xw = new XmlTextWriter(sw) {Formatting = formatting})
            using (var sr = new StringReader(xsl))
            using (var xtr = new XmlTextReader(sr)) {
                var transformer = new XslCompiledTransform();
                transformer.Load(xtr);
                transformer.Transform(node, xw);

                return sw.ToString();
            }
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
