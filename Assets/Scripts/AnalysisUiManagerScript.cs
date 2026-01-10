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
    public List<int[]> pastGamesList;

    //Testing
    protected override void Start()
    {
        buttonTextList = buttonExample.GetComponentsInChildren<TMP_Text>(true).ToList();
        pastGamesList = new List<int[]>();
        /*
        pastGamesList = new List<int[]>();

        string[] pastGamesInfo = storageManager.GetAllPastGames();
        int index = pastGamesInfo.Length - 1;
        int i;

        while (index >= 0) {
            i = index % 3;
            switch (i) {
                case 0:
                    pastGamesList.Add(Array.ConvertAll(pastGamesInfo[index].Split(", "), s => int.Parse(s)));
                    buttonTextList[i].text = (index/3).ToString();
                    break;
                case 1:
                    if (pastGamesInfo[index] == "False") {
                        buttonTextList[i].text = "2-player mode";
                    }
                    else {
                        buttonTextList[i].text = "Ai Mode";
                    }
                    break;
                case 2:
                    if (pastGamesInfo[index] == "False") {
                        buttonTextList[i].text = "Player 2 won";
                    }
                    else {
                        buttonTextList[i].text = "Player 1 won";
                    }
                    break;

            }
            index--;

            if (i == 0) {
                UpdateScrollView(buttonExample, (index-1)/3);
            }
        }
        */

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

            pastGamesList.Add(game.moveList.ToArray());

            UpdateScrollView(buttonExample, index);
            index--;

        }
            
        base.Start();
    }

    public void UpdateTurnIndicator(bool isRed)
    {
        if (isRed) {
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

    public int[] GetGameByIndex(int index)
    {
        return pastGamesList[index];
    }

    private void UpdateScrollView(Button button, int num)
    {
        button.name = num.ToString();
        Instantiate(button, scrollviewContent.transform);
    }
}
