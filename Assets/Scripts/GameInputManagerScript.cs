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

    private Task<int> aiTask;
    private bool aiThinking = false;

    private List<int> movesMade;

    private void Start()
    {
        movesMade = new List<int>();
        StaticData.isTie = false;
        StaticData.redWon = false;
        StaticData.yellowWon = false;
        StaticData.redTurn = true;
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
        if (StaticData.yellowWon || StaticData.redWon || StaticData.isTie || aiThinking) {
            logger.Log("Player tried moving after the round ended or while AI was thinking.");
            return;
        }

        GameObject column = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
        int columnIndex = boardManager.columnList.IndexOf(column);    
        ApplyMove(columnIndex);

        if (!(StaticData.yellowWon || StaticData.redWon || StaticData.isTie) && StaticData.aiMode) {
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
        boardManager.PlayMove(columnIndex);
        logger.Log($"Successfully made a move in column {columnIndex}");

        //Update turn and assume player won before actually checking     
        if (StaticData.redTurn) {
            StaticData.redTurn = false;
            StaticData.redWon = true;
        }
        else {
            StaticData.redTurn = true;
            StaticData.yellowWon = true;
        }

        //Display text based on what happened after the move (win, tie, or nothing)
        if (gameLogicManager.CheckWin()) {
            logger.Log($"A win occured on this turn.");
            gameUiManager.DisplayWin();
        }
        else if (gameLogicManager.CheckTie()) {
            logger.Log("A tie occured on this turn.");
            gameUiManager.DisplayTie();
            StaticData.isTie = true;
        }
        else {
            gameUiManager.UpdateTurnIndicator();
            logger.Log("Game did not end, now other player's turn.");
            StaticData.redWon = false;
            StaticData.yellowWon = false;
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
        StaticData.aiMode = inp;
        gameUiManager.ChangeModeText(StaticData.aiMode, StaticData.aiFirst);
    }

    public void SetAiFirst(bool inp)
    {
        StaticData.aiFirst = inp;
    }

    public void ForfeitRound()
    {
        if (aiThinking) {
            return;
        }
        if (StaticData.redTurn) {
            StaticData.yellowWon = true;
        }
        else {
            StaticData.redWon = true;
        }
        gameUiManager.DisplayWin();
    }

    public void StartNextRound()
    {
        if (StaticData.redWon || StaticData.yellowWon || StaticData.isTie) {
            Game playedGame = new Game
            {
                moveList = movesMade,
                aiFirst = StaticData.aiFirst,
                redWon = StaticData.redWon
            };
            storageManager.SaveGame(playedGame);
        }

        StaticData.redTurn = true;
        StaticData.yellowWon = false;
        StaticData.redWon = false;
        StaticData.isTie = false;

        gameUiManager.UpdateTurnIndicator();
        gameLogicManager.ResetBoard();
        boardManager.CreateBoard();
        movesMade.Clear();

        if (StaticData.aiFirst && StaticData.aiMode) {
            StartCoroutine(PlayAiMove());
        }
    }

    public void StartNewGame()
    {
        StartNextRound();
        gameUiManager.ResetScores();

    }
}
