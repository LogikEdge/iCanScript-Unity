using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Editor {
public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Public interface Utilities
	// ----------------------------------------------------------------------
    void CreateReferenceToVariable(iCS_VisualScriptImp vs, object _pv) {
        var engineObject= _pv as iCS_EngineObject;
        var globalPosition= GraphMousePosition;
        var parent= IStorage.GetNodeAt(globalPosition);
        if(parent != null) {
            QueueOnGUICommand(()=> iCS_UserCommands.CreateReferenceToVariable(parent, globalPosition, engineObject.RawName, vs, engineObject));
        }
    }
	// ----------------------------------------------------------------------
    void CreateFunctionCall(iCS_VisualScriptImp vs, object _puf) {
        var engineObject= _puf as iCS_EngineObject;
        var globalPosition= GraphMousePosition;
        var parent= IStorage.GetNodeAt(globalPosition);
        if(parent != null) {
            QueueOnGUICommand(()=> iCS_UserCommands.CreateFunctionCall(parent, globalPosition, engineObject.RawName, vs, engineObject));
        }
    }
}
}