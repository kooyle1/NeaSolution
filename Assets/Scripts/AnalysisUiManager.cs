using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AnalysisUiManager : BaseUiManager
{
    [Header("References")]
    [SerializeField] private Button buttonExample;
    [SerializeField] private GameObject scrollviewContent;

    protected override void Start()
    {
        for (int i = 0; i < 10; i++) {
            UpdateScrollView(buttonExample, i);
        }
        base.Start();
    }

    public void UpdateScrollView(Button button, int num)
    {
        button.name = num.ToString();
        Instantiate(button, scrollviewContent.transform);
    }
}
