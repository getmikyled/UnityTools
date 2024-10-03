using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using System;
using UnityEditor.Experimental.GraphView;

namespace GetMikyled.MEDialogue
{
    ///-//////////////////////////////////////////////////////////////////
    ///
    public class StartNode : MEDNodeBase
    {
        public string conversationName;
        
        ///-//////////////////////////////////////////////////////////////////= 
        ///
        public StartNode(Rect argPosition) : base(argPosition)
        {
            conversationName = "Conversation";
            
            InitializeClassList();
        }
        
        ///-//////////////////////////////////////////////////////////////////= 
        ///

        ///-//////////////////////////////////////////////////////////////////= 
        ///
        protected override void InitializeClassList()
        {
            mainContainer.AddToClassList("start-node__main-container");
            titleContainer.AddToClassList("start-node__title-container");
            extensionContainer.AddToClassList("start-node__extension-container");
            
            // Manually set backgroundColor, style sheets do not work (UNKNOWN REASON)
            titleContainer.style.backgroundColor = new Color(89 / 255f, 66 / 255f, 66 / 255f);
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        public override void Draw()
        {
            // Title Container
            TextField conversationNameField = MEDialogueElementUtility.CreateTextField(conversationName, "Start Node", 
                (value) =>
            {
                conversationName = value.newValue;
            });
            conversationNameField.AddToClassList("start-node__textfield");
            titleContainer.Insert(0, conversationNameField);
            
            RefreshExpandedState();
        }
    }
}
#endif // UNITY_EDITOR