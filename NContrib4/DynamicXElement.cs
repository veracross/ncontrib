using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NContrib.Extensions;

namespace NContrib4 {

    public class DynamicXElement : DynamicObject {

        protected XElement ActualElement;

        private string _indexAttribute = "id";
        public string IndexAttribute {
            get { return _indexAttribute; }
            set { _indexAttribute = value; }
        }

        public DynamicXElement() {
            ActualElement = null;
        }

        public DynamicXElement(XElement actualElement) {
            ActualElement = actualElement;
        }

        public DynamicXElement(XmlNode node) {
            ActualElement = ToXElement(node);
        }

        public DynamicXElement(string xml) {
            ActualElement = XDocument.Parse(xml).Root;
        }

        /// <summary>
        /// Handler for fetching indexes. This is implemented to fetch an element whose attribute
        /// with name <see cref="IndexAttribute"/> matches this index value.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="indexes"></param>
        /// <param name="result"></param>
        /// <returns>String</returns>
        /// <example>A list of elements called "items" with each "item" attribute having an attribute "id"
        /// could be fetched like: Items["email"] to fetch an item with @id="email"</example>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result) {

            if (ActualElement == null) {
                result = null;
                return true;
            }

            var requestedName = indexes[0] as string;

            var element = ActualElement
                .Elements()
                .Where(n => n.HasAttributes)
                .Where(n => n.Attribute(IndexAttribute) != null)
                .Where(n => NameMatch(requestedName, n.Attribute(IndexAttribute).Value))
                .ToArray();

            if (element.Length == 0) {
                result = new DynamicXElement();
                return true;
            }

            if (element.Length == 1) {
                result = new DynamicXElement(element.First());
                return true;
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Handler for fetching dynamic members. In this case, fetching elements.
        /// No matter what, a <see cref="DynamicXElement"/> is returned.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns>DynamicXElement</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result) {

            if (ActualElement == null) {
                result = new DynamicXElement();
                return true;
            }

            var elements = GetElements(ActualElement, binder.Name).ToArray();
            var elementCount = elements.Count();

            if (elementCount == 0) {
                result = new DynamicXElement();
                return true;
            }
            
            if (elementCount == 1) {
                var e = elements.First();
                
                result = e.HasElements || e.HasAttributes
                    ? new DynamicXElement(e)
                    : (object)e.Value;

                return true;
            }

            result = elements.Select(e => new DynamicXElement(e));
            return true;
        }

        public IEnumerable<dynamic> List(string path) {

            return GetElements(ActualElement, path).Select(e => new DynamicXElement(e));
        }

        /// <summary>Converts the value of the requested element to the requested type T</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="fallback"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public T ValueOf<T>(string name, T fallback, CultureInfo cultureInfo) {
            if (ActualElement == null) return fallback;
            var elements = GetElements(ActualElement, name).ToArray();
            return elements.Count() == 0 ? fallback : elements.First().Value.ConvertTo<T>(cultureInfo);
        }

        /// <summary>Converts the value of the requested element to the requested type T using current culture info</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public T ValueOf<T>(string name, T fallback) {
            return ValueOf(name, fallback, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converts the value of the requested element to the requested type T using the currnent culture info
        /// and the default value of type T as a fallback.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T ValueOf<T>(string name) {
            return ValueOf(name, default(T), CultureInfo.CurrentCulture);
        }

        /// <summary>Fetches elements with the given name using case-insensitive matching</summary>
        /// <param name="doc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static IEnumerable<XElement> GetElements(XContainer doc, string name) {
            return doc.Elements().Where(e => NameMatch(name, e.Name.LocalName));
        }

        /// <summary>Converts a classic XmlNode to an XElement</summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static XElement ToXElement(XmlNode node) {
            var doc = new XDocument();
            using (var writer = doc.CreateWriter())
                node.WriteTo(writer);
            return doc.Root;
        }

        public static bool NameMatch(string propertyName, string xmlName) {
            return string.Compare(propertyName.ToSnakeCase(), xmlName, true) == 0;
        }

        /// <summary>
        /// Handles implicit and explicit conversions of the current type to the requested type.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryConvert(ConvertBinder binder, out object result) {

            if (binder.Type == typeof(String)) {
                result = ActualElement == null ? null : ActualElement.Value;
                return true;
            }

            try {
                result = ActualElement.Value.ConvertTo(binder.Type, CultureInfo.CurrentCulture);
                return true;
            }
            catch (InvalidCastException) {
                result = null;
                return false;
            }
        }

        /// <summary>Return the current element's value when ToString() is requested</summary>
        /// <returns></returns>
        public override string ToString() {

            if (ActualElement == null)
                return string.Empty;

            if (ActualElement.HasAttributes || ActualElement.HasElements)
                return ActualElement.ToString();

            return ActualElement.Value;
        }
    }
}
