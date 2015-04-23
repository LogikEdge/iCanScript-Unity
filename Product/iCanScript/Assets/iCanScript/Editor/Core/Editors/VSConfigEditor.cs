using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Engine;
using TimedAction= Prelude.TimerService.TimedAction;

namespace iCanScript.Editor {
    using Prefs= PreferencesController;

    public class VSConfigEditor : ConfigEditorBase {
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
            var editor= EditorWindow.CreateInstance<VSConfigEditor>();
            editor.ShowUtility();
            editor.iStorage= iStorage;
        }
        // =================================================================================
        // INTERFACES TO BE PROVIDED
        // ---------------------------------------------------------------------------------
        protected override string   GetTitle() {
            return "Visual Script Configuration";
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
            var pos= GetLabelColumnPositions(5);
            GUI.Label(pos[0], "Base Type Override");
            EditorGUI.BeginDisabledGroup(!iStorage.BaseTypeOverride);
            GUI.Label(pos[1], "Base Type Name");
            EditorGUI.EndDisabledGroup();
            GUI.Label(pos[3], "Namespace Override");
            EditorGUI.BeginDisabledGroup(!iStorage.NamespaceOverride);
            GUI.Label(pos[4], "Namespace");
            EditorGUI.EndDisabledGroup();
            
            // -- Value column --
            pos= GetValueColumnPositions(5);
            iStorage.BaseTypeOverride= EditorGUI.Toggle(pos[0], iStorage.BaseTypeOverride);
            EditorGUI.BeginDisabledGroup(!iStorage.BaseTypeOverride);
            iStorage.BaseType= EditorGUI.TextField(pos[1], iStorage.BaseType);
            GUI.Label(pos[2], "<i>(format: namespace.type)</i>");
            EditorGUI.EndDisabledGroup();
            iStorage.NamespaceOverride= EditorGUI.Toggle(pos[3], iStorage.NamespaceOverride);
            EditorGUI.BeginDisabledGroup(!iStorage.NamespaceOverride);
            iStorage.Namespace= EditorGUI.TextField(pos[4], iStorage.Namespace);
            EditorGUI.EndDisabledGroup();
    
            // -- Reset button --
            if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
                iStorage.BaseTypeOverride = false;
                iStorage.BaseType         = Prefs.DefaultBaseType;
                iStorage.NamespaceOverride= false;
                iStorage.Namespace        = Prefs.DefaultNamespace;
            }

    		// -- Save changes --
            if(GUI.changed) {
                iStorage.SaveStorage();                
            }

            // -- Validate user entries --
            var message= Sanity.ValidateVisualScriptBaseType(iStorage);
            if(message != null) {
                DisplayError(pos[1], message);
                return;
            }
            message= Sanity.ValidateVisualScriptNamespace(iStorage);
            if(message != null) {
                DisplayError(pos[4], message);
            }
        }
    }

}
