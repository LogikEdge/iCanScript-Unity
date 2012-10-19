using UnityEngine;
using System.Collections;

public static partial class TextureUtil {
    // ======================================================================
    // Texture fill.
	// ----------------------------------------------------------------------
    // Clears the given texture.
    public static void Clear(ref Texture2D texture) {
        for(int x= 0; x < texture.width; ++x) {
            for(int y= 0; y < texture.height; ++y) {
                texture.SetPixel(x,y,Color.clear);
            }
        }
    }
	// ----------------------------------------------------------------------
    // Draw a circle in a texture.
    public static void Circle(float radius,
                              Color borderColor, Color fillColor,
                              ref Texture2D texture, Vector2 center,
                              float borderWidth= 2f) {
        if(borderWidth < 2f) borderWidth= 2f;
		float outterRingRadius= radius+0.5f*borderWidth;
		float innerRingRadius= radius-0.5f*borderWidth;
		float outterRingRadius2= outterRingRadius*outterRingRadius;
		float innerRingRadius2= innerRingRadius*innerRingRadius;

		for(int x= 0; x < texture.width; ++x) {
			for(int y= 0; y < texture.height; ++y) {
				float rx= (float)x-center.x;
				float ry= (float)y-center.y;
				float r2= rx*rx+ry*ry;
				if(r2 > outterRingRadius2) {
					// Don't draw if outside corner.
				} else if(r2 > innerRingRadius2) {
					float r= Mathf.Sqrt(r2);
					if(r > radius) {
						float ratio= (borderWidth-2f*(r-radius))/borderWidth;
						Color c= borderColor;
						c.a= ratio*borderColor.a;
						c= TextureUtil.AlphaBlend(c, texture.GetPixel(x,y));							
						texture.SetPixel(x,y,c);							
					} else {
						float ratio= (borderWidth-2f*(radius-r))/borderWidth;
						Color c;
						c.r= ratio*borderColor.r+(1f-ratio)*fillColor.r;
						c.g= ratio*borderColor.g+(1f-ratio)*fillColor.g;
						c.b= ratio*borderColor.b+(1f-ratio)*fillColor.b;
						c.a= ratio*borderColor.a+(1f-ratio)*fillColor.a;
						c= TextureUtil.AlphaBlend(c, texture.GetPixel(x,y));														
						texture.SetPixel(x,y,c);						
					}
				} else {
					Color c= fillColor;
					c= TextureUtil.AlphaBlend(c, texture.GetPixel(x,y));														
					texture.SetPixel(x,y,c);
				}
			}
		}
    }  
	// ----------------------------------------------------------------------
    // Draw a box in a texture.
    public static void Box(float x1, float y1, float x2, float y2,
                           Color borderColor, Color fillColor,
                           ref Texture2D texture,
                           float borderWidth= 2f) {
        if(x2 < x1) { var tmp= x1; x1= x2; x2= tmp; }
        if(y2 < y1) { var tmp= y1; y1= y2; y2= tmp; }
        float halfBorderWidth= 0.5f*borderWidth;
		float outterX1= x1-halfBorderWidth;
		float innerX1 = x1+halfBorderWidth;
		float outterX2= x2+halfBorderWidth;
		float innerX2 = x2-halfBorderWidth;
		float outterY1= y1-halfBorderWidth;
		float innerY1 = y1+halfBorderWidth;
		float outterY2= y2+halfBorderWidth;
		float innerY2 = y2-halfBorderWidth;

		for(int x= 0; x < texture.width; ++x) {
			for(int y= 0; y < texture.height; ++y) {
				if(x < outterX1 || x > outterX2 || y < outterY1 || y > outterY2) {
					// Don't draw if outside box.
				} else if(x > innerX1 && x < innerX2 && y > innerY1 && y < innerY2) {
					Color c= TextureUtil.AlphaBlend(fillColor, texture.GetPixel(x,y));														
					texture.SetPixel(x,y,c);				    
				}  else {
                    float distX1= Mathf.Abs(x-x1);
                    float distX2= Mathf.Abs(x-x2);
                    float distY1= Mathf.Abs(y-y1);
                    float distY2= Mathf.Abs(y-y2);
                    float distX= Mathf.Min(distX1, distX2);
                    float distY= Mathf.Min(distY1, distY2);
				    if(x > x1 || x < x2 ||y > y1 || y < y2) {
                        float dist= Mathf.Min(distX, distY);
						float ratio= (borderWidth-2f*(dist))/borderWidth;
						Color c;
						c.r= ratio*borderColor.r+(1f-ratio)*fillColor.r;
						c.g= ratio*borderColor.g+(1f-ratio)*fillColor.g;
						c.b= ratio*borderColor.b+(1f-ratio)*fillColor.b;
						c.a= ratio*borderColor.a+(1f-ratio)*fillColor.a;
						c= TextureUtil.AlphaBlend(c, texture.GetPixel(x,y));														
						texture.SetPixel(x,y,c);						
			        } else {
			            float dist= (distX <= halfBorderWidth && distY <= halfBorderWidth) ?
			                            Mathf.Max(distX, distY) :
			                            Mathf.Min(distX, distY);
						float ratio= (borderWidth-2f*dist)/borderWidth;
						Color c= borderColor;
						c.a= ratio*borderColor.a;
						c= TextureUtil.AlphaBlend(c, texture.GetPixel(x,y));							
						texture.SetPixel(x,y,c);							
                    }
				}
			}
		}
    }

    // ======================================================================
    // Color Blend
	// ----------------------------------------------------------------------
    public static Color AlphaBlend(Color src, Color dst) {
        float keepAlpha= 1f-src.a;
        float outAlpha= src.a + dst.a*keepAlpha;
        if(Math3D.IsZero(outAlpha)) return Color.clear;
        float srcBlend= src.a/outAlpha;
        float dstBlend= dst.a*keepAlpha/outAlpha;
        return new Color(src.r*srcBlend+dst.r*dstBlend, src.g*srcBlend+dst.g*dstBlend, src.b*srcBlend+dst.b*dstBlend, outAlpha);
    }

	// ----------------------------------------------------------------------
    public static void AlphaBlend(Texture2D src, ref Texture2D dst) {
        AlphaBlend(0, 0, src, 0, 0, ref dst, dst.width, dst.height);
    }
	// ----------------------------------------------------------------------
    public static void AlphaBlend(int sx, int sy, Texture2D src, int dx, int dy, ref Texture2D dst, int width, int height) {
        if(src.width-sx < width) width= src.width-sx;
        if(dst.width-dx < width) width= dst.width-dx;
        if(src.height-sy < height) height= src.height-sy;
        if(dst.height-dy < height) height= dst.height-dy;
        for(int x= 0; x < width; ++x) {
            for(int y= 0; y < height; ++y) {
                Color sc= src.GetPixel(sx+x,sy+y);
                int dxx= dx+x;
                int dyy= dy+y;
                Color dc= dst.GetPixel(dxx,dyy);
                dst.SetPixel(dxx,dyy,AlphaBlend(sc,dc));
            }
        }
    }
}
