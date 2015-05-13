using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Editor {
    
    public abstract class DSView {
        // ======================================================================
        // Types
        // ----------------------------------------------------------------------
    	public enum AnchorEnum { TopLeft, TopCenter, TopRight,
    		                     CenterLeft, Center, CenterRight,
    						     BottomLeft, BottomCenter, BottomRight };
	
        // ======================================================================
        // Common view constants
        // ----------------------------------------------------------------------
        public const float   kHorizontalSpacer= 8f;
        public const float   kVerticalSpacer  = 8f;
        public const float   kHorizontalMargin= 10f;
        public const float   kVerticalMargin  = 10f;
        public const float   kScrollbarSize   = 15f;

        // ======================================================================
        // Properties
        // ----------------------------------------------------------------------
        public AnchorEnum Anchor {
            get { return GetAnchor(); }
            set { SetAnchor(value); }
        }
    
        // ======================================================================
        // Initialization
        // ----------------------------------------------------------------------
        public DSView() {}
    
        // ======================================================================
        // Functions to override to create a concrete view.
        // ----------------------------------------------------------------------
    	public abstract void        Display(Rect frameArea);
        public abstract Vector2     GetSizeToDisplay(Rect frameArea);
        public abstract AnchorEnum  GetAnchor();
        public abstract void        SetAnchor(AnchorEnum anchor);
    }
}
