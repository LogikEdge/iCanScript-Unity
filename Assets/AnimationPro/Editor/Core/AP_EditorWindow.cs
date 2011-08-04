using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class GAP_EditorWindow : EditorWindow {
	// ----------------------------------------------------------------------
    protected GAP_EditorWindow Init(List<EditorWindow> _windowList, string _windowTitle, Rect _windowPosition, Vector2 _graphPosition) {
        title= _windowTitle;
        position= _windowPosition;
        myGraphPosition= _graphPosition;
        _windowList.Add(this);
        return this;
    }
	// ----------------------------------------------------------------------
    public void Dealloc() {
#if UNITY_EDITOR
        DestroyImmediate(this);
#else
        Destroy(this);
#endif        
    }
    
    // ======================================================================
    // PROPERTIES
	// ----------------------------------------------------------------------
    protected Vector2 myGraphPosition= Vector2.zero;
 }
