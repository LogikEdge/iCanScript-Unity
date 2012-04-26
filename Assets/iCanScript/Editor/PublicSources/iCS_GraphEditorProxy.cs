using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_GraphEditorProxy : iCS_GraphEditor {
    // ----------------------------------------------------------------------
    public static iCS_GraphEditor GetEditor() {
        iCS_GraphEditorProxy editorProxy= EditorWindow.GetWindow(typeof(iCS_GraphEditorProxy), false, "iCanScript") as iCS_GraphEditorProxy;
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
