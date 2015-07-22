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

    public abstract class ConfigEditorBase : iCS_EditorBase {
        // =================================================================================
        // CONSTANTS
        // ---------------------------------------------------------------------------------
        protected const float kMargin      = 10.0f;
        protected const float kTitleHeight = 40.0f;
        protected const float kColumn1Width= 120.0f;
        protected const float kColumn2Width= 155.0f;
        protected const float kColumn3Width= 205.0f;
        protected const float kColumn1X    = 0;
        protected const float kColumn2X    = kColumn1X+kColumn1Width;
        protected const float kColumn3X    = kColumn2X+kColumn2Width;
    	
        // =================================================================================
        // FIELDS
        // ---------------------------------------------------------------------------------
	    TimedAction	ourRepaintTimer    = null;
    	GUIStyle    titleStyle         = null;
    	GUIStyle    selectionStyle     = null;
    	Texture2D	selectionBackground= null;
    	int         selGridId          = 0;
    	string[]    optionStrings      = null;
    	
        // =================================================================================
        // ABSTRACT INTERFACES
        // ---------------------------------------------------------------------------------
        protected abstract string   GetTitle();
        protected abstract string[] GetMainSelectionGridStrings();
        protected abstract void     ProcessSelection(int selection);
        
        // =================================================================================
        // Activation/Deactivation.
        // ---------------------------------------------------------------------------------
        public new void OnEnable() {
            base.OnEnable();
            Texture2D iCanScriptLogo= null;
            TextureCache.GetTexture(iCS_EditorStrings.TitleLogoIcon, out iCanScriptLogo);
            titleContent= new GUIContent(GetTitle(), iCanScriptLogo);
            optionStrings= GetMainSelectionGridStrings();
            minSize= new Vector2(500f, 400f);
            maxSize= new Vector2(500f, 400f);
        }
        public new void OnDisable() {
            base.OnDisable();
            StopRepaintTimer();
        }
    
        // =================================================================================
        // REPAINT TIMER UTILITY
        // ---------------------------------------------------------------------------------
        /// Starts the repaint time.
        void StartRepaintTimer() {
            if(ourRepaintTimer == null) {
                ourRepaintTimer= TimerService.CreateTimedAction(0.06f, Repaint, /*isLooping=*/true);
                ourRepaintTimer.Schedule();                
            }
        }
        // ---------------------------------------------------------------------------------
        /// Stops the repaint time.
        void StopRepaintTimer() {
            if(ourRepaintTimer != null) {
                ourRepaintTimer.Stop();
                ourRepaintTimer= null;                
            }
        }
        
        // =================================================================================
        // GUI STYLE CREATION
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
        // MAIN WINDOW
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
            float gridHeight= lineHeight*optionStrings.Length;
            selGridId= GUI.SelectionGrid(new Rect(0,kTitleHeight,kColumn1Width,gridHeight), selGridId, optionStrings, 1, selectionStyle);
    
            // Show title
            if(selGridId >= 0 && selGridId < optionStrings.Length) {
                string title= optionStrings[selGridId];            
                GUI.Label(new Rect(kColumn2X+1.5f*kMargin,kMargin, kColumn2Width+kColumn3Width, kTitleHeight), title, titleStyle);
            }
    
            // Execute option specific panel.
            GUI.skin.label.richText= true;
            ProcessSelection(selGridId);
    
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
            if(TextureCache.GetTexture(iCS_EditorStrings.LogoIcon, out iCanScriptLogo)) {
                Rect r= new Rect(0.5f*(kColumn1Width-logoWidth), position.height-logoHeight-10f-2f*versionSize.y, logoWidth, logoHeight);
                GUI.DrawTexture(r, iCanScriptLogo);
            }        		
    	}

    	// =================================================================================
        // DISPLAY UTILITIES
        // ---------------------------------------------------------------------------------
        /// Returns the positions for each line of the label column.
        ///
        /// @param numberOfLines The number of lines needed.
        /// @return Array of positions for each line.
        ///
        protected Rect[] GetLabelColumnPositions(int numberOfLines) {
            var pos= new Rect[numberOfLines];
            pos[0]= new Rect(kColumn2X+kMargin, kMargin+kTitleHeight, kColumn2Width, 20.0f);
            for(int i= 1; i < numberOfLines; ++i) {
                pos[i]= pos[i-1];
                pos[i].y= pos[i-1].yMax;
            }
            return pos;
        }
        // ---------------------------------------------------------------------------------
        /// Returns the positions for each line of the value column.
        ///
        /// @param numberOfLines The number of lines needed.
        /// @return Array of positions for each line.
        ///
        protected Rect[] GetValueColumnPositions(int numberOfLines) {
            var hOffset= GetValueHorizontalOffset();
            var pos= new Rect[numberOfLines];
            pos[0]= new Rect(kColumn3X+kMargin+hOffset, kMargin+kTitleHeight, kColumn3Width-hOffset, 20.0f);
            for(int i= 1; i < numberOfLines; ++i) {
                pos[i]= pos[i-1];
                pos[i].y= pos[i-1].yMax;
            }
            return pos;            
        }
        // =================================================================================
        /// Allows the child class to define an horizontal offset for the value column.
        ///
        /// @return The horizontal offset.
        ///
        protected virtual float GetValueHorizontalOffset() {
            return 0f;
        }
        // =================================================================================
        /// Changes the active selection.
        ///
        /// @param selectionName String with the selection string.
        /// @return _true_ if selection has been changed; _false_ otherwise. 
        ///
        public bool ChangeSelection(string selectionStr) {
            for(int i= 0; i < optionStrings.Length; ++i) {
                if(optionStrings[i] == selectionStr) {
                    selGridId= i;
                    return true;
                }
            }
            return false;
        }
        
    	// =================================================================================
        // ERROR DSIPLAY UTILITIES
        // -------------------------------------------------------------------------
        /// Displays an error message at the given value position.
        ///
        /// @param valuePosition The position of the value in error.
        /// @param message The error message to be displayed.
        ///
        protected void DisplayError(Rect valuePosition, string message) {
            StartRepaintTimer();
            var r= ErrorRect(valuePosition); 
            ErrorController.DisplayErrorMessage(position, r, message);                
        }
        // -------------------------------------------------------------------------
        Rect ErrorRect(Rect r) {
            var size= r.height;
            var x= r.x-size;
            var y= r.y+0.5f*size;
            return Math3D.BuildRectCenteredAt(new Vector2(x, y), size, size);            
        }

    }

}
