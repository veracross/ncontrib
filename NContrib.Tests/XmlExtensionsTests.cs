using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using NContrib.Extensions;
using NUnit.Framework;

namespace NContrib.Tests {

    [TestFixture]
    public class XmlExtensionsTests {

        private XmlDocument _books;

        [TestFixtureSetUp]
        public void Setup() {
            _books = new XmlDocument();
            _books.Load(Path.GetFullPath("fodder/books.xml"));
        }

        [Test]
        public void GetAttributeValue_Exiss_ReturnsTextValue() {
            Assert.AreEqual("bk101", _books.SelectSingleNode("/catalog/book[@id='bk101']").GetAttributeValue("id"));
        }

        [Test]
        public void GetNodeValue_Xpath_ReturnsValue() {
            Assert.AreEqual("Computer", _books.GetNodeValue("/catalog/book[@id='bk101']/genre", "fallback"));
            Assert.AreEqual(new DateTime(2000, 10, 01), _books.GetNodeValue<DateTime>("/catalog/book[@id='bk101']/publish_date"));
            Assert.AreEqual(44.95m, _books.GetNodeValue<decimal>("/catalog/book[@id='bk101']/price", cultureInfo: CultureInfo.InvariantCulture ));
        }

        [Test]
        public void GetNodeValue_PreviouslySelectedNodeAndNoXpath_ReturnsValue() {
            var node = _books.SelectSingleNode("/catalog/book[@id='bk101']/price");
            Assert.AreEqual(44.95m, node.GetNodeValue<decimal>(null, cultureInfo: CultureInfo.InvariantCulture));
        }

        [Test]
        public void Exists_Xpath_ReturnsCorrect() {
            Assert.IsTrue(_books.NodesExist("/catalog/book"));
            Assert.IsTrue(_books.NodesExist("/catalog/book[@id]"));
            Assert.IsTrue(_books.NodesExist("/catalog/book[@id='bk101']"));
            Assert.IsTrue(_books.NodesExist("/catalog/book[@id='bk101']/genre"));
            Assert.IsFalse(_books.NodesExist("/catalog/" + Guid.NewGuid()));
        }
    }
}
