using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;

namespace GetMikyled.MEDialogue
{
    ///-//////////////////////////////////////////////////////////////////
    ///
    public class CharacterOutliner : Blackboard
    {
        private Dictionary<string, Character> characters;
        
        ///-//////////////////////////////////////////////////////////////////
        ///
        public CharacterOutliner(GraphView graphView) : base(graphView)
        {
            title = "Characters";
            characters = new Dictionary<string, Character>();
            /*contentContainer.Add(new DropdownField()
            {
                choices = characters.Keys()
            });*/
        }
    }

}

#endif // UNITY_EDITOR