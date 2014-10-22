using UnityEngine;
using UnityEditor;
using System;
using System.Text.RegularExpressions;

public partial class iCS_VisualEditor : iCS_EditorBase {
	
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	
	private bool myHelpEnabled= true;
	private string myHelpText= null;
	
	// Help strings.  Note <tcolor> should be replaced with title colour markup when used.
	static string titleColour = EditorGUIUtility.isProSkin ? "<color=cyan>" : "<color=blue>";
	const string noHelp= "no help available";
	const string defaultHelp= 
		"<tcolor><b>Keyboard shortcuts (for selected):</b></color>\n" +
		"<tcolor><b>H:</b></color> more Help\t\t<tcolor><b>drag:</b></color> moves node\n" +
		"<tcolor><b>F:</b></color> Focus in view\t\t<tcolor><b>ctrl-drag:</b></color> moves node outside\n" +
		"<tcolor><b>L:</b></color> auto Layout\t\t<tcolor><b>shift-drag:</b></color> move copies node\n" +
		"<tcolor><b>B:</b></color> Bookmark\t\t<tcolor><b>del:</b></color> deletes object\n" +
		"<tcolor><b>G:</b></color> move to bookmark\t<tcolor><b>C:</b></color> Connect to bookmark\n";
	const string libraryHelp= 
		"<tcolor><b>Library tips:</b></color>\n" +
		"From the library window, you can drag objects into a message handler in the Visual Editor window to create nodes.\n" +
		"\n" +
		"<tcolor><b>Keyboard shortcuts:</b></color>\n" +
		"<tcolor><b>H:</b></color> additional help for selected\t <tcolor><b>Enter:</b></color> fold/unfold";
			
	const string InstanceEditorHelp= 
		"<tcolor><b>Instance Wizard tips:</b></color>\n" +
		"The Instance Wizard is used to add and remove ports in a class node.  Select a class node in the visual editor to use.\n";

	
    // ======================================================================
    // Dynamic Properties
    // ----------------------------------------------------------------------

	// Use to turn help window on or off.
	public bool IsHelpEnabled {
	        set { myHelpEnabled= value; }
			get { return myHelpEnabled; }
	}
	
    // ======================================================================
    // Find object under mouse, and prepare help.
	// ----------------------------------------------------------------------
    private void updateHelp() {
		iCS_PickInfo pickInfo= myGraphics.GetPickInfo(GraphMousePosition, IStorage);
		if(pickInfo != null) {
			iCS_EditorObject edObj= pickInfo.PickedObject;
			if(edObj != null)
				myHelpText= getHelpWindowText(edObj);
		}   
	}
	
    // ======================================================================
    // Display library tip in visual editor when mouse is over library
	// ----------------------------------------------------------------------
    public void updateHelpFromLibrary() {
		myHelpText= Regex.Replace(libraryHelp, "<tcolor>", titleColour);
	}
	
    // ======================================================================
    // Display library tip in visual editor when mouse is over library
	// ----------------------------------------------------------------------
    public void updateHelpFromInstanceEditor() {
		myHelpText= Regex.Replace(InstanceEditorHelp, "<tcolor>", titleColour);
	}
	
    // ======================================================================
    // Poplulates the help string which will be displayed in onGUI 
	// ----------------------------------------------------------------------
	public static string getHelpWindowText(iCS_EditorObject edObj) {
		// Polpulate help if pick object is a port
		string helpText= "";
		if(edObj.IsPort) {
			iCS_EditorObject   firstProducerPort= edObj.FirstProducerPort;
			iCS_EditorObject[] endConsumerPortArray= edObj.EndConsumerPorts;					
		    if (edObj.IsInputPort) {
				// there should only be one end consumer port for an input port.			
				if(endConsumerPortArray[0] != null){
					helpText= helpText + GetPortHelpString("", endConsumerPortArray[0]);
				}	
				if (!String.IsNullOrEmpty(helpText)) {
					 helpText= helpText + "\n";
				 }
		   		if(firstProducerPort != null && firstProducerPort != endConsumerPortArray[0]) {
					helpText= helpText + GetPortHelpString("connected-> ", firstProducerPort);
		   		}	
		   	}
			else if(edObj.IsOutputPort) {
		   		if(firstProducerPort != null) {
		   			helpText= helpText + GetPortHelpString("", firstProducerPort);
		   		}
				if (!String.IsNullOrEmpty(helpText)) {
					 helpText= helpText + "\n";
				}			
				//For screen real estate reasons, show only first connection.
				iCS_EditorObject endConsumerPort= endConsumerPortArray[0];
				if(endConsumerPort != null && firstProducerPort != endConsumerPort) {
					helpText= helpText + GetPortHelpString("connected-> ", endConsumerPort);
				}
			}
		}
		// Polpulate help if pick object is a node
		else if (edObj.IsNode){
			helpText= GetNodeHelpString("", edObj);
		}
		
		// Display default help, if no specific help is available.
		return helpText != null ? helpText : Regex.Replace(defaultHelp, "<tcolor>", titleColour);
	}

    // ======================================================================
    // Used by populate help to build node help string
	private static string GetNodeHelpString(string prefix, iCS_EditorObject edObj) {

		string typeName= null;
		// Type names to be displayed in front of node name.
		if(edObj.IsConstructor)
			typeName= "Builder";
		else if(edObj.IsInstanceNode)
			typeName= "Class";
		else if(edObj.IsKindOfFunction)
			typeName= "Function";
		else if(edObj.IsVariableReference)
			typeName= "Variable Reference";
		else if(edObj.IsFunctionCall)
			typeName= "Function Call";
		
		string summary= iCS_HelpController.getHelp(edObj);
		if(String.IsNullOrEmpty(summary))
			summary= noHelp;
		
		if (typeName != null) {
			return "<b>" + prefix + typeName + " " + titleColour + edObj.DisplayName + "</color></b>" + "\n" + summary + "\n";
		}
		else {
			return null;
		}	
	}


    // ======================================================================
    // Used by populate help to build port help string
	private static string GetPortHelpString(string prefix, iCS_EditorObject edObj) {
		if(edObj.ParentNode != null) {
			if(edObj.ParentNode.IsKindOfFunction || edObj.ParentNode.IsVariableReference || edObj.ParentNode.IsFunctionCall) {
				iCS_EditorObject portNameEdObj= edObj;
				if(edObj.ParentNode.ParentNode.IsInstanceNode) {
					portNameEdObj= edObj.IsInputPort ? edObj.ProducerPort : edObj.ConsumerPorts[0];
				}			
				string summary= null;				
				string typeName= iCS_Types.TypeName(portNameEdObj.RuntimeType);
				string displayName= portNameEdObj.DisplayName;
				// Variable name "<Color>" interferes with the RTF ...
				displayName= Regex.Replace(displayName, "<Color>", "< Color >");	
				
				// Handle special types of ports.
				if (portNameEdObj.PortIndex == (int)iCS_PortIndex.Return && edObj.ParentNode.IsClassField) {
					// return port will be same as parent node description, and no need to show type name
					// which will be repeated in port name.
					summary= iCS_HelpController.getHelp(edObj.ParentNode);	
				}
				else if (portNameEdObj.PortIndex == (int)iCS_PortIndex.InInstance || 
						portNameEdObj.PortIndex == (int)iCS_PortIndex.OutInstance) {
					// no need to show type name of Instance ports which will be repeated in port name.
					typeName= "Instance";	
				}
				else if (portNameEdObj.PortIndex == (int)iCS_PortIndex.Return && edObj.ParentNode.IsConstructor) {
					// no need to show type name of builder return port which will be repeated in port name.
					typeName= "Instance";	
				}
				else if( portNameEdObj.PortIndex >= (int)iCS_PortIndex.ParametersEnd ) {
					// No summary available for special types, excluding above exceptions
					summary= noHelp;	
				}
				else {
					summary= iCS_HelpController.getHelp(edObj.ParentNode);
				}
				
				if(String.IsNullOrEmpty(summary)) {
					summary= noHelp;
				}		
				return "<b>" + prefix + typeName + " " + titleColour + displayName + "</color></b>" + "\n" + summary + "\n";
			}
		}
		return null;
	}
	
    // ======================================================================
    // Display the help already populated in myHelpText
	void DisplayHelp() {
		if(myHelpEnabled) {
			GUIStyle style =  EditorStyles.textArea;
			style.richText = true;
			GUI.Box(new Rect(Screen.width-400, Screen.height-100, 400, 100), myHelpText, style);
		}
	}
}
