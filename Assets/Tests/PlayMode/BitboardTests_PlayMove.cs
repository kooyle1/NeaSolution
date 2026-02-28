using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BitboardTests_PlayMove
{
    struct PlayMoveCase
    {
        public ulong pos;
        public ulong board;
        public List<int> moves;
    }

    private BitboardManager PlayMoveTest(PlayMoveCase testCase)
    {
        var obj = new GameObject();
        var bitboard = obj.AddComponent<BitboardManager>();

        foreach (int move in testCase.moves) {
            bitboard.PlayMove(move);
        }

        Object.Destroy(obj);
        return bitboard;
    }
    
    [UnityTest]
    public IEnumerator Test_PlayMove_SingleMove()
    {
        PlayMoveCase testCase = new PlayMoveCase
        {
            pos = 2097152,
            board = 2097152,
            moves = new List<int> { 3 }
        };

        BitboardManager bitboard = PlayMoveTest(testCase);
        yield return null;

        Assert.AreEqual(testCase.board, bitboard.GetBoard());
        Assert.AreEqual(testCase.pos, bitboard.GetPos());
        
    }

    [UnityTest]
    public IEnumerator Test_PlayMove_FullColumn()
    {
        PlayMoveCase testCase = new PlayMoveCase
        {
            pos = 88080384,
            board = 132120576,
            moves = new List<int> { 3, 3, 3, 3, 3, 3 }
        };

        BitboardManager bitboard = PlayMoveTest(testCase);
        yield return null;

        Assert.AreEqual(testCase.board, bitboard.GetBoard());
        Assert.AreEqual(testCase.pos, bitboard.GetPos());

    }

    [UnityTest]
    public IEnumerator Test_PlayMove_FullRow()
    {
        PlayMoveCase testCase = new PlayMoveCase
        {
            pos = 4398314962945,
            board = 4432676798593,
            moves = new List<int> { 0, 1, 2, 3, 4, 5, 6 }
        };

        BitboardManager bitboard = PlayMoveTest(testCase);
        yield return null;

        Assert.AreEqual(testCase.board, bitboard.GetBoard());
        Assert.AreEqual(testCase.pos, bitboard.GetPos());

    }

    [UnityTest]
    public IEnumerator Test_PlayMove_Diagonals()
    {
        PlayMoveCase testCase = new PlayMoveCase
        {
            pos = 1448756674817,
            board = 2173016588673,
            moves = new List<int> {0,1,1,2,2,2,3,3,3,3,4,4,4,4,4,5,5,5,5,5,5}
        };

        BitboardManager bitboard = PlayMoveTest(testCase);
        yield return null;

        Assert.AreEqual(testCase.board, bitboard.GetBoard());
        Assert.AreEqual(testCase.pos, bitboard.GetPos());

    }

    [UnityTest]
    public IEnumerator Test_PlayMove_FullBoard()
    {
        PlayMoveCase testCase = new PlayMoveCase
        {
            pos = 186172381500714,
            board = 279258638311359,
            moves = new List<int> {0,0,0,0,0,0,1,1,1,1,1,1,2,2,2,2,2,2,4,3,3,3,3,3,3,4,4,4,4,4,5,5,5,5,5,5,6,6,6,6,6,6}
        };

        BitboardManager bitboard = PlayMoveTest(testCase);
        yield return null;

        Assert.AreEqual(testCase.board, bitboard.GetBoard());
        Assert.AreEqual(testCase.pos, bitboard.GetPos());

    }
}
