using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "CoreValueGame", menuName = "Core Values/Game", order = 1)]
public class CoreValueGame : ScriptableObject
{
    [Header("Game Details")]
    public string gameTitle;
    public string sceneName;

    [Header("Game State Panels")]
    [TextArea(3,10)]
    public string gameBeginParagraph;
    [TextArea(3, 10)]
    public string gameEndParagraph;
    [TextArea(3, 10)]
    public string gameLoseParagraph;

    [Header("Instructions Panel")]
    public Sprite[] controlsImage;
    public string[] controlsText;
    [TextArea(3, 10)]
    public string instructionsParagraph;

    
}
