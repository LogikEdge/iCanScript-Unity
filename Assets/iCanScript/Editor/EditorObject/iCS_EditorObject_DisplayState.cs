using UnityEngine;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {
    // ======================================================================
    // Display animation attributes -----------------------------------------
	private P.Animate<Rect>	myAnimatedPosition= new P.Animate<Rect>();
	public P.Animate<Rect> AnimatedPosition {
		get { return myAnimatedPosition; }
	}

    // Node layout attributes -----------------------------------------------
    public void Unfold()    { EngineObject.Unfold(); IsDirty= true; }
    public void Fold()      { EngineObject.Fold(); IsDirty= true; }
    public void Iconize()   { EngineObject.Iconize(); IsDirty= true; }
    public bool IsUnfolded  { get { return EngineObject.IsUnfolded; }}
    public bool IsFolded    { get { return EngineObject.IsFolded; }}
    public bool IsIconized  { get { return EngineObject.IsIconized; }}
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
