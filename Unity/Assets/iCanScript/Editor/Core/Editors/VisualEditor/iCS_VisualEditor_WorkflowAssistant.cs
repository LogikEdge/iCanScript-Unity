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
					ShowAssistantMessage("Click-and-Drag to Move Node");									
					if(editorObj.IsNameEditable) {
						ShowAssistantMessage("Double-Click to Edit Node Name");						
					}
					else {
						ShowAssistantMessage("WARNING: The Name of this Node cannot be edited");										
					}
					Repaint();
		            return;											
				}
				case iCS_PickPartEnum.FoldIcon: {
					if(editorObj.IsFoldedInLayout) {
			            ShowAssistantMessage("Click to Unfold Node");
					} else {
			            ShowAssistantMessage("Click to Fold Node");							
					}
					Repaint();
		            return;					
				}
				case iCS_PickPartEnum.MinimizeIcon: {
		            ShowAssistantMessage("Click to Iconize node");
					Repaint();
		            return;										
				}
				default: {
					if(editorObj.IsIconizedInLayout) {
			            ShowAssistantMessage("Double-Click to Open Node");
					}
					else {
						if(editorObj.IsKindOfPackage) {
				            ShowAssistantMessage("Right-Click to Add Package, State Chart, or Control Ports");
							ShowAssistantMessage("Drag from Library to Add functions & variables");
						}						
					}
					ShowAssistantMessage("Click-and-Drag to Move Node");									
					Repaint();
		            return;					
				}
			}
		}
		// -- Pointing at a port --
		if(editorObj.IsPort) {
			switch(pickInfo.PickedPart) {
				case iCS_PickPartEnum.Name: {
					if(editorObj.IsNameEditable) {
						ShowAssistantMessage("Double-Click to Edit Port Name");				
					}
					else {
						ShowAssistantMessage("WARNING: The Name of this Port cannot be edited");										
					}
					Repaint();
		            return;											
				}
				case iCS_PickPartEnum.Value: {
					ShowAssistantMessage("Double-Click to Edit Port Value");				
					Repaint();
		            return;																
				}
				default: {
					ShowAssistantMessage("Drag port on the Edge of the parent Node to Move it");
					ShowAssistantMessage("Drag port onto another port to Create a Data Flow");
					ShowAssistantMessage("Drag port on the Edge of a Package to Create an Interface");
					ShowAssistantMessage("Drag-and-Release port to Popup Quick Create Menu");						
					if(editorObj.IsInputPort & editorObj.ProducerPort == null) {
						ShowAssistantMessage("Drag object from the scene to Initialize the port");				
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
