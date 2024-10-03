using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using System;
using UnityEditor.Experimental.GraphView;

namespace GetMikyled.MEDialogue
{
    ///-//////////////////////////////////////////////////////////////////
    ///
    public class DialogueNode : MEDNodeBase
    {

        public string dialogueName;
        public Dictionary<string, string> choices;  // Port GUID -> Choice Name
        public string dialogueText;
        
        ///-//////////////////////////////////////////////////////////////////
        ///
        public DialogueNode(Rect argPosition) : base(argPosition)
        {
            dialogueName = "Dialogue Name";
            choices = new Dictionary<string, string>();
            dialogueText = "Insert Dialogue Text";

            InitializeClassList();
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        protected override void InitializeClassList()
        {
            mainContainer.AddToClassList("dialogue-node__main-container");
            extensionContainer.AddToClassList("dialogue-node__extension-container");
        }
        
        ///-//////////////////////////////////////////////////////////////////
        ///
        public override void Draw()
        {
            // TITLE CONTAINER

            TextField dialogueNameTextField = MEDialogueElementUtility.CreateTextField(dialogueName, null, (newText) =>
            {
                dialogueName = newText.newValue;
            });
            dialogueNameTextField.AddToClassList("dialogue-node__textfield");
            titleContainer.Insert(0, dialogueNameTextField);

            // MAIN CONTAINER
            Button addChoiceButton = MEDialogueElementUtility.CreateButton("Add Choice", () =>
            {
                string choiceGUID = Guid.NewGuid().ToString();
                choices.Add(choiceGUID, "New Choice");
                AddChoice(choiceGUID);
            });

            mainContainer.Insert(1, addChoiceButton);

            // PORT CONTAINER
            
            // output
            foreach (string choiceGUID in choices.Keys)
            {
                AddChoice(choiceGUID);
            }

            // EXTENSION CONTAINER
            // Contains Dialogue Text
            VisualElement textContainer = new VisualElement();
            extensionContainer.Add(textContainer);

            // Text Foldout
            Foldout textFoldout = new Foldout()
            {
                text = "Dialogue Text"
            };
            TextField textTextField = MEDialogueElementUtility.CreateTextArea(dialogueText, null, (newText) =>
            {
                dialogueText = newText.newValue;
            });
            textFoldout.Add(textTextField);
            textContainer.Add(textFoldout);

            RefreshExpandedState();
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        private void AddChoice(string choiceGUID)
        {
            // Create Choice UI //
            MEDPort choicePort = CreateOutputPort(choiceGUID);
            
            // Delete Button -> Deletes the choice
            Button deleteChoiceButton = new Button() { text = "X" };
            
            // TextField contains ChoiceName
            TextField choiceTextField = MEDialogueElementUtility.CreateTextField(choices[choiceGUID], null, (value) =>
            {
                choices[choiceGUID] = value.newValue;
            });
            choiceTextField.style.flexDirection = FlexDirection.Column;
            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);
            
            outputContainer.Add(choicePort);

            RefreshExpandedState();
            RefreshPorts();
        }
    }
}

#endif // UNITY_EDITOR