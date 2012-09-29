using UnityEngine;
using System.Collections;

public static class iCS_BuiltinTextures {
    // =================================================================================
    // Properties
    // ---------------------------------------------------------------------------------
    public static Texture2D InPortIcon  { get { return myInPortIcon; }}
    public static Texture2D OutPortIcon { get { return myOutPortIcon; }}



    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const int   kPortIconWidth  = 16;
    const int   kPortIconHeight = 12;

    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	static Texture2D   myInPortIcon;
	static Texture2D   myOutPortIcon;

    // =================================================================================
    // Constrcutor
    // ---------------------------------------------------------------------------------
    static iCS_BuiltinTextures() {
        BuildPortIcon(Color.green, Color.red);
    }
    
    // ---------------------------------------------------------------------------------
	static void BuildPortIcon(Color nodeColor, Color typeColor) {
		Texture2D portTemplate= new Texture2D(kPortIconHeight, kPortIconHeight);
		float radius= 0.5f*(kPortIconHeight-3f);
        iCS_PortIcons.BuildCircularPortTemplateImp(radius, ref portTemplate);
        Texture2D portIcon= iCS_PortIcons.BuildPortIcon(nodeColor, typeColor, portTemplate);

        myInPortIcon= new Texture2D(kPortIconWidth, kPortIconHeight);
        myOutPortIcon= new Texture2D(kPortIconWidth, kPortIconHeight);
        Math3D.Clear(ref myInPortIcon);
        Math3D.Clear(ref myOutPortIcon);
        
        int radiusInt= (int)radius;
        int lineLength= kPortIconWidth-radiusInt;
        int lineHeight= kPortIconHeight/2;
        for(int x= 0; x < lineLength; ++x) {
            myInPortIcon.SetPixel(x, lineHeight, typeColor);
            myOutPortIcon.SetPixel(radiusInt+x, lineHeight, typeColor);
        }
        int inOffset= kPortIconWidth-kPortIconHeight;
        for(int x= 0; x < portIcon.width; ++x) {
            for(int y= 0; y < portIcon.height; ++y) {
                Color srcPixel= portIcon.GetPixel(x,y);
                Color dstPixel= myInPortIcon.GetPixel(inOffset+x,y);
                myInPortIcon.SetPixel(inOffset+x, y, Math3D.AlphaBlend(srcPixel, dstPixel));
                dstPixel= myOutPortIcon.GetPixel(x,y);
                myOutPortIcon.SetPixel(x,y, Math3D.AlphaBlend(srcPixel, dstPixel));
            }
        }
        myInPortIcon.Apply();
        myOutPortIcon.Apply();
        myInPortIcon.hideFlags= HideFlags.DontSave;
        myOutPortIcon.hideFlags= HideFlags.DontSave;
	
        Texture2D.DestroyImmediate(portIcon);
        Texture2D.DestroyImmediate(portTemplate);	    
	}

}
