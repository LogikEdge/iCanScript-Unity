using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Subspace;
using Prefs= iCS_PreferencesController;

public partial class iCS_EditorObject {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------

    // ----------------------------------------------------------------------
	public string DisplayName {
		get {
			var name= iCS_PreferencesEditor.RemoveProductPrefix(Name);
			if(Prefs.ShowRuntimeFrameId && IsNode && Application.isPlaying) {
				var action= GetRuntimeObject as SSAction;
				if(action != null) {
					name+= " ("+action.ExecutedRunId+")";
				}
			}
			return name;
		}
	}
    // ----------------------------------------------------------------------
	public SSObject GetRuntimeObject {
		get {
	        iCS_VisualScriptImp bh= IStorage.iCSMonoBehaviour as iCS_VisualScriptImp;
	        return bh == null ? null : (bh.GetRuntimeObject(InstanceId) as SSObject);
		}
	}
    // ----------------------------------------------------------------------
	public int GetExecutionFrameId {
		get {
			var action= GetRuntimeObject as SSAction;
			return action != null ? action.ExecutedRunId : 0;
		}
	}
}
