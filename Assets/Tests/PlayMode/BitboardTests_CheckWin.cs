using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class BitboardTests_CheckWin
{
    
    struct CheckWinCase
    {
        public List<ulong> cases;
        public List<bool> expected;
    }

    private List<bool> CheckWins(List<ulong> cases)
    {
        var obj = new GameObject();
        var bitboard = obj.AddComponent<BitboardManager>();
        List<bool> outcomes = new List<bool>();

        foreach (ulong pos in cases) {
            outcomes.Add(bitboard.CheckWin(pos));
        }

        return outcomes;
    }
    
    [UnityTest]
    public IEnumerator Test_CheckWin_Horizontal()
    {
        CheckWinCase rowsCase = new CheckWinCase
        {
            cases = new List<ulong> { 270549120, 4432676782080, 2113665, 541097984, 186172337460522},
            expected = new List<bool> { true, true, true, false, false }
        };

        List<bool> outcomes = CheckWins(rowsCase.cases);
        yield return null;

        Assert.AreEqual(rowsCase.expected, outcomes);

    }

    [UnityTest]
    public IEnumerator Test_CheckWin_Vertical()
    {
        CheckWinCase rowsCase = new CheckWinCase
        {
            cases = new List<ulong> { 31457280, 65970697666560, 15, 1879048192, 21992111718407},
            expected = new List<bool> { true, true, true, false, false }
        };

        List<bool> outcomes = CheckWins(rowsCase.cases);
        yield return null;

        Assert.AreEqual(rowsCase.expected, outcomes);

    }

    [UnityTest]
    public IEnumerator Test_CheckWin_DiagonalUp()
    {
        CheckWinCase rowsCase = new CheckWinCase
        {
            cases = new List<ulong> { 21053697, 44152802770944, 22128487334016},
            expected = new List<bool> { true, true, false }
        };

        List<bool> outcomes = CheckWins(rowsCase.cases);
        yield return null;

        Assert.AreEqual(rowsCase.expected, outcomes);

    }

    [UnityTest]
    public IEnumerator Test_CheckWin_DiagonalDown()
    {
        CheckWinCase rowsCase = new CheckWinCase
        {
            cases = new List<ulong> { 2130442, 4467860701184, 35177709829},
            expected = new List<bool> { true, true, false }
        };

        List<bool> outcomes = CheckWins(rowsCase.cases);
        yield return null;

        Assert.AreEqual(rowsCase.expected, outcomes);

    }
}
