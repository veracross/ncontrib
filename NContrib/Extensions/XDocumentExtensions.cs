using System;
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

        /// <summary>
        /// As long as it's legal, will merge XML trees
        /// </summary>
        /// <param name="root"></param>
        /// <param name="other"></param>
        public static void MergeTree(this XContainer root, XElement other) {
            if (!root.Descendants().Any(x => x.Name == other.Name))
                root.Add(other);
            else {
                var e = root.Descendants().Single(x => x.Name == other.Name);
                other.Descendants().Action(d => e.MergeTree(d));
            }
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
