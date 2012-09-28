using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class iCS_NodeTextures {
    // ======================================================================
    // CONSTANTS
    // ----------------------------------------------------------------------
    const float kNodeCornerRadius= 8f;
    const float kNodeTitleHeight = 2f*kNodeCornerRadius;
	const float kShadowAlpha     = 0.4f;

    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
	static Texture2D	myNodeTemplate   = null;

    // ----------------------------------------------------------------------
	static Dictionary<Color,Dictionary<Color,Dictionary<Color,Texture2D>>>	myNodeTextures= null;
	
    // ----------------------------------------------------------------------
	public static Texture2D GetNodeTexture(Color nodeColor, Color backColor, Color shadowColor) {
		if(myNodeTextures == null) {
			myNodeTextures= new Dictionary<Color, Dictionary<Color, Dictionary<Color,Texture2D> > >();
		}
		if(!myNodeTextures.ContainsKey(nodeColor)) {
			myNodeTextures[nodeColor]= new Dictionary<Color, Dictionary<Color, Texture2D>>();
		}
		var nodeDir= myNodeTextures[nodeColor];
		if(!nodeDir.ContainsKey(backColor)) {
			nodeDir[backColor]=new Dictionary<Color, Texture2D>();
		}
		var backDir= nodeDir[backColor];
		if(backDir.ContainsKey(shadowColor)) {
			Texture2D nodeTexture= backDir[shadowColor];
			if(nodeTexture != null) return nodeTexture;
		}
		Texture2D texture= new Texture2D(myNodeTemplate.width, myNodeTemplate.height);
		for(int x= 0; x < myNodeTemplate.width; ++x) {
			for(int y= 0; y < myNodeTemplate.height; ++y) {
				Color pixel= myNodeTemplate.GetPixel(x,y);
				if(pixel.g != 0) {
					texture.SetPixel(x,y,nodeColor);
				} else if(pixel.b != 0) {
					texture.SetPixel(x,y,backColor);
				} else if(pixel.r != 0) {
					Color sc= shadowColor;
					sc.a= pixel.a*shadowColor.a;
					texture.SetPixel(x,y,sc);
				} else {
					texture.SetPixel(x,y,pixel);
				}
			}
		}
		texture.Apply();
		texture.hideFlags= HideFlags.DontSave;
		backDir[shadowColor]= texture;
		return texture;
	}
    // ----------------------------------------------------------------------
	// Creates a node template tiled texture
	public static void BuildNodeTemplate(float scale) {
		// Flush all cached textures.
		FlushCachedTextures();
		
        float radius= kNodeCornerRadius*scale;

		int radiusInt    = (int)(radius+0.5f);
		int extraTitleheightInt= (int)(0.75f*radius+0.5f);
		int shadowSizeInt= (int)iCS_Config.NodeShadowSize;
		int cornerOffset= shadowSizeInt+radiusInt;
		int tileSize= radiusInt+extraTitleheightInt+shadowSizeInt;
		int textureSize= 3*tileSize;
		myNodeTemplate= new Texture2D(textureSize,textureSize);
		for(int i= 0; i < textureSize; ++i) {
			for(int j= 0; j < textureSize; ++j) {
				myNodeTemplate.SetPixel(i,j,Color.clear);
			}
		}
		// Draw shadow
		int farCornerOffset= textureSize-cornerOffset-1;
		float alpha= 0.4f;
		float alphaDelta= 0.06f;
		for(int i= shadowSizeInt; i > 0; --i, alpha+= alphaDelta, alphaDelta+=0.06f) {
			Color c= Color.red;
			c.a= alpha;
//		float alpha= kShadowAlpha;
//		for(int i= shadowSizeInt; i > 0; --i, alpha= kShadowAlpha+(1f-kShadowAlpha)*alpha) {
//			Color c= Color.red;
//			c.a= alpha;
			DrawArc(radiusInt, c, c, farCornerOffset+i, farCornerOffset-i,  1,  1, ref myNodeTemplate);
			DrawArc(radiusInt, c, c, farCornerOffset+i, cornerOffset-i   ,  1, -1, ref myNodeTemplate);
			DrawArc(radiusInt, c, c, cornerOffset+i   , cornerOffset-i   , -1, -1, ref myNodeTemplate);			
			for(int j= cornerOffset; j < farCornerOffset; ++j) {
				myNodeTemplate.SetPixel(j+i,shadowSizeInt-i,c);
				myNodeTemplate.SetPixel(textureSize-shadowSizeInt-1+i,j-i,c);								
			}
		}
		// Top right corner.
		DrawArc(radiusInt, Color.green, Color.green, farCornerOffset, farCornerOffset, 1, 1, ref myNodeTemplate);
		// Top Left corner.
		DrawArc(radiusInt, Color.green, Color.green, cornerOffset, farCornerOffset, -1, 1, ref myNodeTemplate);
		// Bottom right corner.
		DrawArc(radiusInt, Color.green, Color.blue, farCornerOffset, cornerOffset, 1, -1, ref myNodeTemplate);
		// Bottom left corner.
		DrawArc(radiusInt, Color.green, Color.blue, cornerOffset, cornerOffset, -1, -1, ref myNodeTemplate);
		
		// Draw filled lines.
		for(int i= cornerOffset; i < textureSize-cornerOffset; ++i) {
			myNodeTemplate.SetPixel(i,shadowSizeInt,Color.green);
			myNodeTemplate.SetPixel(shadowSizeInt,i, Color.green);
			myNodeTemplate.SetPixel(textureSize-shadowSizeInt,i, Color.green);			
			for(int j= shadowSizeInt; j < cornerOffset; ++j) {
				myNodeTemplate.SetPixel(i,textureSize-j, Color.green);							
			}
			for(int j= shadowSizeInt+1; j <= cornerOffset; ++j) {
				myNodeTemplate.SetPixel(i,j, Color.blue);							
			}
		}
		// Draw extra title height.
		for(int i= shadowSizeInt; i < textureSize-shadowSizeInt; ++i) {
			for(int j= 2*tileSize; j <= textureSize-cornerOffset; ++j) {
				myNodeTemplate.SetPixel(i,j, Color.green);				
			}
		}
		// Draw background.
		for(int i= shadowSizeInt+1; i < textureSize-shadowSizeInt; ++i) {
			for(int j= cornerOffset+1; j < 2*tileSize; ++j) {
				myNodeTemplate.SetPixel(i,j, Color.blue);											
			}
		}
		

		myNodeTemplate.hideFlags= HideFlags.DontSave;
		myNodeTemplate.Apply();
	}
	//		GUI.DrawTextureWithTexCoords();
	
    // ----------------------------------------------------------------------
	// Draw a filled 90 degree arc into a texture.
	static void DrawArc(int radius,
		                Color borderColor, Color fillColor,
						int cx, int cy,
						int xs, int ys,
						ref Texture2D texture) {
        float steps= 360f/(4f*Mathf.PI*radius);
        for(float angle= 0f; angle < 90f; angle+= steps) {
			float rad= angle*Mathf.Deg2Rad;
            float s= Mathf.Sin(rad);
            float c= Mathf.Cos(rad);
			for(float r= 0f; r < radius; r+=0.9f) {
				int x= (int)(r*c+0.5f)*xs;
				int y= (int)(r*s+0.5f)*ys;
				texture.SetPixel(cx+x,cy+y,fillColor);
			}

            float dx= radius*c;
            float dy= radius*s;
			int dxi= (int)(dx+0.5f)*xs;
			int dyi= (int)(dy+0.5f)*ys;
			texture.SetPixel(cx+dxi,cy+dyi,borderColor); 
		}		
	}
	
	// ----------------------------------------------------------------------
	static void FlushCachedTextures() {
		if(myNodeTextures == null) return;
		foreach(var backDir in myNodeTextures) {
			foreach(var shadowDir in backDir.Value) {
				foreach(var pair in shadowDir.Value) {
					if(pair.Value != null) Texture2D.DestroyImmediate(pair.Value);
				}
			}
		}
	}
}
