using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor {
public partial class iCS_VisualEditor : iCS_EditorBase {
	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    iCS_EditorObject GetObjectAtMousePosition() {
        return GetObjectAtScreenPosition(ViewportMousePosition);
    }

	// ----------------------------------------------------------------------
    // Returns the node at the given mouse position.
    iCS_EditorObject GetNodeAtMousePosition() {
        Vector2 graphPosition= ViewportToGraph(ViewportMousePosition);
        return GetVisibleNodeAt(graphPosition);
    }

	// ----------------------------------------------------------------------
    // Returns the node at the given mouse position.
    iCS_EditorObject GetNodeWithEdgeAtMousePosition() {
        Vector2 graphPosition= ViewportToGraph(ViewportMousePosition);
        return GetVisibleNodeWithEdgeAt(graphPosition);
    }

	// ----------------------------------------------------------------------
    // Returns the port at the given mouse position.
    iCS_EditorObject GetPortAtMousePosition() {
        Vector2 graphPosition= ViewportToGraph(ViewportMousePosition);
        return GetVisiblePortAt(graphPosition);
    }

	// ----------------------------------------------------------------------
    // Returns the closest port at the given mouse position.
    iCS_EditorObject GetClosestPortAtMousePosition() {
        Vector2 graphPosition= ViewportToGraph(ViewportMousePosition);
        return GetClosestVisiblePortAt(graphPosition);
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    iCS_EditorObject GetObjectAtScreenPosition(Vector2 _screenPos) {
        Vector2 graphPosition= ViewportToGraph(_screenPos);
        iCS_EditorObject port= GetVisiblePortAt(graphPosition);
        if(port != null) {
            if(port.IsIconizedOnDisplay) return port.Parent;
            return port;
        }
        iCS_EditorObject node= GetVisibleNodeAt(graphPosition);                
        if(node != null) return node;
        return null;
    }
	// ----------------------------------------------------------------------
	// Returns the state at the given position.
    iCS_EditorObject GetStateAt(Vector2 point) {
        iCS_EditorObject node= GetVisibleNodeAt(point);
        while(node != null && !node.IsState) {
            node= GetVisibleNodeAt(point, node);
        }
        return node;
    }
	// ----------------------------------------------------------------------
	// Returns the state chart at the given position.
    iCS_EditorObject GetStateChartAt(Vector2 point) {
        iCS_EditorObject node= GetVisibleNodeAt(point);
        while(node != null && !node.IsStateChart) {
            node= GetVisibleNodeAt(point, node);
        }
        return node;
    }

	// ----------------------------------------------------------------------
    // Returns the visible node at graph position.
    iCS_EditorObject GetVisibleNodeAt(Vector2 graphPosition, iCS_EditorObject exclude= null) {
        var node= IStorage.GetNodeAt(graphPosition, exclude);
        // Anything outside the DisplayRoot will be mapped to the DisplayRoot.
        if(node != null && DisplayRoot.IsParentOf(node)) return node;
        return DisplayRoot;
    }
	// ----------------------------------------------------------------------
    // Returns the visible port at graph position.
    iCS_EditorObject GetVisiblePortAt(Vector2 graphPosition) {
        var port= IStorage.GetPortAt(graphPosition);
        if(port != null && DisplayRoot.IsParentOf(port)) return port;
        return null;
    }
	// ----------------------------------------------------------------------
    // Returns the closest visible port at graph position.
    iCS_EditorObject GetClosestVisiblePortAt(Vector2 graphPosition) {
        var port= IStorage.GetClosestPortAt(graphPosition);
        if(port != null && DisplayRoot.IsParentOf(port)) return port;
        return null;        
    }
	// ----------------------------------------------------------------------
    // Returns the closest visible port at graph position.
    iCS_EditorObject GetVisibleNodeWithEdgeAt(Vector2 graphPosition) {
        var node= IStorage.GetNodeWithEdgeAt(graphPosition);
        if(node != null) {
            if(DisplayRoot.IsParentOf(node)) return node;
            if(node == DisplayRoot && IStorage.ShowDisplayRootNode == true) return node;
        }
        return null;
    }
}
}