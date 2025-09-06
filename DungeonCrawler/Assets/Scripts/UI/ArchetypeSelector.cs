using UnityEngine;

[System.Serializable]
public class PlayerArchetypeDefaults
{
    [Header("Prefab")]
    public string archetype;
    public GameObject prefab;

    [Space(5)]
    [TextArea]
    public string description;

    [Space(10)]
    [Header("Core Stats")]
    public int Health = 10;
    public int Mana = 10;

    [Space(10)]
    [Header("Attributes")]
    public int Strength = 10;
    public int Dexterity = 10;
    public int Constitution = 10;
    public int Intelligence = 10;
}
public class ArchetypeSelector : MonoBehaviour
{
    public CharacterStats newPlayerStats;
    public PlayerArchetypeDefaults[] archetypeStats;

    public void AssignArchetypeStats(string chosenArchetype)
    {
        for(int i = 0; i < archetypeStats.Length; i++)
        {
            if (archetypeStats[i].archetype == chosenArchetype)
            {
                newPlayerStats.SetPlayerCharacterStats(archetypeStats[i]);
                break;
            }
        }
    }
}
