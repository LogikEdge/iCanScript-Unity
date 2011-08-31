using UnityEngine;
using System.Collections;

public sealed class AP_Top : AP_Action {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public AP_Action    Action;
    public AP_RootNode  RootNode;
        
    public int          FrameId     { get { return myFrameId; }}
    public AP_Graph     Graph       { get { return RootNode.Graph; }}
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_Top CreateInstance(string _name, AP_RootNode parent) {
        return CreateInstance<AP_Top>().Init(_name, parent);
    }
    // ----------------------------------------------------------------------
    AP_Top Init(string _name, AP_RootNode parent) {
        base.Init(_name, parent);
        RootNode= parent;
        Top= this;
        Action= null;
        // Override the base class initialization since we don't want Top
        // to show in the graph.
//        IsVisible= false;
        return this;
    }


    // ----------------------------------------------------------------------
    // Updates the frame number and executes all enabled children.
    public override void Execute() {
        ++myFrameId;
        if(Action) {
            Action.Execute();
        }
    }
        
    // ======================================================================
    // OBJECT PICKING
    // ----------------------------------------------------------------------
    // Returns the node at the given position
    public AP_Node GetNodeAt(Vector2 _position) {
        return RootNode.GetNodeAt(_position);
    }
    
    // ----------------------------------------------------------------------
    // Returns the connection at the given position.
    public AP_Port GetPortAt(Vector2 _position) {
        return RootNode.GetPortAt(_position);
    }

    // ======================================================================
    // CHILD MANAGEMENT
    // ----------------------------------------------------------------------
    public override void AddChild(AP_Object _object) {
        if(Action != null) {
            Debug.LogError("Top action is already configured.  Remove existing action before adding a new one.");
            return;
        }
        AP_Action child= _object as AP_Action;
        if(!child) {
            Debug.LogError("Trying to add an object that is not an action into Top!");
            return;
        }
        Action= child;
        base.AddChild(_object);
    }
    public override void RemoveChild(AP_Object _object) {
        AP_Action child= _object as AP_Action;
        if(!child || Action != child) {
            Debug.LogError("Trying to remove an object that is not a child of Top!!!");
            return;
        }
        Action= null;
        base.RemoveChild(_object);        
        Dealloc();
    }

}
