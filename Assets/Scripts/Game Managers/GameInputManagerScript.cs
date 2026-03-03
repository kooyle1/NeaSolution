using UnityEngine;
using UnityEngine.EventSystems;

public class GameInputManager : MonoBehaviour
{
    [Header("MonoBehaviour Script References")]
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private GameUiManager gameUiManager;
    [SerializeField] Logger logger;

    [SerializeField] GameController gameController;
    [SerializeField] AiController aiController;


    private void Start()
    {
        gameController.ResetGame();
        SetAiModeFalse();
    }

    /// <summary>
    ///  Updatse difficulty from the difficulty dropdown.
    /// </summary>
    public void UpdateDifficulty(int index)
    {
        GameConfig.aiDifficulty = GameConfig.difficultyList[index];
    }

    /// <summary>
    ///  Plays player's move in a column if the move is valid. Will then play an AI move if ai mode is enabled.
    /// </summary>
    public void PlayMoveOnClick()
    {
        if (aiController.aiThinking) {
            return;
        }
        
        GameObject column = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
        int columnIndex = boardManager.columnList.IndexOf(column);    
        if (!gameController.TryMakeMove(columnIndex)) {
            return;
        }
        UpdateState();

        if (!(GameState.yellowWon || GameState.redWon || GameState.isTie) && GameConfig.aiMode) {
            PlayAiMove();
        }
    }

    private void PlayAiMove()
    {
        aiController.StartSolving(scores => {
            int move = aiController.ReturnMoveByDifficulty(scores);
            gameController.TryMakeMove(move);
            UpdateState();
        });
    }

    /// <summary>
    ///  Calls UI manager to update text displaying game state.
    ///  Calls Game Controller to log game if game state is terminal.
    /// </summary>
    public void UpdateState()
    {
        if (GameState.isTie) {
            gameUiManager.DisplayTie();
            gameController.LogGame();
        }
        else if (GameState.redWon || GameState.yellowWon) {
            gameUiManager.DisplayWin();
            gameController.LogGame();
        }
        else {
            gameUiManager.UpdateTurnIndicator();
        }
    }

    /// <summary>
    ///  Set AI mode true and update UI manager.
    /// </summary>
    public void SetAiModeTrue()
    {
        GameConfig.aiMode = true;
        gameUiManager.ChangeModeText();
    }

    /// <summary>
    ///  Set AI mode false and update UI manager.
    /// </summary>
    public void SetAiModeFalse()
    {
        GameConfig.aiMode = false;
        gameUiManager.ChangeModeText();
    }

    /// <summary>
    ///  Set AI to go first.
    /// </summary>
    public void SetAiFirst()
    {
        GameConfig.aiFirst = true;
    }

    /// <summary>
    ///  Set AI to go second.
    /// </summary>
    public void SetAiSecond()
    {
        GameConfig.aiFirst = false;
    }

    /// <summary>
    ///  Forfeit the round for the current player. Opposite player wins and gains a point.
    /// </summary>
    public void ForfeitRound()
    {
        if (aiController.aiThinking) {
            return;
        }

        if (GameState.redTurn) {
            GameState.yellowWon = true;
        }
        else {
            GameState.redWon = true;
        }
        gameUiManager.DisplayWin();
    }


    /// <summary>
    ///  Reset board and turn and start the next round. Also saves this round's game to the past game list file if game reached an end state.
    /// </summary>
    public void StartNextRound()
    {       
        gameController.ResetGame();
        boardManager.CreateBoard();
        gameUiManager.DisableContinueButton();
        aiController.StopSolving();
        UpdateState();

        if (GameConfig.aiFirst && GameConfig.aiMode) {
            PlayAiMove();
        }
    }

    /// <summary>
    ///  Starts a new round but also resets scores.
    /// </summary>
    public void StartNewGame()
    {
        StartNextRound();
        gameUiManager.ResetScores();
        gameUiManager.UpdateAiWarning();
        gameUiManager.ChangeModeText();

    }
}
