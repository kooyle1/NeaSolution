using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "BoardColors", menuName = "Scriptable Objects/BoardColors")]
public class BoardColors : ScriptableObject
{
    public Color boardColor;
    public Color buttonColor;
    public Color previewRedColor;
    public Color previewYellowColor;
    public Color fullRedColor;
    public Color fullYellowColor;

}
