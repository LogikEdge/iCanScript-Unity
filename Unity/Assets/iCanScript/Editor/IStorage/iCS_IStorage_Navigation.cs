using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
    // ======================================================================
    // ----------------------------------------------------------------------
    public void SaveNavigationState() {
        NavigationHistory.Save(Storage);
    }
    public void ReloadNavigationFromBackwardHistory() {
        NavigationHistory.ReloadFromBackwardHistory(Storage);
        ForcedRelayoutOfTree(DisplayRoot);
    }
    public void ReloadNavigationFromForwardHistory() {
        NavigationHistory.ReloadFromForwardHistory(Storage);
        ForcedRelayoutOfTree(DisplayRoot);
    }
    public bool HasNavigationBackwardHistory {
        get { return NavigationHistory.HasBackwardHistory; }
    }
    public bool HasNavigationForwardHistory {
        get { return NavigationHistory.HasForwardHistory; }
    }
    public void ClearNavigationHistory() {
        NavigationHistory.Clear();
        DisplayRoot= EditorObjects[0];
        ForcedRelayoutOfTree(DisplayRoot);
    }
}

