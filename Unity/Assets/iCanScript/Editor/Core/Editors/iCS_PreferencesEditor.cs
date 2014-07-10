using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Prefs=iCS_PreferencesController;

public class iCS_PreferencesEditor : iCS_EditorBase {
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const float kMargin      = 10.0f;
    const float kTitleHeight = 40.0f;
    const float kColumn1Width= 120.0f;
    const float kColumn2Width= 180.0f;
    const float kColumn3Width= 180.0f;
    const float kColumn1X    = 0;
    const float kColumn2X    = kColumn1X+kColumn1Width;
    const float kColumn3X    = kColumn2X+kColumn2Width;
	
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	GUIStyle    titleStyle= null;
	GUIStyle    selectionStyle= null;
	Texture2D	selectionBackground= null;
	int         selGridId= 0;
	string[]    selGridStrings= new string[]{
	    "Display Options",
	    "Canvas",
	    "Node Colors",
	    "Type Colors",
	    "Instance Wizard",
		"Software Update",
//        "Debug",
#if CODE_GENERATION_CONFIG
	    "Code Engineering"
#endif
	};
	
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    // -------------------------------------------------------------------------
#if CODE_GENERATION_CONFIG
    // -------------------------------------------------------------------------
	// Code Generation Config Accessor
    public static string CodeGenerationFolder {
        get { return EditorPrefs.GetString(kCodeGenerationFolderKey, kCodeGenerationFolder); }
        set {
            // Rename folder
            var current= CodeGenerationFolder;
            if(current == value) return;
            var path= "Assets/";
            if(String.IsNullOrEmpty(AssetDatabase.RenameAsset(path+current, value))) {
                EditorPrefs.SetString(kCodeGenerationFolderKey, value);                                
            }
        }
    }
    public static string BehaviourGenerationSubfolder {
        get { return EditorPrefs.GetString(kBehaviourGenerationSubFolderKey, kBehaviourGenerationSubFolder); }
        set {
            // Rename folder
            var current= BehaviourGenerationSubfolder;
            if(current == value) return;
            var path= "Assets/"+CodeGenerationFolder+"/";
            if(String.IsNullOrEmpty(AssetDatabase.RenameAsset(path+current, value))) {
                EditorPrefs.SetString(kBehaviourGenerationSubFolderKey, value);                
            }
        }
    }
    public static string CodeGenerationFilePrefix {
        get { return EditorPrefs.GetString(kCodeGenerationFilePrefixKey, kCodeGenerationFilePrefix); }
        set {
            var current= CodeGenerationFilePrefix;
            if(current == value) return;
            var codeGenerationRoot= "Assets/"+iCS_PreferencesEditor.CodeGenerationFolder;
            iCS_FileUtility.ChangeRecursivelyAssetsPrefixInDirectory(codeGenerationRoot, current, value);
            EditorPrefs.SetString(kCodeGenerationFilePrefixKey, value);
        }
    }
#endif
    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    public new void OnEnable() {
        base.OnEnable();
        title= "iCanScript Preferences";
        minSize= new Vector2(500f, 400f);
        maxSize= new Vector2(500f, 400f);
    }

    // ---------------------------------------------------------------------------------
	static GUIStyle largeLabelStyleCache= null;
    void RebuildStyles() {
        bool rebuildStyleNeeded= false;
        if(largeLabelStyleCache != EditorStyles.largeLabel) {
            rebuildStyleNeeded= true;
            largeLabelStyleCache= EditorStyles.largeLabel;
        }
		// Build title style
        if(titleStyle == null || rebuildStyleNeeded) {
            titleStyle= new GUIStyle(EditorStyles.largeLabel);                    
	        titleStyle.fontSize= 18;
	        titleStyle.fontStyle= FontStyle.Bold;
		}
		// Build selection grid style
        if(selectionStyle == null || rebuildStyleNeeded) {
            const int additionalPadding= 9;
            selectionStyle= new GUIStyle(EditorStyles.largeLabel);
	        selectionStyle.alignment= TextAnchor.MiddleRight;
	        selectionStyle.padding= new RectOffset(10,10,additionalPadding,additionalPadding);
	        selectionStyle.margin= new RectOffset(0,0,0,0);
			selectionStyle.overflow= new RectOffset(0,0,0,0);
			selectionStyle.border= new RectOffset(0,0,0,0);
			selectionStyle.fixedHeight= 2*additionalPadding+selectionStyle.lineHeight;
			var bkgColor= GUI.skin.settings.selectionColor;
			bkgColor.a= 1f;
			if(selectionBackground == null) {
				selectionBackground= new Texture2D(1,1);				
			}
			selectionBackground.SetPixel(0,0,bkgColor);
			selectionBackground.Apply();
			selectionStyle.onNormal.background= selectionBackground;
			selectionStyle.onNormal.textColor= Color.white;
		}
    }

	// =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    public new void OnGUI() {
        // Draw common stuff for all editors
        base.OnGUI();
        
		// Reset GUI alpha.
		GUI.color= Color.white;
		// Build GUI styles (in case they were changed by user).
        RebuildStyles();
        
        // Outline column 1 area
        Rect column1Rect= new Rect(0,-1,kColumn1Width,position.height+1);
        GUI.Box(column1Rect,"");

        // Show selection grid.
        float lineHeight= selectionStyle.fixedHeight;
        float gridHeight= lineHeight*selGridStrings.Length;
        selGridId= GUI.SelectionGrid(new Rect(0,kTitleHeight,kColumn1Width,gridHeight), selGridId, selGridStrings, 1, selectionStyle);

        // Show title
        if(selGridId >= 0 && selGridId < selGridStrings.Length) {
            string title= selGridStrings[selGridId];            
            GUI.Label(new Rect(kColumn2X+1.5f*kMargin,kMargin, kColumn2Width+kColumn3Width, kTitleHeight), title, titleStyle);
        }

        // Execute option specific panel.
        switch(selGridId) {
            case 0: DisplayOptions(); break;
            case 1: Canvas(); break;
            case 2: NodeColors(); break;
            case 3: TypeColors(); break;
            case 4: InstanceWizard(); break;
			case 5: SoftwareUpdate(); break;
//            case 6: DebugConfig(); break;
#if CODE_GENERATION_CONFIG
            case 7: CodeEngineering(); break;
#endif
            default: break;
        }

        // Show iCanScript version information.
        string version= iCS_EditorConfig.VersionStr;
        GUIContent versionContent= new GUIContent(version);
        Vector2 versionSize= GUI.skin.label.CalcSize(versionContent);
        float x= column1Rect.x+0.5f*(column1Rect.width-versionSize.x);
        float y= column1Rect.yMax-2.5f*versionSize.y;
        Rect pos= new Rect(x, y, versionSize.x, versionSize.y);
        GUI.Label(pos, versionContent);
        pos.y+= versionSize.y;
        string buildDate= iCS_BuildInfo.kBuildDateStr;
        GUIContent buildDateContent= new GUIContent(buildDate);
        Vector2 buildDateSize= GUI.skin.label.CalcSize(buildDateContent);        
        pos.x= column1Rect.x+0.5f*(column1Rect.width-buildDateSize.x);
		pos.width= buildDateSize.x;
		pos.height= buildDateSize.y;
        GUI.Label(pos, buildDateContent);

        // Show product icon
        var logoWidth= 64f;
        var logoHeight= 64f;
        Texture2D iCanScriptLogo= null;
        if(iCS_TextureCache.GetTexture(iCS_EditorStrings.LogoIcon, out iCanScriptLogo)) {
            Rect r= new Rect(0.5f*(kColumn1Width-logoWidth), position.height-logoHeight-10f-2f*versionSize.y, logoWidth, logoHeight);
            GUI.DrawTexture(r, iCanScriptLogo);
        }        		
	}
    // ---------------------------------------------------------------------------------
    void DisplayOptions() {
        // Draw column 2
        Rect p= new Rect(kColumn2X+kMargin, kMargin+kTitleHeight, kColumn2Width, 20.0f);
        GUI.Label(p, "Animation Controls", EditorStyles.boldLabel);
        Rect[] pos= new Rect[9];
        pos[0]= new Rect(p.x, p.yMax, p.width, p.height);
        for(int i= 1; i < 7; ++i) {
            pos[i]= pos[i-1];
            pos[i].y= pos[i-1].yMax;
        }
        pos[6].y+= pos[7].height;
        GUI.Label(pos[8], "Runtime Configuration", EditorStyles.boldLabel);
        pos[8].y+= pos[8].height;
        for(int i= 7; i < pos.Length; ++i) {
            pos[i]= pos[i-1];
            pos[i].y= pos[i-1].yMax;            
        }
        GUI.Label(pos[0], "Animation Enabled");
        GUI.Label(pos[1], "Animation Time");
        GUI.Label(pos[2], "Scroll Speed");
        GUI.Label(pos[3], "Edge Scroll Speed (pixels)");
        GUI.Label(pos[4], "Inverse Zoom");
        GUI.Label(pos[5], "Zoom Speed");
        GUI.Label(pos[6], "Show Runtime Values");
        GUI.Label(pos[7], "Refresh Period (seconds)");
		GUI.Label(pos[8], "Show Frame Id");
        
        // Draw Column 3
        for(int i= 0; i < pos.Length; ++i) {
            pos[i].x+= kColumn2Width;
            pos[i].width= kColumn3Width;
        }
        Prefs.IsAnimationEnabled= EditorGUI.Toggle(pos[0], Prefs.IsAnimationEnabled);
        EditorGUI.BeginDisabledGroup(Prefs.IsAnimationEnabled==false);
        Prefs.AnimationTime= EditorGUI.FloatField(pos[1], Prefs.AnimationTime);
        EditorGUI.EndDisabledGroup();
        Prefs.ScrollSpeed= EditorGUI.FloatField(pos[2], Prefs.ScrollSpeed);
        Prefs.EdgeScrollSpeed= EditorGUI.FloatField(pos[3], Prefs.EdgeScrollSpeed);
        Prefs.InverseZoom= EditorGUI.Toggle(pos[4], Prefs.InverseZoom);
        Prefs.ZoomSpeed= EditorGUI.FloatField(pos[5], Prefs.ZoomSpeed);
        Prefs.ShowRuntimePortValue= EditorGUI.Toggle(pos[6], Prefs.ShowRuntimePortValue);
        Prefs.PortValueRefreshPeriod= EditorGUI.FloatField(pos[7], Prefs.PortValueRefreshPeriod);
		Prefs.ShowRuntimeFrameId= EditorGUI.Toggle(pos[8], Prefs.ShowRuntimeFrameId);

        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
            Prefs.ResetIsAnimationEnabled();
            Prefs.ResetAnimationTime();
            Prefs.ResetScrollSpeed();
            Prefs.ResetEdgeScrollSpeed();
            Prefs.ResetInverseZoom();
            Prefs.ResetZoomSpeed();
            Prefs.ResetShowRuntimePortValue();
            Prefs.ResetPortValueRefreshPeriod();
			Prefs.ResetShowRuntimeFrameId();   
        }
		// Ask to repaint visual editor if an option has changed.
		if(GUI.changed) {
			iCS_EditorController.RepaintVisualEditor();
		}
    }
    // ---------------------------------------------------------------------------------
    void Canvas() {
        // Column 2
        Rect[] pos= new Rect[3];
        pos[0]= new Rect(kColumn2X+kMargin, kMargin+kTitleHeight, kColumn2Width, 20.0f);
        for(int i= 1; i < pos.Length; ++i) {
            pos[i]= pos[i-1];
            pos[i].y= pos[i-1].yMax;
        }
        GUI.Label(pos[0], "Grid Spacing");
        GUI.Label(pos[1], "Grid Color");
        GUI.Label(pos[2], "Background Color");
        // Draw Column 3
        for(int i= 0; i < pos.Length; ++i) {
            pos[i].x+= kColumn2Width;
            pos[i].width= kColumn3Width;
        }
        Prefs.GridSpacing= EditorGUI.FloatField(pos[0], Prefs.GridSpacing);
        Prefs.GridColor= EditorGUI.ColorField(pos[1], Prefs.GridColor);
        Prefs.CanvasBackgroundColor= EditorGUI.ColorField(pos[2], Prefs.CanvasBackgroundColor);
		
        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
            Prefs.ResetGridSpacing();
            Prefs.ResetGridColor();
            Prefs.ResetCanvasBackgroundColor();
        }
		
		// Ask to repaint visual editor if an option has changed.
		if(GUI.changed) {
			iCS_EditorController.RepaintVisualEditor();
		}
    }
    // ---------------------------------------------------------------------------------
    void NodeColors() {
        // Column 2
        Rect[] pos= new Rect[12];
        pos[0]= new Rect(kColumn2X+kMargin, kMargin+kTitleHeight, kColumn2Width, 20.0f);
        for(int i= 1; i < pos.Length; ++i) {
            pos[i]= pos[i-1];
            pos[i].y= pos[i-1].yMax;
        }
        GUI.Label(pos[0],  "Title");
        GUI.Label(pos[1],  "Label");
        GUI.Label(pos[2],  "Value");
        GUI.Label(pos[3],  "Package");
        GUI.Label(pos[4],  "Function");
        GUI.Label(pos[5],  "Object Constructor");
        GUI.Label(pos[6],  "Object Instance");
        GUI.Label(pos[7],  "State");
        GUI.Label(pos[8],  "Entry State");
        GUI.Label(pos[9], "Message Handler");
        GUI.Label(pos[10], "Background");
        GUI.Label(pos[11], "Selected Background");

        // Draw Column 3
        for(int i= 0; i < pos.Length; ++i) {
            pos[i].x+= kColumn2Width;
            pos[i].width= kColumn3Width;
        }
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
        Prefs.BackgroundColor= EditorGUI.ColorField(pos[10], Prefs.BackgroundColor);
        Prefs.SelectedBackgroundColor= EditorGUI.ColorField(pos[11], Prefs.SelectedBackgroundColor);
        
        // Reset Button
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
            Prefs.ResetBackgroundColor();
            Prefs.ResetSelectedBackgroundColor();
        }        
		
		// Ask to repaint visual editor if an option has changed.
		if(GUI.changed) {
			iCS_EditorController.RepaintVisualEditor();
		}
    }
    // ---------------------------------------------------------------------------------
    void TypeColors() {
        // Column 2
        Rect[] pos= new Rect[9];
        pos[0]= new Rect(kColumn2X+kMargin, kMargin+kTitleHeight, kColumn2Width, 20.0f);
        for(int i= 1; i < pos.Length; ++i) {
            pos[i]= pos[i-1];
            pos[i].y= pos[i-1].yMax;
        }
        GUI.Label(pos[0], "Bool");
        GUI.Label(pos[1], "Int");
        GUI.Label(pos[2], "Float");
        GUI.Label(pos[3], "String");
        GUI.Label(pos[4], "Vector2");
        GUI.Label(pos[5], "Vector3");
        GUI.Label(pos[6], "Vector4");
        GUI.Label(pos[7], "Game Object");
        GUI.Label(pos[8], "Default");

        // Draw Column 3
        for(int i= 0; i < pos.Length; ++i) {
            pos[i].x+= kColumn2Width;
            pos[i].width= kColumn3Width;
        }
        Prefs.BoolTypeColor      = EditorGUI.ColorField(pos[0], Prefs.BoolTypeColor);
        Prefs.IntTypeColor       = EditorGUI.ColorField(pos[1], Prefs.IntTypeColor);
        Prefs.FloatTypeColor     = EditorGUI.ColorField(pos[2], Prefs.FloatTypeColor);
        Prefs.StringTypeColor    = EditorGUI.ColorField(pos[3], Prefs.StringTypeColor);
        Prefs.Vector2TypeColor   = EditorGUI.ColorField(pos[4], Prefs.Vector2TypeColor);
        Prefs.Vector3TypeColor   = EditorGUI.ColorField(pos[5], Prefs.Vector3TypeColor);
        Prefs.Vector4TypeColor   = EditorGUI.ColorField(pos[6], Prefs.Vector4TypeColor);
        Prefs.GameObjectTypeColor= EditorGUI.ColorField(pos[7], Prefs.GameObjectTypeColor);
        Prefs.DefaultTypeColor   = EditorGUI.ColorField(pos[8], Prefs.DefaultTypeColor);
        
        // Reset Button
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
		
		// Ask to repaint visual editor if an option has changed.
		if(GUI.changed) {
			iCS_EditorController.RepaintVisualEditor();
		}
    }
    // ---------------------------------------------------------------------------------
    void InstanceWizard() {
        // Header
        Rect p= new Rect(kColumn2X+kMargin, kMargin+kTitleHeight, kColumn2Width, 20.0f);
        GUI.Label(p, "Auto-Creation", EditorStyles.boldLabel);
        Rect p2= new Rect(kColumn3X+kMargin, p.y, 0.5f*kColumn3Width, 20f);
        GUI.Label(p2, "In", EditorStyles.boldLabel);
        p2.x+= 40f;
        GUI.Label(p2, "Out", EditorStyles.boldLabel);
        
        // Column 2
        Rect[] pos= new Rect[5];
        pos[0]= new Rect(p.x, p.yMax, p.width, p.height);
        for(int i= 1; i < pos.Length; ++i) {
            pos[i]= pos[i-1];
            pos[i].y= pos[i-1].yMax;
        }
        GUI.Label(pos[0], "Instance Port");
        GUI.Label(pos[1], "Instance Fields");
        GUI.Label(pos[2], "Class Fields");
        GUI.Label(pos[3], "Instance Properties");
        GUI.Label(pos[4], "Class Properties");
        
        // Draw Column 3
        for(int i= 0; i < pos.Length; ++i) {
            pos[i].x+= kColumn2Width;
            pos[i].width= 40f;
        }
        Prefs.InstanceAutocreateInThis           = EditorGUI.Toggle(pos[0], Prefs.InstanceAutocreateInThis);
        Prefs.InstanceAutocreateInFields         = EditorGUI.Toggle(pos[1], Prefs.InstanceAutocreateInFields);
        Prefs.InstanceAutocreateInClassFields    = EditorGUI.Toggle(pos[2], Prefs.InstanceAutocreateInClassFields);
        Prefs.InstanceAutocreateInProperties     = EditorGUI.Toggle(pos[3], Prefs.InstanceAutocreateInProperties);
        Prefs.InstanceAutocreateInClassProperties= EditorGUI.Toggle(pos[4], Prefs.InstanceAutocreateInClassProperties);
        for(int i= 0; i < pos.Length; ++i) {
            pos[i].x+= 45f;
        }
        Prefs.InstanceAutocreateOutFields         = EditorGUI.Toggle(pos[1], Prefs.InstanceAutocreateOutFields);
        Prefs.InstanceAutocreateOutClassFields    = EditorGUI.Toggle(pos[2], Prefs.InstanceAutocreateOutClassFields);
        Prefs.InstanceAutocreateOutProperties     = EditorGUI.Toggle(pos[3], Prefs.InstanceAutocreateOutProperties);
        Prefs.InstanceAutocreateOutClassProperties= EditorGUI.Toggle(pos[4], Prefs.InstanceAutocreateOutClassProperties);
        
        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
            Prefs.ResetInstanceAutocreateInThis();
            Prefs.ResetInstanceAutocreateInFields();
            Prefs.ResetInstanceAutocreateInClassFields();
            Prefs.ResetInstanceAutocreateInProperties();
            Prefs.ResetInstanceAutocreateInClassProperties();
            Prefs.ResetInstanceAutocreateOutFields();
            Prefs.ResetInstanceAutocreateOutClassFields();
            Prefs.ResetInstanceAutocreateOutProperties();
            Prefs.ResetInstanceAutocreateOutClassProperties();
        }        
    }
    // ---------------------------------------------------------------------------------
	void SoftwareUpdate() {
        // Column 2
        Rect[] pos= new Rect[3];
        pos[0]= new Rect(kColumn2X+kMargin, kMargin+kTitleHeight, kColumn2Width, 20.0f);
        for(int i= 1; i < pos.Length; ++i) {
            pos[i]= pos[i-1];
            pos[i].y= pos[i-1].yMax;
        }
        GUI.Label(pos[0], "Watch for Updates");
        GUI.Label(pos[1], "Verification Internal");
        GUI.Label(pos[2], "Skipped Version");
        
        // Draw Column 3
        for(int i= 0; i < pos.Length; ++i) {
            pos[i].x+= kColumn2Width;
            pos[i].width= kColumn3Width;
        }
		Prefs.SoftwareUpdateWatchEnabled  = EditorGUI.Toggle(pos[0], Prefs.SoftwareUpdateWatchEnabled);
		Prefs.SoftwareUpdateInterval      = (iCS_UpdateInterval)EditorGUI.EnumPopup(pos[1], Prefs.SoftwareUpdateInterval);
		pos[2].width*= 0.5f;
		Prefs.SoftwareUpdateSkippedVersion= EditorGUI.TextField(pos[2], Prefs.SoftwareUpdateSkippedVersion);
		pos[2].x+= pos[2].width;
		if(GUI.Button(pos[2], "Clear")) {
			Prefs.ResetSoftwareUpdateSkippedVersion();			
		}
		
        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
			Prefs.ResetSoftwareUpdateWatchEnabled();
			Prefs.ResetSoftwareUpdateInterval();
			Prefs.ResetSoftwareUpdateSkippedVersion();
        }        
	}
//    // ---------------------------------------------------------------------------------
//	void DebugConfig() {
//        // Column 2
//        Rect[] pos= new Rect[1];
//        pos[0]= new Rect(kColumn2X+kMargin, kMargin+kTitleHeight, kColumn2Width, 20.0f);
//        for(int i= 1; i < pos.Length; ++i) {
//            pos[i]= pos[i-1];
//            pos[i].y= pos[i-1].yMax;
//        }
//        GUI.Label(pos[0], "Enable Trace");
//        
//        // Draw Column 3
//        for(int i= 0; i < pos.Length; ++i) {
//            pos[i].x+= kColumn2Width;
//            pos[i].width= kColumn3Width;
//        }
//		Prefs.DebugTrace  = EditorGUI.Toggle(pos[0], Prefs.DebugTrace);
//		
//        // Reset Button
//        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
//			Prefs.ResetDebugTrace();
//        }        
//	}
#if CODE_GENERATION_CONFIG
    // ---------------------------------------------------------------------------------
	void CodeEngineering() {
        // Column 2
        Rect[] pos= new Rect[3];
        pos[0]= new Rect(kColumn2X+kMargin, kMargin+kTitleHeight, kColumn2Width, 20.0f);
        for(int i= 1; i < pos.Length; ++i) {
            pos[i]= pos[i-1];
            pos[i].y= pos[i-1].yMax;
        }
        GUI.Label(pos[0], "File Prefix");
        GUI.Label(pos[1], "Root Folder");
        GUI.Label(pos[2], "Behaviour Subfolder");

        // Draw Column 3
        for(int i= 0; i < pos.Length; ++i) {
            pos[i].x+= kColumn2Width;
            pos[i].width= kColumn3Width;
        }
        CodeGenerationFilePrefix    = EditorGUI.TextField(pos[0], CodeGenerationFilePrefix);
        CodeGenerationFolder        = EditorGUI.TextField(pos[1], CodeGenerationFolder);
        BehaviourGenerationSubfolder= EditorGUI.TextField(pos[2], BehaviourGenerationSubfolder);
        
        // Reset Button
        if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Use Defaults")) {
            CodeGenerationFilePrefix    = kCodeGenerationFilePrefix;
            CodeGenerationFolder        = kCodeGenerationFolder;
            BehaviourGenerationSubfolder= kBehaviourGenerationSubFolder;
        }        	    
	}
#endif
	
	// =================================================================================
    // Helpers.
    // ---------------------------------------------------------------------------------
    public static string RemoveProductPrefix(string name) {
		if(string.IsNullOrEmpty(name)) return "";
        if(name.StartsWith(iCS_Config.ProductPrefix)) {
            return name.Substring(iCS_Config.ProductPrefix.Length);
        }
        return name;
    }
    // -------------------------------------------------------------------------
    Texture2D LoadIconFromPath(string iconPath) {
        return iCS_TextureCache.GetIcon(iconPath);
    }
    // -------------------------------------------------------------------------
    Texture2D LoadIconFromGUID(string iconGuid) {
        return iCS_TextureCache.GetIconFromGUID(iconGuid);
    }
}
