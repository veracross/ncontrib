using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace NContrib.Tests {

    [TestFixture]
    public class MimeHelperTests {

        [Test]
        public void GetMimeFromFileName_ValidMixedCaseExtensions_Found() {
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", MimeHelper.GetMimeFromFileName("doc1.xlsx"));
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", MimeHelper.GetMimeFromFileName("MYDOC.XLSX"));
            Assert.AreEqual("image/jpeg", MimeHelper.GetMimeFromFileName("pic.jpg"));
            Assert.AreEqual("image/jpeg", MimeHelper.GetMimeFromFileName("PIC.JPG"));
            Assert.AreEqual("image/jpeg", MimeHelper.GetMimeFromFileName("pic.Jpg"));
        }

        [Test]
        public void GetMimeFromFileName_NoExtensions_ReturnsDefault() {
            Assert.AreEqual(MimeHelper.DefaultMimeType, MimeHelper.GetMimeFromFileName(string.Empty));
            Assert.AreEqual(MimeHelper.DefaultMimeType, MimeHelper.GetMimeFromFileName("data"));
        }

        [Test]
        public void GetMimeFromFileName_NewUserTypes_Found() {
            var random = "." + Guid.NewGuid().ToString("N");
            const string mime = "application/x-ncontrib-testing";
            var tempPath = Path.GetTempFileName() + random;

            MimeHelper.UserTypes.Add(random, mime);
            Assert.AreEqual(mime, MimeHelper.GetMimeFromFileName(tempPath));
        }

        [Test]
        public void GetMimeFromFileName_OverrideUserType_ReturnsOverrideValue() {
            const string mime = "application/x-ncontrib-testing";

            MimeHelper.UserTypes.Add(".bmp", mime);
            Assert.AreEqual(mime, MimeHelper.GetMimeFromFileName("image.bmp"));
        }

        [Test]
        public void GetMime_JpegFile_ReturnsCorrectMime() {
            TestPhysicalFile("fodder/kitten.jpg", "image/jpeg");
        }

        [Test]
        public void GetMime_PngFile_ReturnsCorrectMime() {
            TestPhysicalFile("fodder/HyperbolicParaboloid.png", "image/png");
        }

        private static void TestPhysicalFile(string filePath, string mime) {
            var path = Path.GetFullPath(filePath);
            Trace.WriteLine("Sample file: " + path);

            Assert.AreEqual(mime, MimeHelper.GetMimeFromBytes(path));

            var fi = new FileInfo(path);
            Assert.AreEqual(mime, fi.GetMimeFromBytes());
            Assert.AreEqual(mime, fi.GetMimeFromFileName());

            using (var fs = File.OpenRead(path))
                Assert.AreEqual(mime, fs.GetMimeFromBytes());

            using (var fs = File.OpenRead(path)) {
                var buffer = new byte[256];
                fs.Read(buffer, 0, buffer.Length);
                Assert.AreEqual(mime, buffer.GetMimeFromBytes());
            }
        }
    }
}
