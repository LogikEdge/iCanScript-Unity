using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace iCanScript.Internal.Editor {
// ==========================================================================
// Node utilities.
// ==========================================================================
    public partial class iCS_Graphics {
    // ----------------------------------------------------------------------
    bool ShouldDisplayNodeName(iCS_EditorObject node) {
        if(!ShouldShowTitle()) return false;
        if(!node.IsNode) return false;
        if(!node.IsVisibleOnDisplay) return false;
        return true;
    }
    // ----------------------------------------------------------------------
    /// Returns the title of the given node.
    ///
    /// @param node The node object from which to extract the name
    ///
	string GetNodeTitle(iCS_EditorObject node) {
        return node.NodeTitle;
	}
    // ----------------------------------------------------------------------
    /// Return the sub-title of the given node.
    ///
    /// @param node The node from which to extract the sub-title
    ///
    string GetNodeSubTitle(iCS_EditorObject node) {
        return node.NodeSubTitle;
    }
    // ----------------------------------------------------------------------
    // Returns the scaled node name size.
    Vector2 GetNodeNameSize(iCS_EditorObject node) {
        string nodeName= GetNodeTitle(node);
        GUIContent content= new GUIContent(nodeName);
        return node.IsIconizedOnDisplay ?
            Layout.DynamicLabelStyle.CalcSize(content):
            Layout.DynamicTitleStyle.CalcSize(content);
    }
    // ----------------------------------------------------------------------
    // Returns the non-scaled x,y with the scaled size.
    Rect GetNodeNamePosition(iCS_EditorObject node) {
        Vector2 size= GetNodeNameSize(node);
        Rect pos= node.AnimatedRect;
        float x= 0.5f*(pos.x+pos.xMax-size.x/Scale);
        float y= pos.y;
        if(node.IsIconizedOnDisplay) {
            y-= 5f+size.y/Scale;
        } else {
			y+= 0.9f*kNodeCornerRadius-0.5f*size.y/Scale;
        }
        return new Rect(x, y, size.x, size.y);
    }
    // ----------------------------------------------------------------------
    // Returns the scaled x,y,size.
    public Rect GetNodeNameGUIPosition(iCS_EditorObject node) {
        Rect graphRect= GetNodeNamePosition(node);
        var guiPos= TranslateAndScale(Math3D.ToVector2(graphRect));
        return new Rect(guiPos.x, guiPos.y, graphRect.width, graphRect.height);	    
    }
	
}
}
