using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using P=Prelude;

public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    GUIStyle    myAssistantLabelStyle = null;
    GUIStyle    myAssistaneButtonStyle= null;
    Texture2D   myiCanScriptLargeLogo = null;
    Texture2D   myiCanScriptMediumLogo= null;
    Texture2D   myAssistantLogo       = null;
	int			myAssistantLineCount  = 0;
    
    // ----------------------------------------------------------------------
    void ShowWorkflowAssistant() {
        // -- Build the styles used --
        BuildWorkflowAssistantStyles();

		// -- Restart display at the top --
		myAssistantLineCount= 0;

        // -- Ask user the select a game object if none selected --
        var activeGameObject= Selection.activeGameObject;
        if(activeGameObject == null) {
            ShowAssistantMessageWithBlink("Select a Game Object");
	        ShowTextureCenteredAt(Math3D.Middle(ViewportRectForGraph), myiCanScriptLargeLogo);
			Repaint();
            return;
        }
        // -- Ask user to create a visual script --
        var visualScript= activeGameObject.GetComponent("iCS_MonoBehaviourImp") as iCS_MonoBehaviourImp;
        if(visualScript == null) {
            var content= new GUIContent("Create Visual Script", myiCanScriptMediumLogo);
            var contentSize= myAssistaneButtonStyle.CalcSize(content);
            var r= Math3D.BuildRectCenteredAt(Math3D.Middle(ViewportRectForGraph), 1.2f*contentSize.x, 1.2f*contentSize.y);
            EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
            var savedColor= GUI.color;
            GUI.color= iCS_BlinkController.NormalBlinkHighColor;
            if(GUI.Button(r, content, myAssistaneButtonStyle)) {
                EditorApplication.ExecuteMenuItem("iCanScript/Create Visual Script"); 
            }
            GUI.color= savedColor;
            Repaint();
            return;
        }
		// -- Pointing on canvas --
        var pickInfo= myGraphics.GetPickInfo(GraphMousePosition, IStorage);
        if(pickInfo == null || pickInfo.PickedObject.IsBehaviour) {
            ShowAssistantMessage("Right-Click to Add Message Handler or Public Function");
			ShowAssistantMessage("Drag Builder from Library to Create Public Variable");
			Repaint();
            return;
        }
		// -- Pointing at a node --
		var editorObj= pickInfo.PickedObject;
		if(editorObj.IsNode) {
			switch(pickInfo.PickedPart) {
				case iCS_PickPartEnum.Name: {
					ShowAssistantMessage("Double-Click to edit node name");				
					Repaint();
		            return;											
				}
				case iCS_PickPartEnum.FoldIcon: {
		            ShowAssistantMessage("Click to Fold/Unfold node");
					Repaint();
		            return;					
				}
				case iCS_PickPartEnum.MinimizeIcon: {
		            ShowAssistantMessage("Click to Iconize node");
					Repaint();
		            return;										
				}
				default: {
		            ShowAssistantMessage("Right-Click to Add Package, State Chart, or Control Ports");
					ShowAssistantMessage("Drag from Library to Add functions & variables");
					Repaint();
		            return;								
				}
			}
		}
		// -- Pointing at a port --
		if(editorObj.IsPort) {
			switch(pickInfo.PickedPart) {
				case iCS_PickPartEnum.Name: {
					ShowAssistantMessage("Double-Click to edit port name");				
					Repaint();
		            return;											
				}
				case iCS_PickPartEnum.Value: {
					ShowAssistantMessage("Double-Click to edit port value");				
					Repaint();
		            return;																
				}
				default: {
					ShowAssistantMessage("Drag port onto another port to create a data flow");
					ShowAssistantMessage("Drag port on the edge of a package to create an interface");
					ShowAssistantMessage("Drag and release port to quickly Create a Function");						
					if(editorObj.IsInputPort & editorObj.ProducerPort == null) {
						ShowAssistantMessage("Drag an object from the scene to initialize the port");				
					}
					Repaint();
		            return;
				}
			}
		}
    }
    // ----------------------------------------------------------------------
    void ShowAssistantMessageWithBlink(string message) {
		ShowAssistantMessage(message, DisplayWithHighBlink);
    }
    // ----------------------------------------------------------------------
    void ShowAssistantMessage(string message) {
		ShowAssistantMessage(message, fnc=> fnc());
    }
    // ----------------------------------------------------------------------
    void ShowAssistantMessage(string message, Action<Action> displayModifier) {
        var content= new GUIContent(" "+message, myAssistantLogo);
		var y= iCS_ToolbarUtility.GetHeight()+myAssistantLineCount*myAssistantLabelStyle.CalcHeight(content, position.width);
        var r= new Rect(0, y, position.width, position.height);
		displayModifier(()=> GUI.Label(r, content, myAssistantLabelStyle));
		++myAssistantLineCount;
    }
    // ----------------------------------------------------------------------
	void DisplayWithHighBlink(Action displayFnc) {
        var savedColor= GUI.color;
        GUI.color= iCS_BlinkController.NormalBlinkHighColor;
		displayFnc();
        GUI.color= savedColor;		
	}
    // ----------------------------------------------------------------------
    void ShowTextureCenteredAt(Vector2 center, Texture2D texture) {
        var r= Math3D.BuildRectCenteredAt(center, texture.width, texture.height);
        GUI.DrawTexture(r, texture);
    }
    // ----------------------------------------------------------------------
    void BuildWorkflowAssistantStyles() {
        if(myAssistantLabelStyle == null) {
            myAssistantLabelStyle= new GUIStyle(EditorStyles.whiteLabel);
            myAssistantLabelStyle.fontSize= 24;
            myAssistantLabelStyle.fontStyle= FontStyle.Bold;
            myAssistantLabelStyle.normal.textColor= Color.grey;
        }        
        if(myAssistaneButtonStyle == null) {
            myAssistaneButtonStyle= new GUIStyle(GUI.skin.button);
            myAssistaneButtonStyle.fontSize= 24;
            myAssistaneButtonStyle.fontStyle= FontStyle.Bold;
        }        
        if(myiCanScriptLargeLogo == null) {
            iCS_TextureCache.GetIcon(iCS_EditorStrings.LargeLogoIcon, out myiCanScriptLargeLogo);
        }
        if(myiCanScriptMediumLogo == null) {
            iCS_TextureCache.GetIcon(iCS_EditorStrings.LogoIcon, out myiCanScriptMediumLogo);            
        }
        if(myAssistantLogo == null) {
            iCS_TextureCache.GetIcon(iCS_EditorStrings.HelpMediumIcon, out myAssistantLogo);                   
        }
    }
}
