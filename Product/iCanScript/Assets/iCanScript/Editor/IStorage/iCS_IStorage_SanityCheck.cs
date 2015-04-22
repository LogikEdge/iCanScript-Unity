using UnityEngine;
using System.Collections;
using iCanScript.Editor;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    /// Ask each object to perform their own sanity check.
    public void SanityCheck() {
        var kSanityCheckServiceKey= "SanityCheck";
        ErrorController.Clear(kSanityCheckServiceKey);
        // -- Verify base types --
        var message= Sanity.ValidateDefaultBaseType();
        if(message != null) {
            ErrorController.AddError(kSanityCheckServiceKey, message, VisualScript, 0);
        }
        message= Sanity.ValidateVisualScriptBaseType(this);
        if(message != null) {
            ErrorController.AddError(kSanityCheckServiceKey, message, VisualScript, 0);
        }        
        // -- Ask each object to perform their own sanity check --
        ForEach(o=> o.SanityCheck(kSanityCheckServiceKey));
    }

}
