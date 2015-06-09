using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Editor {

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
            // -- Allow to edit Target port when it is a type variable. --
            bool allowEdit= vsObject.IsTargetPort && vsObject.IsTypeVariable;
            if(allowEdit || vsObject.IsNameEditable) {
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
            string description= vsObject.Description;
            if(string.IsNullOrEmpty(description)) description= EmptyStr;
            GUI.changed= false;
            EditorGUILayout.LabelField("Description");
            var newDescription= EditorGUILayout.TextArea(description,  GUILayout.Height(position.height - 30));
            if(GUI.changed) {
                iCS_UserCommands.ChangeDescription(vsObject, newDescription);
            }            
        }
        
        // -------------------------------------------------------------------
        /// Show parent information
        protected void ShowParent() {
            iCS_EditorObject parent= vsObject.ParentNode;
            EditorGUILayout.LabelField("Parent", parent.DisplayName);
        }
		
        // -------------------------------------------------------------------
		/// Convert an enumeration type to another.
		///
		/// @param value The enumeration value to be converted.
		/// @param defaultValue The value to be returned if conversion is
		///                     unsuccessful.
		///
        protected R ConvertEnum<R,T>(T value, R defaultValue) {
            var allowedValues= Enum.GetValues(defaultValue.GetType());
            foreach(var v in allowedValues) {
                if((int)Convert.ChangeType(v, typeof(int)) == (int)Convert.ChangeType(value, typeof(int))) {
                    return (R)v;
                }
            }
            return defaultValue;
        }
		
    }
    
}
