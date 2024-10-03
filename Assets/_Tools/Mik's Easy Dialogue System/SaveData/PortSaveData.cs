using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GetMikyled.MEDialogue
{
    ///-//////////////////////////////////////////////////////////////////
    ///
    [Serializable]
    public class PortSaveData
    {
        public string portGUID;
        public string nodeGUID;
        public List<string> connectedNodeGUIDs;

        public PortSaveData()
        {
            connectedNodeGUIDs = new List<string>();
        }
    }
}