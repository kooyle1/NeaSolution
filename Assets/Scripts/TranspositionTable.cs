using UnityEngine;
using System.Collections.Generic;

public class TranspositionTable : MonoBehaviour
{
    
    private Dictionary<(ulong pos, ulong board), TTEntry> table = new Dictionary<(ulong pos, ulong board), TTEntry>();
    
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

}
