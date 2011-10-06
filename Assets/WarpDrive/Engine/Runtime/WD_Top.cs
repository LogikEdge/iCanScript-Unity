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
    public WD_Behaviour Graph       { get { return RootNode.Graph; }}
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static WD_Top CreateInstance(string _name, WD_RootNode parent) {
        WD_Top instance= CreateInstance<WD_Top>();
        instance.Init(_name, parent);
        return instance;
    }
    // ----------------------------------------------------------------------
    public override void Init(string _name, WD_Aggregate parent) {
        RootNode= parent as WD_RootNode;
        base.Init(_name, parent);
        Top= this;
        Action= null;
    }


    // ----------------------------------------------------------------------
    // Updates the frame number and executes all enabled children.
    public override void Evaluate() {
        ++myFrameId;
        if(Action) {
            Action.Execute();
        }
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
