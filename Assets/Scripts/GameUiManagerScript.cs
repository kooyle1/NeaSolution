using TMPro;
using UnityEngine;
using System;


public class GameUiManagerScript : BaseUiManagerScript
{
    [Header("Subclass Gameobject References")]
    [SerializeField] private TMP_Text redScoreText;
    [SerializeField] private TMP_Text yellowScoreText;
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private TMP_Text gamemodeText;
    [SerializeField] private GameObject continueButton;

    /// <summary>
    ///  Updates the turn indicator text based on who's turn it is.
    /// </summary>
    public void UpdateTurnIndicator()
    {
        if (StaticData.redTurn) {
            turnText.text = "RED TURN"; 
        }
        else {
            turnText.text = "YELLOW TURN";
        }  
    }

    public void ChangeModeText(bool isAiMode, bool aiFirst)
    {
        string aiFirstText;

        if (aiFirst) {
            aiFirstText = "Ai First";
        }
        else {
            aiFirstText = "Ai Second";
        }  
        

        if (isAiMode) {
            gamemodeText.text = "AI Mode: " + aiFirstText;
        }
        else {
            gamemodeText.text = "2-Player: " + aiFirstText;
        }
    }

    /// <summary>
    ///  Updates the turn indicator text to display who just won.
    /// </summary>
    public void DisplayWin()
    {
        if (StaticData.redWon) {
            turnText.text = "RED WON!!";
            UpdateRedScore();
        }
        else {
            turnText.text = "YELLOW WON!!";
            UpdateYellowScore();
        }

        EnableContinueButton();
    }

    /// <summary>
    ///  Updates the turn indicator text to display tie.
    /// </summary>
    public void DisplayTie()
    {
        turnText.text = "TIE!!";
    }

    /// <summary>
    ///  Increment red score by 1.
    /// </summary>
    private void UpdateRedScore()
    {
        StaticData.redPoints++;
        int score = StaticData.redPoints;
        redScoreText.text = score.ToString();
    }

    /// <summary>
    ///  Increment yellow score by 1.
    /// </summary>
    private void UpdateYellowScore()
    {
        StaticData.yellowPoints++;
        int score = StaticData.yellowPoints;
        yellowScoreText.text = score.ToString();
    }

    public void ResetScores()
    {
        StaticData.redPoints = 0;
        StaticData.yellowPoints = 0;
        redScoreText.text = 0.ToString();
        yellowScoreText.text = 0.ToString();

    }

    private void EnableContinueButton()
    {
        base.EnableObject(continueButton);
    }
}
