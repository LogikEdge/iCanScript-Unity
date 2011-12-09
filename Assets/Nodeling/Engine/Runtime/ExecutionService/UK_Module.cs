using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UK_Module : UK_FunctionBase {
    // ======================================================================
    // Field
    // ----------------------------------------------------------------------
    UK_ParallelDispatcher   myDispatcher= null;
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_Module(string name, bool[] paramIsOuts, Vector2 layout) : base(name, paramIsOuts, layout) {
        myDispatcher= new UK_ParallelDispatcher(name, layout);
    }

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        myDispatcher.Execute(frameId);
        MarkAsCurrent(myDispatcher.FrameId);
        IsStalled= myDispatcher.IsStalled;
    }
    public override void ForceExecute(int frameId) {
        myDispatcher.ForceExecute(frameId);
        MarkAsCurrent(myDispatcher.FrameId);
        IsStalled= myDispatcher.IsStalled;
    }

    // ======================================================================
    // Connector Management
    // ----------------------------------------------------------------------
    public void AddChild(object obj) {
        if(IsExecutable(obj)) {
            myDispatcher.AddChild(obj as UK_Action);
        }
    }
    public void RemoveChild(object obj) {
        if(IsExecutable(obj)) {
            myDispatcher.RemoveChild(obj as UK_Action);
        }
    }
    // ----------------------------------------------------------------------
    // Returns true if the object is an executable that we support.
    bool IsExecutable(object _object) {
        return _object is UK_Action;
    }
}
