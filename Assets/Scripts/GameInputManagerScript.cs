using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameInputManagerScript : MonoBehaviour
{
    [Header("MonoBehaviour Script References")]
    [SerializeField] private BoardManagerScript boardManager;
    [SerializeField] private GameUiManagerScript gameUiManager;
    [SerializeField] private GameLogicScript gameLogicManager;
    [SerializeField] private StorageManagerScript storageManager;
    [SerializeField] private SolverScript solver;
    [SerializeField] LoggerScript logger;

    private bool aiMode = false;
    private bool aiFirst = false;
    private bool isRed = true;
    private bool yellowWon = false;
    private bool redWon = false;
    private bool isTie = false;
    private Task<int> aiTask;
    private bool aiThinking = false;

    private List<int> movesMade;

    private void Start()
    {
        movesMade = new List<int>();
    }

    private void Update()
    {
        if (!aiThinking || aiTask == null) return;
        if (!aiTask.IsCompleted) return;

        int move = aiTask.Result;
        aiTask = null;
        aiThinking = false;

        ApplyMove(move);
    }

    public void PlayMoveOnClick()
    {
        if (yellowWon || redWon || isTie || aiThinking) {
            logger.Log("Player tried moving after the round ended or while AI was thinking.");
            return;
        }

        GameObject column = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
        int columnIndex = boardManager.columnList.IndexOf(column);    
        ApplyMove(columnIndex);

        if (!(yellowWon || redWon || isTie) && aiMode) {
            StartCoroutine(PlayAiMove());
        }
                  
    }

    private void ApplyMove(int columnIndex)
    {
        if (!gameLogicManager.CanPlayColumn(columnIndex)) {
            logger.Log("Player tried moving in a full column.");
            return;
        }

        AudioManager.instance.PlayMoveSFX();
        gameLogicManager.PlayMove(columnIndex);
        logger.Log($"Successfully made a move in column {columnIndex}");

        //Update turn and assume player won before actually checking     
        if (isRed) {
            isRed = false;
            redWon = true;
        }
        else {
            isRed = true;
            yellowWon = true;
        }

        boardManager.PlayMove(columnIndex);
        boardManager.UpdateTurn(isRed);

        //Display text based on what happened after the move (win, tie, or nothing)
        if (gameLogicManager.CheckWin()) {
            logger.Log($"A win occured on this turn.");
            gameUiManager.DisplayWin(redWon);
        }
        else if (gameLogicManager.CheckTie()) {
            logger.Log("A tie occured on this turn.");
            gameUiManager.DisplayTie();
            isTie = true;
        }
        else {
            gameUiManager.UpdateTurnIndicator(isRed);
            logger.Log("Game did not end, now other player's turn.");
            redWon = false;
            yellowWon = false;
        }

        movesMade.Add(columnIndex);
    }

    private IEnumerator PlayAiMove()
    {
        yield return new WaitForSeconds(0.01f);

        if (aiThinking) {
            yield return null;
        }

        aiThinking = true;

        ulong pos = gameLogicManager.GetPos();
        ulong board = gameLogicManager.GetBoard();

        aiTask = Task.Run(() =>
        {
            return solver.ReturnBestMove(pos, board);
        });

    }

    public void SetAiMode(bool inp)
    {
        aiMode = inp;
        gameUiManager.ChangeModeText(aiMode, aiFirst);
    }

    public void SetAiFirst(bool inp)
    {
        aiFirst = inp;
    }

    public void StartNextRound()
    {
        if (redWon || yellowWon || isTie) {
            Game playedGame = new Game
            {
                moveList = movesMade,
                aiFirst = aiFirst,
                redWon = redWon
            };
            storageManager.SaveGame(playedGame);
        }

        isRed = true;
        yellowWon = false;
        redWon = false;
        isTie = false;

        gameUiManager.UpdateTurnIndicator(isRed);
        boardManager.UpdateTurn(isRed);
        gameLogicManager.ResetBoard();
        boardManager.CreateBoard();
        movesMade.Clear();

        if (aiFirst && aiMode) {
            StartCoroutine(PlayAiMove());
        }
    }

    public void StartNewGame()
    {
        StartNextRound();
        gameUiManager.ResetScores();

    }

    /* For analysis screen
    public void PlayMove(int column)
    {
        Button slot = GetBottomSlot(boardManager.columnList[column]);
        if (!slot) {
            return;
        }

        gameUiManager.UpdateTurnIndicator(isRed);
        boardManager.UpdateTurn(isRed);
        if (isRed) {
            slot.image.color = boardManager.BoardColors.fullRedColor;
            isRed = false;
        }
        else {
            slot.image.color = boardManager.BoardColors.fullYellowColor;
            isRed = true;
        }
    }

    public void UndoMove(int column)
    {
        Button slot = GetBottomSlot(boardManager.columnList[column]);
        if (!slot) {
            return;
        }

        gameUiManager.UpdateTurnIndicator(isRed);
        boardManager.UpdateTurn(isRed);
        if (isRed) {
            slot.image.color = boardManager.BoardColors.buttonColor;
            isRed = false;
        }
        else {
            slot.image.color = boardManager.BoardColors.buttonColor;
            isRed = true;
        }
    }
    */
}
