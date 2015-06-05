using UnityEngine;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
    
    public static class iCS_EditorConfig {
        // ======================================================================
        // Constants
        // ----------------------------------------------------------------------
        public const float kInitialScale     = 1f;
        public const float kIconicSize       = 32f;
        public const float kIconicArea       = kIconicSize*kIconicSize;
        public const float kMinIconicSize    = 12f;
        public const float kMinIconicArea    = kMinIconicSize*kMinIconicSize;
        public const float kNodeCornerRadius = 8f;
        public const int   kLabelFontSize    = 11;
        public const int   kTitleFontSize    = 13;
        public const int   kSubTitleFontSize = (int)(0.8f*(float)kTitleFontSize);
        public const float kTitlePadding     = 4;
        public const float kNodeTitleHeight  = kTitleFontSize+kSubTitleFontSize+2*kTitlePadding;
        public const float kNodeTitleIconSize= 32f;
    	public const int   kHelpBoxHeight    = 85;
    	public const int   kHelpBoxWidth     = 420;
    
        // ----------------------------------------------------------------------
    	public const float NodeShadowSize= 5.0f;
	
        // ----------------------------------------------------------------------
        public const  float   PortRadius        = 7.05f;
        public const  float   PortDiameter      = 2.0f * PortRadius;
        public const  float   SelectedPortFactor= 1.5f;
        public static Vector2 PortSize;
        public const  float   kMinimumPortSeparation= SelectedPortFactor*PortDiameter;

        // ----------------------------------------------------------------------
        public const  float MarginSize = 2*PortDiameter;
        public const  float kPaddingSize= PortDiameter;

        // ----------------------------------------------------------------------
        public const float EditorWindowMarginSize = MarginSize;
        public const float EditorWindowPaddingSize= kPaddingSize;
        public const float EditorWindowToolbarHeight= 16.0f;
        public const float EditorWindowMinX= EditorWindowMarginSize;
        public const float EditorWindowMinY= EditorWindowMarginSize + EditorWindowToolbarHeight;

        // ======================================================================
    	// Release info
    	public static string VersionId {
    		get {
    			return ""+iCS_Config.MajorVersion+"."+iCS_Config.MinorVersion+"."+iCS_Config.BugFixVersion;	
    		}		
    	}
    	public static string VersionStr {
    		get {
    			return "Version "+VersionId;
    		}
    	}	
	
        // ======================================================================
        static iCS_EditorConfig() {
            PortSize= new Vector2(PortDiameter, PortDiameter);
        }
    }
}
