using System.IO;
using UnityEngine;

public class StorageManagerScript : MonoBehaviour
{
    private string pastGamesFile;

    private void Awake()
    {
        pastGamesFile = Application.dataPath + "/Data/PastGames.txt";
    }

    public void AppendGame(string gameInfo)
    {
        if (!File.Exists(pastGamesFile)) {
            File.WriteAllText(pastGamesFile, gameInfo);
        }

        else {
            using (var writer = new StreamWriter(pastGamesFile, true)) {
                writer.WriteLine(gameInfo);
            }
        }
    }

    public string[] GetAllPastGames()
    {
        return File.ReadAllLines(pastGamesFile);

    }
}
