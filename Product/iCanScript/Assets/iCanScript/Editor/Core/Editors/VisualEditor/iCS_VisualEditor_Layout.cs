using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor {
public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Properties.
    // ----------------------------------------------------------------------
    Rect VisibleGraphRect {
        get {
            return new Rect(ScrollPosition.x, ScrollPosition.y, Viewport.width, Viewport.height);
        }
    }
    Rect VisibleGraphRectWithPadding {
        get {
            const float kPaddingFactor= 0.08f;
            var clipingArea= VisibleGraphRect;
            var xPadding= clipingArea.width  * kPaddingFactor;
            var yPadding= clipingArea.height * kPaddingFactor;
            clipingArea.x+= 0.5f * xPadding;
            clipingArea.y+= 0.5f * yPadding;
            clipingArea.width -= xPadding;
            clipingArea.height-= yPadding;
            return clipingArea;
        }
    }
	// ----------------------------------------------------------------------
    public Vector2 ViewportCenter {
        get {
            return new Vector2(0.5f*position.width/Scale, 0.5f*position.height/Scale);
        }
    }
	// ----------------------------------------------------------------------
    Rect Viewport {
        get {
            return new Rect(0,0,position.width/Scale, position.height/Scale);
        }
    }
	// ----------------------------------------------------------------------
    Rect ViewportRectForGraph {
        get {
            float toolbarHeight= iCS_ToolbarUtility.GetHeight();
            return new Rect(0f, toolbarHeight, position.width, position.height-toolbarHeight);
            }
    }
	// ----------------------------------------------------------------------
    public Vector2 ViewportToGraph(Vector2 v) { return v+ScrollPosition; }
	// ----------------------------------------------------------------------
    Rect WindowRectForGraph {
        get {
            float headerHeight= iCS_ToolbarUtility.GetHeight();
            return new Rect(position.x, position.y+headerHeight, position.width, position.height-headerHeight);
            }
    }
	// ----------------------------------------------------------------------
    Rect WindowRectForHeader {
        get {
            float headerHeight= iCS_ToolbarUtility.GetHeight();
            return new Rect(position.x, position.y, position.width, headerHeight);
            }    
    }
    
}
}