using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// TODO: Need to cleanup obsoleted data port icon code.
public static class iCS_PortIcons {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
	static float		myScale					        = 0f;
	static Texture2D	myInEndPortTemplate             = null;
	static Texture2D	myOutEndPortTemplate            = null;
	static Texture2D	myInRelayPortTemplate  	        = null;
	static Texture2D	myOutRelayPortTemplate  	    = null;
	static Texture2D    myOutMuxPortTemplate            = null;
    static Texture2D    myInTriggerPortTemplate         = null;
    static Texture2D    myOutTriggerPortTemplate        = null;
	static Texture2D	mySelectedInEndPortTemplate     = null;
	static Texture2D	mySelectedOutEndPortTemplate    = null;
	static Texture2D	mySelectedInRelayPortTemplate   = null;
	static Texture2D	mySelectedOutRelayPortTemplate  = null;
	static Texture2D    mySelectedOutMuxPortTemplate    = null;
	static Texture2D    mySelectedInTriggerPortTemplate = null;
	static Texture2D    mySelectedOutTriggerPortTemplate= null;
	
    // ----------------------------------------------------------------------
	static Dictionary<Color,Texture2D>	myInEndPortIcons             = null;
	static Dictionary<Color,Texture2D>	myOutEndPortIcons            = null;
	static Dictionary<Color,Texture2D>	myInRelayPortIcons           = null;
	static Dictionary<Color,Texture2D>	myOutRelayPortIcons          = null;
    static Dictionary<Color,Texture2D>  myOutMuxPortIcons            = null;
    static Dictionary<Color,Texture2D>  myInTriggerPortIcons         = null;
    static Dictionary<Color,Texture2D>  myOutTriggerPortIcons        = null;
	static Dictionary<Color,Texture2D>	mySelectedInEndPortIcons     = null;
	static Dictionary<Color,Texture2D>	mySelectedOutEndPortIcons    = null;
	static Dictionary<Color,Texture2D>	mySelectedInRelayPortIcons   = null;
	static Dictionary<Color,Texture2D>	mySelectedOutRelayPortIcons  = null;
	static Dictionary<Color,Texture2D>	mySelectedOutMuxPortIcons    = null;
	static Dictionary<Color,Texture2D>	mySelectedInTriggerPortIcons = null;
	static Dictionary<Color,Texture2D>	mySelectedOutTriggerPortIcons= null;

    // ======================================================================
    // PORT POLYGONS
    // ----------------------------------------------------------------------
    static Vector2[] myMuxPortPolygon    = null;
    static Vector2[] myTriggerPortPolygon= null;
    
    // ======================================================================
    // POLYGON BUILDER
    // ----------------------------------------------------------------------
    static iCS_PortIcons() {
        // Mux Port Polygon
        myMuxPortPolygon= new Vector2[4];
        myMuxPortPolygon[0]= new Vector2(-0.25f, -0.5f);
        myMuxPortPolygon[1]= new Vector2( 0.25f, -0.25f);
        myMuxPortPolygon[2]= new Vector2( 0.25f,  0.25f);
        myMuxPortPolygon[3]= new Vector2(-0.25f, 0.5f);        
        // Trigger Port Polygon
        myTriggerPortPolygon= new Vector2[4];
        myTriggerPortPolygon[0]= new Vector2( 0,   -0.5f);
        myTriggerPortPolygon[1]= new Vector2( 0.5f, 0);
        myTriggerPortPolygon[2]= new Vector2( 0,    0.5f);
        myTriggerPortPolygon[3]= new Vector2(-0.5f, 0);
    }

	// ----------------------------------------------------------------------
    //  Build template for all port icons
	public static void BuildPortIconTemplates(float scale) {
		if(Math3D.IsEqual(scale, myScale)) return;
		myScale= scale;
		BuildEndPortTemplates(scale);
		BuildRelayPortTemplates(scale);
        BuildMuxPortTemplates(scale);
        BuildTriggerPortTemplates(scale);
		FlushCachedIcons();
	}
	// ----------------------------------------------------------------------
	static void BuildRelayPortTemplates(float scale) {
        float len= scale*iCS_EditorConfig.PortDiameter;
        float selectedLen= len*iCS_EditorConfig.SelectedPortFactor;
		BuildInRelayPortTemplate(len, ref myInRelayPortTemplate);
		BuildOutRelayPortTemplate(len, ref myOutRelayPortTemplate);
		BuildInRelayPortTemplate(selectedLen, ref mySelectedInRelayPortTemplate);
		BuildOutRelayPortTemplate(selectedLen, ref mySelectedOutRelayPortTemplate);
	}
	// ----------------------------------------------------------------------
	static void BuildTriggerPortTemplates(float scale) {
        float selectedScale= scale*iCS_EditorConfig.SelectedPortFactor;
		BuildInTriggerPortTemplate(scale, ref myInTriggerPortTemplate);
		BuildInTriggerPortTemplate(selectedScale, ref mySelectedInTriggerPortTemplate);
		BuildOutTriggerPortTemplate(scale, ref myOutTriggerPortTemplate);
		BuildOutTriggerPortTemplate(selectedScale, ref mySelectedOutTriggerPortTemplate);
	}
	// ----------------------------------------------------------------------
	static void BuildMuxPortTemplates(float scale) {
        float width= scale*iCS_EditorConfig.PortDiameter;
        float height= width*2f;
        float selectedWidth= width*iCS_EditorConfig.SelectedPortFactor;
        float selectedHeight= height*iCS_EditorConfig.SelectedPortFactor;
		BuildMuxPortTemplate(width, height, ref myOutMuxPortTemplate);
		BuildMuxPortTemplate(selectedWidth, selectedHeight, ref mySelectedOutMuxPortTemplate);
	}
	// ----------------------------------------------------------------------
	public static void BuildInRelayPortTemplate(float len, ref Texture2D template) {
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
		template.Apply();
	}
	// ----------------------------------------------------------------------
	public static void BuildOutRelayPortTemplate(float len, ref Texture2D template) {
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
		template.Apply();
	}
	// ----------------------------------------------------------------------
	static void BuildEndPortTemplates(float scale) {
        float radius= scale*iCS_EditorConfig.PortRadius;
        float inInnerRadius= radius-2f*scale;
        float outInnerRadius= radius-2.5f*scale;
		BuildInEndPortTemplate(radius, inInnerRadius, ref myInEndPortTemplate);
		BuildOutEndPortTemplate(radius, outInnerRadius, ref myOutEndPortTemplate);
        float selectedFactor= iCS_EditorConfig.SelectedPortFactor;
		float selectedRadius= selectedFactor*radius;
		float selectedInInnerRadius= selectedFactor*inInnerRadius;
		float selectedOutInnerRadius= selectedFactor*outInnerRadius;
		BuildInEndPortTemplate(selectedRadius, selectedInInnerRadius, ref mySelectedInEndPortTemplate);
		BuildOutEndPortTemplate(selectedRadius, selectedOutInnerRadius, ref mySelectedOutEndPortTemplate);
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
	static void BuildInEndPortTemplate(float radius, float innerRadius, ref Texture2D template) {
        BuildPortTemplate(radius, innerRadius, ref template, BuildInEndPortTemplateImp);
	}
	// ----------------------------------------------------------------------
	static void BuildOutEndPortTemplate(float radius, float innerRadius, ref Texture2D template) {
        BuildPortTemplate(radius, innerRadius, ref template, BuildOutEndPortTemplateImp);
	}
	// ----------------------------------------------------------------------
	public static void BuildInEndPortTemplateImp(float radius, float innerRadius, ref Texture2D texture) {
        float cx= 0.5f*texture.width;
        float cy= 0.5f*texture.height;
        var center= new Vector2(cx,cy);
        iCS_TextureUtil.Clear(ref texture);
        iCS_TextureUtil.DrawFilledCircle(ref texture, radius, center, Color.black);
        iCS_TextureUtil.DrawCircle(ref texture, innerRadius, center, Color.red, 1f+0.25f*myScale);
	}
	// ----------------------------------------------------------------------
	public static void BuildOutEndPortTemplateImp(float radius, float innerRadius, ref Texture2D texture) {
        float cx= 0.5f*texture.width;
        float cy= 0.5f*texture.height;
        var center= new Vector2(cx,cy);
        iCS_TextureUtil.Clear(ref texture);
        iCS_TextureUtil.DrawFilledCircle(ref texture, radius, center, Color.black);
        iCS_TextureUtil.DrawFilledCircle(ref texture, innerRadius, center, Color.red);
	}
	// ----------------------------------------------------------------------
	public static void BuildMuxPortTemplate(float width, float height, ref Texture2D texture) {
        // Compute texture size
        int textureWidth= (int)(width+3f);
        int textureHeight= (int)(height+3f);
        // Allocate texture
        if(texture != null) Texture2D.DestroyImmediate(texture);
        texture= new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
        iCS_TextureUtil.Clear(ref texture);
        // Draw black background polygon.
        var textureCenter= new Vector2(0.5f*textureWidth,0.5f*textureHeight);
        var outterPolygon= Math3D.ScaleAndTranslatePolygon(myMuxPortPolygon, new Vector2(height,height), textureCenter);
        iCS_TextureUtil.DrawFilledPolygon(ref texture, outterPolygon, Color.black);
        // Draw inner color polygon.
        var innerPolygon= Math3D.ScaleAndTranslatePolygon(myMuxPortPolygon, new Vector2(0.6f*height, 0.6f*height), textureCenter);
        iCS_TextureUtil.DrawFilledPolygon(ref texture, innerPolygon, Color.red);
        // Finalize texture.
		texture.hideFlags= HideFlags.DontSave;
		texture.Apply();
	}
    // ----------------------------------------------------------------------
	public static void BuildInTriggerPortTemplate(float scale, ref Texture2D texture) {
        float len= scale*iCS_EditorConfig.PortDiameter*1.4f;
	    var borderSize= 2.8f*scale;
	    if(borderSize < 1f) borderSize= 1f;
	    int textureSize= (int)(len+3f);
	    if(texture != null) Texture2D.DestroyImmediate(texture);
		texture= new Texture2D(textureSize, textureSize);
		iCS_TextureUtil.Clear(ref texture);
        // Draw black background polygon
        var textureCenter= new Vector2(0.5f*textureSize,0.5f*textureSize);
        var outterPolygon= Math3D.ScaleAndTranslatePolygon(myTriggerPortPolygon, new Vector2(len, len), textureCenter);
        iCS_TextureUtil.DrawFilledPolygon(ref texture, outterPolygon, Color.black);
        // Draw inner color polygon.
        var innerPolygon= Math3D.ScaleAndTranslatePolygon(myTriggerPortPolygon, new Vector2(0.6f*len, 0.6f*len), textureCenter);
        iCS_TextureUtil.DrawPolygonOutline(ref texture, innerPolygon, Color.red, 1.4f);
        // Finalize texture.
        texture.hideFlags= HideFlags.DontSave;
 		texture.Apply();
	}
    // ----------------------------------------------------------------------
	public static void BuildOutTriggerPortTemplate(float scale, ref Texture2D texture) {
        float len= scale*iCS_EditorConfig.PortDiameter*1.4f;
	    var borderSize= 2.8f*scale;
	    if(borderSize < 1f) borderSize= 1f;
	    int textureSize= (int)(len+3f);
	    if(texture != null) Texture2D.DestroyImmediate(texture);
		texture= new Texture2D(textureSize, textureSize);
		iCS_TextureUtil.Clear(ref texture);
        // Draw black background polygon
        var textureCenter= new Vector2(0.5f*textureSize,0.5f*textureSize);
        var outterPolygon= Math3D.ScaleAndTranslatePolygon(myTriggerPortPolygon, new Vector2(len, len), textureCenter);
        iCS_TextureUtil.DrawFilledPolygon(ref texture, outterPolygon, Color.black);
        // Draw inner color polygon.
        var innerPolygon= Math3D.ScaleAndTranslatePolygon(myTriggerPortPolygon, new Vector2(0.6f*len, 0.6f*len), textureCenter);
        iCS_TextureUtil.DrawFilledPolygon(ref texture, innerPolygon, Color.red);
        // Finalize texture.
        texture.hideFlags= HideFlags.DontSave;
 		texture.Apply();
	}
	
    // ======================================================================
    // Icon retreival functions
	// ----------------------------------------------------------------------
	// Returns a texture representing the requested circular port icon.
	public static Texture2D GetInEndPortIcon(Color typeColor) {
		return GetPortIcon(typeColor, ref myInEndPortIcons, ref myInEndPortTemplate);
	}
	// ----------------------------------------------------------------------
	// Returns a texture representing the requested end port icon.
	public static Texture2D GetOutEndPortIcon(Color typeColor) {
		return GetPortIcon(typeColor, ref myOutEndPortIcons, ref myOutEndPortTemplate);
	}
	// ----------------------------------------------------------------------
	// Returns a texture representing the requested relay port icon.
	public static Texture2D GetInRelayPortIcon(Color typeColor) {
		return GetPortIcon(typeColor, ref myInRelayPortIcons, ref myInRelayPortTemplate);
	}
	// ----------------------------------------------------------------------
	// Returns a texture representing the requested relay port icon.
	public static Texture2D GetOutRelayPortIcon(Color typeColor) {
		return GetPortIcon(typeColor, ref myOutRelayPortIcons, ref myOutRelayPortTemplate);
	}
	// ----------------------------------------------------------------------
	// Returns a texture representing the requested mux port icon.
	public static Texture2D GetOutMuxPortIcon(Color typeColor) {
		return GetPortIcon(typeColor, ref myOutMuxPortIcons, ref myOutMuxPortTemplate);
	}
	// ----------------------------------------------------------------------
	// Returns a texture representing the requested mux port icon.
	public static Texture2D GetInTriggerPortIcon(Color typeColor) {
		return GetPortIcon(typeColor, ref myInTriggerPortIcons, ref myInTriggerPortTemplate);
	}
	// ----------------------------------------------------------------------
	// Returns a texture representing the requested port icon.
	public static Texture2D GetOutTriggerPortIcon(Color typeColor) {
		return GetPortIcon(typeColor, ref myOutTriggerPortIcons, ref myOutTriggerPortTemplate);
	}
	// ----------------------------------------------------------------------
	// Returns a texture representing the requested end port icon.
	public static Texture2D GetSelectedInEndPortIcon(Color typeColor) {
		return GetPortIcon(typeColor, ref mySelectedInEndPortIcons, ref mySelectedInEndPortTemplate);
	}
	// ----------------------------------------------------------------------
	// Returns a texture representing the requested end port icon.
	public static Texture2D GetSelectedOutEndPortIcon(Color typeColor) {
		return GetPortIcon(typeColor, ref mySelectedOutEndPortIcons, ref mySelectedOutEndPortTemplate);
	}
	// ----------------------------------------------------------------------
	// Returns a texture representing the requested relay port icon.
	public static Texture2D GetSelectedInRelayPortIcon(Color typeColor) {
		return GetPortIcon(typeColor,
			               ref mySelectedInRelayPortIcons, ref mySelectedInRelayPortTemplate);
	}
	// ----------------------------------------------------------------------
	// Returns a texture representing the requested relay port icon.
	public static Texture2D GetSelectedOutRelayPortIcon(Color typeColor) {
		return GetPortIcon(typeColor,
			               ref mySelectedOutRelayPortIcons, ref mySelectedOutRelayPortTemplate);
	}
	// ----------------------------------------------------------------------
	// Returns a texture representing the requested mux port icon.
	public static Texture2D GetSelectedOutMuxPortIcon(Color typeColor) {
		return GetPortIcon(typeColor,
			               ref mySelectedOutMuxPortIcons, ref mySelectedOutMuxPortTemplate);
	}
	// ----------------------------------------------------------------------
	// Returns a selected trigger port icon.
	public static Texture2D GetSelectedInTriggerPortIcon(Color typeColor) {
		return GetPortIcon(typeColor,
			               ref mySelectedInTriggerPortIcons, ref mySelectedInTriggerPortTemplate);
	}
	// ----------------------------------------------------------------------
	// Returns a selected trigger port icon.
	public static Texture2D GetSelectedOutTriggerPortIcon(Color typeColor) {
		return GetPortIcon(typeColor,
			               ref mySelectedOutTriggerPortIcons, ref mySelectedOutTriggerPortTemplate);
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
		Texture2D icon= BuildPortIcon(typeColor, iconTemplate);
		iconSet[typeColor]= icon;
		return icon;
	}
	// ----------------------------------------------------------------------
    public static Texture2D BuildPortIcon(Color typeColor, Texture2D iconTemplate) {
		int width= iconTemplate.width;
		int height= iconTemplate.height;
		Texture2D icon= new Texture2D(width, height);
		for(int x= 0; x < width; ++x) {
			for(int y= 0; y < height; ++y) {
				Color pixel= iconTemplate.GetPixel(x,y);
				if(pixel.r == 0) {
					icon.SetPixel(x,y, pixel);					
				} else {
					// Anti-Aliasing fill.
					Color c;
					c.r= pixel.r*typeColor.r;
					c.g= pixel.r*typeColor.g;
					c.b= pixel.r*typeColor.b;
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
		FlushCachedIcons(ref myInEndPortIcons);
		FlushCachedIcons(ref myOutEndPortIcons);
		FlushCachedIcons(ref myInRelayPortIcons);
		FlushCachedIcons(ref myOutRelayPortIcons);
		FlushCachedIcons(ref myOutMuxPortIcons);
		FlushCachedIcons(ref myInTriggerPortIcons);
		FlushCachedIcons(ref myOutTriggerPortIcons);
		FlushCachedIcons(ref mySelectedInEndPortIcons);
		FlushCachedIcons(ref mySelectedOutEndPortIcons);
		FlushCachedIcons(ref mySelectedInRelayPortIcons);
		FlushCachedIcons(ref mySelectedOutRelayPortIcons);
		FlushCachedIcons(ref mySelectedOutMuxPortIcons);
		FlushCachedIcons(ref mySelectedInTriggerPortIcons);
		FlushCachedIcons(ref mySelectedOutTriggerPortIcons);
	}
	// ----------------------------------------------------------------------
	static void FlushCachedIcons(ref Dictionary<Color,Texture2D> iconSet) {
		if(iconSet == null) return;
		foreach(var pair in iconSet) {
			Texture2D.DestroyImmediate(pair.Value);
		}
	}
}
