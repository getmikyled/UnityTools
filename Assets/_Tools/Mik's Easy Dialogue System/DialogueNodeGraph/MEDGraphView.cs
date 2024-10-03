using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEngine.TextCore.Text;

namespace GetMikyled.MEDialogue
{
    ///-//////////////////////////////////////////////////////////////////
    ///
    public class MEDGraphView : GraphView
    {
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatibleports = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port || startPort.node == port.node || port.direction == startPort.direction)
                {
                    return;
                }

                compatibleports.Add(port);
            });

            return compatibleports;
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        public MEDGraphView()
        {
            AddWindowManipulators();
            AddGridBackground();
            CreateBlackboards();
            StylizeUI();
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        private void StylizeUI()
        {
            StyleSheet graphStyleSheet = (StyleSheet)Resources.Load("DialogueGraphStyleSheet");
            StyleSheet nodeStyleSheet = (StyleSheet)Resources.Load("DialogueNodeStyleSheet");
            styleSheets.Add(graphStyleSheet);
            styleSheets.Add(nodeStyleSheet);
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        public void ClearGraphElements()
        {
            graphElements.ForEach(element => RemoveElement(element));
        }

        #region Manipulators

        ///-//////////////////////////////////////////////////////////////////
        ///
        private void AddWindowManipulators()
        {
            SetupZoom(minScale, maxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(CreateDialogueContextualMenu());
            this.AddManipulator(CreateStartContextualMenu());
        }
        
        ///-//////////////////////////////////////////////////////////////////
        ///
        private IManipulator CreateStartContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Start Node",
                    actionEvent => AddElement(CreateStartNode(actionEvent.eventInfo.mousePosition)))
            );

            return contextualMenuManipulator;
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        private IManipulator CreateDialogueContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Dialogue Node",
                    actionEvent => AddElement(CreateDialogueNode(actionEvent.eventInfo.mousePosition)))
            );

            return contextualMenuManipulator;
        }

#endregion // Manipulators
        
#region Graph Element Creation

        ///-//////////////////////////////////////////////////////////////////
        ///
        public void CreateBlackboards()
        {
            CharacterOutliner characterOutliner = new CharacterOutliner(this);
            Add(characterOutliner);
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        public Edge CreateEdge(MEDPort argInput, MEDPort argOutput)
        {
            Edge edge = new Edge()
            {
                input = argInput,
                output = argOutput
            };
            
            edge.input.Connect(edge);
            edge.output.Connect(edge);
            AddElement(edge);

            return edge;
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        public StartNode CreateStartNode(Vector2 argPos)
        {
            // Create new Start Node
            StartNode startNode = new StartNode(new Rect(argPos, Vector3.zero));
            startNode.CreateOutputPort(Guid.NewGuid().ToString(), "Next Node");
            
            startNode.Draw();

            AddElement(startNode);
            return startNode;
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        public StartNode CreateStartNode(StartNodeSaveData sNodeSaveData)
        {
            // Create new Start Node
            StartNode startNode = new StartNode(new Rect(sNodeSaveData.position, Vector3.zero))
            {
                // Update Node Values w/ Save Data
                GUID = sNodeSaveData.GUID,
                conversationName = sNodeSaveData.conversationName
            };
            startNode.CreateOutputPort(sNodeSaveData.outputPorts[0].portGUID, "Next Node");

            startNode.Draw();

            AddElement(startNode);
            return startNode;
        }

        ///-//////////////////////////////////////////////////////////////////
        /// 
        public DialogueNode CreateDialogueNode(Vector2 argPos)
        {
            DialogueNode dialogueNode = new DialogueNode(new Rect(argPos, Vector3.zero));
            dialogueNode.CreateInputPort(Guid.NewGuid().ToString(), "Input");
            dialogueNode.choices.Add(Guid.NewGuid().ToString(), "New Choice");

            dialogueNode.Draw();
            
            AddElement(dialogueNode);
            return dialogueNode;
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        public DialogueNode CreateDialogueNode(DialogueNodeSaveData dNodeSaveData)
        {
            // Create new dialogue node
            DialogueNode dialogueNode = new DialogueNode(new Rect(dNodeSaveData.position, Vector2.zero))
            {
                // Update Node Values w/ Save Data
                GUID = dNodeSaveData.GUID,
                dialogueName = dNodeSaveData.dialogueName,
                dialogueText = dNodeSaveData.dialogueText,
                choices = new Dictionary<string, string>()
            };

            // Create Input & Output Ports
            dialogueNode.CreateInputPort(dNodeSaveData.inputPorts[0].portGUID, "Input");
            // Add choices to dialogueNode from choiceSaveData
            foreach (ChoiceSaveData choiceSaveData in dNodeSaveData.choicePorts)
            {
                dialogueNode.choices.Add(choiceSaveData.portGUID, choiceSaveData.choiceName);
            }

            dialogueNode.Draw();
            
            AddElement(dialogueNode);
            return dialogueNode;
        }
    }
    
#endregion // Graph Element Creation
}

#endif // UNITY_EDITOR
