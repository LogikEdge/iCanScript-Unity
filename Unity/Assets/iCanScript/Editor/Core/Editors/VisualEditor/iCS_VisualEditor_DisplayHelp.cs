using UnityEngine;
using UnityEditor;
using System;
using System.Text.RegularExpressions;

public partial class iCS_VisualEditor : iCS_EditorBase {
	
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	bool        myHelpEnabled      = true;
	string      myHelpText         = null;
    Texture2D   myHelpLogo         = null;
    Texture2D   myHelpDontLogo     = null;
	bool        myIsDynamicHeight  = false;
    
    
    // ======================================================================
    // Dynamic Properties
    // ----------------------------------------------------------------------

	// Use to turn help window on or off.
	public bool IsHelpEnabled {
	        set { myHelpEnabled= value; }
			get { return myHelpEnabled; }
	}


    // =================================================================================================
    // Help Initialization
	// -------------------------------------------------------------------------------------------------
    void HelpInit() {
        iCS_TextureCache.GetIcon(iCS_EditorStrings.HelpMediumIcon, out myHelpLogo);
        iCS_TextureCache.GetIcon(iCS_EditorStrings.DontIcon_24, out myHelpDontLogo);                   
        HotZoneAdd(HelpHotZone, HelpHotZoneGUI, HelpHotZoneMouseClick, null);
    }
 
    // =================================================================================================
    // Mouse in detected over another window, show contextual help for that window in the Visual Editor.
	// -------------------------------------------------------------------------------------------------
	public void helpWindowChange() {
		EditorWindow edWin= EditorWindow.mouseOverWindow;
		if(edWin != null) {
			myIsDynamicHeight= false;
			myHelpText= iCS_HelpController.getHelp(edWin.GetType());
            Repaint();		    
		}
	}
	   
    // =================================================================================================
    // Mouse in detected over a library item, show help in the Visual Editor.
	// -------------------------------------------------------------------------------------------------
	public void libraryHelp(iCS_MemberInfo memInfo ) {
		EditorWindow edWin= EditorWindow.mouseOverWindow;
		if(edWin!= null && edWin.GetType() == typeof(iCS_LibraryEditorWindow)) { 
			if(memInfo == null) {
				helpWindowChange();
			} 
			else {
				myIsDynamicHeight= true;
				myHelpText= iCS_HelpController.GetHelpTitle(memInfo) + "\n" + iCS_HelpController.getHelp(memInfo);
	            Repaint();	
			} 
		}
	}
				
    // ================================================================================
    // Find object under mouse in VisualEditor, and prepare help.  Use only from onGUI.
	// --------------------------------------------------------------------------------
    void UpdateHelp() {
        // -- Update the helkp information --
		iCS_EditorObject edObj= null;
		iCS_PickInfo pickInfo= myGraphics.GetPickInfo(GraphMousePosition, IStorage);
		if(pickInfo != null) {
			edObj= pickInfo.PickedObject;
			if(edObj != null)
				myIsDynamicHeight= false;
				myHelpText= prepareHelpWindowText(edObj);
		}
	}

    // ================================================================================
    // Hot Zone Management
	// --------------------------------------------------------------------------------
    Rect HelpHotZone {
        get {
            if(myHelpLogo == null) return new Rect(0,0,0,0);
            var margin= 5f;
            var w= myHelpLogo.width;
            var h= myHelpLogo.height;
            return new Rect(position.width-w-margin, position.height-h-margin, w, h);
        }
    }
	// --------------------------------------------------------------------------------
    void HelpHotZoneGUI() {
        GUI.DrawTexture(HelpHotZone, myHelpLogo);
        if(!myHelpEnabled) {
            GUI.DrawTexture(HelpHotZone, myHelpDontLogo);
        }
    }
	// --------------------------------------------------------------------------------
    void HelpHotZoneMouseClick() {
        myHelpEnabled^= true;
    }
    
    
	enum Direction {Producer, Consumer};
	
    // ======================================================================
    // prepares and returns the help string which will be displayed in onGUI 
	// ----------------------------------------------------------------------
	string prepareHelpWindowText(iCS_EditorObject edObj) {
		string helpText= "";
		if(edObj.IsPort) {									
			// find connected port
			Direction direction= edObj.IsInputPort ? Direction.Producer : Direction.Consumer;
			iCS_EditorObject connectedPort= getConnectedPort(edObj, direction);

			// If the connected port is ourself, it is not really a connected port
			if ( connectedPort == edObj) {	
				connectedPort= null;
			}		
			
			// Show help for selected port, and connected port
			string helpPart1 = null;
			string helpPart2 = null;
			string divider= null;
			
			// Treat visible relay ports with different formating.
			if(edObj.IsRelayPort && !edObj.ParentNode.IsInstanceNode) {
				helpText= "<b>" + iCS_HelpController.titleColour + "Relay Port:" + "</color></b>\n\n";
				helpPart1= prepareHelpItemRelayPort(getConnectedPort(edObj, Direction.Producer));
				helpPart2= prepareHelpItemRelayPort(getConnectedPort(edObj, Direction.Consumer));
				divider= "<->\n";
			}
			else {
				helpPart1= prepareHelpItem(edObj);
				if (connectedPort != null) { 
					helpPart2= prepareHelpItem(connectedPort);
					divider= "\n<b>connected-> </b>";
				}
			}			
			helpText += helpPart1 + (connectedPort==null ? "" : divider + helpPart2);			
		}
		else if (edObj.IsNode){
			// Show help for node			
			helpText= prepareHelpItem(edObj);
		}
		return helpText;
	}
	
	iCS_EditorObject getConnectedPort(iCS_EditorObject edObj, Direction direction) {
		iCS_EditorObject port= edObj;
	    if (direction == Direction.Producer) {
			port= edObj.FirstProducerPort;
			if(port.ParentNode.ParentNode.IsInstanceNode) {
				port= port.ConsumerPorts[0];
			}
		}
		else if(direction == Direction.Consumer) {
			port= edObj.EndConsumerPorts[0];
			if(port.ParentNode.ParentNode.IsInstanceNode) {
				port= port.ProducerPort;
			}
		}	
		return port;
	}
	
	// Prepare help text for a single item, to be combined by prepareHelpWindowText.
	string prepareHelpItem(iCS_EditorObject edObj) {
		string helpText= null;
		string title= iCS_HelpController.GetHelpTitle(edObj, true, false);
		string help=  iCS_HelpController.getHelp(edObj);
	
		if (!String.IsNullOrEmpty(title)) {
			helpText= title + "\n";
		}
		if (!String.IsNullOrEmpty(help)) {
			helpText += help + "\n";
		}
		
		return helpText;
	}
	
	// Prepare help text for a single item, to be combined by prepareHelpWindowText.
	string prepareHelpItemRelayPort(iCS_EditorObject edObj) {
		return iCS_HelpController.GetHelpTitle(edObj, true, true) + "\n";
	}

    // =======================================================================
    // Display the help already populated in myHelpText.  Use only from onGUI.
	// -----------------------------------------------------------------------
	void DisplayHelp() {
		if(myHelpText==null)
			return;
		
		int numLines= myHelpText.Length - myHelpText.Replace(Environment.NewLine, string.Empty).Length;
		
		int helpHeight= 100;
		int helpWidth= 400; 
		
		if(myIsDynamicHeight) {
			helpHeight=numLines*17;
			if(numLines<=3) helpHeight=100;
			else if(numLines<=10) helpHeight=200;
		}
		
		int helpPosX= 0;
		int helpPosY= Screen.height-helpHeight;
		
		EditorWindow libEdWin= iCS_EditorController.FindLibraryEditorWindow();
		if(libEdWin != null) {
			if(libEdWin.position.x > position.x) {
				// Relocate Help window closed to library window
				helpPosX= Screen.width-helpWidth;
			}		
		}
			
		
		if(myHelpText != null && myHelpEnabled) {
			GUIStyle style =  EditorStyles.textArea;
			style.richText = true;
			GUI.Box(new Rect(helpPosX, helpPosY, helpWidth, helpHeight), myHelpText, style);
            GUI.DrawTexture(HelpHotZone, myHelpLogo);
		}
	}
}
