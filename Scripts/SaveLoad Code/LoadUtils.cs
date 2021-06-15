using UnityEngine;
using System;
using System.IO;

public static class LoadUtils  {
    
    public static Level LoadLevel(string levelName){
        string folder = Application.dataPath + "/LevelData/";
        string levelFile = levelName + ".json";
        string path = Path.Combine(folder, levelFile);
        if(File.Exists(path)){
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<Level>(json);
        }else{
            return new Level("Something Went Wrong");
        }
    }


}
