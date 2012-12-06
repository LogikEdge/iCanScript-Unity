using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ======================================================================
    /*
        TODO: Revise InitialLayout ...
    */
    void InitialLayout() {
        // Port position will be initialized by the parent node.
        if(IsPort) return;
        // Wait until node becomes visible.
        if(!IsVisible) return;
        // Iconized nodes will be initialized by the parent node.
        if(IsIconized) return;
        // Initialize the node size using the children position ratio.
        DisplaySize= ComputeNodeSizeFromRatio();
        // Layout any uninitialized port.
        InitialPortLayout();
        // Nothing more to do if we have no visible children.
        if(IsFolded) {
            return;
        }
        // Update global position of child nodes.
        var childNodes= BuildListOfChildNodes(c=> !c.IsFloating);
        var childrenRect= NodeGlobalChildRect;
        foreach(var cNode in childNodes) {
            var ratio= cNode.NodePositionRatio;
            var x= childrenRect.x+ratio.x*childrenRect.width;
            var y= childrenRect.y+ratio.y*childrenRect.height;
            cNode.GlobalPosition= new Vector2(x,y);
        }
    }
}
