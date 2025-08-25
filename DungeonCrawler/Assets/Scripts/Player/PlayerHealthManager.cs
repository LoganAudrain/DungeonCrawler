using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManaManager : MonoBehaviour
{
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    public Image[] stars;
    public Sprite fullStar;
    public Sprite halfStar;
    public Sprite emptyStar;

    public CharacterStats characterStats;

    // Overlay threshold
    public int overlayStepHealth = 100; 
    public int overlayStepMana = 100;  

    // Progressive overlay colors (like Minecraft)
    public Color[] overlayColors = new Color[]
    {
        Color.yellow, // 101–200
        Color.green,  // 201–300
        Color.blue,   // 301–400
        new Color(0.6f, 0f, 1f), // purple for 401–500
        Color.red     // fallback for >500
    };

    private Color defaultHeartColor = Color.white;
    private Color defaultStarColor = Color.white;

    void Start()
    {
        if (characterStats == null)
            characterStats = GetComponent<CharacterStats>();

        UpdateHearts();
        UpdateStars();
    }

    void Update()
    {
        UpdateHearts();
        UpdateStars();
    }

    private void UpdateStars()
    {
        if (stars == null || characterStats == null)
            return;

        int currentMana = characterStats.GetCurrentMana;
        int maxMana = characterStats.GetMaxMana;

        int numStars = Mathf.Min(stars.Length, Mathf.CeilToInt(maxMana / 10f));

        // Overlay tier and color should be based on currentMana, not maxMana
        int overlayTier = (currentMana - 1) / overlayStepMana;
        int overlayStart = overlayStepMana * overlayTier + 1;

        int starsToOverlay = 0;
        if (overlayTier > 0)
        {
            starsToOverlay = Mathf.Clamp((currentMana - overlayStart) / 10 + 1, 0, numStars);
        }

        Color overlayColor = overlayTier > 0 ? GetOverlayColor(currentMana, overlayStepMana, defaultStarColor) : defaultStarColor;

        int lastOverlayStarIndex = starsToOverlay - 1;

        for (int i = 0; i < stars.Length; i++)
        {
            if (i < numStars)
            {
                stars[i].enabled = true;
                int manaForThisStar = currentMana - (i * 10);

                if (i < starsToOverlay - 1)
                {
                    // All overlay stars except the last: always full and overlay color
                    stars[i].sprite = fullStar;
                    stars[i].color = overlayColor;
                }
                else if (i == lastOverlayStarIndex && starsToOverlay > 0)
                {
                    // The last overlay star: reflect mana status and overlay color
                    if (manaForThisStar >= 10)
                        stars[i].sprite = fullStar;
                    else if (manaForThisStar >= 5)
                        stars[i].sprite = halfStar;
                    else
                        stars[i].sprite = emptyStar;
                    stars[i].color = overlayColor;
                }
                else
                {
                    // Non-overlay stars: normal color and status
                    if (manaForThisStar >= 10)
                        stars[i].sprite = fullStar;
                    else if (manaForThisStar >= 5)
                        stars[i].sprite = halfStar;
                    else
                        stars[i].sprite = emptyStar;
                    stars[i].color = defaultStarColor;
                }
            }
            else
            {
                stars[i].enabled = false;
            }
        }
    }

    private void UpdateHearts()
    {
        if (hearts == null || characterStats == null)
            return;

        int currentHealth = characterStats.GetCurrentHealth;
        int maxHealth = characterStats.GetMaxHealth;

        int numHearts = Mathf.Min(hearts.Length, Mathf.CeilToInt(maxHealth / 10f));

        // Overlay tier and color should be based on currentHealth, not maxHealth
        int overlayTier = (currentHealth - 1) / overlayStepHealth;
        int overlayStart = overlayStepHealth * overlayTier + 1;

        int heartsToOverlay = 0;
        if (overlayTier > 0)
        {
            heartsToOverlay = Mathf.Clamp((currentHealth - overlayStart) / 10 + 1, 0, numHearts);
        }

        Color overlayColor = overlayTier > 0 ? GetOverlayColor(currentHealth, overlayStepHealth, defaultHeartColor) : defaultHeartColor;

        int lastOverlayHeartIndex = heartsToOverlay - 1;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < numHearts)
            {
                hearts[i].enabled = true;
                int healthForThisHeart = currentHealth - (i * 10);

                if (i < heartsToOverlay - 1)
                {
                    // All overlay hearts except the last: always full and overlay color
                    hearts[i].sprite = fullHeart;
                    hearts[i].color = overlayColor;
                }
                else if (i == lastOverlayHeartIndex && heartsToOverlay > 0)
                {
                    // The last overlay heart: reflect health status and overlay color
                    if (healthForThisHeart >= 10)
                        hearts[i].sprite = fullHeart;
                    else if (healthForThisHeart >= 5)
                        hearts[i].sprite = halfHeart;
                    else
                        hearts[i].sprite = emptyHeart;
                    hearts[i].color = overlayColor;
                }
                else
                {
                    // Non-overlay hearts: normal color and status
                    if (healthForThisHeart >= 10)
                        hearts[i].sprite = fullHeart;
                    else if (healthForThisHeart >= 5)
                        hearts[i].sprite = halfHeart;
                    else
                        hearts[i].sprite = emptyHeart;
                    hearts[i].color = defaultHeartColor;
                }
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
    /// Returns overlay color based on how many steps above the threshold the max value is.
    /// </summary>
    private Color GetOverlayColor(int maxValue, int stepSize, Color defaultColor)
    {
        if (maxValue <= stepSize)
            return defaultColor;

        int stepIndex = (maxValue - 1) / stepSize - 1; // 101–200 = index 0, 201–300 = index 1, etc.

        if (stepIndex >= 0 && stepIndex < overlayColors.Length)
            return overlayColors[stepIndex];

        // Fallback color if above defined tiers
        return overlayColors[overlayColors.Length - 1];
    }
}
