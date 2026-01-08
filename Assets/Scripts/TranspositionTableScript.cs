using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TranspositionTableScript : MonoBehaviour
{

    private Dictionary<(ulong, ulong), TTEntry> table;
    private int capacity;

    public void SetSize(int _capacity)
    {
        capacity = _capacity;
        table = new Dictionary<(ulong, ulong), TTEntry>(capacity);
    }

    public void Clear()
    {
        table.Clear();
    }
    
    public enum TTFlag {
        EXACT,
        UPPERBOUND,
        LOWERBOUND
    }
    
    public struct TTEntry {
        public int value;
        public int depth;
        public TTFlag flag;
    }

    public void Store(ulong pos, ulong board, TTEntry entry)
    {
        if (table.Count >= capacity) {
            var oldestKey = table.Keys.First();
            table.Remove(oldestKey);
        }
        
        table[(pos, board)] = entry;
    }

    public bool TryLookup(ulong pos, ulong board, out TTEntry entry)
    {
        return table.TryGetValue((pos, board), out entry);
    }

}
