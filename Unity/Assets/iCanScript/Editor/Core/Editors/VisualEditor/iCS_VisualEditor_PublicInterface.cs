using UnityEngine;
using UnityEditor;
using System.Collections;

public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Public interface Utilities
	// ----------------------------------------------------------------------
    void BuildPublicInterfaceMenu(GameObject gameObject, iCS_EditorObject parent, iCS_VisualScriptImp vs, Vector2 mousePosition) {
        var publicVariables= iCS_VisualScriptData.FindPublicVariables(vs);
        var userFunctions= iCS_VisualScriptData.FindUserFunctions(vs);
        if(publicVariables.Length != 0 || userFunctions.Length != 0) {
            GenericMenu gMenu= new GenericMenu();
            gMenu.AddItem(new GUIContent("+ <GameObject>"), false,
                o=> QueueOnGUICommand(()=> CreateGameObjectNode(gameObject, parent, mousePosition)), gameObject);
            bool hasVariable= false;
            foreach(var pv in publicVariables) {
                if(hasVariable == false) {
                    gMenu.AddSeparator("");
                    gMenu.AddSeparator("Variables");
                }
                hasVariable= true;
                gMenu.AddItem(new GUIContent("+ "+pv.Name), false,
                              o=> CreatePortProxy(vs, o), pv); 
            }
            bool hasUserFunction= false;
            foreach(var puf in userFunctions) {
                if(hasUserFunction == false) {
                    gMenu.AddSeparator("");
                    gMenu.AddSeparator("User Functions");
                }
                hasUserFunction= true;
                gMenu.AddItem(new GUIContent("+ "+puf.Name), false,
                              o=> CreateUserFunctionCall(vs, o), puf); 
            }
            gMenu.ShowAsContext();
        }
    }
	// ----------------------------------------------------------------------
    void CreatePortProxy(iCS_VisualScriptImp vs, object _pv) {
        var engineObject= _pv as iCS_EngineObject;
        var globalPosition= GraphMousePosition;
        var parent= IStorage.GetNodeAt(globalPosition);
        if(parent != null) {
            QueueOnGUICommand(()=> iCS_UserCommands.CreateVariableProxy(parent, globalPosition, engineObject.Name, vs, engineObject));
        }
    }
	// ----------------------------------------------------------------------
    void CreateUserFunctionCall(iCS_VisualScriptImp vs, object _puf) {
        var engineObject= _puf as iCS_EngineObject;
        var globalPosition= GraphMousePosition;
        var parent= IStorage.GetNodeAt(globalPosition);
        if(parent != null) {
            QueueOnGUICommand(()=> iCS_UserCommands.CreateUserFunctionCall(parent, globalPosition, engineObject.Name, vs, engineObject));
        }
    }
}
