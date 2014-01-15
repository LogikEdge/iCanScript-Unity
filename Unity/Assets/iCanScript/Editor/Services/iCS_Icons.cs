using UnityEngine;
using UnityEditor;
using System.Collections;

public enum iCS_DefaultNodeIcons {
    iCanScript, Unity, DotNet, Company, Library,
    Behaviour, Package, Message, ObjectInstance,
    Function, Variable, Builder,
    StateChart, State, EntryState,
	OnStateEntry, OnStateUpdate, OnStateExit
};

public static class iCS_Icons {
    // =========================================================================
    // Default Icon file names.
	// -------------------------------------------------------------------------
    const string kiCanScriptIcon    = "iCS_Logo_32x32.png";
    const string kUnityIcon         = "iCS_UnityLogo_32x32.png";
    const string kDotNetIcon        = "iCS_DotNetLogo_32x32.png";
    const string kCompanyIcon       = "iCS_CompanyIcon_32x32.png";
    const string kBehaviourIcon     = "iCS_BehaviourIcon_32x32.png";
    const string kPackageIcon       = "iCS_PackageIcon_32x32.png";
    const string kMessageIcon       = "iCS_MessageIcon_32x32.png";
    const string kObjectInstanceIcon= "iCS_ObjectInstanceIcon_32x32.png";
    const string kFunctionIcon      = "iCS_FunctionIcon_32x32.png";
    const string kVariableIcon      = "iCS_VariableIcon_32x32.png";
    const string kBuilderIcon       = "iCS_BuilderIcon_32x32.png";
    const string kStateChartIcon    = "iCS_StateChartIcon_32x32.png";
    const string kStateIcon         = "iCS_StateIcon_32x32.png";
    const string kEntryStateIcon    = "iCS_EntryStateIcon_32x32.png";
    const string kDefaultIcon       = "iCS_DefaultIcon_32x32.png";
	const string kOnStateEntryIcon	= "ics-on-state-entry-icon_32x32x32.png";
	const string kOnStateUpdateIcon	= "ics-on-state-update-icon_32x32x32.png";
	const string kOnStateExitIcon   = "ics-on-state-exit-icon_32x32x32.png";
	// -------------------------------------------------------------------------
    const string kiCanScriptLibraryIcon    = "iCS_Logo_32x32.png";
    const string kUnityLibraryIcon         = "iCS_UnityLogo_32x32.png";
    const string kDotNetLibraryIcon        = "iCS_DotNetLogo_32x32.png";
    const string kCompanyLibraryIcon       = "iCS_CompanyIcon_32x32.png";
    const string kLibraryIcon              = "iCS_LibraryIcon_32x32.png";
    const string kBehaviourLibraryIcon     = "iCS_BehaviourIcon_32x32.png";
    const string kPackageLibraryIcon       = "iCS_PackageIcon_32x32.png";
    const string kMessageLibraryIcon       = "iCS_MessageIcon_32x32.png";
    const string kObjectInstanceLibraryIcon= "iCS_ObjectInstanceIcon_16x16.png";
    const string kFunctionLibraryIcon      = "iCS_FunctionIcon_32x32.png";
    const string kVariableLibraryIcon      = "iCS_VariableIcon_32x32.png";
    const string kBuilderLibraryIcon       = "iCS_BuilderIcon_16x16.png";
    const string kStateChartLibraryIcon    = "iCS_StateChartIcon_16x16.png";
    const string kStateLibraryIcon         = "iCS_StateIcon_16x16.png";
    const string kEntryStateLibraryIcon    = "iCS_EntryStateIcon_16x16.png";
    
    // -------------------------------------------------------------------------
    public static Texture2D GetIconFor(iCS_EditorObject obj) {
        // No default icon for null object.
        if(obj == null) {
            return null;
        }
		// Product icons cannot be changed.
		Texture2D icon= null;
		if(obj.IsOnStateEntryPackage) {
            icon= iCS_TextureCache.GetIcon(kOnStateEntryIcon);
			if(icon != null) return icon;
		} else if(obj.IsOnStateUpdatePackage) {
            icon= iCS_TextureCache.GetIcon(kOnStateUpdateIcon);			
			if(icon != null) return icon;
		} else if(obj.IsOnStateExitPackage) {
            icon= iCS_TextureCache.GetIcon(kOnStateExitIcon);		
			if(icon != null) return icon;
		}
        // Use user defined icon if it exists ... otherwise use default for the
        // given type of node.
        if(obj.IconGUID != null) {
        	icon= iCS_TextureCache.GetIconFromGUID(obj.IconGUID);
            if(icon != null) return icon;
        }
        return GetDefaultNodeIconFor(obj);
    }

    // -------------------------------------------------------------------------
    public static Texture2D GetDefaultNodeIconFor(iCS_EditorObject obj) {
        if(obj.IsBehaviour) {
            return GetDefaultNodeIconFor(iCS_DefaultNodeIcons.Behaviour);
        } else if(obj.IsMessage) {
            return GetDefaultNodeIconFor(iCS_DefaultNodeIcons.Message);            
        } else if(obj.IsInstanceNode) {
            return GetDefaultNodeIconFor(iCS_DefaultNodeIcons.ObjectInstance);
        } else if(obj.IsKindOfPackage) {
            return GetDefaultNodeIconFor(iCS_DefaultNodeIcons.Package);
        } else if(obj.IsConstructor) {
            return GetDefaultNodeIconFor(iCS_DefaultNodeIcons.Builder);
        } else if(obj.IsKindOfFunction) {
            return GetDefaultNodeIconFor(iCS_DefaultNodeIcons.Function);
        } else if(obj.IsStateChart) {
            return GetDefaultNodeIconFor(iCS_DefaultNodeIcons.StateChart);            
        } else if(obj.IsEntryState) {
            return GetDefaultNodeIconFor(iCS_DefaultNodeIcons.EntryState);            
        } else if(obj.IsState) {
            return GetDefaultNodeIconFor(iCS_DefaultNodeIcons.State);
		} else if(obj.IsOnStateEntryPackage) {
			return GetDefaultNodeIconFor(iCS_DefaultNodeIcons.OnStateEntry);
		} else if(obj.IsOnStateUpdatePackage) {
			return GetDefaultNodeIconFor(iCS_DefaultNodeIcons.OnStateUpdate);
		} else if(obj.IsOnStateExitPackage) {
			return GetDefaultNodeIconFor(iCS_DefaultNodeIcons.OnStateExit);
		}
        return iCS_TextureCache.GetIcon(kDefaultIcon);
    }

    // -------------------------------------------------------------------------
    public static Texture2D GetDefaultNodeIconFor(iCS_DefaultNodeIcons iconType) {
        Texture2D icon= null;
        switch(iconType) {
            case iCS_DefaultNodeIcons.iCanScript:
                icon= iCS_TextureCache.GetIcon(kiCanScriptIcon); break;
            case iCS_DefaultNodeIcons.Unity:
                icon= iCS_TextureCache.GetIcon(kUnityIcon); break;
            case iCS_DefaultNodeIcons.DotNet:
                icon= iCS_TextureCache.GetIcon(kDotNetIcon); break;
            case iCS_DefaultNodeIcons.Company:
                icon= iCS_TextureCache.GetIcon(kCompanyIcon); break;
            case iCS_DefaultNodeIcons.Behaviour:
                icon= iCS_TextureCache.GetIcon(kBehaviourIcon); break;
            case iCS_DefaultNodeIcons.Package:
                icon= iCS_TextureCache.GetIcon(kPackageIcon); break;
            case iCS_DefaultNodeIcons.Message:
                icon= iCS_TextureCache.GetIcon(kMessageIcon); break;
            case iCS_DefaultNodeIcons.ObjectInstance:
                icon= iCS_TextureCache.GetIcon(kObjectInstanceIcon); break;
            case iCS_DefaultNodeIcons.Function:
                icon= iCS_TextureCache.GetIcon(kFunctionIcon); break;
            case iCS_DefaultNodeIcons.Variable:
                icon= iCS_TextureCache.GetIcon(kVariableIcon); break;
            case iCS_DefaultNodeIcons.Builder:
                icon= iCS_TextureCache.GetIcon(kBuilderIcon); break;
            case iCS_DefaultNodeIcons.StateChart:
                icon= iCS_TextureCache.GetIcon(kStateChartIcon); break;
            case iCS_DefaultNodeIcons.State:
                icon= iCS_TextureCache.GetIcon(kStateIcon); break;
            case iCS_DefaultNodeIcons.EntryState:
                icon= iCS_TextureCache.GetIcon(kEntryStateIcon); break;
            case iCS_DefaultNodeIcons.OnStateEntry:
                icon= iCS_TextureCache.GetIcon(kOnStateEntryIcon); break;
            case iCS_DefaultNodeIcons.OnStateUpdate:
                icon= iCS_TextureCache.GetIcon(kOnStateUpdateIcon); break;
            case iCS_DefaultNodeIcons.OnStateExit:
                icon= iCS_TextureCache.GetIcon(kOnStateExitIcon); break;
        }
        return icon == null ? iCS_TextureCache.GetIcon(kDefaultIcon) : icon;
    }

    // -------------------------------------------------------------------------
    public static Texture2D GetLibraryNodeIconFor(iCS_DefaultNodeIcons iconType) {
        Texture2D icon= null;
        switch(iconType) {
            case iCS_DefaultNodeIcons.iCanScript:
                icon= iCS_TextureCache.GetIcon(kiCanScriptLibraryIcon); break;
            case iCS_DefaultNodeIcons.Unity:
                icon= iCS_TextureCache.GetIcon(kUnityLibraryIcon); break;
            case iCS_DefaultNodeIcons.DotNet:
                icon= iCS_TextureCache.GetIcon(kDotNetLibraryIcon); break;
            case iCS_DefaultNodeIcons.Company:
                icon= iCS_TextureCache.GetIcon(kCompanyLibraryIcon); break;
            case iCS_DefaultNodeIcons.Library:
                icon= iCS_TextureCache.GetIcon(kLibraryIcon); break;            
            case iCS_DefaultNodeIcons.Behaviour:
                icon= iCS_TextureCache.GetIcon(kBehaviourLibraryIcon); break;
            case iCS_DefaultNodeIcons.Package:
                icon= iCS_TextureCache.GetIcon(kPackageLibraryIcon); break;
            case iCS_DefaultNodeIcons.Message:
                icon= iCS_TextureCache.GetIcon(kMessageLibraryIcon); break;
            case iCS_DefaultNodeIcons.ObjectInstance:
                icon= iCS_TextureCache.GetIcon(kObjectInstanceLibraryIcon); break;
            case iCS_DefaultNodeIcons.Function:
                icon= iCS_TextureCache.GetIcon(kFunctionLibraryIcon); break;
            case iCS_DefaultNodeIcons.Variable:
                icon= iCS_TextureCache.GetIcon(kVariableLibraryIcon); break;
            case iCS_DefaultNodeIcons.Builder:
                icon= iCS_TextureCache.GetIcon(kBuilderLibraryIcon); break;
            case iCS_DefaultNodeIcons.StateChart:
                icon= iCS_TextureCache.GetIcon(kStateChartLibraryIcon); break;
            case iCS_DefaultNodeIcons.State:
                icon= iCS_TextureCache.GetIcon(kStateLibraryIcon); break;
            case iCS_DefaultNodeIcons.EntryState:
                icon= iCS_TextureCache.GetIcon(kEntryStateLibraryIcon); break;
        }
        return icon == null ? iCS_TextureCache.GetIcon(kPackageLibraryIcon) : icon;
    }

}

    