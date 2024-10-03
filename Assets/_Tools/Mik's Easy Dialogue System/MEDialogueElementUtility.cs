using System;
using UnityEngine.UIElements;

namespace GetMikyled.MEDialogue
{
    ///-//////////////////////////////////////////////////////////////////
    ///
    public class MEDialogueElementUtility
    {
        ///-//////////////////////////////////////////////////////////////////
        ///
        public static Button CreateButton(string argText, Action onClicked)
        {
            Button button = new Button()
            {
                text = argText
            };
            button.clicked += onClicked;

            return button;
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        public static Label CreateLabel(string argLabel = null)
        {
            Label label = new Label()
            {
                text = argLabel
            };

            return label;
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        public static TextField CreateTextField(string argValue = null, string argLabel = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            // Set Properties
            TextField textField = new TextField()
            {
                label = argLabel,
                value = argValue
            };

            textField[0].style.fontSize = 15;
            textField[0].style.marginTop = 6;

            // Register Callback for onValueChanged
            if (onValueChanged != null)
            {
                textField.RegisterValueChangedCallback(onValueChanged);
            }

            return textField;
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        public static TextField CreateTextArea(string argValue = null, string argLabel = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textArea = CreateTextField(argValue, argLabel, onValueChanged);
            textArea.multiline = true;

            return textArea;
        }
    }
}