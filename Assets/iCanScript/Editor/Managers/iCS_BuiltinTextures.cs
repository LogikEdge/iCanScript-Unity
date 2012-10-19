using UnityEngine;
using System.Collections;

public static class iCS_BuiltinTextures {
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    public static Texture2D InDataPortIcon          { get { return myInDataPortIcon; }}
    public static Texture2D OutDataPortIcon         { get { return myOutDataPortIcon; }}
    public static Texture2D InValuePortIcon         { get { return myInValuePortIcon; }}
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
    const int   kPortIconWidth   = 16;
    const int   kPortIconHeight  = 12;
    const int   kMinimizeIconSize= 16;
    const int   kMaximizeIconSize= 16;

    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    static float        myScale= 1f;
	static Texture2D    myInDataPortIcon;
	static Texture2D    myOutDataPortIcon;
	static Texture2D    myInValuePortIcon;
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
        BuildDataPortIcons(Color.green, Color.red);
        BuildValuePortIcons(Color.green, Color.red);
        BuildTransitionPortIcons();
    }
    // ---------------------------------------------------------------------------------
    static void BuildScaleDependantTextures() {
        BuildMinimizeIcon();
        BuildMaximizeIcon();        
    }
    // ---------------------------------------------------------------------------------
	static void BuildDataPortIcons(Color nodeColor, Color typeColor) {
		Texture2D portTemplate= new Texture2D(kPortIconHeight, kPortIconHeight);
		float radius= 0.5f*(kPortIconHeight-3f);
        iCS_PortIcons.BuildDataPortTemplateImp(radius, ref portTemplate);
        Texture2D portIcon= iCS_PortIcons.BuildPortIcon(nodeColor, typeColor, portTemplate);

        myInDataPortIcon= new Texture2D(kPortIconWidth, kPortIconHeight);
        myOutDataPortIcon= new Texture2D(kPortIconWidth, kPortIconHeight);
        TextureUtil.Clear(ref myInDataPortIcon);
        TextureUtil.Clear(ref myOutDataPortIcon);
        
        int radiusInt= (int)radius;
        int lineLength= kPortIconWidth-radiusInt;
        int lineHeight= kPortIconHeight/2;
        for(int x= 0; x < lineLength; ++x) {
            myInDataPortIcon.SetPixel(x, lineHeight, typeColor);
            myOutDataPortIcon.SetPixel(radiusInt+x, lineHeight, typeColor);
        }
        int inOffset= kPortIconWidth-kPortIconHeight;
        TextureUtil.AlphaBlend(0, 0, portIcon, inOffset, 0, ref myInDataPortIcon,  portIcon.width, portIcon.height);
        TextureUtil.AlphaBlend(0, 0, portIcon, 0,        0, ref myOutDataPortIcon, portIcon.width, portIcon.height);

        // Finalize icons.
        myInDataPortIcon.Apply();
        myOutDataPortIcon.Apply();
        myInDataPortIcon.hideFlags= HideFlags.DontSave;
        myOutDataPortIcon.hideFlags= HideFlags.DontSave;
	
        Texture2D.DestroyImmediate(portIcon);
        Texture2D.DestroyImmediate(portTemplate);	    
	}

    // ---------------------------------------------------------------------------------
	static void BuildValuePortIcons(Color nodeColor, Color typeColor) {
		Texture2D portTemplate= new Texture2D(kPortIconHeight, kPortIconHeight);
		float len= kPortIconHeight-2f;
        iCS_PortIcons.BuildValuePortTemplate(len, ref portTemplate);
        Texture2D portIcon= iCS_PortIcons.BuildPortIcon(nodeColor, typeColor, portTemplate);

        myInValuePortIcon= new Texture2D(kPortIconWidth, kPortIconHeight);
        TextureUtil.Clear(ref myInValuePortIcon);
        
        int portCenter= (int)(0.5f*kPortIconHeight);
        for(int x= 0; x < kPortIconWidth-portCenter; ++x) {
            myInValuePortIcon.SetPixel(x, portCenter-1, typeColor);
        }
        int inOffset= kPortIconWidth-kPortIconHeight;
        TextureUtil.AlphaBlend(0, 0, portIcon, 1+inOffset, 1, ref myInValuePortIcon,  portIcon.width, portIcon.height);

        // Finalize icons.
        myInValuePortIcon.Apply();
        myInValuePortIcon.hideFlags= HideFlags.DontSave;
	
        Texture2D.DestroyImmediate(portIcon);
        Texture2D.DestroyImmediate(portTemplate);	    
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
