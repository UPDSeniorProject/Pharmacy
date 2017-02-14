using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Text;

[XmlRoot("OutputXML")]
public class OutputClass
{
    [XmlArray("Tasks")]
    [XmlArrayItem("Task")]
    public List<Task> tasks = new List<Task>();

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(OutputClass));
        using (var stream = new FileStream(path, FileMode.Create))
        {
           // var xmlWriter = new XmlTextWriter(stream, Encoding.UTF8);
            //serializer.Serialize(xmlWriter, this);
            serializer.Serialize(stream, this);
        }
    }

    public static OutputClass Load(string path)
    {
        var serializer = new XmlSerializer(typeof(OutputClass));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as OutputClass;
        }
    }

    //Loads the xml directly from the given string. Useful in combination with www.text.
    public static OutputClass LoadFromText(string text)
    {
        var serializer = new XmlSerializer(typeof(OutputClass));
        return serializer.Deserialize(new StringReader(text)) as OutputClass;
    }
}
