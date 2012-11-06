using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
    public bool IsIconizedOnDisplay(iCS_EditorObject eObj)  { return IsIconized(eObj); }
    public bool IsUnfoldedOnDisplay(iCS_EditorObject eObj)  { return IsUnfolded(eObj); }
    public bool IsFoldedOnDisplay(iCS_EditorObject eObj)    { return IsFolded(eObj); }
    public bool IsVisibleOnDisplay(iCS_EditorObject eObj)   { return IsVisible(eObj); }
    public bool IsIconizedInLayout(iCS_EditorObject eObj)   { return IsIconized(eObj); }
    public bool IsUnfoldedInLayout(iCS_EditorObject eObj)   { return IsUnfolded(eObj); }
    public bool IsFoldedInLayout(iCS_EditorObject eObj)     { return IsFolded(eObj); }
    public bool IsVisibleInLayout(iCS_EditorObject eObj)    { return IsVisible(eObj); }
    
    // ======================================================================
    // Display Options
    // ----------------------------------------------------------------------
    public bool IsVisible(iCS_EditorObject eObj) {
        if(!IsValid(eObj.ParentId)) return true;
        iCS_EditorObject parent= eObj.Parent;
        if(eObj.IsNode && (parent.IsFolded || parent.IsIconized)) return false;
		if(eObj.IsDataPort && (parent.IsDataPort || parent.IsIconized)) return false;
        return IsVisible(parent);
    }
    public bool IsVisible(int id) { return !IsValid(id) ? false : IsVisible(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public bool IsFolded(iCS_EditorObject eObj) { return eObj.IsFolded; }
    // ----------------------------------------------------------------------
    public void Fold(iCS_EditorObject eObj) {
        if(!eObj.IsNode) return;    // Only nodes can be folded.
        if(eObj.IsFunction) {
            Unfold(eObj);
            SetDirty(eObj);
            return;
        }
        eObj.Fold();
        ForEachChild(eObj, child=> { if(child.IsPort) child.Unfold(); });
        SetDirty(eObj);
    }
    public void Fold(int id) { if(IsValid(id)) Fold(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public bool IsIconized(iCS_EditorObject eObj) {
        return eObj.IsIconized;
    }
    public void Iconize(iCS_EditorObject eObj) {
        if(!eObj.IsNode) return;
        eObj.Iconize();
        ForEachChild(eObj, child=> { if(child.IsPort) child.Iconize(); });
        SetDirty(eObj);
        if(IsValid(eObj.ParentId)) {
            SetDirty(eObj.Parent);
        }
    }
    public void Iconize(int id) { if(IsValid(id)) Iconize(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public bool IsUnfolded(iCS_EditorObject eObj) { return eObj.IsUnfolded; }
    public void Unfold(iCS_EditorObject eObj) {
        if(!eObj.IsNode) return;
        eObj.Unfold();
        ForEachChild(eObj, child=> { if(child.IsPort) child.Unfold(); });
        NodeLayout(eObj,true);
        if(IsValid(eObj.ParentId)) {
            iCS_EditorObject parent= eObj.Parent;
            SetDirty(parent);
        }
    }
    public void Unfold(int id) { if(IsValid(id)) Unfold(EditorObjects[id]); }

}
