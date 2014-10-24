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
		string helpText= "";
		if(edObj.IsPort) {			
			// Show help for selected port				
			helpText= helpText + GetPortTitle(edObj) + "\n" + iCS_HelpController.getHelp(edObj) + "\n";			
			// find connected port
			iCS_EditorObject connectedPort= null;
		    if (edObj.IsInputPort) {
				connectedPort= edObj.FirstProducerPort;
			}
			else if(edObj.IsOutputPort) {
				//For screen real estate reasons, show only first connection.
				connectedPort= edObj.EndConsumerPorts[0];
			}
					
			if (connectedPort != null && connectedPort != edObj) {	
				helpText= helpText + "\n<b>connected-> </b>" + GetPortTitle(connectedPort) + 
					"\n" + iCS_HelpController.getHelp(connectedPort) + "\n";
			}
		}
		else if (edObj.IsNode){
			string title= GetNodeTitle(edObj);
			string help= iCS_HelpController.getHelp(edObj);
			if (! (String.IsNullOrEmpty(title) && String.IsNullOrEmpty(help)))
				helpText= title + "\n" + help + "\n";
		}
		
		// Display default help, if no specific help is available.
		if (!String.IsNullOrEmpty(helpText)) {
			return helpText != null ? helpText : Regex.Replace(defaultHelp, "<tcolor>", titleColour);
		}
		else
			return Regex.Replace(defaultHelp, "<tcolor>", titleColour);

	}
	
	private static string GetNodeTitle(iCS_EditorObject edObj) {
		if (edObj==null)
			return null;
				
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
		else
			return null;
		
		return "<b>" + typeName + " " + titleColour + edObj.DisplayName + "</color></b>";
	}

	private static string GetPortTitle(iCS_EditorObject edObj) {
		if (edObj!=null) {
//			if(edObj.ParentNode != null) {
//				if(edObj.ParentNode.IsKindOfFunction || edObj.ParentNode.IsVariableReference || edObj.ParentNode.IsFunctionCall) {	
			
			if (edObj.IsInputPort) {
				edObj= edObj.EndConsumerPorts[0];
			}
			else if (edObj.IsOutputPort) {
				edObj= edObj.FirstProducerPort;
			}
			
			if(edObj.ParentNode.ParentNode.IsInstanceNode) 
				edObj= edObj.IsInputPort ? edObj.ProducerPort : edObj.ConsumerPorts[0];	
			
			// If the parent.parent is an instance node, step back one
			if(edObj.ParentNode.ParentNode.IsInstanceNode)
				edObj = edObj.IsInputPort ? edObj.ProducerPort : edObj.ConsumerPorts[0];			
				
			string displayName= edObj.DisplayName;
			// Variable name "<Color>" interferes with the RTF ...
			displayName= Regex.Replace(displayName, "<Color>", "< Color >");	
			return "<b>" + iCS_Types.TypeName(edObj.RuntimeType) + " " + titleColour + displayName + "</color></b>";
		}
//			}
//		}
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
