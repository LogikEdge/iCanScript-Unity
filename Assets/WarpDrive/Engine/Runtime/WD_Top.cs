using UnityEngine;
using System.Collections;

[System.Serializable]
public sealed class WD_Top : WD_Action {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public WD_Action    Action;
    public WD_RootNode  RootNode;
        
    public int          FrameId     { get { return myFrameId; }}
    public WD_Graph     Graph       { get { return RootNode.Graph; }}
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static WD_Top CreateInstance(string _name, WD_RootNode parent) {
        return CreateInstance<WD_Top>().Init(_name, parent);
    }
    // ----------------------------------------------------------------------
    WD_Top Init(string _name, WD_RootNode parent) {
        RootNode= parent;
        base.Init(_name, parent);
        Top= this;
        Action= null;
        return this;
    }


    // ----------------------------------------------------------------------
    // Updates the frame number and executes all enabled children.
    protected override void Evaluate() {
        ++myFrameId;
        if(Action) {
            Action.Execute();
        }
    }
        
    // ======================================================================
    // OBJECT PICKING
    // ----------------------------------------------------------------------
    // Returns the node at the given position
    public WD_Node GetNodeAt(Vector2 _position) {
        return RootNode.GetNodeAt(_position);
    }
    
    // ----------------------------------------------------------------------
    // Returns the connection at the given position.
    public WD_Port GetPortAt(Vector2 _position) {
        return RootNode.GetPortAt(_position);
    }

    // ======================================================================
    // CHILD MANAGEMENT
    // ----------------------------------------------------------------------
    public override void AddChild(WD_Object _object) {
        if(Action != null) {
            Debug.LogError("Top action is already configured.  Remove existing action before adding a new one.");
            return;
        }
        WD_Action child= _object as WD_Action;
        if(!child) {
            Debug.LogError("Trying to add an object that is not an action into Top!");
            return;
        }
        Action= child;
        base.AddChild(_object);
    }
    public override void RemoveChild(WD_Object _object) {
        WD_Action child= _object as WD_Action;
        if(!child || Action != child) {
            Debug.LogError("Trying to remove an object that is not a child of Top!!!");
            return;
        }
        Action= null;
        base.RemoveChild(_object);        
        Dealloc();
    }

}
