using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Editor {
public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Public interface Utilities
	// ----------------------------------------------------------------------
    bool BuildPublicInterfaceMenu(GameObject gameObject, iCS_EditorObject parent, iCS_VisualScriptImp vs, Vector2 mousePosition) {
        var publicVariables= iCS_VisualScriptData.FindPublicVariableDefinitions(vs);
        var userFunctions= iCS_VisualScriptData.FindFunctionDefinitions(vs);
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
                gMenu.AddItem(new GUIContent("+ "+pv.RawName), false,
                              o=> CreateReferenceToVariable(vs, o), pv); 
            }
            bool hasUserFunction= false;
            foreach(var puf in userFunctions) {
                if(hasUserFunction == false) {
                    gMenu.AddSeparator("");
                    gMenu.AddSeparator("User Functions");
                }
                hasUserFunction= true;
                gMenu.AddItem(new GUIContent("+ "+puf.RawName), false,
                              o=> CreateFunctionCall(vs, o), puf); 
            }
            gMenu.ShowAsContext();
            return true;
        }
        return false;
    }
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