using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class BitboardTests_CanPlayColumn
{
    private bool TestColumn(int column)
    {
        var obj = new GameObject();
        var bitboard = obj.AddComponent<BitboardManager>();

        ulong board = 278146123350144;
        return bitboard.CanPlayColumn(column, board);
    }

    [UnityTest]
    public IEnumerator Test_CanPlayColumn_Empty()
    {
        bool canPlay = TestColumn(0);
        yield return null;
        Assert.AreEqual(true, canPlay);
    }

    [UnityTest]
    public IEnumerator Test_CanPlayColumn_Height1()
    {
        bool canPlay = TestColumn(1);
        yield return null;
        Assert.AreEqual(true, canPlay);
    }

    [UnityTest]
    public IEnumerator Test_CanPlayColumn_Height2()
    {
        bool canPlay = TestColumn(2);
        yield return null;
        Assert.AreEqual(true, canPlay);
    }

    [UnityTest]
    public IEnumerator Test_CanPlayColumn_Height3()
    {
        bool canPlay = TestColumn(3);
        yield return null;
        Assert.AreEqual(true, canPlay);
    }

    [UnityTest]
    public IEnumerator Test_CanPlayColumn_Height4()
    {
        bool canPlay = TestColumn(4);
        yield return null;
        Assert.AreEqual(true, canPlay);
    }

    [UnityTest]
    public IEnumerator Test_CanPlayColumn_Height5()
    {
        bool canPlay = TestColumn(5);
        yield return null;
        Assert.AreEqual(true, canPlay);
    }

    [UnityTest]
    public IEnumerator Test_CanPlayColumn_Height6()
    {
        bool canPlay = TestColumn(6);
        yield return null;
        Assert.AreEqual(false, canPlay);
    }
}
