using NUnit.Framework;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class StorageManagerScript : MonoBehaviour
{
    private PastGames pastGames = new PastGames();
    private Settings settings = new Settings();
    
    private string pastGamesFile;
    private string settingsFile;

    private void Awake()
    {
        pastGamesFile = Application.persistentDataPath + "/PastGames.json";
        settingsFile = Application.persistentDataPath + "/Settings.json";
        StaticData.settings = LoadSettings();
    }

    public void ChangeVolume(float vol) {
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

        return JsonUtility.FromJson<Settings>(settingsData);
    }

    private void SaveSettings()
    {
        string settingsData = JsonUtility.ToJson(settings);
        File.WriteAllText(settingsFile, settingsData);
        StaticData.settings = settings;
    }

    public void SaveGame(Game game)
    {
        pastGames.gameList = LoadPastGames();
        pastGames.gameList.Add(game);
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
    public bool aiFirst;
    public bool redWon;
}

[System.Serializable]
public class Settings
{
    public float volume;
    public bool symbolMode;
    public BoardColors boardColors;
    public UiColors uiColors;
}
