using UnityEngine;
using System.Collections;

public static partial class iCS_UserCommands {
    // ----------------------------------------------------------------------
    // Change the display root to the selected object.
    public static void SetAsDisplayRoot(iCS_EditorObject obj) {
        if(obj == null || !obj.IsNode) return;
        var iStorage= obj.IStorage;
        if(iStorage.DisplayRoot == obj) return;
        iStorage.SaveNavigationState();
        iStorage.DisplayRoot= obj;
        iStorage.ForcedRelayoutOfTree(obj);
        SendDisplayRootChange(iStorage);
    }
    // ----------------------------------------------------------------------
    // Change the display root to the parent of the selected object.
    public static void ResetDisplayRoot(iCS_IStorage iStorage) {
        if(iStorage == null) return;
        iStorage.ClearNavigationHistory();
        SendDisplayRootChange(iStorage);
    }
    // ----------------------------------------------------------------------
	public static void ToggleShowDisplayRootNode(iCS_IStorage iStorage) {
		iStorage.ShowDisplayRootNode= !iStorage.ShowDisplayRootNode;
        iStorage.SaveStorage("Toggle Show Display Root");
	}
    // ----------------------------------------------------------------------
    public static void ReloadFromBackwardNavigationHistory(iCS_IStorage iStorage) {
        iStorage.ReloadFromBackwardNavigationHistory();
    }
    // ----------------------------------------------------------------------
    public static void ReloadFromForwardNavigationHistory(iCS_IStorage iStorage) {
        iStorage.ReloadFromForwardNavigationHistory();
    }
    // ----------------------------------------------------------------------
    public static void ClearNavigationHistory(iCS_IStorage iStorage) {
        iStorage.ClearNavigationHistory();
    }
}
