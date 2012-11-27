using UnityEngine;
using System.Collections;
using P=Prelude;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  ANIMATED LAYOUT
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // Fields ========================================================
	private P.Animate<Rect>	myAnimatedPosition= new P.Animate<Rect>();

    // Accessors =====================================================
	public P.Animate<Rect> AnimatedPosition {
		get { return myAnimatedPosition; }
	}
}
