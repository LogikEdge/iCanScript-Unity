using UnityEngine;
using System.Collections;
using P= iCanScript.Internal.Prelude;
using iCanScript;

namespace iCanScript.Internal.Editor {
    
    public class iCS_ScrollView {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
        Rect    ScreenArea    = new Rect(0,0,0,0);  // In screen coordonates
        Rect    Canvas        = new Rect(0,0,0,0);  // In graph coordonates
        Vector2 ScrollPosition= Vector2.zero;       // Offset in canvas

        P.Animate<Vector2>    ScrollPositionAnimation= new P.Animate<Vector2>(TimerService.EditorTime);
    
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
            // Animate the scroll position.
            if(ScrollPositionAnimation.IsActive) {
            	ScrollPositionAnimation.Update();
                ScrollPosition= ScrollPositionAnimation.CurrentValue;
            }
        
    	    // Adjust scroll window bounds.
            ScreenArea= new Rect(0, iCS_EditorConfig.EditorWindowToolbarHeight, screenArea.width, screenArea.height-iCS_EditorConfig.EditorWindowToolbarHeight);

            // Update scroll viewport.
            Rect graphRect= new Rect(rootNodeRect.x - iCS_EditorConfig.MarginSize,
                                     rootNodeRect.y - iCS_EditorConfig.MarginSize,
                                     rootNodeRect.width + 2*iCS_EditorConfig.MarginSize,
                                     rootNodeRect.height + 2*iCS_EditorConfig.MarginSize);

            // Assure that the canvas covers the graph area.
            if(Canvas.x > graphRect.x) {
                float dx= Canvas.x-graphRect.x;
                Canvas.x= graphRect.x;
                Canvas.width+= dx;
                ScrollPosition.x+= dx;
                if(ScrollPositionAnimation.IsActive) {
                    Vector2 tmp= ScrollPositionAnimation.TargetValue;
                    tmp.x+= dx;
                    ScrollPositionAnimation.TargetValue= tmp;
                    tmp= ScrollPositionAnimation.StartValue;
                    tmp.x+= dx;
                    ScrollPositionAnimation.StartValue= tmp;
                }
            }
            if(Canvas.y > graphRect.y) {
                float dy= Canvas.y-graphRect.y;
                Canvas.y= graphRect.y;
                Canvas.height+= dy;
                ScrollPosition.y+= dy;
                if(ScrollPositionAnimation.IsActive) {
                    Vector2 tmp= ScrollPositionAnimation.TargetValue;
                    tmp.y+= dy;
                    ScrollPositionAnimation.TargetValue= tmp;
                    tmp= ScrollPositionAnimation.StartValue;
                    tmp.y+= dy;
                    ScrollPositionAnimation.StartValue= tmp;
                }
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
                float dx= -ScrollPosition.x;
                ScrollPosition.x= 0;
                if(ScrollPositionAnimation.IsActive) {
                    Vector2 tmp= ScrollPositionAnimation.TargetValue;
                    tmp.x+= dx;
                    ScrollPositionAnimation.TargetValue= tmp;
                    tmp= ScrollPositionAnimation.StartValue;
                    tmp.x+= dx;
                    ScrollPositionAnimation.StartValue= tmp;
                }
            }
            if(!Math3D.IsZero(ScrollPosition.y) && Math3D.IsSmaller(Canvas.y+ScrollPosition.y, graphRect.y)) {
                Canvas.y+= ScrollPosition.y;
                Canvas.height-= ScrollPosition.y;
                float dy= -ScrollPosition.y;
                ScrollPosition.y= 0;
                if(ScrollPositionAnimation.IsActive) {
                    Vector2 tmp= ScrollPositionAnimation.TargetValue;
                    tmp.y+= dy;
                    ScrollPositionAnimation.TargetValue= tmp;
                    tmp= ScrollPositionAnimation.StartValue;
                    tmp.y+= dy;
                    ScrollPositionAnimation.StartValue= tmp;
                }
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
        public void CenterAt(Vector2 graphPos, float deltaTime= -1f) {
            Vector2 screenMiddleOffset= new Vector2(0.5f*ScreenArea.width, 0.5f*ScreenArea.height);
            Vector2 targetScrollPosition= graphPos - CanvasOrigin - screenMiddleOffset;
            StartScrollAnim(targetScrollPosition, deltaTime < -Mathf.Epsilon ? 0.4f : deltaTime);
        }

        // ----------------------------------------------------------------------
        public void StartScrollAnim(Vector2 targetScrollPosition, float deltaTime) {
            ScrollPositionAnimation.Start(ScrollPosition, targetScrollPosition, deltaTime,
                (s,e,r)=> Math3D.Lerp(s,e,r)
            );        
        }
    }
}

