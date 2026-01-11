using UnityEngine;

[CreateAssetMenu(fileName = "BoardColors", menuName = "Scriptable Objects/BoardColors")]
public class BoardColors : ScriptableObject
{
    [Header("Settings")]
    public Color boardColor;
    public Color buttonColor;
    public Color boardOutlineColor;
    public Color previewRedColor;
    public Color previewYellowColor;
    public Color fullRedColor;
    public Color fullYellowColor;

}
