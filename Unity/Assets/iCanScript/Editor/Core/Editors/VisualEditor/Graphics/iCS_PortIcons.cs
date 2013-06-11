using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// TODO: Need to cleanup obsoleted data port icon code.
public static class iCS_PortIcons {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
	static float		myScale					       = 0f;
	static Texture2D	myInEndPortTemplate            = null;
	static Texture2D	myOutEndPortTemplate           = null;
	static Texture2D	myInRelayPortTemplate  	       = null;
	static Texture2D	myOutRelayPortTemplate  	   = null;
	static Texture2D    myMuxPortTemplate              = null;
    static Texture2D    myTriggerPortTemplate          = null;
	static Texture2D	mySelectedInEndPortTemplate    = null;
	static Texture2D	mySelectedOutEndPortTemplate   = null;
	static Texture2D	mySelectedInRelayPortTemplate  = null;
	static Texture2D	mySelectedOutRelayPortTemplate = null;
	static Texture2D    mySelectedMuxPortTemplate      = null;
	static Texture2D    mySelectedTriggerPortTemplate  = null;
	
    // ----------------------------------------------------------------------
	static Dictionary<Color,Texture2D>	myInEndPortIcons           = null;
	static Dictionary<Color,Texture2D>	myOutEndPortIcons          = null;
	static Dictionary<Color,Texture2D>	myInRelayPortIcons         = null;
	static Dictionary<Color,Texture2D>	myOutRelayPortIcons        = null;
    static Dictionary<Color,Texture2D>  myMuxPortIcons             = null;
    static Dictionary<Color,Texture2D>  myTriggerPortIcons         = null;
	static Dictionary<Color,Texture2D>	mySelectedInEndPortIcons   = null;
	static Dictionary<Color,Texture2D>	mySelectedOutEndPortIcons  = null;
	static Dictionary<Color,Texture2D>	mySelectedInRelayPortIcons = null;
	static Dictionary<Color,Texture2D>	mySelectedOutRelayPortIcons= null;
	static Dictionary<Color,Texture2D>	mySelectedMuxPortIcons     = null;
	static Dictionary<Color,Texture2D>	mySelectedTriggerPortIcons = null;

	// ----------------------------------------------------------------------
    //  Build template for all port icons
	public static void BuildPortIconTemplates(float scale) {
		if(Math3D.IsEqual(scale, myScale)) return;
		myScale= scale;
		BuildEndPortTemplates(scale);
		BuildRelayPortTemplates(scale);
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
        float len= scale*iCS_EditorConfig.PortDiameter*1.4f;
        float selectedLen= len*iCS_EditorConfig.SelectedPortFactor;
		BuildTriggerPortTemplate(len, ref myTriggerPortTemplate);
		BuildTriggerPortTemplate(selectedLen, ref mySelectedTriggerPortTemplate);
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
//	// ----------------------------------------------------------------------
//	public static void BuildMuxPortTemplateImp(float w1, float w2, float h1, float h2, float borderSize, float aaWidth, ref Texture2D texture) {
//        // Compute dimensions of mux port
//        float width = Mathf.Max(w1,w2);
//        float height= Mathf.Max(h1,h2);
//        float halfWidth= 0.5f*width;
//        float halfHeight= 0.5f*height;
//        // Compute texture size
//        int textureWidth= (int)(width+1f);
//        int textureHeight= (int)(height+1f);
//        // Allocate texture
//        if(texture != null) Texture2D.DestroyImmediate(texture);
//        texture= new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
//        // Compute points
//        Vector2 outterTopLeft    = new Vector2(0.5f*(width-w1),       0.5f*(height-h1));
//        Vector2 outterTopRight   = new Vector2(width-0.5f*(width-w1), 0.5f*(height-h2));
//        Vector2 outterBottomRight= new Vector2(width-0.5f*(width-w2), height-0.5f*(height-h2));
//        Vector2 outterBottomLeft = new Vector2(0.5f*(width-w2),       height-0.5f*(height-h1));
//        Vector2 innerTopLeft     = new Vector2(outterTopLeft.x+borderSize,     outterTopLeft.y+borderSize);
//        Vector2 innerTopRight    = new Vector2(outterTopRight.x-borderSize,    outterTopRight.y+borderSize);
//        Vector2 innerBottomRight = new Vector2(outterBottomRight.x-borderSize, outterBottomRight.y-borderSize);
//        Vector2 innerBottomLeft  = new Vector2(outterBottomLeft.x+borderSize,  outterBottomLeft.y-borderSize);
//        // Build mux port.
//        for(int x= 0; x < texture.width; ++x) {
//            for(int y= 0; y < texture.height; ++y) {
//                float xLeft= outterTopLeft.x+(outterBottomLeft.x-outterTopLeft.x)*(outterTopLeft.y-y);
//                float xRight= outterTopRight.x+(outterBottomRight.x-outterTopRight.x)*(outterTopRight.y-y);
//                float yTop= outterTopRight.y+(outterTopLeft.y-outterTopRight.y)*(outterTopLeft.x-x);
//                float yBottom= outterBottomLeft.y+(outterBottomRight.y-outterBottomLeft.y)*(outterBottomLeft.x-x);
//                // TODO: Mux Port Icon to be completed...
//            }
//        }
//        // Finalize texture.
//		texture.hideFlags= HideFlags.DontSave;
//		texture.Apply();
//	}
    // ----------------------------------------------------------------------
	public static void BuildTriggerPortTemplate(float len, ref Texture2D texture) {
	    var borderSize= 2.8f*myScale;
	    if(borderSize < 1f) borderSize= 1f;
	    int textureSize= (int)(len+3f);
	    if(texture != null) Texture2D.DestroyImmediate(texture);
		texture= new Texture2D(textureSize, textureSize);
		iCS_TextureUtil.Clear(ref texture);
        // Build polygons.
        var halfLen= 0.5f*(len+1f);
        var outterPolygon= new Vector2[4];
        outterPolygon[0]= new Vector2(1, halfLen);
        outterPolygon[1]= new Vector2(halfLen, 1);
        outterPolygon[2]= new Vector2(len, halfLen);
        outterPolygon[3]= new Vector2(halfLen, len);
        var innerPolygon= new Vector2[4];
        innerPolygon[0]= new Vector2(1+borderSize, halfLen);
        innerPolygon[1]= new Vector2(halfLen, 1+borderSize);
        innerPolygon[2]= new Vector2(len-borderSize, halfLen);
        innerPolygon[3]= new Vector2(halfLen, len-borderSize);
        iCS_TextureUtil.DrawFilledPolygon(ref texture, outterPolygon, Color.black);
        iCS_TextureUtil.DrawPolygonOutline(ref texture, innerPolygon, Color.red, 1.4f);
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
	public static Texture2D GetMuxPortIcon(Color typeColor) {
		return GetPortIcon(typeColor, ref myMuxPortIcons, ref myMuxPortTemplate);
	}
	// ----------------------------------------------------------------------
	// Returns a texture representing the requested mux port icon.
	public static Texture2D GetTriggerPortIcon(Color typeColor) {
		return GetPortIcon(typeColor, ref myTriggerPortIcons, ref myTriggerPortTemplate);
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
	public static Texture2D GetSelectedMuxPortIcon(Color typeColor) {
		return GetPortIcon(typeColor,
			               ref mySelectedMuxPortIcons, ref mySelectedMuxPortTemplate);
	}
	// ----------------------------------------------------------------------
	// Returns a selected trigger port icon.
	public static Texture2D GetSelectedTriggerPortIcon(Color typeColor) {
		return GetPortIcon(typeColor,
			               ref mySelectedTriggerPortIcons, ref mySelectedTriggerPortTemplate);
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
		FlushCachedIcons(ref myMuxPortIcons);
		FlushCachedIcons(ref myTriggerPortIcons);
		FlushCachedIcons(ref mySelectedInEndPortIcons);
		FlushCachedIcons(ref mySelectedOutEndPortIcons);
		FlushCachedIcons(ref mySelectedInRelayPortIcons);
		FlushCachedIcons(ref mySelectedOutRelayPortIcons);
		FlushCachedIcons(ref mySelectedMuxPortIcons);
		FlushCachedIcons(ref mySelectedTriggerPortIcons);
	}
	// ----------------------------------------------------------------------
	static void FlushCachedIcons(ref Dictionary<Color,Texture2D> iconSet) {
		if(iconSet == null) return;
		foreach(var pair in iconSet) {
			Texture2D.DestroyImmediate(pair.Value);
		}
	}
}
