using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_NodeTooltipEditor : iCS_ISubEditor {
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
    string              myOriginalTooltip= null;
    
    public iCS_NodeTooltipEditor(iCS_EditorObject target, iCS_IStorage iStorage) {
		myIStorage= iStorage;
		myTarget= target;
        myOriginalName= myTarget.DisplayName;
        myOriginalTooltip= myTarget.Tooltip;
    }
    
//    void OnEnable() {
//        position= new Rect(Screen.width/2, Screen.height/2, 250, 75);
//    }
    
    public bool Update() {
        if(myTarget == null || myIStorage == null) {
            Debug.LogWarning("iCanScript: NodeTitlePopup invoked before it is initialized.");
//			Close();
            return false;
        }
        string name= myTarget.DisplayName;
        if(name == null || name == "") name= EmptyStr;
        if(myTarget.IsNameEditable) {
            name= EditorGUILayout.TextField("Name", name);
            if(name != EmptyStr && name != myTarget.DisplayName) {
                myTarget.DisplayName= name;
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
                myTarget.DisplayName= myOriginalName;
                myTarget.Tooltip= myOriginalTooltip;
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

