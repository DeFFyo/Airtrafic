using UnityEngine;

[CreateAssetMenu(fileName = "LevelDefinition", menuName = "Game/Level Definition")]
public class LevelDefinition : ScriptableObject
{
    public string levelName;
    public Sprite icon;
    public string sceneName;
}
