using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace SolStandard.Utility.Parsing
{
    public class ObjectTypesXmlParser
    {
        public static Dictionary<string, Dictionary<string, string>> ParseObjectTypesXml(string filePath)
        {
            XmlReader xmlReader = XmlReader.Create(filePath);

            while (xmlReader.NodeType != XmlNodeType.Element)
            {
                xmlReader.Read();
            }

            XElement objectTypes = XElement.Load(xmlReader);

            Dictionary<string, Dictionary<string, string>> objectTypesInFile =
                new Dictionary<string, Dictionary<string, string>>();

            foreach (XElement objectType in objectTypes.Elements())
            {
                Dictionary<string, string> objectTypeProperties = new Dictionary<string, string>();

                foreach (XElement property in objectType.Elements())
                {
                    string key = property.Attribute("name").Value;

                    string value = (property.Attribute("default") != null) ? property.Attribute("default").Value : "";

                    objectTypeProperties.Add(key, value);
                }

                string objectTypeName = objectType.Attribute("name").Value;
                objectTypesInFile.Add(objectTypeName, objectTypeProperties);
            }

            return objectTypesInFile;
        }
    }
}