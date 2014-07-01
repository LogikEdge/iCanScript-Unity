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
        CenterAt(obj.LayoutPosition);
    }
	// ----------------------------------------------------------------------
    public void CenterAndScaleOn(iCS_EditorObject obj) {
        if(obj == null || IStorage == null) return;
        while(obj != null && !obj.IsVisibleInLayout) obj= obj.Parent;
        if(obj == null) return;
        var size= obj.LayoutSize;
        float newScale= 1.0f;
        if(obj.IsNode) {
            float borderScale= obj.IsBehaviour ? 1.0f : 1.1f;
            float widthScale= position.width/(borderScale*size.x);
            float heightScale= position.height/(borderScale*size.y);
            newScale= Mathf.Min(2.0f, Mathf.Min(widthScale, heightScale));
        }
        CenterAtWithScale(obj.LayoutPosition, newScale);
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
    public void ScrollBy(Vector2 point) {
        if(IStorage == null) return;
        Vector2 newScrollPosition= ScrollPosition+point;
        float deltaTime= Prefs.AnimationTime;
        myAnimatedScrollPosition.Start(ScrollPosition, newScrollPosition, deltaTime, (start,end,ratio)=> Math3D.Lerp(start, end, ratio));
        ScrollPosition= newScrollPosition;        
    }
	// ----------------------------------------------------------------------
    public void ScaleTo(float newScale) {
        if(IStorage == null) return;
        float deltaTime= Prefs.AnimationTime;
        myAnimatedScale.Start(Scale, newScale, deltaTime, (start,end,ratio)=> Math3D.Lerp(start, end, ratio));
        Scale= newScale;        
    }
	// ----------------------------------------------------------------------
    public void MakeVisibleInViewport(iCS_EditorObject obj) {
        var r= obj.LayoutRect;
        var clipArea= ClipingArea;
        // Try to make obj visible in viewport.
        var intersection= Math3D.Intersection(r, clipArea);
        if(Math3D.IsNotEqual(r, intersection)) {
            // By default, focus on parent
            var parent= obj.ParentNode;
            if(parent == null) {
                parent= obj;
            }
            CenterAndScaleOn(parent);
        }
//        // Scale up if visual script is too small.
//        var displayRootArea= Math3D.Area(DisplayRoot.LayoutRect);
//        var viewportArea   = Math3D.Area(clipArea);
//        if(displayRootArea < viewportArea*0.25f) {
//            CenterAndScaleOn(DisplayRoot);
//        }
    }
	// ----------------------------------------------------------------------
    public void ReduceEmptyViewport() {
        var displayRootRect= DisplayRoot.LayoutRect;
        var viewportRect= ClipingArea;
        // Attempt to adjust size
        var lowerScale= 0.75f;
        var higherScale= 1.5f;
        bool IsScaleAdjusted= false;
        if(Scale > lowerScale) {
            var xScale= Scale;
            var yScale= Scale;
            if(displayRootRect.width > viewportRect.width) {
                xScale= Scale*viewportRect.width/displayRootRect.width;
            }
            if(displayRootRect.height > viewportRect.height) {
                yScale= Scale*viewportRect.height/displayRootRect.height;
            }
            var newScale= Mathf.Min(xScale, yScale);
            if(newScale < lowerScale) newScale= lowerScale;
            if(Math3D.IsNotEqual(Scale, newScale)) {
                ScaleTo(newScale);
                viewportRect= ClipingArea;
                IsScaleAdjusted= true;
            }
        }
        if(IsScaleAdjusted == false && Scale < higherScale) {
            var xScale= Scale;
            var yScale= Scale;
            if(displayRootRect.width < viewportRect.width) {
                xScale= Scale*viewportRect.width/displayRootRect.width;
            }
            if(displayRootRect.height < viewportRect.height) {
                yScale= Scale*viewportRect.height/displayRootRect.height;
            }
            var newScale= Mathf.Min(xScale, yScale);
            if(newScale > higherScale) newScale= higherScale;
            if(Math3D.IsNotEqual(Scale, newScale)) {
                ScaleTo(newScale);
                viewportRect= ClipingArea;
                IsScaleAdjusted= true;
            }            
        }
        // Should move left
        if(displayRootRect.x > viewportRect.x && displayRootRect.xMax > viewportRect.xMax) {
            var leftOffset= displayRootRect.x-viewportRect.x;
            var rightOffset= displayRootRect.xMax-viewportRect.xMax;
            var xOffset= Mathf.Min(leftOffset, rightOffset);
            ScrollBy(new Vector2(xOffset, 0));
        }
        // Should move right
        if(displayRootRect.xMax < viewportRect.xMax && displayRootRect.x < viewportRect.x) {
            var leftOffset= viewportRect.x-displayRootRect.x;
            var rightOffset= viewportRect.xMax-displayRootRect.xMax;
            var xOffset= Mathf.Min(leftOffset, rightOffset);
            ScrollBy(new Vector2(-xOffset, 0));
        }
        // Should move up
        if(displayRootRect.y > viewportRect.y && displayRootRect.yMax > viewportRect.yMax) {
            var upOffset= displayRootRect.y-viewportRect.y;
            var downOffset= displayRootRect.yMax-viewportRect.yMax;
            var yOffset= Mathf.Min(upOffset, downOffset);
            ScrollBy(new Vector2(0, yOffset));
        }
        // Should move down
        if(displayRootRect.yMax < viewportRect.yMax && displayRootRect.y < viewportRect.y) {
            var upOffset= viewportRect.y-displayRootRect.y;
            var downOffset= viewportRect.yMax-displayRootRect.yMax;
            var yOffset= Mathf.Min(upOffset, downOffset);
            ScrollBy(new Vector2(0, -yOffset));
        }
    }
}
