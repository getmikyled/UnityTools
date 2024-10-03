using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

#if UNITY_EDITOR
using Unity.VisualScripting;
using UnityEditor;

namespace GetMikyled.MEDialogue 
{
    ///-//////////////////////////////////////////////////////////////////
    ///
    public class MEDIOUtility
    {
        private static MEDGraphView graphView;
        private static MEDialogueGraph meDialogueGraphFile;
        private static string fileName;
        private static string graphFolderPath = "Assets/Resources/Dialogue";

#region Saving
        public static void InitializeSave(MEDGraphView argGraphView, string argFileName)
        {
            Debug.Log("Save initialized");
            graphView = argGraphView;
            fileName = argFileName;

            CreateStaticFolders();
            CreateDSGraphFile();
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        public static void CreateStaticFolders()
        {
            CreateFolder("Assets", "Resources");  
            CreateFolder("Assets/Resources", "Dialogue");   // Folder that holds dialog graph files
        }


        ///-//////////////////////////////////////////////////////////////////
        ///
        public static void CreateFolder(string argPath, string argFolderName)
        {
            if (AssetDatabase.IsValidFolder(argPath + "/" + argFolderName))
            {
                return;
            }
            AssetDatabase.CreateFolder(argPath, argFolderName);
        }
        
        ///-//////////////////////////////////////////////////////////////////
        ///
        public static void CreateDSGraphFile()
        {
            // Create the dialogueGraphFile and get graph view elements
            meDialogueGraphFile = CreateAsset<MEDialogueGraph>(graphFolderPath, fileName);
            GetElementsFromGraphView();
            
            // Save assets and mark as dirty to ensure saving
            EditorUtility.SetDirty(meDialogueGraphFile);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        private static void GetElementsFromGraphView()
        {
            // Save nodes in graph view
            HashSet<string> savedNodes = new HashSet<string>();
            graphView.graphElements.ForEach(graphElement =>
            {
                // Save Dialogue and Start Nodes to the dialogueGraphFile
                if (graphElement is StartNode sNode)
                {
                    meDialogueGraphFile.SaveStartNode(sNode);
                    savedNodes.Add(sNode.GUID);
                }
                else if (graphElement is DialogueNode dNode)
                {
                    meDialogueGraphFile.SaveDialogueNode(dNode);
                    savedNodes.Add(dNode.GUID);
                }
            });
            
            // TO DO: Use the savedNodes to filter through deleted nodes and remove them from the dialogueGraphFile
            
            // Save edges
            meDialogueGraphFile.edges = new List<EdgeSaveData>();
            foreach (Edge edge in graphView.edges)
            {
                meDialogueGraphFile.edges.Add(new EdgeSaveData()
                {
                    // Input/Output GUID: nodeGUID + "_" + portGUID
                    inputGUID = ((MEDPort)edge.input).GUID,
                    outputGUID = ((MEDPort)edge.output).GUID
                });
            }
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        private static T CreateAsset<T>(string argPath, string argAssetName) where T : ScriptableObject
        {
            string fullPath = $"{argPath}/{argAssetName}.asset";

            T asset = LoadAsset<T>(fullPath);

            // If asset isn't found, create a new asset
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();

                AssetDatabase.CreateAsset(asset, fullPath);
            }
            return asset;
        }
        #endregion // SAVING

#region LOADING

        ///-//////////////////////////////////////////////////////////////////
        ///
        public static void LoadFile(MEDGraphView medGraphView, MEDialogueGraph meDialogueGraph)
        {
            graphView = medGraphView;
            LoadGraphElements(meDialogueGraph);
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        public static void LoadGraphElements(MEDialogueGraph meDialogueGraph)
        {
            // Clear the graph view of existing nodes
            graphView.ClearGraphElements();
            
            // For simplifying edge creation
            Dictionary<string, MEDPort> portMap = new Dictionary<string, MEDPort>();        // Port Node GUID -> PORT
            
            // Create Start Nodes in GraphView
            foreach (StartNodeSaveData sNodeSaveData in meDialogueGraph.startNodes)
            {
                // Create Start Node from Save Data
                StartNode sNode = graphView.CreateStartNode(sNodeSaveData);
                
                // Add Ports to Port Map for Edge Connecting
                foreach (VisualElement element in sNode.outputContainer.Children())
                {
                    if (element is MEDPort port)
                    {
                        portMap.Add(port.GUID, port);
                    }
                }
            }
            
            // Create Dialogue Nodes in GraphView
            foreach (DialogueNodeSaveData dNodeSaveData in meDialogueGraph.dialogueNodes)
            {
                // Create Dialogue Node
                DialogueNode dNode = graphView.CreateDialogueNode(dNodeSaveData);
                
                // Add Ports to Port Map for Edge Connecting
                // Input Ports
                foreach (VisualElement element in dNode.inputContainer.Children())
                {
                    if (element is MEDPort port)
                    {
                        portMap.Add(port.GUID, port);
                    }
                }
                // Output Ports
                foreach (VisualElement element in dNode.outputContainer.Children())
                {
                    if (element is MEDPort port)
                    {
                        portMap.Add(port.GUID, port);
                    }
                }
            }
            
            // Create Edges
            foreach (EdgeSaveData edgeSaveData in meDialogueGraph.edges)
            {
                MEDPort inputPort = portMap[edgeSaveData.inputGUID];
                MEDPort outputPort = portMap[edgeSaveData.outputGUID];
                graphView.CreateEdge(inputPort, outputPort);
            }
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        private static T LoadAsset<T>(string argPath) where T : ScriptableObject
        {
            return AssetDatabase.LoadAssetAtPath<T>(argPath);
        }
        
 #endregion // LOADING
    }
}

#endif // UNITY_EDITOR