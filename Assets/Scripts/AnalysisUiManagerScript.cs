using UnityEngine;
using UnityEngine.UI;

public class AnalysisUiManagerScript : BaseUiManagerScript
{
    [Header("Gameobject References")]
    [SerializeField] private Button buttonExample;
    [SerializeField] private GameObject scrollviewContent;

    //Testing
    protected override void Start()
    {
        for (int i = 0; i < 10; i++) {
            UpdateScrollView(buttonExample, i);
        }
        base.Start();
    }

    //Testing
    public void UpdateScrollView(Button button, int num)
    {
        button.name = num.ToString();
        Instantiate(button, scrollviewContent.transform);
    }
}
