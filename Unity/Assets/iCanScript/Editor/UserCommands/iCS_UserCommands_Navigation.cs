using UnityEngine;
using System.Collections;

public static partial class iCS_UserCommands {
    // ----------------------------------------------------------------------
    // Change the display root to the selected object.
    public static void SetAsDisplayRoot(iCS_EditorObject obj) {
        if(obj == null || !obj.IsNode) return;
        var iStorage= obj.IStorage;
        if(iStorage.DisplayRoot == obj) return;
		// The display root must be a package with visible children capabilities.
		var newDisplayRoot= obj;
		while(newDisplayRoot != null && (!newDisplayRoot.IsKindOfPackage || newDisplayRoot.IsInstanceNode)) {
			newDisplayRoot= newDisplayRoot.ParentNode;
		}
		if(newDisplayRoot == null) return;
        iStorage.SaveNavigationState();
        iStorage.DisplayRoot= newDisplayRoot;
        iStorage.ForcedRelayoutOfTree();
        iStorage.ResetAllAnimationPositions();
        SendDisplayRootChange(iStorage);
        iStorage.SaveStorage();
    }
    // ----------------------------------------------------------------------
    // Change the display root to the parent of the selected object.
    public static void ResetDisplayRoot(iCS_IStorage iStorage) {
        if(iStorage == null) return;
        iStorage.ClearNavigationHistory();
        SendDisplayRootChange(iStorage);
        iStorage.SaveStorage();
    }
    // ----------------------------------------------------------------------
	public static void ToggleShowDisplayRootNode(iCS_IStorage iStorage) {
		iStorage.ShowDisplayRootNode= !iStorage.ShowDisplayRootNode;
        iStorage.SaveStorage();
	}
    // ----------------------------------------------------------------------
    public static void ReloadFromBackwardNavigationHistory(iCS_IStorage iStorage) {
        iStorage.ReloadFromBackwardNavigationHistory();
        iStorage.SaveStorage();
    }
    // ----------------------------------------------------------------------
    public static void ReloadFromForwardNavigationHistory(iCS_IStorage iStorage) {
        iStorage.ReloadFromForwardNavigationHistory();
        iStorage.SaveStorage();
    }
    // ----------------------------------------------------------------------
    public static void ClearNavigationHistory(iCS_IStorage iStorage) {
        iStorage.ClearNavigationHistory();
        iStorage.SaveStorage();
    }
}
