using UnityEngine;
using System.Collections;
using iCanScript.Editor;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    /// Ask each object to perform their own sanity check.
    public void SanityCheck() {
        // -- Don't perform sanity check on transient data --
        if(!IsRootObjectAType) return;
        
        // -- Validate user inputs --
        var kSanityCheckServiceKey= "SanityCheck";
        ErrorController.Clear(kSanityCheckServiceKey);

        // -- Validate visual script type name --
        var message= Sanity.ValidateVisualScriptTypeName(this);
        if(message != null) {
            ErrorController.AddError(kSanityCheckServiceKey, message, VisualScript, 0);
        }

        // -- Validate CSharpFileName --
        if(CSharpFileName != TypeName) {
            message= "CSharp file name: <b><color=red>"+CSharpFileName+"</color></b> doesn't match the type name: <b><color=red>"+TypeName+"</color></b>";
        }
        
        // -- Verify base types --
        message= Sanity.ValidateEngineBaseType();
        if(message != null) {
            ErrorController.AddError(kSanityCheckServiceKey, message, VisualScript, 0);
        }
        message= Sanity.ValidateVisualScriptBaseType(this);
        if(message != null) {
            ErrorController.AddError(kSanityCheckServiceKey, message, VisualScript, 0);
        }        

        // -- Verify namespaces --
        message= Sanity.ValidateEditorNamespace();
        if(message != null) {
            ErrorController.AddError(kSanityCheckServiceKey, message, VisualScript, 0);
        }
        message= Sanity.ValidateEngineNamespace();
        if(message != null) {
            ErrorController.AddError(kSanityCheckServiceKey, message, VisualScript, 0);
        }
        message= Sanity.ValidateVisualScriptNamespace(this);
        if(message != null) {
            ErrorController.AddError(kSanityCheckServiceKey, message, VisualScript, 0);
        }        

        // -- Validate folders --
        message= Sanity.ValidateEditorCodeGenerationFolder();
        if(message != null) {
            ErrorController.AddError(kSanityCheckServiceKey, message, VisualScript, 0);
        }
        message= Sanity.ValidateEngineCodeGenerationFolder();
        if(message != null) {
            ErrorController.AddError(kSanityCheckServiceKey, message, VisualScript, 0);
        }
        
        // -- Ask each object to perform their own sanity check --
        ForEach(o=> o.SanityCheck(kSanityCheckServiceKey));
    }

}
