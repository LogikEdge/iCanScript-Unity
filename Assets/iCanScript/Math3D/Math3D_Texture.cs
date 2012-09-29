using UnityEngine;
using System.Collections;

public static partial class Math3D {
	// ----------------------------------------------------------------------
    // Clears the given texture.
    public static void Clear(ref Texture2D texture) {
        for(int x= 0; x < texture.width; ++x) {
            for(int y= 0; y < texture.height; ++y) {
                texture.SetPixel(x,y,Color.clear);
            }
        }
    }
    
    // ======================================================================
    // Color Blend
	// ----------------------------------------------------------------------
    public static Color AlphaBlend(Color src, Color dst) {
        float keepAlpha= 1f-src.a;
        float outAlpha= src.a + dst.a*keepAlpha;
        if(IsZero(outAlpha)) return Color.clear;
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
