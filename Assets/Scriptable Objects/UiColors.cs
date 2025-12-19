using UnityEngine;

[CreateAssetMenu(fileName = "UiColors", menuName = "Scriptable Objects/UiColors")]
public class UiColors : ScriptableObject
{
    [Header("Settings")]
    public Color buttonColor;
    public Color backgroundColor;
    public Color outlineColor;
    public Color textColor;

}
