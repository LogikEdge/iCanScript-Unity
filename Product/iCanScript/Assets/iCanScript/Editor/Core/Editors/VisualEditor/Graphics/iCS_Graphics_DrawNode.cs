using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor {
public partial class iCS_Graphics {
    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
	const  float kTileRatio= 1f/3f;
	const  float kTilePos0 = 0f;
	const  float kTilePos1 = 1f/3f;
	const  float kTilePos2 = 2f/3f;
	static Rect  kTopLeftTileCoord    = new Rect(kTilePos0, kTilePos2, kTileRatio, kTileRatio);
	static Rect  kTopMidTileCoord     = new Rect(kTilePos1, kTilePos2, kTileRatio, kTileRatio);
	static Rect  kTopRightTileCoord   = new Rect(kTilePos2, kTilePos2, kTileRatio, kTileRatio);
	static Rect  kMidLeftTileCoord    = new Rect(kTilePos0, kTilePos1, kTileRatio, kTileRatio-0.01f);
	static Rect  kMidMidTileCoord     = new Rect(kTilePos1, kTilePos1, kTileRatio, kTileRatio-0.01f);
	static Rect  kMidRightTileCoord   = new Rect(kTilePos2, kTilePos1, kTileRatio, kTileRatio-0.01f);
	static Rect  kBottomLeftTileCoord = new Rect(kTilePos0, kTilePos0, kTileRatio, kTileRatio);
	static Rect  kBottomMidTileCoord  = new Rect(kTilePos1, kTilePos0, kTileRatio, kTileRatio);
	static Rect  kBottomRightTileCoord= new Rect(kTilePos2, kTilePos0, kTileRatio, kTileRatio);

	// ----------------------------------------------------------------------
	void DrawNode(Rect r, iCS_EditorObject node, Color nodeColor, Color backgroundColor, Color shadowColor) {
        backgroundColor.a= 0.2f;
		// Reajust screen position for fix size shadow.
		float shadowSize= iCS_EditorConfig.NodeShadowSize;
		float shadowSize2= 2f*shadowSize;
		Rect screenPos= new Rect(r.x-shadowSize, r.y-shadowSize, r.width+shadowSize2, r.height+shadowSize2);

		// Get texture.
		Texture2D nodeTexture= iCS_NodeTextures.GetNodeTexture(nodeColor, backgroundColor, shadowColor);
		int tileSize= (int)(nodeTexture.width*kTileRatio+0.1f);
		int tileSize2= 2*tileSize;
		
		float middleWidth = screenPos.width -tileSize2;
		float middleHeight= screenPos.height-tileSize2;

        // Draw node title
		Rect pos= new Rect(screenPos.x, screenPos.y, tileSize, tileSize);
		GUI.DrawTextureWithTexCoords(pos, nodeTexture, kTopLeftTileCoord);
		pos.x= pos.xMax;
		pos.width= middleWidth;
		GUI.DrawTextureWithTexCoords(pos, nodeTexture, kTopMidTileCoord);
		pos.x= pos.xMax;
		pos.width= tileSize;
		GUI.DrawTextureWithTexCoords(pos, nodeTexture, kTopRightTileCoord);
        // Draw node center
		if(middleHeight > 0f) {
            float heightRatio= middleHeight >= tileSize ? kTileRatio-0.01f : middleHeight/(3f*tileSize);
    		pos= new Rect(screenPos.x, pos.yMax,tileSize,middleHeight);
            Rect coord= kMidLeftTileCoord;
            coord.height= heightRatio;
    		GUI.DrawTextureWithTexCoords(pos, nodeTexture, coord);                
			pos.x= pos.xMax;
			pos.width= middleWidth;
            coord= kMidMidTileCoord;
            coord.height= heightRatio;
			GUI.DrawTextureWithTexCoords(pos, nodeTexture, coord);
			pos.x= pos.xMax;
			pos.width= tileSize;
            coord= kMidRightTileCoord;
            coord.height= heightRatio;
			GUI.DrawTextureWithTexCoords(pos, nodeTexture, coord);			
		}
        // Draw node bottom
		pos= new Rect(screenPos.x, pos.yMax, tileSize, tileSize);
		GUI.DrawTextureWithTexCoords(pos, nodeTexture, kBottomLeftTileCoord);
		pos.x= pos.xMax;
		pos.width= middleWidth;
		GUI.DrawTextureWithTexCoords(pos, nodeTexture, kBottomMidTileCoord);
		pos.x= pos.xMax;
		pos.width= tileSize;
		GUI.DrawTextureWithTexCoords(pos, nodeTexture, kBottomRightTileCoord);

        // Show title.
		GUI.color= Color.white;
        if(!ShouldShowTitle()) return;
        var titleStyle   = Layout.DynamicTitleStyle;
        var subTitleStyle= Layout.DynamicSubTitleStyle;
        var title= new GUIContent(GetNodeTitle(node));
        var subTitle= GetNodeSubTitle(node);
        Vector2 titleSize= titleStyle.CalcSize(title);
        var scale= titleSize.y / iCS_EditorConfig.kTitleFontSize;
        var titleLeft  = r.x+scale*iCS_EditorConfig.kNodeTitleIconSize;
        var titleTop   = r.y;
        var subTitleTop= titleTop+titleSize.y;
        var titleWidth = r.width-titleLeft;
        GUI.Label(new Rect(titleLeft, titleTop, titleWidth, r.height), title, titleStyle);
        GUI.Label(new Rect(titleLeft, subTitleTop, titleWidth, r.height), subTitle, subTitleStyle);
	}
}
}