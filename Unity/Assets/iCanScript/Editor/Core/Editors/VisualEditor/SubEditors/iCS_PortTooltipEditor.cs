using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_PortTooltipEditor : iCS_ISubEditor {
    // ======================================================================
    // Constants.
	// ----------------------------------------------------------------------
    const string EmptyStr= "(empty)";
    
    // ======================================================================
    // Field.
	// ----------------------------------------------------------------------
    iCS_EditorObject    myTarget         = null;
    string	            myOriginalName   = null;
    string	            myOriginalTooltip= null;
    
    public iCS_PortTooltipEditor(iCS_EditorObject target) {
        myTarget= target;
        myOriginalName= myTarget.Name;
        myOriginalTooltip= myTarget.Tooltip;
    }
    
    
    public bool Update() {
        if(myTarget == null) {
            Debug.LogWarning("iCanScript: Port Editor invoked before it is initialized.");
            return false;
        }
        string name= myTarget.Name;
        if(name == null || name == "") name= EmptyStr;
        if(myTarget.IsNameEditable) {
            name= EditorGUILayout.TextField("Name", name);
            if(name != EmptyStr && name != myTarget.Name) {
                myTarget.Name= name;
            }                    
        } else {
            EditorGUILayout.LabelField("Name", name);                    
        }
        // Show object tooltip.
        string tooltip= myTarget.Tooltip;
        if(tooltip == null || tooltip == "") tooltip= EmptyStr;
        tooltip= EditorGUILayout.TextField("Tooltip", tooltip);
        if(tooltip != EmptyStr && tooltip != myTarget.Tooltip) {
            myTarget.Tooltip= tooltip;
        }

        GUILayout.BeginHorizontal(); {
            if(GUILayout.Button("Cancel")) {
                myTarget.Name= myOriginalName;
                myTarget.Tooltip= myOriginalTooltip;
            }
            if(GUILayout.Button("Save")) {
            }
        }
        GUILayout.EndHorizontal();
        return true;
    }
}
