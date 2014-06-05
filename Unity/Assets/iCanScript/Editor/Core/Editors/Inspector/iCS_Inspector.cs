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
    private iCS_IStorage              myIStorage            = null;
	private iCS_EditorObject          mySelectedObject      = null;
	private Dictionary<string,object> myFoldoutDB           = new Dictionary<string,object>();
	
	// ----------------------------------------------------------------------
    // Display state properties.
	private bool    mySelectedObjectFold= true;
    private bool    myShowInputs        = false;
    private bool    myShowOutputs       = false;
    private bool    myEngineObjectFold  = false;
    
	// ----------------------------------------------------------------------
    // Repaint abort processing variables.
    private bool                myAbortRepaint          = false;
    private iCS_IStorage        myPreviousStorage       = null;
    private int                 myPreviousModificationId= -1;
    private iCS_EditorObject    myPreviousSelectedObject= null;
    private int                 myPreviousPortSourceId  = -1;
    
	// ----------------------------------------------------------------------
    // Keyboard input functions
    iCS_BufferedTextField myNameEditor   = new iCS_BufferedTextField();
    iCS_BufferedTextField myTooltipEditor= new iCS_BufferedTextField();

    
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
        iCS_EditorController.Update();
        var monoBehaviour= target as iCS_MonoBehaviourImp;
        if(monoBehaviour == null || !iCS_StorageController.IsSameVisualScript(monoBehaviour, iCS_StorageController.IStorage)) {
            myIStorage= null;
			mySelectedObject= null;
            return;
        }

        // Configure the editor with the selected graph.
		myIStorage= iCS_StorageController.IStorage;
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
                    if(myPreviousPortSourceId != mySelectedObject.ProviderPortId) {
                        myAbortRepaint= true;
                        myPreviousPortSourceId= mySelectedObject.ProviderPortId;
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
        if(mySelectedObject == null) {
            EditorGUILayout.LabelField("No Visual Script object selected.");
        }
        else {
            mySelectedObjectFold= EditorGUILayout.Foldout(mySelectedObjectFold, "Selected Object");
            if(mySelectedObjectFold) {
                EditorGUI.indentLevel= 1;
                // Display object id.
                EditorGUILayout.LabelField("id", SelectedObject.InstanceId.ToString());
                // Display object type.
                var typeName= ObjectNames.NicifyVariableName(SelectedObject.ObjectType.ToString());
                EditorGUILayout.LabelField("Type", typeName);
                // Display object name.
                string name= SelectedObject.RawName;
                if(mySelectedObject.IsOutStatePort) name= myIStorage.FindAConnectedPort(SelectedObject).RawName;
                if(name == null || name == "") name= EmptyStr;
                if(mySelectedObject.IsNameEditable) {
                    myNameEditor.Update("Name", SelectedObject.RawName,
                        newName=> iCS_UserCommands.ChangeName(SelectedObject, newName)
                    );
                } else {
                    EditorGUILayout.LabelField("Name", name);                    
                }
                // Show object tooltip.
                string toolTip= SelectedObject.Tooltip;
                if(mySelectedObject.IsOutStatePort) toolTip= myIStorage.FindAConnectedPort(SelectedObject).Tooltip;
                if(toolTip == null || toolTip == "") toolTip= EmptyStr;
				GUI.SetNextControlName("tooltip");
                myTooltipEditor.Update("Tooltip", toolTip,
                    newTooltip=> {
                        iCS_UserCommands.OpenTransaction(myIStorage);
                        SelectedObject.Tooltip= newTooltip;
                        if(SelectedObject.IsStatePort) {
                            if(SelectedObject.IsOutStatePort) myIStorage.FindAConnectedPort(SelectedObject).Tooltip= toolTip;
                            else SelectedObject.ProviderPort.Tooltip= toolTip;
                        }
                        iCS_UserCommands.CloseTransaction(myIStorage, "Change tooltip");
                    }
                );
                // Show inspector specific for each type of component.
                if(SelectedObject.IsNode)      InspectNode(SelectedObject);
                else if(SelectedObject.IsPort) InspectPort(SelectedObject);
            }            

            // Show default inspector
            EditorGUI.indentLevel= 0;
            myEngineObjectFold= EditorGUILayout.Foldout(myEngineObjectFold, "Selected Engine Object");
            if(myEngineObjectFold) {
                // Update engine selected object.
                var storage= myIStorage.Storage;
                var selectedId= storage.SelectedObject;
                storage.EngineObject= selectedId != -1 ? storage.EngineObjects[selectedId] : null;

                GUI.enabled= false;
                EditorGUI.indentLevel= 1;
        		DrawDefaultInspector();            
            }
        }

        // Allow repaint for modifications done by the user.
        myPreviousModificationId= myIStorage.ModificationId;		
	}

	// ----------------------------------------------------------------------
    void InspectNode(iCS_EditorObject node) {
        // Show runtime frame id.
        var runtimeObject= myIStorage.GetRuntimeObject(node) as iCS_Action;
        if(runtimeObject != null) {
            EditorGUILayout.LabelField("FrameId", runtimeObject.FrameId.ToString());
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
        var runtimeObject= myIStorage.GetRuntimeObject(node) as iCS_ISignature;
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
                    iCS_EditorObject outPort= port.ProviderPort;
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
        iCS_GuiUtilities.OnInspectorDataPortGUI(port, myIStorage, 1, myFoldoutDB);        
    }


}
