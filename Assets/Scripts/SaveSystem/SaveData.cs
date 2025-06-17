using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int clearedStageNumber = 0;
    public List<int> encountEnemies = new();

    public static SaveData Instance;

    private const string SaveKey = "SaveData";

    public void StageClear(int stageNumber)
    {
        clearedStageNumber = Mathf.Max(clearedStageNumber, stageNumber);

        Save();
    }

    public void OnEncountEnemy(EnemyType type)
    {
        if (!encountEnemies.Contains((int)type)) encountEnemies.Add((int)type);

        Save();
    }

    public bool HasEncountered(EnemyType type)
    {
        return encountEnemies.Contains((int)type);
    }

    public static void Save()
    {
        string json = JsonUtility.ToJson(Instance);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public static void Load()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            Instance = JsonUtility.FromJson<SaveData>(json);
            Save();
        }
        else
        {
            Instance = new SaveData();
        }
    }

    public static void Reset()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        Load();
    }
}
