using UnityEngine;
using UnityEditor;
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
    string GetNodeName(iCS_EditorObject node, iCS_IStorage iStorage) {
        return ObjectNames.NicifyVariableName(iStorage.Preferences.HiddenPrefixes.GetName(node.Name));    
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
        float x= 0.5f*(pos.x+pos.xMax-size.x);
        float y= pos.y;
        if(IsMinimized(node, iStorage)) {
            y-= 5f+size.y;
        } else {
            y+= 0.5f*(kNodeCornerRadius-size.y/Scale);
        }
        return new Rect(x, y, size.x, size.y);
    }
    // ----------------------------------------------------------------------
    // Returns the scaled x,y,size.
    Rect GetNodeNameGUIPosition(iCS_EditorObject port, iCS_IStorage iStorage) {
        Rect graphRect= GetNodeNamePosition(port, iStorage);
        var guiPos= TranslateAndScale(Math3D.ToVector2(graphRect));
        return new Rect(guiPos.x, guiPos.y, graphRect.width, graphRect.height);	    
    }

}
