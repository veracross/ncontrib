using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using NContrib.Extensions;
using NUnit.Framework;

namespace NContrib.Tests {

    [TestFixture]
    public class XmlExtensionsTests {

        [Test]
        public void GetNodeValue_Xpath_ReturnsValue() {
            const string xml = @"<root><objects><object id=""1""><name>hello</name></object></objects></root>";
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            Trace.WriteLine(doc.WriteToString());
            
            Assert.AreEqual("hello", doc.GetNodeValue("/root/objects/object[@id=1]/name", "fallback"));
        }
    }
}
