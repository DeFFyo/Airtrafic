using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelList", menuName = "Game/Level List")]
public class LevelList : ScriptableObject
{
    public List<LevelDefinition> levels = new List<LevelDefinition>();
}
