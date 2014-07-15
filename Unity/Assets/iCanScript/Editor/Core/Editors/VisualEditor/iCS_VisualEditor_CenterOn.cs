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
    public void ReframeOn(iCS_EditorObject target, Vector2 targetPos) {
        return;
//        var initialScrollPosition= ScrollPosition;
//        var initialScale= Scale;
//        float deltaTime= Prefs.AnimationTime;
//        Vector2 pivot= Vector2.zero;
        // Reposition Target.
        var newPos= target.GlobalPosition;
        if(Math3D.IsNotEqual(targetPos, newPos)){
            ScrollPosition+= newPos-targetPos;
        }   
        // Display entire graph if it easily fits in viewport.
        var displayRootScale= ProposeViewportScalingFor(IStorage.DisplayRoot, 0f, 2f);
        if(Math3D.IsWithin(displayRootScale, 0.75f, 2f)) {
            Debug.Log("Centering on Display Root");
            CenterAndScaleOn(IStorage.DisplayRoot);
//            Scale= displayRootScale;
//            pivot= IStorage.DisplayRoot.LayoutPosition;
//            initialScrollPosition= ScrollPosition + (Scale-initialScale)*pivot;
//            ScrollPosition= initialScrollPosition;
//            RepositionInViewport(IStorage.DisplayRoot);
//            myAnimatedScrollPosition.Start(initialScrollPosition, ScrollPosition, deltaTime, (start,end,ratio)=> Math3D.Lerp(start, end, ratio));
            return;
        }
        // Move the focus node to the parent if it is the only child node.
        var focusNode= target.ParentNode;
        if(focusNode == null) focusNode= target;
        var focusParent= focusNode.ParentNode;
        while(focusNode != IStorage.DisplayRoot && focusParent != null &&
              focusParent.NumberOfChildNodes() == 1 && !focusParent.IsBehaviour) {
            focusNode= focusParent;
            focusParent= focusNode.ParentNode;
        }
        // Determine if we should rescale to fit the focus node.
        var newScale= ProposeViewportScalingFor(focusNode, 0.5f, 2f);
        if(Math3D.IsWithin(newScale, 0.5f, 2f)) {
            Debug.Log("Centering on=> "+focusNode.Name);
//            CenterAndScaleOn(focusNode);
            ScaleOnPivot(focusNode.GlobalPosition, newScale);
//            RepositionInViewport(focusNode);
            return;
        }
        Debug.Log("Reframing on=> "+focusNode.Name);
//        pivot= IStorage.DisplayRoot.LayoutPosition;
//        initialScrollPosition= ScrollPosition + (Scale-initialScale)*pivot;
        Scale= newScale;
//        pivot= IStorage.DisplayRoot.LayoutPosition;
//        initialScrollPosition= ScrollPosition + (Scale-initialScale)*pivot;
//        ScrollPosition= initialScrollPosition;
        RepositionInViewport(focusNode);
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
    public float ProposeViewportScalingFor(iCS_EditorObject node, float minScale= 0.50f, float maxScale= 1.5f) {
        var viewportRect= VisibleGraphRectWithPadding;
        var nodeSize= node.LocalSize;
        var xScale= Scale * viewportRect.width / nodeSize.x;
        var yScale= Scale * viewportRect.height / nodeSize.y;
        var newScale= Mathf.Min(xScale, yScale);
        if(newScale < minScale) newScale= minScale;
        if(newScale > maxScale) newScale= maxScale;
        return newScale;
    }
	// ----------------------------------------------------------------------
    public void RepositionInViewport(iCS_EditorObject node) {
        var nodeRect  = node.GlobalRect;
        var nodeCenter= Math3D.Middle(nodeRect);
        var viewportRect  = VisibleGraphRectWithPadding;
        var viewportCenter= Math3D.Middle(viewportRect);
        // Should center vertically
        if(nodeRect.width < viewportRect.width) {
            ScrollPosition+= new Vector2(nodeCenter.x - viewportCenter.x, 0);
            viewportRect  = VisibleGraphRectWithPadding;
            viewportCenter= Math3D.Middle(viewportRect);
        }
        // Should move left
        if(nodeRect.x > viewportRect.x && nodeRect.xMax > viewportRect.xMax) {
            var leftOffset= nodeRect.x-viewportRect.x;
            var rightOffset= nodeRect.xMax-viewportRect.xMax;
            var xOffset= Mathf.Min(leftOffset, rightOffset);
            ScrollPosition+= new Vector2(xOffset, 0);
            viewportRect  = VisibleGraphRectWithPadding;
            viewportCenter= Math3D.Middle(viewportRect);
        }
        // Should move right
        if(nodeRect.xMax < viewportRect.xMax && nodeRect.x < viewportRect.x) {
            var leftOffset= viewportRect.x-nodeRect.x;
            var rightOffset= viewportRect.xMax-nodeRect.xMax;
            var xOffset= Mathf.Min(leftOffset, rightOffset);
            ScrollPosition+= new Vector2(-xOffset, 0);
            viewportRect  = VisibleGraphRectWithPadding;
            viewportCenter= Math3D.Middle(viewportRect);
        }
        // Should center horizontally
        if(nodeRect.height < viewportRect.height) {
            ScrollPosition+= new Vector2(0, nodeCenter.y - viewportCenter.y);
            viewportRect  = VisibleGraphRectWithPadding;
            viewportCenter= Math3D.Middle(viewportRect);
        }
        // Should move top
        if(nodeRect.y > viewportRect.y && nodeRect.yMax > viewportRect.yMax) {
            var upOffset= nodeRect.y-viewportRect.y;
            var downOffset= nodeRect.yMax-viewportRect.yMax;
            var yOffset= Mathf.Min(upOffset, downOffset);
            ScrollPosition+= new Vector2(0, yOffset);
            viewportRect  = VisibleGraphRectWithPadding;
            viewportCenter= Math3D.Middle(viewportRect);
        }
        // Should move bottom
        if(nodeRect.yMax < viewportRect.yMax && nodeRect.y < viewportRect.y) {
            var upOffset= viewportRect.y-nodeRect.y;
            var downOffset= viewportRect.yMax-nodeRect.yMax;
            var yOffset= Mathf.Min(upOffset, downOffset);
            ScrollPosition+= new Vector2(0, -yOffset);
            viewportRect  = VisibleGraphRectWithPadding;
            viewportCenter= Math3D.Middle(viewportRect);
        }
    }
}
