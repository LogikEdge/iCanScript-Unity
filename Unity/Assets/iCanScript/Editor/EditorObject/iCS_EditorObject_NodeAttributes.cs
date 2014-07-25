using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  NODE ATTRIBUTES
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // Node specific attributes ---------------------------------------------
    public string MethodName {
		get { return EngineObject.MethodName; }
		set {
			var engineObject= EngineObject;
            if(engineObject.MethodName == value) return;
		    engineObject.MethodName= value;
		}
	}
    public int NbOfParams {
		get { return EngineObject.NbOfParams; }
		set {
			var engineObject= EngineObject;
            if(engineObject.NbOfParams == value) return;
		    engineObject.NbOfParams= value;
		}
	}
	public string IconPath {
		get {
			var guid= IconGUID;
			if(guid == null) return null;
			return AssetDatabase.GUIDToAssetPath(guid);
		}
		set { 
			if(string.IsNullOrEmpty(value)) {
				IconGUID= null;
				return;
			}
			var guid= iCS_TextureCache.IconPathToGUID(value);
			if(guid != null) {
				IconGUID= guid;
			}
		}
	}
    public string IconGUID {
		get { return EngineObject.IconGUID; }
		set {
			var engineObject= EngineObject;
			if(engineObject.IconGUID == value) return;
			engineObject.IconGUID= value;
		}
	}
	public int ExecutionPriority {
	    get { return EngineObject.ExecutionPriority; }
	    set {
			var engineObject= EngineObject;
            if(engineObject.ExecutionPriority == value) return;
	        engineObject.ExecutionPriority= value;
	    }
	}
    public MethodBase GetMethodBase(List<iCS_EditorObject> editorObjects) {
        return EngineObject.GetMethodBase(EditorToEngineList(editorObjects));
    }
    public FieldInfo GetFieldInfo() {
        return EngineObject.GetFieldInfo();
    }
}
