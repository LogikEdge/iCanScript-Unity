using UnityEngine;
using System.Collections;

public static class iCS_BuiltinTextures {
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    public static Texture2D InEndPortIcon           { get { return myInEndPortIcon; }}
    public static Texture2D OutEndPortIcon          { get { return myOutEndPortIcon; }}
    public static Texture2D InRelayPortIcon         { get { return myInRelayPortIcon; }}
    public static Texture2D OutRelayPortIcon        { get { return myOutRelayPortIcon; }}
    public static Texture2D InTransitionPortIcon    { get { return myInTransitionPortIcon; }}
    public static Texture2D OutTransitionPortIcon   { get { return myOutTransitionPortIcon; }}

    // =================================================================================
    // Accessors
    // ---------------------------------------------------------------------------------
    public static Texture2D MinimizeIcon(float scale) {
        if(Math3D.IsNotEqual(scale,myScale)) {
            myScale= scale;
            BuildScaleDependantTextures();
        }
        return myMinimizeIcon;
    }
    public static Texture2D MaximizeIcon(float scale) {
        if(Math3D.IsNotEqual(scale,myScale)) {
            myScale= scale;
            BuildScaleDependantTextures();
        }
        return myMaximizeIcon;
    }
    public static Texture2D FoldIcon(float scale) {
        if(Math3D.IsNotEqual(scale,myScale)) {
            myScale= scale;
            BuildScaleDependantTextures();
        }
        return myFoldIcon;
    }
    public static Texture2D UnfoldIcon(float scale) {
        if(Math3D.IsNotEqual(scale,myScale)) {
            myScale= scale;
            BuildScaleDependantTextures();
        }
        return myUnfoldIcon;
    }
    
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    public const int   kPortIconWidth   = 16;
    public const int   kPortIconHeight  = 12;
    public const int   kMinimizeIconSize= 16;
    public const int   kMaximizeIconSize= 32;
    public const int   kFoldIconSize    = 16;
    public const int   kUnfoldIconSize  = 16;

    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    static float        myScale= 1f;
	static Texture2D    myInEndPortIcon;
	static Texture2D    myOutEndPortIcon;
	static Texture2D    myInRelayPortIcon;
	static Texture2D    myOutRelayPortIcon;
	static Texture2D    myInTransitionPortIcon;
	static Texture2D    myOutTransitionPortIcon;
	static Texture2D    myMinimizeIcon;
	static Texture2D    myMaximizeIcon;
	static Texture2D    myFoldIcon;
	static Texture2D    myUnfoldIcon;

    // =================================================================================
    // Polygons
    // ---------------------------------------------------------------------------------
    static Vector2[] myFoldIconPolygon  = null;
    static Vector2[] myUnfoldIconPolygon= null;
    
    // =================================================================================
    // Constrcutor
    // ---------------------------------------------------------------------------------
    static iCS_BuiltinTextures() {
        // Build polygons.
        myFoldIconPolygon= new Vector2[3];
        myFoldIconPolygon[0]= new Vector2(-0.5f, -0.45f);
        myFoldIconPolygon[1]= new Vector2( 0.5f,  0);
        myFoldIconPolygon[2]= new Vector2(-0.5f,  0.45f);
        myUnfoldIconPolygon= new Vector2[3];
        myUnfoldIconPolygon[0]= new Vector2(-0.45f, 0.5f);
        myUnfoldIconPolygon[1]= new Vector2( 0.45f, 0.5f);
        myUnfoldIconPolygon[2]= new Vector2( 0,   -0.5f);
        // Build scale independent textures.
        BuildScaleIndependantTextures();
        BuildScaleDependantTextures();
    }
    // ---------------------------------------------------------------------------------
    static void BuildScaleIndependantTextures() {
        BuildEndPortIcons(Color.red);
        BuildRelayPortIcons(Color.red);
        BuildTransitionPortIcons();
    }
    // ---------------------------------------------------------------------------------
    static void BuildScaleDependantTextures() {
        BuildMinimizeIcon();
        BuildMaximizeIcon();
        BuildFoldIcon();
        BuildUnfoldIcon();        
    }
    // ---------------------------------------------------------------------------------
	static void BuildEndPortIcons(Color typeColor) {
		Texture2D inPortTemplate= new Texture2D(kPortIconHeight, kPortIconHeight);
		Texture2D outPortTemplate= new Texture2D(kPortIconHeight, kPortIconHeight);
		float radius= 0.5f*(kPortIconHeight-3f);
        iCS_PortIcons.BuildInEndPortTemplateImp(radius, radius-2, ref inPortTemplate);
        iCS_PortIcons.BuildOutEndPortTemplateImp(radius, radius-2, ref outPortTemplate);
        Texture2D portInIcon= iCS_PortIcons.BuildPortIcon(typeColor, inPortTemplate);
        Texture2D portOutIcon= iCS_PortIcons.BuildPortIcon(typeColor, outPortTemplate);

        myInEndPortIcon= new Texture2D(kPortIconWidth, kPortIconHeight);
        myOutEndPortIcon= new Texture2D(kPortIconWidth, kPortIconHeight);
        iCS_TextureUtil.Clear(ref myInEndPortIcon);
        iCS_TextureUtil.Clear(ref myOutEndPortIcon);
        int xOffset= (kPortIconWidth-kPortIconHeight)>>1;
        iCS_AlphaBlend.NormalBlend(0, 0, portInIcon , xOffset, 0, ref myInEndPortIcon,  portOutIcon.width, portOutIcon.height);
        iCS_AlphaBlend.NormalBlend(0, 0, portOutIcon, xOffset, 0, ref myOutEndPortIcon, portOutIcon.width, portOutIcon.height);

        // Finalize icons.
        myInEndPortIcon.Apply();
        myOutEndPortIcon.Apply();
        myInEndPortIcon.hideFlags= HideFlags.DontSave;
        myOutEndPortIcon.hideFlags= HideFlags.DontSave;
	
        Texture2D.DestroyImmediate(portInIcon);
        Texture2D.DestroyImmediate(portOutIcon);
        Texture2D.DestroyImmediate(inPortTemplate);	    
        Texture2D.DestroyImmediate(outPortTemplate);	    
	}

    // ---------------------------------------------------------------------------------
	static void BuildRelayPortIcons(Color typeColor) {
		Texture2D inPortTemplate= new Texture2D(kPortIconHeight, kPortIconHeight);
		Texture2D outPortTemplate= new Texture2D(kPortIconHeight, kPortIconHeight);
		float len= kPortIconHeight-2f;
        iCS_PortIcons.BuildInRelayPortTemplate (len, ref inPortTemplate);
        iCS_PortIcons.BuildOutRelayPortTemplate(len, ref outPortTemplate);
        Texture2D inPortIcon = iCS_PortIcons.BuildPortIcon(typeColor, inPortTemplate);
        Texture2D outPortIcon= iCS_PortIcons.BuildPortIcon(typeColor, outPortTemplate);

        myInRelayPortIcon = new Texture2D(kPortIconWidth, kPortIconHeight);
        myOutRelayPortIcon= new Texture2D(kPortIconWidth, kPortIconHeight);
        iCS_TextureUtil.Clear(ref myInRelayPortIcon);
        iCS_TextureUtil.Clear(ref myOutRelayPortIcon);
        int xOffset= (kPortIconWidth-kPortIconHeight)>>1;
        iCS_AlphaBlend.NormalBlend(0, 0, inPortIcon,  xOffset, 0, ref myInRelayPortIcon,  inPortIcon.width,  inPortIcon.height);
        iCS_AlphaBlend.NormalBlend(0, 0, outPortIcon, xOffset, 0, ref myOutRelayPortIcon, outPortIcon.width, outPortIcon.height);

        // Finalize icons.
        myInRelayPortIcon.Apply();
        myOutRelayPortIcon.Apply();
        myInRelayPortIcon.hideFlags= HideFlags.DontSave;
        myOutRelayPortIcon.hideFlags= HideFlags.DontSave;
	
        Texture2D.DestroyImmediate(inPortIcon);
        Texture2D.DestroyImmediate(inPortTemplate);	    
        Texture2D.DestroyImmediate(outPortIcon);
        Texture2D.DestroyImmediate(outPortTemplate);	    
	}

    // ---------------------------------------------------------------------------------
    static void BuildTransitionPortIcons() {
        // Create textures.
        myInTransitionPortIcon= new Texture2D(kPortIconWidth, kPortIconHeight);
        myOutTransitionPortIcon= new Texture2D(kPortIconWidth, kPortIconHeight);
        iCS_TextureUtil.Clear(ref myInTransitionPortIcon);
        iCS_TextureUtil.Clear(ref myOutTransitionPortIcon);
        
        // Build out transition port.
        float halfHeight= 0.5f*kPortIconHeight;
        float radius= halfHeight-1.5f;
		iCS_TextureUtil.Circle(radius, Color.white, Color.white, ref myOutTransitionPortIcon, new Vector2(halfHeight, halfHeight));

        // Add horizontal line.
		int halfHeightInt= (int)halfHeight;
		for(int x= halfHeightInt; x < kPortIconWidth; ++x) {
		    myOutTransitionPortIcon.SetPixel(x, halfHeightInt, Color.white);
            myInTransitionPortIcon.SetPixel(x-halfHeightInt, halfHeightInt, Color.white);
		}
		
        // Finalize icons.
        myInTransitionPortIcon.Apply();
        myOutTransitionPortIcon.Apply();
        myInTransitionPortIcon.hideFlags= HideFlags.DontSave;
        myOutTransitionPortIcon.hideFlags= HideFlags.DontSave;
    }

    // ---------------------------------------------------------------------------------
    static void BuildMaximizeIcon() {        
        // Allocate icon.
        float size= /*myScale**/kMaximizeIconSize;
        int sizeInt= ((int)size)+1;
        float offset= 0.5f*((float)sizeInt-size);
//        if(myMaximizeIcon != null) Texture2D.DestroyImmediate(myMaximizeIcon);
        myMaximizeIcon= new Texture2D(sizeInt, sizeInt);
        iCS_TextureUtil.Clear(ref myMaximizeIcon);
        // Draw minimize icon.
        float halfSize= 0.5f*size;

        float radius= halfSize-0.15f*size;
        var center= new Vector2(halfSize+offset, halfSize+offset);
        float borderWidth= 1f;
        Color borderColor= Color.black;
        Color fillColor= Color.white;
        iCS_TextureUtil.DrawFilledCircle(ref myMaximizeIcon, radius-0.5f, center, fillColor);
        iCS_TextureUtil.DrawCircle(ref myMaximizeIcon, radius, center, borderColor, borderWidth);
        float lineHeight= 0.05f*size;
        float lineHalfWidth= 0.15f*size;
        iCS_TextureUtil.DrawLine(ref myMaximizeIcon,
                                 new Vector2(center.x-lineHalfWidth, center.y),
                                 new Vector2(center.x+lineHalfWidth, center.y),
                                 Color.black,
                                 lineHeight);
        iCS_TextureUtil.DrawLine(ref myMaximizeIcon,
                                 new Vector2(center.x, center.y-lineHalfWidth),
                                 new Vector2(center.x, center.y+lineHalfWidth),
                                 Color.black,
                                 lineHeight);

//        iCS_TextureUtil.Circle(halfSize-0.15f*size,
//                           Color.black, Color.white,
//                           ref myMaximizeIcon, new Vector2(halfSize+offset, halfSize+offset));
//        int halfSizeInt= (int)halfSize;
//        for(int i= (int)(0.30f*size); i <= (int)(0.70f*size); ++i) {
//            myMaximizeIcon.SetPixel(1+i, 1+halfSizeInt, Color.black);
//            myMaximizeIcon.SetPixel(1+i, 1+halfSizeInt-1, Color.black);
//            myMaximizeIcon.SetPixel(1+halfSizeInt, 1+i, Color.black);
//            myMaximizeIcon.SetPixel(1+halfSizeInt-1, 1+i, Color.black);
//        }
        // Finalize icons.
        myMaximizeIcon.Apply();
        myMaximizeIcon.hideFlags= HideFlags.DontSave;
    }
    // ---------------------------------------------------------------------------------
    static void BuildMinimizeIcon() {        
        // Allocate icon.
        float size= myScale*kMinimizeIconSize;
        int sizeInt= ((int)size)+1;
        float offset= 0.5f*((float)sizeInt-size);
        if(myMinimizeIcon != null) Texture2D.DestroyImmediate(myMinimizeIcon);
        myMinimizeIcon= new Texture2D(sizeInt, sizeInt);
        iCS_TextureUtil.Clear(ref myMinimizeIcon);
        // Draw minimize icon.
        float halfSize= 0.5f*size;
        float radius= halfSize-0.15f*size;
        var center= new Vector2(halfSize+offset, halfSize+offset);
        float borderWidth= 1.2f;
        Color borderColor= Color.black;
        Color fillColor= new Color(0,0,0,0.3f);
        iCS_TextureUtil.DrawFilledCircle(ref myMinimizeIcon, radius-0.5f, center, fillColor);
        iCS_TextureUtil.DrawCircle(ref myMinimizeIcon, radius, center, borderColor, borderWidth);
        float lineHeight= 0.1f*size;
        float lineHalfWidth= 0.15f*size;
        iCS_TextureUtil.DrawLine(ref myMinimizeIcon,
                                 new Vector2(center.x-lineHalfWidth, center.y),
                                 new Vector2(center.x+lineHalfWidth, center.y),
                                 Color.black,
                                 lineHeight);
        // Finalize icons.
        myMinimizeIcon.Apply();
        myMinimizeIcon.hideFlags= HideFlags.DontSave;
    }
    // ---------------------------------------------------------------------------------
    static void BuildFoldIcon() {
        // Build polygon
        float size= myScale*kFoldIconSize;
        int textureSize= ((int)size)+1;
        float offset= 0.5f*((float)textureSize-size);
        var center= new Vector2(0.5f*textureSize+offset, 0.5f*textureSize+offset);
        var scale= new Vector2(0.65f*size, 0.65f*size);
        var polygon= Math3D.ScaleAndTranslatePolygon(myFoldIconPolygon, scale, center);
        // Build texture
        if(myFoldIcon != null) Texture2D.DestroyImmediate(myFoldIcon);
		myFoldIcon= new Texture2D(textureSize, textureSize);
		iCS_TextureUtil.Clear(ref myFoldIcon);
        Color c= new Color(0,0,0,0.3f);
	    iCS_TextureUtil.DrawFilledPolygon(ref myFoldIcon, polygon, c);
	    float lineWidth= 1.2f*myScale;
	    iCS_TextureUtil.DrawPolygonOutline(ref myFoldIcon, polygon, Color.black, lineWidth);
        // Finalize icons.
        myFoldIcon.Apply();
        myFoldIcon.hideFlags= HideFlags.DontSave;
    }
    // ---------------------------------------------------------------------------------
    static void BuildUnfoldIcon() {
        // Build polygon
        float size= myScale*kUnfoldIconSize;
        int textureSize= ((int)size)+1;
        float offset= 0.5f*((float)textureSize-size);
        var center= new Vector2(0.5f*textureSize+offset, 0.5f*textureSize+offset);
        var scale= new Vector2(0.65f*size, 0.65f*size);
        var polygon= Math3D.ScaleAndTranslatePolygon(myUnfoldIconPolygon, scale, center);
        // Build texture
        if(myUnfoldIcon != null) Texture2D.DestroyImmediate(myUnfoldIcon);
		myUnfoldIcon= new Texture2D(textureSize, textureSize);
		iCS_TextureUtil.Clear(ref myUnfoldIcon);
        Color c= new Color(0,0,0,0.3f);
	    iCS_TextureUtil.DrawFilledPolygon(ref myUnfoldIcon, polygon, c);
	    float lineWidth= 1.2f*myScale;
	    iCS_TextureUtil.DrawPolygonOutline(ref myUnfoldIcon, polygon, Color.black, lineWidth);
        // Finalize icons.
        myUnfoldIcon.Apply();
        myUnfoldIcon.hideFlags= HideFlags.DontSave;
    }
}
