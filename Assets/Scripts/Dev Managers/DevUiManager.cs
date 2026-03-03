using UnityEngine;
using TMPro;

public class DevUiManager : BaseUiManager
{
    [SerializeField] private TMP_Text averageText;

    protected override void Start()
    {
        return; //No need to update UI since users cant access this scene
    }

    /// <summary>
    ///  Displays inputted average time and nodes.
    /// </summary>
    public void SetAverage(double time, int nodes)
    {
        string text = $"AVERAGE TIME(ms): {time.ToString()}, AVERAGE EXPLORED NODES: {nodes.ToString()} ";
        averageText.text = text; 
    }
}
