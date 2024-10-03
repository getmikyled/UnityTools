using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor.Experimental.GraphView;

namespace GetMikyled.MEDialogue
{
    ///-//////////////////////////////////////////////////////////////////
    ///
    public abstract class MEDNodeBase : Node
    {
        public string GUID;
        
        ///-//////////////////////////////////////////////////////////////////
        ///
        public MEDNodeBase(Rect argPosition)
        {
            GUID = Guid.NewGuid().ToString();
            SetPosition(argPosition);
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        protected abstract void InitializeClassList();

        ///-//////////////////////////////////////////////////////////////////
        ///
        public abstract void Draw();

        ///-//////////////////////////////////////////////////////////////////
        ///
        public MEDPort CreateInputPort(string portGUID, string portName = null)
        {
            MEDPort port = new MEDPort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            port.portName = portName;
            port.GUID = portGUID;
            
            inputContainer.Add(port);
            
            return port;
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        public MEDPort CreateOutputPort(string portGUID, string portName = null)
        {
            MEDPort port = new MEDPort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            port.portName = portName;
            port.GUID = portGUID;
            
            outputContainer.Add(port);
            
            return port;
        }
    }
}

#endif // UNITY_EDITOR