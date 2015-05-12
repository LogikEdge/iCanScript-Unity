using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Internal.Editor {
    using Prefs= PreferencesController;

    public partial class iCS_VisualEditor : iCS_EditorBase {
        // ======================================================================
        // SCROLL ZONE
    	// ----------------------------------------------------------------------
        void ProcessScrollZone() {
            // Compute the amount of scroll needed.
            var dir= CanScrollInDirection(DetectScrollZone());
            if(Math3D.IsZero(dir)) return;
            dir*= Prefs.EdgeScrollSpeed*myDeltaTime;
    
            // Adjust according to scroll zone.
            switch(DragType) {
                case DragTypeEnum.PortConnection:
                case DragTypeEnum.TransitionCreation: {
                    MouseDragStartPosition-= dir;
                    ScrollPosition= ScrollPosition+dir;
                    ProcessDrag();
    				myNeedRepaint= true;
                    break;
                }
                default: break;
            }
        }
    	// ----------------------------------------------------------------------
        void DrawScrollZone() {
            var dir= CanScrollInDirection(DetectScrollZone());
            if(Math3D.IsZero(dir)) return;
            ShowScrollButton(dir);
        }
    	// ----------------------------------------------------------------------
        bool IsInScrollZone() {
            return Math3D.IsNotZero(DetectScrollZone());
        }
    	// ----------------------------------------------------------------------
        const float scrollButtonSize=24f;
        Vector2 DetectScrollZone() {
            Vector2 direction= Vector2.zero;
            float headerHeight= iCS_ToolbarUtility.GetHeight();
            Rect rect= new Rect(0,headerHeight,position.width,position.height-headerHeight);
            if(!rect.Contains(WindowMousePosition)) return direction;
            if(position.width < 3f*scrollButtonSize || position.height < 3f*scrollButtonSize) return direction;
            if(WindowMousePosition.x < scrollButtonSize) {
                direction.x= -(scrollButtonSize-WindowMousePosition.x)/scrollButtonSize;
            }
            if(WindowMousePosition.x > position.width-scrollButtonSize) {
                direction.x= (WindowMousePosition.x-position.width+scrollButtonSize)/scrollButtonSize;
            }
            if(WindowMousePosition.y < scrollButtonSize+headerHeight) {
                direction.y= -(scrollButtonSize+headerHeight-WindowMousePosition.y)/scrollButtonSize;
            }
            if(WindowMousePosition.y > position.height-scrollButtonSize) {
                direction.y= (WindowMousePosition.y-position.height+scrollButtonSize)/scrollButtonSize;
            }
            return direction;        
        }
    	// ----------------------------------------------------------------------
        Vector2 CanScrollInDirection(Vector2 dir) {
            Rect rootRect= DisplayRoot.GlobalRect;
            var rootCenter= Math3D.Middle(rootRect);
            var topLeftCorner= ViewportToGraph(new Vector2(0, iCS_ToolbarUtility.GetHeight()/Scale));
            var bottomRightCorner= ViewportToGraph(new Vector2(position.width/Scale, position.height/Scale));
            if(Math3D.IsSmaller(dir.x, 0f)) {
                if(!rootRect.Contains(new Vector2(topLeftCorner.x, rootCenter.y))) {
                    dir.x= 0f;
                }
            }
            if(Math3D.IsGreater(dir.x,0f)) {
                if(!rootRect.Contains(new Vector2(bottomRightCorner.x, rootCenter.y))) {
                    dir.x= 0f;
                }
            }
            if(Math3D.IsSmaller(dir.y, 0f)) {
                if(!rootRect.Contains(new Vector2(rootCenter.x, topLeftCorner.y))) {
                    dir.y= 0f;
                }
            }
            if(Math3D.IsGreater(dir.y,0f)) {
                if(!rootRect.Contains(new Vector2(rootCenter.x, bottomRightCorner.y))) {
                    dir.y= 0f;
                }
            }
            return dir;
        }
    	// ----------------------------------------------------------------------
        void ShowScrollButton(Vector2 direction) {
            if(Math3D.IsZero(direction)) return;
            float headerHeight= iCS_ToolbarUtility.GetHeight();
            Rect rect= new Rect(0,headerHeight,position.width,position.height-headerHeight);
            if(Math3D.IsSmaller(direction.x, 0f)) {
                rect= Math3D.Intersection(rect, new Rect(0, 0, scrollButtonSize, position.height-1f));            
            }
            if(Math3D.IsGreater(direction.x, 0f)) {
                rect= Math3D.Intersection(rect, new Rect(position.width-scrollButtonSize, 0, scrollButtonSize-2f, position.height-1f));            
            }
            if(Math3D.IsSmaller(direction.y, 0f)) {
                rect= Math3D.Intersection(rect, new Rect(0, headerHeight, position.width-2f, scrollButtonSize-1f));
            }
            if(Math3D.IsGreater(direction.y, 0f)) {
                rect= Math3D.Intersection(rect, new Rect(0, position.height-scrollButtonSize, position.width-2f, scrollButtonSize-1f));
            }
            Color backgroundColor= new Color(1f,1f,1f,0.06f);
            iCS_Graphics.DrawRect(rect, backgroundColor, backgroundColor);
            // Draw arrow head
            direction.Normalize();
            Vector3[] tv= new Vector3[4];
            tv[0]= 0.4f*scrollButtonSize * direction;            
            Quaternion q1= Quaternion.AngleAxis(90f, Vector3.forward);
            tv[1]= q1*tv[0];
            Quaternion q2= Quaternion.AngleAxis(270f, Vector3.forward);
            tv[2]= q2*tv[0];
            tv[3]= tv[0];
            var center= Math3D.Middle(rect);
            for(int i= 0; i < 4; ++i) {
                tv[i].x+= center.x-0.2f*scrollButtonSize*direction.x;
                tv[i].y+= center.y-0.2f*scrollButtonSize*direction.y;
            }
            Color arrowColor= new Color(1f,1f,1f,0.5f);
            Handles.DrawSolidRectangleWithOutline(tv, arrowColor, arrowColor);
        }
    }
}