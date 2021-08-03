using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("WorldItems")]
public class Objects
{
    [XmlArray("MapObjects")]
    public List<SceneObjs> mapObjs = new List<SceneObjs>();

}



//https://answers.unity.com/questions/1714688/i-can-load-xml-data-but-i-cant-to-save-it-here-the.html - TY @Priyanka-Rajwanshi
public class ObjectsClass
{
    public static Objects Load(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Objects));
        using (var stream = new FileStream(path, FileMode.Open))
        {
           
            return serializer.Deserialize(stream) as Objects;
            
        }
    }
    public static void Save(Objects container, string path)
    {
        var serializer = new XmlSerializer(typeof(Objects));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, container);
        }
    }
}
