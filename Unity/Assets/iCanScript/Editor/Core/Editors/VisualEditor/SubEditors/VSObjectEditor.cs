using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Editor {

    public class VSObjectEditor : EditorWindow {
        // ======================================================================
        // Constants.
    	// ----------------------------------------------------------------------
        protected const string EmptyStr= "(empty)";

        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        protected iCS_EditorObject  vsObject= null;
        
            
        // ===================================================================
        // FUNCTIONS TO OVERRIDE FOR EACH SPECIFIC EDITOR
        // -------------------------------------------------------------------
        protected virtual void OnObjectSpecificGUI() {}
        
        // ===================================================================
        // EDITOR ENTRY POINT
        // -------------------------------------------------------------------
    	public void OnGUI() {
            EditorGUI.indentLevel= 0;

            // Display object name.
            string name= vsObject.DisplayName;
            if(string.IsNullOrEmpty(name)) name= EmptyStr;
            if(vsObject.IsNameEditable) {
                GUI.changed= false;
                var newName= EditorGUILayout.TextField("Name", vsObject.DisplayName);
                if(GUI.changed) {
                    iCS_UserCommands.ChangeName(vsObject, newName);
                }
            } else {
                EditorGUILayout.LabelField("Name", name);                    
            }

            // Display Type information.
            var typeName= iCS_ObjectNames.ToTypeName(iCS_Types.TypeName(vsObject.RuntimeType));
            if(!string.IsNullOrEmpty(typeName)) {
                EditorGUILayout.LabelField("Type", typeName);
            }
            
            // Object Specific editor.
            OnObjectSpecificGUI();
            
            // Show object description.
            string tooltip= vsObject.Tooltip;
            if(string.IsNullOrEmpty(tooltip)) tooltip= EmptyStr;
            GUI.changed= false;
            EditorGUILayout.LabelField("Description");
            var newTooltip= EditorGUILayout.TextArea(tooltip,  GUILayout.Height(position.height - 30));
            if(GUI.changed) {
                iCS_UserCommands.ChangeTooltip(vsObject, newTooltip);
            }
    	}
        
    }
    
}
