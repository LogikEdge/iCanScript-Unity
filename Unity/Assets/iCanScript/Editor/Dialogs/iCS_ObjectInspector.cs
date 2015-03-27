using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Subspace;

namespace iCanScript.Editor {
public class iCS_ObjectInspector : EditorWindow {
    // ======================================================================
    // Constants.
	// ----------------------------------------------------------------------
    const string EmptyStr= "(empty)";
    
    // ======================================================================
    // Fields
	// ----------------------------------------------------------------------
    private iCS_EditorObject          myObject   = null;
	private Dictionary<string,object> myFoldoutDB= new Dictionary<string,object>();
	
	// ----------------------------------------------------------------------
    // Display state properties.
    private bool    myShowInputs = false;
    private bool    myShowOutputs= false;
        
    // ======================================================================
    // Properties
	// ----------------------------------------------------------------------
    public iCS_EditorObject InspectedObject { get { return myObject; }}
    
    // ======================================================================
    // Initialization/Teardown
    // ----------------------------------------------------------------------
    public static iCS_ObjectInspector CreateInstance(iCS_EditorObject theObject, Vector2 pos) {
        iCS_ObjectInspector objectInspector= ScriptableObject.CreateInstance(typeof(iCS_ObjectInspector)) as iCS_ObjectInspector;
        objectInspector.Init(theObject, pos);
        return objectInspector;
    }
    public void Init(iCS_EditorObject theObject, Vector2 pos) {
        myObject= theObject;
        title= theObject.Name;
        position= new Rect(pos.x, pos.y, 300, 200);
        ShowAuxWindow();
    }
    
    public void OnEnable() {}
    public void OnDisable() {}

    // ======================================================================
    // GUI Update
    // ----------------------------------------------------------------------
    public void OnGUI() {
        if(myObject == null) return;

        // Update the title
        title= "Inspector :: "+myObject.Name;
        
        // Use inspector skin.
        GUI.skin= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector) as GUISkin;
        EditorGUI.indentLevel= 0;
        GUI.enabled= true;
		EditorGUIUtility.LookLikeControls();

        // Draw common attribute of target.
        if(myObject == null) {
            EditorGUILayout.LabelField("No object selected.");
            return;
        }
        var iStorage= myObject.IStorage;
        
        // Display object id.
        EditorGUILayout.LabelField("id", myObject.InstanceId.ToString());
        // Display object type.
        var typeName= ObjectNames.NicifyVariableName(myObject.ObjectType.ToString());
        EditorGUILayout.LabelField("Type", typeName);
        // Display object initial name.
        if(myObject.IsNode) {
            EditorGUILayout.LabelField("Initial Name", myObject.DefaultName);
        }
        // Display object name.
        string name= myObject.Name;
        if(myObject.IsOutStatePort) name= iStorage.FindAConnectedPort(myObject).Name;
        if(string.IsNullOrEmpty(name)) name= EmptyStr;
        if(myObject.IsNameEditable) {
            GUI.changed= false;
            var newName= EditorGUILayout.TextField("Name", myObject.Name);
            if(GUI.changed) {
                iCS_UserCommands.ChangeName(myObject, newName);
            }
        } else {
            EditorGUILayout.LabelField("Name", name);                    
        }
//        // Show object tooltip.
//        string toolTip= myObject.Tooltip;
//        if(myObject.IsOutStatePort) toolTip= iStorage.FindAConnectedPort(myObject).Tooltip;
//        if(string.IsNullOrEmpty(toolTip)) toolTip= EmptyStr;
//		GUI.SetNextControlName("tooltip");
//        myTooltipEditor.Update("Tooltip", toolTip,
//            newTooltip=> {
//                iCS_UserCommands.OpenTransaction(iStorage);
//                myObject.Tooltip= newTooltip;
//                if(myObject.IsStatePort) {
//                    if(myObject.IsOutStatePort) iStorage.FindAConnectedPort(myObject).Tooltip= toolTip;
//                    else myObject.ProducerPort.Tooltip= toolTip;
//                }
//                iCS_UserCommands.CloseTransaction(iStorage, "Change tooltip");
//            }
//        );
        // Show inspector specific for each type of component.
        if(myObject.IsNode)      InspectNode(myObject);
        else if(myObject.IsPort) InspectPort(myObject);
//
//        // Allow repaint for modifications done by the user.
//        myPreviousModificationId= myIStorage.ModificationId;		

    }
	// ----------------------------------------------------------------------
    void InspectNode(iCS_EditorObject node) {
        // Show runtime frame id.
        var iStorage= node.IStorage;
        var runtimeObject= iStorage.GetRuntimeObject(node) as SSAction;
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
        var iStorage= node.IStorage;
        List<iCS_EditorObject> inPorts= new List<iCS_EditorObject>();
        List<iCS_EditorObject> outPorts= new List<iCS_EditorObject>();
        iStorage.ForEachChild(node,
            child=> {
                if(child.IsInDataOrControlPort)  inPorts.Add(child);
                if(child.IsOutDataOrControlPort) outPorts.Add(child);
            }
        );

        // Show inputs.
        var runtimeObject= iStorage.GetRuntimeObject(node) as SSNodeAction;
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
                Prelude.forEach(port=> iCS_GuiUtilities.OnInspectorDataPortGUI(port, iStorage, indentLevel, myFoldoutDB), inPorts);
            }        
        }

        // Show outputs
        if(outPorts.Count > 0 && runtimeObject != null) {
            EditorGUI.indentLevel= 1;
            myShowOutputs= EditorGUILayout.Foldout(myShowOutputs, "Outputs");
            if(myShowOutputs) {
                Prelude.forEach(port=> iCS_GuiUtilities.OnInspectorDataPortGUI(port, iStorage, 2, myFoldoutDB), outPorts);
            }            
        }
    }

	// ----------------------------------------------------------------------
    // Inspect state node.
    void InspectStateNode(iCS_EditorObject node) {
        // Collect transitions.
        var iStorage= node.IStorage;
        iCS_EditorObject[] dataPorts= iStorage.RecalculatePortIndexes(node);
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
                    iCS_EditorObject inPort= iStorage.FindAConnectedPort(port);
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
        iCS_GuiUtilities.OnInspectorDataPortGUI(port, port.IStorage, 1, myFoldoutDB);        
    }

}

}
