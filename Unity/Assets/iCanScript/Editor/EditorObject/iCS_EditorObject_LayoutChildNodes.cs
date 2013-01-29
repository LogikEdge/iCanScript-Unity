using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
//    // ----------------------------------------------------------------------
//    void LayoutChildNodes() {
//        UpdateGlobalPositionFromRatio();
//    }
//    // ----------------------------------------------------------------------
//    iCS_EditorObject[] UpdateGlobalPositionFromRatio() {
//        var childNodes= BuildListOfChildNodes(c=> !c.IsFloating);
//        var childrenRect= NodeGlobalChildRect;
//        foreach(var cNode in childNodes) {
//            var ratio= cNode.NodePositionRatio;
//            var x= childrenRect.x+ratio.x*childrenRect.width;
//            var y= childrenRect.y+ratio.y*childrenRect.height;
//            cNode.GlobalPosition= new Vector2(x,y);
//        }        
//        return childNodes;
//    }
//    // ----------------------------------------------------------------------
//    // Returns the global position according to position ratio and current
//    // parent size.
//    Vector2 NodeGlobalPositionFromRatio {
//        get {
//            if(!IsParentValid) return GlobalPosition;
//            var childrenRect= Parent.NodeGlobalChildRect;
//            var ratio= NodePositionRatio;
//            var x= childrenRect.x+ratio.x*childrenRect.width;
//            var y= childrenRect.y+ratio.y*childrenRect.height;
//            return new Vector2(x,y);
//        }
//    }
}
