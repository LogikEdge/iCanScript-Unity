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
                         iCS_IVisualScriptData storage= library;
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
            if(library.EngineObjects.Count == 0) {
                Debug.LogWarning("iCanScript: Storage of pasted object is empty !!!");
            }
            PasteIntoGraph(GraphMousePosition, library, library.EngineObjects[0]);
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
                // Special case for game object
                // TODO: Should use common reflection of variables/properties derived from Component.
                if(iCS_Types.IsA<GameObject>(dragObjType)) {
                    var go= draggedObject as GameObject;
                    if(iCS_Types.IsA<Transform>(portType)) {
                        iCS_UserCommands.DragAndDropSetPortValue(eObj, go.transform);
                    }
                    if(iCS_Types.IsA<Collider2D>(portType)) {
                        iCS_UserCommands.DragAndDropSetPortValue(eObj, go.collider2D);
                    }
                    if(iCS_Types.IsA<Collider>(portType)) {
                        iCS_UserCommands.DragAndDropSetPortValue(eObj, go.collider);
                    }
                    if(iCS_Types.IsA<Renderer>(portType)) {
                        iCS_UserCommands.DragAndDropSetPortValue(eObj, go.renderer);
                    }
                    if(iCS_Types.IsA<Rigidbody2D>(portType)) {
                        iCS_UserCommands.DragAndDropSetPortValue(eObj, go.rigidbody2D);
                    }
                    if(iCS_Types.IsA<Rigidbody>(portType)) {
                        iCS_UserCommands.DragAndDropSetPortValue(eObj, go.rigidbody);
                    }
                    if(iCS_Types.IsA<Animation>(portType)) {
                        iCS_UserCommands.DragAndDropSetPortValue(eObj, go.animation);
                    }
                    if(iCS_Types.IsA<Camera>(portType)) {
                        iCS_UserCommands.DragAndDropSetPortValue(eObj, go.camera);
                    }
                    if(iCS_Types.IsA<ConstantForce>(portType)) {
                        iCS_UserCommands.DragAndDropSetPortValue(eObj, go.constantForce);
                    }
                    if(iCS_Types.IsA<NetworkView>(portType)) {
                        iCS_UserCommands.DragAndDropSetPortValue(eObj, go.networkView);
                    }
                    if(iCS_Types.IsA<ParticleEmitter>(portType)) {
                        iCS_UserCommands.DragAndDropSetPortValue(eObj, go.particleEmitter);
                    }
                    if(iCS_Types.IsA<ParticleSystem>(portType)) {
                        iCS_UserCommands.DragAndDropSetPortValue(eObj, go.particleSystem);
                    }
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
