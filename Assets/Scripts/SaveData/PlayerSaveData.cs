using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    // internal use
    public const int CURRENT_SAVE_VERSION = 0;
    private string filePath; public void SetFilePath(string filePath) { this.filePath = filePath; }
    private bool isDirty = false;


    // Save Data
    [SerializeField] string playerSaveId; 
    [SerializeField] int saveVersion; // Initialized in constructor: increment when notable changes are made. 
    [SerializeField] List<LevelSaveData> bestLevelData; // Save to json. List of best level records
    [SerializeField] List<LevelCompletionRecord> completionRecordData; // Save to json. List of level 

    [System.NonSerialized] // Convenience to use at runtime. Converted from bestLevelData
    Dictionary<string, LevelSaveData> LevelSaveData_Dictionary; 

    public PlayerSaveData() // constructor
    {
        playerSaveId = Guid.NewGuid().ToString();
        saveVersion = CURRENT_SAVE_VERSION; // increment when notable changes are made after release
        bestLevelData = new List<LevelSaveData>();
        completionRecordData = new List<LevelCompletionRecord>();
        LevelSaveData_Dictionary = new Dictionary<string, LevelSaveData>();
    } // constructor

    public void InitLevelDataDictionary()
    {
        foreach (LevelSaveData level in bestLevelData)
        {
            LevelSaveData_Dictionary.Add(level.name, level);
        }
        string debug_string = LevelSaveData_Dictionary.ToString();
        Debug.Log(debug_string);

        foreach (var kvp in LevelSaveData_Dictionary)
        {
            Debug.Log($"{kvp.Key} : {kvp.Value}");
        }
    }

    public void RecordLevelResult(LevelCompletionRecord levelCompletionRecord)
    {
        isDirty = true;
        // Add level to dictionary. If failed, then don't save to best list. 
        this.completionRecordData.Add(levelCompletionRecord);
        if (!levelCompletionRecord.completed) return; // No need to add to LevelSaveData_List

        // Compare previous and new LevelSaveData
        LevelSaveData newLevelSaveData = levelCompletionRecord.ConvertToLevelSaveData();
        //if (LevelSaveData_Dictionary.TryGetValue(levelCompletionRecord.levelName, out LevelSaveData oldLevelSaveData))
        if (GetBestLevel(levelCompletionRecord.levelName, out LevelSaveData oldLevelSaveData))
        {
            if (newLevelSaveData.score > oldLevelSaveData.score)
            {
                // New high score! 
                //LevelSaveData_Dictionary.Add(newLevelSaveData.name, newLevelSaveData);
                LevelSaveData_Dictionary[levelCompletionRecord.levelName] = newLevelSaveData;
            }
        }
        else
        {
            // First time completion!
            LevelSaveData_Dictionary.Add(newLevelSaveData.name, newLevelSaveData);
        }

        UpdateBestLevelData();
    }

    public void UpdateBestLevelData()
    {

        bestLevelData.Clear();
        //foreach (LevelSaveData levelSaveData in LevelSaveData_Dictionary)
        foreach (KeyValuePair<string, LevelSaveData> pair in LevelSaveData_Dictionary)
        {
            bestLevelData.Add(pair.Value);
        }
    }

    public bool GetBestLevel(string levelName, out LevelSaveData levelSaveData)
    {
        return LevelSaveData_Dictionary.TryGetValue(levelName, out levelSaveData);
    }
    public bool IsLevelCompleted(string levelName)
    {
        return LevelSaveData_Dictionary.ContainsKey(levelName);
    }
    public int GetTotalScore()
    {
        int totalScore = 0;
        foreach (LevelSaveData levelSaveData in bestLevelData)
        {
            totalScore += levelSaveData.score;
        }
        return totalScore;
    }

    public struct PrereqStats
    {
        public bool scoreSufficient;
        public bool[] levelSufficient;
    }
    public bool IsLevelLocked(Level level, out PrereqStats stats)
    {
        stats = new PrereqStats();
        // Fail is totalScore is too low. 
        stats.scoreSufficient = GetTotalScore() >= level.prereqs.minScore;


        // Fail if any level isn't present in completed levels
        bool anyLevelsInsufficient = false;
        stats.levelSufficient = new bool[level.prereqs.requiredLevels.Length];
        var reqLvl = level.prereqs.requiredLevels;
        for (var i = 0; i < reqLvl.Length; i++)  
        {
            stats.levelSufficient[i] = IsLevelCompleted(reqLvl[i].LevelName);
            if (!stats.levelSufficient[i]) anyLevelsInsufficient = true;
        }

        // Return true if all prerequisities are met!
        return !stats.scoreSufficient || anyLevelsInsufficient;
    }

    // Discard out argument
    public bool IsLevelLocked(Level level) { return IsLevelLocked(level, out _); }


    //====================================================================
    #region Saving and Loading
    public void SaveToJson()
    {
        // Dirty Flag Check. Don't save if changes haven't been made. 
        if (!isDirty) return;

        // Grab PlayerSaveData from GameManager and put it into json to place into a save file. 
        string json = JsonUtility.ToJson(this, true);
        File.WriteAllText(this.filePath, json);
        Debug.Log(json);
        Debug.Log("Saved to: " + this.filePath);

        isDirty = false;
    }

    public static PlayerSaveData LoadFromJson(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Debug.Log(json);

            // Code to upgrade save file 
            SaveHeader header = JsonUtility.FromJson<SaveHeader>(json);
            if (header.saveVersion < CURRENT_SAVE_VERSION)
            {
                json = UpgradeSave(json, header.saveVersion);
            }
            // Save file done upgrading

            PlayerSaveData loaded = JsonUtility.FromJson<PlayerSaveData>(json);
            Debug.Log("PlayerID: " + loaded.playerSaveId.ToString() + "\n" +
                "SaveVersion: " + loaded.saveVersion.ToString());
            loaded.InitLevelDataDictionary();
            loaded.SetFilePath(filePath);

            
            if (loaded.completionRecordData.Count > 0)
            {
                int count = loaded.completionRecordData.Count;
                int nextId = loaded.completionRecordData[count - 1].id;
                LevelCompletionRecord.SetNextId(nextId + 1);
            }

            return loaded;
        }
        else
        {
            Debug.LogWarning("No save file found, returning new save.");
            PlayerSaveData playeSaveData = new PlayerSaveData();
            playeSaveData.SetFilePath(filePath);
            return playeSaveData;
        }
    }

    public static string UpgradeSave(string json, int saveVersion)
    {
        /* ADD CODE WHEN NECESSARY FOR SAVE UPGRADE
        while (saveVersion < CURRENT_SAVE_VERSION)
        {
            switch (saveVersion)
            {
                case 0:
                    json = UpgradeV0ToV1(json);
                    break;
                case 1:
                    json = UpgradeV1ToV2(json);
                    break;
                case 2:
                    json = UpgradeV2ToV3(json);
                    break; 
            }

            saveVersion++;
        }*/

        return json;
    }
    public static string UpgradeV0ToV1(string json)
    {
        SaveV0 old = JsonUtility.FromJson<SaveV0>(json);

        SaveV1 upgraded = new SaveV1
        {
            saveVersion = 1

            // Place old variables into new variables. 
            //health = old.health,
            
            // Create new defaults. 
            //maxHealth = 100
        };

        return JsonUtility.ToJson(upgraded);
    }
    #endregion
}

[System.Serializable] 
public class SaveHeader
{
    public int saveVersion = 0;
}

[System.Serializable] 
public class SaveV0
{
    public int saveVersion = 0;

    /*
    Add the appropriate fields for this versions save file. 
    */
}

[System.Serializable] 
public class SaveV1
{
    public int saveVersion = 0;

    /*
    Add the appropriate fields for this versions save file. 
    */
}

