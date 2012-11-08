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
    // Proxy Methods
    // ----------------------------------------------------------------------
	public bool IsIdValid(int id)	{ return id >= 0 && id < EditorObjects.Count; }
	public bool	IsParentValid		{ get { return IsIdValid(ParentId); }}
	public bool IsSourceValid		{ get { return IsIdValid(SourceId); }}
	
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
		set { ParentId= (value != null ? value.InstanceId : -1); }
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
		set {
            if(myIsDirty == value) return;
		    myIsDirty= value;
		    if(value) {
		        myIStorage.IsDirty= true;
//                Debug.Log(Name+" is dirty");
		    }
	    }
	}
    public Rect LocalPosition {
		get {
            var engObj= EngineObject;
		    float x= engObj.LocalPosition.x-0.5f*engObj.DisplaySize.x;
		    float y= engObj.LocalPosition.y-0.5f*engObj.DisplaySize.y;
		    return new Rect(x, y, engObj.DisplaySize.x, engObj.DisplaySize.y);
		}
		set {
		    float x= value.x+0.5f*value.width;
		    float y= value.y+0.5f*value.height;
		    EngineObject.LocalPosition= new Vector2(x, y);
		    EngineObject.DisplaySize= new Vector2(value.width, value.height);
		}
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
		var engineObject= new iCS_EngineObject(id, name, type, parentId, objectType, Math3D.ToVector2(localPosition), new Vector2(localPosition.width, localPosition.height));
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
            myIStorage.ForEach(
                child=> {
                    if(child.IsPort && child.SourceId == InstanceId) {
                        child.SourceId= -1;
                    }                    
                }
            );
        }
		// Update child lists.
		if(IsParentValid) Parent.RemoveChild(this);
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
			GrowEngineObjects(id, iStorage);
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
			GrowEditorObjects(id, iStorage);
			iStorage.EditorObjects.Add(editorObject);
		}		
	}
	private static void GrowEngineObjects(int size, iCS_IStorage iStorage) {
        // Reserve space to contain the total amount of objects.
        if(size > iStorage.EngineObjects.Capacity) {
            iStorage.EngineObjects.Capacity= size;
        }
        // Add the number of missing objects.
        for(int len= iStorage.EngineObjects.Count; size > len; ++len) {
            iStorage.EngineObjects.Add(iCS_EngineObject.CreateInvalidInstance());
        }
	}
	private static void GrowEditorObjects(int size, iCS_IStorage iStorage) {
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
		    if(iStorage.EngineObjects[i].InstanceId == -1) {
		        iStorage.EditorObjects.Add(null);
		    } else {
		        iStorage.EditorObjects.Add(new iCS_EditorObject(i, iStorage));
		    }
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
    }

}
