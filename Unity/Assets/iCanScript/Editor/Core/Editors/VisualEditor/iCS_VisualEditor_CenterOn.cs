using UnityEngine;
using UnityEditor;
using System.Collections;
using Prefs=iCS_PreferencesController;

public partial class iCS_VisualEditor : iCS_EditorBase {
	// ----------------------------------------------------------------------
    public void SmartFocusOn(iCS_EditorObject obj) {
        var focusNode= obj;
        // Focus on port parent.
        if(obj.IsPort) {
            focusNode= obj.ParentNode;
        }
        // Focus on parent:
        //   - if display option is iconized or folded;
        //   - node does not contain visual script
        if(focusNode.IsIconizedInLayout || focusNode.IsFoldedInLayout ||
           focusNode.IsInstanceNode || focusNode.IsKindOfFunction) {
            var parent= focusNode.ParentNode;
            if(parent != null) {
                focusNode= parent;
            }
        }
        // Move to higher parent if scale is too large.
        if(focusNode != IStorage.DisplayRoot) {
            var focusNodeParent= focusNode.ParentNode;
            if(focusNodeParent != null) {
                var scale= ProposeViewportScalingFor(focusNodeParent);            
                while(scale > 0.75f) {
                    focusNode= focusNodeParent;
                    focusNodeParent= focusNode.ParentNode;
                    if(focusNode == IStorage.DisplayRoot || focusNodeParent == null) break;
                    scale= ProposeViewportScalingFor(focusNodeParent);            
                }                
            }
        }
        // Center on focus node
        iCS_EditorUtility.SafeCenterOn(focusNode, IStorage);                        
    }
	// ----------------------------------------------------------------------
    public void CenterOnRoot() {
        CenterOn(DisplayRoot);            
    }
	// ----------------------------------------------------------------------
    public void CenterAndScaleOnRoot() {
        CenterAndScaleOn(DisplayRoot);
    }
	// ----------------------------------------------------------------------
    public void CenterOnSelected() {
        CenterOn(SelectedObject ?? DisplayRoot);
    }
	// ----------------------------------------------------------------------
    public void CenterAndScaleOnSelected() {
        CenterAndScaleOn(SelectedObject ?? DisplayRoot);
    }
	// ----------------------------------------------------------------------
    public void CenterOn(iCS_EditorObject obj) {
        if(obj == null || IStorage == null) return;
        CenterAt(obj.GlobalPosition);
    }
	// ----------------------------------------------------------------------
    public void CenterAndScaleOn(iCS_EditorObject obj) {
        if(obj == null || IStorage == null) return;
        while(obj != null && !obj.IsVisibleInLayout) obj= obj.Parent;
        if(obj == null) return;
        var size= obj.LocalSize;
        float newScale= 1.0f;
        if(obj.IsNode) {
            float borderScale= obj.IsBehaviour ? 1.0f : 1.1f;
            float widthScale= position.width/(borderScale*size.x);
            float heightScale= position.height/(borderScale*size.y);
            newScale= Mathf.Min(2.0f, Mathf.Min(widthScale, heightScale));
        }
        CenterAtWithScale(obj.GlobalPosition, newScale);
    }
	// ----------------------------------------------------------------------
    public void CenterAt(Vector2 point) {
        if(IStorage == null) return;
        Vector2 newScrollPosition= point-0.5f/Scale*new Vector2(position.width, position.height);
        float deltaTime= Prefs.AnimationTime;
        myAnimatedScrollPosition.Start(ScrollPosition, newScrollPosition, deltaTime, (start,end,ratio)=> Math3D.Lerp(start, end, ratio));
        ScrollPosition= newScrollPosition;
    }
	// ----------------------------------------------------------------------
    public void CenterAtWithScale(Vector2 point, float newScale) {
        if(IStorage == null) return;
        Vector2 newScrollPosition= point-0.5f/newScale*new Vector2(position.width, position.height);
        float deltaTime= Prefs.AnimationTime;
        myAnimatedScrollPosition.Start(ScrollPosition, newScrollPosition, deltaTime, (start,end,ratio)=> Math3D.Lerp(start, end, ratio));
        ScrollPosition= newScrollPosition;
        myAnimatedScale.Start(Scale, newScale, deltaTime, (start,end,ratio)=> Math3D.Lerp(start, end, ratio));
        Scale= newScale;
    }
	// ----------------------------------------------------------------------
    public void ScaleOnPivot(Vector2 pivot, float newScale) {
        float deltaTime= Prefs.AnimationTime;
        var newScrollPosition= pivot + (ScrollPosition - pivot) * Scale / newScale;
        myAnimatedScrollPosition.Start(ScrollPosition, newScrollPosition, deltaTime, (start,end,ratio)=> Math3D.Lerp(start, end, ratio));
        myAnimatedScale.Start(Scale, newScale, deltaTime, (start,end,ratio)=> Math3D.Lerp(start, end, ratio));
        ScrollPosition= newScrollPosition;
        Scale= newScale;
    }
	// ----------------------------------------------------------------------
    public bool IsNodeFullyVisibleInViewport(iCS_EditorObject node) {
        var r= node.GlobalRect;
        var clipArea= VisibleGraphRect;
        // Try to make obj visible in viewport.
        var intersection= Math3D.Intersection(r, clipArea);
        return Math3D.IsEqual(r, intersection);
    }
	// ----------------------------------------------------------------------
    public float ProposeViewportScalingFor(iCS_EditorObject node, float minScale, float maxScale) {
        var newScale= ProposeViewportScalingFor(node);
        if(newScale < minScale) newScale= minScale;
        if(newScale > maxScale) newScale= maxScale;
        return newScale;
    }
	// ----------------------------------------------------------------------
    public float ProposeViewportScalingFor(iCS_EditorObject node) {
        var viewportRect= VisibleGraphRectWithPadding;
        var nodeSize= node.LocalSize;
        var xScale= Scale * viewportRect.width / nodeSize.x;
        var yScale= Scale * viewportRect.height / nodeSize.y;
        return Mathf.Min(xScale, yScale);
    }
}
