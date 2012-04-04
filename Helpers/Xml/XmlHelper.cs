using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

namespace Helpers.Xml
{
    public class XmlHelper
    {
        public static T FromString<T>(string text)
        {
            using (StringReader stringReader = new StringReader(text))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(stringReader);
            }
        }
        public static T FromFile<T>(string path)
        {
            using (StreamReader file = File.OpenText(path))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(file);
            }
        }
        public static void ToFile(string path, object o)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(o.GetType());
            using (StreamWriter file = File.CreateText(path))
            {
                xmlSerializer.Serialize(file, o);
            }
        }
        public static string ToString(object o)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(o.GetType());
            using (StringWriter stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, o);

                stringWriter.Flush();
                return stringWriter.ToString();
            }
        }
    }
}
