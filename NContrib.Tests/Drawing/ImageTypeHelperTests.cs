using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NContrib.Drawing;
using NUnit.Framework;

namespace NContrib.Tests.Drawing {

    [TestFixture]
    public class ImageTypeHelperTests {

        [Test]
        public void GetImageEncoderFromMimeType_StandardSupportedTypes_ReturnsEncoder() {
            
            Assert.AreEqual("BMP", ImageTypeHelper.GetImageEncoderFromMimeType("image/bmp").FormatDescription);
            Assert.AreEqual("JPEG", ImageTypeHelper.GetImageEncoderFromMimeType("image/jpeg").FormatDescription);
            Assert.AreEqual("GIF", ImageTypeHelper.GetImageEncoderFromMimeType("image/gif").FormatDescription);
            Assert.AreEqual("TIFF", ImageTypeHelper.GetImageEncoderFromMimeType("image/tiff").FormatDescription);
            Assert.AreEqual("PNG", ImageTypeHelper.GetImageEncoderFromMimeType("image/png").FormatDescription);
        }
    }
}
