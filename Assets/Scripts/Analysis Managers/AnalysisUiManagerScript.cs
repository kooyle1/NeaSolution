using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnalysisUiManagerScript : BaseUiManager
{
    [Header("Gameobject References")]
    [SerializeField] private Button buttonExample;
    [SerializeField] private GameObject scrollviewContent;

    [Header("Subclass Gameobject References")]
    [SerializeField] private TMP_Text solutionText;
    [SerializeField] private TMP_Text moveRatingText;
    [SerializeField] private GameObject solutionObject;
    [SerializeField] private TMP_Text turnText;


    private List<TMP_Text> buttonTextList;
    private Dictionary<Button, Game> pastGamesDict;

    protected override void Start()
    {
        buttonTextList = buttonExample.GetComponentsInChildren<TMP_Text>(true).ToList();
        pastGamesDict = new Dictionary<Button, Game>();
        List<Game> gamesInfoList = StorageManager.instance.LoadPastGames();
        gamesInfoList.Reverse();

        int index = gamesInfoList.Count - 1;
        foreach (Game game in gamesInfoList) {
            buttonTextList[0].text = (index + 1).ToString();
            buttonTextList[1].text = game.mode;

            
            if (game.isTie) {
                buttonTextList[2].text = "Tie";
            }
            else if (game.redWon) {
                buttonTextList[2].text = "Player 1 won";
            }
            else {
                buttonTextList[2].text = "Player 2 won";
            }

            UpdateScrollView(buttonExample, game);
            index--;

        }
            
        base.Start();
    }

    /// <summary>
    ///  Changes turn based on current turn in Static Data.
    /// </summary>
    public void UpdateTurnIndicator()
    {
        if (GameState.redWon) {
            turnText.text = "Current State: Red Won";
        }
        else if (GameState.yellowWon) {
            turnText.text = "Current State: Yellow Won";
        }
        else if (GameState.redTurn) {
            turnText.text = "Current State: Red Turn";
        }
        else {
            turnText.text = "Current State: Yellow Turn";
        }
    }

    /// <summary>
    ///  Displays text showing the solution with the column index (displayed column index starts counting at 1 not 0).
    /// </summary>
    public void DisplaySolution(int bestMove, string moveRating)
    {
        solutionText.text = $"Optimal Move: Column {bestMove + 1}";
        moveRatingText.text = "Move Rating: " + moveRating;
        base.EnableObject(solutionObject);
    }

    /// <summary>
    ///  Returns the game corresponding to the index. 
    /// </summary>
    public Game GetGameByIndex(GameObject obj)
    {
        Button button = obj.GetComponent<Button>();
        return pastGamesDict[button];
    }

    private void UpdateScrollView(Button button, Game game)
    {
        Button newButton = Instantiate(button, scrollviewContent.transform);
        pastGamesDict[newButton] = game;
    }
}
