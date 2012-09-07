using UnityEngine;
using System.Collections;

/*
    TODO: Cleanup Get/SetDisplayPosition.
*/
public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    // Returns the absolute position of the given object.
    public Rect GetLayoutPosition(iCS_EditorObject eObj) {
        return Storage.GetPosition(eObj);
    }
    public Rect GetLayoutPosition(int id) {
        return GetLayoutPosition(EditorObjects[id]);
    }
    public Vector2 GetLayoutCenterPosition(iCS_EditorObject eObj) {
        return Math3D.Middle(GetLayoutPosition(eObj));
    }

    // ----------------------------------------------------------------------
    // Returns the local position of the given object.
    public Rect GetLayoutLocalPosition(iCS_EditorObject eObj) {
        return eObj.LocalPosition;
    }
    // ----------------------------------------------------------------------
    public void SetLayoutPosition(iCS_EditorObject node, Rect _newPos) {
        // Adjust node size.
        Rect position= GetLayoutPosition(node);
        node.LocalPosition.width = _newPos.width;
        node.LocalPosition.height= _newPos.height;
        // Reposition node.
        if(!IsValid(node.ParentId)) {
            node.LocalPosition.x= _newPos.x;
            node.LocalPosition.y= _newPos.y;            
        }
        else {
            Rect deltaMove= new Rect(_newPos.xMin-position.xMin, _newPos.yMin-position.yMin, _newPos.width-position.width, _newPos.height-position.height);
            node.LocalPosition.x+= deltaMove.x;
            node.LocalPosition.y+= deltaMove.y;
            float separationX= Math3D.IsNotZero(deltaMove.x) ? deltaMove.x : deltaMove.width;
            float separationY= Math3D.IsNotZero(deltaMove.y) ? deltaMove.y : deltaMove.height;
            var separationVector= new Vector2(separationX, separationY);
            LayoutParent(node, separationVector);
        }
    }    
    // ----------------------------------------------------------------------
    public void SetLayoutLocalPosition(iCS_EditorObject eObj, Rect newLocalPos) {
        eObj.LocalPosition= newLocalPos;
    }
    
//    // ----------------------------------------------------------------------
//    public Rect GetDisplayPosition(iCS_EditorObject eObj) {
//        var cache= StorageCache[eObj.InstanceId];
//        var layoutPos= GetLayoutPosition(eObj);
//        if(cache.DisplayPosition.IsActive) {
//            if(Math3D.IsNotEqual(layoutPos, cache.DisplayPosition.TargetValue)) {
//                cache.DisplayPosition.TargetValue= layoutPos;
//            }
//            cache.DisplayPosition.Update();
//            return cache.DisplayPosition.CurrentValue;
//        }
//        SetDisplayOptionBeforeAnimation(eObj, eObj.DisplayOption);
//        return layoutPos;
//    }
//    // ----------------------------------------------------------------------
//    public void SetDisplayPosition(iCS_EditorObject eObj, Rect newPos, bool performAnimation) {
//        var cache= StorageCache[eObj.InstanceId];
//        if(performAnimation) {
//            var currentPos= cache.DisplayPosition.CurrentValue;
//            var animTime  = iCS_PreferencesEditor.AnimationTime;
//            cache.DisplayPosition.Start(currentPos, newPos, animTime, (start,end,ratio)=> Math3D.Lerp(start, end, ratio));            
//        } else {
//            cache.DisplayPosition.Reset(newPos);
//        }
//    }
}
