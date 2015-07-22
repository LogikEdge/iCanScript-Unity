using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
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
                if(GetValidDragAndDropObjectForPort(objectUnderMouse, draggedObject) != null) {
    		    	DragAndDrop.visualMode = DragAndDropVisualMode.Link;
    				return;                
                }                
            }
			if(objectUnderMouse.IsNode) {
                var draggedLibrary= GetDraggedLibrary(draggedObject);
                var draggedEngineObject= draggedLibrary != null ? draggedLibrary.EngineObjects[0] : null;
                // -- Don't accept to drag an object directly under Behaviour --
                if(objectUnderMouse.IsBehaviour) {
                    if(draggedEngineObject != null &&
                       iCS_AllowedChildren.CanAddChildNode(draggedEngineObject.RawName, draggedEngineObject, objectUnderMouse, IStorage)) {
       		    	    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;                        
                    }
                    else {
                        DragAndDrop.visualMode= DragAndDropVisualMode.Rejected;                        
                    }
                    return;
                }
                // -- Don't accept to drag object outside the root node --
                if(objectUnderMouse == DisplayRoot && IStorage.ShowDisplayRootNode) {
                    if(!DisplayRoot.GlobalRect.Contains(GraphMousePosition)) {
                        DragAndDrop.visualMode= DragAndDropVisualMode.Rejected;
                        return;
                    }
                }
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
                             if(iCS_AllowedChildren.CanAddChildNode(engineObject.RawName, engineObject.ObjectType, objectUnderMouse, IStorage)) {
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
                if(iCS_Types.IsA<GameObject>(dragObjType) && PrefabUtility.GetPrefabType(IStorage.HostGameObject) == PrefabType.Prefab) {
                    var isSceneObject= UnityUtility.IsSceneGameObject(draggedObject as GameObject);
                    if(isSceneObject == true) {
                        ShowNotification(new GUIContent("Unity does not allow binding a Scene object to a Prefab."));
                        return;                        
                    }
                }
	            if(iCS_Types.IsA(portType, dragObjType)) {			
                    iCS_UserCommands.DragAndDropSetPortValue(eObj, draggedObject);
					// Remove data so that we don't get called multiple times (Unity bug !!!).
		            DragAndDrop.AcceptDrag();
					return;
				}
                // Special case for game object
                if(iCS_Types.IsA<GameObject>(dragObjType) && iCS_Types.IsA<Component>(portType)) {
                    var go= draggedObject as GameObject;
                    foreach(var component in go.GetComponents<Component>()) {
                        if(iCS_Types.IsA(portType, component.GetType())) {
                            iCS_UserCommands.DragAndDropSetPortValue(eObj, component);
        					// Remove data so that we don't get called multiple times (Unity bug !!!).
        		            DragAndDrop.AcceptDrag();
                            return;
                        }
                    }
                }
			}
			if(eObj.IsNode && !eObj.IsBehaviour) {
                // Don't accept to drag object outside the root node.
                if(eObj == DisplayRoot && IStorage.ShowDisplayRootNode) {
                    if(!DisplayRoot.GlobalRect.Contains(GraphMousePosition)) {
                        return;
                    }
                }
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
                    // Determine if game object contains a visual script.
                    var vs= gameObject.GetComponent<iCS_VisualScriptImp>();
                    if(vs == null) {
                        CreateGameObjectNode(gameObject, eObj, GraphMousePosition);
                    }
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
    void DragAndDropExited() {}

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
	// ----------------------------------------------------------------------
    UnityEngine.Object GetValidDragAndDropObjectForPort(iCS_EditorObject port, UnityEngine.Object draggedObject) {
        Type portType= port.RuntimeType;
        Type dragObjType= draggedObject.GetType();
        if(iCS_Types.IsA(portType, dragObjType)) {			
			return draggedObject;
		}
        // Special case for game object
        if(iCS_Types.IsA<GameObject>(dragObjType) && iCS_Types.IsA<Component>(portType)) {
            var go= draggedObject as GameObject;
            foreach(var component in go.GetComponents<Component>()) {
                if(iCS_Types.IsA(portType, component.GetType())) {
                    return component;
                }
            }
        }
        return null;
    }
	// ----------------------------------------------------------------------
    void CreateGameObjectNode(GameObject go, iCS_EditorObject parent, Vector2 graphMousePosition) {
        var instance= iCS_UserCommands.CreateGameObject(go, parent, graphMousePosition);
        if(PrefabUtility.GetPrefabType(IStorage.HostGameObject) == PrefabType.Prefab) {
            var isSceneObject= UnityUtility.IsSceneGameObject(go);
            if(isSceneObject == true) {
                ShowNotification(new GUIContent("Unity does not allow binding a Scene object to a Prefab."));
                var thisPort= IStorage.PropertiesWizardGetInputThisPort(instance);
                if(thisPort != null) {
                    thisPort.Value= null;
                }
            }
        }                                
    }
}
}
