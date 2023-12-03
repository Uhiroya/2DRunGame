using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveDataInterface 
{
    static readonly string DataPath = Application.dataPath + "/StreamingAssets/";
    
    public static bool LoadJson<T>(out T data) where T : ISaveData ,new()
    {
        data = new T();
        if(File.Exists(DataPath + data.FileName))
        {
            using(StreamReader reader = new StreamReader(DataPath + data.FileName))
            { 
                string jsonData =  reader.ReadToEnd(); 
                data = JsonUtility.FromJson<T>(jsonData);
                return true;
            }
        }
        else
        {
            return false;
        }
    }
    public static void SaveJson<T>(T data) where T : ISaveData
    {
        string jsonData = JsonUtility.ToJson(data);
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
    public string FileName => this.GetType().Name + ".json";
    float _highScore;
    public float HighScore => _highScore;
    public void SaveScore(float score)
    {
        if(score > _highScore)
        {
            _highScore = score;
        }
    }
}
