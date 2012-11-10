using UnityEngine;
using System;
using System.Collections;

public partial class iCS_IStorage {    
    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
    const string EnablePortStr= "enable";
    
    // ======================================================================
    // Creation
    // ----------------------------------------------------------------------
    public iCS_EditorObject CreateEnablePort(int parentId) {
        iCS_EditorObject enablePort= CreatePort(EnablePortStr, parentId, typeof(bool), iCS_ObjectTypeEnum.EnablePort);
        enablePort.IsNameEditable= false;
        return enablePort;
    }
    
    // ======================================================================
    // Module helpers
    // ----------------------------------------------------------------------
    public bool HasEnablePort(iCS_EditorObject module) {
        return UntilMatchingChildPort(module, p=> p.IsEnablePort);
    }
    public iCS_EditorObject GetEnablePort(iCS_EditorObject module) {
        iCS_EditorObject enablePort= null;
        UntilMatchingChildPort(module, p=> { if(p.IsEnablePort) { enablePort= p; return true; } return false; });
        return enablePort;
    }
}
