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

    private bool isRed = true;

    public override void SetColors()
    {  
        base.SetDropdownColors();       
        base.SetColors();
    }

    public void UpdateTurnIndicator()
    {
        if (isRed) {
            turnText.text = "YELLOW TURN";
            isRed = false;
            return;
        }

        turnText.text = "RED TURN";
        isRed = true;   
        
    }

    public void UpdateRedScore()
    {
        int score = Convert.ToInt32(redScoreText.text) + 1;
        redScoreText.text = score.ToString();
    }

    public void UpdateYellowScore()
    {
        int score = Convert.ToInt32(yellowScoreText.text) + 1;
        yellowScoreText.text = score.ToString();
    }
}
