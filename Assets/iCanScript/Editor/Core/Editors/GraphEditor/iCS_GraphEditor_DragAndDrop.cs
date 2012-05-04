using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

// ===========================================================================
// Unity Drag & Drop.
// ===========================================================================
public partial class iCS_GraphEditor : iCS_EditorWindow {
    // ======================================================================
    // Respond to Unity drag & drop protocol.
	// ----------------------------------------------------------------------
    void DragAndDropPerform() {
        IStorage.RegisterUndo("DragAndDrop");			
        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        DragAndDrop.AcceptDrag();
    }
	// ----------------------------------------------------------------------
    void DragAndDropUpdated() {
	    iCS_EditorObject eObj= GetObjectAtMousePosition();
		if(eObj != null) {
	        UnityEngine.Object draggedObject= GetDraggedObject();
	        if(eObj.IsInputPort) {
	            Type portType= eObj.RuntimeType;
	            Type dragObjType= draggedObject.GetType();
	            if(iCS_Types.IsA(portType, dragObjType)) {			
			    	DragAndDrop.visualMode = DragAndDropVisualMode.Link;
					return;
				}
			}
			if(eObj.IsNode && draggedObject is Texture) {
		    	DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				return;				
			}
		}
    	DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
    }
	// ----------------------------------------------------------------------
    void DragAndDropExited() {
        UnityEngine.Object draggedObject= GetDraggedObject();
		if(draggedObject == null) { return; }

		// Copy/Paste library from prefab
        iCS_Storage storage= GetDraggedLibrary(draggedObject);
		if(storage != null) {
            PasteIntoGraph(ViewportToGraph(MousePosition), storage, storage.EditorObjects[0]);
			// Remove data so that we don't get called multiple times (Unity bug !!!).
            DragAndDrop.objectReferences= new UnityEngine.Object[0];
			return;
		}
		
	    iCS_EditorObject eObj= GetObjectAtMousePosition();
		if(eObj != null) {
	        if(eObj.IsInputPort) {
	            Type portType= eObj.RuntimeType;
	            Type dragObjType= draggedObject.GetType();
	            if(iCS_Types.IsA(portType, dragObjType)) {			
                    UpdatePortInitialValue(eObj, draggedObject);
					// Remove data so that we don't get called multiple times (Unity bug !!!).
		            DragAndDrop.objectReferences= new UnityEngine.Object[0];
					return;
				}
			}
			if(eObj.IsNode && draggedObject is Texture) {
                Texture newTexture= draggedObject as Texture;
                string iconGUID= newTexture != null ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newTexture)) : null;
                if(newTexture != null) {
                    eObj.IconGUID= iconGUID;                    
                    IStorage.Minimize(eObj);
                    // Remove data so that we don't get called multiple times (Unity bug !!!).
                    DragAndDrop.objectReferences= new UnityEngine.Object[0];
				}
				return;				
			}
		}
		Undo.PerformUndo();
    }

    // ======================================================================
    // Drag & drop related utilities.
	// ----------------------------------------------------------------------
    UnityEngine.Object GetDraggedObject() {
        UnityEngine.Object[] draggedObjects= DragAndDrop.objectReferences;
        return draggedObjects.Length >= 1 ? draggedObjects[0] : null;        
    }
	// ----------------------------------------------------------------------
    iCS_Storage GetDraggedLibrary(UnityEngine.Object draggedObject) {
        if(draggedObject != null) {
            GameObject go= draggedObject as GameObject;
            if(go != null) {
                iCS_Storage storage= go.GetComponent<iCS_Library>();
                return storage;
            }
        }
        return null;
    }

    // ======================================================================
    // Port value update.
	// ----------------------------------------------------------------------
    void UpdatePortInitialValue(iCS_EditorObject port, object newValue) {
        UpdateRuntimeValue(port, newValue);
		IStorage.SetInitialPortValue(port, newValue);
		IStorage.SetPortValue(port, newValue);
        IStorage.SetDirty(IStorage.GetParent(port));
    }
    // -----------------------------------------------------------------------
    public void UpdateRuntimeValue(iCS_EditorObject port, object newValue) {
        if(!port.IsInDataPort) return;
        if(port.IsModulePort) {
            iCS_EditorObject[] connectedPorts= IStorage.FindConnectedPorts(port);
            foreach(var cp in connectedPorts) {
                UpdateRuntimeValue(cp, newValue);
            }
            return;
        }
        iCS_EditorObject parent= IStorage.GetParent(port);
        if(parent == null) return;
        // Get runtime object if it exists.
        iCS_IParams runtimeObject= IStorage.GetRuntimeObject(parent) as iCS_IParams;
        if(runtimeObject == null) return;
        runtimeObject.SetParameter(port.PortIndex, newValue);
    }
}
