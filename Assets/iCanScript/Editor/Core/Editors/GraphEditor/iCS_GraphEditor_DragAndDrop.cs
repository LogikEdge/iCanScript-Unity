using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

// ===========================================================================
// Unity Drag & Drop.
// ===========================================================================
public partial class iCS_GraphEditor : iCS_EditorWindow {
    // ======================================================================
    // Types.
	// ----------------------------------------------------------------------
    enum DraggedObjectTypeEnum { Library, Texture, Unknown };
    
    // ======================================================================
    // Respond to Unity drag & drop protocol.
	// ----------------------------------------------------------------------
    void DragAndDropPerform() {
        // Show a copy icon on the drag
//        if(GetDraggedObjectType(GetDraggedObject()) != DraggedObjectTypeEnum.Unknown) {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            DragAndDrop.AcceptDrag();                                                            
//        }        
    }
	// ----------------------------------------------------------------------
    void DragAndDropUpdated() {
        // Show a copy icon on the drag
//        if(GetDraggedObjectType(GetDraggedObject()) != DraggedObjectTypeEnum.Unknown) {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
//        }        
    }
	// ----------------------------------------------------------------------
    void DragAndDropExited() {
        UnityEngine.Object draggedObject= GetDraggedObject();
        switch(GetDraggedObjectType(draggedObject)) {
            // Paste iCanScript Library into graph.
            case DraggedObjectTypeEnum.Library: {
                iCS_Storage storage= GetDraggedLibrary(draggedObject);
                if(storage != null) {
                    PasteIntoGraph(ViewportToGraph(MousePosition), storage, storage.EditorObjects[0]);
                }
                break;
            }
            // Update node iconic representation or initialize an node texture input.
            case DraggedObjectTypeEnum.Texture: {
                Texture newTexture= GetDraggedTexture(draggedObject);
                iCS_EditorObject eObj= GetObjectAtMousePosition();
                if(eObj.IsPort) {
                    Type portType= eObj.RuntimeType;
                    Type dragObjType= draggedObject.GetType();
                    if(iCS_Types.IsA(portType, dragObjType)) {
                        myStorage.RegisterUndo("Change port value");
                        UpdatePortInitialValue(eObj, draggedObject);
                    }
                } else if(eObj.IsNode) {
                    string iconGUID= newTexture != null ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newTexture)) : null;
                    if(newTexture != null) {
                        Debug.Log("Changing node icon: "+iconGUID);
                        myStorage.RegisterUndo("Change node Icon");
                        eObj.IconGUID= iconGUID;                    
                        myStorage.Minimize(eObj);
                    }
                }
                break;
            }
            default: {
                iCS_EditorObject eObj= GetObjectAtMousePosition();
                if(eObj.IsPort) {
                    Type portType= eObj.RuntimeType;
                    Type dragObjType= draggedObject.GetType();
                    Debug.Log("Dragging "+dragObjType.Name+" unto "+portType.Name);
                    if(iCS_Types.IsA(portType, dragObjType)) {
                        Debug.Log("Set port value accepted.");
                        myStorage.RegisterUndo("Set port value");
                        UpdatePortInitialValue(eObj, draggedObject);
                    }
                }
                break;
            }
        }
    }

    // ======================================================================
    // Drag & drop related utilities.
	// ----------------------------------------------------------------------
    UnityEngine.Object GetDraggedObject() {
        UnityEngine.Object[] draggedObjects= DragAndDrop.objectReferences;
        return draggedObjects.Length >= 1 ? draggedObjects[0] : null;        
    }
	// ----------------------------------------------------------------------
    DraggedObjectTypeEnum GetDraggedObjectType(UnityEngine.Object obj) {
        if(obj == null) return DraggedObjectTypeEnum.Unknown;
        // Library
        if(obj is GameObject) {
            GameObject go= obj as GameObject;
            iCS_Storage storage= go.GetComponent<iCS_Library>();
            return storage != null ? DraggedObjectTypeEnum.Library : DraggedObjectTypeEnum.Unknown;
        }
        // Icon
        if(obj is Texture) {
            return DraggedObjectTypeEnum.Texture;
        }
        return DraggedObjectTypeEnum.Unknown;
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
    Texture GetDraggedTexture(UnityEngine.Object draggedObject) {
        return draggedObject != null ? draggedObject as Texture : null;
    }

    // ======================================================================
    // Port value update.
	// ----------------------------------------------------------------------
    void UpdatePortInitialValue(iCS_EditorObject port, object newValue) {
        UpdateRuntimeValue(port, newValue);
		myStorage.SetInitialPortValue(port, newValue);
		myStorage.SetPortValue(port, newValue);
        myStorage.SetDirty(myStorage.GetParent(port));
    }
    // -----------------------------------------------------------------------
    public void UpdateRuntimeValue(iCS_EditorObject port, object newValue) {
        if(!port.IsInDataPort) return;
        if(port.IsModulePort) {
            iCS_EditorObject[] connectedPorts= myStorage.FindConnectedPorts(port);
            foreach(var cp in connectedPorts) {
                UpdateRuntimeValue(cp, newValue);
            }
            return;
        }
        iCS_EditorObject parent= myStorage.GetParent(port);
        if(parent == null) return;
        // Get runtime object if it exists.
        iCS_IParams runtimeObject= myStorage.GetRuntimeObject(parent) as iCS_IParams;
        if(runtimeObject == null) return;
        runtimeObject.SetParameter(port.PortIndex, newValue);
    }
}
