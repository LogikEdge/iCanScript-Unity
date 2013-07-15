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
                     if(objectUnderMouse.IsKindOfAggregate) {
                         var storage= GetDraggedLibrary(draggedObject);
                         if(storage ==  null) {
         		    	    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;                        
                            return;
                         }
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
    			if(draggedObject is Texture && eObj.IsIconizedOnDisplay) {
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
                if(eObj.IsKindOfAggregate && draggedObject is GameObject) {
        	        IStorage.RegisterUndo("DragAndDrop");			
                    GameObject gameObject= draggedObject as GameObject;
                    CreateGameObject(eObj.InstanceId, gameObject, GraphMousePosition);
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
    iCS_Storage GetDraggedLibrary(UnityEngine.Object draggedObject) {
        if(draggedObject != null) {
            GameObject go= draggedObject as GameObject;
            if(go != null) {
                iCS_Storage storage= go.GetComponent<iCS_LibraryImp>();
                return storage;
            }
        }
        return null;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject CreateGameObject(int parentId, GameObject gameObject, Vector2 position) {
        var module= IStorage.CreateAggregate(parentId, GraphMousePosition, gameObject.name, iCS_ObjectTypeEnum.Package, gameObject.GetType());
        var thisPort= IStorage.FindThisInputPort(module);
        if(thisPort != null) {
            thisPort.PortValue= gameObject;
        }
        /*
            TODO: Should construct the full gameobject including its components
        */
//        Component[] components= gameObject.GetComponents(typeof(Component));
//        foreach(var component in components) {
//            IStorage.CreateAggregate(parentId, GraphMousePosition+new Vector2(100,0), component.name, iCS_ObjectTypeEnum.Module, component.GetType());
//        }
        return module;
    }
}
