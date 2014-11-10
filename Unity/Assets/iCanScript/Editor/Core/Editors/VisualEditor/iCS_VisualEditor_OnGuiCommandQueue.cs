using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_VisualEditor : iCS_EditorBase {
    List<Action> OnGUICommandQueue= new List<Action>();

    // ======================================================================
    // ----------------------------------------------------------------------
    void RunOnGUICommands() {
		OnGUICommandQueue.ForEach(f=> f());
		OnGUICommandQueue.Clear();
    }
    public void QueueOnGUICommand(Action fnc) {
        OnGUICommandQueue.Add(fnc);
    }
}
