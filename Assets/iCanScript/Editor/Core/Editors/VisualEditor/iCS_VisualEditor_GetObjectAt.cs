using UnityEngine;
using System.Collections;

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
        return IStorage.GetNodeAt(graphPosition);
    }

	// ----------------------------------------------------------------------
    // Returns the port at the given mouse position.
    iCS_EditorObject GetPortAtMousePosition() {
        Vector2 graphPosition= ViewportToGraph(ViewportMousePosition);
        return IStorage.GetPortAt(graphPosition);
    }

	// ----------------------------------------------------------------------
    // Returns the closest port at the given mouse position.
    iCS_EditorObject GetClosestPortAtMousePosition() {
        Vector2 graphPosition= ViewportToGraph(ViewportMousePosition);
        return IStorage.GetClosestPortAt(graphPosition);
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    iCS_EditorObject GetObjectAtScreenPosition(Vector2 _screenPos) {
        Vector2 graphPosition= ViewportToGraph(_screenPos);
        iCS_EditorObject port= IStorage.GetPortAt(graphPosition);
        if(port != null) {
            if(IStorage.IsMinimized(port)) return IStorage.GetParent(port);
            return port;
        }
        iCS_EditorObject node= IStorage.GetNodeAt(graphPosition);                
        if(node != null) return node;
        return null;
    }
	// ----------------------------------------------------------------------
	// Returns the state at the given position.
    iCS_EditorObject GetStateAt(Vector2 point) {
        iCS_EditorObject node= IStorage.GetNodeAt(point);
        while(node != null && !node.IsState) {
            node= IStorage.GetNodeAt(point, node);
        }
        return node;
    }
	// ----------------------------------------------------------------------
	// Returns the state chart at the given position.
    iCS_EditorObject GetStateChartAt(Vector2 point) {
        iCS_EditorObject node= IStorage.GetNodeAt(point);
        while(node != null && !node.IsStateChart) {
            node= IStorage.GetNodeAt(point, node);
        }
        return node;
    }

}
