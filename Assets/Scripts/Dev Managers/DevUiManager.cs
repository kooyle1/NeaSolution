using UnityEngine;
using TMPro;

public class DevUiManager : BaseUiManager
{
    [SerializeField] private TMP_Text averageText;

    protected override void Start()
    {
        return;
    }

    public void SetAverage(double time, int nodes)
    {
        string text = $"AVERAGE TIME(ms): {time.ToString()}, AVERAGE EXPLORED NODES: {nodes.ToString()} ";
        averageText.text = text; 
    }
}
