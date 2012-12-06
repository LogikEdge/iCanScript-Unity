using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    void LayoutChildNodes() {
        var childNodes= UpdateGlobalPositionFromRatio();
        foreach(var c in childNodes) {
            c.GlobalPosition= c.GlobalPositionFromRatio;
        }
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject[] UpdateGlobalPositionFromRatio() {
        var childNodes= BuildListOfChildNodes(c=> !c.IsFloating);
        var childrenRect= NodeGlobalChildRect;
        foreach(var cNode in childNodes) {
            var ratio= cNode.NodePositionRatio;
            var x= childrenRect.x+ratio.x*childrenRect.width;
            var y= childrenRect.y+ratio.y*childrenRect.height;
            cNode.GlobalPositionFromRatio= new Vector2(x,y);
        }        
        return childNodes;
    }
}
