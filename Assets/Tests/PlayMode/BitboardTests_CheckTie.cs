using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class BitboardTests_CheckTie
{
    struct CheckTieCase
    {
        public List<ulong> cases;
        public List<bool> expected;
    }

    private List<bool> CheckTies(List<ulong> cases)
    {
        var obj = new GameObject();
        var bitboard = obj.AddComponent<BitboardManager>();
        bitboard.UpdateTieCheckMask();
        List<bool> outcomes = new List<bool>();

        foreach (ulong board in cases) {
            outcomes.Add(bitboard.CheckTie(board));
        }

        return outcomes;
    }

    [UnityTest]
    public IEnumerator Test_CheckTie_FalseTies()
    {
        CheckTieCase rowsCase = new CheckTieCase
        {
            cases = new List<ulong> { 4917500575744, 31028737590151, 31572865 }, 
            expected = new List<bool> { false, false, false }
        };

        List<bool> outcomes = CheckTies(rowsCase.cases);
        yield return null;

        Assert.AreEqual(rowsCase.expected, outcomes);

    }

    [UnityTest]
    public IEnumerator Test_CheckTie_AlmostTies()
    {
        CheckTieCase rowsCase = new CheckTieCase
        {
            cases = new List<ulong> { 138521149956031, 279258638311327 }, 
            expected = new List<bool> { false, false }
        };

        List<bool> outcomes = CheckTies(rowsCase.cases);
        yield return null;

        Assert.AreEqual(rowsCase.expected, outcomes);

    }

    [UnityTest]
    public IEnumerator Test_CheckTie_FullBoards()
    {
        CheckTieCase rowsCase = new CheckTieCase
        {
            cases = new List<ulong> { 279258638311359 }, 
            expected = new List<bool> { true }
        };

        List<bool> outcomes = CheckTies(rowsCase.cases);
        yield return null;

        Assert.AreEqual(rowsCase.expected, outcomes);

    }
}
