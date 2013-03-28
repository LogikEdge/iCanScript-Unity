using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public static class iCS_EditorMgr {
    // =================================================================================
    // Types
    // ---------------------------------------------------------------------------------
    class EditorInfo {
        public string           Key   = null;
        public EditorWindow     Window= null;
        public iCS_EditorBase   Editor= null;
        public EditorInfo(string key, EditorWindow window, iCS_EditorBase editor) {
            Key= key;
            Window= window;
            Editor= editor;
        }
    }
    
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    static List<EditorInfo>   myEditors= null;
    
    // =================================================================================
    // Initialization
    // ---------------------------------------------------------------------------------
    static iCS_EditorMgr() {
        myEditors= new List<EditorInfo>();
    }
    
    // =================================================================================
    // Dispatcher
    // ---------------------------------------------------------------------------------
    public static void OnEnable(EditorWindow window) {
        Add(window);
        var editor= FindEditor(window);
        if(editor == null) return;
        editor.OnEnable(window);
    }
    public static void OnDisable(EditorWindow window) {
        var editor= FindEditor(window);
        if(editor == null) return;
        editor.OnDisable(window);
        Remove(window);
    }
    public static void OnGUI(EditorWindow window) {
        var editor= FindEditor(window);
        if(editor == null) return;
        editor.OnGUI();        
    }
    public static void OnSelectionChange(EditorWindow window) {
        var editor= FindEditor(window);
        if(editor == null) return;
        editor.OnSelectionChange();        
    }
    public static void Update(EditorWindow window) {
        var editor= FindEditor(window);
        if(editor == null) return;
        editor.Update();                
    }
    
    // =================================================================================
    // Window management
    // ---------------------------------------------------------------------------------
    // FIXME: The relationship between proxy type and editor name needs to be cleaned up.
    public static void Add(EditorWindow window) {
        var windowTypeName= window.GetType().Name;
        iCS_EditorBase editor= null;
        if(windowTypeName == "iCS_PreferencesProxy") {
            editor= new iCS_PreferencesEditor();
        } else if(windowTypeName == "iCS_HierarchyEditorProxy") {
            editor= new iCS_HierarchyEditor();
        } else if(windowTypeName == "iCS_InstanceEditorProxy") {
            editor= new iCS_InstanceEditor();
        } else if(windowTypeName == "iCS_LibraryEditorProxy") {
            editor= new iCS_LibraryEditor();
        } else if(windowTypeName == "iCS_VisualEditorProxy") {
            editor= new iCS_VisualEditor();
        }
        if(editor == null) {
            Debug.LogWarning("iCanScript: Unable to create editor for: "+windowTypeName);
            return;
        }
        myEditors.Add(new EditorInfo(editor.GetType().Name, window, editor));
    }
    public static void Remove(EditorWindow window) {
        int idx= FindIndexOf(window);
        if(idx >= 0) myEditors.RemoveAt(idx);
    }
    
    // =================================================================================
    // Event distribution.
    // ---------------------------------------------------------------------------------
	public static void Update() {
		iCS_StorageMgr.Update();
	}
	
    // =================================================================================
    // Search/Iterations
    // ---------------------------------------------------------------------------------
    static iCS_EditorBase FindEditor(EditorWindow window) {
        var editorInfo= FindEditorInfo(window);
        return editorInfo != null ? editorInfo.Editor : null;
    }
    // ---------------------------------------------------------------------------------
    static EditorInfo FindEditorInfo(EditorWindow window) {
        int idx= FindIndexOf(window);
        return idx >= 0 ? myEditors[idx] : null;
    }
    // ---------------------------------------------------------------------------------
    static EditorInfo FindEditorInfo(string key) {
        int idx= FindIndexOf(key);
        return idx >= 0 ? myEditors[idx] : null;        
    }
    // ---------------------------------------------------------------------------------
    static int FindIndexOf(EditorWindow window) {
        for(int i= 0; i < myEditors.Count; ++i) {
            if(myEditors[i].Window == window) {
                return i;
            }
        }        
        return -1;
    }
    // ---------------------------------------------------------------------------------
    static int FindIndexOf(string key) {
        for(int i= 0; i < myEditors.Count; ++i) {
            if(myEditors[i].Key == key) {
                return i;
            }
        }        
        return -1;
    }
    
    // ======================================================================
    public static void ShowVisualEditor() {
        EditorApplication.ExecuteMenuItem("Window/iCanScript/Visual Editor");
    }
    public static void ShowInstanceEditor() {
        EditorApplication.ExecuteMenuItem("Window/iCanScript/Instance Wizard");        
    }
    public static void ShowHierarchyEditor() {
        EditorApplication.ExecuteMenuItem("Window/iCanScript/Hierarchy");                
    }
    public static void ShowLibraryEditor() {
        EditorApplication.ExecuteMenuItem("Window/iCanScript/Library");                
    }
    public static void ShowPreferences() {
        EditorApplication.ExecuteMenuItem("Window/iCanScript/Preferences");                
    }

    // ======================================================================
    public static EditorWindow FindWindow(string key) {
        int idx= FindIndexOf(key);
        return idx >= 0 ? myEditors[idx].Window : null;        
    }
    public static EditorWindow FindWindow(Type type) {
        return FindWindow(type.Name);
    }
    public static EditorWindow FindWindow<T>() {
        return FindWindow(typeof(T));
    }
    public static EditorWindow FindVisualEditorWindow() {
        return FindWindow<iCS_VisualEditor>();
    } 
    public static EditorWindow FindInstanceEditorWindow() {
        return FindWindow<iCS_InstanceEditor>();
    }
    public static EditorWindow FindHierarchyEditorWindow() {
        return FindWindow<iCS_HierarchyEditor>();
    }
    public static EditorWindow FindLibraryEditorWindow() {
        return FindWindow<iCS_LibraryEditor>();
    }    
    // ======================================================================
    public static iCS_VisualEditor FindVisualEditor() {
        int idx= FindIndexOf(typeof(iCS_VisualEditor).Name);
        return idx >= 0 ? myEditors[idx].Editor as iCS_VisualEditor : null;
    } 
    public static iCS_InstanceEditor FindInstanceEditor() {
        int idx= FindIndexOf(typeof(iCS_InstanceEditor).Name);
        return idx >= 0 ? myEditors[idx].Editor as iCS_InstanceEditor : null;
    }
    public static iCS_HierarchyEditor FindHierarchyEditor() {
        int idx= FindIndexOf(typeof(iCS_HierarchyEditor).Name);
        return idx >= 0 ? myEditors[idx].Editor as iCS_HierarchyEditor : null;
    }
    public static iCS_LibraryEditor FindLibraryEditor() {
        int idx= FindIndexOf(typeof(iCS_LibraryEditor).Name);
        return idx >= 0 ? myEditors[idx].Editor as iCS_LibraryEditor : null;
    }    
    // ======================================================================
	public static void RepaintVisualEditor() {
		var editor= FindVisualEditor();
		editor.MyWindow.Repaint();
	}
}
