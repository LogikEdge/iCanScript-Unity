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
        // COMMON UTILITIES
        // -------------------------------------------------------------------
        /// Edits the name of the visual script object.
        ///
        /// @param label The label to use for the name.
        ///
        protected void EditName(string label) {
            string name= vsObject.DisplayName;
            if(string.IsNullOrEmpty(name)) name= EmptyStr;
            if(vsObject.IsNameEditable) {
                GUI.changed= false;
                var newName= EditorGUILayout.TextField(label, vsObject.DisplayName);
                if(GUI.changed) {
                    iCS_UserCommands.ChangeName(vsObject, newName);
                }
            } else {
                EditorGUILayout.LabelField(label, name);                    
            }
        }
        
        // -------------------------------------------------------------------
        /// Edits the object description.
        protected void EditDescription() {
            string tooltip= vsObject.Tooltip;
            if(string.IsNullOrEmpty(tooltip)) tooltip= EmptyStr;
            GUI.changed= false;
            EditorGUILayout.LabelField("Description");
            var newTooltip= EditorGUILayout.TextArea(tooltip,  GUILayout.Height(position.height - 30));
            if(GUI.changed) {
                iCS_UserCommands.ChangeTooltip(vsObject, newTooltip);
            }            
        }
        
        // -------------------------------------------------------------------
        /// Show parent information
        protected void ShowParent() {
            iCS_EditorObject parent= vsObject.ParentNode;
            EditorGUILayout.LabelField("Parent", parent.DisplayName);
        }
    }
    
}
