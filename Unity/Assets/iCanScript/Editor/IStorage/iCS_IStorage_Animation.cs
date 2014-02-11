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


    // ======================================================================
	// Animation Control
    // ----------------------------------------------------------------------
    public void StopAllAnimations() {
        myAnimationTime.Reset();
    }
    // ----------------------------------------------------------------------
    public void TakeAnimationSnapshotForAll() {
        ForEach(
            obj => {
                var r= obj.LayoutRect;
                obj.myAnimatedRect.Reset(r);
                obj.myAnimatedRect.StartValue= r;
            }
        );
    }
    // ----------------------------------------------------------------------
    public void StartAllAnimations() {
        // Update animated objects.
        myAnimatedObjects.Clear();
        myAnimationTime.Start(Prefs.MinAnimationTime);
        ForEach(
            obj => {
                if(!Math3D.IsEqual(obj.LayoutRect, obj.myAnimatedRect.StartValue)) {
                    myAnimatedObjects.Add(obj);
                    obj.myAnimatedRect.Start(obj.myAnimatedRect.StartValue, obj.LayoutRect, myAnimationTime);
                }
            }
        );
    }
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
}
