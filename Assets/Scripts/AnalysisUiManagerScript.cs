using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnalysisUiManagerScript : BaseUiManagerScript
{
    [Header("Gameobject References")]
    [SerializeField] private Button buttonExample;
    [SerializeField] private GameObject scrollviewContent;

    [Header("MonoBehaviour Script References")]
    [SerializeField] private StorageManagerScript storageManager;

    [Header("Subclass Gameobject References")]
    [SerializeField] private TMP_Text solutionText;
    [SerializeField] private GameObject solutionObject;
    [SerializeField] private TMP_Text turnText;


    public List<TMP_Text> buttonTextList;
    public Dictionary<int, Game> pastGamesDict;

    //Testing
    protected override void Start()
    {
        buttonTextList = buttonExample.GetComponentsInChildren<TMP_Text>(true).ToList();
        pastGamesDict = new Dictionary<int, Game>();
        List<Game> gamesInfoList = storageManager.LoadPastGames();
        gamesInfoList.Reverse();

        int index = gamesInfoList.Count - 1;
        foreach (Game game in gamesInfoList) {
            buttonTextList[0].text = (index + 1).ToString();

            if (game.aiFirst) {
                buttonTextList[1].text = "AI mode";            
            }
            else {
                buttonTextList[1].text = "2-player mode";
            }

            if (game.redWon) {
                buttonTextList[2].text = "Player 1 won";
            }
            else {
                buttonTextList[2].text = "Player 2 won";
            }

            pastGamesDict[index] = game;

            UpdateScrollView(buttonExample, index);
            index--;

        }
            
        base.Start();
    }

    public void UpdateTurnIndicator()
    {
        if (StaticData.redTurn) {
            turnText.text = "Current State: Red Turn";
        }
        else {
            turnText.text = "Current State: Yellow Turn";
        }
    }

    public void DisplaySolution(int columnIndex)
    {
        solutionText.text = $"Optimal Move: Column {columnIndex + 1}";
        base.EnableObject(solutionObject);
    }

    public Game GetGameByIndex(int index)
    {
        return pastGamesDict[index];
    }

    private void UpdateScrollView(Button button, int num)
    {
        button.name = num.ToString();
        Instantiate(button, scrollviewContent.transform);
    }
}
