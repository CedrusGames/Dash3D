using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEditor;
using  Radix.Core;

[System.Serializable]

public class SaveLoadSystem
{
    public static SaveLoadSystem instance;

  
    public int LevelIndex = 0;
    public int RandomLevelIndex=0;
    public int CurrentPlayerMoney { get { return _currentPlayerMoney; } set { _currentPlayerMoney = value; UIManager.instance.UpdateMoneyText(); } }
    int _currentPlayerMoney = 0;
    public int PlayerMoney  { get { return _PlayerMoney; } set { _PlayerMoney = value; UIManager.instance.UpdateMoneyText(); } }
     int _PlayerMoney = 0;
     
     public int IngameScore=0;
     public  int TotalScore = 0;
     
    public static void Save()
    {

        string path = Application.persistentDataPath + "/RadixSave.dat";
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path, FileMode.OpenOrCreate);
        bf.Serialize(file, instance);
        file.Close();
        
    }

    public static void Load()
    {
        
        string path = Application.persistentDataPath + "/RadixSave.dat";
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            SaveLoadSystem data = (SaveLoadSystem)bf.Deserialize(file);
            file.Close();
            instance = data;
        }
        else
        {
            instance = new SaveLoadSystem();
            Save();
        }
    }
#if UNITY_EDITOR

    [MenuItem("Radix/Clear Save File")]
    private static void ClearSave()
    {
        string path = Application.persistentDataPath + "/RadixSave.dat";
        if (File.Exists(path))
        {
            File.Delete(path);

        }
    }
     
#endif   
    
}