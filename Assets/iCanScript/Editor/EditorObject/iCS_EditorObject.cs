using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_EditorObject {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    iCS_IStorage    	myIStorage               = null;
    int             	myId                     = -1;
    bool            	myIsFloating             = false;
    bool            	myIsDirty                = false;
    public List<int>	myChildren               = new List<int>();
    Vector2             myDisplaySize            = Vector2.zero;
    Vector2             myLocalPosition          = Vector2.zero;
    Vector2             myGlobalPositionFromRatio= Vector2.zero;
    bool                myIsSticky               = false;
    
    // ======================================================================
    // Conversion Utilities
    // ----------------------------------------------------------------------
    public List<iCS_EditorObject> EditorObjects {
        get { return myIStorage.EditorObjects; }
    }
    public iCS_EditorObject EditorObject {
		get { return EditorObjects[myId]; }
	}
    public List<iCS_EngineObject> EngineObjects {
        get { return myIStorage.EngineObjects; }
    }
    public iCS_EngineObject EngineObject {
		get { return EngineObjects[myId]; }
	}
    List<iCS_EngineObject> EditorToEngineList(List<iCS_EditorObject> editorObjects) {
        return Prelude.map(eo=> (eo != null ? eo.EngineObject : null), editorObjects);
    }
    
    // ======================================================================
    // Engine Object Accessors
    // ----------------------------------------------------------------------
    public iCS_ObjectTypeEnum ObjectType {
		get { return EngineObject.ObjectType; }
		set {
            var engineObject= EngineObject;
            if(engineObject.ObjectType == value) return;
		    engineObject.ObjectType= value;
		    IsDirty= true;
		}
	}
    // ----------------------------------------------------------------------
    public int ParentId {
		get { return EngineObject.ParentId; }
		set {
			int pid= EngineObject.ParentId;
			if(pid == value) return;
			if(IsIdValid(pid)) {
				var oldParent= EditorObjects[pid];
				oldParent.RemoveChild(this);
			}
			EngineObject.ParentId= value;
			if(IsIdValid(value)) {
				var newParent= EditorObjects[value];
				newParent.AddChild(this);
			}
		}
	}
    // ----------------------------------------------------------------------
    public iCS_DisplayOptionEnum DisplayOption {
        get { return EngineObject.DisplayOption; }
        set {
            var engineObject= EngineObject;
            if(engineObject.DisplayOption == value) return;
            engineObject.DisplayOption= value;
            IsDirty= true;
        }
    }
    // ----------------------------------------------------------------------
    public Type RuntimeType {
		get { return EngineObject.RuntimeType; }
	}
    // ----------------------------------------------------------------------
    public string RawName {
		get { return EngineObject.RawName; }
		set {
            var engineObject= EngineObject;
            if(engineObject.RawName == value) return;
		    engineObject.RawName= value;
		    IsDirty= true;
		}
	}
    // ----------------------------------------------------------------------
    public string Name {
		get { return EngineObject.Name; }
		set {
            var engineObject= EngineObject;
            if(engineObject.Name == value) return;
		    engineObject.Name= value;
		    IsDirty= true;
		}
	}
    // ----------------------------------------------------------------------
    public bool IsNameEditable {
		get { return EngineObject.IsNameEditable; }
		set { EngineObject.IsNameEditable= value; }
	}
    // ----------------------------------------------------------------------
    public string Tooltip {
		get { return EngineObject.Tooltip; }
		set { EngineObject.Tooltip= value; }
	}
    
    // ======================================================================
    // High-Level Properties
    // ----------------------------------------------------------------------
	public bool IsIdValid(int id)	{ return id >= 0 && id < EditorObjects.Count; }
	public bool	IsParentValid		{ get { return IsIdValid(ParentId); }}
	public bool IsSourceValid		{ get { return IsIdValid(SourceId); }}
	
    public int InstanceId { 
		get { return myId; }
	}
    public iCS_EditorObject Parent {
		get { return IsParentValid ? myIStorage[ParentId] : null; }
		set { ParentId= (value != null ? value.InstanceId : -1); }
	}
    public bool IsFloating {
		get { return myIsFloating; }
		set {
            if(myIsFloating == value) return;
		    myIsFloating= value;
		    IsDirty= true;
		}
	}
    public bool IsDirty {
		get { return myIsDirty; }
		set {
            if(myIsDirty == value) return;
		    myIsDirty= value;
		    if(value) {
		        myIStorage.IsDirty= true;
		    }
	    }
	}
	public bool IsSticky {
	    get { return myIsSticky; }
	    set { myIsSticky= value; }
	}
	public Vector2 GlobalPositionFromRatio {
	    get { return myGlobalPositionFromRatio; }
	    set { myGlobalPositionFromRatio= value; }
	}
	
    // ======================================================================
    // Constructors/Builders
    // ----------------------------------------------------------------------
	// Creates an instance of an editor/engine object pair.
	public static iCS_EditorObject CreateInstance(int id, string name, Type type,
												  int parentId, iCS_ObjectTypeEnum objectType,
                            					  iCS_IStorage iStorage) {
		if(id < 0) return null;
		// Create engine object.
		var engineObject= new iCS_EngineObject(id, name, type, parentId, objectType);
		AddEngineObject(id, engineObject, iStorage);
		// Create editor object.
		var editorObject= new iCS_EditorObject(id, iStorage);
		AddEditorObject(id, editorObject);
		editorObject.IsDirty= true;
		return editorObject;
	}
    // ----------------------------------------------------------------------
    // Duplicate the given editor object with a new id and parent.
    public static iCS_EditorObject Clone(int id, iCS_EditorObject toClone, iCS_EditorObject parent,
                                         iCS_IStorage iStorage) {
		if(id < 0) return null;
		// Create engine object.
        var engineObject= iCS_EngineObject.Clone(id, toClone.EngineObject,
												 (parent == null ? null : parent.EngineObject));
		AddEngineObject(id, engineObject, iStorage);
		// Create editor object.
		var editorObject= new iCS_EditorObject(id, iStorage);
		AddEditorObject(id, editorObject);
        editorObject.DisplaySize= toClone.DisplaySize;
		editorObject.IsDirty= true;
		return editorObject;
    }

    // ----------------------------------------------------------------------
    // Reinitialize the editor object to its default values.
    public void DestroyInstance() {
        // Destroy any children.
        ForEachChild(child=> child.DestroyInstance());        
        // Disconnect any port sourcing from this object.
        if(IsPort) {
            myIStorage.ForEach(
                child=> {
                    if(child.IsPort && child.SourceId == InstanceId) {
                        child.SourceId= -1;
                    }                    
                }
            );
        }
		// Update child lists.
		if(IsParentValid) {
			Parent.RemoveChild(this);
		}
        // Assure that the selected object is not us.
        if(myIStorage.SelectedObject == EditorObject) myIStorage.SelectedObject= null;
		// Reset the associated engine object.
        EngineObject.DestroyInstance();
        // Remove editor object.
        EditorObjects[myId]= null;
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
			GrowEngineObjectList(id, iStorage);
			iStorage.EngineObjects.Add(engineObject);
		}		
	}
    // ----------------------------------------------------------------------
	private static void AddEditorObject(int id, iCS_EditorObject editorObject) {
		var iStorage= editorObject.myIStorage;
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
			GrowEditorObjectList(id, iStorage);
			iStorage.EditorObjects.Add(editorObject);
		}		
	}
	private static void GrowEngineObjectList(int size, iCS_IStorage iStorage) {
        // Reserve space to contain the total amount of objects.
        if(size > iStorage.EngineObjects.Capacity) {
            iStorage.EngineObjects.Capacity= size;
        }
        // Add the number of missing objects.
        for(int len= iStorage.EngineObjects.Count; size > len; ++len) {
            iStorage.EngineObjects.Add(iCS_EngineObject.CreateInvalidInstance());
        }
	}
	private static void GrowEditorObjectList(int size, iCS_IStorage iStorage) {
        // Reserve space to contain the total amount of objects.
        if(size > iStorage.EditorObjects.Capacity) {
            iStorage.EditorObjects.Capacity= size;
        }
        // Add the number of missing objects.
        for(int len= iStorage.EditorObjects.Count; size > len; ++len) {
            iStorage.EditorObjects.Add(null);
        }
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
		iStorage.EditorObjects.Capacity= iStorage.EngineObjects.Count;		
		for(int i= 0; i < iStorage.EngineObjects.Count; ++i) {
            iCS_EditorObject editorObj= null;
		    if(iStorage.EngineObjects[i].InstanceId != -1) {
		        editorObj= new iCS_EditorObject(i, iStorage);
		    }
	        iStorage.EditorObjects.Add(editorObj);
		}
		RebuildChildrenLists(iStorage);
	}
    // ----------------------------------------------------------------------
	private static void RebuildChildrenLists(iCS_IStorage iStorage) {
		iStorage.ForEach(
		    obj=> {
    			if(obj.IsParentValid) {
    				obj.Parent.AddChild(obj);
    			}					        
		    }
		);
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
		IsDirty= true;
    }

}
