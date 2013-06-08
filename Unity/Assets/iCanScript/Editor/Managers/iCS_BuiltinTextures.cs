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
    
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    public const int   kPortIconWidth   = 16;
    public const int   kPortIconHeight  = 12;
    public const int   kMinimizeIconSize= 16;
    public const int   kMaximizeIconSize= 16;

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

    // =================================================================================
    // Constrcutor
    // ---------------------------------------------------------------------------------
    static iCS_BuiltinTextures() {
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
    }
    // ---------------------------------------------------------------------------------
	static void BuildEndPortIcons(Color typeColor) {
		Texture2D inPortTemplate= new Texture2D(kPortIconHeight, kPortIconHeight);
		Texture2D outPortTemplate= new Texture2D(kPortIconHeight, kPortIconHeight);
		float radius= 0.5f*(kPortIconHeight-3f);
        iCS_PortIcons.BuildInEndPortTemplateImp(radius, radius-2, 1f, ref inPortTemplate);
        iCS_PortIcons.BuildOutEndPortTemplateImp(radius, radius-2, 1f, ref outPortTemplate);
        Texture2D portInIcon= iCS_PortIcons.BuildPortIcon(typeColor, inPortTemplate);
        Texture2D portOutIcon= iCS_PortIcons.BuildPortIcon(typeColor, outPortTemplate);

        myInEndPortIcon= new Texture2D(kPortIconWidth, kPortIconHeight);
        myOutEndPortIcon= new Texture2D(kPortIconWidth, kPortIconHeight);
        TextureUtil.Clear(ref myInEndPortIcon);
        TextureUtil.Clear(ref myOutEndPortIcon);
        
        int radiusInt= (int)radius;
        int lineLength= kPortIconWidth-radiusInt;
        int lineHeight= kPortIconHeight/2;
        for(int x= 0; x < lineLength; ++x) {
            myInEndPortIcon.SetPixel(x, lineHeight, typeColor);
            myOutEndPortIcon.SetPixel(radiusInt+x, lineHeight, typeColor);
        }
        int inOffset= kPortIconWidth-kPortIconHeight;
        TextureUtil.AlphaBlend(0, 0, portInIcon , inOffset, 0, ref myInEndPortIcon,  portOutIcon.width, portOutIcon.height);
        TextureUtil.AlphaBlend(0, 0, portOutIcon, 0,        0, ref myOutEndPortIcon, portOutIcon.width, portOutIcon.height);

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
        TextureUtil.Clear(ref myInRelayPortIcon);
        TextureUtil.Clear(ref myOutRelayPortIcon);
        
        int portCenter= (int)(0.5f*kPortIconHeight);
        for(int x= 0; x < kPortIconWidth-portCenter; ++x) {
            myInRelayPortIcon.SetPixel(x, portCenter-1, typeColor);
            myOutRelayPortIcon.SetPixel(portCenter+x, portCenter-1, typeColor);
        }
        int inOffset= kPortIconWidth-kPortIconHeight;
        TextureUtil.AlphaBlend(0, 0, inPortIcon,  1+inOffset, 1, ref myInRelayPortIcon,  inPortIcon.width,  inPortIcon.height);
        TextureUtil.AlphaBlend(0, 0, outPortIcon, 1,          1, ref myOutRelayPortIcon, outPortIcon.width, outPortIcon.height);

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
        TextureUtil.Clear(ref myInTransitionPortIcon);
        TextureUtil.Clear(ref myOutTransitionPortIcon);
        
        // Build out transition port.
        float halfHeight= 0.5f*kPortIconHeight;
        float radius= halfHeight-1.5f;
		TextureUtil.Circle(radius, Color.white, Color.white, ref myOutTransitionPortIcon, new Vector2(halfHeight, halfHeight));

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
    static void BuildMinimizeIcon() {        
        // Allocate icon.
        float size= myScale*kMinimizeIconSize;
        int sizeInt= ((int)size)+1;
        float offset= 0.5f*((float)sizeInt-size);
        myMinimizeIcon= new Texture2D(sizeInt, sizeInt);
        TextureUtil.Clear(ref myMinimizeIcon);
        // Draw minimize icon.
        float halfSize= 0.5f*size;
        TextureUtil.Circle(halfSize-0.15f*size,
                           Color.black, new Color(0,0,0,0.30f),
                           ref myMinimizeIcon, new Vector2(halfSize+offset, halfSize+offset),
                           2f*myScale);
        float boxHeight= 0.03f*size;
        float halfBoxHeight= 0.5f*boxHeight;
        TextureUtil.Box(0.35f*size+offset, halfSize-halfBoxHeight+offset,
                        0.65f*size+offset, halfSize+halfBoxHeight+offset,
                        Color.black, Color.black,
                        ref myMinimizeIcon, 2f*myScale);
        // Finalize icons.
        myMinimizeIcon.Apply();
        myMinimizeIcon.hideFlags= HideFlags.DontSave;
    }
    // ---------------------------------------------------------------------------------
    static void BuildMaximizeIcon() {        
        // Allocate icon.
        float size= /*myScale**/kMaximizeIconSize;
        int sizeInt= ((int)size)+1;
        float offset= 0.5f*((float)sizeInt-size);
        myMaximizeIcon= new Texture2D(sizeInt, sizeInt);
        TextureUtil.Clear(ref myMaximizeIcon);
        // Draw minimize icon.
        float halfSize= 0.5f*size;
        TextureUtil.Circle(halfSize-0.15f*size,
                           Color.black, Color.white,
                           ref myMaximizeIcon, new Vector2(halfSize+offset, halfSize+offset));
        int halfSizeInt= (int)halfSize;
        for(int i= (int)(0.30f*size); i <= (int)(0.70f*size); ++i) {
            myMaximizeIcon.SetPixel(1+i, 1+halfSizeInt, Color.black);
            myMaximizeIcon.SetPixel(1+i, 1+halfSizeInt-1, Color.black);
            myMaximizeIcon.SetPixel(1+halfSizeInt, 1+i, Color.black);
            myMaximizeIcon.SetPixel(1+halfSizeInt-1, 1+i, Color.black);
        }
        // Finalize icons.
        myMaximizeIcon.Apply();
        myMaximizeIcon.hideFlags= HideFlags.DontSave;
    }
}
