using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public static class iCS_PortIcons {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
	static Texture2D	myCircularPortIconTemplate= null;
	static Texture2D	mySquarePortIconTemplate  = null;

    // ----------------------------------------------------------------------
	static Dictionary<Color,Dictionary<Color,Texture2D>>	myCircularPortIcons= null;
	static Dictionary<Color,Dictionary<Color,Texture2D>>	mySquarePortIcons  = null;

	// ----------------------------------------------------------------------
    //  Build template for all port icons
	public static void BuildPortIconTemplates(float scale) {
//		Debug.Log("Rebuilding port icons: "+scale);
		BuildCircularPortIconTemplates(scale);
		BuildSquarePortIconTemplates(scale);
		FlushCachedIcons();
	}
	// ----------------------------------------------------------------------
	static void BuildSquarePortIconTemplates(float scale) {
        float len= scale*iCS_Config.PortRadius*3.7f;
		float margin= len*0.3f;
		
		// Create texture.
		int lenInt= (int)(len+1f);
		int marginInt= (int)(margin);
		int topMarginInt= (int)(len-margin);
		mySquarePortIconTemplate= new Texture2D(lenInt, lenInt);
		for(int x= 0; x < lenInt; ++x) {
			for(int y= 0; y < lenInt; ++y) {
				if(x == 0 || y == 0 || x == lenInt-1 || y == lenInt-1) {
					mySquarePortIconTemplate.SetPixel(x,y,Color.red);
				} else  if(x < marginInt || x > topMarginInt || y < marginInt || y > topMarginInt) {
					mySquarePortIconTemplate.SetPixel(x,y,Color.black);
				} else {
					mySquarePortIconTemplate.SetPixel(x,y,Color.blue);					
				}
			}
		}
		mySquarePortIconTemplate.Apply();
	}
	// ----------------------------------------------------------------------
	static void BuildCircularPortIconTemplates(float scale) {
        float radius= scale*iCS_Config.PortRadius*1.85f;
		float innerRadius= 0.6f*radius;
		// Create texture.
		int widthInt= (int)(2f*radius+2f);
		int heightInt= (int)(2f*radius+2f);
		myCircularPortIconTemplate= new Texture2D(widthInt, heightInt);
		for(int i= 0; i < widthInt; ++i) {
			for(int j= 0; j < heightInt; ++j) {
				myCircularPortIconTemplate.SetPixel(i,j,Color.clear);
			}
		}
		float mx= 0.5f*myCircularPortIconTemplate.width;
		float my= 0.5f*myCircularPortIconTemplate.height;
        float steps= 360f/(4f*Mathf.PI*radius);
        for(float angle= 0f; angle < 360f; angle+= steps) {
			float rad= angle*Mathf.Deg2Rad;
            float s= Mathf.Sin(rad);
            float c= Mathf.Cos(rad);
			int x;
			int y;
			float r;
			for(r= 0f; r < innerRadius; r+=0.9f) {
				x= (int)(mx+r*c);
				y= (int)(my+r*s);
				myCircularPortIconTemplate.SetPixel(x,y,Color.blue);
			}
			for(; r < radius; r+= 0.9f) {
				x= (int)(mx+r*c);
				y= (int)(my+r*s);				
				myCircularPortIconTemplate.SetPixel(x,y,Color.black);
			}
			x= (int)(mx+radius*c);
			y= (int)(my+radius*s);
			myCircularPortIconTemplate.SetPixel(x,y,Color.red); 
		}
		myCircularPortIconTemplate.Apply();
	}

	// ----------------------------------------------------------------------
	// Returns a texture representing the requested circular port icon.
	public static Texture2D GetCircularPortIcon(Color nodeColor, Color typeColor) {
		return GetPortIcon(nodeColor, typeColor, ref myCircularPortIcons, ref myCircularPortIconTemplate);
	}
	// ----------------------------------------------------------------------
	// Returns a texture representing the requested square port icon.
	public static Texture2D GetSquarePortIcon(Color nodeColor, Color typeColor) {
		return GetPortIcon(nodeColor, typeColor, ref mySquarePortIcons, ref mySquarePortIconTemplate);
	}
	// ----------------------------------------------------------------------
	// Returns a texture representing the requested circular port icon.
	static Texture2D GetPortIcon(Color nodeColor, Color typeColor,
								 ref Dictionary<Color,Dictionary<Color,Texture2D>> iconSet,
								 ref Texture2D iconTemplate) {
		if(iconSet == null) {
			iconSet= new Dictionary<Color,Dictionary<Color,Texture2D>>();
		}
		Dictionary<Color,Texture2D> dict;
		if(iconSet.ContainsKey(nodeColor)) {
			dict= iconSet[nodeColor];
		} else {
			dict= new Dictionary<Color,Texture2D>();
			iconSet[nodeColor]= dict;
		}
		if(dict.ContainsKey(typeColor)) {
			var existingIcon= dict[typeColor];
			if(existingIcon != null) return existingIcon;
		}
		int width= iconTemplate.width;
		int height= iconTemplate.height;
		Texture2D icon= new Texture2D(width, height);
		for(int x= 0; x < width; ++x) {
			for(int y= 0; y < height; ++y) {
				Color pixel= iconTemplate.GetPixel(x,y);
				if(pixel.r != 0) {
					icon.SetPixel(x,y,nodeColor);
				} else if(pixel.b != 0) {
					icon.SetPixel(x,y, typeColor);
				} else {
					icon.SetPixel(x,y, pixel);
				}
			}
		}
		icon.Apply();
		icon.hideFlags= HideFlags.DontSave;
		dict[typeColor]= icon;
		return icon;
	}
	// ----------------------------------------------------------------------
	// Flush cached icons.
	static void FlushCachedIcons() {
		FlushCachedIcons(ref myCircularPortIcons);
		FlushCachedIcons(ref mySquarePortIcons);
	}
	// ----------------------------------------------------------------------
	static void FlushCachedIcons(ref Dictionary<Color,Dictionary<Color,Texture2D>> iconSet) {
		if(iconSet == null) return;
		foreach(var dictPair in iconSet) {
			foreach(var pair in dictPair.Value) {
				Texture2D.DestroyImmediate(pair.Value);
			}
		}
		iconSet= null;
	}
}
