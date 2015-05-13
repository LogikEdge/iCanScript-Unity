using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using iCanScript;

namespace iCanScript.Internal.Editor {
    public static class iCS_NodeTextures {
        // ======================================================================
        // CONSTANTS
        // ----------------------------------------------------------------------
        const float kNodeCornerRadius= iCS_EditorConfig.kNodeCornerRadius;
        const float kNodeTitleHeight = iCS_EditorConfig.kNodeTitleHeight;
    	const float kShadowAlpha     = 0.4f;

        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
    	static int			myRadius          = 0;
    	static int			myExtraTitleHeight= 0;
    	static Texture2D	myNodeTemplate    = null;

        // ----------------------------------------------------------------------
    	static Dictionary<Color,Dictionary<Color,Dictionary<Color,Texture2D>>>	myNodeTextures= null;
	
        // ======================================================================
        // Constructor
        // ----------------------------------------------------------------------
        static iCS_NodeTextures() {
            BuildNodeTemplate(1.0f);
        }
    
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
    		Texture2D texture= new Texture2D(myNodeTemplate.width, myNodeTemplate.height, TextureFormat.ARGB32, false);
    		for(int x= 0; x < myNodeTemplate.width; ++x) {
    			for(int y= 0; y < myNodeTemplate.height; ++y) {
    				Color pixel= myNodeTemplate.GetPixel(x,y);
    				float totalChannels= pixel.g+pixel.b+pixel.r;
    				if(totalChannels == 0) {
    					texture.SetPixel(x,y, pixel);					
    				} else {
    					// Anti-Aliasing fill.
    					float shadowColorRatio= pixel.r/totalChannels;
    					float nodeColorRatio= pixel.g/totalChannels;
    					float backColorRatio= pixel.b/totalChannels;
    					Color c;
    					c.r= nodeColorRatio*nodeColor.r+backColorRatio*backColor.r+shadowColorRatio*shadowColor.r;
    					c.g= nodeColorRatio*nodeColor.g+backColorRatio*backColor.g+shadowColorRatio*shadowColor.g;
    					c.b= nodeColorRatio*nodeColor.b+backColorRatio*backColor.b+shadowColorRatio*shadowColor.b;
    					c.a= pixel.a*(nodeColorRatio*nodeColor.a+backColorRatio*backColor.a+shadowColorRatio*shadowColor.a);
    					texture.SetPixel(x,y, c);					
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
            float radius= kNodeCornerRadius*scale;
    		int radiusInt    = (int)(radius+0.5f);
    		int extraTitleheightInt= (int)(kNodeTitleHeight*scale+0.5f)-radiusInt;

    		// We don't need to rebuild node textures if nothing has changed.
    		if(radiusInt == myRadius && extraTitleheightInt == myExtraTitleHeight) {
    			return;
    		}
    		myRadius= radiusInt;
    		myExtraTitleHeight= extraTitleheightInt;
		
    		// Flush all cached textures.
    		FlushCachedTextures();
		
    		int shadowSizeInt= (int)iCS_EditorConfig.NodeShadowSize;
    		int cornerOffset= shadowSizeInt+radiusInt;
    		int tileSize= radiusInt+extraTitleheightInt+shadowSizeInt;
    		int textureSize= 3*tileSize;
    		myNodeTemplate= new Texture2D(textureSize,textureSize, TextureFormat.ARGB32, false);
            iCS_TextureUtil.Clear(ref myNodeTemplate);
    		// Draw shadow
    		int farCornerOffset= textureSize-cornerOffset-1;
    		int farSideOffset= textureSize-shadowSizeInt-1;
    		float alpha= 0.25f;
    		float blendedAlpha= 0.25f;
    		for(int i= shadowSizeInt; i > 0; --i) {
    			Color c= Color.red;
    			c.a= alpha;
    			DrawArc(radiusInt, c, c, farCornerOffset+i, farCornerOffset-i,  1,  1, ref myNodeTemplate);
    			DrawArc(radiusInt, c, c, farCornerOffset+i, cornerOffset-i   ,  1, -1, ref myNodeTemplate);
    			DrawArc(radiusInt, c, c, cornerOffset+i   , cornerOffset-i   , -1, -1, ref myNodeTemplate);			
    		}
    		for(int i= shadowSizeInt; i > 0; --i) {
    			Color c= Color.red;
    			c.a= blendedAlpha;
    			for(int j= cornerOffset-1; j <= farCornerOffset+2; ++j) {
    				myNodeTemplate.SetPixel(j+i,shadowSizeInt-i,c);
    				myNodeTemplate.SetPixel(farSideOffset+i,j-i,c);								
    			}
    			blendedAlpha= alpha+(1f-alpha)*blendedAlpha;
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
    		for(int i= cornerOffset; i < farCornerOffset; ++i) {
    			myNodeTemplate.SetPixel(i,shadowSizeInt,Color.green);
    			myNodeTemplate.SetPixel(shadowSizeInt,i, Color.green);
    			myNodeTemplate.SetPixel(farSideOffset,i, Color.green);			
    			for(int j= shadowSizeInt+1; j <= cornerOffset; ++j) {
    				myNodeTemplate.SetPixel(i,textureSize-j, Color.green);							
    			}
    			for(int j= shadowSizeInt+1; j <= cornerOffset; ++j) {
    				myNodeTemplate.SetPixel(i,j, Color.blue);							
    			}
    		}
    		// Draw extra title height.
    		for(int i= shadowSizeInt; i <= farSideOffset; ++i) {
    			for(int j= 2*tileSize; j <= textureSize-cornerOffset; ++j) {
    				myNodeTemplate.SetPixel(i,j, Color.green);				
    			}
    		}
    		// Draw background.
    		for(int i= shadowSizeInt+1; i < farSideOffset; ++i) {
    			for(int j= cornerOffset+1; j < 2*tileSize; ++j) {
    				myNodeTemplate.SetPixel(i,j, Color.blue);											
    			}
    		}
		

    		myNodeTemplate.hideFlags= HideFlags.DontSave;
    		myNodeTemplate.Apply();
    	}
	
        // ----------------------------------------------------------------------
    	// Draw a filled 90 degree arc into a texture.
    	static void DrawArc(float radius,
    		                Color borderColor, Color fillColor,
    						int cx, int cy,
    						int xs, int ys,
    						ref Texture2D texture,
    						bool alphaBlend= true) {
    		float ringWidth= 2f;
    		float outterRingRadius= radius+0.5f*ringWidth;
    		float innerRingRadius= radius-0.5f*ringWidth;
    		float outterRingRadius2= outterRingRadius*outterRingRadius;
    		float innerRingRadius2= innerRingRadius*innerRingRadius;

    		int size= (int)(outterRingRadius+1f);
    		for(int x= 0; x < size; ++x) {
    			for(int y= 0; y < size; ++y) {
    				float rx= (float)x;
    				float ry= (float)y;
    				float r2= rx*rx+ry*ry;
    				if(r2 > outterRingRadius2) {
    					// Don't draw if outside corner.
    				} else if(r2 > innerRingRadius2) {
    					float r= Mathf.Sqrt(r2);
    					if(r > radius) {
    						float ratio= (ringWidth-2f*(r-radius))/ringWidth;
    						Color c= borderColor;
    						c.a= ratio*borderColor.a;
    						if(alphaBlend) {
    							c= iCS_AlphaBlend.NormalBlend(c, texture.GetPixel(cx+x*xs, cy+y*ys));							
    						}
    						texture.SetPixel(cx+x*xs,cy+y*ys,c);							
    					} else {
    						float ratio= (ringWidth-2f*(radius-r))/ringWidth;
    						Color c;
    						c.r= ratio*borderColor.r+(1f-ratio)*fillColor.r;
    						c.g= ratio*borderColor.g+(1f-ratio)*fillColor.g;
    						c.b= ratio*borderColor.b+(1f-ratio)*fillColor.b;
    						c.a= ratio*borderColor.a+(1f-ratio)*fillColor.a;
    						if(alphaBlend) {
    							c= iCS_AlphaBlend.NormalBlend(c, texture.GetPixel(cx+x*xs, cy+y*ys));														
    						}
    						texture.SetPixel(cx+x*xs,cy+y*ys,c);						
    					}
    				} else {
    					Color c= fillColor;
    					if(alphaBlend) {
    						c= iCS_AlphaBlend.NormalBlend(c, texture.GetPixel(cx+x*xs, cy+y*ys));														
    					}
    					texture.SetPixel(cx+x*xs, cy+y*ys, c);
    				}
    			}
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
    
}
