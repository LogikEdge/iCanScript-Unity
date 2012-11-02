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
    // Fields
	// ----------------------------------------------------------------------
    private iCS_IStorage              myStorage             = null;
	private iCS_EditorObject          mySelectedObject      = null;
	private Dictionary<string,object> myFoldoutDB           = new Dictionary<string,object>();
	
	// ----------------------------------------------------------------------
    // Display state properties.
	private bool    mySelectedObjectFold= true;
    private bool    myShowInputs        = false;
    private bool    myShowOutputs       = false;
    
	// ----------------------------------------------------------------------
    // Repaint abort processing variables.
    private bool                myAbortRepaint          = false;
    private iCS_IStorage        myPreviousStorage       = null;
    private int                 myPreviousModificationId= -1;
    private iCS_EditorObject    myPreviousSelectedObject= null;
    private int                 myPreviousPortSource    = -1;
    
    // ======================================================================
    // Properties
	// ----------------------------------------------------------------------
	public  iCS_EditorObject SelectedObject {
	    get {
	        iCS_EditorObject selectedObject= myStorage != null ? myStorage.SelectedObject : null;
	        if(selectedObject != mySelectedObject) {
                myFoldoutDB.Clear();
	            mySelectedObject= selectedObject;
	        }
	        return mySelectedObject;
	    }
	}

	// ----------------------------------------------------------------------
    // Bring up the graph editor window when the inspector is activated.
	public void OnEnable ()
	{
        // The state of the inspector is non-persistant.
        hideFlags= HideFlags.DontSave;
	}
	
	// ----------------------------------------------------------------------
    // Deactivate the edition of the graph.
	public void OnDisable ()
	{
        // Forget the selected object.
		myStorage= null;
		mySelectedObject= null;
	}
	
	// ----------------------------------------------------------------------
    // Bring up the graph editor window when the inspector is activated.
    void UpdateVariables() {
        // Verify that the target reflects the selected storage.
        iCS_EditorMgr.Update();
        iCS_Storage realStorage= target as iCS_Storage;
        if(realStorage == null || realStorage != iCS_StorageMgr.Storage) {
            myStorage= null;
			mySelectedObject= null;
            return;
        }

        // Configure the editor with the selected graph.
		myStorage= iCS_StorageMgr.IStorage;
		mySelectedObject= myStorage.SelectedObject;
    }
    
	// ----------------------------------------------------------------------
    // Paint to inspector for the selected object (see editor).
	public override void OnInspectorGUI ()
	{
        // Update all variables we rely on.
        UpdateVariables();
		
        // Nothing to show if no storage is selected.
        if(myStorage == null) return;
        
        // Restore inspector skin.
        GUI.skin= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector) as GUISkin;
        
        // Draw inspector window
        EditorGUI.indentLevel= 0;
		DrawDefaultInspector();
		EditorGUIUtility.LookLikeControls();

        // Frame count.
		if(myStorage.Storage is iCS_Behaviour && Application.isPlaying) {
		    iCS_Behaviour behaviour= myStorage.Storage as iCS_Behaviour;
		    EditorGUILayout.LabelField("UpdateFrameId", behaviour.UpdateFrameId.ToString());
		    EditorGUILayout.LabelField("FixedUpdateFrameId", behaviour.FixedUpdateFrameId.ToString());
		}
		
        // Stop any repaint if the selected object has changed between the layout and repaint events.
        switch(Event.current.type) {
            case EventType.Layout:
                myAbortRepaint= false;
                if(myPreviousStorage != myStorage) {
                    myAbortRepaint= true;
                    myPreviousStorage= myStorage;
                }
                if(myPreviousModificationId != myStorage.ModificationId) {
                    myAbortRepaint= true;
                    myPreviousModificationId= myStorage.ModificationId;
                }
                if(myPreviousSelectedObject != mySelectedObject) {
                    myAbortRepaint= true;
                    myPreviousSelectedObject= mySelectedObject;
                }
                if(mySelectedObject != null && mySelectedObject.IsDataPort) {
                    if(myPreviousPortSource != mySelectedObject.Source) {
                        myAbortRepaint= true;
                        myPreviousPortSource= mySelectedObject.Source;
                    }
                }
                break;
            case EventType.Repaint:
                if(myAbortRepaint) {
                    Repaint();
                    return;                        
                }
                break;
        }
        
        // Draw selected object.
        EditorGUI.indentLevel= 0;
        if(mySelectedObject != null && SelectedObject.IsValid) {
            mySelectedObjectFold= EditorGUILayout.Foldout(mySelectedObjectFold, "Selected Object");
            if(mySelectedObjectFold) {
                // Display object name.
                EditorGUI.indentLevel= 1;
                string name= SelectedObject.RawName;
                if(mySelectedObject.IsOutStatePort) name= myStorage.FindAConnectedPort(SelectedObject).RawName;
                if(name == null || name == "") name= EmptyStr;
                if(mySelectedObject.IsNameEditable) {
                    name= EditorGUILayout.TextField("Name", name);
                    if(name != EmptyStr && name != SelectedObject.RawName) {
                        SelectedObject.Name= name;
                        if(SelectedObject.IsStatePort) {
                            if(SelectedObject.IsOutStatePort) myStorage.FindAConnectedPort(SelectedObject).Name= name;
                            else myStorage.GetSource(SelectedObject).Name= name;                            
                        }
                        myStorage.SetDirty(SelectedObject);
                    }                    
                } else {
                    EditorGUILayout.LabelField("Name", name);                    
                }
                // Show object tooltip.
                string toolTip= SelectedObject.RawTooltip;
                if(mySelectedObject.IsOutStatePort) toolTip= myStorage.FindAConnectedPort(SelectedObject).RawTooltip;
                if(toolTip == null || toolTip == "") toolTip= EmptyStr;
				GUI.SetNextControlName("tooltip");
                toolTip= EditorGUILayout.TextField("Tool Tip", toolTip);
                if(toolTip != EmptyStr && toolTip != mySelectedObject.RawTooltip) {
                    SelectedObject.Tooltip= toolTip;
                    if(SelectedObject.IsStatePort) {
                        if(SelectedObject.IsOutStatePort) myStorage.FindAConnectedPort(SelectedObject).Tooltip= toolTip;
                        else myStorage.GetSource(SelectedObject).Tooltip= toolTip;
                    }
                }
                // Show inspector specific for each type of component.
                if(SelectedObject.IsNode)      InspectNode(SelectedObject);
                else if(SelectedObject.IsPort) InspectPort(SelectedObject);
            }            
        }

        // Allow repaint for modifications done by the user.
        myPreviousModificationId= myStorage.ModificationId;
	}

	// ----------------------------------------------------------------------
    void InspectNode(iCS_EditorObject node) {
        // Show runtime frame id.
        iCS_Function runtimeObject= myStorage.GetRuntimeObject(node) as iCS_Function;
        if(runtimeObject != null) {
            EditorGUILayout.LabelField("FrameId", runtimeObject.FrameId.ToString());
        }
        // Show Iconic image configuration.
        Texture2D iconicTexture= iCS_TextureCache.GetIconFromGUID(node.IconGUID);
        Object newTexture= EditorGUILayout.ObjectField("Iconic Texture", iconicTexture, typeof(Texture2D), false) as Texture2D;
        if(newTexture != iconicTexture) {
            node.IconGUID= newTexture != null ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newTexture)) : null;
        }
        // Display specific node type information.
        switch(node.ObjectType) {
			case iCS_ObjectTypeEnum.StateChart:
				break;
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
        myStorage.ForEachChild(node,
            child=> {
                if(child.IsInDataPort)  inPorts.Add(child);
                if(child.IsOutDataPort) outPorts.Add(child);
            }
        );

        // Show inputs.
        iCS_IParams runtimeObject= myStorage.GetRuntimeObject(node) as iCS_IParams;
        if(inPorts.Count > 0) {
            int indentLevel= 1;
            if(runtimeObject != null) {
                EditorGUI.indentLevel= indentLevel;
                myShowInputs= EditorGUILayout.Foldout(myShowInputs, "Inputs");                
                ++indentLevel;
            } else {
                myShowInputs= true;
            }
            if(myShowInputs) {
                EditorGUIUtility.LookLikeControls();
                Prelude.forEach(port=> iCS_GuiUtilities.OnInspectorDataPortGUI(port, myStorage, indentLevel, myFoldoutDB), inPorts);
            }        
        }

        // Show outputs
        if(outPorts.Count > 0 && runtimeObject != null) {
            EditorGUI.indentLevel= 1;
            myShowOutputs= EditorGUILayout.Foldout(myShowOutputs, "Outputs");
            if(myShowOutputs) {
                Prelude.forEach(port=> iCS_GuiUtilities.OnInspectorDataPortGUI(port, myStorage, 2, myFoldoutDB), outPorts);
            }            
        }
    }

	// ----------------------------------------------------------------------
    // Inspect state node.
    void InspectStateNode(iCS_EditorObject node) {
        // Collect transitions.
        iCS_EditorObject[] dataPorts= myStorage.GetSortedChildDataPorts(node);
        List<iCS_EditorObject> inPorts= new List<iCS_EditorObject>();
        List<iCS_EditorObject> outPorts= new List<iCS_EditorObject>();
        foreach(var child in dataPorts) {
            if(child.IsInStatePort)  inPorts.Add(child);
            if(child.IsOutStatePort) outPorts.Add(child);
        }
        
        // Show outbound transitions.
        if(outPorts.Count > 0) {
            EditorGUI.indentLevel= 1;
            myShowOutputs= EditorGUILayout.Foldout(myShowOutputs, "Outbound Transitions");
            if(myShowOutputs) {
                EditorGUI.indentLevel= 2;
                foreach(var port in outPorts) {
                    iCS_EditorObject inPort= myStorage.FindAConnectedPort(port);
                    EditorGUILayout.LabelField("Name", inPort.Name);                        
                    EditorGUILayout.LabelField("State", myStorage.GetParent(inPort).Name);                    
                }
            }
        }
        // Show inbound transitions.
        if(inPorts.Count > 0) {
            EditorGUI.indentLevel= 1;
            myShowInputs= EditorGUILayout.Foldout(myShowInputs, "Inbound Transitions");
            if(myShowInputs) {
                EditorGUI.indentLevel= 2;
                foreach(var port in inPorts) {
                    EditorGUILayout.LabelField("Name", port.Name);                        
                    iCS_EditorObject outPort= myStorage.GetSource(port);
                    EditorGUILayout.LabelField("State", myStorage.GetParent(outPort).Name);                    
                }
            }
        }
    }
    
	// ----------------------------------------------------------------------
    // Inspects the selected port.
    void InspectPort(iCS_EditorObject port) {
        iCS_EditorObject parent= myStorage.GetParent(port);
        EditorGUILayout.LabelField("Parent", parent.Name);
        string inOut= port.IsInputPort ? (port.IsEnablePort ? "enable":"in") : "out";
        EditorGUILayout.LabelField("Direction", inOut);
        iCS_GuiUtilities.OnInspectorDataPortGUI(port, myStorage, 1, myFoldoutDB);        
    }


}
