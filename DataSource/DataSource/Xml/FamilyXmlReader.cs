using DataSource.Model.FileSystem;
using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace DataSource.Xml
{
    internal class FamilyXmlReader
    {
        private const string XmlDataStart = "<entry";
        private const string XmlDataEnd = "/entry>";

        internal RevitFamilyFile RevitFile { get; set; }

        private XDocument Document;

        public XElement Root { get { return Document.Root; } }

        public FamilyXmlReader(RevitFamilyFile revitFile)
        {
            if (revitFile is null) { throw new ArgumentNullException(nameof(revitFile)); }
            if (revitFile.Exists() == false) { throw new ArgumentException("File does not exist: " + revitFile); }

            RevitFile = revitFile;
        }

        public void ReadData()
        {
            var xmlData = GetFamilyXmlData();
            try
            {
                Document = XDocument.Parse(xmlData);
            }
            catch (Exception exp)
            {
                throw new FamilyXmlReadException(ExceptionStatus.NotValidData, exp);
            }
        }


        /// <summary>
        /// Faster ExtractPartAtom reimplementation,
        /// independent of Revit API, for standalone 
        /// external use. By Håvard Dagsvik, Symetri.
        /// </summary>
        /// <param name="file">Family file path</param>
        /// <returns>XML data</returns>
        private string GetFamilyXmlData()
        {
            byte[] fileContentArray;
            try
            {
                fileContentArray = File.ReadAllBytes(RevitFile.FullPath);
            }
            catch (Exception exp)
            {
                //status = MetaDataStatus.Error;
                throw new FamilyXmlReadException(ExceptionStatus.Unkown, "Could not read file", exp);
            }

            var fileContent = Encoding.UTF8.GetString(fileContentArray);
            var start = fileContent.IndexOf(XmlDataStart, StringComparison.CurrentCulture);
            string xml_data;
            if (start == -1)
            {
                //status = MetaDataStatus.Error;
                throw new FamilyXmlReadException(ExceptionStatus.NoStart);
            }
            else
            {
                var end = fileContent.IndexOf(XmlDataEnd, StringComparison.CurrentCulture);
                if (end == -1)
                {
                    //status = MetaDataStatus.Repairable;
                    throw new FamilyXmlReadException(ExceptionStatus.NoEnd);
                }
                else
                {
                    end += 7;
                    var length = end - start;
                    if (length <= 0)
                    {
                        //status = MetaDataStatus.Repairable;
                        throw new FamilyXmlReadException(ExceptionStatus.NotValidData);
                    }
                    else
                    {
                        xml_data = fileContent.Substring(start, length);
                    }
                }
            }
            return xml_data;
        }

    }
}
