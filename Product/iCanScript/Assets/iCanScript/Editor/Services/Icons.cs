using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Internal.Editor {
	
	public enum DefaultNodeIcons {
	    iCanScript, Unity, DotNet, Company, Library,
	    Package, Message, ObjectInstance,
	    Function, Variable, Builder,
	    StateChart, State, EntryState,
		OnStateEntry, OnStateUpdate, OnStateExit
	};
	
	public static class Icons {
	    // =========================================================================
	    // Default Icon file names.
		// -------------------------------------------------------------------------
	    public const string kiCanScriptIcon    = "iCS_Logo_32x32.png";
	    public const string kUnityIcon         = "iCS_UnityLogo_32x32.png";
	    public const string kDotNetIcon        = "iCS_DotNetLogo_32x32.png";
	    public const string kCompanyIcon       = "iCS_CompanyIcon_32x32.png";
	    public const string kNamespaceIcon     = "iCS_LibraryIcon_32x32.png";
	    public const string kFunctionIcon      = "iCS_FunctionIcon_32x32.png";
	    public const string kConstructorIcon   = "iCS_BuilderIcon_32x32.png";
	    public const string kTypeIcon          = "iCS_ObjectInstanceIcon_32x32.png";
	    public const string kEventHandlerIcon  = "iCS_MessageIcon_32x32.png";
	    const string kPackageIcon       = "iCS_PackageIcon_32x32.png";
	    const string kVariableIcon      = "iCS_VariableIcon_32x32.png";
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
	            icon= TextureCache.GetIcon(kOnStateEntryIcon);
				if(icon != null) return icon;
			} else if(obj.IsOnStateUpdatePackage) {
	            icon= TextureCache.GetIcon(kOnStateUpdateIcon);			
				if(icon != null) return icon;
			} else if(obj.IsOnStateExitPackage) {
	            icon= TextureCache.GetIcon(kOnStateExitIcon);		
				if(icon != null) return icon;
			}
	        // Use user defined icon if it exists ... otherwise use default for the
	        // given type of node.
	        if(obj.IconGUID != null) {
	        	icon= TextureCache.GetIconFromGUID(obj.IconGUID);
	            if(icon != null) return icon;
	        }
	        return GetDefaultNodeIconFor(obj);
	    }
	
	    // -------------------------------------------------------------------------
	    public static Texture2D GetDefaultNodeIconFor(iCS_EditorObject obj) {
	        if(obj.IsEventHandler) {
	            return GetDefaultNodeIconFor(DefaultNodeIcons.Message);            
	        } else if(obj.IsInstanceNode) {
	            return GetDefaultNodeIconFor(DefaultNodeIcons.ObjectInstance);
	        } else if(obj.IsKindOfPackage) {
	            return GetDefaultNodeIconFor(DefaultNodeIcons.Package);
	        } else if(obj.IsConstructor) {
	            return GetDefaultNodeIconFor(DefaultNodeIcons.Builder);
	        } else if(obj.IsKindOfFunction) {
	            return GetDefaultNodeIconFor(DefaultNodeIcons.Function);
	        } else if(obj.IsStateChart) {
	            return GetDefaultNodeIconFor(DefaultNodeIcons.StateChart);            
	        } else if(obj.IsEntryState) {
	            return GetDefaultNodeIconFor(DefaultNodeIcons.EntryState);            
	        } else if(obj.IsState) {
	            return GetDefaultNodeIconFor(DefaultNodeIcons.State);
			} else if(obj.IsOnStateEntryPackage) {
				return GetDefaultNodeIconFor(DefaultNodeIcons.OnStateEntry);
			} else if(obj.IsOnStateUpdatePackage) {
				return GetDefaultNodeIconFor(DefaultNodeIcons.OnStateUpdate);
			} else if(obj.IsOnStateExitPackage) {
				return GetDefaultNodeIconFor(DefaultNodeIcons.OnStateExit);
	        }
	        return TextureCache.GetIcon(kDefaultIcon);
	    }
	
	    // -------------------------------------------------------------------------
	    public static Texture2D GetDefaultNodeIconFor(DefaultNodeIcons iconType) {
	        Texture2D icon= null;
	        switch(iconType) {
	            case DefaultNodeIcons.iCanScript:
	                icon= TextureCache.GetIcon(kiCanScriptIcon); break;
	            case DefaultNodeIcons.Unity:
	                icon= TextureCache.GetIcon(kUnityIcon); break;
	            case DefaultNodeIcons.DotNet:
	                icon= TextureCache.GetIcon(kDotNetIcon); break;
	            case DefaultNodeIcons.Company:
	                icon= TextureCache.GetIcon(kCompanyIcon); break;
	            case DefaultNodeIcons.Package:
	                icon= TextureCache.GetIcon(kPackageIcon); break;
	            case DefaultNodeIcons.Message:
	                icon= TextureCache.GetIcon(kEventHandlerIcon); break;
	            case DefaultNodeIcons.ObjectInstance:
	                icon= TextureCache.GetIcon(kTypeIcon); break;
	            case DefaultNodeIcons.Function:
	                icon= TextureCache.GetIcon(kFunctionIcon); break;
	            case DefaultNodeIcons.Variable:
	                icon= TextureCache.GetIcon(kVariableIcon); break;
	            case DefaultNodeIcons.Builder:
	                icon= TextureCache.GetIcon(kConstructorIcon); break;
	            case DefaultNodeIcons.StateChart:
	                icon= TextureCache.GetIcon(kStateChartIcon); break;
	            case DefaultNodeIcons.State:
	                icon= TextureCache.GetIcon(kStateIcon); break;
	            case DefaultNodeIcons.EntryState:
	                icon= TextureCache.GetIcon(kEntryStateIcon); break;
	            case DefaultNodeIcons.OnStateEntry:
	                icon= TextureCache.GetIcon(kOnStateEntryIcon); break;
	            case DefaultNodeIcons.OnStateUpdate:
	                icon= TextureCache.GetIcon(kOnStateUpdateIcon); break;
	            case DefaultNodeIcons.OnStateExit:
	                icon= TextureCache.GetIcon(kOnStateExitIcon); break;
	        }
	        return icon == null ? TextureCache.GetIcon(kDefaultIcon) : icon;
	    }
	
	    // -------------------------------------------------------------------------
	    public static Texture2D GetLibraryIconFor(DefaultNodeIcons iconType) {
	        Texture2D icon= null;
	        switch(iconType) {
	            case DefaultNodeIcons.iCanScript:
	                icon= TextureCache.GetIcon(kiCanScriptLibraryIcon); break;
	            case DefaultNodeIcons.Unity:
	                icon= TextureCache.GetIcon(kUnityLibraryIcon); break;
	            case DefaultNodeIcons.DotNet:
	                icon= TextureCache.GetIcon(kDotNetLibraryIcon); break;
	            case DefaultNodeIcons.Company:
	                icon= TextureCache.GetIcon(kCompanyLibraryIcon); break;
	            case DefaultNodeIcons.Library:
	                icon= TextureCache.GetIcon(kLibraryIcon); break;            
	            case DefaultNodeIcons.Package:
	                icon= TextureCache.GetIcon(kPackageLibraryIcon); break;
	            case DefaultNodeIcons.Message:
	                icon= TextureCache.GetIcon(kMessageLibraryIcon); break;
	            case DefaultNodeIcons.ObjectInstance:
	                icon= TextureCache.GetIcon(kObjectInstanceLibraryIcon); break;
	            case DefaultNodeIcons.Function:
	                icon= TextureCache.GetIcon(kFunctionLibraryIcon); break;
	            case DefaultNodeIcons.Variable:
	                icon= TextureCache.GetIcon(kVariableLibraryIcon); break;
	            case DefaultNodeIcons.Builder:
	                icon= TextureCache.GetIcon(kBuilderLibraryIcon); break;
	            case DefaultNodeIcons.StateChart:
	                icon= TextureCache.GetIcon(kStateChartLibraryIcon); break;
	            case DefaultNodeIcons.State:
	                icon= TextureCache.GetIcon(kStateLibraryIcon); break;
	            case DefaultNodeIcons.EntryState:
	                icon= TextureCache.GetIcon(kEntryStateLibraryIcon); break;
	        }
	        return icon == null ? TextureCache.GetIcon(kPackageLibraryIcon) : icon;
	    }
	
	}
}

    