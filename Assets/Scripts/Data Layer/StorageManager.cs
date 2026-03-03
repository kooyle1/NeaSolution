using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public class StorageManager : MonoBehaviour
{
    public static StorageManager instance { get; private set; }
    
    [SerializeField] Settings defaultSettings;
    [SerializeField] private List<BoardColors> boardColorsList;
    [SerializeField] private List<UiColors> uiColorsList;

    public Settings settings { get; private set; }
    public BoardColors boardColors { get; private set; }
    public UiColors uiColors { get; private set; }
    public PastGames pastGames { get; private set; } = new PastGames();    

    //Variables for loading opening book via multithreading
    private Dictionary<ulong, sbyte> openingBook;
    private Task<Dictionary<ulong, sbyte>> currentTask;
    private Action<Dictionary<ulong, sbyte>> callback;
    public bool isLoading { get; private set; }    

    private string pastGamesFile;
    private string settingsFile;

    private void Awake()
    {
        if (instance == null) {
            instance = this;
        }

        else {
            Destroy(gameObject);
            return;
        }

        pastGamesFile = Path.Combine(Application.persistentDataPath, "PastGames.json");
        settingsFile = Path.Combine(Application.persistentDataPath, "Settings.json");

        if (!CanRunFunction(LoadSettings)) {
            instance.settings = defaultSettings;
            SaveSettings(); //Go back to default settings if there is an error 
        }
        else {
            settings = LoadSettings();
        }
        if (!CanRunFunction(LoadPastGames)) {
            SavePastGames(); //Empty past games file if there is an error
        }
        else {
            pastGames.gameList = LoadPastGames();
        }
    }

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

    /// <summary>
    ///  Load book from the book12.bin file.
    ///  Loading is performed on a background thread so as to not clog up the main thread.
    /// </summary>
    public void LoadBook(Action<Dictionary<ulong, sbyte>> onTaskFinished)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "book12.bin");
        if (currentTask != null) {
            return;
        }
        isLoading = true;
        callback = onTaskFinished;
        currentTask = Task.Run(() => LoadBinary(path));
    }


    /// <summary>
    ///  Create an opening book binary file from csv file.
    /// </summary>
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

    /// <summary>
    ///  Load opening book from csv file.
    /// </summary>
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

    /// <summary>
    ///  Load opening book from binary file.
    /// </summary>
    private Dictionary<ulong, sbyte> LoadBinary(string binaryPath)
    {
        if (openingBook != null) {
            return openingBook;
        }
        openingBook = new Dictionary<ulong, sbyte>();
        FileStream stream = new FileStream(binaryPath, FileMode.Open, FileAccess.Read, FileShare.Read);

        using (BinaryReader reader = new BinaryReader(stream)) {
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
        boardColors = boardColorsList[settings.boardColorsIndex];
        uiColors = uiColorsList[settings.uiColorsIndex];

        return settings;    
    }

    /// <summary>
    ///  Encodes settings variable as a string and writes it to the Settings.json file.
    /// </summary>
    private void SaveSettings()
    {
        boardColors = boardColorsList[settings.boardColorsIndex];
        uiColors = uiColorsList[settings.uiColorsIndex];
        string settingsData = JsonUtility.ToJson(settings);
        File.WriteAllText(settingsFile, settingsData);
    }

    /// <summary>
    ///  Adds game to the past game list.
    /// </summary>
    public void SaveGame(Game game)
    {
        if (!CanRunFunction(LoadPastGames)) {
            pastGames = new PastGames();
            SavePastGames();
        }
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
}
