using System;
using ExtInspector;
using UnityEngine;

namespace Samples
{
    public class ButtonsExample : MonoBehaviour
    {
        [SerializeField] private bool _errorOut;

        [field: SerializeField] private string _labelByField;

        [SerializeField]
        [AboveButton(nameof(ClickButton), "Click <color=green><icon='eye-regular.png' /></color>!", false, "")]
        [AboveButton(nameof(ClickButton2), nameof(GetButtonLabel), true, "OK")]
        [AboveButton(nameof(ClickButton), "Click <color=green><icon='eye-regular.png' /></color>!", false, "")]
        [AboveButton(nameof(ClickButton2), nameof(GetButtonLabel), true, "OK")]

        [BelowButton(nameof(ClickButton2), nameof(GetButtonLabel), true, "OK")]
        [BelowButton(nameof(ClickButton), "Below <color=green><icon='eye-regular.png' /></color>!", false)]
        [BelowButton(nameof(ClickButton2), nameof(GetButtonLabel), true, "OK")]
        [BelowButton(nameof(ClickButton), "Below <color=green><icon='eye-regular.png' /></color>!", false)]
        [BelowButton(nameof(ClickButton), nameof(_labelByField), true)]

        [PostFieldButton(nameof(ToggleAndError), nameof(GetButtonLabelIcon), true)]
        [Range(0, 10)]
        private int _someInt;

        private void ClickButton()
        {
            Debug.Log("CLICKED!");
        }

        private string GetButtonLabel()
        {
            return _errorOut
                ? "Error <color=red>me</color>!"
                : "No <color=green>Error</color>!";
        }

        private string GetButtonLabelIcon() => _errorOut
            ? "<color=red><icon='eye-regular.png' /></color>"
            : "<color=green><icon='eye-regular.png' /></color>";

        private void ClickButton2()
        {
            Debug.Log("CLICKED 2!");
            if(_errorOut)
            {
                throw new Exception("Expected exception!");
            }
        }

        private void ToggleAndError()
        {
            Toggle();
            ClickButton2();
        }

        private void Toggle()
        {
            _errorOut = !_errorOut;
        }
    }
}
