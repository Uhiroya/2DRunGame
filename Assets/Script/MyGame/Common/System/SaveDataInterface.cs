using System;
using System.IO;
using UnityEngine;

public static class SaveDataInterface
{
    private static readonly string DataPath = Application.dataPath + "/StreamingAssets/";

    public static bool LoadJson<T>(out T data) where T : ISaveData, new()
    {
        data = new T();
        if (File.Exists(DataPath + data.FileName))
            using (var reader = new StreamReader(DataPath + data.FileName))
            {
                var jsonData = reader.ReadToEnd();
                data = JsonUtility.FromJson<T>(jsonData);
                return true;
            }

        return false;
    }

    public static void SaveJson<T>(T data) where T : ISaveData
    {
        var jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(DataPath + data.FileName, jsonData);
    }
}

public interface ISaveData
{
    string FileName { get; }
}

[Serializable]
public class SaveData : ISaveData
{
    private static string _fileName;
    public float _highScore;
    public float HighScore => _highScore;
    public string FileName => _fileName ??= GetType().FullName + ".json";

    public void SaveScore(float score)
    {
        if (score > _highScore) _highScore = score;
    }
}
