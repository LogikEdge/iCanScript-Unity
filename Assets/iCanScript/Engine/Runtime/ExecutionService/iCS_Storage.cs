using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This class is the main storage of uCode.  All object are derived
// from this storage class.
public class iCS_Storage : MonoBehaviour {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	[System.Serializable]
	public class UnityObjectRef {
		public int     	LinkCnt= 0;
		public Object	UnityObject= null;
		
		public UnityObjectRef(Object obj= null) {
			LinkCnt= 1;
			UnityObject= obj;
		}
	}
	
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
                      public iCS_UserPreferences      Preferences  = new iCS_UserPreferences();
    [HideInInspector] public List<iCS_EditorObject>   EditorObjects= new List<iCS_EditorObject>();
    [HideInInspector] public List<UnityObjectRef>     UnityObjects = new List<UnityObjectRef>();
    [HideInInspector] public int                      UndoRedoId   = 0;

    // ----------------------------------------------------------------------
    public bool IsValidEditorObject(int id) { return id >= 0 && id < EditorObjects.Count && EditorObjects[id] != null; }
    public bool IsValidUnityObject(int id)  { return id >= 0 && id < UnityObjects.Count && UnityObjects[id].LinkCnt > 0; }


    // ======================================================================
    // UnityObject Utilities
    // ----------------------------------------------------------------------
    public int AddUnityObject(Object obj) {
		// Search for an existing entry.
        int id= 0;
		int availableSlot= -1;
		for(id= 0; id < UnityObjects.Count; ++id) {
			if(UnityObjects[id].LinkCnt > 0 && UnityObjects[id].UnityObject == obj) {
				++(UnityObjects[id].LinkCnt);
				return id;
			}
			if(UnityObjects[id].LinkCnt == 0) {
				availableSlot= id;
			}
		}
		if(availableSlot != -1) {
			UnityObjects[availableSlot].LinkCnt= 1;
			UnityObjects[availableSlot].UnityObject= obj;
			return availableSlot;
		}
        UnityObjects.Add(new UnityObjectRef(obj));
        return id;
    }
    // ----------------------------------------------------------------------
	public void RemoveUnityObject(Object obj) {
		for(int id= 0; id < UnityObjects.Count; ++id) {
			if(UnityObjects[id].UnityObject == obj) {
				if(UnityObjects[id].LinkCnt > 0) {
					--(UnityObjects[id].LinkCnt);
					if(UnityObjects[id].LinkCnt == 0) {
						UnityObjects[id].UnityObject= null;
					}					
				}
				return;
			}
		}		
	}
    // ----------------------------------------------------------------------
    public Object GetUnityObject(int id) {
        return (id < UnityObjects.Count) ? UnityObjects[id].UnityObject : null;
    }
    // ----------------------------------------------------------------------
    public int SetUnityObject(int id, Object value) {
		int newId= AddUnityObject(value);
        if(IsValidUnityObject(id)) {
			RemoveUnityObject(GetUnityObject(id));
		}
		return newId;
    }

    // ======================================================================
    // EditorObject Utilities
    // ----------------------------------------------------------------------
    public iCS_EditorObject GetParent(iCS_EditorObject child) {
        if(child == null || child.ParentId == -1) return null;
        return EditorObjects[child.ParentId]; 
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject GetSource(iCS_EditorObject port) {
        if(port == null || port.Source == -1) return null;
        return EditorObjects[port.Source];
    }
    // ----------------------------------------------------------------------
    // Returns the absolute position of the node.
    public Rect GetPosition(iCS_EditorObject node) {
        if(!IsValidEditorObject(node.ParentId)) return node.LocalPosition;
        Rect position= GetPosition(EditorObjects[node.ParentId]);
        return new Rect(position.x+node.LocalPosition.x,
                        position.y+node.LocalPosition.y,
                        node.LocalPosition.width,
                        node.LocalPosition.height);
    }
    // ----------------------------------------------------------------------
    // Returns the last data port in the connection or NULL if none exist.
    public iCS_EditorObject GetDataConnectionSource(iCS_EditorObject port) {
        if(port == null || !port.IsDataPort) return null;
        for(iCS_EditorObject sourcePort= GetSource(port); sourcePort != null && sourcePort.IsDataPort; sourcePort= GetSource(port)) {
            port= sourcePort;
        }
        return port;
    }
    
}
