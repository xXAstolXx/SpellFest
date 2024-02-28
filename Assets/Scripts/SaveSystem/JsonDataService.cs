using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class JsonDataService
{
    public static bool SaveData<T>(string relativePath, T Data, bool doEncrypt)
    {
        string path = Path.Combine(Application.persistentDataPath, relativePath);

        try 
        {
            if (File.Exists(path))
            {
                Debug.Log("Data exists. Deleting old file and writing a new one!");
                File.Delete(path);
            }

            using FileStream stream = File.Create(path);
            stream.Close();
            File.WriteAllText(path, JsonConvert.SerializeObject(Data));
            return true;
        } 
        catch (Exception e)
        {
            Debug.LogError("Unable to save data due to: " + e.Message + e.StackTrace);
            return false;
        }
    }

    public static T LoadData<T>(string relativePath, bool doEncrypt)
    {
        string path = Path.Combine(Application.persistentDataPath, relativePath);

        if (File.Exists(path) == false)
        {
            Debug.LogError($"Cannot load file at {path}. File does not exist!");
            throw new FileNotFoundException($"{path} does not exist!");
        }

        try
        {
            T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            return data;
        }
        catch(Exception e)
        {
            Debug.LogError($"Failed to load dat6a due to: {e.Message} {e.StackTrace}");
            throw e;
        }
    }
}
