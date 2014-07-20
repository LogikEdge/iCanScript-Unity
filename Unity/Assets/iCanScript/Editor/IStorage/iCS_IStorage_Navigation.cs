using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
    // ======================================================================
    // ----------------------------------------------------------------------
    public void SaveNavigationState() {
        NavigationHistory.Save(Storage, SelectedObject.GlobalPosition);
    }
    public void ReloadFromBackwardNavigationHistory() {
        var selObjGlobalPosition= SelectedObject.GlobalPosition;
		DisplayRoot.WrappingOffset= Vector2.zero;
        DisplayRoot.ReduceChildrenAnchorPosition();
        selObjGlobalPosition= NavigationHistory.ReloadFromBackwardHistory(Storage, selObjGlobalPosition);
		DisplayRoot.WrappingOffset= Vector2.zero;
        ForcedRelayoutOfTree(SelectedObject, selObjGlobalPosition);
    }
    public void ReloadFromForwardNavigationHistory() {
        var selObjGlobalPosition= SelectedObject.GlobalPosition;
		DisplayRoot.WrappingOffset= Vector2.zero;
        selObjGlobalPosition= NavigationHistory.ReloadFromForwardHistory(Storage, selObjGlobalPosition);
		DisplayRoot.WrappingOffset= Vector2.zero;
        ForcedRelayoutOfTree(SelectedObject, selObjGlobalPosition);
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

