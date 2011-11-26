using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This class is used to edit WD_Behaviour components.
public class WD_Inspector : Editor {
    // ======================================================================
    // Constants.
	// ----------------------------------------------------------------------
    const string EmptyStr= "(empty)";
    
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
        // Inspect the assemblies for uCode components.
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
		if(Storage.Storage is WD_Behaviour) {
		    WD_Behaviour behaviour= Storage.Storage as WD_Behaviour;
		    EditorGUILayout.LabelField("UpdateFrameId", behaviour.UpdateFrameId.ToString());
		    EditorGUILayout.LabelField("FixedUpdateFrameId", behaviour.FixedUpdateFrameId.ToString());
		}
		
        // Draw selected object.
        EditorGUI.indentLevel= 0;
        if(SelectedObject != null && SelectedObject.IsValid) {
            selectedObjectFold= EditorGUILayout.Foldout(selectedObjectFold, "Selected Object");
            if(selectedObjectFold) {
                // Display type information.
                EditorGUI.indentLevel= 1;
                EditorGUILayout.LabelField("Type", SelectedObject.TypeName);
                // Display object name.
                string name= SelectedObject.RawName;
                if(SelectedObject.IsOutStatePort) name= Storage.FindAConnectedPort(SelectedObject).RawName;
                if(name == null || name == "") name= EmptyStr;
                if(SelectedObject.IsNameEditable) {
                    name= EditorGUILayout.TextField("Name", name);
                    if(name != EmptyStr && name != SelectedObject.RawName) {
                        SelectedObject.Name= name;
                        if(SelectedObject.IsNode) {
                            WD_RuntimeDesc rtDesc= new WD_RuntimeDesc(SelectedObject.RuntimeArchive);
                            rtDesc.Name= name;
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
    void InspectNode(WD_EditorObject node) {
        // Show runtime frame id.
        WD_Function runtimeObject= Storage.GetRuntimeObject(node) as WD_Function;
        if(runtimeObject != null) {
            EditorGUILayout.LabelField("FrameId", runtimeObject.FrameId.ToString());
        }
        // Show Iconic image configuration.
        Texture2D iconicTexture= WD_Graphics.GetCachedIconFromGUID(node.IconGUID);
        Object newTexture= EditorGUILayout.ObjectField("Iconic Texture", iconicTexture, typeof(Texture2D), false) as Texture2D;
        if(newTexture != iconicTexture) {
            node.IconGUID= newTexture != null ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newTexture)) : null;
        }
        // Display specific node type information.
        switch(node.ObjectType) {
            case WD_ObjectTypeEnum.State:
                InspectStateNode(node);
                break;
            default:
                InspectDataProcessingNode(node);
                break;
        }
    }
    
	// ----------------------------------------------------------------------
    // Inspects data processing node.
    void InspectDataProcessingNode(WD_EditorObject node) {
        // Collect data ports.
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
                Prelude.forEach(port=> WD_GuiUtilities.OnInspectorGUI(port, Storage), inPorts);
            }        
        }

        // Show outputs
        if(outPorts.Count > 0) {
            EditorGUI.indentLevel= 1;
            showOutputs= EditorGUILayout.Foldout(showOutputs, "Outputs");
            if(showOutputs) {
                EditorGUI.indentLevel= 2;
                Prelude.forEach(port=> WD_GuiUtilities.OnInspectorGUI(port, Storage), outPorts);
            }            
        }
    }

	// ----------------------------------------------------------------------
    // Inspect state node.
    void InspectStateNode(WD_EditorObject node) {
        // Collect transitions.
        List<WD_EditorObject> inPorts= new List<WD_EditorObject>();
        List<WD_EditorObject> outPorts= new List<WD_EditorObject>();
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
                    WD_EditorObject inPort= Storage.FindAConnectedPort(port);
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
                    WD_EditorObject outPort= Storage.GetSource(port);
                    EditorGUILayout.LabelField("State", Storage.GetParent(outPort).Name);                    
                }
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
