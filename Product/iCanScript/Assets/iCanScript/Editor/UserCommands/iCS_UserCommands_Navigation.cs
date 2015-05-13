using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    public static partial class iCS_UserCommands {
        // ----------------------------------------------------------------------
        // Change the display root to the selected object.
        public static void SetAsDisplayRoot(iCS_EditorObject obj) {
            if(obj == null || !obj.IsNode) return;
            var iStorage= obj.IStorage;
            if(iStorage.DisplayRoot == obj) return;
    		// The display root must be a package with visible children capabilities.
    		var newDisplayRoot= obj;
    		while(newDisplayRoot != null && newDisplayRoot.IsInstanceNode) {
    			newDisplayRoot= newDisplayRoot.ParentNode;
    		}
    		if(newDisplayRoot == null) return;
            OpenTransaction(iStorage);
            iStorage.SaveNavigationState();
            iStorage.DisplayRoot= newDisplayRoot;
            iStorage.ForcedRelayoutOfTree();
            iStorage.ResetAllAnimationPositions();
            SendDisplayRootChange(iStorage);
            CloseTransaction(iStorage, "Set Display Root=> "+obj.DisplayName);
        }
        // ----------------------------------------------------------------------
        // Change the display root to the parent of the selected object.
        public static void ResetDisplayRoot(iCS_IStorage iStorage) {
            if(iStorage == null) return;
            OpenTransaction(iStorage);
            iStorage.ClearNavigationHistory();
            SendDisplayRootChange(iStorage);
            CloseTransaction(iStorage, "Reset Display Root");
        }
        // ----------------------------------------------------------------------
    	public static void ToggleShowDisplayRootNode(iCS_IStorage iStorage) {
            OpenTransaction(iStorage);
    		iStorage.ShowDisplayRootNode= !iStorage.ShowDisplayRootNode;
            CloseTransaction(iStorage, "Toggle Show Display Root");
    	}
        // ----------------------------------------------------------------------
        public static void ReloadFromBackwardNavigationHistory(iCS_IStorage iStorage) {
            OpenTransaction(iStorage);
            iStorage.ReloadFromBackwardNavigationHistory();
            CloseTransaction(iStorage, "Previous Display Root");
        }
        // ----------------------------------------------------------------------
        public static void ReloadFromForwardNavigationHistory(iCS_IStorage iStorage) {
            OpenTransaction(iStorage);
            iStorage.ReloadFromForwardNavigationHistory();
            CloseTransaction(iStorage, "Next Display Root");
        }
        // ----------------------------------------------------------------------
        public static void ClearNavigationHistory(iCS_IStorage iStorage) {
            OpenTransaction(iStorage);
            iStorage.ClearNavigationHistory();
            CloseTransaction(iStorage, "Clear Display Root");
        }
    }

}

