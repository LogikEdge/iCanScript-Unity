using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_EditorObject {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    iCS_IStorage    myIStorage       = null;
    int             myId             = -1;
    bool            myIsFloating     = false;
    List<int>		myChildren       = new List<int>();
    bool            myIsSticky       = false;

    // ======================================================================
    // Conversion Utilities
    // ----------------------------------------------------------------------
    public iCS_IStorage IStorage {
        get { return myIStorage; }
    }
	public iCS_VisualScriptData Storage {
		get { return myIStorage.Storage; }
	}
    public iCS_MonoBehaviourImp iCSMonoBehaviour {
        get { return myIStorage.iCSMonoBehaviour; }
    }
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
		}
	}
    // ----------------------------------------------------------------------
    public string Name {
		get {
            if(IsDataPort) {
                if(IsProgrammaticInstancePort) {
                    return "<"+iCS_Types.TypeName(RuntimeType)+" &>";
                }                
            }
            return EngineObject.Name;
        }
		set {
            var engineObject= EngineObject;
            if(engineObject.Name == value) return;
		    engineObject.Name= value;
		}
	}
    // ----------------------------------------------------------------------
    public string FullName {
        get { return Storage.GetFullName(iCSMonoBehaviour, EngineObject); }
    }
    // ----------------------------------------------------------------------
    public string DefaultName {
        get {
            var defaultName= Name;
            if(IsPackage) {
                defaultName= "";                
            }
            else if(IsConstructor) {
                defaultName= "Variable";
            }
            else {
                if(IsNode) {
                    var desc= iCS_LibraryDatabase.GetAssociatedDescriptor(this);
                    if(desc != null) {
                        defaultName= desc.DisplayName;
                    }
                    else {
                        defaultName= EngineObject.MethodName;                        
                    }
                }
                else {
                    // TODO: Support retreiving the initial port name.
                }
            }
            return defaultName+"<"+iCS_Types.TypeName(iCS_Types.RemoveRefOrPointer(RuntimeType))+">";            
        }
    }
    // ----------------------------------------------------------------------
    public bool IsNameEditable {
		get { return EngineObject.IsNameEditable && !IsMessageHandler; }
		set {
            var engineObject= EngineObject;
            if(engineObject.IsNameEditable == value) return;
		    engineObject.IsNameEditable= value;
		}
	}
    // ----------------------------------------------------------------------
    public string Tooltip {
		get { return EngineObject.Tooltip; }
		set {
            var engineObject= EngineObject;
            if(engineObject.Tooltip == value) return;
		    engineObject.Tooltip= value;
		}
	}
    // ----------------------------------------------------------------------
    public string Stereotype {
        get {
            if(!IsNode) return null;
            if(IsMessageHandler)    return "MessageHandler";
            if(IsConstructor)       return "Builder";
            if(IsTypeCast)          return "TypeCast";
            if(IsInstanceNode)      return "Instance";
            if(IsStateChart)        return "StateChart";
            if(IsState)             return "State";
            if(IsTransitionPackage) return "Trigger";
            if(IsKindOfFunction)    return "Function";
            if(IsKindOfPackage)     return "Package";
            return "";
        }
    }
    // ----------------------------------------------------------------------
    public string NodeTitle {
        get {
            if(iCS_PreferencesController.ShowNodeStereotype) {
                return DisplayName+" <"+Stereotype+">";
            }
            return DisplayName;
        }
    }
	
    // ======================================================================
    // High-Level Properties
    // ----------------------------------------------------------------------
	public bool IsIdValid(int id)	{ return id >= 0 && id < EditorObjects.Count && id < EngineObjects.Count && EditorObjects[id] != null; }
	public bool	IsParentValid		{ get { return IsIdValid(ParentId); }}
	public bool IsSourceValid		{ get { return IsIdValid(ProducerPortId); }}

	public bool IsValid {
		get { return IsIdValid(InstanceId); }
	}
    public int InstanceId { 
		get { return myId; }
	}
    public iCS_EditorObject Parent {
		get { return IsParentValid ? myIStorage[ParentId] : null; }
		set { ParentId= (value != null ? value.InstanceId : -1); }
	}
	public iCS_EditorObject ParentNode {
	    get {
	        var parent= Parent;
	        while(parent != null && !parent.IsNode) parent= parent.Parent;
	        return parent;
	    }
	}
    public bool IsFloating {
		get { return myIsFloating; }
		set {
            if(myIsFloating == value) return;
		    myIsFloating= value;
		}
	}
	public bool IsParentFloating {
		get {
			for( var parent= ParentNode; parent != null; parent= parent.ParentNode)  {
				if( parent.IsFloating ) return true;
			}
			return false;
		}
	}
	public bool IsSticky {
	    get { return myIsSticky; }
	    set { myIsSticky= value; }
	}
	public bool IsDisplayRoot {
	    get {
            return this.InstanceId == Storage.DisplayRoot;
	    }
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
        RunOnCreated(editorObject);
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
        editorObject.LocalSize= toClone.LocalSize;
        RunOnCreated(editorObject);
        if(editorObject.IsInDataOrControlPort && toClone.ProducerPortId == -1) {
            editorObject.InitialValue= toClone.IStorage.GetInitialPortValueFromArchive(toClone);
            editorObject.IStorage.StoreInitialPortValueInArchive(editorObject);
        }
		return editorObject;
    }

    // ----------------------------------------------------------------------
    // Reinitialize the editor object to its default values.
    public void DestroyInstance() {
        // Invoke event
        RunOnWillDestroy(this);
        // Destroy any children.
        ForEachChild(child=> child.DestroyInstance());        
        // Disconnect any port sourcing from this object.
        if(IsPort) {
            myIStorage.ForEach(
                child=> {
                    if(child.IsPort && child.ProducerPortId == InstanceId) {
                        child.ProducerPortId= -1;
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
    }
    // ----------------------------------------------------------------------
	public static void RebuildFromEngineObjects(iCS_IStorage iStorage) {
		iStorage.EditorObjects.Clear();
		iStorage.EditorObjects.Capacity= iStorage.EngineObjects.Count;		
		for(int i= 0; i < iStorage.EngineObjects.Count; ++i) {
            iCS_EditorObject editorObj= null;
            var engineObj= iStorage.EngineObjects[i];
		    if(iCS_VisualScriptData.IsValid(engineObj, iStorage.VisualScript)) {
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
