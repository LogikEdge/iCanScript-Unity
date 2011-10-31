using UnityEngine;
using System.Collections;

public partial class WD_IStorage {
    // ======================================================================
    // Module helpers
    // ----------------------------------------------------------------------
    public bool HasEnablePort(WD_EditorObject module) {
        return ForEachChildPort(module, p=> p.IsEnablePort);
    }
    public WD_EditorObject GetEnablePort(WD_EditorObject module) {
        WD_EditorObject enablePort= null;
        ForEachChildPort(module, p=> { if(p.IsEnablePort) { enablePort= p; return true; } return false; });
        return enablePort;
    }
}
