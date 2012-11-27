using UnityEngine;
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
		set { EngineObject.MethodName= value; }
	}
    public int NbOfParams {
		get { return EngineObject.NbOfParams; }
		set { EngineObject.NbOfParams= value; }
	}
    public string IconGUID {
		get { return EngineObject.IconGUID; }
		set {
			var engineObject= EngineObject;
			if(engineObject.IconGUID == value) return;
			engineObject.IconGUID= value;
			IsDirty= true;
		}
	}
	public int ExecutionPriority {
	    get { return EngineObject.ExecutionPriority; }
	    set { EngineObject.ExecutionPriority= value; }
	}
    public MethodBase GetMethodBase(List<iCS_EditorObject> editorObjects) {
        return EngineObject.GetMethodBase(EditorToEngineList(editorObjects));
    }
    public FieldInfo GetFieldInfo() {
        return EngineObject.GetFieldInfo();
    }
}
