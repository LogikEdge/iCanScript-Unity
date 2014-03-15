//
// iCS_UserCommands_Others
//
//#define DEBUG
using UnityEngine;
using System.Collections;
using Pref= iCS_PreferencesController;


public static partial class iCS_UserCommands {
    // ======================================================================
    // Miscellanious User Commands.
	// ----------------------------------------------------------------------
    // OK
    public static iCS_EditorObject SetAsStateEntry(iCS_EditorObject state) {
#if DEBUG
        Debug.Log("iCanScript: Set As Entry State => "+state.Name);
#endif
        if(state == null) return null;
        var iStorage= state.IStorage;
        var name= state.Name;
        iStorage.RegisterUndo("Set As Entry "+name);
        iStorage.ForEachChild(state.Parent,
            child=>{
                if(child.IsEntryState) {
                    child.IsEntryState= false;
                }
            }
        );
        state.IsEntryState= true;
        iStorage.IsDirty= true;
        return state;
    }
	// ----------------------------------------------------------------------
    public static void ShowInHierarchy(iCS_EditorObject obj) {
        var editor= iCS_EditorMgr.FindHierarchyEditor();
        if(editor != null) {
            editor.ShowElement(obj);
        }
    }
    // ----------------------------------------------------------------------
    // Change the display root to the selected object.
    public static void IsolateInView(iCS_EditorObject obj) {
        if(obj == null || !obj.IsNode) return;
        var iStorage= obj.IStorage;
        iStorage.RegisterUndo("Focus On "+obj.Name);
        iStorage.DisplayRoot= obj;
    }
    // ----------------------------------------------------------------------
    // Change the display root to the parent of the selected object.
    public static void FocusOnParent(iCS_EditorObject obj) {
        if(obj == null) return;
        var parent= obj.ParentNode;
        if(parent == null) return;
        var iStorage= parent.IStorage;
        iStorage.RegisterUndo("Focus On "+parent.Name);
        parent.IStorage.DisplayRoot= parent;
    }
    // ----------------------------------------------------------------------
	public static void ToggleShowRootNode(iCS_IStorage iStorage) {
		iStorage.RegisterUndo("Toggle Show Root Node");
		Pref.ShowRootNode= !Pref.ShowRootNode;
	}
}
