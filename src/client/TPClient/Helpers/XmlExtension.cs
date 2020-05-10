using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace TPClient.Helpers
{
    public static class XmlExtension
    {
        public static string Serialize<T>(this T value) where T : class
        {
            if (value == null)
            {
                return String.Empty;
            }

            var serializer = new XmlSerializer(typeof(T));

            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
                {
                    serializer.Serialize(xmlWriter, value);
                    return stringWriter.ToString();
                }
            }
        }

        public static T Deserialize<T>(this string xmlValue) where T : class
        {
            T result;

            var serializer = new XmlSerializer(typeof(T));

            using (TextReader reader = new StringReader(xmlValue))
            {
                var deserializedItem = serializer.Deserialize(reader);
                result = deserializedItem as T;
            }

            return result;
        }

        public static T Clone<T>(this T value) where T : class
        {
            var result = Serialize(value);
            return Deserialize<T>(result);
        }
    }
}
