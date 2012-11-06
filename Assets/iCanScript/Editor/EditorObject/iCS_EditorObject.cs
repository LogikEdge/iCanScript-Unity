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
    public List<iCS_EditorObject> EditorObjects {
        get { return myIStorage.EditorObjects; }
    }
    public iCS_EditorObject EditorObject {
		get { return myId >= 0 && myId < EditorObjects.Count ? EditorObjects[myId] : null; }
	}
    public List<iCS_EngineObject> EngineObjects {
        get { return myIStorage.EngineObjects; }
    }
    public iCS_EngineObject EngineObject {
		get { return myId >= 0 && myId < EngineObjects.Count ? EngineObjects[myId] : null; }
	}
    List<iCS_EngineObject> EditorToEngineList(List<iCS_EditorObject> editorObjects) {
        return Prelude.map(eo=> (eo != null ? eo.EngineObject : null), editorObjects);
    }
    
    // ======================================================================
    // Proxy Methods
    // ----------------------------------------------------------------------
	public bool IsIdValid(int id)	{ return id >= 0 && id < EditorObjects.Count; }
    public bool IsValid				{ get { return IsIdValid(myId); }}
	public bool	IsParentValid		{ get { return IsValid && IsIdValid(ParentId); }}
	public bool IsSourceValid		{ get { return IsValid && IsIdValid(SourceId); }}
	
    public iCS_ObjectTypeEnum ObjectType {
		get { return EngineObject.ObjectType; }
		set { EngineObject.ObjectType= value; IsDirty= true; }
	}
    public int InstanceId { 
		get { return myId; }
	}
    public int ParentId {
		get { return EngineObject.ParentId; }
		set {
			int pid= EngineObject.ParentId;
			if(pid == value) return;
			if(IsIdValid(pid)) {
				EditorObjects[pid].RemoveChild(this);
				EditorObjects[pid].IsDirty= true;
			}
			EngineObject.ParentId= value;
			if(IsIdValid(value)) {
				EditorObjects[value].AddChild(this);
				EditorObjects[value].IsDirty= true;
			}
		}
	}
    public iCS_EditorObject Parent {
		get { return IsParentValid ? myIStorage[ParentId] : null; }
		set { if(IsValid) ParentId= (value != null ? value.InstanceId : -1); }
	}
    public Type RuntimeType {
		get { return EngineObject.RuntimeType; }
	}
    public string RawName {
		get { return EngineObject.RawName; }
		set { EngineObject.RawName= value; IsDirty= true; }
	}
    public string Name {
		get { return EngineObject.Name; }
		set { EngineObject.Name= value; IsDirty= true; }
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
		set { myIsFloating= value; IsDirty= true; }
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
	// Creates an instance of an editor/engine object pair.
	public static iCS_EditorObject CreateInstance(int id, string name, Type type,
												  int parentId, iCS_ObjectTypeEnum objectType,
                            					  Rect localPosition, iCS_IStorage iStorage) {
		if(id < 0) return null;
		// Create engine object.
		var engineObject= new iCS_EngineObject(id, name, type, parentId, objectType, localPosition);
		AddEngineObject(id, engineObject, iStorage);
		// Create editor object.
		var editorObject= new iCS_EditorObject(id, iStorage);
		AddEditorObject(id, editorObject);
		return editorObject;
	}
    // ----------------------------------------------------------------------
    // Duplicate the given editor object with a new id and parent.
    public static iCS_EditorObject Clone(int id, iCS_EditorObject toClone, iCS_EditorObject parent,
                                         Vector2 localPosition, iCS_IStorage iStorage) {
		if(id < 0) return null;
		// Create engine object.
        var engineObject= iCS_EngineObject.Clone(id, toClone.EngineObject,
												 (parent == null ? null : parent.EngineObject),
												 localPosition);
		AddEngineObject(id, engineObject, iStorage);
		// Create editor object.
		var editorObject= new iCS_EditorObject(id, iStorage);
		AddEditorObject(id, editorObject);
		return editorObject;
    }

    // ----------------------------------------------------------------------
    // Reinitialize the editor object to its default values.
    public void DestroyInstance() {
        // Destroy any children.
        ForEachChild(child=> child.DestroyInstance());        
        // Disconnect any port sourcing from this object.
        if(IsPort) {
            foreach(var child in EditorObjects) {
                if(child != null && child.IsPort) {
                    if(child.SourceId == InstanceId) {
                        child.SourceId= -1;
                    }
                }
            }
        }
		// Update child lists.
		if(ParentId != -1) Parent.RemoveChild(this);
		if(Children != null) Children.Clear();
		// Clear transient data.
        IsFloating= false;
        IsDirty= false;
		InitialValue= null;
		myAnimatedPosition.Reset();
		// Reset the associated engine object.
        EngineObject.DestroyInstance();
    }
    
    // ======================================================================
	// Grow database if needed.
    // ----------------------------------------------------------------------
	private static void AddEngineObject(int id, iCS_EngineObject engineObject, iCS_IStorage iStorage) {
		int len= iStorage.EngineObjects.Count;
		if(id < 0) return;
		if (id < len) {
			iStorage.EngineObjects[id]= engineObject;
			return;
		}
		if(id == len) {
			iStorage.EngineObjects.Add(engineObject);
			return;
		}
		if(id > len) {
			GrowEngineObjects(id-1, iStorage);
			iStorage.EngineObjects.Add(engineObject);
		}		
	}
	private static void AddEditorObject(int id, iCS_EditorObject editorObject) {
		var iStorage= editorObject.myIStorage;
		SanityCheck(iStorage);
		int len= iStorage.EditorObjects.Count;
		if(id < 0) return;
		if (id < len) {
			iStorage.EditorObjects[id]= editorObject;
			return;
		}
		if(id == len) {
			iStorage.EditorObjects.Add(editorObject);
			return;
		}
		if(id > len) {
			GrowEditorObjects(id-1, iStorage);
			iStorage.EditorObjects.Add(editorObject);
		}		
	}
	private static void GrowObjects(int id, iCS_IStorage iStorage) {
		GrowEngineObjects(id, iStorage);
		GrowEditorObjects(id, iStorage);
	}
	private static void GrowEngineObjects(int id, iCS_IStorage iStorage) {
        if(id >= 0) {
            // Grow engine object array if needed.
            for(int len= iStorage.EngineObjects.Count; id >= len; ++len) {
                iStorage.EngineObjects.Add(iCS_EngineObject.CreateInvalidInstance());
            }
        }
	}
	private static void GrowEditorObjects(int id, iCS_IStorage iStorage) {
        if(id >= 0) {
            // Grow engine object array if needed.
            for(int len= iStorage.EditorObjects.Count; id >= len; ++len) {
                iStorage.EditorObjects.Add(CreateInvalidInstance(iStorage));
            }
        }		
	}
	// ----------------------------------------------------------------------
	// Creates an invalid editor object to be used as a placeholder.
	public static iCS_EditorObject CreateInvalidInstance(iCS_IStorage iStorage) {
		return new iCS_EditorObject(-1, iStorage);
	}
    
    // ======================================================================
	// Rebuild from engine database.
    // ----------------------------------------------------------------------
    public iCS_EditorObject(int id, iCS_IStorage iStorage) {
        myIStorage= iStorage;
        myId= id;
		var parent= Parent;
		if(parent != null) parent.AddChild(this);
        IsDirty= true;        
    }
    // ----------------------------------------------------------------------
	public static void RebuildFromEngineObjects(iCS_IStorage iStorage) {
		iStorage.EditorObjects.Clear();			
		foreach(var engineObject in iStorage.EngineObjects) {
			iCS_EditorObject editorObject= new iCS_EditorObject(engineObject.InstanceId, iStorage);
			iStorage.EditorObjects.Add(editorObject);
		}
		RebuildChildrenLists(iStorage);
	}
    // ----------------------------------------------------------------------
	private static void RebuildChildrenLists(iCS_IStorage iStorage) {
		foreach(var obj in iStorage.EditorObjects) {
			if(obj.IsParentValid) {
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
		IsDirty= true;
    }
    // ----------------------------------------------------------------------
    public void RemoveChild(iCS_EditorObject toDelete) {
        int id= toDelete.InstanceId;
        for(int i= 0; i < myChildren.Count; ++i) {
            if(myChildren[i] == id) {
                myChildren.RemoveAt(i);
				IsDirty= true;
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
   
    // ======================================================================
	// Returns true if Editor and Engine object containers match.
	public static bool SanityCheck(iCS_IStorage iStorage) {
		if(iStorage.EngineObjects.Count != iStorage.EditorObjects.Count) {
			Debug.LogWarning("iCanScript: Mismatch in Engine & Editor container size !!!");
			return false;
		}
		return true;
	}
}
