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

}
