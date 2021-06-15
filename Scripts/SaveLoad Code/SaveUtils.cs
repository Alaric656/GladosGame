using UnityEngine;
using System;
using System.IO;

public static class SaveUtils  {

    public static void SaveLevel(Level level){
        string json = JsonUtility.ToJson(level);
        //Debug.Log(json);
        string folder = Application.dataPath + "/LevelData/";
        string levelFile = level.GetName() + ".json";
        if(!Directory.Exists(folder)){
            Directory.CreateDirectory(folder);
        }
        string path = Path.Combine(folder, levelFile);
        if(File.Exists(path)){
            File.Delete(path);
        }
        File.WriteAllText(path,json);
    }
}
