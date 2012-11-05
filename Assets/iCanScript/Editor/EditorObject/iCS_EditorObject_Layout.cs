using UnityEngine;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
	private P.Animate<Rect>	myAnimatedPosition= new P.Animate<Rect>();
	public P.Animate<Rect> AnimatedPosition {
		get { return myAnimatedPosition; }
	}

    // Node layout attributes -----------------------------------------------
    public bool IsUnfolded                  { get { return EngineObject.IsUnfolded; }}
    public bool IsFolded                    { get { return EngineObject.IsFolded; }}
    public bool IsIconized                  { get { return EngineObject.IsIconized; }}
    public void Unfold()                    { EngineObject.Unfold(); }
    public void Fold()                      { EngineObject.Fold(); }
    public void Iconize()                   { EngineObject.Iconize(); }
    

}
