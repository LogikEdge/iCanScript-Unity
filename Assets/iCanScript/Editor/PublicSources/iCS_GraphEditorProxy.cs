using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_GraphEditorProxy : iCS_GraphEditor {
    // ----------------------------------------------------------------------
    protected override void InvokeInstaller() {
    	iCS_Installer.Install();        
    }
}
