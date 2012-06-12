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
			if(eObj.IsNode) {
    			if(draggedObject is Texture && eObj.IsMinimized) {
		    	    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				    return;				
                }
                if(draggedObject is GameObject && eObj.IsModule) {
		    	    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				    return;				                    
                }
			}
		}
    	DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
    }
	// ----------------------------------------------------------------------
    void DragAndDropExited() {
        /*
            TODO: Should support undo for Drag&Drop.
        */
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
		    /*
		      TODO: Should automatically create class module for a dragged object not on a port.
		    */
	        if(eObj.IsInputPort) {
	            Type portType= eObj.RuntimeType;
	            Type dragObjType= draggedObject.GetType();
	            if(iCS_Types.IsA(portType, dragObjType)) {			
                    IStorage.UpdatePortInitialValue(eObj, draggedObject);
                    /*
                        TODO: Update node name if the port is "this" and the object is unnamed.
                    */
					// Remove data so that we don't get called multiple times (Unity bug !!!).
		            DragAndDrop.objectReferences= new UnityEngine.Object[0];
					return;
				}
			}
			if(eObj.IsNode) {
                // Allow change of icon on minimized nodes.
    			if(draggedObject is Texture && eObj.IsMinimized) {
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
                // Allow dropping Unity object in modules.
                if(eObj.IsModule && draggedObject is GameObject) {
                    GameObject gameObject= draggedObject as GameObject;
                    var module= IStorage.CreateModule(eObj.InstanceId, MouseGraphPosition, gameObject.name, iCS_ObjectTypeEnum.Module, gameObject.GetType());
                    var thisPort= IStorage.FindThisInputPort(module);
                    if(thisPort != null) {
                        IStorage.UpdatePortInitialValue(thisPort, draggedObject);
                    }
					// Remove data so that we don't get called multiple times (Unity bug !!!).
		            DragAndDrop.objectReferences= new UnityEngine.Object[0];
                    return;
                }
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

}
