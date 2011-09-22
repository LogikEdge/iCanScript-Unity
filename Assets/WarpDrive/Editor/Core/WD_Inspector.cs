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
    private WD_Behaviour    Graph= null;
	private WD_Editor	    Editor= null;
	public  WD_Object       SelectedObject {
	    get { return mySelectedObject; }
	    set { mySelectedObject= value; Repaint(); }
	}
	private WD_Object mySelectedObject= null;

	// ----------------------------------------------------------------------
    // Display state properties.
	private bool    myFold= true;
    private bool    showInputs= false;
    private bool    showOutputs= false;
    

	// ----------------------------------------------------------------------
    // Bring up the graph editor window when the inspector is activated.
	public void OnEnable ()
	{
        // The state of the inspector is non-persistant.
        hideFlags= HideFlags.DontSave;
        
        // Get graph reference.
        Graph= target as WD_Behaviour;
        Graph.Init();
        
		// Create the graph editor.
        if(Editor == null) {
            Editor= EditorWindow.GetWindow<WD_Editor>();
            DontDestroyOnLoad(Editor);
            Editor.hideFlags= HideFlags.DontSave;            
        }

        // Configure the editor with the selected graph.
        Editor.Activate(Graph, this);
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
                SelectedObject.Case<WD_Node, WD_Port>(
                    (node)   => { node.Layout(); },
                    (port)   => { port.Parent.Layout(); },
                    (unknown)=> { Debug.Log("Unknown type"); }
                );
            }
            EditorGUILayout.LabelField("Type", SelectedObject.GetType().Name.Substring(WD_EditorConfig.TypePrefix.Length));
            EditorGUILayout.Toggle("Is Valid", SelectedObject.IsValid);
            SelectedObject.Case<WD_Node, WD_Port>(
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
        _node.ForEachChild<WD_DataPort>(
            (port)=> {
                if(port.IsInput) ++inCount;
                if(port.IsOutput) ++outCount;
            }
        );
        if(inCount > 0) {
            showInputs= EditorGUILayout.Foldout(showInputs, "Inputs");
            if(showInputs) {
                EditorGUIUtility.LookLikeControls();
                _node.ForEachChild<WD_DataPort>(
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
                _node.ForEachChild<WD_DataPort>(
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
        if(port is WD_DataPort) {
            WD_GuiUtilities.OnInspectorGUI(port as WD_DataPort);            
        }
    }


}
