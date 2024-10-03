using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR

using UnityEditor.Experimental.GraphView;

namespace GetMikyled.MEDialogue
{
    ///-//////////////////////////////////////////////////////////////////
    ///
    public class MEDNodeSaveDataBase
    {
        public string GUID;
        public Vector2 position;

        public List<PortSaveData> inputPorts;
        public List<PortSaveData> outputPorts;

        protected virtual void SavePortsData(MEDNodeBase n)
        {
            inputPorts = new List<PortSaveData>();
            outputPorts = new List<PortSaveData>();
            
            // Save input port information
            foreach (VisualElement element in n.inputContainer.Children())
            {
                if (element is MEDPort inputPort)
                {
                    // Create port save data
                    PortSaveData portSaveData = new PortSaveData()
                    {
                        portGUID = inputPort.GUID,
                        nodeGUID = n.GUID
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
            foreach (VisualElement element in n.outputContainer.Children())
            {
                if (element is MEDPort outputPort)
                {
                    // Create port save data
                    PortSaveData portSaveData = new PortSaveData()
                    {
                        portGUID = outputPort.GUID,
                        nodeGUID = n.GUID
                    };
                    
                    // Add connections
                    foreach (Edge edge in outputPort.connections)
                    {
                        portSaveData.connectedNodeGUIDs.Add(((MEDNodeBase)edge.input.node).GUID);
                    }
                    outputPorts.Add(portSaveData);
                }
            }
        }
    }
}

#endif // UNITY_EDITOR