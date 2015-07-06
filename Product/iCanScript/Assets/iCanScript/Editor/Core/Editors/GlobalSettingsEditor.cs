using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Internal.Engine;
using TimedAction= iCanScript.Internal.Prelude.TimerService.TimedAction;

namespace iCanScript.Internal.Editor {
    using Prefs= PreferencesController;

    public class GlobalSettingsEditor : ConfigEditorBase {
        // =================================================================================
        // Fields
        // ---------------------------------------------------------------------------------
    	string[] myOptionStrings= new string[]{
    	    "Display Options",
    	    "Canvas",
    	    "Node Colors",
    	    "Type Colors",
    		"Software Update"
    	};
    	
        // =================================================================================
        // INITIALIZATION
        // ---------------------------------------------------------------------------------
        public static void Init(iCS_IStorage iStorage) {
            var editor= EditorWindow.CreateInstance<GlobalSettingsEditor>();
            editor.ShowUtility();
        }

        // =================================================================================
        // INTERFACES TO BE PROVIDED
        // ---------------------------------------------------------------------------------
        protected override string   GetTitle() {
            return "iCanScript Global Setting";
        }
        protected override string[] GetMainSelectionGridStrings() {
            return myOptionStrings;
        }
        protected override void     ProcessSelection(int selection) {
            // Execute option specific panel.
            switch(selection) {
                case 0: DisplayOptions(); break;
                case 1: Canvas(); break;
                case 2: NodeColors(); break;
                case 3: TypeColors(); break;
    			case 4: SoftwareUpdate(); break;
                default: break;
            }
        }
        

    	// =================================================================================
        // DISPLAY OPTION PANEL
        // ---------------------------------------------------------------------------------
        void DisplayOptions() {
            // -- Label column --
            var pos= GetLabelColumnPositions(7);
            GUI.Label(pos[0], "Animation Controls", EditorStyles.boldLabel);
            GUI.Label(pos[1], "Animation Enabled");
            GUI.Label(pos[2], "Animation Time");
            GUI.Label(pos[3], "Scroll Speed");
            GUI.Label(pos[4], "Edge Scroll Speed (pixels)");
            GUI.Label(pos[5], "Inverse Zoom");
            GUI.Label(pos[6], "Zoom Speed");
            
            // -- Value column --
            pos= GetValueColumnPositions(7);
            Prefs.IsAnimationEnabled= EditorGUI.Toggle(pos[1], Prefs.IsAnimationEnabled);
            EditorGUI.BeginDisabledGroup(Prefs.IsAnimationEnabled==false);
            Prefs.AnimationTime= EditorGUI.FloatField(pos[2], Prefs.AnimationTime);
            EditorGUI.EndDisabledGroup();
            Prefs.ScrollSpeed= EditorGUI.FloatField(pos[3], Prefs.ScrollSpeed);
            Prefs.EdgeScrollSpeed= EditorGUI.FloatField(pos[4], Prefs.EdgeScrollSpeed);
            Prefs.InverseZoom= EditorGUI.Toggle(pos[5], Prefs.InverseZoom);
            Prefs.ZoomSpeed= EditorGUI.FloatField(pos[6], Prefs.ZoomSpeed);
    
            // -- Reset button --
            if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
                Prefs.ResetIsAnimationEnabled();
                Prefs.ResetAnimationTime();
                Prefs.ResetScrollSpeed();
                Prefs.ResetEdgeScrollSpeed();
                Prefs.ResetInverseZoom();
                Prefs.ResetZoomSpeed();
            }
    		// -- Repaint visual editor if option has changed --
    		if(GUI.changed) {
    			iCS_EditorController.RepaintVisualEditor();
    		}
        }
        // ---------------------------------------------------------------------------------
        void Canvas() {
            // -- Label column --
            Rect[] pos= GetLabelColumnPositions(4);
            GUI.Label(pos[0], "Grid Spacing");
            GUI.Label(pos[1], "Minor Grid Color");
            GUI.Label(pos[2], "Major Grid Color");
            GUI.Label(pos[3], "Background Color");
            // -- Value column --
            pos= GetValueColumnPositions(4);
            Prefs.GridSpacing= EditorGUI.FloatField(pos[0], Prefs.GridSpacing);
            Prefs.MinorGridColor= EditorGUI.ColorField(pos[1], Prefs.MinorGridColor);
            Prefs.MajorGridColor= EditorGUI.ColorField(pos[2], Prefs.MajorGridColor);
            Prefs.CanvasBackgroundColor= EditorGUI.ColorField(pos[3], Prefs.CanvasBackgroundColor);
    		
            // -- Reset button --
            if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
                Prefs.ResetGridSpacing();
                Prefs.ResetGridColor();
                Prefs.ResetCanvasBackgroundColor();
            }
    		
    		// -- Repaint visual editor if option has changed --
    		if(GUI.changed) {
    			iCS_EditorController.RepaintVisualEditor();
    		}
        }
        // ---------------------------------------------------------------------------------
        void NodeColors() {
            // -- Label column --
            Rect[] pos= GetLabelColumnPositions(13);
            GUI.Label(pos[0],  "Title");
            GUI.Label(pos[1],  "Label");
            GUI.Label(pos[2],  "Value");
            GUI.Label(pos[3],  "Package");
            GUI.Label(pos[4],  "Function");
            GUI.Label(pos[5],  "Object Constructor");
            GUI.Label(pos[6],  "Object Instance");
            GUI.Label(pos[7],  "State");
            GUI.Label(pos[8],  "Entry State");
            GUI.Label(pos[9],  "Message Handler");
            GUI.Label(pos[10],  "User Function");
            GUI.Label(pos[11], "Background");
            GUI.Label(pos[12], "Selected Background");
    
            // -- Value column --
            pos= GetValueColumnPositions(13);
            Prefs.NodeTitleColor= EditorGUI.ColorField(pos[0], Prefs.NodeTitleColor);
            Prefs.NodeLabelColor= EditorGUI.ColorField(pos[1], Prefs.NodeLabelColor);
            Prefs.NodeValueColor= EditorGUI.ColorField(pos[2], Prefs.NodeValueColor);
            Prefs.PackageNodeColor= EditorGUI.ColorField(pos[3], Prefs.PackageNodeColor);
            Prefs.FunctionNodeColor= EditorGUI.ColorField(pos[4], Prefs.FunctionNodeColor);
            Prefs.ConstructorNodeColor= EditorGUI.ColorField(pos[5], Prefs.ConstructorNodeColor);
            Prefs.InstanceNodeColor= EditorGUI.ColorField(pos[6], Prefs.InstanceNodeColor);
            Prefs.StateNodeColor= EditorGUI.ColorField(pos[7], Prefs.StateNodeColor);
            Prefs.EntryStateNodeColor= EditorGUI.ColorField(pos[8], Prefs.EntryStateNodeColor);
            Prefs.MessageNodeColor= EditorGUI.ColorField(pos[9], Prefs.MessageNodeColor);
            Prefs.UserFunctionNodeColor= EditorGUI.ColorField(pos[10], Prefs.UserFunctionNodeColor);
            Prefs.BackgroundColor= EditorGUI.ColorField(pos[11], Prefs.BackgroundColor);
            Prefs.SelectedBackgroundColor= EditorGUI.ColorField(pos[12], Prefs.SelectedBackgroundColor);
            
            // -- Reset button --
            if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
                Prefs.ResetNodeTitleColor();
                Prefs.ResetNodeLabelColor();
                Prefs.ResetNodeValueColor();
                Prefs.ResetPackageNodeColor();
                Prefs.ResetFunctionNodeColor();
                Prefs.ResetConstructorNodeColor();
                Prefs.ResetInstanceNodeColor();
                Prefs.ResetStateNodeColor();
                Prefs.ResetEntryStateNodeColor();
                Prefs.ResetMessageNodeColor();
                Prefs.ResetUserFunctionNodeColor();
                Prefs.ResetBackgroundColor();
                Prefs.ResetSelectedBackgroundColor();
            }        
    		
    		// -- Repaint visual editor if option has changed --
    		if(GUI.changed) {
    			iCS_EditorController.RepaintVisualEditor();
    		}
        }
        // ---------------------------------------------------------------------------------
        void TypeColors() {
            // -- Label column --
            Rect[] pos= GetLabelColumnPositions(9);
            GUI.Label(pos[0], "Bool");
            GUI.Label(pos[1], "Int");
            GUI.Label(pos[2], "Float");
            GUI.Label(pos[3], "String");
            GUI.Label(pos[4], "Vector2");
            GUI.Label(pos[5], "Vector3");
            GUI.Label(pos[6], "Vector4");
            GUI.Label(pos[7], "Game Object");
            GUI.Label(pos[8], "Default");
    
            // -- Value column --
            pos= GetValueColumnPositions(9);
            Prefs.BoolTypeColor      = EditorGUI.ColorField(pos[0], Prefs.BoolTypeColor);
            Prefs.IntTypeColor       = EditorGUI.ColorField(pos[1], Prefs.IntTypeColor);
            Prefs.FloatTypeColor     = EditorGUI.ColorField(pos[2], Prefs.FloatTypeColor);
            Prefs.StringTypeColor    = EditorGUI.ColorField(pos[3], Prefs.StringTypeColor);
            Prefs.Vector2TypeColor   = EditorGUI.ColorField(pos[4], Prefs.Vector2TypeColor);
            Prefs.Vector3TypeColor   = EditorGUI.ColorField(pos[5], Prefs.Vector3TypeColor);
            Prefs.Vector4TypeColor   = EditorGUI.ColorField(pos[6], Prefs.Vector4TypeColor);
            Prefs.GameObjectTypeColor= EditorGUI.ColorField(pos[7], Prefs.GameObjectTypeColor);
            Prefs.DefaultTypeColor   = EditorGUI.ColorField(pos[8], Prefs.DefaultTypeColor);
            
            // -- Reset button --
            if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
                Prefs.ResetBoolTypeColor();
    			Prefs.ResetIntTypeColor();
    			Prefs.ResetFloatTypeColor();
    			Prefs.ResetStringTypeColor();
    			Prefs.ResetVector2TypeColor();
    			Prefs.ResetVector3TypeColor();
    			Prefs.ResetVector4TypeColor();
    			Prefs.ResetGameObjectTypeColor();
                Prefs.ResetDefaultTypeColor();
    		}
    		
    		// -- Repaint visual editor if option has changed --
    		if(GUI.changed) {
    			iCS_EditorController.RepaintVisualEditor();
    		}
        }
        // ---------------------------------------------------------------------------------
    	void SoftwareUpdate() {
            // -- Label column --
            Rect[] pos= GetLabelColumnPositions(3);
            GUI.Label(pos[0], "Watch for Updates");
            GUI.Label(pos[1], "Verification Internal");
            GUI.Label(pos[2], "Skipped Version");
            
            // -- Value column --
            pos= GetValueColumnPositions(3);
    		Prefs.SoftwareUpdateWatchEnabled  = EditorGUI.Toggle(pos[0], Prefs.SoftwareUpdateWatchEnabled);
    		Prefs.SoftwareUpdateInterval      = (iCS_UpdateInterval)EditorGUI.EnumPopup(pos[1], Prefs.SoftwareUpdateInterval);
    		pos[2].width*= 0.5f;
    		Prefs.SoftwareUpdateSkippedVersion= EditorGUI.TextField(pos[2], Prefs.SoftwareUpdateSkippedVersion);
    		pos[2].x+= pos[2].width;
    		if(GUI.Button(pos[2], "Clear")) {
    			Prefs.ResetSoftwareUpdateSkippedVersion();			
    		}
    		
            // -- Reset button --
            if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
    			Prefs.ResetSoftwareUpdateWatchEnabled();
    			Prefs.ResetSoftwareUpdateInterval();
    			Prefs.ResetSoftwareUpdateSkippedVersion();
            }        
    	}
        
    }

}
