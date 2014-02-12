using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;
using Prefs=iCS_PreferencesController;

public partial class iCS_IStorage {
    // ======================================================================
	// Fields
    // ----------------------------------------------------------------------
    public P.TimeRatio            myAnimationTime= new P.TimeRatio();
    public List<iCS_EditorObject> myAnimatedObjects= new List<iCS_EditorObject>();
    public bool[]                 myWasPresent;
    public bool[]                 myWasVisible;

    // ======================================================================
	// Animation Control
    // ----------------------------------------------------------------------
    public void AnimateGraph(iCS_EditorObject obj, Action<iCS_EditorObject> fnc) {
        StopAllAnimations();
        TakeAnimationSnapshotForAll();
        fnc(obj);
        StartAllAnimations();
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
            if(!myWasVisible[i]) continue;
            // Get copy of the initial position.
            var r= obj.LayoutRect;
            obj.myAnimatedRect.Reset(r);
            obj.AnimationStartRect= r;
        }
    }
    // ----------------------------------------------------------------------
    public void StartAllAnimations() {
        // Update animated objects.
        myAnimatedObjects.Clear();
        myAnimationTime.Start(Prefs.MinAnimationTime);
        ForEach(
            obj => {
                bool isVisible= IsVisibleInLayout(obj);
                int i= obj.InstanceId;
                if(!myWasVisible[i]) {
                    if(!isVisible) {
                        return;
                    }
                    var parent= GetFirstVisibleParentNode(obj);
                    if(parent != null) {
                        var pp= parent.LayoutPosition;
                        var r= new Rect(pp.x, pp.y, 0, 0);
                        obj.myAnimatedRect.Reset(r);
                        obj.AnimationStartRect= r;
                    }
                }
                if(!Math3D.IsEqual(obj.LayoutRect, obj.myAnimatedRect.StartValue)) {
                    myAnimatedObjects.Add(obj);
                    obj.myAnimatedRect.Start(obj.myAnimatedRect.StartValue, obj.LayoutRect, myAnimationTime);
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
    private iCS_EditorObject GetFirstVisibleParentNode(iCS_EditorObject obj) {
        var parent= obj.ParentNode;
        while(parent != null && !parent.IsVisibleInLayout) {
            parent= parent.ParentNode;
        }
        return parent;
    }
}
