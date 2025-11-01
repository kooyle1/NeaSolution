using TMPro;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;


public class GameUiManager : BaseUiManager
{
    [Header("Subclass References")]
    [SerializeField] private TMP_Text redScoreText;
    [SerializeField] private TMP_Text yellowScoreText;
    [SerializeField] private TMP_Text turnText;
    [Space]
    [SerializeField] private GameObject editBoardOptions;

    public override void SetColors()
    {  
        base.SetDropdownColors();       
        base.SetColors();
    }

    /// <summary>
    ///  Updates the text displaying who's turn it is.
    /// </summary>
    public void UpdateTurnIndicator(bool redPlayedTurn, bool playerWon = false)
    {
        if (redPlayedTurn) {
            if (playerWon) {
                Debug.Log("fdfd");
                turnText.text = "YELLOW WON!!";
                return;
            }
            turnText.text = "YELLOW TURN";
        }
        else {
            if (playerWon) {
                Debug.Log("fdfd");
                turnText.text = "RED WON!!";
                return;
            }
            turnText.text = "RED TURN";
        }  
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
        editBoardOptions.SetActive(true);
    }

    public void DisableEditBoardOptions()
    {
        editBoardOptions.SetActive(false);
    }
}
