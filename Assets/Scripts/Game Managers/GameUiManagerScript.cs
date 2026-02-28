using TMPro;
using UnityEngine;

public class GameUiManager : BaseUiManager
{
    [Header("Subclass Gameobject References")]
    [SerializeField] private TMP_Text redScoreText;
    [SerializeField] private TMP_Text yellowScoreText;
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private TMP_Text gamemodeText;
    [SerializeField] private TMP_Text aiWarningText;
    [SerializeField] private TMP_Text aiLoadingText;
    [SerializeField] private GameObject continueButton;

    private void Update()
    {
        if (StorageManager.instance.isLoading) {
            return;
        }
        base.DisableObject(aiLoadingText.gameObject);
    }

    public void UpdateAiWarning()
    {
        if (GameConfig.rows != 6 || GameConfig.cols != 7) {
            base.EnableObject(aiWarningText.gameObject);
        }
        else {
            base.DisableObject(aiWarningText.gameObject);
        }
    }

    public void EnableAiMode(GameObject aiSelect)
    {
        if (GameConfig.rows == 6 && GameConfig.cols == 7 && !StorageManager.instance.isLoading) {
            base.EnableObject(aiSelect);
        }
    }

    public void EnableForfeit(GameObject forfeitOption)
    {
        if (!(GameState.yellowWon || GameState.redWon || GameState.isTie)) {
            base.EnableObject(forfeitOption);
        }
    }

    /// <summary>
    ///  Updates the turn indicator text based on who's turn it is.
    /// </summary>
    public void UpdateTurnIndicator()
    {
        if (GameState.redTurn) {
            turnText.text = "RED TURN"; 
        }
        else {
            turnText.text = "YELLOW TURN";
        }  

        if (!GameConfig.aiMode) {
            return;
        }
        if (GameConfig.aiFirst && GameState.redTurn || !GameConfig.aiFirst && !GameState.redTurn) {
            turnText.text += " (AI THINKING)";
        }
    }

    public void ChangeModeText()
    {    
        if (GameConfig.aiMode) {
            gamemodeText.text = "AI Mode: ";
            if (GameConfig.aiFirst) {
                gamemodeText.text += "Ai First";
            }
            else {
                gamemodeText.text +=  "Ai Second";
            }
        }
        else {
            gamemodeText.text = "2-Player";
        }
    }

    /// <summary>
    ///  Updates the turn indicator text to display who just won.
    /// </summary>
    public void DisplayWin()
    {
        if (GameState.redWon) {
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
        turnText.text = "TIE!!"; UpdateRedScore(); UpdateYellowScore();
        Debug.
            Log(
            "hi"

            );
        EnableContinueButton();
    }

    /// <summary>
    ///  Increment red score by 1.
    /// </summary>
    private void UpdateRedScore()
    {
        MatchState.redPoints++;
        int score = MatchState.redPoints;
        redScoreText.text = score.ToString();
    }

    /// <summary>
    ///  Increment yellow score by 1.
    /// </summary>
    private void UpdateYellowScore()
    {
        MatchState.yellowPoints++;
        int score = MatchState.yellowPoints;
        yellowScoreText.text = score.ToString();
    }

    public void ResetScores()
    {
        MatchState.redPoints = 0;
        MatchState.yellowPoints = 0;
        redScoreText.text = 0.ToString();
        yellowScoreText.text = 0.ToString();
    }

    public void EnableContinueButton()
    {
        base.EnableObject(continueButton);
    }

    public void DisableContinueButton()
    {
        base.DisableObject(continueButton);
    }

}
