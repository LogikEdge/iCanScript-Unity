//#define BUFFERED_INPUT
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Subspace;

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
    private iCS_IStorage              myIStorage            = null;
	private iCS_EditorObject          mySelectedObject      = null;
	private Dictionary<string,object> myFoldoutDB           = new Dictionary<string,object>();
	
	// ----------------------------------------------------------------------
    // Display state properties.
    private bool    myShowInputs        = false;
    private bool    myShowOutputs       = false;
    
	// ----------------------------------------------------------------------
    // Repaint abort processing variables.
    private bool                myAbortRepaint          = false;
    private iCS_IStorage        myPreviousStorage       = null;
    private int                 myPreviousModificationId= -1;
    private iCS_EditorObject    myPreviousSelectedObject= null;
    private int                 myPreviousPortSourceId  = -1;
    
	// ----------------------------------------------------------------------
    // Keyboard input functions
#if BUFFERED_INPUT
    iCS_BufferedTextField myNameEditor   = new iCS_BufferedTextField();
    iCS_BufferedTextField myTooltipEditor= new iCS_BufferedTextField();
#endif

    
    // ======================================================================
    // Properties
	// ----------------------------------------------------------------------
	public  iCS_EditorObject SelectedObject {
	    get {
	        iCS_EditorObject selectedObject= myIStorage != null ? myIStorage.SelectedObject : null;
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
		myIStorage= null;
		mySelectedObject= null;
	}
	
	// ----------------------------------------------------------------------
    // Bring up the graph editor window when the inspector is activated.
    void UpdateVariables() {
        // Verify that the target reflects the selected storage.
        iCS_VisualScriptDataController.Update();
        var monoBehaviour= target as iCS_MonoBehaviourImp;
        if(monoBehaviour == null || !iCS_VisualScriptDataController.IsSameVisualScript(monoBehaviour, iCS_VisualScriptDataController.IStorage)) {
            myIStorage= null;
			mySelectedObject= null;
            return;
        }

        // Configure the editor with the selected graph.
		myIStorage= iCS_VisualScriptDataController.IStorage;
		mySelectedObject= myIStorage.SelectedObject;
    }
    
	// ----------------------------------------------------------------------
    // Paint to inspector for the selected object (see editor).
	public override void OnInspectorGUI ()
	{
        // Update all variables we rely on.
        UpdateVariables();
		
        // Nothing to show if no storage is selected.
        if(myIStorage == null) {
            return;
        }
        
        // Restore inspector skin.
        GUI.skin= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector) as GUISkin;
        
        // Draw inspector window
        EditorGUI.indentLevel= 0;
        GUI.enabled= true;
		EditorGUIUtility.LookLikeControls();

        // Stop any repaint if the selected object has changed between the layout and repaint events.
        switch(Event.current.type) {
            case EventType.Layout:
                myAbortRepaint= false;
                if(myPreviousStorage != myIStorage) {
                    myAbortRepaint= true;
                    myPreviousStorage= myIStorage;
                }
                if(myPreviousModificationId != myIStorage.ModificationId) {
                    myAbortRepaint= true;
                    myPreviousModificationId= myIStorage.ModificationId;
                }
                if(myPreviousSelectedObject != mySelectedObject) {
                    myAbortRepaint= true;
                    myPreviousSelectedObject= mySelectedObject;
                }
                if(mySelectedObject != null && mySelectedObject.IsDataOrControlPort) {
                    if(myPreviousPortSourceId != mySelectedObject.ProducerPortId) {
                        myAbortRepaint= true;
                        myPreviousPortSourceId= mySelectedObject.ProducerPortId;
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
        DrawDefaultInspector();
        
        // Draw selected object.
        EditorGUI.indentLevel= 0;
        if(mySelectedObject == null) {
            EditorGUILayout.LabelField("No Visual Script object selected.  Script size= "+myIStorage.EngineStorage.EngineObjects.Count);
            return;
        }

        EditorGUI.indentLevel= 0;
        // Display selected object name.
        string name= SelectedObject.RawName;
        if(mySelectedObject.IsOutStatePort) name= myIStorage.FindAConnectedPort(SelectedObject).RawName;
        if(name == null || name == "") name= EmptyStr;
        if(mySelectedObject.IsNameEditable) {
#if BUFFERED_INPUT
            myNameEditor.Update("Name", SelectedObject.RawName,
                newName=> iCS_UserCommands.ChangeName(SelectedObject, newName)
            );
#else
            GUI.changed= false;
            var newName= EditorGUILayout.TextField("Name", SelectedObject.RawName);
            if(GUI.changed) {
                iCS_UserCommands.ChangeName(SelectedObject, newName);
            }
#endif
        } else {
            EditorGUILayout.LabelField("Name", name);                    
        }
//        EditorGUILayout.Vector2Field("Global Position", mySelectedObject.GlobalPosition);
//        EditorGUILayout.Vector2Field("Local Anchor Position", mySelectedObject.LocalAnchorPosition);
//        EditorGUILayout.Vector2Field("Collision Offset", mySelectedObject.CollisionOffset);
//        EditorGUILayout.Vector2Field("Wrapping Offset", mySelectedObject.WrappingOffset);
        // Display selected object initial name.
        if(SelectedObject.IsNode) {
            EditorGUILayout.LabelField("Initial Name", mySelectedObject.DefaultName);
        }
        // Display object id.
        EditorGUILayout.LabelField("id", SelectedObject.InstanceId.ToString());
        // Display object type.
        var typeName= ObjectNames.NicifyVariableName(SelectedObject.ObjectType.ToString());
        EditorGUILayout.LabelField("Type", typeName);
        // Show object tooltip.
        string tooltip= SelectedObject.Tooltip;
        if(mySelectedObject.IsOutStatePort) tooltip= myIStorage.FindAConnectedPort(SelectedObject).Tooltip;
        if(tooltip == null || tooltip == "") tooltip= EmptyStr;
#if BUFFERED_INPUT
		GUI.SetNextControlName("tooltip");
        myTooltipEditor.Update("Tooltip", tooltip,
            newTooltip=> {
                iCS_UserCommands.OpenTransaction(myIStorage);
                SelectedObject.Tooltip= newTooltip;
                if(SelectedObject.IsStatePort) {
                    if(SelectedObject.IsOutStatePort) myIStorage.FindAConnectedPort(SelectedObject).Tooltip= toolTip;
                    else SelectedObject.ProducerPort.Tooltip= tooltip;
                }
                iCS_UserCommands.CloseTransaction(myIStorage, "Change tooltip");
            }
        );
#else
        GUI.changed= false;
        var newTooltip= EditorGUILayout.TextField("Tooltip", tooltip);
        if(GUI.changed) {
            iCS_UserCommands.ChangeTooltip(SelectedObject, newTooltip);
        }
#endif
        // Show inspector specific for each type of component.
        if(SelectedObject.IsNode)      InspectNode(SelectedObject);
        else if(SelectedObject.IsPort) InspectPort(SelectedObject);

        // Allow repaint for modifications done by the user.
        myPreviousModificationId= myIStorage.ModificationId;		
	}

	// ----------------------------------------------------------------------
    void InspectNode(iCS_EditorObject node) {
        // Show runtime frame id.
        var runtimeObject= myIStorage.GetRuntimeObject(node) as SSAction;
        if(runtimeObject != null) {
            EditorGUILayout.LabelField("RunId", runtimeObject.EvaluatedRunId.ToString());
        }
        // Show Iconic image configuration.
        Texture2D iconicTexture= iCS_TextureCache.GetIconFromGUID(node.IconGUID);
        Texture2D newTexture= EditorGUILayout.ObjectField("Iconic Texture", iconicTexture, typeof(Texture2D), false) as Texture2D;
        if(newTexture != iconicTexture) {
            iCS_UserCommands.ChangeIcon(node, newTexture);
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
        myIStorage.ForEachChild(node,
            child=> {
                if(child.IsInDataOrControlPort)  inPorts.Add(child);
                if(child.IsOutDataOrControlPort) outPorts.Add(child);
            }
        );

        // Show inputs.
        var runtimeObject= myIStorage.GetRuntimeObject(node) as SSActionWithSignature;
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
                Prelude.forEach(port=> iCS_GuiUtilities.OnInspectorDataPortGUI(port, myIStorage, indentLevel, myFoldoutDB), inPorts);
            }        
        }

        // Show outputs
        if(outPorts.Count > 0 && runtimeObject != null) {
            EditorGUI.indentLevel= 1;
            myShowOutputs= EditorGUILayout.Foldout(myShowOutputs, "Outputs");
            if(myShowOutputs) {
                Prelude.forEach(port=> iCS_GuiUtilities.OnInspectorDataPortGUI(port, myIStorage, 2, myFoldoutDB), outPorts);
            }            
        }
    }

	// ----------------------------------------------------------------------
    // Inspect state node.
    void InspectStateNode(iCS_EditorObject node) {
        // Collect transitions.
        iCS_EditorObject[] dataPorts= myIStorage.RecalculatePortIndexes(node);
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
                    iCS_EditorObject inPort= myIStorage.FindAConnectedPort(port);
                    EditorGUILayout.LabelField("Name", inPort.Name);                        
                    EditorGUILayout.LabelField("State", inPort.Parent.Name);                    
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
                    iCS_EditorObject outPort= port.ProducerPort;
                    EditorGUILayout.LabelField("State", outPort.Parent.Name);                    
                }
            }
        }
    }
    
	// ----------------------------------------------------------------------
    // Inspects the selected port.
    void InspectPort(iCS_EditorObject port) {
        iCS_EditorObject parent= port.Parent;
        EditorGUILayout.LabelField("Parent", parent.Name);
        EditorGUILayout.LabelField("Port Index", port.PortIndex.ToString());
        EditorGUILayout.EnumPopup("Iteration", port.PortIterationSignature);
        iCS_GuiUtilities.OnInspectorDataPortGUI(port, myIStorage, 1, myFoldoutDB);        
    }


}
