using NUnit.Framework;

namespace NContrib.Tests {

    [TestFixture]
    public class RangeTests {

        [Test]
        public void Includes_NumberInInclusiveRange_ReturnsCorrect() {
            var r = new Range<int>(10, 20);
            Assert.IsTrue(r.Includes(10));
            Assert.IsTrue(r.Includes(11));
            Assert.IsTrue(r.Includes(19));
            Assert.IsTrue(r.Includes(20));
            Assert.IsFalse(r.Includes(9));
            Assert.IsFalse(r.Includes(21));
        }

        [Test]
        public void Includes_NumberInNonInclusiveRange_ReturnsCorrect() {
            var r = new Range<int>(10, 20, false, false);
            Assert.IsFalse(r.Includes(10));
            Assert.IsTrue(r.Includes(11));
            Assert.IsTrue(r.Includes(19));
            Assert.IsFalse(r.Includes(20));
            Assert.IsFalse(r.Includes(9));
            Assert.IsFalse(r.Includes(21));
        }

        [Test]
        public void Includes_NumberInMinInclusiveRange_ReturnsCorrect() {
            var r = new Range<int>(10, 20, true, false);
            Assert.IsTrue(r.Includes(10));
            Assert.IsTrue(r.Includes(11));
            Assert.IsTrue(r.Includes(19));
            Assert.IsFalse(r.Includes(20));
            Assert.IsFalse(r.Includes(9));
            Assert.IsFalse(r.Includes(21));
        }

        [Test]
        public void Includes_NumberInMaxInclusiveRange_ReturnsCorrect() {
            var r = new Range<int>(10, 20, false, true);
            Assert.IsFalse(r.Includes(10));
            Assert.IsTrue(r.Includes(11));
            Assert.IsTrue(r.Includes(19));
            Assert.IsTrue(r.Includes(20));
            Assert.IsFalse(r.Includes(9));
            Assert.IsFalse(r.Includes(21));
        }
    }
}
