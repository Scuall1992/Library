using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;


public class GameManager : BaseBoot
{
    public static GameManager Instance;
    [SerializeField] public DataObject Data;
    [SerializeField] public PlayerDataObject PlayerData;
    
    private string FileName = "ProjectData";
    private string statFileName = "PlayerData";
    private string logFilePath = "log.txt";
    private string dataPath = "";
    private string statisticPath = "";

    public async override UniTask Boot()
    {
        await base.Boot();
        await Configure();
    }

    private async UniTask Configure()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        
        dataPath = $"{Application.persistentDataPath}/{FileName}.json";
        statisticPath = $"{Application.persistentDataPath}/{statFileName}.json";
        logFilePath = Path.Combine(Application.persistentDataPath, logFilePath);
            
        await LoadGameDataFromJson();
        
        await LoadPlayerDataFromJson();
    }


    private async UniTask LoadGameDataFromJson()
    {
        DataObject _rawObj = new DataObject();
        
        if (File.Exists(dataPath))
        {
            string raw = await File.ReadAllTextAsync(dataPath);
            _rawObj = JsonConvert.DeserializeObject<DataObject>(raw);
        }
        else
        {
            await SaveGameDataToJson();
        }

        Data = _rawObj;

    }

    public async UniTask SaveGameDataToJson()
    {
        await File.WriteAllTextAsync(dataPath, JsonConvert.SerializeObject(Data));
    }

    private async UniTask LoadPlayerDataFromJson()
    {
        if (File.Exists(statisticPath))
        {
            string raw = await File.ReadAllTextAsync(statisticPath);
            PlayerData = JsonConvert.DeserializeObject<PlayerDataObject>(raw);
            
        }
        else
        {
            PlayerData = new PlayerDataObject();
            SavePlayerDataToJson().Forget();
        }        
    }

    private async UniTask SavePlayerDataToJson()
    {
        var json = JsonConvert.SerializeObject(PlayerData, Formatting.Indented);
        await File.WriteAllTextAsync(statisticPath, json);
    }

    public void AddRecord(PlayerRecord newRecord)
    {
        if (PlayerData.records != null)
            PlayerData.records.Add(newRecord);
        else
        {
            PlayerData.records = new List<PlayerRecord> { newRecord };
        }
        SavePlayerDataToJson().Forget();
    }
    
    public void LogMessage(string message)
    {
        try
        {
            using (StreamWriter writer = File.AppendText(logFilePath))
            {
                writer.WriteLine($"{DateTime.Now} : {message}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error writing to log file: " + e.Message);
        }
    }
}