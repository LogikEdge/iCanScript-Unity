using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using iCanScript;

namespace iCanScript.Internal.Editor {
    // TODO: Need to cleanup obsoleted data port icon code.
    public static class iCS_PortIcons {
        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
    	static float		myScale					                    = 0f;

        static Texture2D    myEnablePortTemplate                        = null;
        static Texture2D    myTriggerPortTemplate                       = null;
    	static Texture2D    mySelectedEnablePortTemplate                = null;
    	static Texture2D    mySelectedTriggerPortTemplate               = null;

    	static Texture2D	myInLocalVariablePortTemplate               = null;
    	static Texture2D	myOutLocalVariablePortTemplate              = null;
    	static Texture2D	mySelectedInLocalVariablePortTemplate       = null;
    	static Texture2D	mySelectedOutLocalVariablePortTemplate      = null;

    	static Texture2D	myInPublicVariablePortTemplate  	        = null;
    	static Texture2D	myOutPublicVariablePortTemplate  	        = null;
    	static Texture2D	mySelectedInPublicVariablePortTemplate      = null;
    	static Texture2D	mySelectedOutPublicVariablePortTemplate     = null;
        
    	static Texture2D	myInPrivateVariablePortTemplate  	        = null;
    	static Texture2D	myOutPrivateVariablePortTemplate  	        = null;
    	static Texture2D	mySelectedInPrivateVariablePortTemplate     = null;
    	static Texture2D	mySelectedOutPrivateVariablePortTemplate    = null;
        
    	static Texture2D	myInStaticPublicVariablePortTemplate  	      = null;
    	static Texture2D	myOutStaticPublicVariablePortTemplate  	      = null;
    	static Texture2D	mySelectedInStaticPublicVariablePortTemplate  = null;
    	static Texture2D	mySelectedOutStaticPublicVariablePortTemplate = null;
        
    	static Texture2D	myInStaticPrivateVariablePortTemplate  	      = null;
    	static Texture2D	myOutStaticPrivateVariablePortTemplate  	  = null;
    	static Texture2D	mySelectedInStaticPrivateVariablePortTemplate = null;
    	static Texture2D	mySelectedOutStaticPrivateVariablePortTemplate= null;
        
    	static Texture2D	myInParameterPortTemplate  	                = null;
    	static Texture2D	myOutParameterPortTemplate  	            = null;
    	static Texture2D	mySelectedInParameterPortTemplate           = null;
    	static Texture2D	mySelectedOutParameterPortTemplate          = null;
        
    	static Texture2D	myConstantPortTemplate  	                = null;
    	static Texture2D	mySelectedConstantPortTemplate              = null;
        
    	static Texture2D    myInMuxPortTopTemplate                      = null;
    	static Texture2D    myInMuxPortBottomTemplate                   = null;
    	static Texture2D    myInMuxPortLeftTemplate                     = null;
    	static Texture2D    myInMuxPortRightTemplate                    = null;
    	static Texture2D    myOutMuxPortTopTemplate                     = null;
    	static Texture2D    myOutMuxPortBottomTemplate                  = null;
    	static Texture2D    myOutMuxPortLeftTemplate                    = null;
    	static Texture2D    myOutMuxPortRightTemplate                   = null;
    	static Texture2D    mySelectedInMuxPortTopTemplate              = null;
    	static Texture2D    mySelectedInMuxPortBottomTemplate           = null;
    	static Texture2D    mySelectedInMuxPortLeftTemplate             = null;
    	static Texture2D    mySelectedInMuxPortRightTemplate            = null;
    	static Texture2D    mySelectedOutMuxPortTopTemplate             = null;
    	static Texture2D    mySelectedOutMuxPortBottomTemplate          = null;
    	static Texture2D    mySelectedOutMuxPortLeftTemplate            = null;
    	static Texture2D    mySelectedOutMuxPortRightTemplate           = null;
	
        // ----------------------------------------------------------------------
        static Dictionary<Color,Texture2D>  myEnablePortIcons                        = null;
        static Dictionary<Color,Texture2D>  myTriggerPortIcons                       = null;
    	static Dictionary<Color,Texture2D>	mySelectedEnablePortIcons                = null;
    	static Dictionary<Color,Texture2D>	mySelectedTriggerPortIcons               = null;

    	static Dictionary<Color,Texture2D>	myInLocalVariablePortIcons               = null;
    	static Dictionary<Color,Texture2D>	myOutLocalVariablePortIcons              = null;
    	static Dictionary<Color,Texture2D>	mySelectedInLocalVariablePortIcons       = null;
    	static Dictionary<Color,Texture2D>	mySelectedOutLocalVariablePortIcons      = null;

    	static Dictionary<Color,Texture2D>	myInPublicVariablePortIcons              = null;
    	static Dictionary<Color,Texture2D>	myOutPublicVariablePortIcons             = null;
    	static Dictionary<Color,Texture2D>	mySelectedInPublicVariablePortIcons      = null;
    	static Dictionary<Color,Texture2D>	mySelectedOutPublicVariablePortIcons     = null;

    	static Dictionary<Color,Texture2D>	myInPrivateVariablePortIcons             = null;
    	static Dictionary<Color,Texture2D>	myOutPrivateVariablePortIcons            = null;
    	static Dictionary<Color,Texture2D>	mySelectedInPrivateVariablePortIcons     = null;
    	static Dictionary<Color,Texture2D>	mySelectedOutPrivateVariablePortIcons    = null;

    	static Dictionary<Color,Texture2D>	myInStaticPublicVariablePortIcons          = null;
    	static Dictionary<Color,Texture2D>	myOutStaticPublicVariablePortIcons         = null;
    	static Dictionary<Color,Texture2D>	mySelectedInStaticPublicVariablePortIcons  = null;
    	static Dictionary<Color,Texture2D>	mySelectedOutStaticPublicVariablePortIcons = null;

    	static Dictionary<Color,Texture2D>	myInStaticPrivateVariablePortIcons         = null;
    	static Dictionary<Color,Texture2D>	myOutStaticPrivateVariablePortIcons        = null;
    	static Dictionary<Color,Texture2D>	mySelectedInStaticPrivateVariablePortIcons = null;
    	static Dictionary<Color,Texture2D>	mySelectedOutStaticPrivateVariablePortIcons= null;

    	static Dictionary<Color,Texture2D>	myInParameterPortIcons                   = null;
    	static Dictionary<Color,Texture2D>	myOutParameterPortIcons                  = null;
    	static Dictionary<Color,Texture2D>	mySelectedInParameterPortIcons           = null;
    	static Dictionary<Color,Texture2D>	mySelectedOutParameterPortIcons          = null;

    	static Dictionary<Color,Texture2D>	myConstantPortIcons                      = null;
    	static Dictionary<Color,Texture2D>	mySelectedConstantPortIcons              = null;

        static Dictionary<Color,Texture2D>  myInMuxPortTopIcons            = null;
        static Dictionary<Color,Texture2D>  myInMuxPortBottomIcons         = null;
        static Dictionary<Color,Texture2D>  myInMuxPortLeftIcons           = null;
        static Dictionary<Color,Texture2D>  myInMuxPortRightIcons          = null;
        static Dictionary<Color,Texture2D>  myOutMuxPortTopIcons           = null;
        static Dictionary<Color,Texture2D>  myOutMuxPortBottomIcons        = null;
        static Dictionary<Color,Texture2D>  myOutMuxPortLeftIcons          = null;
        static Dictionary<Color,Texture2D>  myOutMuxPortRightIcons         = null;
    	static Dictionary<Color,Texture2D>	mySelectedInMuxPortTopIcons    = null;
    	static Dictionary<Color,Texture2D>	mySelectedInMuxPortBottomIcons = null;
    	static Dictionary<Color,Texture2D>	mySelectedInMuxPortLeftIcons   = null;
    	static Dictionary<Color,Texture2D>	mySelectedInMuxPortRightIcons  = null;
    	static Dictionary<Color,Texture2D>	mySelectedOutMuxPortTopIcons   = null;
    	static Dictionary<Color,Texture2D>	mySelectedOutMuxPortBottomIcons= null;
    	static Dictionary<Color,Texture2D>	mySelectedOutMuxPortLeftIcons  = null;
    	static Dictionary<Color,Texture2D>	mySelectedOutMuxPortRightIcons = null;

        // ======================================================================
        // PORT POLYGONS
        // ----------------------------------------------------------------------
        static Vector2[] myMuxPortPolygon      = null;
        static Vector2[] myControlPortPolygon  = null;
        static Vector2[] myParameterPortPolygon= null;
    
        // ======================================================================
        // POLYGON BUILDER
        // ----------------------------------------------------------------------
        static iCS_PortIcons() {
            // -- Mux Port Polygon --
            myMuxPortPolygon= new Vector2[4];
            myMuxPortPolygon[0]= new Vector2(-0.25f, -0.5f);
            myMuxPortPolygon[1]= new Vector2( 0.25f, -0.25f);
            myMuxPortPolygon[2]= new Vector2( 0.25f,  0.25f);
            myMuxPortPolygon[3]= new Vector2(-0.25f, 0.5f);        
            // -- Control Port Polygon --
            myControlPortPolygon= new Vector2[4];
            myControlPortPolygon[0]= new Vector2( 0,   -0.5f);
            myControlPortPolygon[1]= new Vector2( 0.5f, 0);
            myControlPortPolygon[2]= new Vector2( 0,    0.5f);
            myControlPortPolygon[3]= new Vector2(-0.5f, 0);
            // -- Parameter Port Polygon --
            myParameterPortPolygon= new Vector2[3];
            myParameterPortPolygon[0]= new Vector2(0,      0.5f);
            myParameterPortPolygon[1]= new Vector2(0.5f,  -0.5f);
            myParameterPortPolygon[2]= new Vector2(-0.5f, -0.5f);
        }

    	// ----------------------------------------------------------------------
        //  Build template for all port icons
    	public static void BuildPortIconTemplates(float scale) {
    		if(Math3D.IsEqual(scale, myScale)) return;
    		myScale= scale;
    		BuildLocalVariablePortTemplates(scale);
    		BuildPublicVariablePortTemplates(scale);
    		BuildPrivateVariablePortTemplates(scale);
            BuildParameterPortTemplates(scale);
            BuildConstantPortTemplates(scale);
            BuildMuxPortTemplates(scale);
            BuildControlPortTemplates(scale);
    		FlushCachedIcons();
    	}
    	// ----------------------------------------------------------------------
    	static void BuildPublicVariablePortTemplates(float scale) {
            float len= scale*iCS_EditorConfig.PortDiameter;
            float selectedLen= len*iCS_EditorConfig.SelectedPortFactor;
    		BuildInPublicVariablePortTemplate(len, ref myInPublicVariablePortTemplate);
    		BuildOutPublicVariablePortTemplate(len, ref myOutPublicVariablePortTemplate);
    		BuildInPublicVariablePortTemplate(selectedLen, ref mySelectedInPublicVariablePortTemplate);
    		BuildOutPublicVariablePortTemplate(selectedLen, ref mySelectedOutPublicVariablePortTemplate);
    		BuildInStaticPublicVariablePortTemplate(len, ref myInStaticPublicVariablePortTemplate);
    		BuildOutStaticPublicVariablePortTemplate(len, ref myOutStaticPublicVariablePortTemplate);
    		BuildInStaticPublicVariablePortTemplate(selectedLen, ref mySelectedInStaticPublicVariablePortTemplate);
    		BuildOutStaticPublicVariablePortTemplate(selectedLen, ref mySelectedOutStaticPublicVariablePortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	static void BuildPrivateVariablePortTemplates(float scale) {
            float len= scale*iCS_EditorConfig.PortDiameter;
            float selectedLen= len*iCS_EditorConfig.SelectedPortFactor;
    		BuildInPrivateVariablePortTemplate(len, ref myInPrivateVariablePortTemplate);
    		BuildOutPrivateVariablePortTemplate(len, ref myOutPrivateVariablePortTemplate);
    		BuildInPrivateVariablePortTemplate(selectedLen, ref mySelectedInPrivateVariablePortTemplate);
    		BuildOutPrivateVariablePortTemplate(selectedLen, ref mySelectedOutPrivateVariablePortTemplate);
    		BuildInStaticPrivateVariablePortTemplate(len, ref myInStaticPrivateVariablePortTemplate);
    		BuildOutStaticPrivateVariablePortTemplate(len, ref myOutStaticPrivateVariablePortTemplate);
    		BuildInStaticPrivateVariablePortTemplate(selectedLen, ref mySelectedInStaticPrivateVariablePortTemplate);
    		BuildOutStaticPrivateVariablePortTemplate(selectedLen, ref mySelectedOutStaticPrivateVariablePortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	static void BuildParameterPortTemplates(float scale) {
            float selectedScale= scale*iCS_EditorConfig.SelectedPortFactor;
    		BuildInParameterPortTemplate(scale, ref myInParameterPortTemplate);
    		BuildOutParameterPortTemplate(scale, ref myOutParameterPortTemplate);
    		BuildInParameterPortTemplate(selectedScale, ref mySelectedInParameterPortTemplate);
    		BuildOutParameterPortTemplate(selectedScale, ref mySelectedOutParameterPortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	static void BuildControlPortTemplates(float scale) {
            float selectedScale= scale*iCS_EditorConfig.SelectedPortFactor;
    		BuildEnablePortTemplate(scale, ref myEnablePortTemplate);
    		BuildEnablePortTemplate(selectedScale, ref mySelectedEnablePortTemplate);
    		BuildTriggerPortTemplate(scale, ref myTriggerPortTemplate);
    		BuildTriggerPortTemplate(selectedScale, ref mySelectedTriggerPortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	static void BuildMuxPortTemplates(float scale) {
            float width= scale*iCS_EditorConfig.PortDiameter;
            float height= width*2f;
            float selectedWidth= width*iCS_EditorConfig.SelectedPortFactor;
            float selectedHeight= height*iCS_EditorConfig.SelectedPortFactor;
    		BuildMuxPortTemplate(width, height, ref myInMuxPortTopTemplate, true, 270f);
    		BuildMuxPortTemplate(selectedWidth, selectedHeight, ref mySelectedInMuxPortTopTemplate, true, 270f);
    		BuildMuxPortTemplate(width, height, ref myInMuxPortBottomTemplate, true, 90f);
    		BuildMuxPortTemplate(selectedWidth, selectedHeight, ref mySelectedInMuxPortBottomTemplate, true, 90f);
    		BuildMuxPortTemplate(width, height, ref myInMuxPortLeftTemplate, true, 180f);
    		BuildMuxPortTemplate(selectedWidth, selectedHeight, ref mySelectedInMuxPortLeftTemplate, true, 180f);
    		BuildMuxPortTemplate(width, height, ref myInMuxPortRightTemplate, true, 0f);
    		BuildMuxPortTemplate(selectedWidth, selectedHeight, ref mySelectedInMuxPortRightTemplate, true, 0f);
    		BuildMuxPortTemplate(width, height, ref myOutMuxPortTopTemplate, false, 270f);
    		BuildMuxPortTemplate(selectedWidth, selectedHeight, ref mySelectedOutMuxPortTopTemplate, false, 270f);
    		BuildMuxPortTemplate(width, height, ref myOutMuxPortBottomTemplate, false, 90f);
    		BuildMuxPortTemplate(selectedWidth, selectedHeight, ref mySelectedOutMuxPortBottomTemplate, false, 90f);
    		BuildMuxPortTemplate(width, height, ref myOutMuxPortLeftTemplate, false, 180f);
    		BuildMuxPortTemplate(selectedWidth, selectedHeight, ref mySelectedOutMuxPortLeftTemplate, false, 180f);
    		BuildMuxPortTemplate(width, height, ref myOutMuxPortRightTemplate, false, 0f);
    		BuildMuxPortTemplate(selectedWidth, selectedHeight, ref mySelectedOutMuxPortRightTemplate, false, 0f);
    	}
    	// ----------------------------------------------------------------------
    	public static void BuildInPublicVariablePortTemplate(float len, ref Texture2D template) {
    		// Create texture.
            BuildInPublicVariablePortTemplateCommon(len, ref template);
    		template.Apply();
    	}
    	// ----------------------------------------------------------------------
    	public static void BuildInStaticPublicVariablePortTemplate(float len, ref Texture2D template) {
    		// Create texture.
            BuildInPublicVariablePortTemplateCommon(len, ref template);
    		int lenInt= (int)(len+1f);
            BuildSquareOutline(ref template, lenInt, Color.green);
    		template.Apply();
    	}
    	// ----------------------------------------------------------------------
    	public static void BuildOutPublicVariablePortTemplate(float len, ref Texture2D template) {
    		// Create texture.
            BuildOutPublicVariablePortTemplateCommon(len, ref template);
    		template.Apply();
    	}
    	// ----------------------------------------------------------------------
    	public static void BuildOutStaticPublicVariablePortTemplate(float len, ref Texture2D template) {
    		// Create texture.
            BuildOutPublicVariablePortTemplateCommon(len, ref template);
    		int lenInt= (int)(len+1f);
            BuildSquareOutline(ref template, lenInt, Color.green);
    		template.Apply();
    	}
    	// ----------------------------------------------------------------------
    	public static void BuildInPrivateVariablePortTemplate(float len, ref Texture2D template) {
    		// Create texture.
            BuildInPrivateVariablePortTemplateCommon(len, ref template);
    		template.Apply();
    	}
    	// ----------------------------------------------------------------------
    	public static void BuildInStaticPrivateVariablePortTemplate(float len, ref Texture2D template) {
    		// Create texture.
            BuildInPrivateVariablePortTemplateCommon(len, ref template);
    		int lenInt= (int)(len+1f);
            BuildSquareOutline(ref template, lenInt, Color.green);
    		template.Apply();
    	}
    	// ----------------------------------------------------------------------
    	public static void BuildOutPrivateVariablePortTemplate(float len, ref Texture2D template) {
    		// Create texture.
            BuildOutPrivateVariablePortTemplateCommon(len, ref template);
    		template.Apply();
    	}
    	// ----------------------------------------------------------------------
    	public static void BuildOutStaticPrivateVariablePortTemplate(float len, ref Texture2D template) {
    		// Create texture.
            BuildOutPrivateVariablePortTemplateCommon(len, ref template);
    		int lenInt= (int)(len+1f);
            BuildSquareOutline(ref template, lenInt, Color.green);
    		template.Apply();
    	}

    	// ----------------------------------------------------------------------
        static void BuildSquareOutline(ref Texture2D template, int len, Color color) {
            int maxX= len-1;
            int maxY= len-1;
            iCS_TextureUtil.DrawLine(ref template, Vector2.zero, new Vector2(0, maxY), color);
            iCS_TextureUtil.DrawLine(ref template, Vector2.zero, new Vector2(maxX, 0), color);
            iCS_TextureUtil.DrawLine(ref template, new Vector2(maxX, maxY), new Vector2(maxX, 0), color);
            iCS_TextureUtil.DrawLine(ref template, new Vector2(maxX, maxY), new Vector2(0, maxY), color);            
        }
    	// ----------------------------------------------------------------------
    	public static void BuildInPublicVariablePortTemplateCommon(float len, ref Texture2D template) {
    		// Create texture.
    		int lenInt= (int)(len+1f);
    		int borderSize= myScale > 1.5 ? 3 : (myScale > 1.25f ? 2 : (myScale > 0.75 ? 1 : 0));
            // Remove previous template.
            if(template != null) Texture2D.DestroyImmediate(template);
    		template= new Texture2D(lenInt, lenInt);
    		for(int x= 0; x < lenInt; ++x) {
    			for(int y= 0; y < lenInt; ++y) {
    				if(x <= borderSize || y <= borderSize || x >= lenInt-1-borderSize || y >= lenInt-1-borderSize) {
    					template.SetPixel(x,y,Color.black);
    				} else  if(x <= borderSize+1 || x >= lenInt-borderSize-2 || y <= borderSize+1 || y >= lenInt-borderSize-2) {
    					template.SetPixel(x,y,Color.red);
    				} else {
    					template.SetPixel(x,y,Color.black);					
    				}
    			}
    		}
    		template.hideFlags= HideFlags.DontSave;
    	}
    	// ----------------------------------------------------------------------
    	public static void BuildOutPublicVariablePortTemplateCommon(float len, ref Texture2D template) {
    		// Create texture.
    		int lenInt= (int)(len+1f);
    		int borderSize= myScale > 1.5 ? 4 : (myScale > 1.25f ? 3 : (myScale > 0.9f ? 2 : (myScale > 0.5 ? 1 : 0)));
            // Remove previous template.
            if(template != null) Texture2D.DestroyImmediate(template);
    		template= new Texture2D(lenInt, lenInt);
    		for(int x= 0; x < lenInt; ++x) {
    			for(int y= 0; y < lenInt; ++y) {
    				if(x <= borderSize || y <= borderSize || x >= lenInt-1-borderSize || y >= lenInt-1-borderSize) {
    					template.SetPixel(x,y,Color.black);
    				} else {
    					template.SetPixel(x,y,Color.red);					
    				}
    			}
    		}
    		template.hideFlags= HideFlags.DontSave;
    	}
    	// ----------------------------------------------------------------------
    	public static void BuildInPrivateVariablePortTemplateCommon(float len, ref Texture2D template) {
    		// -- Create base type variable texture. --
            BuildInPublicVariablePortTemplateCommon(len, ref template);
            // -- Add Private varibale indication line. --
    		int lenInt= (int)(len+1f);
    		int borderSize= myScale > 1.5 ? 4 : (myScale > 1.25f ? 3 : (myScale > 0.9f ? 2 : (myScale > 0.5 ? 1 : 0)));
            iCS_TextureUtil.DrawLine(ref template, new Vector2(borderSize+1,borderSize+1), new Vector2(lenInt-borderSize-2,lenInt-borderSize-2), Color.red);
    		template.Apply();
    	}
    	// ----------------------------------------------------------------------
    	public static void BuildOutPrivateVariablePortTemplateCommon(float len, ref Texture2D template) {
    		// -- Create base type variable texture. --
            BuildOutPublicVariablePortTemplateCommon(len, ref template);
            // -- Add Private varibale indication line. --
    		int lenInt= (int)(len+1f);
    		int borderSize= myScale > 1.5 ? 4 : (myScale > 1.25f ? 3 : (myScale > 0.9f ? 2 : (myScale > 0.5 ? 1 : 0)));
            float lineWidth= myScale < 0.75f ? 1f : 1.5f*myScale;
            iCS_TextureUtil.DrawLine(ref template, new Vector2(borderSize+1,borderSize+1), new Vector2(lenInt-borderSize-2,lenInt-borderSize-2), Color.black, lineWidth);
    		template.hideFlags= HideFlags.DontSave;
    	}
    	// ----------------------------------------------------------------------
    	static void BuildLocalVariablePortTemplates(float scale) {
            float radius= scale*iCS_EditorConfig.PortRadius;
            float inInnerRadius= radius-2f*scale;
            float outInnerRadius= radius-2.5f*scale;
    		BuildInLocalVariablePortTemplate(radius, inInnerRadius, ref myInLocalVariablePortTemplate);
    		BuildOutLocalVariablePortTemplate(radius, outInnerRadius, ref myOutLocalVariablePortTemplate);
            float selectedFactor= iCS_EditorConfig.SelectedPortFactor;
    		float selectedRadius= selectedFactor*radius;
    		float selectedInInnerRadius= selectedFactor*inInnerRadius;
    		float selectedOutInnerRadius= selectedFactor*outInnerRadius;
    		BuildInLocalVariablePortTemplate(selectedRadius, selectedInInnerRadius, ref mySelectedInLocalVariablePortTemplate);
    		BuildOutLocalVariablePortTemplate(selectedRadius, selectedOutInnerRadius, ref mySelectedOutLocalVariablePortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	static void BuildConstantPortTemplates(float scale) {
            float radius= scale*iCS_EditorConfig.PortRadius;
            float inInnerRadius= radius-2f*scale;
    		BuildConstantPortTemplate(radius, inInnerRadius, ref myConstantPortTemplate);
            float selectedFactor= iCS_EditorConfig.SelectedPortFactor;
    		float selectedRadius= selectedFactor*radius;
    		float selectedInInnerRadius= selectedFactor*inInnerRadius;
    		BuildConstantPortTemplate(selectedRadius, selectedInInnerRadius, ref mySelectedConstantPortTemplate);
    	}
    	// ----------------------------------------------------------------------
        delegate void PortTemplateBuilder(float radius, float innerRadius, ref Texture2D template);
    	static void BuildPortTemplate(float radius, float innerRadius, ref Texture2D template, PortTemplateBuilder builder) {
            // Remove previous template.
            if(template != null) Texture2D.DestroyImmediate(template);
    		// Create texture.
    		int widthInt= (int)(2f*radius+3f);
    		int heightInt= (int)(2f*radius+3f);
    		template= new Texture2D(widthInt, heightInt, TextureFormat.ARGB32, false);
    		builder(radius, innerRadius, ref template);
    		// Finalize texture.
    		template.hideFlags= HideFlags.DontSave;
    		template.Apply();
    	}
    	// ----------------------------------------------------------------------
    	static void BuildInLocalVariablePortTemplate(float radius, float innerRadius, ref Texture2D template) {
            BuildPortTemplate(radius, innerRadius, ref template, BuildInLocalVariablePortTemplateImp);
    	}
    	// ----------------------------------------------------------------------
    	static void BuildOutLocalVariablePortTemplate(float radius, float innerRadius, ref Texture2D template) {
            BuildPortTemplate(radius, innerRadius, ref template, BuildOutLocalVariablePortTemplateImp);
    	}
    	// ----------------------------------------------------------------------
    	public static void BuildInLocalVariablePortTemplateImp(float radius, float innerRadius, ref Texture2D texture) {
            float cx= 0.5f*texture.width;
            float cy= 0.5f*texture.height;
            var center= new Vector2(cx,cy);
            iCS_TextureUtil.Clear(ref texture);
            iCS_TextureUtil.DrawFilledCircle(ref texture, radius, center, Color.black);
            iCS_TextureUtil.DrawCircle(ref texture, innerRadius, center, Color.red, 1f+0.25f*myScale);
    	}
    	// ----------------------------------------------------------------------
    	public static void BuildOutLocalVariablePortTemplateImp(float radius, float innerRadius, ref Texture2D texture) {
            float cx= 0.5f*texture.width;
            float cy= 0.5f*texture.height;
            var center= new Vector2(cx,cy);
            iCS_TextureUtil.Clear(ref texture);
            iCS_TextureUtil.DrawFilledCircle(ref texture, radius, center, Color.black);
            iCS_TextureUtil.DrawFilledCircle(ref texture, innerRadius, center, Color.red);
    	}
    	// ----------------------------------------------------------------------
    	public static void BuildConstantPortTemplate(float radius, float innerRadius, ref Texture2D template) {
            // -- Remove previous template. --
            if(template != null) Texture2D.DestroyImmediate(template);
    		// -- Create texture. --
    		int widthInt= (int)(2f*radius+3f);
    		int heightInt= (int)(2f*radius+3f);
    		template= new Texture2D(widthInt, heightInt, TextureFormat.ARGB32, false);
            // -- Build port image. --
            float cx= 0.5f*template.width;
            float cy= 0.5f*template.height;
            var center= new Vector2(cx,cy);
            iCS_TextureUtil.Fill(ref template, Color.black);
            iCS_TextureUtil.DrawCircle(ref template, innerRadius, center, Color.red, 1f+0.25f*myScale);
            int maxX= widthInt-1;
            int maxY= heightInt-1;
            iCS_TextureUtil.DrawLine(ref template, Vector2.zero, new Vector2(0, maxY), Color.green);
            iCS_TextureUtil.DrawLine(ref template, Vector2.zero, new Vector2(maxX, 0), Color.green);
            iCS_TextureUtil.DrawLine(ref template, new Vector2(maxX, maxY), new Vector2(maxX, 0), Color.green);
            iCS_TextureUtil.DrawLine(ref template, new Vector2(maxX, maxY), new Vector2(0, maxY), Color.green);
    		// -- Finalize texture. --
    		template.hideFlags= HideFlags.DontSave;
    		template.Apply();
    	}
    	// ----------------------------------------------------------------------
    	public static void BuildMuxPortTemplate(float width, float height, ref Texture2D texture, bool isInPort, float rotation= 0f) {
            // Compute texture size
            Vector2[] muxPolygon= null;
            int textureWidth= (int)(width+3f);
            int textureHeight= (int)(height+3f);
            if(Math3D.IsEqual(rotation, 0f)) {
                if(isInPort) {
                    muxPolygon= Math3D.FlipPolygonVertically(myMuxPortPolygon, 0f);
                } else {
                    muxPolygon= myMuxPortPolygon;                
                }
            } else if(Math3D.IsEqual(rotation, 90f)) {
                if(isInPort) {
                    muxPolygon= Math3D.Rotate90DegreesPolygon(myMuxPortPolygon);
                    muxPolygon= Math3D.FlipPolygonHorizontally(muxPolygon, 0f);
                } else {
                    muxPolygon= Math3D.Rotate90DegreesPolygon(myMuxPortPolygon);                
                }
                textureWidth= (int)(height+3f);
                textureHeight= (int)(width+3f);
            } else if(Math3D.IsEqual(rotation, 180f)) {
                if(isInPort) {
                    muxPolygon= myMuxPortPolygon;                                
                } else {
                    muxPolygon= Math3D.FlipPolygonVertically(myMuxPortPolygon, 0f);                
                }
            } else {
                if(isInPort) {
                    muxPolygon= Math3D.Rotate90DegreesPolygon(myMuxPortPolygon);                                
                } else {
                    muxPolygon= Math3D.Rotate90DegreesPolygon(myMuxPortPolygon);
                    muxPolygon= Math3D.FlipPolygonHorizontally(muxPolygon, 0f);                
                }
                textureWidth= (int)(height+3f);
                textureHeight= (int)(width+3f);
            }
            // Allocate texture
            if(texture != null) Texture2D.DestroyImmediate(texture);
            texture= new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
            iCS_TextureUtil.Clear(ref texture);
            // Draw black background polygon.
            var textureCenter= new Vector2(0.5f*textureWidth,0.5f*textureHeight);
            var outterPolygon= Math3D.ScaleAndTranslatePolygon(muxPolygon, new Vector2(height,height), textureCenter);
            iCS_TextureUtil.DrawFilledPolygon(ref texture, outterPolygon, Color.black);
            // Draw inner color polygon.
            var innerPolygon= Math3D.ScaleAndTranslatePolygon(muxPolygon, new Vector2(0.6f*height, 0.6f*height), textureCenter);
            if(isInPort) {
                iCS_TextureUtil.DrawPolygonOutline(ref texture, innerPolygon, Color.red);
            } else {
                iCS_TextureUtil.DrawFilledPolygon(ref texture, innerPolygon, Color.red);            
            }
            // Finalize texture.
    		texture.hideFlags= HideFlags.DontSave;
    		texture.Apply();
    	}
        // ----------------------------------------------------------------------
    	public static void BuildEnablePortTemplate(float scale, ref Texture2D texture) {
            float len= scale*iCS_EditorConfig.PortDiameter*1.4f;
    	    var borderSize= 2.8f*scale;
    	    if(borderSize < 1f) borderSize= 1f;
    	    int textureSize= (int)(len+3f);
    	    if(texture != null) Texture2D.DestroyImmediate(texture);
    		texture= new Texture2D(textureSize, textureSize);
    		iCS_TextureUtil.Clear(ref texture);
            // Draw black background polygon
            var textureCenter= new Vector2(0.5f*textureSize,0.5f*textureSize);
            var outterPolygon= Math3D.ScaleAndTranslatePolygon(myControlPortPolygon, new Vector2(len, len), textureCenter);
            iCS_TextureUtil.DrawFilledPolygon(ref texture, outterPolygon, Color.black);
            // Draw inner color polygon.
            var innerPolygon= Math3D.ScaleAndTranslatePolygon(myControlPortPolygon, new Vector2(0.6f*len, 0.6f*len), textureCenter);
            iCS_TextureUtil.DrawPolygonOutline(ref texture, innerPolygon, Color.red, 1.4f);
            // Finalize texture.
            texture.hideFlags= HideFlags.DontSave;
     		texture.Apply();
    	}
        // ----------------------------------------------------------------------
    	public static void BuildTriggerPortTemplate(float scale, ref Texture2D texture) {
            float len= scale*iCS_EditorConfig.PortDiameter*1.4f;
    	    var borderSize= 2.8f*scale;
    	    if(borderSize < 1f) borderSize= 1f;
    	    int textureSize= (int)(len+3f);
    	    if(texture != null) Texture2D.DestroyImmediate(texture);
    		texture= new Texture2D(textureSize, textureSize);
    		iCS_TextureUtil.Clear(ref texture);
            // Draw black background polygon
            var textureCenter= new Vector2(0.5f*textureSize,0.5f*textureSize);
            var outterPolygon= Math3D.ScaleAndTranslatePolygon(myControlPortPolygon, new Vector2(len, len), textureCenter);
            iCS_TextureUtil.DrawFilledPolygon(ref texture, outterPolygon, Color.black);
            // Draw inner color polygon.
            var innerPolygon= Math3D.ScaleAndTranslatePolygon(myControlPortPolygon, new Vector2(0.6f*len, 0.6f*len), textureCenter);
            iCS_TextureUtil.DrawFilledPolygon(ref texture, innerPolygon, Color.red);
            // Finalize texture.
            texture.hideFlags= HideFlags.DontSave;
     		texture.Apply();
    	}
        // ----------------------------------------------------------------------
    	public static void BuildInParameterPortTemplate(float scale, ref Texture2D texture) {
            float len= scale*iCS_EditorConfig.PortDiameter*1.4f;
    	    var borderSize= 2.8f*scale;
    	    if(borderSize < 1f) borderSize= 1f;
    	    int textureSize= (int)(len+3f);
    	    if(texture != null) Texture2D.DestroyImmediate(texture);
    		texture= new Texture2D(textureSize, textureSize);
    		iCS_TextureUtil.Clear(ref texture);
            // Draw black background polygon
            var textureCenter= new Vector2(0.5f*textureSize,0.5f*textureSize);
            var outterPolygon= Math3D.ScaleAndTranslatePolygon(myParameterPortPolygon, new Vector2(len, len), textureCenter);
            iCS_TextureUtil.DrawFilledPolygon(ref texture, outterPolygon, Color.black);
            // Draw inner color polygon.
            textureCenter= new Vector2(textureCenter.x, textureCenter.y-0.4f*borderSize);
            var innerPolygon= Math3D.ScaleAndTranslatePolygon(myParameterPortPolygon, new Vector2(0.6f*len, 0.6f*len), textureCenter);
            iCS_TextureUtil.DrawPolygonOutline(ref texture, innerPolygon, Color.red, 1.4f);
            // Finalize texture.
            texture.hideFlags= HideFlags.DontSave;
     		texture.Apply();
    	}
        // ----------------------------------------------------------------------
    	public static void BuildOutParameterPortTemplate(float scale, ref Texture2D texture) {
            float len= scale*iCS_EditorConfig.PortDiameter*1.4f;
    	    var borderSize= 2.8f*scale;
    	    if(borderSize < 1f) borderSize= 1f;
    	    int textureSize= (int)(len+3f);
    	    if(texture != null) Texture2D.DestroyImmediate(texture);
    		texture= new Texture2D(textureSize, textureSize);
    		iCS_TextureUtil.Clear(ref texture);
            // Draw black background polygon
            var textureCenter= new Vector2(0.5f*textureSize,0.5f*textureSize);
            var outterPolygon= Math3D.ScaleAndTranslatePolygon(myParameterPortPolygon, new Vector2(len, len), textureCenter);
            iCS_TextureUtil.DrawFilledPolygon(ref texture, outterPolygon, Color.black);
            // Draw inner color polygon.
            var innerPolygon= Math3D.ScaleAndTranslatePolygon(myParameterPortPolygon, new Vector2(0.6f*len, 0.6f*len), textureCenter);
            iCS_TextureUtil.DrawFilledPolygon(ref texture, innerPolygon, Color.red);
            // Finalize texture.
            texture.hideFlags= HideFlags.DontSave;
     		texture.Apply();
    	}
	
        // ======================================================================
        // Icon retreival functions
    	// ----------------------------------------------------------------------
    	// Returns a texture representing an enable port.
    	public static Texture2D GetEnablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myEnablePortIcons, ref myEnablePortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a trigger port.
    	public static Texture2D GetTriggerPortIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myTriggerPortIcons, ref myTriggerPortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a selected enable port.
    	public static Texture2D GetSelectedEnablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedEnablePortIcons, ref mySelectedEnablePortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a trigger port.
    	public static Texture2D GetSelectedTriggerPortIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedTriggerPortIcons, ref mySelectedTriggerPortTemplate);
    	}

    	// ----------------------------------------------------------------------
    	// Returns a texture representing a local variable port.
    	public static Texture2D GetInLocalVariablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myInLocalVariablePortIcons, ref myInLocalVariablePortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a local variable port.
    	public static Texture2D GetOutLocalVariablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myOutLocalVariablePortIcons, ref myOutLocalVariablePortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a selected local variable port.
    	public static Texture2D GetSelectedInLocalVariablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref mySelectedInLocalVariablePortIcons, ref mySelectedInLocalVariablePortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a selected local variable port.
    	public static Texture2D GetSelectedOutLocalVariablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref mySelectedOutLocalVariablePortIcons, ref mySelectedOutLocalVariablePortTemplate);
    	}

    	// ----------------------------------------------------------------------
    	// Returns a texture representing a public variable.
    	public static Texture2D GetInPublicVariablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myInPublicVariablePortIcons, ref myInPublicVariablePortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a public variable.
    	public static Texture2D GetOutPublicVariablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myOutPublicVariablePortIcons, ref myOutPublicVariablePortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a selected public variable.
    	public static Texture2D GetSelectedInPublicVariablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedInPublicVariablePortIcons, ref mySelectedInPublicVariablePortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a selected public variable.
    	public static Texture2D GetSelectedOutPublicVariablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedOutPublicVariablePortIcons, ref mySelectedOutPublicVariablePortTemplate);
    	}

    	// ----------------------------------------------------------------------
    	// Returns a texture representing a public variable.
    	public static Texture2D GetInStaticPublicVariablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myInStaticPublicVariablePortIcons, ref myInStaticPublicVariablePortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a public variable.
    	public static Texture2D GetOutStaticPublicVariablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myOutStaticPublicVariablePortIcons, ref myOutStaticPublicVariablePortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a selected public variable.
    	public static Texture2D GetSelectedInStaticPublicVariablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedInStaticPublicVariablePortIcons, ref mySelectedInStaticPublicVariablePortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a selected public variable.
    	public static Texture2D GetSelectedOutStaticPublicVariablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedOutStaticPublicVariablePortIcons, ref mySelectedOutStaticPublicVariablePortTemplate);
    	}

    	// ----------------------------------------------------------------------
    	// Returns a texture representing a private variable.
    	public static Texture2D GetInPrivateVariablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myInPrivateVariablePortIcons, ref myInPrivateVariablePortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a private variable.
    	public static Texture2D GetOutPrivateVariablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myOutPrivateVariablePortIcons, ref myOutPrivateVariablePortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a selected private variable.
    	public static Texture2D GetSelectedInPrivateVariablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedInPrivateVariablePortIcons, ref mySelectedInPrivateVariablePortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a selected private variable.
    	public static Texture2D GetSelectedOutPrivateVariablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedOutPrivateVariablePortIcons, ref mySelectedOutPrivateVariablePortTemplate);
    	}

    	// ----------------------------------------------------------------------
    	// Returns a texture representing a private variable.
    	public static Texture2D GetInStaticPrivateVariablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myInStaticPrivateVariablePortIcons, ref myInStaticPrivateVariablePortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a private variable.
    	public static Texture2D GetOutStaticPrivateVariablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myOutStaticPrivateVariablePortIcons, ref myOutStaticPrivateVariablePortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a selected private variable.
    	public static Texture2D GetSelectedInStaticPrivateVariablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedInStaticPrivateVariablePortIcons, ref mySelectedInStaticPrivateVariablePortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a selected private variable.
    	public static Texture2D GetSelectedOutStaticPrivateVariablePortIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedOutStaticPrivateVariablePortIcons, ref mySelectedOutStaticPrivateVariablePortTemplate);
    	}

    	// ----------------------------------------------------------------------
    	// Returns a texture representing a parameter port.
    	public static Texture2D GetInParameterPortIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myInParameterPortIcons, ref myInParameterPortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a parameter port.
    	public static Texture2D GetOutParameterPortIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myOutParameterPortIcons, ref myOutParameterPortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a selected parameter port.
    	public static Texture2D GetSelectedInParameterPortIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedInParameterPortIcons, ref mySelectedInParameterPortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a selected parameter port.
    	public static Texture2D GetSelectedOutParameterPortIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedOutParameterPortIcons, ref mySelectedOutParameterPortTemplate);
    	}

    	// ----------------------------------------------------------------------
    	// Returns a texture representing a parameter port.
    	public static Texture2D GetConstantPortIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myConstantPortIcons, ref myConstantPortTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing a selected parameter port.
    	public static Texture2D GetSelectedConstantPortIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedConstantPortIcons, ref mySelectedConstantPortTemplate);
    	}

    	// ----------------------------------------------------------------------
    	// Returns a texture representing the requested mux port icon.
    	public static Texture2D GetInMuxPortTopIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myInMuxPortTopIcons, ref myInMuxPortTopTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing the requested mux port icon.
    	public static Texture2D GetInMuxPortBottomIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myInMuxPortBottomIcons, ref myInMuxPortBottomTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing the requested mux port icon.
    	public static Texture2D GetInMuxPortLeftIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myInMuxPortLeftIcons, ref myInMuxPortLeftTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing the requested mux port icon.
    	public static Texture2D GetInMuxPortRightIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myInMuxPortRightIcons, ref myInMuxPortRightTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing the requested mux port icon.
    	public static Texture2D GetOutMuxPortTopIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myOutMuxPortTopIcons, ref myOutMuxPortTopTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing the requested mux port icon.
    	public static Texture2D GetOutMuxPortBottomIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myOutMuxPortBottomIcons, ref myOutMuxPortBottomTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing the requested mux port icon.
    	public static Texture2D GetOutMuxPortLeftIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myOutMuxPortLeftIcons, ref myOutMuxPortLeftTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing the requested mux port icon.
    	public static Texture2D GetOutMuxPortRightIcon(Color typeColor) {
    		return GetPortIcon(typeColor, ref myOutMuxPortRightIcons, ref myOutMuxPortRightTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing the requested mux port icon.
    	public static Texture2D GetSelectedInMuxPortTopIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedInMuxPortTopIcons, ref mySelectedInMuxPortTopTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing the requested mux port icon.
    	public static Texture2D GetSelectedInMuxPortBottomIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedInMuxPortBottomIcons, ref mySelectedInMuxPortBottomTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing the requested mux port icon.
    	public static Texture2D GetSelectedInMuxPortLeftIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedInMuxPortLeftIcons, ref mySelectedInMuxPortLeftTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing the requested mux port icon.
    	public static Texture2D GetSelectedInMuxPortRightIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedInMuxPortRightIcons, ref mySelectedInMuxPortRightTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing the requested mux port icon.
    	public static Texture2D GetSelectedOutMuxPortTopIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedOutMuxPortTopIcons, ref mySelectedOutMuxPortTopTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing the requested mux port icon.
    	public static Texture2D GetSelectedOutMuxPortBottomIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedOutMuxPortBottomIcons, ref mySelectedOutMuxPortBottomTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing the requested mux port icon.
    	public static Texture2D GetSelectedOutMuxPortLeftIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedOutMuxPortLeftIcons, ref mySelectedOutMuxPortLeftTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing the requested mux port icon.
    	public static Texture2D GetSelectedOutMuxPortRightIcon(Color typeColor) {
    		return GetPortIcon(typeColor,
    			               ref mySelectedOutMuxPortRightIcons, ref mySelectedOutMuxPortRightTemplate);
    	}
    	// ----------------------------------------------------------------------
    	// Returns a texture representing the requested circular port icon.
    	static Texture2D GetPortIcon(Color typeColor,
    								 ref Dictionary<Color,Texture2D> iconSet,
    								 ref Texture2D iconTemplate) {
    		if(iconSet == null) {
    			iconSet= new Dictionary<Color,Texture2D>();
    		}
    		if(iconSet.ContainsKey(typeColor)) {
    			var existingIcon= iconSet[typeColor];
    			if(existingIcon != null) {
    			    return existingIcon;
    		    }
    		}
    		Texture2D icon= BuildPortIcon(typeColor, Color.gray, iconTemplate);
    		iconSet[typeColor]= icon;
    		return icon;
    	}
    	// ----------------------------------------------------------------------
        public static Texture2D BuildPortIcon(Color typeColor, Texture2D iconTemplate) {
            return BuildPortIcon(typeColor, Color.gray, iconTemplate);
        }
    
    	// ----------------------------------------------------------------------
        public static Texture2D BuildPortIcon(Color typeColor, Color borderColor, Texture2D iconTemplate) {
    		int width= iconTemplate.width;
    		int height= iconTemplate.height;
    		Texture2D icon= new Texture2D(width, height);
    		for(int x= 0; x < width; ++x) {
    			for(int y= 0; y < height; ++y) {
    				Color pixel= iconTemplate.GetPixel(x,y);
    				if(pixel.r == 0 && pixel.g == 0) {
    					icon.SetPixel(x,y, pixel);
    				} else {
    					// Anti-Aliasing fill.
    					Color c;
    					c.r= pixel.r*typeColor.r + pixel.g*borderColor.r;
    					c.g= pixel.r*typeColor.g + pixel.g*borderColor.g;
    					c.b= pixel.r*typeColor.b + pixel.g*borderColor.b;
    					c.a= pixel.a;
    					icon.SetPixel(x,y, c);
    				}
    			}
    		}
    		icon.Apply();
    		icon.hideFlags= HideFlags.DontSave;
    		return icon;        
        }
    
    	// ----------------------------------------------------------------------
    	// Flush cached icons.
    	static void FlushCachedIcons() {
    		FlushCachedIcons(ref myEnablePortIcons);
    		FlushCachedIcons(ref myTriggerPortIcons);
    		FlushCachedIcons(ref mySelectedEnablePortIcons);
    		FlushCachedIcons(ref mySelectedTriggerPortIcons);

    		FlushCachedIcons(ref myInLocalVariablePortIcons);
    		FlushCachedIcons(ref myOutLocalVariablePortIcons);
    		FlushCachedIcons(ref mySelectedInLocalVariablePortIcons);
    		FlushCachedIcons(ref mySelectedOutLocalVariablePortIcons);

    		FlushCachedIcons(ref myInPublicVariablePortIcons);
    		FlushCachedIcons(ref myOutPublicVariablePortIcons);
    		FlushCachedIcons(ref mySelectedInPublicVariablePortIcons);
    		FlushCachedIcons(ref mySelectedOutPublicVariablePortIcons);

    		FlushCachedIcons(ref myInPrivateVariablePortIcons);
    		FlushCachedIcons(ref myOutPrivateVariablePortIcons);
    		FlushCachedIcons(ref mySelectedInPrivateVariablePortIcons);
    		FlushCachedIcons(ref mySelectedOutPrivateVariablePortIcons);

    		FlushCachedIcons(ref myInStaticPublicVariablePortIcons);
    		FlushCachedIcons(ref myOutStaticPublicVariablePortIcons);
    		FlushCachedIcons(ref mySelectedInStaticPublicVariablePortIcons);
    		FlushCachedIcons(ref mySelectedOutStaticPublicVariablePortIcons);

    		FlushCachedIcons(ref myInStaticPrivateVariablePortIcons);
    		FlushCachedIcons(ref myOutStaticPrivateVariablePortIcons);
    		FlushCachedIcons(ref mySelectedInStaticPrivateVariablePortIcons);
    		FlushCachedIcons(ref mySelectedOutStaticPrivateVariablePortIcons);

    		FlushCachedIcons(ref myInParameterPortIcons);
    		FlushCachedIcons(ref myOutParameterPortIcons);
    		FlushCachedIcons(ref mySelectedInParameterPortIcons);
    		FlushCachedIcons(ref mySelectedOutParameterPortIcons);

    		FlushCachedIcons(ref myConstantPortIcons);
    		FlushCachedIcons(ref mySelectedConstantPortIcons);

    		FlushCachedIcons(ref myOutMuxPortRightIcons);
    		FlushCachedIcons(ref mySelectedOutMuxPortRightIcons);
    	}
    	// ----------------------------------------------------------------------
    	static void FlushCachedIcons(ref Dictionary<Color,Texture2D> iconSet) {
    		if(iconSet == null) return;
    		foreach(var pair in iconSet) {
    			Texture2D.DestroyImmediate(pair.Value);
    		}
    	}
    }
    
}
