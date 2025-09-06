using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    public static void SaveCharacter(CharacterStats data)
    {
        string json = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/playerData/" + data.characterName + ".json", json);
    }

    public static CharacterStats LoadCharacter(string characterName)
    {
        string path = Application.persistentDataPath + "/playerData/" + characterName + ".json";

        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            CharacterStats loadedData = JsonUtility.FromJson<CharacterStats>(json);
            return loadedData;
        }
        else
        {
            Debug.LogWarning("Save file not found: " + path);
            return null;
        }
    }

    public static List<string> GetAllCharacterNames()
    {
        string folderPath = Application.persistentDataPath + "/playerData/";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            return new List<string>();
        }

        string[] files = Directory.GetFiles(folderPath, "*.json");

        List<string> characterNames = new List<string>();
        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            characterNames.Add(fileName);
        }

        return characterNames;
    }
}
 