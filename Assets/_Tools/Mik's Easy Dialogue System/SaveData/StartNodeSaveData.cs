using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
#endif // UNITY_EDITOR

namespace GetMikyled.MEDialogue
{
    ///-//////////////////////////////////////////////////////////////////
    ///
    [Serializable]
    public class StartNodeSaveData : MEDNodeSaveDataBase
    {
        public string conversationName;
        public string nextNodeGUID;

#if UNITY_EDITOR
        ///-//////////////////////////////////////////////////////////////////
        ///
        public void Initialize(StartNode sNode)
        {
            GUID = sNode.GUID;
            conversationName = sNode.conversationName;
            position = sNode.GetPosition().position;

            SavePortsData(sNode);
        }
#endif // UNITY_EDITOR
    }
}