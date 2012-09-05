using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
    public bool IsMinimizedOnDisplay(iCS_EditorObject eObj) { return IsMinimized(eObj); }
    public bool IsMaximizedOnDisplay(iCS_EditorObject eObj) { return IsMaximized(eObj); }
    public bool IsFoldedOnDisplay(iCS_EditorObject eObj)    { return IsFolded(eObj); }
    public bool IsVisibleOnDisplay(iCS_EditorObject eObj)   { return IsVisible(eObj); }
    public bool IsMinimizedInLayout(iCS_EditorObject eObj)  { return IsMinimized(eObj); }
    public bool IsMaximizedInLayout(iCS_EditorObject eObj)  { return IsMaximized(eObj); }
    public bool IsFoldedInLayout(iCS_EditorObject eObj)     { return IsFolded(eObj); }
    public bool IsVisibleInLayout(iCS_EditorObject eObj)    { return IsVisible(eObj); }
    
    // ======================================================================
    // Display Options
    // ----------------------------------------------------------------------
    public bool IsVisible(iCS_EditorObject eObj) {
        if(IsInvalid(eObj.ParentId)) return true;
        iCS_EditorObject parent= GetParent(eObj);
        if(eObj.IsNode && (parent.IsFolded || parent.IsMinimized)) return false;
		if(eObj.IsDataPort && (parent.IsDataPort || parent.IsMinimized)) return false;
        return IsVisible(parent);
    }
    public bool IsVisible(int id) { return IsInvalid(id) ? false : IsVisible(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public bool IsFolded(iCS_EditorObject eObj) { return eObj.IsFolded; }
    // ----------------------------------------------------------------------
    public void Fold(iCS_EditorObject eObj) {
        if(!eObj.IsNode) return;    // Only nodes can be folded.
        if(eObj.IsFunction) {
            Maximize(eObj);
            SetDirty(eObj);
            return;
        }
        eObj.Fold();
        ForEachChild(eObj, child=> { if(child.IsPort) child.Maximize(); });
        SetDirty(eObj);
    }
    public void Fold(int id) { if(IsValid(id)) Fold(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public bool IsMinimized(iCS_EditorObject eObj) {
        return eObj.IsMinimized;
    }
    public void Minimize(iCS_EditorObject eObj) {
        if(!eObj.IsNode) return;
        eObj.Minimize();
        ForEachChild(eObj, child=> { if(child.IsPort) child.Minimize(); });
        SetDirty(eObj);
        if(IsValid(eObj.ParentId)) {
            SetDirty(GetParent(eObj));
        }
    }
    public void Minimize(int id) { if(IsValid(id)) Minimize(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public bool IsMaximized(iCS_EditorObject eObj) { return eObj.IsMaximized; }
    public void Maximize(iCS_EditorObject eObj) {
        if(!eObj.IsNode) return;
        eObj.Maximize();
        ForEachChild(eObj, child=> { if(child.IsPort) child.Maximize(); });
        NodeLayout(eObj,true);
        if(IsValid(eObj.ParentId)) {
            iCS_EditorObject parent= GetParent(eObj);
            SetDirty(parent);
        }
    }
    public void Maximize(int id) { if(IsValid(id)) Maximize(EditorObjects[id]); }

}
