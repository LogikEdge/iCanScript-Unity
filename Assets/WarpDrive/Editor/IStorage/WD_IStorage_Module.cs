using UnityEngine;
using System.Collections;

public partial class WD_IStorage {
    // ======================================================================
    // Module helpers
    // ----------------------------------------------------------------------
    public bool HasEnablePort(WD_EditorObject obj) {
        return ForEachChildPort(obj, p=> p.IsEnablePort);
    }

}
