using TMPro;
using UnityEngine;
using System;


public class GameUiManagerScript : BaseUiManagerScript
{
    [Header("Subclass Gameobject References")]
    [SerializeField] private TMP_Text redScoreText;
    [SerializeField] private TMP_Text yellowScoreText;
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private GameObject editBoardPopup;

    /// <summary>
    ///  Updates the turn indicator text based on who's turn it is.
    /// </summary>
    public void UpdateTurnIndicator(bool isRed)
    {
        if (isRed) {
            turnText.text = "RED TURN"; 
        }
        else {
            turnText.text = "YELLOW TURN";
        }  
    }

    /// <summary>
    ///  Updates the turn indicator text to display who just won.
    /// </summary>
    public void DisplayWin(bool redWon)
    {
        if (redWon) {
            turnText.text = "RED WON!!";
        }
        else {
            turnText.text = "YELLOW WON!!";
        }
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
    public void UpdateRedScore()
    {
        int score = Convert.ToInt32(redScoreText.text) + 1;
        redScoreText.text = score.ToString();
    }

    /// <summary>
    ///  Increment yellow score by 1.
    /// </summary>
    public void UpdateYellowScore()
    {
        int score = Convert.ToInt32(yellowScoreText.text) + 1;
        yellowScoreText.text = score.ToString();
    }

    public void EnableEditBoardOptions()
    {
        editBoardPopup.SetActive(true);
    }

    public void DisableEditBoardOptions()
    {
        editBoardPopup.SetActive(false);
    }
}
