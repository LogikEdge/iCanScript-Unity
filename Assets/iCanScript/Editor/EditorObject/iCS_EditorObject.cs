using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_EditorObject {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    iCS_IStorage    myIStorage  = null;
    int             myId        = -1;
    bool            myIsFloating= false;
    bool            myIsDirty   = false;
    
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
		set { EngineObject.ParentId= value; }
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
            for(int len= iStorage.EngineObjects.Count; id >= len; ++len) {
                iStorage.EngineObjects.Add(null);
            }
            iStorage.EngineObjects[id]= engineObject;            
        }
        Init(id, iStorage);
    }
    // ----------------------------------------------------------------------
    public iCS_EditorObject(int id, string name, Type type, int parentId, iCS_ObjectTypeEnum objectType,
                            Rect localPosition, iCS_IStorage iStorage) 
    : this(id, iStorage, new iCS_EngineObject(id, name, type, parentId, objectType, localPosition))
    {}
    // ----------------------------------------------------------------------
    public iCS_EditorObject(iCS_IStorage iStorage, iCS_EngineObject engineObject) {
        Init(engineObject == null ? -1 : engineObject.InstanceId, iStorage);
    }
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
        IsFloating= false;
        IsDirty= false;
		InitialValue= null;
        EngineObject.Reset();
    }    
}
