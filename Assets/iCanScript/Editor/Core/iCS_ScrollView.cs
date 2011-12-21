using UnityEngine;
using System.Collections;

public class iCS_ScrollView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    Rect    ScreenArea    = new Rect(0,0,0,0);  // In screen coordonates
    Rect    Canvas        = new Rect(0,0,0,0);  // In graph coordonates
    Vector2 ScrollPosition= Vector2.zero;       // Offset in canvasjik

    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    Vector2 ScreenOrigin   { get { return new Vector2(ScreenArea.x, ScreenArea.y); }}
    Vector2 CanvasOrigin   { get { return new Vector2(Canvas.x, Canvas.y); }}
    Vector2 ViewportOrigin { get { return CanvasOrigin + ScrollPosition; }}
    
    // ----------------------------------------------------------------------
    public void Begin() {
        ScrollPosition= GUI.BeginScrollView(ScreenArea, ScrollPosition, Canvas);        
    }

    // ----------------------------------------------------------------------
    public void End() {
        GUI.EndScrollView();        
    }
    
    // ----------------------------------------------------------------------
    public void Update(Rect screenArea, Rect rootNodeRect) {
	    // Adjust scroll window bounds.
        ScreenArea= new Rect(0, iCS_EditorConfig.EditorWindowToolbarHeight, screenArea.width, screenArea.height-iCS_EditorConfig.EditorWindowToolbarHeight);

        // Update scroll viewport.
        Rect graphRect= new Rect(rootNodeRect.x - iCS_EditorConfig.GutterSize,
                                 rootNodeRect.y - iCS_EditorConfig.GutterSize,
                                 rootNodeRect.width + 2*iCS_EditorConfig.GutterSize,
                                 rootNodeRect.height + 2*iCS_EditorConfig.GutterSize);

        // Assure that the canvas covers the graph area.
        if(Canvas.x > graphRect.x) {
            float dx= Canvas.x-graphRect.x;
            Canvas.x= graphRect.x;
            ScrollPosition.x+= dx;
            Canvas.width+= dx;
        }
        if(Canvas.y > graphRect.y) {
            float dy= Canvas.y-graphRect.y;
            Canvas.y= graphRect.y;
            ScrollPosition.y+= dy;
            Canvas.height+= dy;
        }
		if(Canvas.xMax < graphRect.xMax) {
            Canvas.width+= graphRect.xMax - Canvas.xMax;
        }
        if(Canvas.yMax < graphRect.yMax) {
            Canvas.height+= graphRect.yMax - Canvas.yMax;
        }
        
        // Shrink so that it does not exceed the combined graph and screen area.
        if(!Math3D.IsZero(ScrollPosition.x) && Math3D.IsSmaller(Canvas.x+ScrollPosition.x, graphRect.x)) {
            Canvas.x+= ScrollPosition.x;
            Canvas.width-= ScrollPosition.x;
            ScrollPosition.x= 0;
        }
        if(!Math3D.IsZero(ScrollPosition.y) && Math3D.IsSmaller(Canvas.y+ScrollPosition.y, graphRect.y)) {
            Canvas.y+= ScrollPosition.y;
            Canvas.height-= ScrollPosition.y;
            ScrollPosition.y= 0;
        }
        if(Math3D.IsGreater(Canvas.xMax, graphRect.xMax)) {
            Canvas.width-= Canvas.xMax-graphRect.xMax;
        }
        if(Math3D.IsGreater(Canvas.yMax, graphRect.yMax)) {
            Canvas.height-= Canvas.yMax-graphRect.yMax;
        }
        
        // Adjust viewport width/height.
        if(Math3D.IsSmaller(Canvas.width, ScrollPosition.x+ScreenArea.width)) {
            Canvas.width= ScrollPosition.x+ScreenArea.width;            
        }
        if(Math3D.IsSmaller(Canvas.height, ScrollPosition.y+ScreenArea.height)) {
            Canvas.height= ScrollPosition.y+ScreenArea.height;                    
        }
    }
    
    // ----------------------------------------------------------------------
    // Convert's screen to graph coordinates.
    public Vector2 ScreenToGraph(Vector2 point) {
        Vector2 screenOffset= point - ScreenOrigin;
        return ViewportOrigin + screenOffset;
    }

    // ----------------------------------------------------------------------
    public void CenterAt(Vector2 graphPos) {
        Vector2 screenMiddleOffset= new Vector2(0.5f*ScreenArea.width, 0.5f*ScreenArea.height);
        ScrollPosition= graphPos - CanvasOrigin - screenMiddleOffset;
    }
}
