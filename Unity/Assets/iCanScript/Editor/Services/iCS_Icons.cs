using UnityEngine;
using UnityEditor;
using System.Collections;

public static class iCS_Icons {
    // =========================================================================
    // Icon file names.
	// -------------------------------------------------------------------------
    public const string kFunctionIcon         = "iCS_FunctionIcon_32x32.png";

    // -------------------------------------------------------------------------
    public static Texture2D GetIconFor(iCS_EditorObject obj) {
        // No default icon for null object.
        if(obj == null) {
            return null;
        }
        // Use user defined icon if it exists ... otherwise use default for the
        // given type of node.
//        if(obj.IsNode) {
            if(obj.IconGUID != null) {
                Texture2D icon= iCS_TextureCache.GetIconFromGUID(obj.IconGUID);
                if(icon != null) return icon;
            }
            return GetDefaultNodeIconFor(obj);
//        }
        // Return default port icon.            
    }

    // -------------------------------------------------------------------------
    public static Texture2D GetDefaultNodeIconFor(iCS_EditorObject obj) {
        if(obj.IsBehaviour) {
            
        } else if(obj.IsMessage) {
            return iCS_TextureCache.GetIcon(iCS_EditorStrings.MessageHierarchyIcon);            
        } else if(obj.IsKindOfFunction) {
            return iCS_TextureCache.GetIcon(kFunctionIcon);
        } else if(obj.IsStateChart) {
            
        } else if(obj.IsEntryState) {

        } else if(obj.IsState) {
            
        }
        return iCS_TextureCache.GetIcon(iCS_EditorStrings.FunctionIcon);
    }
}
