using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

// ==========================================================================
// Node utilities.
// ==========================================================================
public partial class iCS_Graphics {
    // ----------------------------------------------------------------------
    bool ShouldDisplayNodeName(iCS_EditorObject node, iCS_IStorage iStorage) {
        if(!ShouldShowTitle()) return false;
        if(!node.IsNode) return false;
        if(!IsVisible(node,iStorage)) return false;
        return true;
    }
    // ----------------------------------------------------------------------
    public string GetNodeName(iCS_EditorObject node, iCS_IStorage iStorage) {
        return iStorage.Preferences.HiddenPrefixes.GetName(node.Name);    
    }	
    // ----------------------------------------------------------------------
    // Returns the scaled node name size.
    Vector2 GetNodeNameSize(iCS_EditorObject node, iCS_IStorage iStorage) {
        string portName= GetNodeName(node, iStorage);
        GUIContent content= new GUIContent(portName);
        return IsMinimized(node, iStorage) ? LabelStyle.CalcSize(content) : TitleStyle.CalcSize(content);
    }
    // ----------------------------------------------------------------------
    // Returns the non-scaled x,y with the scaled size.
    Rect GetNodeNamePosition(iCS_EditorObject node, iCS_IStorage iStorage) {
        Vector2 size= GetNodeNameSize(node, iStorage);
        Rect pos= iStorage.GetPosition(node);
        float x= 0.5f*(pos.x+pos.xMax-size.x/Scale);
        float y= pos.y;
        if(IsMinimized(node, iStorage)) {
            y-= 5f+size.y/Scale;
        } else {
			y+= 0.9f*kNodeCornerRadius-0.5f*size.y/Scale;
        }
        return new Rect(x, y, size.x, size.y);
    }
    // ----------------------------------------------------------------------
    // Returns the scaled x,y,size.
    public Rect GetNodeNameGUIPosition(iCS_EditorObject node, iCS_IStorage iStorage) {
        Rect graphRect= GetNodeNamePosition(node, iStorage);
        var guiPos= TranslateAndScale(Math3D.ToVector2(graphRect));
        return new Rect(guiPos.x, guiPos.y, graphRect.width, graphRect.height);	    
    }
    // ----------------------------------------------------------------------
    // Returns the tooltip for the given node.
	string GetNodeTooltip(iCS_EditorObject node, iCS_IStorage iStorage) {
		string tooltip= "Name: "+(node.RawName ?? "")+"\n";
		// Type information
		Type runtimeType= node.RuntimeType;
		if(runtimeType != null) tooltip+= "Type: "+iCS_Types.TypeName(runtimeType)+"\n";
		// Number of direct children
		int nbOfChildren= 0;
		iStorage.ForEachChildNode(node, c=> ++nbOfChildren);
		tooltip+= "Child nodes: "+nbOfChildren+"\n";
		// Number of descendents.
		nbOfChildren= 0;
		iStorage.ForEachChildRecursive(node, c=> { if(c.IsNode) ++nbOfChildren; });
		tooltip+= "Total nodes: "+nbOfChildren+"\n";
		// User defined tooltip
		if(iCS_Strings.IsNotEmpty(node.Tooltip)) tooltip+= node.Tooltip;
		return tooltip;
	}
}
