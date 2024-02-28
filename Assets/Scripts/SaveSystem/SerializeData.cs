using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Object = System.Object;

public class SerializeData
{
    public static void Save(Object objectToSerialize, string fileName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(fileName, FileMode.OpenOrCreate);
        formatter.Serialize(stream, objectToSerialize);
        Debug.Log("can write: "+stream.CanWrite);
        stream.Dispose();
    }

    public static Object Load(string fileName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(fileName, FileMode.Open);
        Object deserializedObject = formatter.Deserialize(stream);
        Debug.Log("can read: " + stream.CanRead);

        stream.Dispose();

        return deserializedObject;
    }
}
