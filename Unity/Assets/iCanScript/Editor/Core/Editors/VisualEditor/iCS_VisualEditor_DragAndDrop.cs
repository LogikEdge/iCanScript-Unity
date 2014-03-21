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
	// TODO: Need to revise valid drop node.
    void DragAndDropUpdated() {
	    iCS_EditorObject objectUnderMouse= GetObjectAtMousePosition();
		if(objectUnderMouse != null) {
	        UnityEngine.Object draggedObject= GetDraggedObject();
	        if(objectUnderMouse.IsInputPort) {
	            Type portType= objectUnderMouse.RuntimeType;
	            Type dragObjType= draggedObject.GetType();
	            if(iCS_Types.IsA(portType, dragObjType)) {			
			    	DragAndDrop.visualMode = DragAndDropVisualMode.Link;
					return;
				}
			}
			if(objectUnderMouse.IsNode) {
    			if(draggedObject is Texture && objectUnderMouse.IsIconizedOnDisplay) {
		    	    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				    return;				
                }
                if(draggedObject is GameObject) {
                     if(objectUnderMouse.IsKindOfPackage) {
                         var library= GetDraggedLibrary(draggedObject);
                         if(library ==  null) {
         		    	    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;                        
                            return;
                         }
                         var storage= library.Storage;
                         if(storage.EngineObjects.Count > 0) {
                             var engineObject= storage.EngineObjects[0];
                             if(iCS_AllowedChildren.CanAddChildNode(engineObject.Name, engineObject.ObjectType, objectUnderMouse, IStorage)) {
                                 DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                 return;
                             }
                         }
                    }   
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
        var library= GetDraggedLibrary(draggedObject);
		if(library != null) {
            PasteIntoGraph(GraphMousePosition, library, library.Storage.EngineObjects[0]);
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
                    iCS_UserCommands.DragAndDropSetPortValue(eObj, draggedObject);
					// Remove data so that we don't get called multiple times (Unity bug !!!).
		            DragAndDrop.AcceptDrag();
					return;
				}
			}
			if(eObj.IsNode) {
                // Allow change of icon on minimized nodes.
    			if(draggedObject is Texture && eObj.IsIconizedOnDisplay) {
                    Texture newTexture= draggedObject as Texture;
                    iCS_UserCommands.ChangeIcon(eObj, newTexture);
                    // Remove data so that we don't get called multiple times (Unity bug !!!).
    		        DragAndDrop.AcceptDrag();
                    return;				
                }
                // Allow dropping Unity object in modules.
                if(eObj.IsKindOfPackage && draggedObject is GameObject) {
                    GameObject gameObject= draggedObject as GameObject;
                    iCS_UserCommands.CreateGameObject(gameObject, eObj, GraphMousePosition);
					// Remove data so that we don't get called multiple times (Unity bug !!!).
		            DragAndDrop.AcceptDrag();
                    return;
                }
                /*
                    FEATURE: Should support resource drag&drop.
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
    iCS_LibraryImp GetDraggedLibrary(UnityEngine.Object draggedObject) {
        if(draggedObject != null) {
            GameObject go= draggedObject as GameObject;
            if(go != null) {
                return go.GetComponent<iCS_LibraryImp>();
            }
        }
        return null;
    }
}
