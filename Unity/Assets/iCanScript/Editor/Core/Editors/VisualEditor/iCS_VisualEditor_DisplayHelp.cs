using UnityEngine;
using UnityEditor;
using System;
using System.Text.RegularExpressions;

public partial class iCS_VisualEditor : iCS_EditorBase {
	
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	
	bool myHelpEnabled= true;
	string myHelpText= defaultHelp;
	
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


    // =================================================================================================
    // Mouse in detected over another window, show contextual help for that window in the Visual Editor.
	// -------------------------------------------------------------------------------------------------
	public void helpWindowChange() {
		EditorWindow edWin= EditorWindow.mouseOverWindow;
		if(edWin != null)
			myHelpText= iCS_HelpController.getHelp(edWin.GetType());
	}
		
    // ================================================================================
    // Find object under mouse in VisualEditor, and prepare help.  Use only from onGUI.
	// --------------------------------------------------------------------------------
    private void UpdateHelp() {	
		iCS_EditorObject edObj= null;
		iCS_PickInfo pickInfo= myGraphics.GetPickInfo(GraphMousePosition, IStorage);
		if(pickInfo != null) {
			edObj= pickInfo.PickedObject;
			if(edObj != null)
				myHelpText= prepareHelpWindowText(edObj);
		}
	}
	
    // ======================================================================
    // prepares and returns the help string which will be displayed in onGUI 
	// ----------------------------------------------------------------------
	public string prepareHelpWindowText(iCS_EditorObject edObj) {
		string helpText= "";
		string title= null;
		string help= null;
		if(edObj.IsPort) {						
			// Show help for selected port	
			title= iCS_HelpController.GetHelpTitle(edObj);
			help= iCS_HelpController.getHelp(edObj);
			if (! (String.IsNullOrEmpty(title) && String.IsNullOrEmpty(help)))					
				helpText= helpText + title + "\n" + help + "\n\n";	
			

			// find connected port
			iCS_EditorObject connectedPort= null;
		    if (edObj.IsInputPort) {
				connectedPort= edObj.FirstProducerPort;
				if(connectedPort.ParentNode.ParentNode.IsInstanceNode) {
					connectedPort= connectedPort.ConsumerPorts[0];
				}
			}
			else if(edObj.IsOutputPort) {
				//For screen real estate reasons, show only first connection.
				connectedPort= edObj.EndConsumerPorts[0];
				if(connectedPort.ParentNode.ParentNode.IsInstanceNode) {
					connectedPort= connectedPort.ProducerPort;
				}
			}				
			
			// show help for connected port				
			if (connectedPort != null && connectedPort != edObj) {	
				title= iCS_HelpController.GetHelpTitle(connectedPort);
				help= iCS_HelpController.getHelp(connectedPort);
				if (! (String.IsNullOrEmpty(title) && String.IsNullOrEmpty(help)))
					helpText= helpText + "<b>connected-> </b>" + title + "\n" + help + "\n";
			}
		}
		else if (edObj.IsNode){
			// Show help for node			
			title= iCS_HelpController.GetHelpTitle(edObj);
			help= iCS_HelpController.getHelp(edObj);
			if (! (String.IsNullOrEmpty(title) && String.IsNullOrEmpty(help)))
				helpText= title + "\n" + help + "\n";
		}
		else {
			
		}
		
		// Display default help, if no specific help is available.
		if (!String.IsNullOrEmpty(helpText)) {
			return helpText != null ? helpText : Regex.Replace(defaultHelp, "<tcolor>", titleColour);
		}
		else
			return Regex.Replace(defaultHelp, "<tcolor>", titleColour);
	}
	

    // =======================================================================
    // Display the help already populated in myHelpText.  Use only from onGUI.
	// -----------------------------------------------------------------------
	private void DisplayHelp() {
		if(myHelpText != null && myHelpEnabled) {
			GUIStyle style =  EditorStyles.textArea;
			style.richText = true;
			GUI.Box(new Rect(Screen.width-400, Screen.height-100, 400, 100), myHelpText, style);
		}
	}
}
