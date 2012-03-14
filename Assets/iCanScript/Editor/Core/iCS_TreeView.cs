using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_TreeView : EditorWindow {
    // =================================================================================
    // Types
    // ---------------------------------------------------------------------------------
	iCS_ITreeViewItem	myRoot          = null;
	Vector2				myScrollPosition= Vector2.zero;
	
    // ---------------------------------------------------------------------------------
    void OnEnable() {
    }
    // ---------------------------------------------------------------------------------
    void OnDisable() {
    }
    // ---------------------------------------------------------------------------------
    void OnGUI() {
		if(myRoot == null) return;

		int level= 0;
		myScrollPosition= EditorGUILayout.BeginScrollView(myScrollPosition);
		DisplayItem(myRoot);
		EditorGUILayout.EndScrollView();
	}
    // ---------------------------------------------------------------------------------
	void DisplayItem(iCS_ITreeViewItem item) {
		if(!item.HasChildren()) {
			EditorGUILayout.SelectableLabel(item.ToString());
		} else {
			EditorGUILayout.Foldout(false, item.ToString());
		}
	}
}
