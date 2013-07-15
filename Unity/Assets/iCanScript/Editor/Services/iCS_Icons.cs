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
    // Default Icon file names.
	// -------------------------------------------------------------------------
    const string kCompanyIcon       = "iCS_CompanyIcon_32x32.png";
    const string kLibraryIcon       = "iCS_LibraryIcon_32x32.png";
    const string kBehaviourIcon     = "iCS_BehaviourIcon_32x32.png";
    const string kPackageIcon       = "iCS_PackageIcon_32x32.png";
    const string kMessageIcon       = "iCS_MessageIcon_32x32.png";
    const string kObjectInstanceIcon= "iCS_ObjectInstanceIcon_32x32.png";
    const string kFunctionIcon      = "iCS_FunctionIcon_32x32.png";
    const string kVariableIcon      = "iCS_VariableIcon_32x32.png";
    const string kStateChartIcon    = "iCS_StateChartIcon_32x32.png";
    const string kStateIcon         = "iCS_StateIcon_32x32.png";
    const string kEntryStateIcon    = "iCS_EntryStateIcon_32x32.png";
    

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
        Texture2D icon= null;
        switch(iconType) {
            case iCS_DefaultIcons.Company:
                icon= iCS_TextureCache.GetIcon(kCompanyIcon); break;
            case iCS_DefaultIcons.Library:
                icon= iCS_TextureCache.GetIcon(kLibraryIcon); break;
            case iCS_DefaultIcons.Behaviour:
                icon= iCS_TextureCache.GetIcon(kBehaviourIcon); break;
            case iCS_DefaultIcons.Package:
                icon= iCS_TextureCache.GetIcon(kPackageIcon); break;
            case iCS_DefaultIcons.Message:
                icon= iCS_TextureCache.GetIcon(kMessageIcon); break;
            case iCS_DefaultIcons.ObjectInstance:
                icon= iCS_TextureCache.GetIcon(kObjectInstanceIcon); break;
            case iCS_DefaultIcons.Function:
                icon= iCS_TextureCache.GetIcon(kFunctionIcon); break;
            case iCS_DefaultIcons.Variable:
                icon= iCS_TextureCache.GetIcon(kVariableIcon); break;
            case iCS_DefaultIcons.StateChart:
                icon= iCS_TextureCache.GetIcon(kStateChartIcon); break;
            case iCS_DefaultIcons.State:
                icon= iCS_TextureCache.GetIcon(kStateIcon); break;
            case iCS_DefaultIcons.EntryState:
                icon= iCS_TextureCache.GetIcon(kEntryStateIcon); break;
        }
        return icon == null ? iCS_TextureCache.GetIcon(kPackageIcon) : icon;
    }

}

    