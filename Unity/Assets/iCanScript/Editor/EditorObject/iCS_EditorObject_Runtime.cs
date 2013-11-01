using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_EditorObject {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------

    // ----------------------------------------------------------------------
	public string DisplayName {
		get {
			var name= iCS_PreferencesEditor.RemoveProductPrefix(Name);
			if(iCS_PreferencesEditor.ShowRuntimeFrameId && IsNode && Application.isPlaying) {
				var action= GetRuntimeObject as iCS_Action;
				if(action != null) {
					name+= " ("+action.ExecutionFrameId+")";
				}
			}
			return name;
		}
	}
    // ----------------------------------------------------------------------
	public iCS_Object GetRuntimeObject {
		get {
	        iCS_VisualScriptImp bh= Storage as iCS_VisualScriptImp;
	        return bh == null ? null : (bh.GetRuntimeObject(InstanceId) as iCS_Object);
		}
	}
    // ----------------------------------------------------------------------
	public int GetCurrentFrameId {
		get {
			var action= GetRuntimeObject as iCS_Action;
			return action != null ? action.CurrentFrameId : 0;
		}
	}
    // ----------------------------------------------------------------------
	public int GetExecutionFrameId {
		get {
			var action= GetRuntimeObject as iCS_Action;
			return action != null ? action.ExecutionFrameId : 0;
		}
	}
}
