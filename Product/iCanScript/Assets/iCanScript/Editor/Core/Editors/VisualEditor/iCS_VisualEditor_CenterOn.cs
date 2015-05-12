using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Internal.Editor {
    using Prefs= PreferencesController;

    public partial class iCS_VisualEditor : iCS_EditorBase {
    	// ----------------------------------------------------------------------
        public void SmartFocusOn(iCS_EditorObject obj) {
            if(IStorage == null) return;
            var focusNode= obj;
            // Focus on port parent.
            if(obj.IsPort) {
                focusNode= obj.ParentNode;
            }
            // Move to higher parent if scale is too large.
            float scale= ProposeViewportScalingFor(focusNode, 0.75f, 1f);
            if(Math3D.IsSmallerOrEqual(scale, 0.75f)) {
                CenterAndScaleOn(focusNode);
                return;
            }
            if(focusNode != IStorage.DisplayRoot) {
                var focusNodeParent= focusNode.ParentNode;
                if(focusNodeParent != null) {
                    var parentScale= ProposeViewportScalingFor(focusNodeParent, 0.80f, 1f);
                    while(parentScale > 0.80f) {
                        scale= parentScale;
                        focusNode= focusNodeParent;
                        focusNodeParent= focusNode.ParentNode;
                        if(focusNode == IStorage.DisplayRoot || focusNodeParent == null) break;
                        parentScale= ProposeViewportScalingFor(focusNodeParent, 0.80f, 1f);            
                    }                
                }
            }
            // Center on focus node
            if(focusNode == IStorage.DisplayRoot) {
                CenterAndScaleOnRoot();
            }
            else {
                CenterAtWithScale(focusNode.GlobalPosition, scale);            
            }
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
            AnimateScrollPosition(ScrollPosition, newScrollPosition, deltaTime);
            iCS_UserCommands.SetScrollPosition(IStorage, newScrollPosition);
        }
    	// ----------------------------------------------------------------------
        public void CenterAtWithScale(Vector2 point, float newScale) {
            if(IStorage == null) return;
            Vector2 newScrollPosition= point-0.5f/newScale*new Vector2(position.width, position.height);
            float deltaTime= Prefs.AnimationTime;
            AnimateScrollPosition(ScrollPosition, newScrollPosition, deltaTime);
            iCS_UserCommands.SetScrollPosition(IStorage, newScrollPosition);
            AnimateScale(Scale, newScale, deltaTime);
            iCS_UserCommands.SetZoom(IStorage, newScale);
        }
    	// ----------------------------------------------------------------------
        public void ScaleOnPivot(Vector2 pivot, float newScale) {
            float deltaTime= Prefs.AnimationTime;
            var newScrollPosition= pivot + (ScrollPosition - pivot) * Scale / newScale;
            AnimateScrollPosition(ScrollPosition, newScrollPosition, deltaTime);
            AnimateScale(Scale, newScale, deltaTime);
            iCS_UserCommands.SetScrollPosition(IStorage, newScrollPosition);
            iCS_UserCommands.SetZoom(IStorage, newScale);
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
    	// ----------------------------------------------------------------------
        public void BoundOnDisplayRoot() {
            var displayRoot= IStorage.DisplayRoot;
            var displayRootRect= displayRoot.GlobalRect;
            var viewportRect= VisibleGraphRectWithPadding;
            var vxMin= viewportRect.x;
            var vxMax= viewportRect.xMax;
            var vyMin= viewportRect.y;
            var vyMax= viewportRect.yMax;
            var drxMin= displayRootRect.x;
            var drxMax= displayRootRect.xMax;
            var dryMin= displayRootRect.y;
            var dryMax= displayRootRect.yMax;
            
            Vector2 adjustment= Vector2.zero;
            if(drxMin > vxMin && drxMax > vxMax) {
                var scrollDistance= Mathf.Min(drxMin-vxMin, drxMax-vxMax);
                adjustment.x+= scrollDistance;
            }
            if(drxMin < vxMin && drxMax < vxMax) {
                var scrollDistance= Mathf.Min(vxMin-drxMin, vxMax-drxMax);
                adjustment.x-= scrollDistance;
            }
            if(dryMin > vyMin && dryMax > vyMax) {
                var scrollDistance= Mathf.Min(dryMin-vyMin, dryMax-vyMax);
                adjustment.y+= scrollDistance;
            }
            if(dryMin < vyMin && dryMax < vyMax) {
                var scrollDistance= Mathf.Min(vyMin-dryMin, vyMax-dryMax);
                adjustment.y-= scrollDistance;
            }
            IStorage.ScrollPosition+= adjustment;
        }
    
    	// ======================================================================
    	// Utilities
    	// ----------------------------------------------------------------------
    	public void AnimateScrollPosition(Vector2 startPos, Vector2 targetPos, float deltaTime) {
            myAnimatedScrollPosition.Start(startPos, targetPos, deltaTime,
    									   (start,end,ratio)=> Math3D.Lerp(start, end, ratio));
    	}
    	// ----------------------------------------------------------------------
    	public void AnimateScale(float startScale, float targetScale, float deltaTime) {
            myAnimatedScale.Start(startScale, targetScale, deltaTime,
    							  (start,end,ratio)=> Math3D.Lerp(start, end, ratio));
    	}
    }
}