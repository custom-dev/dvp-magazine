using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

using ICSharpCode.SharpZipLib.Zip;

namespace Developpez.MagazineTool
{
    public class GeneratorEPub
    {
        private Generator _generator;
        private MagazineType _magazine;
        private string _outputZipFileName;

        private GeneratorEPub(Generator generator, MagazineType magazine)
        {
            string epubFileName = String.Format("mag{0}.epub", magazine.Numero);

            _outputZipFileName = Path.Combine(generator.RootDirectory.FullName, epubFileName);
            _generator = generator;
            _magazine = magazine;
        }

        public static GeneratorEPub Create(Generator generator, MagazineType magazine)
        {
            return new GeneratorEPub(generator, magazine);
        }

        public bool DeleteIfExists()
        {
            if (File.Exists(_outputZipFileName))
            {
                File.Delete(_outputZipFileName);
            }
            return true;
        }

        public bool Generate()
        {
            DeleteIfExists();
            //ZipFile epubFile = ZipFile.Create(_outputZipFileName);
            using (FileStream fileStream = File.Create(_outputZipFileName))
            using (ZipOutputStream epubFile = new ZipOutputStream(fileStream))
            {
                bool ok = true;

                ok &= AddMimetype(epubFile);
                ok &= AddContainer(epubFile);
                ok &= AddContent(epubFile);
                ok &= AddCouverture(epubFile);
                return ok;
            }
        }

        private static bool AddMimetype(ZipOutputStream zip)
        {
            ZipEntry entry = new ZipEntry("mimetype");
            entry.CompressionMethod = CompressionMethod.Stored;
            
            zip.PutNextEntry(entry);

            using(StreamWriter writer = new StreamWriter(zip, Encoding.UTF8, 512, true))
            {
                writer.Write("application/epub+zip");
            }
            
            return true;
        }

        private static bool AddContainer(ZipOutputStream zip)
        {
            ZipEntry entry = new ZipEntry("META-INF/container.xml");
            zip.PutNextEntry(entry);

            using (StreamWriter writer = new StreamWriter(zip, Encoding.UTF8, 512, true))
            {
                string content = ReadFromManifest("Developpez.MagazineTool.Templates.epub.META_INF.container.xml");
                writer.Write(content);
            }

            return true;
        }

        private static string ReadFromManifest(string manifest)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(manifest))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private bool AddContent(ZipOutputStream zip)
        {
            ZipEntry entry = new ZipEntry("OEBPS/content.opf");
            zip.PutNextEntry(entry);

            using (StreamWriter writer = new StreamWriter(zip, Encoding.UTF8, 512, true))
            {
                string content = ReadFromManifest("Developpez.MagazineTool.Templates.epub.OEBPS.content.xslt");
                string xmlMagazine = _magazine.Serialize();
                using (StringReader magStream = new StringReader(xmlMagazine))
                using (TextReader textReader = new StringReader(content))
                using (XmlReader xmlReader = XmlReader.Create(textReader))
                {
                    XPathDocument myXPathDoc = new XPathDocument(magStream);
                    XslCompiledTransform myXslTrans = new XslCompiledTransform();
                    myXslTrans.Load(xmlReader);
                    myXslTrans.Transform(myXPathDoc, null, writer);
                }
            }

            return true;
        }

        private bool AddCouverture(ZipOutputStream zip)
        {
            ZipEntry entry = new ZipEntry("OEBPS/couverture.xml");
            zip.PutNextEntry(entry);

            using (StreamWriter writer = new StreamWriter(zip, Encoding.UTF8, 512, true))
            {
                string content = ReadFromManifest("Developpez.MagazineTool.Templates.epub.OEBPS.couverture.xslt");
                string xmlMagazine = _magazine.Serialize();
                using (StringReader magStream = new StringReader(xmlMagazine))
                using (TextReader textReader = new StringReader(content))
                using (XmlReader xmlReader = XmlReader.Create(textReader))
                {
                    XPathDocument myXPathDoc = new XPathDocument(magStream);
                    XslCompiledTransform myXslTrans = new XslCompiledTransform();
                    myXslTrans.Load(xmlReader);
                    myXslTrans.Transform(myXPathDoc, null, writer);
                }
            }

            return true;
        }
    }
}
