using UnityEngine;
using System.Collections;

public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Properties.
    // ----------------------------------------------------------------------
    Rect ClipingArea {
        get {
            return new Rect(ScrollPosition.x, ScrollPosition.y, Viewport.width, Viewport.height);
        }
    }
	// ----------------------------------------------------------------------
    Vector2 ViewportCenter {
        get {
            return new Vector2(0.5f/Scale*position.width, 0.5f/Scale*position.height);
        }
    }
	// ----------------------------------------------------------------------
    Rect Viewport {
        get {
            return new Rect(0,0,position.width/Scale, position.height/Scale);
        }
    }
	// ----------------------------------------------------------------------
    Vector2 ViewportToGraph(Vector2 v) { return v+ScrollPosition; }
	// ----------------------------------------------------------------------
    Rect GraphArea {
        get {
            float headerHeight= iCS_ToolbarUtility.GetHeight();
            return new Rect(position.x, position.y+headerHeight, position.width, position.height-headerHeight);
            }
    }
	// ----------------------------------------------------------------------
    Rect HeaderArea {
        get {
            float headerHeight= iCS_ToolbarUtility.GetHeight();
            return new Rect(position.x, position.y, position.width, headerHeight);
            }    
    }
	// ----------------------------------------------------------------------
    float UsableWindowWidth {
        get {
            return position.width-2*iCS_Config.EditorWindowGutterSize;            
        }
    }    
	// ----------------------------------------------------------------------
    float UsableWindowHeight {
        get {
            return position.height-2*iCS_Config.EditorWindowGutterSize+iCS_Config.EditorWindowToolbarHeight;
            
        }
    }
    
}
