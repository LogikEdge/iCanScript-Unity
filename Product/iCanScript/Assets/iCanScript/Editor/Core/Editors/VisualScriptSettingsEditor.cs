using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Internal.Engine;
using TimedAction= iCanScript.Internal.Prelude.TimerService.TimedAction;

namespace iCanScript.Internal.Editor {
    using Prefs= PreferencesController;

    public class VisualScriptSettingsEditor : ConfigEditorBase {
        // =================================================================================
        // Fields
        // ---------------------------------------------------------------------------------
        iCS_IStorage    iStorage             = null;
    	string[]        vsConfigOptionStrings= new string[]{
    	    "General"
    	};
    	
        // =================================================================================
        // INITIALIZATION
        // ---------------------------------------------------------------------------------
        public static void Init(iCS_IStorage iStorage) {
            var editor= EditorWindow.CreateInstance<VisualScriptSettingsEditor>();
            editor.ShowUtility();
            editor.iStorage= iStorage;
        }

        // =================================================================================
        // INTERFACES TO BE PROVIDED
        // ---------------------------------------------------------------------------------
        protected override string   GetTitle() {
            return "Visual Script Settings";
        }
        protected override string[] GetMainSelectionGridStrings() {
            return vsConfigOptionStrings;
        }
        protected override void     ProcessSelection(int selection) {
            // -- Don't continue if our VS is not accessible --
            if(iStorage == null) {
                Close();
                return;
            }
            // -- Execute option specific panel. --
            switch(selection) {
                case 0: General(); break;
                default: break;
            }
        }
        

    	// =================================================================================
        // DISPLAY OPTION PANEL
        // ---------------------------------------------------------------------------------
        void General() {
            // -- Label column --
            var pos= GetLabelColumnPositions(6);
            GUI.Label(pos[0], "Type Name");
            GUI.Label(pos[2], "Is Editor Script");
            GUI.Label(pos[3], "Base Type Name");
            EditorGUI.BeginDisabledGroup(true);
            GUI.Label(pos[5], "Namespace");
            EditorGUI.EndDisabledGroup();
            
            // -- Value column --
            pos= GetValueColumnPositions(6);
            iStorage.TypeName= EditorGUI.TextField(pos[0], iStorage.TypeName);
            var savedGuiChanged= GUI.changed;
            GUI.changed= false;
            iStorage.IsEditorScript= EditorGUI.Toggle(pos[2], iStorage.IsEditorScript);
            if(GUI.changed) {
                UpdateEditorScriptOption();
            }
            GUI.changed |= savedGuiChanged;
            iStorage.BaseType= EditorGUI.TextField(pos[3], iStorage.BaseType);
            GUI.Label(pos[4], "<i>(format: namespace.type)</i>");
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.TextField(pos[5], iStorage.Namespace);
            EditorGUI.EndDisabledGroup();
    
            // -- Reset button --
            if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
                iStorage.TypeName         = iStorage.EditorObjects[0].CodeName;
                iStorage.IsEditorScript   = false;
                iStorage.BaseType         = CodeGenerationUtility.DefaultEngineBaseTypeString;
            }

    		// -- Save changes --
            if(GUI.changed) {
                iStorage.SaveStorage();
            }

            // -- Validate user entries --
            var message= Sanity.ValidateVisualScriptBaseType(iStorage);
            if(message != null) {
                DisplayError(pos[4], message);
                return;
            }
            message= Sanity.ValidateVisualScriptTypeName(iStorage, /*shortFormat=*/true);
            if(message != null) {
                DisplayError(pos[0], message);
                return;
            }
        }
        
        // ---------------------------------------------------------------------------------
        /// Updates the option to determines if this is an editor script.
        void UpdateEditorScriptOption() {
            if(iStorage.IsEditorScript) {
                iStorage.BaseTypeOverride= true;
                iStorage.BaseType        = CodeGenerationUtility.DefaultEditorBaseTypeString;
            }
            else {
                iStorage.BaseTypeOverride= false;
                iStorage.BaseType        = CodeGenerationUtility.DefaultEngineBaseTypeString;
            }
        }
    }

}
