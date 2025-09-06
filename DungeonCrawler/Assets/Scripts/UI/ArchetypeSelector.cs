using TMPro;
using UnityEngine;

[System.Serializable]
public class PlayerArchetypeDefaults
{
    [Header("Visual")]
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
    [Header("Player")]
    public GameObject NewPlayer;
    public CharacterStats newPlayerStats;

    [Space(10)]
    public PlayerArchetypeDefaults[] archetypeStats;

    [Space(10)]
    [Header("Archetype UI")]
    public TextMeshProUGUI archetypeDescriptionUI;

    public TextMeshProUGUI Health;
    public TextMeshProUGUI Mana;
    public TextMeshProUGUI Strength;
    public TextMeshProUGUI Dexterity;
    public TextMeshProUGUI Constitution;
    public TextMeshProUGUI Intelligence;

    public void Start()
    {
        if (NewPlayer == null)
        {
            Debug.LogError("NewPlayer is null! Make sure it is assigned in the inspector or created before use.");
        }
        else
        {
            newPlayerStats = NewPlayer.GetComponent<CharacterStats>();

            if (newPlayerStats == null)
            {
                Debug.LogError("CharacterStats component not found on " + NewPlayer.name + "!");
            }
        }

    }

    public void AssignArchetypeStats(string chosenArchetype)
    {
        for(int i = 0; i < archetypeStats.Length; i++)
        {
            if (archetypeStats[i].archetype == chosenArchetype)
            {
                newPlayerStats.SetPlayerCharacterStats(archetypeStats[i]);
                UpdateUIElements(archetypeStats[i]);
                break;
            }
        }
    }

    private void UpdateUIElements(PlayerArchetypeDefaults stats)
    {
        archetypeDescriptionUI.text = stats.description;

        Health.text = stats.Health.ToString();
        Mana.text = stats.Mana.ToString();

        Strength.text = stats.Strength.ToString();
        Dexterity.text = stats.Dexterity.ToString();
        Constitution.text = stats.Constitution.ToString();
        Intelligence.text = stats.Intelligence.ToString();

        SpriteRenderer newPlayerRenderer = NewPlayer.GetComponent<SpriteRenderer>();
        Animator newPlayerAnim = NewPlayer.GetComponent<Animator>();

        SpriteRenderer archPrefabRenderer = stats.prefab.GetComponent<SpriteRenderer>();
        Animator archPrefabAnim = stats.prefab.GetComponent<Animator>();

        if (newPlayerRenderer != null && archPrefabRenderer != null)
        {
            newPlayerRenderer.sprite = archPrefabRenderer.sprite;
        }
        else
            Debug.LogError("Be sure that both the New Player game obj and the archetype prefab have sprite renderers");

        if (newPlayerAnim != null && archPrefabAnim != null)
        {
            newPlayerAnim.runtimeAnimatorController = archPrefabAnim.runtimeAnimatorController;
        }
        else
            Debug.LogError("Be sure that both the New Player game obj and the archetype prefab have Animators");
    }
}
