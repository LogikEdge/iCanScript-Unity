using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This class is used to edit WD_Behaviour components.
[CustomEditor (typeof (WD_Storage))]
public class WD_Inspector : Editor {
    // ======================================================================
    // PROPERTIES
	// ----------------------------------------------------------------------
    private WD_IStorage     Storage= null;
	private WD_Editor	    Editor = null;
	public  WD_EditorObject SelectedObject {
	    get { return mySelectedObject; }
	    set { mySelectedObject= value; Repaint(); }
	}
	private WD_EditorObject mySelectedObject= null;

	// ----------------------------------------------------------------------
    // Display state properties.
	private bool    selectedObjectFold= true;
    private bool    showInputs        = false;
    private bool    showOutputs       = false;
    

	// ----------------------------------------------------------------------
    // Bring up the graph editor window when the inspector is activated.
	public void OnEnable ()
	{
        // Inspect the assemblies for WarpDrive components.
        WD_Reflection.ParseAppDomain();
        
        // The state of the inspector is non-persistant.
        hideFlags= HideFlags.DontSave;
        
        // Get graph reference.
        Storage= new WD_IStorage(target as WD_Storage);
        
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
        
        // Restore inspector skin.
        GUI.skin= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector) as GUISkin;
        
        // Draw inspector window
		DrawDefaultInspector();
		
        // Draw selected object.
        EditorGUI.indentLevel= 0;
        if(SelectedObject != null) {
            selectedObjectFold= EditorGUILayout.Foldout(selectedObjectFold, "Selected Object");
            if(selectedObjectFold) {
                EditorGUI.indentLevel= 1;
                EditorGUILayout.LabelField("Type", SelectedObject.TypeName);
                string name= SelectedObject.Name;
                if(name == null || name == "") name= "(empty)";
                if(SelectedObject.IsNameEditable) {
                    name= EditorGUILayout.TextField("Name", name);
                    if(name != "(empty)" && name != SelectedObject.Name) {
                        SelectedObject.Name= name;
                        Storage.SetDirty(SelectedObject);
                    }                    
                } else {
                    EditorGUILayout.LabelField("Name", name);                    
                }
                // Show inspector specific for each type of component.
                if(SelectedObject.IsNode)      InspectNode(SelectedObject);
                else if(SelectedObject.IsPort) InspectPort(SelectedObject);
            }            
        }
	}

	// ----------------------------------------------------------------------
    // Inspects the selected node.
    void InspectNode(WD_EditorObject node) {
        List<WD_EditorObject> inPorts= new List<WD_EditorObject>();
        List<WD_EditorObject> outPorts= new List<WD_EditorObject>();
        Storage.ForEachChild(node,
            child=> {
                if(child.IsInDataPort)  inPorts.Add(child);
                if(child.IsOutDataPort) outPorts.Add(child);
            }
        );

        // Show inputs.
        if(inPorts.Count > 0) {
            EditorGUI.indentLevel= 1;
            showInputs= EditorGUILayout.Foldout(showInputs, "Inputs");
            if(showInputs) {
                EditorGUIUtility.LookLikeControls();
                EditorGUI.indentLevel= 2;
                Prelude.forEach(port=> WD_GuiUtilities.OnInspectorGUI(port), inPorts);
            }        
        }

        // Show outputs
        if(outPorts.Count > 0) {
            EditorGUI.indentLevel= 1;
            showOutputs= EditorGUILayout.Foldout(showOutputs, "Outputs");
            if(showOutputs) {
                EditorGUI.indentLevel= 2;
                Prelude.forEach(port=> WD_GuiUtilities.OnInspectorGUI(port), outPorts);
            }            
        }
    }

	// ----------------------------------------------------------------------
    // Inspects the selected port.
    void InspectPort(WD_EditorObject port) {
//        if(port is WD_FieldPort) {
//            WD_GuiUtilities.OnInspectorGUI(port as WD_FieldPort);            
//        }
    }


}
