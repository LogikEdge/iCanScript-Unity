using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_PortTooltipEditor : iCS_SubEditor {
    // ======================================================================
    // Constants.
	// ----------------------------------------------------------------------
    const string EmptyStr= "(empty)";
    
    // ======================================================================
    // Field.
	// ----------------------------------------------------------------------
    string	myOriginalName   = null;
    string	myOriginalTooltip= null;
    
    public override void Init(iCS_EditorObject target, iCS_IStorage storage) {
		base.Init(target, storage);
        myOriginalName= myTarget.RawName;
        myOriginalTooltip= myTarget.RawToolTip;
    }
    
    void OnEnable() {
        position= new Rect(Screen.width/2, Screen.height/2, 250, 75);
    }
    
    void OnGUI() {
        if(myTarget == null || myStorage == null) {
            Debug.LogWarning("iCanScript: Port Editor invoked before it is initialized.");
			Close();
            return;
        }
        string name= myTarget.RawName;
        if(name == null || name == "") name= EmptyStr;
        if(myTarget.IsNameEditable) {
            name= EditorGUILayout.TextField("Name", name);
            if(name != EmptyStr && name != myTarget.RawName) {
                myTarget.Name= name;
                myStorage.SetDirty(myTarget);
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
                Close();
            }
            if(GUILayout.Button("Save")) {
                Close();
            }
        }
        GUILayout.EndHorizontal();
    }
}
