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
        iStorage.ForEachChild(state.Parent,
            child=>{
                if(child.IsEntryState) {
                    child.IsEntryState= false;
                }
            }
        );
        state.IsEntryState= true;
        var name= state.Name;
        iStorage.SaveStorage("Set As Entry "+name);
        return state;
    }
	// ----------------------------------------------------------------------
    public static void ShowInHierarchy(iCS_EditorObject obj) {
        var editor= iCS_EditorMgr.FindTreeViewEditor();
        if(editor != null) {
            editor.ShowElement(obj);
        }
    }
    // ----------------------------------------------------------------------
    // Change the display root to the selected object.
    public static void SetAsDisplayRoot(iCS_EditorObject obj) {
        if(obj == null || !obj.IsNode) return;
        var iStorage= obj.IStorage;
        iStorage.DisplayRoot= obj;
        iStorage.SaveStorage("Change Display Root");
    }
    // ----------------------------------------------------------------------
    // Change the display root to the parent of the selected object.
    public static void FocusOnParent(iCS_EditorObject obj) {
        if(obj == null) return;
        var parent= obj.ParentNode;
        if(parent == null) return;
        var iStorage= parent.IStorage;
        iStorage.DisplayRoot= parent;
        iStorage.SaveStorage("Change Display Root");
    }
    // ----------------------------------------------------------------------
	public static void ToggleShowDisplayRootNode(iCS_IStorage iStorage) {
		iStorage.ShowDisplayRootNode= !iStorage.ShowDisplayRootNode;
        iStorage.SaveStorage("Toggle Show Display Root");
	}
    // ----------------------------------------------------------------------
    public static void ChangeName(iCS_EditorObject obj, string name) {
        if(string.Compare(obj.RawName, name) == 0) return;
        obj.Name= name;
        obj.IStorage.SaveStorage("Change name => "+name);
    }
}
