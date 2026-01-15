using NUnit.Framework;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Linq;
using System;

public class StorageManagerScript : MonoBehaviour
{
    [SerializeField] Settings defaultSettings;
    [SerializeField] public List<BoardColors> boardColorsList;
    [SerializeField] public  List<UiColors> uiColorsList;

    private PastGames pastGames = new PastGames();
    private Settings settings = new Settings();

    private string pastGamesFile;
    private string settingsFile;

    private void Awake()
    {
        pastGamesFile = Path.Combine(Application.persistentDataPath, "PastGames.json");
        settingsFile = Path.Combine(Application.persistentDataPath, "Settings.json");

        bool settingsIssue = false;
        
        if (!CanLoadJson(LoadSettings)) {
            settingsIssue = true;
        }
        else {
            var s = LoadSettings();

            bool allNonNullRefs =
                s.GetType()
                 .GetFields(System.Reflection.BindingFlags.Public)
                 .Where(f => !f.FieldType.IsValueType)   
                 .All(f => f.GetValue(s) != null);
            if (!allNonNullRefs) {
                settingsIssue = true;
            }
        }

        if (settingsIssue) {
            settings = defaultSettings;
            SaveSettings();
        }
        if (!CanLoadJson(LoadPastGames)) {
            SavePastGames();
        }
        StaticData.settings = LoadSettings();
    }

    private bool CanLoadJson<T>(Func<T> loadFunction)
    {
        try {
            loadFunction();
            return true;
        }
        catch {
            return false;
        }
    }


    public void ChangeWindowedMode(bool windowed)
    {
        settings = LoadSettings();
        settings.windowedMode = windowed;
        SaveSettings();
    }

    public void ChangeVolume(float vol)
    {
        settings = LoadSettings();
        settings.volume = vol;
        SaveSettings();
    }

    public void ChangeUiColors(UiColors uiColors)
    {
        settings = LoadSettings();
        settings.uiColors = uiColors;
        SaveSettings();
    }

    public void ChangeBoardColors(BoardColors boardColors)
    {
        settings = LoadSettings();
        settings.boardColors = boardColors;
        SaveSettings();
    }

    public void ChangeSymbolMode(bool symbolMode)
    {
        settings = LoadSettings();
        settings.symbolMode = symbolMode;
        SaveSettings();
    }

    public Settings LoadSettings()
    {
        string settingsData = File.ReadAllText(settingsFile);
        Settings settings = JsonUtility.FromJson<Settings>(settingsData);
        settings.boardColors = boardColorsList[settings.boardColorsIndex];
        settings.uiColors = uiColorsList[settings.uiColorsIndex];

        return settings;    
    }

    private void SaveSettings()
    {
        settings.boardColorsIndex = boardColorsList.IndexOf(settings.boardColors);
        settings.uiColorsIndex = uiColorsList.IndexOf(settings.uiColors);
        string settingsData = JsonUtility.ToJson(settings);
        File.WriteAllText(settingsFile, settingsData);
        StaticData.settings = settings;
    }

    public void SaveGame(Game game)
    {
        pastGames.gameList = LoadPastGames();
        pastGames.gameList.Add(game);
        SavePastGames();
    }

    private void SavePastGames()
    {
        string pastGamesData = JsonUtility.ToJson(pastGames);
        File.WriteAllText(pastGamesFile, pastGamesData);
    }

    public List<Game> LoadPastGames()
    {
        string pastGamesData = File.ReadAllText(pastGamesFile);
        List<Game> games = JsonUtility.FromJson<PastGames>(pastGamesData).gameList;
        return games;

    }

}

[System.Serializable]
public class PastGames
{
    public List<Game> gameList = new List<Game>();
}

[System.Serializable]
public class Game
{
    public List<int> moveList;
    public int rows;
    public int cols;
    public bool aiFirst;
    public bool redWon;
}

[System.Serializable]
public class Settings
{
    public float volume;
    public bool symbolMode;
    public bool windowedMode;
    public int boardColorsIndex;
    public int uiColorsIndex;
    public BoardColors boardColors;
    public UiColors uiColors;
}
