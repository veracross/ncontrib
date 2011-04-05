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
        /// Handler for fetching indexes. XML attributes are fetched from here
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="indexes"></param>
        /// <param name="result"></param>
        /// <returns>String</returns>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result) {

            if (ActualElement == null) {
                result = null;
                return true;
            }

            var attributeName = indexes[0] as string;
            var attribute = ActualElement.Attributes().FirstOrDefault(e => string.Compare(e.Name.LocalName, attributeName, true) == 0);

            if (attribute != null) {
                result = attribute.Value;
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

            var elements = GetElements(ActualElement, binder.Name);
            var elementCount = elements.Count();

            if (elementCount == 0) {
                result = new DynamicXElement();
                return true;
            }

            if (elementCount == 1) {
                result = new DynamicXElement(elements.First());
                return true;
            }

            result = elements.Select(e => new DynamicXElement(e));
            return true;
        }

        /// <summary>Converts the value of the requested element to the requested type T</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="fallback"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public T ValueOf<T>(string name, T fallback, CultureInfo cultureInfo) {
            if (ActualElement == null) return fallback;
            var elements = GetElements(ActualElement, name);
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

        /// <summary>
        /// Converts the value of the current element to the type T with a fallback and specified culture info
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fallback"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public T As<T>(T fallback, CultureInfo cultureInfo) {
            return ActualElement == null ? fallback : ActualElement.Value.ConvertTo<T>(cultureInfo);
        }

        /// <summary>
        /// Converts the value of the current element to the type T with a fallback and current culture info
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public T As<T>(T fallback) {
            return As(fallback, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converst the value of the current element to type T using T's default as a fallback and current culture info
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T As<T>() {
            return As(default(T));
        }

        /// <summary>Fetches elements with the given name using case-insensitive matching</summary>
        /// <param name="doc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static IEnumerable<XElement> GetElements(XContainer doc, string name) {
            return doc.Elements().Where(e => string.Compare(e.Name.LocalName, name, true) == 0);
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
            return ActualElement == null ? base.ToString() : ActualElement.Value;
        }
    }
}
