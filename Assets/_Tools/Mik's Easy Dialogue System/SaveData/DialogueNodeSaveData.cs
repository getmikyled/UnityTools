using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

#if UNITY_EDITOR

using UnityEditor.Experimental.GraphView;

namespace GetMikyled.MEDialogue 
{
    ///-//////////////////////////////////////////////////////////////////
    ///
    [Serializable]
    public class DialogueNodeSaveData : MEDNodeSaveDataBase
    {
        public string dialogueName;
        public string dialogueText;

        public List<ChoiceSaveData> choicePorts;

        ///-//////////////////////////////////////////////////////////////////
        ///
        public void Initialize(DialogueNode dNode)
        {
            GUID = dNode.GUID;
            dialogueName = dNode.dialogueName;
            dialogueText = dNode.dialogueText;
            position = dNode.GetPosition().position;
            
            SavePortsData(dNode);
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        protected override void SavePortsData(MEDNodeBase n)
        {
            DialogueNode dNode = (DialogueNode)n;
            inputPorts = new List<PortSaveData>();
            choicePorts = new List<ChoiceSaveData>();
            
            // Save input port information
            foreach (VisualElement element in dNode.inputContainer.Children())
            {
                if (element is MEDPort inputPort)
                {
                    // Create port save data
                    PortSaveData portSaveData = new PortSaveData()
                    {
                        portGUID = inputPort.GUID,
                        nodeGUID = dNode.GUID,
                    };
                    
                    // Add connections
                    foreach (Edge edge in inputPort.connections)
                    {
                        portSaveData.connectedNodeGUIDs.Add(((MEDNodeBase)edge.output.node).GUID);
                    }
                    inputPorts.Add(portSaveData);
                }
            }
            
            // Save output port information
            foreach (VisualElement element in dNode.outputContainer.Children())
            {
                if (element is MEDPort outputPort)
                {
                    // Create port save data
                    ChoiceSaveData choiceSaveData = new ChoiceSaveData()
                    {
                        portGUID = outputPort.GUID,
                        nodeGUID = dNode.GUID,
                        choiceName = dNode.choices[outputPort.GUID]
                    };
                    
                    // Add connections
                    foreach (Edge edge in outputPort.connections)
                    {
                        choiceSaveData.connectedNodeGUIDs.Add(((MEDNodeBase)edge.input.node).GUID);
                    }
                    choicePorts.Add(choiceSaveData);
                }
            }
        }
    }
}

#endif // UNITY_EDITOR