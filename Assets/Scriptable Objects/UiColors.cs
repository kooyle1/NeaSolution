using UnityEngine;

[CreateAssetMenu(fileName = "UiColors", menuName = "Scriptable Objects/UiColors")]
public class UiColors : ScriptableObject
{
    [Header("Colors")]
    public Color buttonColor;
    public Color backgroundColor;
    public Color outlineColor;
    public Color textColor;

}
