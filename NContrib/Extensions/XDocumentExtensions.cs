using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace NContrib.Extensions {

    public static class XDocumentExtensions {

        /// <summary>
        /// Creates an XDocument
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static XDocument ToXDocument(this XmlDocument doc) {
            return XDocument.Load(doc.ToXmlReader());
        }

        public static string GetValue(this XElement node, string name, string fallback = null) {
            var e = node.Elements().SingleOrDefault(el => el.Name.Namespace == node.Name.Namespace && el.Name.LocalName == name);
            return e == null ? fallback : e.Value;
        }

        public static void AddFragment(this XElement target, XElement other) {
            
            if (!target.Elements().Any(e => e.DescribeSelector() == other.DescribeSelector())) {
                target.Add(other);
            }
            else {
                target.Elements().Single(e => e.DescribeSelector() == other.DescribeSelector()).AddFragment(other.Elements().First());

            }
        }

        public static string DescribePath(this XElement e) {
            var up = e.Ancestors().Reverse().Select(a => a.DescribeSelector()).Join("/");
            var me = (up.IsBlank() ? "" : up + "/") + e.DescribeSelector();
            return me;
        }

        public static string DescribeSelector(this XElement e) {
            var me = e.Name.ToString();
            if (e.HasAttributes)
                me += "[" + e.Attributes().Describe() + "]";
            return me;
        }

        public static string Describe(this IEnumerable<XAttribute> attributes) {
            return attributes.Select(a => string.Format("@{0}=\"{1}\"", a.Name, a.Value)).Join(",");
        }

        /// <summary>
        /// Returns an XML node as a string with specified formatting
        /// </summary>
        /// <param name="doc">XmlNode</param>
        /// <param name="settingsOverrides">Settings overrides</param>
        /// <returns>XML node as a string</returns>
        public static string WriteToString(this XDocument doc, object settingsOverrides) {

            var ms = new MemoryStream();

            var settings = new XmlWriterSettings {
                OmitXmlDeclaration = false,
                Indent = true,
                Encoding = Encoding.GetEncoding(doc.Declaration.Encoding),
            };

            if (settingsOverrides != null)
                settings.CopyPropertiesFrom(settingsOverrides);

            using (var xw = XmlWriter.Create(ms, settings)) {
                doc.Save(xw);
                xw.Flush();

                var sr = new StreamReader(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Returns an XmlNode as a string with indented formatting
        /// </summary>
        /// <param name="node">XmlNode</param>
        /// <returns>XML document as a string with indented formatting</returns>
        public static string WriteToString(this XDocument node) {
            return node.WriteToString(null);
        }
    }
}
