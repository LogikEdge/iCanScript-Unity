using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class UK_DispatcherBase : UK_Action {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    protected List<UK_Action> myExecuteQueue= new List<UK_Action>();
    protected int             myQueueIdx = 0;
    protected bool            myIsStaled= false;
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public bool IsStaled { get { return myIsStaled; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_DispatcherBase(string name) : base(name) {}

    // ======================================================================
    // Queue Management
    // ----------------------------------------------------------------------
    public void AddChild(UK_Action action) {
        myExecuteQueue.Add(action);
    }
    public void RemoveChild(UK_Action action) {
        myExecuteQueue.Remove(action);
    }
}
