using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
    // ======================================================================
    // ----------------------------------------------------------------------
    public void SaveNavigationState() {
        NavigationHistory.Save(Storage);
    }
    public void ReloadFromBackwardNavigationHistory() {
        DisplayRoot.ReduceChildrenAnchorPosition();
        NavigationHistory.ReloadFromBackwardHistory(Storage);
        ForcedRelayoutOfTree(DisplayRoot);
    }
    public void ReloadFromForwardNavigationHistory() {
        NavigationHistory.ReloadFromForwardHistory(Storage);
        ForcedRelayoutOfTree(DisplayRoot);
    }
    public bool HasBackwardNavigationHistory {
        get { return NavigationHistory.HasBackwardHistory; }
    }
    public bool HasForwardNavigationHistory {
        get { return NavigationHistory.HasForwardHistory; }
    }
    public void ClearNavigationHistory() {
        NavigationHistory.Clear();
        DisplayRoot= EditorObjects[0];
        ForcedRelayoutOfTree(DisplayRoot);
    }
}

