using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelColors", menuName = "ScriptableObjects/LevelColors", order = 1)]
public class LevelColors : ScriptableObject
{
    [SerializeField] public Material[] LvlColors;

}
