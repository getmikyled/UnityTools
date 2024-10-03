using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.UIElements;

namespace GetMikyled.MEDialogue
{
    ///-//////////////////////////////////////////////////////////////////
    ///
    public class MEDialogueWindow : EditorWindow
    {
        private const string defaultFileName = "DialogueFile";

        private static MEDGraphView _medGraphView;

        ///-//////////////////////////////////////////////////////////////////
        ///
        [MenuItem("Window/Dialog Graph")]
        public static void OpenFromMenu()
        {
            GetWindow<MEDialogueWindow>("MEDialogue");
        }
        
        ///-//////////////////////////////////////////////////////////////////
        ///
        public static void OpenFromFile(MEDialogueGraph soMeDialogueGraph)
        {
            GetWindow<MEDialogueWindow>("MEDialogue");  
            if (soMeDialogueGraph != null)
            {
                MEDIOUtility.LoadFile(_medGraphView, soMeDialogueGraph);
            }
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        private void OnEnable()
        {
            AddGraphView();
            AddToolbar();
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        private void AddGraphView()
        {
            _medGraphView = new MEDGraphView();
            _medGraphView.StretchToParentSize();

            rootVisualElement.Add(_medGraphView);
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            // Toolbar Contents
            TextField fileName = MEDialogueElementUtility.CreateTextField(defaultFileName, "File Name: ");
            Button saveButton = MEDialogueElementUtility.CreateButton("Save", () =>
            {
                MEDIOUtility.InitializeSave(_medGraphView, fileName.value);
            });

            toolbar.Add(fileName);
            toolbar.Add(saveButton);

            rootVisualElement.Add(toolbar);
        }
    }
}

#endif // UNITY_EDITOR
