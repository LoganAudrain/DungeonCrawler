using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite HalfHeart;
    public Sprite emptyHeart;
    public CharacterStats characterStats;
    void Start()
    {
        if (characterStats == null)
        {
            characterStats = GetComponent<CharacterStats>();
        }
        UpdateHearts();
    }

    void Update()
    {
        UpdateHearts();
    }

    private void UpdateHearts()
    {
        if (hearts == null || characterStats == null)
            return;

        int currentHealth = characterStats.GetCurrentHealth;
        int maxHealth = characterStats.GetMaxHealth;

        int numHearts = Mathf.CeilToInt(maxHealth / 10f);

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < numHearts)
            {
                hearts[i].enabled = true;
                int heartHealth = currentHealth - (i * 10);

                if (heartHealth >= 10)
                {
                    hearts[i].sprite = fullHeart;
                }
                else if (heartHealth >= 5)
                {
                    hearts[i].sprite = HalfHeart;
                }
                else
                {
                    hearts[i].sprite = emptyHeart;
                }
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
}
