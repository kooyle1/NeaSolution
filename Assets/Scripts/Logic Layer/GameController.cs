using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Monobehaviour Script References")]
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private BitboardManager bitboard;
    [SerializeField] private Logger logger;

    private List<int> movesMade;

    private void Start()
    {
        movesMade = new List<int>();
    }

    /// <summary>
    ///   Tries to place a coin in the game board in the given column. Returns false if it failed.
    /// </summary>
    public bool TryMakeMove(int columnIndex)
    {
        if (GameState.yellowWon || GameState.redWon || GameState.isTie || !bitboard.CanPlayColumn(columnIndex)) {
            logger.Log("Player tried moving after the round ended or while AI was thinking.");
            return false;
        }

        ApplyMove(columnIndex);
        return true;
    }

    /// <summary>
    ///   Tries to remove a coin in the game board in the given column. Returns false if it failed.
    /// </summary>
    public void TryUndoMove(int columnIndex)
    {
        if (movesMade.Count == 0 || movesMade[^1] != columnIndex) {
            return;
        }

        AudioManager.instance.PlayMoveSFX();
        bitboard.UndoMove(columnIndex);
        boardManager.RemoveCoin(columnIndex);
        logger.Log($"Successfully undid a move in column {columnIndex}");

        EvaluateGameState();
        GameState.redTurn = !GameState.redTurn;

        movesMade.RemoveAt(movesMade.Count - 1);    
    }

    /// <summary>
    ///   Resets: GameState variables, bitboard, list storing played moves.
    /// </summary>
    public void ResetGame()
    {
        GameState.redTurn = true;
        GameState.yellowWon = false;
        GameState.redWon = false;
        GameState.isTie = false;

        bitboard.ResetBitboard();
        bitboard.UpdateTieCheckMask();
        movesMade.Clear();
    }

    /// <summary>
    ///   Places coin in both physical game board and bitboard assuming the move is legal.
    ///   Also evaluates the game state and checks whether game has ended then swaps turn.
    ///   Then adds the move to movesMade.
    /// </summary>
    private void ApplyMove(int columnIndex)
    {
        AudioManager.instance.PlayMoveSFX(); //Play SFX
        bitboard.PlayMove(columnIndex); //Play move in the bitboard
        boardManager.PlayMove(columnIndex); //Place coin in the board
        logger.Log($"Successfully made a move in column {columnIndex}");

        EvaluateGameState();
        GameState.redTurn = !GameState.redTurn;

        movesMade.Add(columnIndex);
    }

    /// <summary>
    ///   Uses bitboard to check if a win or tie occurred.
    ///   Changes game state accordingly (does not swap turn).
    /// </summary>
    private void EvaluateGameState()
    {
        //Assume player won before actually checking to avoid nested ifs in CheckWin    
        if (GameState.redTurn) {
            GameState.redWon = true;
        }
        else {
            GameState.yellowWon = true;
        }

        //Change GameState based on current game state
        if (bitboard.CheckWin()) {
            GameState.isTie = false;    
            logger.Log($"A win occured on this turn.");
        }
        else if (bitboard.CheckTie()) {
            logger.Log("A tie occured on this turn.");
            GameState.isTie = true;
            GameState.redWon = false;
            GameState.yellowWon = false;
        }
        else {
            logger.Log("Game did not end, now other player's turn.");
            GameState.isTie = false;
            GameState.redWon = false;
            GameState.yellowWon = false;
        }
    }

    /// <summary>
    ///   Calls the storage manager to store the played game.
    /// </summary>
    public void LogGame()
    {
        //Record gamemode into a string
        string mode = "";
        if (!GameConfig.aiMode) {
            mode = "2-player";
        }
        else {
            mode = "Ai mode: ";
            if (GameConfig.aiFirst) {
                mode += "Ai first ";
            }
            else {
                mode += "Ai second ";
            }
            mode += "(" + GameConfig.aiDifficulty + ")";
        }

        //Create a new Game object using GameState and GameConfig
        Game playedGame = new Game
        {
            moveList = movesMade,
            rows = GameConfig.rows,
            cols = GameConfig.cols,
            mode = mode,
            redWon = GameState.redWon,
            isTie = GameState.isTie
        };
        StorageManager.instance.SaveGame(playedGame);
    }
}
