using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private BitboardManager bitboard;
    [SerializeField] private Logger logger;

    private List<int> movesMade;

    private void Start()
    {
        movesMade = new List<int>();
    }

    public bool TryMakeMove(int columnIndex)
    {
        if (GameState.yellowWon || GameState.redWon || GameState.isTie || !bitboard.CanPlayColumn(columnIndex)) {
            logger.Log("Player tried moving after the round ended or while AI was thinking.");
            return false;
        }

        ApplyMove(columnIndex);
        return true;
    }

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
    private void ApplyMove(int columnIndex)
    {
        if (!bitboard.CanPlayColumn(columnIndex)) {
            logger.Log("Player tried moving in a full column.");
            return;
        }
        AudioManager.instance.PlayMoveSFX(); //Play SFX
        bitboard.PlayMove(columnIndex); //Play move in the bitboard
        boardManager.PlayMove(columnIndex); //Place coin in the board
        logger.Log($"Successfully made a move in column {columnIndex}");

        EvaluateGameState();
        GameState.redTurn = !GameState.redTurn;

        movesMade.Add(columnIndex);
    }

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
            GameState.redWon = false;
            GameState.yellowWon = false;
        }
    }

    public void LogGame()
    {
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
