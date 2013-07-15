using UnityEngine;
using UnityEditor;
using System.Collections;

public enum iCS_DefaultIcons {
    Company, Library, Behaviour,
    Package, Message, ObjectInstance,
    Function, Variable,
    StateChart, State, EntryState };

public static class iCS_Icons {
    // =========================================================================
    // Icon file names.
	// -------------------------------------------------------------------------
    const string kFunctionIcon= "iCS_FunctionIcon_32x32.png";
    const string kPackageIcon = "iCS_PackageIcon_32x32.png";
    const string kMessageIcon = "iCS_MessageIcon_32x32.png";
	

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
            return GetDefaultNodeIconFor(iCS_DefaultIcons.Message);            
        } else if(obj.IsKindOfFunction) {
            return GetDefaultNodeIconFor(iCS_DefaultIcons.Function);
        } else if(obj.IsStateChart) {
            return GetDefaultNodeIconFor(iCS_DefaultIcons.StateChart);            
        } else if(obj.IsEntryState) {
            return GetDefaultNodeIconFor(iCS_DefaultIcons.EntryState);            
        } else if(obj.IsState) {
            return GetDefaultNodeIconFor(iCS_DefaultIcons.State);
        }
        return GetDefaultNodeIconFor(iCS_DefaultIcons.Package);
    }

    // -------------------------------------------------------------------------
    public static Texture2D GetDefaultNodeIconFor(iCS_DefaultIcons iconType) {
        switch(iconType) {
            case iCS_DefaultIcons.Company:
            case iCS_DefaultIcons.Library:
            case iCS_DefaultIcons.Behaviour:
                break;
            case iCS_DefaultIcons.Package:
                return iCS_TextureCache.GetIcon(kPackageIcon);
            case iCS_DefaultIcons.Message:
                return iCS_TextureCache.GetIcon(kMessageIcon);
            case iCS_DefaultIcons.ObjectInstance:
                break;
            case iCS_DefaultIcons.Function:
                return iCS_TextureCache.GetIcon(kFunctionIcon);
            case iCS_DefaultIcons.Variable:
            case iCS_DefaultIcons.StateChart:
            case iCS_DefaultIcons.State:
            case iCS_DefaultIcons.EntryState:
                break;
        }
        return iCS_TextureCache.GetIcon(kPackageIcon);
    }

}

    