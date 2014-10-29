using UnityEngine;
using UnityEditor;
using System;
using System.Text.RegularExpressions;

public partial class iCS_VisualEditor : iCS_EditorBase {
	
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	
	bool myHelpEnabled= true;
	string myHelpText= null;
	
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
			if (!String.IsNullOrEmpty(title)) {
				helpText= title + "\n";
			}
			if (!String.IsNullOrEmpty(help)) {
				helpText= helpText + help + "\n";
			}
		}
		return !String.IsNullOrEmpty(helpText) ? helpText : "";
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
