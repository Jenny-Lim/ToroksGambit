using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private string saveGameFilePath;
    private string saveVolumePath;

    public bool hasSaveGame;
    public float savedVolume;


    private void Awake()
    {

        //Debug.Log(Application.persistentDataPath);
        saveGameFilePath = Application.persistentDataPath + "/SaveData.dat";
        saveVolumePath = Application.persistentDataPath + "/VolumeData.dat";
        instance = this;
        LoadVolume();

        if(File.Exists(saveGameFilePath))
        {
            //Debug.Log("FILE");
            hasSaveGame = true;
        }
        else
        {
            //Debug.Log("NO FILE");
        }

    }

    public void StartNew()//new game button
    {
        if(File.Exists(saveGameFilePath))
        {
            File.Delete(saveGameFilePath);
        }
    }

    public void LoadSave()//continue game button
    {
        if(File.Exists(saveGameFilePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(saveGameFilePath, FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();

            GameStateManager.instance.currentLevelNumber = data.savedLevelNum;
            Currency.instance.tickets = data.savedCurrency;
            Inventory.instance.SetInventoryCount(data.savedPieces);

        }
    }

    public void LoadVolume()//continue game button
    {
        if(File.Exists(saveVolumePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(saveVolumePath, FileMode.Open);
            SaveVolume data = (SaveVolume)bf.Deserialize(file);
            file.Close();

            savedVolume = data.volumeLevel;

        }
        else
        {
            savedVolume = 0;
        }
    }

    public void SaveVolume(float newVolume)
    {
        savedVolume = newVolume;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(saveVolumePath);

        SaveVolume data = new SaveVolume();
        data.volumeLevel = savedVolume;
        bf.Serialize(file, data);
        file.Close();

        Debug.Log("SAVE Volume");
    }

    public void SaveGame()//after every board
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(saveGameFilePath);

        SaveData data = new SaveData();
        data.savedLevelNum = GameStateManager.instance.currentLevelNumber;
        data.savedPieces = Inventory.instance.GetInventoryCount();
        data.savedCurrency = Currency.instance.tickets;
        bf.Serialize(file, data);
        file.Close();

        Debug.Log("SAVE SUCCESFUL");

    }
}

[Serializable]
class SaveData
{
    public int savedLevelNum {get; set;}
    public int[] savedPieces {get; set;}
    public int savedCurrency {get; set;}

}
[Serializable]
class SaveVolume
{
    public float volumeLevel {get; set;}
}

