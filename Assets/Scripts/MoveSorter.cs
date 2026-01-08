using UnityEngine;

public class MoveSorter
{
    private int size = 0;

    private struct Entry
    {
        public ulong move;
        public int score;
    }

    private readonly Entry[] entries = new Entry[Connect4PositionMB.WIDTH];

    public void Reset() => size = 0;

    public void Add(ulong move, int score)
    {
        int pos = size++;
        while (pos > 0 && entries[pos - 1].score > score) {
            entries[pos] = entries[pos - 1];
            pos--;
        }

        entries[pos].move = move;
        entries[pos].score = score;
    }

    public ulong GetNext()
    {
        if (size == 0) return 0;
        return entries[--size].move;
    }
}

