using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_PortNameEditor : iCS_ISubEditor {
    // ======================================================================
    // Constants.
	// ----------------------------------------------------------------------
    const string EmptyStr= "(empty)";
    
    // ======================================================================
    // Field.
	// ----------------------------------------------------------------------
    iCS_IStorage        myIStorage       = null;
    iCS_EditorObject    myTarget         = null;
    string	            myOriginalName   = null;
    string	            myOriginalTooltip= null;
    
    public iCS_PortNameEditor(iCS_EditorObject target, iCS_IStorage iStorage) {
        myIStorage= iStorage;
        myTarget= target;
        myOriginalName= myTarget.RawName;
        myOriginalTooltip= myTarget.RawToolTip;
    }
    
//    void OnEnable() {
//        position= new Rect(Screen.width/2, Screen.height/2, 250, 75);
//    }
    
    public bool Update() {
        if(myTarget == null || myIStorage == null) {
            Debug.LogWarning("iCanScript: Port Editor invoked before it is initialized.");
//			Close();
            return false;
        }
        string name= myTarget.RawName;
        if(name == null || name == "") name= EmptyStr;
        if(myTarget.IsNameEditable) {
            name= EditorGUILayout.TextField("Name", name);
            if(name != EmptyStr && name != myTarget.RawName) {
                myTarget.Name= name;
                myIStorage.SetDirty(myTarget);
            }                    
        } else {
            EditorGUILayout.LabelField("Name", name);                    
        }
        // Show object tooltip.
        string toolTip= myTarget.RawToolTip;
        if(toolTip == null || toolTip == "") toolTip= EmptyStr;
        toolTip= EditorGUILayout.TextField("Tooltip", toolTip);
        if(toolTip != EmptyStr && toolTip != myTarget.RawToolTip) {
            myTarget.ToolTip= toolTip;
        }

        GUILayout.BeginHorizontal(); {
            if(GUILayout.Button("Cancel")) {
                myTarget.RawName= myOriginalName;
                myTarget.RawToolTip= myOriginalTooltip;
//                Close();
            }
            if(GUILayout.Button("Save")) {
//                Close();
            }
        }
        GUILayout.EndHorizontal();
        return true;
    }
}
