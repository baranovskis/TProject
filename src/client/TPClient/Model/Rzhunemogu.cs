using System.Xml.Serialization;

namespace TPClient.Model
{
    [XmlRoot("root")]
    public class Rzhunemogu
    {
        [XmlElement("content")]
        public string Content { get; set; }
    }
}
