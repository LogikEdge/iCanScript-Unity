using UnityEngine;
using UnityEditor;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {

    public class iCS_AboutDialog : EditorWindow {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
    	const float kSpacer= 5f;
    	GUIStyle    h1     = null;

        // ======================================================================
        // Initialization/Teardown
        // ----------------------------------------------------------------------
        public void OnEnable() {
            Texture2D iCanScriptLogo= null;
            TextureCache.GetTexture(iCS_EditorStrings.TitleLogoIcon, out iCanScriptLogo);
            titleContent= new GUIContent("About iCanScript", iCanScriptLogo);
        }
        public void OnDisable() {
        }

        // ======================================================================
        // GUI Update
        // ----------------------------------------------------------------------
        public void OnGUI() {
    		// Init.
    		if(h1 == null) {
    			BuildStyles();
    		}

            // Show product icon
            var logoWidth= 64f;
            var logoHeight= 64f;
            Texture2D iCanScriptLogo= null;
            if(TextureCache.GetTexture(iCS_EditorStrings.LogoIcon, out iCanScriptLogo)) {
                Rect r= new Rect(kSpacer, kSpacer, logoWidth, logoHeight);
                GUI.DrawTexture(r, iCanScriptLogo);
            }

    		// Show product name
    		GUIContent title= new GUIContent("iCanScript");
    		var titleSize= h1.CalcSize(title);
    		Rect rTitle= new Rect(2f*kSpacer+logoWidth, kSpacer, titleSize.x, titleSize.y);
    		GUI.Label(rTitle, title, h1);

    		// Show version
            GUIContent versionContent= new GUIContent(iCS_EditorConfig.VersionStr);
            Vector2 versionSize= GUI.skin.label.CalcSize(versionContent);
            Rect rVersion= new Rect(2f*kSpacer+logoWidth, rTitle.yMax, versionSize.x, versionSize.y);
            GUI.Label(rVersion, versionContent);


    		// Column info
    		float column1X= kSpacer;
    		var editionTitle= new GUIContent("Edition: ");
    		var buildDateTitle= new GUIContent("Build Date: ");
    		var userLicenseTitle= new GUIContent("User License: ");
    		var buildDateTitleSize= GUI.skin.label.CalcSize(buildDateTitle);
    		var userLicenseTitleSize= GUI.skin.label.CalcSize(userLicenseTitle);
    		float column1Width= Math3D.Max(buildDateTitleSize.x,
    									   userLicenseTitleSize.x);
    		var labelHeight= buildDateTitleSize.y;

    		float column2X= column1X+column1Width+kSpacer;
    		float column2Width= position.width-column2X-kSpacer;

    		// Edition
    		var rEdition= new Rect(column1X, kSpacer+logoHeight+labelHeight, column2Width, labelHeight);
    		GUI.Label(rEdition, editionTitle);
    		rEdition.x= column2X; rEdition.width= column2Width;
    		GUI.Label(rEdition, EditionController.ToString());

    		// User License
    		var rUserLicense= new Rect(column1X, rEdition.yMax, column1Width, labelHeight);
    		GUI.Label(rUserLicense, userLicenseTitle);
    		var userLicense= LicenseController.LicenseAsString();
    		if(string.IsNullOrEmpty(userLicense)) {
    			userLicense= "None";
    		}
    		var userLicenseSize= GUI.skin.label.CalcSize(new GUIContent(userLicense));
    		rUserLicense.x= column2X; rUserLicense.width= column2Width;
    		GUI.Label(rUserLicense, userLicense);

    		// Build date
    		var rBuildDate= new Rect(column1X, rUserLicense.yMax, column2Width, labelHeight);
    		GUI.Label(rBuildDate, buildDateTitle);
    		GUIContent buildDate= new GUIContent(iCS_BuildInfo.kBuildDateStr);
    		rBuildDate.x= column2X; rBuildDate.width= column2Width;
    		GUI.Label(rBuildDate, buildDate);

    		// Disclamer
    		GUIContent copyright= new GUIContent("(c) copyright LogikEdge 2021.  All rights reserved.");
    		var copyrightSize= GUI.skin.label.CalcSize(copyright);
    		var rCopyright= new Rect(column1X, rBuildDate.yMax+copyrightSize.y, copyrightSize.x, copyrightSize.y);
    		GUI.Label(rCopyright, copyright);

    		var p= position;
    		p.width= Math3D.Max(copyrightSize.x+2f*kSpacer, column2X+userLicenseSize.x+kSpacer);
    		p.height= rCopyright.yMax+kSpacer;
    		position= p;
        }

        // ======================================================================
        // Helpers
        // ----------------------------------------------------------------------
    	void BuildStyles() {
    		// Build H1 style
    		h1= new GUIStyle(EditorStyles.boldLabel);
    		h1.fontSize= 24;
    	}
    }

}
