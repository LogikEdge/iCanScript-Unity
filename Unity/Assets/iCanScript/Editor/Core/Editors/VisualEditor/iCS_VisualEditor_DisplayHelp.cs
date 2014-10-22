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
	string titleColour = EditorGUIUtility.isProSkin ? "<color=cyan>" : "<color=blue>";
	string noHelp= "no tip available";
	string defaultHelp= 
		"<tcolor><b>Keyboard shortcuts (for selected):</b></color>\n" +
		"<tcolor><b>H:</b></color> more Help\t\t<tcolor><b>drag:</b></color> moves node\n" +
		"<tcolor><b>F:</b></color> Focus in view\t\t<tcolor><b>ctrl-drag:</b></color> moves node outside\n" +
		"<tcolor><b>L:</b></color> auto Layout\t\t<tcolor><b>shift-drag:</b></color> move copies node\n" +
		"<tcolor><b>B:</b></color> Bookmark\t\t<tcolor><b>del:</b></color> deletes object\n" +
		"<tcolor><b>G:</b></color> move to bookmark\t<tcolor><b>C:</b></color> Connect to bookmark\n";
	
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
    void updateHelp() {
		iCS_PickInfo pickInfo= myGraphics.GetPickInfo(GraphMousePosition, IStorage);
		if(pickInfo != null) {
			iCS_EditorObject edObj= pickInfo.PickedObject;
			if(edObj != null)
				PopulateHelpString(edObj);
		}   
	}
	
    // ======================================================================
    // Poplulates the help string which will be displayed in onGUI 
	// ----------------------------------------------------------------------
	void PopulateHelpString(iCS_EditorObject edObj) {
		myHelpText=null;
		// Polpulate help if pick object is a port
		if(edObj.IsPort) {
			myHelpText="";
			iCS_EditorObject   firstProducerPort= edObj.FirstProducerPort;
			iCS_EditorObject[] endConsumerPortArray= edObj.EndConsumerPorts;					
		    if (edObj.IsInputPort) {
				// there should only be one end consumer port for an input port.			
				if(endConsumerPortArray[0] != null){
					myHelpText= myHelpText + GetPortHelpString("", endConsumerPortArray[0]);
				}	
				if (!String.IsNullOrEmpty(myHelpText)) {
					 myHelpText= myHelpText + "\n";
				 }
		   		if(firstProducerPort != null && firstProducerPort != endConsumerPortArray[0]) {
					myHelpText= myHelpText + GetPortHelpString("connected-> ", firstProducerPort);
		   		}	
		   	}
			else if(edObj.IsOutputPort) {
		   		if(firstProducerPort != null) {
		   			myHelpText= myHelpText + GetPortHelpString("", firstProducerPort);
		   		}
				if (!String.IsNullOrEmpty(myHelpText)) {
					 myHelpText= myHelpText + "\n";
				}			
				//For screen real estate reasons, show only first connection.
				iCS_EditorObject endConsumerPort= endConsumerPortArray[0];
				if(endConsumerPort != null && firstProducerPort != endConsumerPort) {
					myHelpText= myHelpText + GetPortHelpString("connected-> ", endConsumerPort);
				}
			}
		}
		// Polpulate help if pick object is a node
		else if (edObj.IsNode){
			myHelpText= GetNodeHelpString("", edObj);
		}
		
		// Display default help, if no specific help is available.
		if (String.IsNullOrEmpty(myHelpText))	
			myHelpText= Regex.Replace(defaultHelp, "<tcolor>", titleColour);
	}

    // ======================================================================
    // Used by populate help to build node help string
	string GetNodeHelpString(string prefix, iCS_EditorObject edObj) {
		iCS_MemberInfo memberInfo= iCS_LibraryDatabase.GetAssociatedDescriptor(edObj);
		
		string summary= memberInfo != null ? memberInfo.Summary : noHelp ;
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
		
		if (memberInfo != null || typeName != null ) {
			return "<b>" + prefix + typeName + " " + titleColour + edObj.DisplayName + "</color></b>" + "\n" + summary + "\n";
		}
		else {
			return null;
		}	
	}


    // ======================================================================
    // Used by populate help to build port help string
	string GetPortHelpString(string prefix, iCS_EditorObject edObj) {
		if(edObj.ParentNode != null) {
			if(edObj.ParentNode.IsKindOfFunction || edObj.ParentNode.IsVariableReference || edObj.ParentNode.IsFunctionCall) {
				iCS_EditorObject portNameEdObj= edObj;
				if(edObj.ParentNode.ParentNode.IsInstanceNode) {
					portNameEdObj= edObj.IsInputPort ? edObj.ProducerPort : edObj.ConsumerPorts[0];
				}			
				string summary= null;
	    		iCS_MemberInfo memberInfo= iCS_LibraryDatabase.GetAssociatedDescriptor(edObj.ParentNode);
				string typeName= iCS_Types.TypeName(portNameEdObj.RuntimeType);
				string displayName= portNameEdObj.DisplayName;
				// Variable name "<Color>" interferes with the RTF ...
				displayName= Regex.Replace(displayName, "<Color>", "< Color >");	
				
				if(memberInfo != null) {
					// Handle special types of ports.
					if (portNameEdObj.PortIndex == (int)iCS_PortIndex.Return && edObj.ParentNode.IsClassField) {
						// return port will be same as parent node description, and no need to show type name
						// which will be repeated in port name.
						summary= memberInfo.Summary;	
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
						summary= memberInfo.Summary;
					}
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
