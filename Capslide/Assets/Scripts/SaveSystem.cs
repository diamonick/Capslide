using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    /// <summary>
    /// Create new Capslide data.
    /// </summary>
    public static void Create(GameManager GM)
    {
        BinaryFormatter BF = new BinaryFormatter();
        string filePath = $"{Application.persistentDataPath}/CapslideData.dat";
        FileStream file = File.Create(filePath);

        CapslideData data = new CapslideData(GM);

        BF.Serialize(file, data);
        file.Close();
        Debug.Log("Data saved!");
    }

    /// <summary>
    /// Save all data for Capslide.
    /// </summary>
    /// <param name="GM">Game Manager</param>
    public static void Save(GameManager GM)
    {
        BinaryFormatter BF = new BinaryFormatter();
        string filePath = $"{Application.persistentDataPath}/CapslideData.dat";
        FileStream file = File.Create(filePath);

        CapslideData data = new CapslideData(GM);

        BF.Serialize(file, data);
        file.Close();
        Debug.Log("Data saved!");
    }

    /// <summary>
    /// Load all data for Capslide.
    /// </summary>
    /// <returns>TRUE if data exists. Otherwise, FALSE.</returns>
    public static CapslideData Load()
    {
        string filePath = $"{Application.persistentDataPath}/CapslideData.dat";
        if (File.Exists(filePath))
        {
            BinaryFormatter BF = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open);
            CapslideData data = (CapslideData)BF.Deserialize(file);
            file.Close();

            //Loaded data
            //diamonds = data.player_Diamonds;
            Debug.Log("Data loaded!");

            return data;
        }
        else
        {
            Debug.LogError("Could not load Capslide data.");
            return null;
        }
    }

    /// <summary>
    /// Erase Capslide data.
    /// </summary>
    public static void Erase()
    {
        string filePath = $"{Application.persistentDataPath}/CapslideData.dat";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Data erased!");
        }
    }
}
