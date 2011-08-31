using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This class is used to edit AP_Graph components.
[CustomEditor (typeof (AP_Graph))]
public class AP_Inspector : Editor {
    // ======================================================================
    // PROPERTIES
	// ----------------------------------------------------------------------
    private AP_Graph    Graph= null;
	private AP_Editor	Editor= null;
	public  AP_Object   SelectedObject {
	    get { return mySelectedObject; }
	    set { mySelectedObject= value; Repaint(); }
	}
	private AP_Object mySelectedObject= null;

	// ----------------------------------------------------------------------
    // Display state properties.
	private bool    myFold= true;
	private bool    myGuiSkinErrorSeen= false;
    private bool    showInputs= false;
    private bool    showOutputs= false;
    

	// ----------------------------------------------------------------------
    // Bring up the graph editor window when the inspector is activated.
	public void OnEnable ()
	{
        // The state of the inspector is non-persistant.
        hideFlags= HideFlags.DontSave;
        
        // Get graph reference.
        Graph= target as AP_Graph;
        Graph.Init();
        
        // Configure node appearance;
        if(Graph.GuiSkin == null) {
            GUISkin inspectorSkin= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector) as GUISkin;
            string skinPath= AP_EditorConfig.GuiAssetPath + "/AP_GUISkin.GUISkin";
            Graph.GuiSkin= AssetDatabase.LoadAssetAtPath(skinPath, typeof(GUISkin)) as GUISkin;
            if(Graph.GuiSkin != null) {
                Graph.GuiSkin.font= inspectorSkin.font;                
            }
            else {
                Graph.GuiSkin= inspectorSkin;
                AP_Graphics.ResourceMissingError(skinPath, ref myGuiSkinErrorSeen);
            }
        }
        
		// Create the graph editor.
        if(Editor == null) {
            Editor= AP_Editor.GetWindow();
            DontDestroyOnLoad(Editor);
            Editor.hideFlags= HideFlags.DontSave;            
        }

        // Configure the editor with the selected graph.
        Editor.Activate(Graph.RootNode, this);
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
        // Restore inspector skin.
        GUI.skin= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector) as GUISkin;
        
        // Draw inspector window
		DrawDefaultInspector();
		
        // Draw selected object.
		myFold= EditorGUILayout.InspectorTitlebar(myFold, SelectedObject);
		if(myFold && SelectedObject != null) {
            string name= SelectedObject.name;
            if(name == null || name == "") name= "(empty)";
            name= EditorGUILayout.TextField("Name", name);
            if(name != "(empty)" && SelectedObject.name != name) {
                SelectedObject.name= name;
                SelectedObject.Case<AP_Node, AP_Port>(
                    (node)   => { node.Layout(); },
                    (port)   => { port.Parent.Layout(); },
                    (unknown)=> { Debug.Log("Unknown type"); }
                );
            }
            EditorGUILayout.LabelField("Type", SelectedObject.GetType().Name.Substring(AP_EditorConfig.TypePrefix.Length));
            EditorGUILayout.Toggle("Is Valid", SelectedObject.IsValid);
            SelectedObject.Case<AP_Node, AP_Port>(
                (node)   => { InspectNode(node); },
                (port)   => { InspectPort(port); },
                (unknown)=> { Debug.Log("Unknown type"); }
            );
		}
	}

	// ----------------------------------------------------------------------
    // Inspects the selected node.
    void InspectNode(AP_Node _node) {
        // Show inputs.
        int inCount= 0;
        int outCount= 0;
        _node.ForEachChild<AP_DataPort>(
            (port)=> {
                if(port.IsInput) ++inCount;
                if(port.IsOutput) ++outCount;
            }
        );
        if(inCount > 0) {
            showInputs= EditorGUILayout.Foldout(showInputs, "Inputs");
            if(showInputs) {
                EditorGUIUtility.LookLikeControls();
                _node.ForEachChild<AP_DataPort>(
                    (port)=> {
                        if(port.IsInput) AP_GuiUtilities.OnInspectorGUI(port);
                    }
                );
            }        
        }

        // Show outputs
        if(outCount > 0) {
            showOutputs= EditorGUILayout.Foldout(showOutputs, "Outputs");
            if(showOutputs) {
                _node.ForEachChild<AP_DataPort>(
                    (port)=> {
                            if(port.IsOutput) AP_GuiUtilities.OnInspectorGUI(port);
                    }
                );
            }            
        }
    }

	// ----------------------------------------------------------------------
    // Inspects the selected port.
    void InspectPort(AP_Port port) {
        if(port is AP_DataPort) {
            AP_GuiUtilities.OnInspectorGUI(port as AP_DataPort);            
        }
    }


}
