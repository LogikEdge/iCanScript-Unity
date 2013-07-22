using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
    // ======================================================================
    // Display Options
    // ----------------------------------------------------------------------
    public void Fold(iCS_EditorObject eObj) {
        if(!eObj.IsNode) return;    // Only nodes can be folded.
        if(eObj.IsKindOfFunction || eObj.IsInstanceNode) {
            Unfold(eObj);
            return;
        }
        eObj.Fold();
    }
    public void Fold(int id) { if(IsValid(id)) Fold(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public void Iconize(iCS_EditorObject eObj) {
        if(!eObj.IsNode) return;
        eObj.Iconize();
    }
    public void Iconize(int id) { if(IsValid(id)) Iconize(EditorObjects[id]); }
    // ----------------------------------------------------------------------
    public void Unfold(iCS_EditorObject eObj) {
        if(!eObj.IsNode) return;
        eObj.Unfold();
    }
    public void Unfold(int id) { if(IsValid(id)) Unfold(EditorObjects[id]); }

}
