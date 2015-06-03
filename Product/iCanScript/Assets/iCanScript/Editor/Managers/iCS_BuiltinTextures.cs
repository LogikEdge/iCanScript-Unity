using UnityEngine;
using UnityEditor;
using System.Collections;
using iCanScript;
using Prefs= iCanScript.Internal.Editor.PreferencesController;

namespace iCanScript.Internal.Editor {
    
    public static class iCS_BuiltinTextures {
        // =================================================================================
        // Properties
        // ---------------------------------------------------------------------------------
        public static Texture2D InEndPortIcon           { get { return myInEndPortIcon; }}
        public static Texture2D OutEndPortIcon          { get { return myOutEndPortIcon; }}
        public static Texture2D InRelayPortIcon         { get { return myInRelayPortIcon; }}
        public static Texture2D OutRelayPortIcon        { get { return myOutRelayPortIcon; }}
        public static Texture2D InTriggerIcon           { get { return myInTriggerPortIcon; }}
        public static Texture2D OutTriggerIcon          { get { return myOutTriggerPortIcon; }}
        public static Texture2D InTransitionPortIcon    { get { return myInTransitionPortIcon; }}
        public static Texture2D OutTransitionPortIcon   { get { return myOutTransitionPortIcon; }}

        // =================================================================================
        // Accessors
        // ---------------------------------------------------------------------------------
        public static Texture2D MinimizeIcon(float scale) {
            if(Math3D.IsNotEqual(scale,myScale)) {
                myScale= scale;
                BuildScaleDependantTextures();
            }
            return myMinimizeIcon;
        }
        public static Texture2D MaximizeIcon(float scale) {
            if(Math3D.IsNotEqual(scale,myScale)) {
                myScale= scale;
                BuildScaleDependantTextures();
            }
            return myMaximizeIcon;
        }
        public static Texture2D FoldIcon(float scale) {
            if(Math3D.IsNotEqual(scale,myScale)) {
                myScale= scale;
                BuildScaleDependantTextures();
            }
            return myFoldIcon;
        }
        public static Texture2D UnfoldIcon(float scale) {
            if(Math3D.IsNotEqual(scale,myScale)) {
                myScale= scale;
                BuildScaleDependantTextures();
            }
            return myUnfoldIcon;
        }
        public static Texture2D BackwardNavigationHistoryIcon() {
            Validate();
            if(myBackwardNavigationHistoryIcon == null) {
                BuildNavigationHistoryIcons();
            }
            return myBackwardNavigationHistoryIcon;
        }
        public static Texture2D ForwardNavigationHistoryIcon() {
            Validate();
            if(myForwardNavigationHistoryIcon == null) {
                BuildNavigationHistoryIcons();
            }
            return myForwardNavigationHistoryIcon;
        }
    
        // =================================================================================
        // Constants
        // ---------------------------------------------------------------------------------
        public const int   kPortIconWidth   = 16;
        public const int   kPortIconHeight  = 16;
        public const int   kMinimizeIconSize= 16;
        public const int   kMaximizeIconSize= 32;
        public const int   kFoldIconSize    = 16;
        public const int   kUnfoldIconSize  = 16;

        // =================================================================================
        // Fields
        // ---------------------------------------------------------------------------------
        static float        myScale= 1f;
        static bool         myIsProSkin= false;
    	static Texture2D    myInEndPortIcon;
    	static Texture2D    myOutEndPortIcon;
    	static Texture2D    myInRelayPortIcon;
    	static Texture2D    myOutRelayPortIcon;
    	static Texture2D    myInTriggerPortIcon;
    	static Texture2D    myOutTriggerPortIcon;
    	static Texture2D    myInTransitionPortIcon;
    	static Texture2D    myOutTransitionPortIcon;
    	static Texture2D    myMinimizeIcon;
    	static Texture2D    myMaximizeIcon;
    	static Texture2D    myFoldIcon;
    	static Texture2D    myUnfoldIcon;
        static Texture2D    myBackwardNavigationHistoryIcon;
        static Texture2D    myForwardNavigationHistoryIcon;

        // =================================================================================
        // Polygons
        // ---------------------------------------------------------------------------------
        static Vector2[] myFoldIconPolygon                 = null;
        static Vector2[] myUnfoldIconPolygon               = null;
        static Vector2[] myBackwardNavigationHistoryPolygon= null;
        static Vector2[] myForwardNavigationHistoryPolygon = null;
    
        // =================================================================================
        // Constrcutor
        // ---------------------------------------------------------------------------------
        static iCS_BuiltinTextures() {
            // Build polygons.
            myFoldIconPolygon= new Vector2[3];
            myFoldIconPolygon[0]= new Vector2(-0.5f, -0.45f);
            myFoldIconPolygon[1]= new Vector2( 0.5f,  0);
            myFoldIconPolygon[2]= new Vector2(-0.5f,  0.45f);
            myUnfoldIconPolygon= new Vector2[3];
            myUnfoldIconPolygon[0]= new Vector2(-0.45f, 0.5f);
            myUnfoldIconPolygon[1]= new Vector2( 0.45f, 0.5f);
            myUnfoldIconPolygon[2]= new Vector2( 0,   -0.5f);
            // Build scale independent textures.
            BuildScaleIndependantTextures();
            BuildScaleDependantTextures();
        }
        // ---------------------------------------------------------------------------------
        static void Validate() {
            if(myIsProSkin != EditorGUIUtility.isProSkin) {
                myBackwardNavigationHistoryPolygon= null;
                myForwardNavigationHistoryPolygon= null;
                myIsProSkin= EditorGUIUtility.isProSkin;
            }
        }
        // ---------------------------------------------------------------------------------
        static void BuildScaleIndependantTextures() {
            BuildLocalVariablePortIcons(Color.red);
            BuildPublicVariablePortIcons(Color.red);
            BuildTransitionPortIcons();
            BuildControlPortIcons();
        }
        // ---------------------------------------------------------------------------------
        static void BuildScaleDependantTextures() {
            BuildMinimizeIcon();
            BuildMaximizeIcon();
            BuildFoldIcon();
            BuildUnfoldIcon();        
        }
        // ---------------------------------------------------------------------------------
    	static void BuildLocalVariablePortIcons(Color typeColor) {
    		Texture2D inPortTemplate= new Texture2D(kPortIconHeight, kPortIconHeight, TextureFormat.ARGB32, false);
    		Texture2D outPortTemplate= new Texture2D(kPortIconHeight, kPortIconHeight, TextureFormat.ARGB32, false);
    		float radius= 0.5f*(kPortIconHeight-3f);
            iCS_PortIcons.BuildInLocalVariablePortTemplateImp(radius, radius-2, ref inPortTemplate);
            iCS_PortIcons.BuildOutLocalVariablePortTemplateImp(radius, radius-2, ref outPortTemplate);
            Texture2D portInIcon= iCS_PortIcons.BuildPortIcon(typeColor, inPortTemplate);
            Texture2D portOutIcon= iCS_PortIcons.BuildPortIcon(typeColor, outPortTemplate);

            myInEndPortIcon= new Texture2D(kPortIconWidth, kPortIconHeight, TextureFormat.ARGB32, false);
            myOutEndPortIcon= new Texture2D(kPortIconWidth, kPortIconHeight, TextureFormat.ARGB32, false);
            iCS_TextureUtil.Clear(ref myInEndPortIcon);
            iCS_TextureUtil.Clear(ref myOutEndPortIcon);
            int xOffset= (kPortIconWidth-kPortIconHeight)>>1;
            iCS_AlphaBlend.NormalBlend(0, 0, portInIcon , xOffset, 0, ref myInEndPortIcon,  portInIcon.width, portInIcon.height);
            iCS_AlphaBlend.NormalBlend(0, 0, portOutIcon, xOffset, 0, ref myOutEndPortIcon, portOutIcon.width, portOutIcon.height);

            // Finalize icons.
            myInEndPortIcon.Apply();
            myOutEndPortIcon.Apply();
            myInEndPortIcon.hideFlags= HideFlags.DontSave;
            myOutEndPortIcon.hideFlags= HideFlags.DontSave;
	
            Texture2D.DestroyImmediate(portInIcon);
            Texture2D.DestroyImmediate(portOutIcon);
            Texture2D.DestroyImmediate(inPortTemplate);	    
            Texture2D.DestroyImmediate(outPortTemplate);	    
    	}

        // ---------------------------------------------------------------------------------
    	static void BuildPublicVariablePortIcons(Color typeColor) {
    		Texture2D inPortTemplate= new Texture2D(kPortIconHeight, kPortIconHeight, TextureFormat.ARGB32, false);
    		Texture2D outPortTemplate= new Texture2D(kPortIconHeight, kPortIconHeight, TextureFormat.ARGB32, false);
    		float len= kPortIconHeight-2f;
            iCS_PortIcons.BuildInPublicVariablePortTemplate (len, ref inPortTemplate);
            iCS_PortIcons.BuildOutPublicVariablePortTemplate(len, ref outPortTemplate);
            Texture2D inPortIcon = iCS_PortIcons.BuildPortIcon(typeColor, inPortTemplate);
            Texture2D outPortIcon= iCS_PortIcons.BuildPortIcon(typeColor, outPortTemplate);

            myInRelayPortIcon = new Texture2D(kPortIconWidth, kPortIconHeight, TextureFormat.ARGB32, false);
            myOutRelayPortIcon= new Texture2D(kPortIconWidth, kPortIconHeight, TextureFormat.ARGB32, false);
            iCS_TextureUtil.Clear(ref myInRelayPortIcon);
            iCS_TextureUtil.Clear(ref myOutRelayPortIcon);
            int xOffset= (kPortIconWidth-kPortIconHeight)>>1;
            iCS_AlphaBlend.NormalBlend(0, 0, inPortIcon,  xOffset, 0, ref myInRelayPortIcon,  inPortIcon.width,  inPortIcon.height);
            iCS_AlphaBlend.NormalBlend(0, 0, outPortIcon, xOffset, 0, ref myOutRelayPortIcon, outPortIcon.width, outPortIcon.height);

            // Finalize icons.
            myInRelayPortIcon.Apply();
            myOutRelayPortIcon.Apply();
            myInRelayPortIcon.hideFlags= HideFlags.DontSave;
            myOutRelayPortIcon.hideFlags= HideFlags.DontSave;
	
            Texture2D.DestroyImmediate(inPortIcon);
            Texture2D.DestroyImmediate(inPortTemplate);	    
            Texture2D.DestroyImmediate(outPortIcon);
            Texture2D.DestroyImmediate(outPortTemplate);	    
    	}
        // ---------------------------------------------------------------------------------
        static void BuildControlPortIcons() {
    		Texture2D inPortTemplate= new Texture2D(kPortIconHeight, kPortIconHeight, TextureFormat.ARGB32, false);
    		Texture2D outPortTemplate= new Texture2D(kPortIconHeight, kPortIconHeight, TextureFormat.ARGB32, false);
            float scale= 0.8f*kPortIconHeight/(iCS_EditorConfig.PortDiameter*1.4f);
            iCS_PortIcons.BuildEnablePortTemplate(scale, ref inPortTemplate);
            iCS_PortIcons.BuildTriggerPortTemplate(scale, ref outPortTemplate);
            Texture2D portInIcon= iCS_PortIcons.BuildPortIcon(Prefs.BoolTypeColor, inPortTemplate);
            Texture2D portOutIcon= iCS_PortIcons.BuildPortIcon(Prefs.BoolTypeColor, outPortTemplate);

            myInTriggerPortIcon= new Texture2D(kPortIconWidth, kPortIconHeight, TextureFormat.ARGB32, false);
            myOutTriggerPortIcon= new Texture2D(kPortIconWidth, kPortIconHeight, TextureFormat.ARGB32, false);
            iCS_TextureUtil.Clear(ref myInTriggerPortIcon);
            iCS_TextureUtil.Clear(ref myOutTriggerPortIcon);
            iCS_AlphaBlend.NormalBlend(0, 0, portInIcon , 0, 0, ref myInTriggerPortIcon,  portInIcon.width, portInIcon.height);
            iCS_AlphaBlend.NormalBlend(0, 0, portOutIcon, 0, 0, ref myOutTriggerPortIcon, portOutIcon.width, portOutIcon.height);

            // Finalize icons.
            myInTriggerPortIcon.Apply();
            myOutTriggerPortIcon.Apply();
            myInTriggerPortIcon.hideFlags= HideFlags.DontSave;
            myOutTriggerPortIcon.hideFlags= HideFlags.DontSave;
	
            Texture2D.DestroyImmediate(portInIcon);
            Texture2D.DestroyImmediate(portOutIcon);
            Texture2D.DestroyImmediate(inPortTemplate);	    
            Texture2D.DestroyImmediate(outPortTemplate);	            
        }
        // ---------------------------------------------------------------------------------
        static void BuildTransitionPortIcons() {
            // Create textures.
            myInTransitionPortIcon= new Texture2D(kPortIconWidth, kPortIconHeight, TextureFormat.ARGB32, false);
            myOutTransitionPortIcon= new Texture2D(kPortIconWidth, kPortIconHeight, TextureFormat.ARGB32, false);
            iCS_TextureUtil.Clear(ref myInTransitionPortIcon);
            iCS_TextureUtil.Clear(ref myOutTransitionPortIcon);
        
            // Build out transition port.
            float halfHeight= 0.5f*kPortIconHeight;
            float radius= halfHeight-1.5f;
    		iCS_TextureUtil.DrawFilledCircle(ref myOutTransitionPortIcon, radius, new Vector2(halfHeight, halfHeight), Color.white);

            // Add horizontal line.
    		int halfHeightInt= (int)halfHeight;
    		for(int x= halfHeightInt; x < kPortIconWidth; ++x) {
    		    myOutTransitionPortIcon.SetPixel(x, halfHeightInt, Color.white);
                myInTransitionPortIcon.SetPixel(x-halfHeightInt, halfHeightInt, Color.white);
    		}
		
            // Finalize icons.
            myInTransitionPortIcon.Apply();
            myOutTransitionPortIcon.Apply();
            myInTransitionPortIcon.hideFlags= HideFlags.DontSave;
            myOutTransitionPortIcon.hideFlags= HideFlags.DontSave;
        }

        // ---------------------------------------------------------------------------------
        static void BuildMaximizeIcon() {        
            // Allocate icon.
            float size= /*myScale**/kMaximizeIconSize;
            int sizeInt= ((int)size)+1;
            float offset= 0.5f*((float)sizeInt-size);
    //        if(myMaximizeIcon != null) Texture2D.DestroyImmediate(myMaximizeIcon);
            myMaximizeIcon= new Texture2D(sizeInt, sizeInt, TextureFormat.ARGB32, false);
            iCS_TextureUtil.Clear(ref myMaximizeIcon);
            // Draw minimize icon.
            float halfSize= 0.5f*size;

            float radius= halfSize-0.15f*size;
            var center= new Vector2(halfSize+offset, halfSize+offset);
            float borderWidth= 1f;
            Color borderColor= Color.black;
            Color fillColor= Color.white;
            iCS_TextureUtil.DrawFilledCircle(ref myMaximizeIcon, radius-0.5f, center, fillColor);
            iCS_TextureUtil.DrawCircle(ref myMaximizeIcon, radius, center, borderColor, borderWidth);
            float lineHeight= 0.05f*size;
            float lineHalfWidth= 0.15f*size;
            iCS_TextureUtil.DrawLine(ref myMaximizeIcon,
                                     new Vector2(center.x-lineHalfWidth, center.y),
                                     new Vector2(center.x+lineHalfWidth, center.y),
                                     Color.black,
                                     lineHeight);
            iCS_TextureUtil.DrawLine(ref myMaximizeIcon,
                                     new Vector2(center.x, center.y-lineHalfWidth),
                                     new Vector2(center.x, center.y+lineHalfWidth),
                                     Color.black,
                                     lineHeight);

    //        iCS_TextureUtil.Circle(halfSize-0.15f*size,
    //                           Color.black, Color.white,
    //                           ref myMaximizeIcon, new Vector2(halfSize+offset, halfSize+offset));
    //        int halfSizeInt= (int)halfSize;
    //        for(int i= (int)(0.30f*size); i <= (int)(0.70f*size); ++i) {
    //            myMaximizeIcon.SetPixel(1+i, 1+halfSizeInt, Color.black);
    //            myMaximizeIcon.SetPixel(1+i, 1+halfSizeInt-1, Color.black);
    //            myMaximizeIcon.SetPixel(1+halfSizeInt, 1+i, Color.black);
    //            myMaximizeIcon.SetPixel(1+halfSizeInt-1, 1+i, Color.black);
    //        }
            // Finalize icons.
            myMaximizeIcon.Apply();
            myMaximizeIcon.hideFlags= HideFlags.DontSave;
        }
        // ---------------------------------------------------------------------------------
        static void BuildMinimizeIcon() {        
            // Allocate icon.
            float size= myScale*kMinimizeIconSize;
            int sizeInt= ((int)size)+1;
            float offset= 0.5f*((float)sizeInt-size);
            if(myMinimizeIcon != null) Texture2D.DestroyImmediate(myMinimizeIcon);
            myMinimizeIcon= new Texture2D(sizeInt, sizeInt, TextureFormat.ARGB32, false);
            iCS_TextureUtil.Clear(ref myMinimizeIcon);
            // Draw minimize icon.
            float halfSize= 0.5f*size;
            float radius= halfSize-0.15f*size;
            var center= new Vector2(halfSize+offset, halfSize+offset);
            float borderWidth= 1.2f;
            Color borderColor= Color.black;
            Color fillColor= new Color(0,0,0,0.3f);
            iCS_TextureUtil.DrawFilledCircle(ref myMinimizeIcon, radius-0.5f, center, fillColor);
            iCS_TextureUtil.DrawCircle(ref myMinimizeIcon, radius, center, borderColor, borderWidth);
            float lineHeight= 0.1f*size;
            float lineHalfWidth= 0.15f*size;
            iCS_TextureUtil.DrawLine(ref myMinimizeIcon,
                                     new Vector2(center.x-lineHalfWidth, center.y),
                                     new Vector2(center.x+lineHalfWidth, center.y),
                                     Color.black,
                                     lineHeight);
            // Finalize icons.
            myMinimizeIcon.Apply();
            myMinimizeIcon.hideFlags= HideFlags.DontSave;
        }
        // ---------------------------------------------------------------------------------
        static void BuildFoldIcon() {
            // Build polygon
            float size= myScale*kFoldIconSize;
            int textureSize= ((int)size)+1;
            float offset= 0.5f*((float)textureSize-size);
            var center= new Vector2(0.5f*textureSize+offset, 0.5f*textureSize+offset);
            var scale= new Vector2(0.65f*size, 0.65f*size);
            var polygon= Math3D.ScaleAndTranslatePolygon(myFoldIconPolygon, scale, center);
            // Build texture
            if(myFoldIcon != null) Texture2D.DestroyImmediate(myFoldIcon);
    		myFoldIcon= new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
    		iCS_TextureUtil.Clear(ref myFoldIcon);
            Color c= new Color(0,0,0,0.5f);
    	    iCS_TextureUtil.DrawFilledPolygon(ref myFoldIcon, polygon, c);
    	    float lineWidth= 1.2f*myScale;
    	    iCS_TextureUtil.DrawPolygonOutline(ref myFoldIcon, polygon, Color.black, lineWidth);
            // Finalize icons.
            myFoldIcon.Apply();
            myFoldIcon.hideFlags= HideFlags.DontSave;
        }
        // ---------------------------------------------------------------------------------
        static void BuildUnfoldIcon() {
            // Build polygon
            float size= myScale*kUnfoldIconSize;
            int textureSize= ((int)size)+1;
            float offset= 0.5f*((float)textureSize-size);
            var center= new Vector2(0.5f*textureSize+offset, 0.5f*textureSize+offset);
            var scale= new Vector2(0.65f*size, 0.65f*size);
            var polygon= Math3D.ScaleAndTranslatePolygon(myUnfoldIconPolygon, scale, center);
            // Build texture
            if(myUnfoldIcon != null) Texture2D.DestroyImmediate(myUnfoldIcon);
    		myUnfoldIcon= new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
    		iCS_TextureUtil.Clear(ref myUnfoldIcon);
            Color c= new Color(0,0,0,0.5f);
    	    iCS_TextureUtil.DrawFilledPolygon(ref myUnfoldIcon, polygon, c);
    	    float lineWidth= 1.2f*myScale;
    	    iCS_TextureUtil.DrawPolygonOutline(ref myUnfoldIcon, polygon, Color.black, lineWidth);
            // Finalize icons.
            myUnfoldIcon.Apply();
            myUnfoldIcon.hideFlags= HideFlags.DontSave;
        }
        // ---------------------------------------------------------------------------------
        static void BuildNavigationHistoryIcons() {
            // Build polygons
            myBackwardNavigationHistoryPolygon= new Vector2[3];
            myBackwardNavigationHistoryPolygon[0]= new Vector2(-0.5f, -0.45f);
            myBackwardNavigationHistoryPolygon[1]= new Vector2( 0.5f,  0);
            myBackwardNavigationHistoryPolygon[2]= new Vector2(-0.5f,  0.45f);
            myForwardNavigationHistoryPolygon= new Vector2[3];
            myForwardNavigationHistoryPolygon[0]= new Vector2( 0.5f, -0.45f);
            myForwardNavigationHistoryPolygon[1]= new Vector2(-0.5f,  0);
            myForwardNavigationHistoryPolygon[2]= new Vector2( 0.5f,  0.45f);
            // Scale and translate polygon to meet desired size needs.
            float size= 16f;
            int textureSize= ((int)size)+1;
            float offset= 0.5f*((float)textureSize-size);
            var center= new Vector2(0.5f*textureSize+offset, 0.5f*textureSize+offset);
            var scale= new Vector2(0.7f*size, 0.7f*size);
            var backwardPolygon= Math3D.ScaleAndTranslatePolygon(myBackwardNavigationHistoryPolygon, scale, center);
            var forwardPolygon= Math3D.ScaleAndTranslatePolygon(myForwardNavigationHistoryPolygon, scale, center);
            // Allocate & Clear textures
    		myForwardNavigationHistoryIcon= new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
    		myBackwardNavigationHistoryIcon= new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
    		iCS_TextureUtil.Clear(ref myForwardNavigationHistoryIcon);
    		iCS_TextureUtil.Clear(ref myBackwardNavigationHistoryIcon);
            // Build texture
            Color c= EditorStyles.label.normal.textColor;
    	    iCS_TextureUtil.DrawFilledPolygon(ref myForwardNavigationHistoryIcon, backwardPolygon, c);
    	    iCS_TextureUtil.DrawFilledPolygon(ref myBackwardNavigationHistoryIcon, forwardPolygon, c);
            // Finalize icons.
            myForwardNavigationHistoryIcon.Apply();
            myBackwardNavigationHistoryIcon.Apply();
            myForwardNavigationHistoryIcon.hideFlags= HideFlags.DontSave;        
            myBackwardNavigationHistoryIcon.hideFlags= HideFlags.DontSave;        
        }
    }
}
