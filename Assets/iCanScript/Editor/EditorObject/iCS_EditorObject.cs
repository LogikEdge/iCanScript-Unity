using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_EditorObject {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    iCS_IStorage    	myIStorage  = null;
    int             	myId        = -1;
    bool            	myIsFloating= false;
    bool            	myIsDirty   = false;
    public List<int>	myChildren  = new List<int>();
 
    // ======================================================================
    // Conversion Utilities
    // ----------------------------------------------------------------------
    public iCS_EngineObject EngineObject {
		get { return myIStorage.EngineObjects[myId]; }
	}
    List<iCS_EngineObject> EditorToEngineList(List<iCS_EditorObject> editorObjects) {
        return Prelude.map(eo=> eo.EngineObject, editorObjects);
    }
    
    // ======================================================================
    // Proxy Methods
    // ----------------------------------------------------------------------
    public bool   IsValid {
		get { return myId != -1; }
	}
    public iCS_ObjectTypeEnum ObjectType {
		get { return EngineObject.ObjectType; }
		set { EngineObject.ObjectType= value; }
	}
    public int InstanceId { 
		get { return myId; }
	}
    public int ParentId {
		get { return EngineObject.ParentId; }
		set {
			int pid= EngineObject.ParentId;
			if(pid != -1 && pid != value) {
				myIStorage[pid].RemoveChild(this);
			}
			EngineObject.ParentId= value;
			if(value != -1) {
				myIStorage[value].AddChild(this);
			}
		}
	}
    public iCS_EditorObject Parent {
		get { return (IsValid && ParentId != -1) ? myIStorage[ParentId] : null; }
		set { if(IsValid) ParentId= (value != null ? value.InstanceId : -1); }
	}
    public Type RuntimeType {
		get { return EngineObject.RuntimeType; }
	}
    public string RawName {
		get { return EngineObject.RawName; }
		set { EngineObject.RawName= value; }
	}
    public string Name {
		get { return EngineObject.Name; }
		set { EngineObject.Name= value; }
	}
    public bool IsNameEditable {
		get { return EngineObject.IsNameEditable; }
		set { EngineObject.IsNameEditable= value; }
	}
    public string RawTooltip {
		get { return EngineObject.RawTooltip; }
		set { EngineObject.RawTooltip= value; }
	}
    public string Tooltip {
		get { return EngineObject.Tooltip; }
		set { EngineObject.Tooltip= value; }
	}
    public bool IsFloating {
		get { return myIsFloating; }
		set { myIsFloating= value; }
	}
    public bool IsDirty {
		get { return myIsDirty; }
		set { myIsDirty= value; }
	}
    public Rect LocalPosition {
		get { return EngineObject.LocalPosition; }
		set { EngineObject.LocalPosition= value; }
	}
    
    // ======================================================================
    // Constructors/Builders
    // ----------------------------------------------------------------------
    public iCS_EditorObject(int id, iCS_IStorage iStorage, iCS_EngineObject engineObject) {
        if(id >= 0) {
            // Grow engine object array if needed.
			GrowEngineObjects(id, iStorage);
            iStorage.EngineObjects[id]= engineObject;            
        } else {
			Debug.LogError("Trying to create an EditorObject with an invalid id: "+id);
		}
        Init(id, iStorage);
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject(int id, string name, Type type, int parentId, iCS_ObjectTypeEnum objectType,
                            Rect localPosition, iCS_IStorage iStorage) 
    : this(id, iStorage, new iCS_EngineObject(id, name, type, parentId, objectType, localPosition))
    {}
    // ----------------------------------------------------------------------
    void Init(int id, iCS_IStorage iStorage) {
        myIStorage= iStorage;
        myId= id;
        myIsDirty= true;        
    }
    // ----------------------------------------------------------------------
    // Duplicate the given editor object with a new id and parent.
    public static iCS_EditorObject Clone(int id, iCS_EditorObject toClone, iCS_EditorObject parent,
                                         Vector2 localPosition, iCS_IStorage iStorage) {
        var engineObject= iCS_EngineObject.Clone(id, toClone.EngineObject,
												 (parent == null ? null : parent.EngineObject),
												 localPosition);
        return new iCS_EditorObject(id, iStorage, engineObject);
    }

    // ----------------------------------------------------------------------
    // Reinitialize the editor object to its default values.
    public void Reset() {
		if(ParentId != -1) Parent.RemoveChild(this);
        IsFloating= false;
        IsDirty= false;
		InitialValue= null;
		myAnimatedPosition.Reset();
		myChildren.Clear();
        EngineObject.Reset();
    } 

    // ======================================================================
	// Grow database if needed.
    // ----------------------------------------------------------------------
	private static void GrowObjects(int id, iCS_IStorage iStorage) {
		GrowEngineObjects(id, iStorage);
		GrowEditorObjects(id, iStorage);
	}
	private static void GrowEngineObjects(int id, iCS_IStorage iStorage) {
        if(id >= 0) {
            // Grow engine object array if needed.
            for(int len= iStorage.EngineObjects.Count; id >= len; ++len) {
                iStorage.EngineObjects.Add(null);
            }
        }
	}
	private static void GrowEditorObjects(int id, iCS_IStorage iStorage) {
        if(id >= 0) {
            // Grow engine object array if needed.
            for(int len= iStorage.EditorObjects.Count; id >= len; ++len) {
                iStorage.EditorObjects.Add(null);
            }
        }		
	}
	
    // ======================================================================
	// Rebuild from engine database.
    // ----------------------------------------------------------------------
    public iCS_EditorObject(int id, iCS_IStorage iStorage) {
        Init(id, iStorage);
    }
    // ----------------------------------------------------------------------
	public static void RebuildFromEngineObjects(iCS_IStorage iStorage) {
		iStorage.EditorObjects.Clear();
		foreach(var engineObject in iStorage.EngineObjects) {
			iCS_EditorObject editorObject= null;
			if(engineObject != null) {
				editorObject= new iCS_EditorObject(engineObject.InstanceId, iStorage);
			}			
			iStorage.EditorObjects.Add(editorObject);
		}
		RebuildChildrenLists(iStorage);
	}
    // ----------------------------------------------------------------------
	private static void RebuildChildrenLists(iCS_IStorage iStorage) {
		foreach(var obj in iStorage.EditorObjects) {
			if(obj != null && obj.ParentId != -1) {
				obj.Parent.AddChild(obj);
			}			
		}
	}

    // ======================================================================
    // Child container management.
    // ----------------------------------------------------------------------
	public List<int> Children { get { return myChildren; }}
    // ----------------------------------------------------------------------
    public void AddChild(iCS_EditorObject toAdd) {
        int id= toAdd.InstanceId;
        if(Prelude.elem(id, myChildren.ToArray())) return;
        myChildren.Add(id);
    }
    // ----------------------------------------------------------------------
    public void RemoveChild(iCS_EditorObject toDelete) {
        int id= toDelete.InstanceId;
        for(int i= 0; i < myChildren.Count; ++i) {
            if(myChildren[i] == id) {
                myChildren.RemoveAt(i);
                return;
            }
        }
    }
    // ----------------------------------------------------------------------
    public bool AreChildrenInSameOrder(int[] orderedChildren) {
        int i= 0;
        for(int j= 0; j < Children.Count; ++j) {
            if(Children[j] == orderedChildren[i]) {
                if(++i >= orderedChildren.Length) return true;
            };
        }
        return false;
    }
    // ----------------------------------------------------------------------
    public void ReorderChildren(int[] orderedChildren) {
        if(AreChildrenInSameOrder(orderedChildren)) return;
        List<int> others= Prelude.filter(c=> Prelude.notElem(c,orderedChildren), Children);
        int i= 0;
        Prelude.forEach(c=> Children[i++]= c, orderedChildren);
        Prelude.forEach(c=> Children[i++]= c, others);
    }
   
}
