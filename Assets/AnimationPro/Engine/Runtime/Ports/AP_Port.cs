using UnityEngine;
using System.Collections;

public abstract class AP_Port : AP_Object {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public enum EdgeEnum { Top, Bottom, Right, Left };

    public           Vector2        LocalPosition  = Vector2.zero;
    public           bool           IsBeingDragged = false;
    public           EdgeEnum       Edge           = EdgeEnum.Left;

//#if UNITY_EDITOR
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// BEGIN EDITOR SECTION
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

    // ======================================================================
    // LAYOUT
    // ----------------------------------------------------------------------
    public override void DoLayout() {
        // Don't interfear with dragging.
        if(IsBeingDragged) return;
        AP_Node parentNode= Parent as AP_Node;
        if(!parentNode) return;
        Rect parentPosition= parentNode.Position;
        // Make certain that the port is on an edge.
        switch(Edge) {
            case EdgeEnum.Top:
                if(!MathfExt.IsZero(LocalPosition.y)) {
                    LocalPosition.y= 0;
                    Parent.IsEditorDirty= true;                    
                }
                if(LocalPosition.x > parentPosition.width) {
                    LocalPosition.x= parentPosition.width-AP_EditorConfig.PortSize;
                    Parent.IsEditorDirty= true;
                }
                break;
            case EdgeEnum.Bottom:
                if(MathfExt.IsNotEqual(LocalPosition.y, parentPosition.height)) {
                    LocalPosition.y= parentPosition.height;
                    Parent.IsEditorDirty= true;                    
                }
                if(LocalPosition.x > parentPosition.width) {
                    LocalPosition.x= parentPosition.width-AP_EditorConfig.PortSize;
                    Parent.IsEditorDirty= true;
                }
                break;
            case EdgeEnum.Left:
                if(!MathfExt.IsZero(LocalPosition.x)) {
                    LocalPosition.x= 0;
                    Parent.IsEditorDirty= true;                    
                }
                if(LocalPosition.y > parentPosition.height) {
                    LocalPosition.y= parentPosition.height-AP_EditorConfig.PortSize;
                    Parent.IsEditorDirty= true;
                }
                break;
            case EdgeEnum.Right:
                if(MathfExt.IsNotEqual(LocalPosition.x, parentPosition.width)) {
                    LocalPosition.x= parentPosition.width;
                    Parent.IsEditorDirty= true;                    
                }
                if(LocalPosition.y > parentPosition.height) {
                    LocalPosition.y= parentPosition.height-AP_EditorConfig.PortSize;
                    Parent.IsEditorDirty= true;
                }
                break;            
        }
    }
    // ----------------------------------------------------------------------
    public void SnapToParent() {
        AP_Node parentNode= Parent as AP_Node;
        float parentHeight= parentNode.Position.height;
        float parentWidth= parentNode.Position.width;
        float portRadius= AP_EditorConfig.PortRadius;
        if(MathfExt.IsWithin(LocalPosition.y, -portRadius, portRadius)) {
            Edge= EdgeEnum.Top;
        }        
        if(MathfExt.IsWithin(LocalPosition.y, parentHeight-portRadius, parentHeight+portRadius)) {
            Edge= EdgeEnum.Bottom;
        }
        if(MathfExt.IsWithin(LocalPosition.x, -portRadius, portRadius)) {
            Edge= EdgeEnum.Left;
        }
        if(MathfExt.IsWithin(LocalPosition.x, parentWidth-portRadius, parentWidth+portRadius)) {
            Edge= EdgeEnum.Right;
        }
        IsEditorDirty= true;
        Layout();
    }
    // ----------------------------------------------------------------------
    public bool IsOnTopEdge        { get { return Edge == EdgeEnum.Top; }}
    public bool IsOnBottomEdge     { get { return Edge == EdgeEnum.Bottom; }}
    public bool IsOnLeftEdge       { get { return Edge == EdgeEnum.Left; }}
    public bool IsOnRightEdge      { get { return Edge == EdgeEnum.Right; }}
    public bool IsOnHorizontalEdge { get { return IsOnTopEdge || IsOnBottomEdge; }}
    public bool IsOnVerticalEdge   { get { return IsOnLeftEdge || IsOnRightEdge; }}

    // ----------------------------------------------------------------------
    // Returns the minimal distance from the parent.
    public float GetDistanceFromParent() {
        AP_Node parentNode= Parent as AP_Node;
        if(parentNode.IsInside(Position)) return 0;
        Rect parentPosition= parentNode.Position;
        if(Position.x > parentPosition.xMin && Position.x < parentPosition.xMax) {
            return Mathf.Min(Mathf.Abs(Position.y-parentPosition.yMin),
                             Mathf.Abs(Position.y-parentPosition.yMax));
        }
        if(Position.y > parentPosition.yMin && Position.y < parentPosition.yMax) {
            return Mathf.Min(Mathf.Abs(Position.x-parentPosition.xMin),
                             Mathf.Abs(Position.x-parentPosition.xMax));
        }
        float distance= Vector2.Distance(Position, parentNode.GetTopLeftCorner());
        distance= Mathf.Min(distance, Vector2.Distance(Position, parentNode.GetTopRightCorner()));
        distance= Mathf.Min(distance, Vector2.Distance(Position, parentNode.GetBottomLeftCorner()));
        distance= Mathf.Min(distance, Vector2.Distance(Position, parentNode.GetBottomRightCorner()));
        return distance;
    }

    // ----------------------------------------------------------------------
    // Returns true if the distance to parent is less then twice the port size.
    public bool IsNearParent() {
        if(Parent == null) return false;
        return GetDistanceFromParent() <= AP_EditorConfig.PortSize*2;
    }

	// ----------------------------------------------------------------------
    public AP_Port GetOverlappingPort() {
        AP_Port foundPort= null;
        Top.ForEachRecursive<AP_Port>(
            (port)=> {
                if(port != this) {
                    float distance= Vector2.Distance(port.Position, Position);
                    if(distance <= 1.5*AP_EditorConfig.PortSize) {
                        foundPort= port;
                    }
                }
            }
        );
        return foundPort;
    }	

    // ----------------------------------------------------------------------
    public Vector2  Position {
        get {
            AP_Node parentNode= Parent as AP_Node;
            return new Vector2(parentNode.Position.x, parentNode.Position.y) + LocalPosition;
        }
    }
//#endif

}
