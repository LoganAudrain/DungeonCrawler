using UnityEngine;
using UnityEngine.UI;

public class ExclusiveTogglePair : MonoBehaviour
{
    [SerializeField] private Toggle toggleA;
    [SerializeField] private Toggle toggleB;

    private void Start()
    {
        if (!toggleA.isOn && !toggleB.isOn)
        {
            toggleA.isOn = true;
        }

        toggleA.onValueChanged.AddListener(OnToggleAChanged);
        toggleB.onValueChanged.AddListener(OnToggleBChanged);
    }

    private void OnToggleAChanged(bool isOn)
    {
        if (isOn)
        {
            toggleB.isOn = false;
        }
        else if (!toggleB.isOn)
        {
            toggleB.isOn = true;
        }
    }

    private void OnToggleBChanged(bool isOn)
    {
        if (isOn)
        {
            toggleA.isOn = false;
        }
        else if (!toggleA.isOn)
        {
            toggleA.isOn = true;
        }
    }
}
