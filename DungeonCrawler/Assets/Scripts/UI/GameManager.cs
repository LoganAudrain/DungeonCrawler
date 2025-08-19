using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject interactText;

    public void ShowinteractText()
    {
        if (interactText != null)
            interactText.SetActive(true);
    }

    public void HideinteractText()
    {
        if (interactText != null)
            interactText.SetActive(false);
    }
}
