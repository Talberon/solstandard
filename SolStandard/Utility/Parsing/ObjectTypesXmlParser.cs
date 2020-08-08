using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace SolStandard.Utility.Parsing
{
    public static class ObjectTypesXmlParser
    {
        public static Dictionary<string, Dictionary<string, string>> ParseObjectTypesXml(string filePath)
        {
            var xmlReader = XmlReader.Create(filePath);

            while (xmlReader.NodeType != XmlNodeType.Element)
            {
                xmlReader.Read();
            }

            XElement objectTypes = XElement.Load(xmlReader);

            var objectTypesInFile =
                new Dictionary<string, Dictionary<string, string>>();

            foreach (XElement objectType in objectTypes.Elements())
            {
                var objectTypeProperties = new Dictionary<string, string>();

                foreach (XElement property in objectType.Elements())
                {
                    // ReSharper disable once PossibleNullReferenceException
                    string key = property.Attribute("name").Value;

                    // ReSharper disable once PossibleNullReferenceException
                    string value = (property.Attribute("default") != null) ? property.Attribute("default").Value : "";

                    objectTypeProperties.Add(key, value);
                }

                // ReSharper disable once PossibleNullReferenceException
                string objectTypeName = objectType.Attribute("name").Value;
                objectTypesInFile.Add(objectTypeName, objectTypeProperties);
            }

            return objectTypesInFile;
        }
    }
}