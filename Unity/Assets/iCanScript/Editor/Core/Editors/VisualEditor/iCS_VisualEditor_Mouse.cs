using UnityEngine;
using System.Collections;

public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Fields
	// ----------------------------------------------------------------------
	Vector2 myMousePosition  = Vector2.zero;
	Vector2 MouseDownPosition= Vector2.zero;
	float   MouseUpTime      = 0f;

    // ======================================================================
    // Properties
	// ----------------------------------------------------------------------
    Vector2 WindowMousePosition		{ get { return myMousePosition; }}
    Vector2 ViewportMousePosition	{ get { return myMousePosition/Scale; } }
    Vector2 GraphMousePosition 		{ get { return ViewportToGraph(ViewportMousePosition); }}
	bool    IsMouseInWindow         {
	    get {
            var mousePosition= Event.current.mousePosition;
            return mousePosition.x >= 0 && mousePosition.x < position.width &&
                   mousePosition.y >= 0 && mousePosition.y < position.height;
	    }
	}
	bool    IsMouseInToolbar         {
	    get {
            var mousePosition= Event.current.mousePosition;
	        return mousePosition.x >= 0 && mousePosition.x < position.width &&
	               mousePosition.y >= 0 && mousePosition.y < iCS_ToolbarUtility.GetHeight();
	    }
	}
	
	// ----------------------------------------------------------------------
	void UpdateMouse() {
        if(IsMouseInWindow) {
            myMousePosition= Event.current.mousePosition;            
        }
	}
}
