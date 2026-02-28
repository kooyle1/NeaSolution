using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;

public class StorageManager : MonoBehaviour
{
    public static StorageManager instance { get; private set; }
    
    [SerializeField] Settings defaultSettings;
    [SerializeField] public List<BoardColors> boardColorsList;
    [SerializeField] public  List<UiColors> uiColorsList;

    private PastGames pastGames = new PastGames();
    public Settings settings { get; private set; }

    private Dictionary<ulong, sbyte> openingBook;
    private Task<Dictionary<ulong, sbyte>> currentTask;
    private Action<Dictionary<ulong, sbyte>> callback;
    public bool isLoading { get; private set; }    

    private string pastGamesFile;
    private string settingsFile;

    private void Update()
    {
        if (!isLoading || currentTask == null) return;

        if (currentTask.IsCompleted) {
            isLoading = false;
            openingBook = currentTask.Result;
            currentTask = null;

            callback?.Invoke(openingBook);
        }
    }

    public void LoadBook(Action<Dictionary<ulong, sbyte>> onTaskFinished)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "book12.bin");

        isLoading = true;
        callback = onTaskFinished;
        currentTask = Task.Run(() => LoadBinary(path));
    }

    private void SaveBinary(string binaryPath)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(binaryPath, FileMode.Create))) {
            writer.Write(openingBook.Count);

            foreach (var kvp in openingBook) {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
        }
    }

    private void LoadCSV(string path)
    {
        foreach (var line in File.ReadLines(path)) {
            if (line.StartsWith("key")) continue;

            var parts = line.Split(',');

            ulong key = ulong.Parse(parts[0]);
            sbyte score = sbyte.Parse(parts[1]);

            openingBook[key] = score;
        }
    }

    private Dictionary<ulong, sbyte> LoadBinary(string binaryPath)
    {
        if (openingBook != null) {
            return openingBook;
        }
        openingBook = new Dictionary<ulong, sbyte>();

        using (BinaryReader reader = new BinaryReader(File.Open(binaryPath, FileMode.Open))) {
            int count = reader.ReadInt32();

            openingBook = new Dictionary<ulong, sbyte>(count);

            for (int i = 0; i < count; i++) {
                ulong key = reader.ReadUInt64();
                sbyte value = reader.ReadSByte();
                openingBook[key] = value;
            }
        }

        return openingBook;
    }

    private void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else {
            Destroy(gameObject);
        }

        pastGamesFile = Path.Combine(Application.persistentDataPath, "PastGames.json");
        settingsFile = Path.Combine(Application.persistentDataPath, "Settings.json");

        bool settingsIssue = false;
        
        if (!CanRunFunction(LoadSettings)) {
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
        if (!CanRunFunction(LoadPastGames)) {
            SavePastGames();
        }
        settings = LoadSettings();
    }

    /// <summary>
    ///  Returns false if function throws an exception. Returns true if function runs without issue.
    /// </summary>
    private bool CanRunFunction<T>(Func<T> loadFunction)
    {
        try {
            loadFunction();
            return true;
        }
        catch {
            return false;
        }
    }

    /// <summary>
    ///  Changes a setting and saves it. Takes in an Action of the form settings => settings.value = newValue.
    /// </summary>
    public void ChangeSetting(Action<Settings> update)
    {
        settings = LoadSettings();
        update(settings);
        SaveSettings();
    }

    /// <summary>
    ///  Reads the Settings.json file and returns settings. 
    /// </summary>
    private Settings LoadSettings()
    {
        string settingsData = File.ReadAllText(settingsFile);
        Settings settings = JsonUtility.FromJson<Settings>(settingsData);
        settings.boardColors = boardColorsList[settings.boardColorsIndex];
        settings.uiColors = uiColorsList[settings.uiColorsIndex];

        return settings;    
    }

    /// <summary>
    ///  Encodes settings variable as a string and writes it to the Settings.json file.
    /// </summary>
    private void SaveSettings()
    {
        settings.boardColorsIndex = boardColorsList.IndexOf(settings.boardColors);
        settings.uiColorsIndex = uiColorsList.IndexOf(settings.uiColors);
        string settingsData = JsonUtility.ToJson(settings);
        File.WriteAllText(settingsFile, settingsData);
    }

    /// <summary>
    ///  Adds game to the past game list.
    /// </summary>
    public void SaveGame(Game game)
    {
        pastGames.gameList = LoadPastGames();
        pastGames.gameList.Add(game);
        SavePastGames();
    }

    /// <summary>
    ///  Encodes pastGames variable as a string and writes it to the PastGames.json file.
    /// </summary>
    private void SavePastGames()
    {
        string pastGamesData = JsonUtility.ToJson(pastGames);
        File.WriteAllText(pastGamesFile, pastGamesData);
    }

    /// <summary>
    ///  Returns the list of past games.
    /// </summary>
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
    public string mode;
    public bool isTie;
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
