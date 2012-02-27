using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_EditorProxy : iCS_Editor {
    // ----------------------------------------------------------------------
    public static iCS_Editor GetEditor() {
        iCS_EditorProxy editorProxy= EditorWindow.GetWindow(typeof(iCS_EditorProxy), false, "iCanScript") as iCS_EditorProxy;
        DontDestroyOnLoad(editorProxy);
        editorProxy.hideFlags= HideFlags.DontSave;                    
        // Assure that the components are properly installed.
        return editorProxy;        
    } 
    // ----------------------------------------------------------------------
    protected override void InvokeInstaller() {
    	iCS_Installer.Install();        
    }
    // ----------------------------------------------------------------------
    protected override iCS_ClassWizard GetClassWizard() {
        return iCS_ClassWizardProxy.GetClassWizard();
    }
}
