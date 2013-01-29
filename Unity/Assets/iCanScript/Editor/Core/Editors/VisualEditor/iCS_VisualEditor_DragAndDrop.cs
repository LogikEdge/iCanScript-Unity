using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

// ===========================================================================
// Unity Drag & Drop.
// ===========================================================================
public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Respond to Unity drag & drop protocol.
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
    			if(draggedObject is Texture && eObj.IsIconized) {
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
    void DragAndDropPerformed() {
        UnityEngine.Object draggedObject= GetDraggedObject();
		if(draggedObject == null) { return; }
		
		// Copy/Paste library from prefab
        iCS_Storage storage= GetDraggedLibrary(draggedObject);
		if(storage != null) {
		    IStorage.RegisterUndo("DragAndDrop");			
            PasteIntoGraph(GraphMousePosition, storage, storage.EngineObjects[0]);
			// Remove data so that we don't get called multiple times (Unity bug !!!).
            DragAndDrop.AcceptDrag();
			return;
		}
		
	    iCS_EditorObject eObj= GetObjectAtMousePosition();
		if(eObj != null) {
	        if(eObj.IsInputPort) {
	            Type portType= eObj.RuntimeType;
	            Type dragObjType= draggedObject.GetType();
	            if(iCS_Types.IsA(portType, dragObjType)) {			
        	        IStorage.RegisterUndo("DragAndDrop");			
                    eObj.PortValue= draggedObject;
                    /*
                        TODO: Update node name if the port is "this" and the object is unnamed.
                    */
					// Remove data so that we don't get called multiple times (Unity bug !!!).
		            DragAndDrop.AcceptDrag();
					return;
				}
			}
			if(eObj.IsNode) {
                // Allow change of icon on minimized nodes.
    			if(draggedObject is Texture && eObj.IsIconized) {
                    Texture newTexture= draggedObject as Texture;
                    string iconGUID= newTexture != null ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newTexture)) : null;
                    if(newTexture != null) {
            	        IStorage.RegisterUndo("DragAndDrop");			
                        eObj.IconGUID= iconGUID;                    
                        IStorage.Iconize(eObj);
                        // Remove data so that we don't get called multiple times (Unity bug !!!).
    		            DragAndDrop.AcceptDrag();
    				}
    				return;				
                }
                // Allow dropping Unity object in modules.
                if(eObj.IsModule && draggedObject is GameObject) {
        	        IStorage.RegisterUndo("DragAndDrop");			
                    GameObject gameObject= draggedObject as GameObject;
                    CreateGameObject(eObj.InstanceId, gameObject, GraphMousePosition);
					// Remove data so that we don't get called multiple times (Unity bug !!!).
		            DragAndDrop.AcceptDrag();
                    return;
                }
                /*
                    TODO: Should support resource drag&drop.
                */
			}
		}
    }
	// ----------------------------------------------------------------------
    void DragAndDropExited() {
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
	// ----------------------------------------------------------------------
    iCS_EditorObject CreateGameObject(int parentId, GameObject gameObject, Vector2 position) {
        var module= IStorage.CreateModule(parentId, GraphMousePosition, gameObject.name, iCS_ObjectTypeEnum.Module, gameObject.GetType());
        var thisPort= IStorage.FindThisInputPort(module);
        if(thisPort != null) {
            thisPort.PortValue= gameObject;
        }
        /*
            TODO: Should construct the full gameobject including its components
        */
//        Component[] components= gameObject.GetComponents(typeof(Component));
//        foreach(var component in components) {
//            IStorage.CreateModule(parentId, GraphMousePosition+new Vector2(100,0), component.name, iCS_ObjectTypeEnum.Module, component.GetType());
//        }
        return module;
    }
}