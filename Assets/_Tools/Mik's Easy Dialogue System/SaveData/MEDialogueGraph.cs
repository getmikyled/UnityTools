using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR 
using UnityEditor.Callbacks;
using UnityEditor;
#endif //UNITY_EDITOR 

namespace GetMikyled.MEDialogue
{
    ///-//////////////////////////////////////////////////////////////////
    ///
    [Serializable]
    [CreateAssetMenu(fileName = "DialogueGraph")]
    public class MEDialogueGraph : ScriptableObject
    {
        public List<StartNodeSaveData> startNodes;
        public List<DialogueNodeSaveData> dialogueNodes;
        public List<EdgeSaveData> edges;

        public MEDialogueGraph()
        {
            startNodes = new List<StartNodeSaveData>();
            dialogueNodes = new List<DialogueNodeSaveData>();
            edges = new List<EdgeSaveData>();
        }
        
#if UNITY_EDITOR 
        
        ///-//////////////////////////////////////////////////////////////////
        ///
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            MEDialogueGraph meDialogueGraph = EditorUtility.InstanceIDToObject(instanceID) as MEDialogueGraph;
            if (meDialogueGraph != null)
            {
                MEDialogueWindow.OpenFromFile(meDialogueGraph);
                return true;
            }
            return false;
        }
        
#endif // UNITY_EDITOR 

        ///-//////////////////////////////////////////////////////////////////
        ///
        public StartNodeSaveData SaveStartNode(StartNode graphNode)
        {
            // Check if StartNodeSaveData exists
            StartNodeSaveData sNodeSaveData = startNodes.FirstOrDefault(sNode => sNode.GUID.Equals(graphNode.GUID));
            if (sNodeSaveData == null)
            {
                // If graph node is new, create new save data
                sNodeSaveData = new StartNodeSaveData();
                startNodes.Add(sNodeSaveData);
            }
            
            // Save the data
            // TO DO: Figure out how to connect nextNodeGUID
            sNodeSaveData.Initialize(graphNode);
            
            return sNodeSaveData;
        }
        
        ///-//////////////////////////////////////////////////////////////////
        ///
        public DialogueNodeSaveData SaveDialogueNode(DialogueNode graphNode)
        {
            // Check if DialogueNodeSaveData exists
            DialogueNodeSaveData dNodeSaveData = dialogueNodes.FirstOrDefault(dNode => dNode.GUID.Equals(graphNode.GUID));
            if (dNodeSaveData == null)
            {
                // If graph node is new, create new save data
                dNodeSaveData = new DialogueNodeSaveData();
                dialogueNodes.Add(dNodeSaveData);
            }
            
            // Save the data
            dNodeSaveData.Initialize(graphNode);
            return dNodeSaveData;
        }
    }
}
