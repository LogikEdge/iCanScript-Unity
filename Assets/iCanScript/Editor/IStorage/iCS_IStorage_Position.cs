using UnityEngine;
using System.Collections;

/*
    TODO: Cleanup Get/SetDisplayPosition.
*/
public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    // Returns the absolute position of the given object.
    public Rect GetLayoutPosition(iCS_EditorObject eObj) {
        return Storage.GetPosition(eObj.EngineObject);
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
        node.LocalPosition= new Rect(node.LocalPosition.x, node.LocalPosition.y, _newPos.width, _newPos.height);
        // Reposition node.
        if(!IsValid(node.ParentId)) {
            node.LocalPosition= new Rect(_newPos.x, _newPos.y, node.LocalPosition.width, node.LocalPosition.height);
        }
        else {
            Rect deltaMove= new Rect(_newPos.xMin-position.xMin, _newPos.yMin-position.yMin, _newPos.width-position.width, _newPos.height-position.height);
            node.LocalPosition= new Rect(node.LocalPosition.x+deltaMove.x, node.LocalPosition.y+deltaMove.y,
                                         node.LocalPosition.width, node.LocalPosition.height);
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
    
   	// ----------------------------------------------------------------------
    public Rect GetVisiblePosition(iCS_EditorObject edObj) {
		// Return the layout position if the object is visible.
		if(IsVisible(edObj)) return GetLayoutPosition(edObj);
		// Return the center of the most visible parent if not visible.
		var parent= edObj.Parent;
		for(; parent != null && !IsVisible(parent); parent= parent.Parent);
		if(parent != null) {
            Vector2 midPoint= Math3D.Middle(GetLayoutPosition(parent));
            return new Rect(midPoint.x, midPoint.y, 0, 0);							
		}
		return GetLayoutPosition(edObj);
    }
}
