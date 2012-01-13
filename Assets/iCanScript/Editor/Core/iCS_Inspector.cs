using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This class is used to edit iCS_Behaviour components.
public class iCS_Inspector : Editor {
    // ======================================================================
    // Constants.
	// ----------------------------------------------------------------------
    const string EmptyStr= "(empty)";
    
    // ======================================================================
    // PROPERTIES
	// ----------------------------------------------------------------------
    private iCS_IStorage     Storage= null;
	private iCS_Editor	     Editor = null;
	public  iCS_EditorObject SelectedObject {
	    get { return mySelectedObject; }
	    set {
            if(mySelectedObject != value) {
                FoldoutDB.Clear();
    	        mySelectedObject= value;
            }
	        Repaint();
	    }
	}
	private iCS_EditorObject mySelectedObject= null;
	private Dictionary<string,bool> FoldoutDB= new Dictionary<string,bool>();
	
	// ----------------------------------------------------------------------
    // Display state properties.
	private bool    selectedObjectFold= true;
    private bool    showInputs        = false;
    private bool    showOutputs       = false;
    
	// ----------------------------------------------------------------------
    // Bring up the graph editor window when the inspector is activated.
	public void OnEnable ()
	{
        // The state of the inspector is non-persistant.
        hideFlags= HideFlags.DontSave;
        
        // Create the editor.
        ActivateEditor();
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
    // Bring up the graph editor window when the inspector is activated.
    void ActivateEditor() {
		// Create the graph editor.
        if(Editor == null) {
            Editor= EditorWindow.GetWindow(typeof(iCS_Editor), false, "iCanScript") as iCS_Editor;
            DontDestroyOnLoad(Editor);
            Editor.hideFlags= HideFlags.DontSave;            
        }

        // Configure the editor with the selected graph.
        iCS_Storage realStorage= target as iCS_Storage;
        if(Editor.Storage == null || Editor.Storage.Storage != realStorage) {
            Editor.Deactivate();
            mySelectedObject= null; 
            Storage= new iCS_IStorage(realStorage);        
            Editor.Activate(Storage, this);                        
            Editor.SetInspector(this);
        } else {
            Storage= Editor.Storage;
            Editor.SetInspector(this);
        }        
    }
    
	// ----------------------------------------------------------------------
    // Paint to inspector for the selected object (see editor).
	public override void OnInspectorGUI ()
	{
        if(Storage == null) return;
        
        // Make certain our editor is active.
        ActivateEditor();
        
        // Restore inspector skin.
        GUI.skin= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector) as GUISkin;
        
        // Draw inspector window
        EditorGUI.indentLevel= 0;
		DrawDefaultInspector();

        // Frame count.
		if(Storage.Storage is iCS_Behaviour && Application.isPlaying) {
		    iCS_Behaviour behaviour= Storage.Storage as iCS_Behaviour;
		    EditorGUILayout.LabelField("UpdateFrameId", behaviour.UpdateFrameId.ToString());
		    EditorGUILayout.LabelField("FixedUpdateFrameId", behaviour.FixedUpdateFrameId.ToString());
		}
		
        // Draw selected object.
        EditorGUI.indentLevel= 0;
        if(SelectedObject != null && SelectedObject.IsValid) {
            selectedObjectFold= EditorGUILayout.Foldout(selectedObjectFold, "Selected Object");
            if(selectedObjectFold) {
                // Display object name.
                EditorGUI.indentLevel= 1;
                string name= SelectedObject.RawName;
                if(SelectedObject.IsOutStatePort) name= Storage.FindAConnectedPort(SelectedObject).RawName;
                if(name == null || name == "") name= EmptyStr;
                if(SelectedObject.IsNameEditable) {
                    name= EditorGUILayout.TextField("Name", name);
                    if(name != EmptyStr && name != SelectedObject.RawName) {
                        SelectedObject.Name= name;
                        if(SelectedObject.IsNode) {
                            iCS_RuntimeDesc rtDesc= new iCS_RuntimeDesc(SelectedObject.RuntimeArchive);
                            rtDesc.DisplayName= name;
                            SelectedObject.RuntimeArchive= rtDesc.Encode(rtDesc.Id);
                        }
                        if(SelectedObject.IsStatePort) {
                            if(SelectedObject.IsOutStatePort) Storage.FindAConnectedPort(SelectedObject).Name= name;
                            else Storage.GetSource(SelectedObject).Name= name;                            
                        }
                        Storage.SetDirty(SelectedObject);
                    }                    
                } else {
                    EditorGUILayout.LabelField("Name", name);                    
                }
                // Show object tooltip.
                string toolTip= SelectedObject.RawToolTip;
                if(SelectedObject.IsOutStatePort) toolTip= Storage.FindAConnectedPort(SelectedObject).RawToolTip;
                if(toolTip == null || toolTip == "") toolTip= EmptyStr;
                toolTip= EditorGUILayout.TextField("Tool Tip", toolTip);
                if(toolTip != EmptyStr && toolTip != SelectedObject.RawToolTip) {
                    SelectedObject.ToolTip= toolTip;
                    if(SelectedObject.IsStatePort) {
                        if(SelectedObject.IsOutStatePort) Storage.FindAConnectedPort(SelectedObject).ToolTip= toolTip;
                        else Storage.GetSource(SelectedObject).ToolTip= toolTip;
                    }
                }
                // Show inspector specific for each type of component.
                if(SelectedObject.IsNode)      InspectNode(SelectedObject);
                else if(SelectedObject.IsPort) InspectPort(SelectedObject);
            }            
        }
	}

	// ----------------------------------------------------------------------
    void InspectNode(iCS_EditorObject node) {
        // Show runtime frame id.
        iCS_Function runtimeObject= Storage.GetRuntimeObject(node) as iCS_Function;
        if(runtimeObject != null) {
            EditorGUILayout.LabelField("FrameId", runtimeObject.FrameId.ToString());
        }
        // Show Iconic image configuration.
        Texture2D iconicTexture= iCS_Graphics.GetCachedIconFromGUID(node.IconGUID);
        Object newTexture= EditorGUILayout.ObjectField("Iconic Texture", iconicTexture, typeof(Texture2D), false) as Texture2D;
        if(newTexture != iconicTexture) {
            node.IconGUID= newTexture != null ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newTexture)) : null;
        }
        // Display specific node type information.
        switch(node.ObjectType) {
            case iCS_ObjectTypeEnum.State:
                InspectStateNode(node);
                break;
            default:
                InspectDataProcessingNode(node);
                break;
        }
    }
    
	// ----------------------------------------------------------------------
    // Inspects data processing node.
    void InspectDataProcessingNode(iCS_EditorObject node) {
        // Collect data ports.
        List<iCS_EditorObject> inPorts= new List<iCS_EditorObject>();
        List<iCS_EditorObject> outPorts= new List<iCS_EditorObject>();
        Storage.ForEachChild(node,
            child=> {
                if(child.IsInDataPort)  inPorts.Add(child);
                if(child.IsOutDataPort) outPorts.Add(child);
            }
        );

        // Show inputs.
        iCS_FunctionBase runtimeObject= Storage.GetRuntimeObject(node) as iCS_FunctionBase;
        if(inPorts.Count > 0) {
            int indentLevel= 1;
            if(runtimeObject != null) {
                EditorGUI.indentLevel= indentLevel;
                showInputs= EditorGUILayout.Foldout(showInputs, "Inputs");                
                ++indentLevel;
            } else {
                showInputs= true;
            }
            if(showInputs) {
                EditorGUIUtility.LookLikeControls();
                Prelude.forEach(port=> iCS_GuiUtilities.OnInspectorDataPortGUI(port, Storage, indentLevel, FoldoutDB), inPorts);
            }        
        }

        // Show outputs
        if(outPorts.Count > 0 && runtimeObject != null) {
            EditorGUI.indentLevel= 1;
            showOutputs= EditorGUILayout.Foldout(showOutputs, "Outputs");
            if(showOutputs) {
                Prelude.forEach(port=> iCS_GuiUtilities.OnInspectorDataPortGUI(port, Storage, 2, FoldoutDB), outPorts);
            }            
        }
    }

	// ----------------------------------------------------------------------
    // Inspect state node.
    void InspectStateNode(iCS_EditorObject node) {
        // Collect transitions.
        List<iCS_EditorObject> inPorts= new List<iCS_EditorObject>();
        List<iCS_EditorObject> outPorts= new List<iCS_EditorObject>();
        Storage.ForEachChild(node,
            child=> {
                if(child.IsInStatePort)  inPorts.Add(child);
                if(child.IsOutStatePort) outPorts.Add(child);
            }
        );
        
        // Show outbound transitions.
        if(outPorts.Count > 0) {
            EditorGUI.indentLevel= 1;
            showOutputs= EditorGUILayout.Foldout(showOutputs, "Outbound Transitions");
            if(showOutputs) {
                EditorGUI.indentLevel= 2;
                foreach(var port in outPorts) {
                    iCS_EditorObject inPort= Storage.FindAConnectedPort(port);
                    EditorGUILayout.LabelField("Name", inPort.Name);                        
                    EditorGUILayout.LabelField("State", Storage.GetParent(inPort).Name);                    
                }
            }
        }
        // Show inbound transitions.
        if(inPorts.Count > 0) {
            EditorGUI.indentLevel= 1;
            showInputs= EditorGUILayout.Foldout(showInputs, "Inbound Transitions");
            if(showInputs) {
                EditorGUI.indentLevel= 2;
                foreach(var port in inPorts) {
                    EditorGUILayout.LabelField("Name", port.Name);                        
                    iCS_EditorObject outPort= Storage.GetSource(port);
                    EditorGUILayout.LabelField("State", Storage.GetParent(outPort).Name);                    
                }
            }
        }
    }
    
	// ----------------------------------------------------------------------
    // Inspects the selected port.
    void InspectPort(iCS_EditorObject port) {
        iCS_EditorObject parent= Storage.GetParent(port);
        EditorGUILayout.LabelField("Parent", parent.Name);
        string inOut= port.IsInputPort ? (port.IsEnablePort ? "enable":"in") : "out";
        EditorGUILayout.LabelField("Direction", inOut);        
    }


}
