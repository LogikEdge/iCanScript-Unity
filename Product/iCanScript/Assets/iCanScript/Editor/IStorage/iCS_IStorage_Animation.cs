using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using P=iCanScript.Internal.Prelude;
using Prefs= iCanScript.Internal.Editor.PreferencesController;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_IStorage {
        // ======================================================================
    	// Fields
        // ----------------------------------------------------------------------
        public P.TimeRatio            myAnimationTime= new P.TimeRatio(TimerService.EditorTime);
        public List<iCS_EditorObject> myAnimatedObjects= new List<iCS_EditorObject>();
        public bool[]                 myWasPresent;
        public bool[]                 myWasVisible;

        // ======================================================================
    	// Animation Control
        // ----------------------------------------------------------------------
        private bool isAnimateGraphReentered= false;
        public void AnimateGraph(iCS_EditorObject obj, Action<iCS_EditorObject> fnc) {
            // Execute user action and return if we are already animating.
            if(isAnimateGraphReentered) {
                fnc(obj);
                return;
            }
            // Animate graph.
            isAnimateGraphReentered= true;
            try {
                PrepareToAnimate();
                fnc(obj);
                StartToAnimate();            
            }
            finally {
                isAnimateGraphReentered= false;            
            }
        }
        // ----------------------------------------------------------------------
        public void PrepareToAnimate() {
            StopAllAnimations();
            if(Prefs.IsAnimationEnabled) {
                TakeAnimationSnapshotForAll();            
            }
        }
        // ----------------------------------------------------------------------
        public void StartToAnimate() {
            if(Prefs.IsAnimationEnabled) {
                StartAllAnimations();            
            }        
        }
        // ----------------------------------------------------------------------
        public void UpdateAllAnimations() {
            if(myAnimationTime.IsElapsed) {
                myAnimatedObjects.Clear();
            }
            foreach(var obj in myAnimatedObjects) {
                obj.myAnimatedRect.Update();
            }
        }
        // ----------------------------------------------------------------------
        public bool IsAnimated(iCS_EditorObject obj) {
            return myAnimatedObjects.Contains(obj);
        }
        // ----------------------------------------------------------------------
        public void StopAllAnimations() {
            myAnimationTime.Reset();
        }
        // ----------------------------------------------------------------------
        public void ResetAllAnimationPositions() {
            ForEach(
                obj => {
                    if(IsVisibleInLayout(obj)) {
                        obj.ResetAnimationRect(obj.GlobalRect);
                    }
                }
            );
        }
        // ----------------------------------------------------------------------
        public void TakeAnimationSnapshotForAll() {
            int len= EditorObjects.Count;
            myWasPresent= new bool[len];
            myWasVisible= new bool[len];
            for(int i= 0; i < EditorObjects.Count; ++i) {
                // Update presence & visible flags.
                var obj= EditorObjects[i];
                if(obj == null || !obj.IsValid) {
                    myWasPresent[i]= myWasVisible[i]= false;
                    continue;
                }
                myWasPresent[i]= true;
                myWasVisible[i]= IsVisibleInLayout(obj);
                // Get copy of the initial position.
                if(obj == DisplayRoot || DisplayRoot.IsParentOf(obj)) {
                    obj.ResetAnimationRect(obj.GlobalRect);
                }
            }
        }
        // ----------------------------------------------------------------------
        public void StartAllAnimations() {
            // Update animated objects.
            myAnimatedObjects.Clear();
            myAnimationTime.Start(Prefs.AnimationTime);
            ForEachRecursiveDepthFirst(DisplayRoot,
                obj => {
                    bool isVisible= IsVisibleInLayout(obj);
                    bool wasVisible= WasVisible(obj);
                    if(!wasVisible) {
                        if(!isVisible) {
                            return;
                        }
                        if(!WasPresent(obj)) {
                            var parent= obj.ParentNode;
                            Vector2 pos;
                            if(obj.IsNode && WasPresent(parent)) {
                                pos= obj.GlobalPosition;                            
                            }
                            else {
                                parent= GetFirstNotPresentParentNode(parent);
                                pos= parent.GlobalPosition;                                                            
                            }
                            var r= new Rect(pos.x, pos.y, 0, 0);
                            obj.ResetAnimationRect(r);                        
                        }
                    }
                    var objLayoutRect= obj.GlobalRect;
                    if(!Math3D.IsEqual(objLayoutRect, obj.myAnimatedRect.StartValue)) {
                        myAnimatedObjects.Add(obj);
                        obj.myAnimatedRect.Start(obj.myAnimatedRect.StartValue, objLayoutRect, myAnimationTime);
                    }
                }
            );
        }
        // ----------------------------------------------------------------------
        private bool IsVisibleInLayout(iCS_EditorObject obj) {
            if(obj.IsVisibleInLayout) return true;
            return obj.IsPort && obj.ParentNode.IsVisibleInLayout;
        }
        // ----------------------------------------------------------------------
        private bool WasVisible(iCS_EditorObject obj) {
            if(obj == null) return false;
            int id= obj.InstanceId;
            if(id < 0 || id >= myWasVisible.Length) return false;
            return myWasVisible[id];
        }
        // ----------------------------------------------------------------------
        private bool WasPresent(iCS_EditorObject obj) {
            if(obj == null) return false;
            int id= obj.InstanceId;
            if(id < 0 || id >= myWasPresent.Length) return false;
            return myWasPresent[id];        
        }
        // ----------------------------------------------------------------------
        private iCS_EditorObject GetFirstNotPresentParentNode(iCS_EditorObject obj) {
            var parent= obj.ParentNode;
            if(WasPresent(parent)) return obj;
            while(parent != null) {
                var grandParent= parent.ParentNode;
                if(WasPresent(grandParent)) {
                    return parent;
                }
                parent= grandParent;
            }
            return EditorObjects[0];
        }
    }

}

