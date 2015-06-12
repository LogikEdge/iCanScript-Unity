using UnityEngine;
using UnityEditor;
using System.Collections;
using iCanScript.Internal.Editor.CodeEngineering;

/*
    TODO: Should show runId in header bar.
*/
namespace iCanScript.Internal.Editor {
    using Prefs= PreferencesController;

    public partial class iCS_VisualEditor : iCS_EditorBase {
        // =======================================================================
        // Toolbar Constants
        // -----------------------------------------------------------------------
        const float kSliderSize= 60f;
        
        // -----------------------------------------------------------------------
    	void Toolbar() {
    		// No toolbar if editor snapshot without background requested.
    		if(iCS_DevToolsConfig.IsFrameWithoutBackground) return;
    		
    		// Build standard toolbar at top of editor window.
    		Rect r= iCS_ToolbarUtility.BuildToolbar(position.width, 0f);
    
    		// Insert an initial spacer.
    		float spacer= 8f;
    
    		// --------------
    		// LEFT TOOLBAR
            // -- Navigation History --
            var backwardNavigationIcon= iCS_BuiltinTextures.BackwardNavigationHistoryIcon();
            var forwardNavigationIcon= iCS_BuiltinTextures.ForwardNavigationHistoryIcon();
            var hasBackwardHistory= IStorage.HasBackwardNavigationHistory;
            float width= backwardNavigationIcon.width+spacer;
            if(iCS_ToolbarUtility.Button(ref r, width, hasBackwardHistory, backwardNavigationIcon, 0, 0)) {
                if(hasBackwardHistory) {
                    iCS_UserCommands.ReloadFromBackwardNavigationHistory(IStorage);
                }            
            }
            var hasForwardHistory= IStorage.HasForwardNavigationHistory;
            width= forwardNavigationIcon.width+spacer;
            if(iCS_ToolbarUtility.Button(ref r, width, hasForwardHistory, forwardNavigationIcon, 0, 0)) {
                if(hasForwardHistory) {
                    iCS_UserCommands.ReloadFromForwardNavigationHistory(IStorage);
                }            
            }

            // -- Generate & Delete C#
            if(iCS_ToolbarUtility.Button(ref r, 100, true, "Generate C#", spacer, 0)) {
                var codeGenerator= new CodeGenerator();
                codeGenerator.GenerateCodeFor(IStorage);
            }
            if(iCS_ToolbarUtility.Button(ref r, 100, true, "Delete C#", spacer, 0)) {
                var codeGenerator= new CodeGenerator();
                codeGenerator.DeleteGeneratedFilesFor(IStorage);
            }
    
            
            // -- Zoom factor --
            float newScale= iCS_ToolbarUtility.Slider(ref r, kSliderSize, Scale, 2f, 0.15f, spacer, spacer, true);
            iCS_ToolbarUtility.MiniLabel(ref r, "Zoom", 10f, 0, true);
    		if(Math3D.IsNotEqual(newScale, Scale)) {
                Vector2 pivot= ViewportToGraph(ViewportCenter);
                CenterAtWithScale(pivot, newScale);
    		}
            
            // -- Global Settings --
            if(iCS_ToolbarUtility.Button(ref r, 100, true, "Global Settings", 0, spacer, true)) {
                var editor= EditorWindow.CreateInstance<GlobalSettingsEditor>();
                editor.ShowUtility();
            }
            // -- Project Settings --
            if(iCS_ToolbarUtility.Button(ref r, 100, true, "Project Settings", 0, spacer, true)) {
                VisualScriptSettingsEditor.Init(IStorage);
            }
            // -- Visual Script Settings --
            if(iCS_ToolbarUtility.Button(ref r, 100, true, "Visual Script Settings", 0, spacer, true)) {
                VisualScriptSettingsEditor.Init(IStorage);
            }
            
    		// -- Show Display Root Node. --
            if(IStorage.DisplayRoot != IStorage.RootObject) {
        		IStorage.ShowDisplayRootNode= iCS_ToolbarUtility.Toggle(ref r, IStorage.ShowDisplayRootNode, spacer, spacer);
                iCS_ToolbarUtility.MiniLabel(ref r, "Show Root Node", 0,0);
                iCS_ToolbarUtility.Separator(ref r);                
            }
    
    		// --------------
    		// CENTER TOOLBAR
            // Show game object name in middle of toolbar.
    		var name= IStorage.TypeName;
            var baseType= CodeGenerationUtility.GetBaseType(IStorage);
            if(baseType != null && baseType != typeof(void)) {
                name+= " : "+baseType.Name;
            }
    		iCS_ToolbarUtility.CenteredTitle(ref r, name);
    
            // Trial information.
            ShowTrialInformation(ref r);
    
    		// Show scroll position
    		var scrollPositionAsStr= ScrollPosition.ToString();
    		var scrollPositionAsGUIContent= new GUIContent(scrollPositionAsStr);
    		var scrollPositionSize= EditorStyles.label.CalcSize(scrollPositionAsGUIContent);
    		r= new Rect(position.width-scrollPositionSize.x, position.height-scrollPositionSize.y,
    			        scrollPositionSize.x, scrollPositionSize.y);
    		GUI.Label(r, scrollPositionAsGUIContent);
    	}
        // -----------------------------------------------------------------------
        void ShowTrialInformation(ref Rect r) {
            if(EditionController.IsCommunityEdition) {
                // -- Display trial remaining information --
                GUIStyle style= EditorStyles.textField;
                style.richText= true;
                style.fontStyle= FontStyle.Bold;
                style.fontSize= (int)(style.fontSize*1.1f);
                var nbOfVisualScriptRemaining= EditionController.CommunityVisualScriptsRemaining;
                var nbOfNodesRemaining= EditionController.CommunityNodesRemaining;
                var visualScriptsMessage= "Visual Scripts Remaining= "+nbOfVisualScriptRemaining;
                var nodesMessage="Nodes Remaining= "+nbOfNodesRemaining;
                var percentVisualScriptsRemaining= EditionController.CommunityPercentVisualScriptsRemaining;
                var percentNodesRemaining        = EditionController.CommunityPercentNodesRemaining;
                if(percentVisualScriptsRemaining <= 0.4f) {
                    if(percentVisualScriptsRemaining <= 0.2f) {
                        visualScriptsMessage= "<color=red>"+visualScriptsMessage+"</color>";
                    }
                    else {
                        visualScriptsMessage= "<color=orange>"+visualScriptsMessage+"</color>";                    
                    }
                }
                if(percentNodesRemaining <= 0.4f) {
                    if(percentNodesRemaining <= 0.2f) {
                        nodesMessage= "<color=red>"+nodesMessage+"</color>";
                    }
                    else {
                        nodesMessage= "<color=orange>"+nodesMessage+"</color>";
                    }
                }
                var trialMessage= "COMMUNITY EDITION ==> "+visualScriptsMessage+" / "+nodesMessage+" <==";
                var trialMessageSize= style.CalcSize(new GUIContent(trialMessage));
                var x= 0.5f*(position.width-trialMessageSize.x);
                GUI.Label(new Rect(x,r.yMax,trialMessageSize.x, trialMessageSize.y), trialMessage, style);
    
    //            // -- Display Purchase information --
    //            if(nbOfVisualScriptRemaining < 0 || nbOfNodesRemaining < 0) {
    //                var toolbarHeight= iCS_ToolbarUtility.GetHeight();
    //                var width = position.width;
    //                var height= position.height-toolbarHeight;
    //                var purchaseRect= new Rect(0.15f*width, toolbarHeight+0.15f*height, 0.7f*width, 0.7f*height);
    //                GUI.Box(purchaseRect, "Please Purchase");
    //            }
            }        
        }
    }
}
