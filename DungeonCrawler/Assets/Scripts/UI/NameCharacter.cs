using UnityEngine;
using UnityEngine.UI;

public class NameCharacter : MonoBehaviour
{
    public CharacterStats character;
    public InputField inputField;
    public Text nameButtonText;

    public void NameMyCharacter()
    {
        character.characterName = inputField.text;

        if(character.characterName == "")
        {
            character.characterName = "John Doe";
        }

        nameButtonText.text = character.characterName;
    }
}
