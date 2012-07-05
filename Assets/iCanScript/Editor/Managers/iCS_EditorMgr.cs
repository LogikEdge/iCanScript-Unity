using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public static class iCS_EditorMgr {
    // =================================================================================
    // Types
    // ---------------------------------------------------------------------------------
    class EditorInfo {
        public string           Key                   = null;
        public EditorWindow     Window                = null;
        public iCS_EditorBase   Editor                = null;
        public EditorInfo(string key, EditorWindow window, iCS_EditorBase editor) {
            Key= key;
            Window= window;
            Editor= editor;
        }
    }
    
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    static List<EditorInfo>   myEditors       = null;
    
    // =================================================================================
    // Initialization
    // ---------------------------------------------------------------------------------
    static iCS_EditorMgr() {
        myEditors= new List<EditorInfo>();
    }
    
    // =================================================================================
    // Window management
    // ---------------------------------------------------------------------------------
    public static void Add(string key, EditorWindow window, System.Object editor) {
        myEditors.Add(new EditorInfo(key, window, editor as iCS_EditorBase));
    }
    public static void Remove(string key) {
        int idx= FindIndexOf(key);
        if(idx >= 0) myEditors.RemoveAt(idx);
    }
    
    // =================================================================================
    // Event distribution.
    // ---------------------------------------------------------------------------------
	public static void Update() {
		iCS_StorageMgr.Update();
	}
	
    // ======================================================================
    // Search/Iterations
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
    public static void ShowGraphEditor() {
        EditorApplication.ExecuteMenuItem("Window/iCanScript/Graph");
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
    public static EditorWindow FindGraphEditorWindow() {
        return FindWindow<iCS_GraphEditor>();
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
    public static iCS_GraphEditor FindGraphEditor() {
        int idx= FindIndexOf(typeof(iCS_GraphEditor).Name);
        return idx >= 0 ? myEditors[idx].Editor as iCS_GraphEditor : null;
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
}
