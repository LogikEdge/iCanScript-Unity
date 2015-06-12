using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Editor {
    public static class iCS_EditorController {
        // =================================================================================
        // Fields
        // ---------------------------------------------------------------------------------
        static List<iCS_EditorBase>   myEditors= null;
        
        // =================================================================================
        // Initialization
        // ---------------------------------------------------------------------------------
        static iCS_EditorController() {
            myEditors= new List<iCS_EditorBase>();
        }
        public static void Start()    {}
        public static void Shutdown() {}
        
        // =================================================================================
        // Window management
        // ---------------------------------------------------------------------------------
        // FIXME: The relationship between proxy type and editor name needs to be cleaned up.
        public static void Add(iCS_EditorBase editor) {
            if(FindIndexOf(editor) == -1) {
                myEditors.Add(editor);            
            }
        }
        public static void Remove(iCS_EditorBase editor) {
            int idx= FindIndexOf(editor);
            if(idx >= 0) myEditors.RemoveAt(idx);
        }
        
        // =================================================================================
        // Search/Iterations
        // ---------------------------------------------------------------------------------
        static int FindIndexOf(iCS_EditorBase editor) {
            for(int i= 0; i < myEditors.Count; ++i) {
                if(myEditors[i] == editor) {
                    return i;
                }
            }        
            return -1;
        }
        
        // ======================================================================
        public static void ShowVisualEditor() {
            EditorApplication.ExecuteMenuItem("iCanScript/Editors/Visual Editor");
        }
        public static void ShowHierarchyEditor() {
            EditorApplication.ExecuteMenuItem("iCanScript/Editors/Hierarchy");                
        }
        public static void ShowLibraryEditor() {
            EditorApplication.ExecuteMenuItem("iCanScript/Editors/Library");                
        }
        public static void ShowPreferences() {
            EditorApplication.ExecuteMenuItem("iCanScript/Preferences...");                
        }
    
        // ======================================================================
        public static EditorWindow FindWindow(Type type) {
            foreach(var ed in myEditors) {
                if(iCS_Types.IsA(type, ed.GetType())) {
                    return ed;
                }
            }
            return null;
        }
        public static EditorWindow FindWindow<T>() {
            return FindWindow(typeof(T));
        }
        public static EditorWindow FindVisualEditorWindow() {
            return FindWindow<iCS_VisualEditor>();
        } 
        public static EditorWindow FindTreeViewEditorWindow() {
            return FindWindow<iCS_TreeViewEditor>();
        }
        public static EditorWindow FindLibraryEditorWindow() {
            return FindWindow<LibraryEditor>();
        }    
        // ======================================================================
        public static iCS_VisualEditor FindVisualEditor() {
            return FindWindow(typeof(iCS_VisualEditor)) as iCS_VisualEditor;
        } 
        public static iCS_TreeViewEditor FindTreeViewEditor() {
            return FindWindow(typeof(iCS_TreeViewEditor)) as iCS_TreeViewEditor;
        }
        public static LibraryEditor FindLibraryEditor() {
            return FindWindow(typeof(LibraryEditor)) as LibraryEditor;
        }    
        public static GlobalSettingsEditor FindGlobalSettingsEditor() {
            return FindWindow(typeof(GlobalSettingsEditor)) as GlobalSettingsEditor;
        }
        // ======================================================================
    	public static void RepaintVisualEditor() {
    		var editor= FindVisualEditor();
    		if(editor != null) {
    			editor.Repaint();			
    		}
    	}
        public static void RepaintTreeViewEditor() {
    		var editor= FindTreeViewEditor();
    		if(editor != null) {
    			editor.Repaint();			
    		}
        }
        public static void RepaintLibraryEditor() {
    		var editor= FindLibraryEditor();
    		if(editor != null) {
    			editor.Repaint();			
    		}
        }
    	public static void RepaintGlobalSettingsEditor() {
    		var editor= FindGlobalSettingsEditor();
    		if(editor != null) {
    			editor.Repaint();			
    		}
    	}
        public static void RepaintAllEditors() {
            foreach(var editor in myEditors) {
                editor.Repaint();
            }
        }
        public static void RepaintEditorsWithLabels() {
            RepaintVisualEditor();
            RepaintTreeViewEditor();
        }
        public static void RepaintEditorsWithValues() {
            RepaintVisualEditor();
        }
        // ======================================================================
        public static void ShowNotificationOnVisualEditor(string message) {
            ShowNotificationOnVisualEditor(new GUIContent(message));
        }
        public static void ShowNotificationOnVisualEditor(GUIContent content) {
            var visualEditor= FindVisualEditor();
            if(visualEditor == null) return;
            visualEditor.ShowNotification(content);
        }
        // ======================================================================
        public static void OpenVisualEditor() {
            if(FindVisualEditor() == null) {
                ShowVisualEditor();
            }
        }
        public static void OpenLibraryEditor() {
            if(FindLibraryEditor() == null) {
                ShowLibraryEditor();
            }
        }
    }
}