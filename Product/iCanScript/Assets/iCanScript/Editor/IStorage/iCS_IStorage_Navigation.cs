using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_IStorage {
        // ======================================================================
        // ----------------------------------------------------------------------
        public void SaveNavigationState() {
            NavigationHistory.Save(Storage);
        }
        public void ReloadFromBackwardNavigationHistory() {
    		DisplayRoot.WrappingOffset= Vector2.zero;
            DisplayRoot.ReduceChildrenAnchorPosition();
            NavigationHistory.ReloadFromBackwardHistory(Storage);
    		DisplayRoot.WrappingOffset= Vector2.zero;
            ForcedRelayoutOfTree();
        }
        public void ReloadFromForwardNavigationHistory() {
    		DisplayRoot.WrappingOffset= Vector2.zero;
            NavigationHistory.ReloadFromForwardHistory(Storage);
    		DisplayRoot.WrappingOffset= Vector2.zero;
            ForcedRelayoutOfTree();
        }
        public bool HasBackwardNavigationHistory {
            get { return NavigationHistory.HasBackwardHistory; }
        }
        public bool HasForwardNavigationHistory {
            get { return NavigationHistory.HasForwardHistory; }
        }
        public void ClearNavigationHistory() {
    		DisplayRoot.WrappingOffset= Vector2.zero;
            NavigationHistory.Clear();
            DisplayRoot= EditorObjects[0];
    		DisplayRoot.WrappingOffset= Vector2.zero;
            ForcedRelayoutOfTree();
        }
    }

}

