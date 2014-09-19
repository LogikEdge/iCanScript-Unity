using UnityEngine;
using UnityEditor;
using System.Collections;

public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Public interface Utilities
	// ----------------------------------------------------------------------
    void BuildPublicInterfaceMenu(iCS_VisualScriptImp vs) {
        var publicObjects= iCS_VisualScriptData.GetPublicObjects(vs);
        if(publicObjects != null) {
            GenericMenu gMenu= new GenericMenu();
            var publicVariables= iCS_VisualScriptData.GetPublicVariables(publicObjects);
            bool hasVariable= false;
            foreach(var pv in publicVariables) {
                if(hasVariable == false) {
                    gMenu.AddSeparator("");
                    gMenu.AddSeparator("Variables");
                }
                hasVariable= true;
                gMenu.AddItem(new GUIContent(pv.Name), false,
                              o=> CreateVariableProxy(vs, o), pv); 
            }
            bool hasUserFunction= false;
            var publicUserFunctions= iCS_VisualScriptData.GetPublicUserFunctions(publicObjects);
            foreach(var puf in publicUserFunctions) {
                if(hasUserFunction == false) {
                    gMenu.AddSeparator("");
                    gMenu.AddSeparator("User Functions");
                }
                hasUserFunction= true;
                gMenu.AddItem(new GUIContent(puf.Name), false,
                              o=> CreateUserFunctionProxy(vs, o), puf); 
            }
            gMenu.ShowAsContext();
        }
    }
    void CreateVariableProxy(iCS_VisualScriptImp vs, object _pv) {
        var pv= _pv as iCS_EngineObject;
        Debug.Log("Trying to create variable proxy=> "+pv.Name);
    }
    void CreateUserFunctionProxy(iCS_VisualScriptImp vs, object _puf) {
        var puf= _puf as iCS_EngineObject;
        Debug.Log("trying to create user fucntion proxy=> "+puf.Name);
    }
}
