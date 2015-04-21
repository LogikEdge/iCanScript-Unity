using UnityEngine;
using System.Collections;
using iCanScript.Editor;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    /// Ask each object to perform their own sanity check.
    public void SanityCheck() {
        var kSanityCheckServiceKey= "SanityCheck";
        ErrorController.Clear(kSanityCheckServiceKey);
        // -- Verify visual script attributes --
        
        // -- Ask each object to perform their own sanity check --
        ForEach(o=> o.SanityCheck(kSanityCheckServiceKey));
    }

}
