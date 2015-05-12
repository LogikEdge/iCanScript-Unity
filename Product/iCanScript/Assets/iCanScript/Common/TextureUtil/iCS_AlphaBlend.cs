using UnityEngine;

namespace iCanScript.Internal {
    
    public enum iCS_AlphaBlendMode { Normal };

    public static class iCS_AlphaBlend {
        // ======================================================================
        // Color Blend
        // ----------------------------------------------------------------------
        public static void Blend(ref Texture2D texture, int x, int y, Color c, iCS_AlphaBlendMode alphaBlendMode) {
            switch(alphaBlendMode) {
                case iCS_AlphaBlendMode.Normal: {
                    NormalBlend(ref texture, x, y, c);
                    break;
                }
                default: {
                    texture.SetPixel(x,y,c);
                    break;                
                }
            }
        }
        // ----------------------------------------------------------------------
        public static void NormalBlend(ref Texture2D texture, int x, int y, Color c) {
            c= iCS_AlphaBlend.NormalBlend(c, texture.GetPixel(x,y));
            texture.SetPixel(x,y,c);        
        }
        // ----------------------------------------------------------------------
        public static Color NormalBlend(Color src, Color dst) {
            float keepAlpha= 1f-src.a;
            float outAlpha= src.a + dst.a*keepAlpha;
            if(Math3D.IsZero(outAlpha)) return Color.clear;
            float srcBlend= src.a/outAlpha;
            float dstBlend= dst.a*keepAlpha/outAlpha;
            return new Color(src.r*srcBlend+dst.r*dstBlend, src.g*srcBlend+dst.g*dstBlend, src.b*srcBlend+dst.b*dstBlend, outAlpha);
        }
    
        // ----------------------------------------------------------------------
        public static void NormalBlend(Texture2D src, ref Texture2D dst) {
            NormalBlend(0, 0, src, 0, 0, ref dst, dst.width, dst.height);
        }
        // ----------------------------------------------------------------------
        public static void NormalBlend(int sx, int sy, Texture2D src, int dx, int dy, ref Texture2D dst, int width, int height) {
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
                    dst.SetPixel(dxx,dyy,NormalBlend(sc,dc));
                }
            }
        }
    }
}
