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

    private int newRowCount = 6;
    private int newColCount = 7;

    private void Update()
    {
        if (StorageManager.instance.isLoading) {
            return;
        }
        base.DisableObject(aiLoadingText.gameObject);
    }

    /// <summary>
    ///  Displays warning that AI is not available if board size is not 6x7 or opening book is loading.
    /// </summary>
    public void UpdateAiWarning()
    {
        if (GameConfig.rows != 6 || GameConfig.cols != 7) {
            base.EnableObject(aiWarningText.gameObject);
        }
        else {
            base.DisableObject(aiWarningText.gameObject);
        }
    }

    /// <summary>
    ///  Only enables inputted AI window object if board size is 6x7 and opening book has stopped loading.
    /// </summary>
    public void EnableAiWindow(GameObject aiSelect)
    {
        if (GameConfig.rows == 6 && GameConfig.cols == 7 && !StorageManager.instance.isLoading) {
            base.EnableObject(aiSelect);
        }
    }

    /// <summary>
    ///  Only enables inputted Forfeit window object if game hasn't ended.
    /// </summary>
    public void EnableForfeit(GameObject forfeitOption)
    {
        if (!(GameState.yellowWon || GameState.redWon || GameState.isTie)) {
            base.EnableObject(forfeitOption);
        }
    }

    /// <summary>
    ///  Updates the turn indicator text based on who's turn it is using GameConfig and GameState.
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

    /// <summary>
    ///  Updates text displaying mode using GameConfig.
    /// </summary>
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
            gamemodeText.text += $"\n({GameConfig.aiDifficulty})";
        }
        else {
            gamemodeText.text = "2-Player";
        }
    }

    /// <summary>
    ///  Updates the turn indicator text to display who just won using GameState.
    ///  Also increments winning player's score using MatchState.
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
        Debug.Log(MatchState.yellowPoints);
        int score = MatchState.yellowPoints;
        yellowScoreText.text = score.ToString();
    }

    /// <summary>
    ///  Reset both player's scores and update UI.
    /// </summary>
    public void ResetScores()
    {
        MatchState.redPoints = 0;
        MatchState.yellowPoints = 0;
        redScoreText.text = 0.ToString();
        yellowScoreText.text = 0.ToString();
    }

    /// <summary>
    ///  Update row count from dropdown. Does not apply to GameConfig until confirm is pressed.
    /// </summary>
    public void UpdateRowCount(int index)
    {
        newRowCount = index + 6;
    }

    /// <summary>
    ///  Update column count from dropdown. Does not apply to GameConfig until confirm is pressed.
    /// </summary>
    public void UpdateColumnCount(int index)
    {
        newColCount = index + 6;
    }

    /// <summary>
    ///  Applies any changes to col/row count to GameConfig.
    /// </summary>
    public void ConfirmBoardUpdate()
    {
        GameConfig.rows = newRowCount;
        GameConfig.cols = newColCount;

        if (newRowCount != 6 || newColCount != 7) {
            GameConfig.aiMode = false;
        }
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
