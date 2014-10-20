using UnityEngine;
using UnityEditor;
using System;

public partial class iCS_VisualEditor : iCS_EditorBase {
	
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	private string myHelpText  = null;
	string nameColour = EditorGUIUtility.isProSkin ? "<color=cyan>" : "<color=blue>";
	
    // ======================================================================
    // Poplulates the help string which will be displayed on on GUI when 
	// a node/port is floated over.
	// ----------------------------------------------------------------------
	void PopulateHelp(iCS_EditorObject edObj) {
		if(edObj==null)
			return;
		
		Rect position= edObj.AnimatedRect;
		bool isMouseOver= position.Contains(GraphMousePosition);
		if(isMouseOver) {
			myHelpText="";
			// Polpulate help if pointed object is a port
			if(edObj.IsPort) {
				iCS_EditorObject   firstPort= edObj.FirstProducerPort;
				iCS_EditorObject[] endPortArray= edObj.EndConsumerPorts;
							
			    if (edObj.IsInputPort) {
					// there should only be one end consumer port for an input port.
					
					if(endPortArray[0] != null){
			   			myHelpText= myHelpText + GetPortHelpString("", edObj, endPortArray[0].ParentNode);
					}	
			   		if(firstPort != null && firstPort != endPortArray[0]) {
			   			myHelpText= myHelpText + "\n\n" + GetPortHelpString("connected-> ", firstPort, firstPort.ParentNode);
			   		}	
			   	}
				else if(edObj.IsOutputPort) {
			   		if(firstPort != null) {
			   			myHelpText= myHelpText + GetPortHelpString("", edObj, firstPort.ParentNode);
			   		}
					myHelpText= myHelpText + "\n";
					foreach(iCS_EditorObject endPort in endPortArray) {
						if(endPort != null && firstPort != endPort){
							myHelpText= myHelpText + "\n" + GetPortHelpString("connected-> ", endPort, endPort.ParentNode);
						}
					}
				}
			}
			// Polpulate help if pointed object is a node
			else {
				iCS_MemberInfo memberInfo= iCS_LibraryDatabase.GetAssociatedDescriptor(edObj);
				myHelpText= memberInfo != null ? memberInfo.Summary : null;
				if(String.IsNullOrEmpty(myHelpText)) {
					myHelpText= "no tip available";
				}
				else {
					myHelpText= "<b>" + nameColour + edObj + "</color></b>" + "\n" + myHelpText;
				}
			}
		}
	}

    // ======================================================================
    // Used by populate help to build help string
	string GetPortHelpString(string prefix, iCS_EditorObject nameEdObj, iCS_EditorObject edObj) {
		if(edObj != null && edObj.IsKindOfFunction) {
			string displayName= nameEdObj.DisplayName;
			string typeName= iCS_Types.TypeName(nameEdObj.RuntimeType);
	    	iCS_MemberInfo memberInfo= iCS_LibraryDatabase.GetAssociatedDescriptor(edObj);
			string summary= memberInfo != null ? memberInfo.Summary : null;					
			if(!String.IsNullOrEmpty(summary)) {
				return "<b>" + prefix + typeName + " " + nameColour + displayName + "</color></b>" + "\n" + summary;
			}
		}
		return null;
	}
}