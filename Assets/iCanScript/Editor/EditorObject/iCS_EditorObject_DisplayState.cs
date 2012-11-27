using UnityEngine;
using System.Collections;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  DISPLAY STATE
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // ======================================================================
    public void Unfold()            { DisplayOption= iCS_DisplayOptionEnum.Unfolded; IsDirty= true; }
    public void Fold()              { DisplayOption= iCS_DisplayOptionEnum.Folded;   IsDirty= true; }
    public void Iconize()           { DisplayOption= iCS_DisplayOptionEnum.Iconized; IsDirty= true; }
    public bool IsUnfolded          { get { return DisplayOption == iCS_DisplayOptionEnum.Unfolded; }}
    public bool IsFolded            { get { return DisplayOption == iCS_DisplayOptionEnum.Folded;   }}
    public bool IsIconized          { get { return DisplayOption == iCS_DisplayOptionEnum.Iconized; }}

    public bool IsVisible {
        get {
            var parent= Parent;
            if(parent == null) return true;    
            if(parent.IsIconized) return false;
            if(IsNode && parent.IsFolded) return false;
            return parent.IsVisible;            
        }
    }

}
