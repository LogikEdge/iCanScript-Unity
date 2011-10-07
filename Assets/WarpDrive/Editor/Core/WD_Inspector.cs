using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This class is used to edit WD_Behaviour components.
[CustomEditor (typeof (WD_Behaviour))]
public class WD_Inspector : Editor {
    // ======================================================================
    // PROPERTIES
	// ----------------------------------------------------------------------
    private WD_Storage      Storage= null;
	private WD_Editor	    Editor = null;
	public  WD_EditorObject SelectedObject {
	    get { return mySelectedObject; }
	    set { mySelectedObject= value; Repaint(); }
	}
	private WD_EditorObject mySelectedObject= null;

	// ----------------------------------------------------------------------
    // Display state properties.
	private bool    myFold= true;
    private bool    showInputs= false;
    private bool    showOutputs= false;
    

	// ----------------------------------------------------------------------
    // Bring up the graph editor window when the inspector is activated.
	public void OnEnable ()
	{
        // Inspect the assemblies for WarpDrive components.
        WD_Reflection.ParseAppDomain();
        
        // The state of the inspector is non-persistant.
        hideFlags= HideFlags.DontSave;
        
        // Get graph reference.
        Storage= target as WD_Storage;
        
		// Create the graph editor.
        if(Editor == null) {
            Editor= EditorWindow.GetWindow<WD_Editor>();
            DontDestroyOnLoad(Editor);
            Editor.hideFlags= HideFlags.DontSave;            
        }

        // Configure the editor with the selected graph.
        Editor.Activate(Storage, this);
	}
	
	// ----------------------------------------------------------------------
    // Deactivate the edition of the graph.
	public void OnDisable ()
	{
        // Deactivate the editor.
        if(Editor != null) {
            Editor.Deactivate();
	    }

        // Forget the selected object.
		mySelectedObject= null;
	}
	
	// ----------------------------------------------------------------------
    // Paint to inspector for the selected object (see editor).
	public override void OnInspectorGUI ()
	{
        if(Storage == null) return;
        WD_EditorObjectMgr editorObjects= Storage.EditorObjects;
        
        // Restore inspector skin.
        GUI.skin= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector) as GUISkin;
        
        // Draw inspector window
		DrawDefaultInspector();
		
        // Draw selected object.
        WD_Object rtSelectedObject= SelectedObject != null ? editorObjects.GetRuntimeObject(SelectedObject) as WD_Object: null;
		myFold= EditorGUILayout.InspectorTitlebar(myFold, rtSelectedObject);
		if(myFold && rtSelectedObject != null) {
            string name= rtSelectedObject.name;
            if(name == null || name == "") name= "(empty)";
            name= EditorGUILayout.TextField("Name", name);
            if(name != "(empty)" && rtSelectedObject.name != name) {
                rtSelectedObject.name= SelectedObject.Name= name;
                rtSelectedObject.Case<WD_Node, WD_Port>(
                    (node)   => { editorObjects.Layout(SelectedObject); },
                    (port)   => { editorObjects.Layout(SelectedObject.ParentId); },
                    (unknown)=> { Debug.Log("Unknown type"); }
                );
            }
            EditorGUILayout.LabelField("Type", rtSelectedObject.GetType().Name.Substring(WD_EditorConfig.TypePrefix.Length));
            EditorGUILayout.Toggle("Is Valid", rtSelectedObject.IsValid);
            rtSelectedObject.Case<WD_Node, WD_Port>(
                (node)   => { InspectNode(node); },
                (port)   => { InspectPort(port); },
                (unknown)=> { Debug.Log("Unknown type"); }
            );
		}
	}

	// ----------------------------------------------------------------------
    // Inspects the selected node.
    void InspectNode(WD_Node _node) {
        // Show inputs.
        int inCount= 0;
        int outCount= 0;
        _node.ForEachChild<WD_FieldPort>(
            (port)=> {
                if(port.IsInput) ++inCount;
                if(port.IsOutput) ++outCount;
            }
        );
        if(inCount > 0) {
            showInputs= EditorGUILayout.Foldout(showInputs, "Inputs");
            if(showInputs) {
                EditorGUIUtility.LookLikeControls();
                _node.ForEachChild<WD_FieldPort>(
                    (port)=> {
                        if(port.IsInput) WD_GuiUtilities.OnInspectorGUI(port);
                    }
                );
            }        
        }

        // Show outputs
        if(outCount > 0) {
            showOutputs= EditorGUILayout.Foldout(showOutputs, "Outputs");
            if(showOutputs) {
                _node.ForEachChild<WD_FieldPort>(
                    (port)=> {
                            if(port.IsOutput) WD_GuiUtilities.OnInspectorGUI(port);
                    }
                );
            }            
        }
    }

	// ----------------------------------------------------------------------
    // Inspects the selected port.
    void InspectPort(WD_Port port) {
        if(port is WD_FieldPort) {
            WD_GuiUtilities.OnInspectorGUI(port as WD_FieldPort);            
        }
    }


}
