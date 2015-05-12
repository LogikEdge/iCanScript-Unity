using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using P= iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	/// Defines the base class that represents a property in the library.
    public class LibraryProperty : LibraryMethodInfo {
        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
        public string	propertyName	{ get { return methodInfo.Name; }}

        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryProperty(MethodInfo methodInfo) : base(methodInfo) {}
    }
    
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	/// Defines the class that represents a property get in the library.
    public class LibraryPropertyGetter : LibraryProperty {
        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryPropertyGetter(MethodInfo methodInfo) : base(methodInfo) {}

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------		
		internal override string GetDisplayString() {
			var result= new StringBuilder(64);
			result.Append(mainValueBegin);
			result.Append(GetNodeName());
			result.Append(mainValueEnd);
			result.Append(" --> ");
			result.Append(secondPartBegin);
			result.Append(NameUtility.ToDisplayName(returnType));
			result.Append(secondPartEnd);
			return result.ToString();
		}
        // ----------------------------------------------------------------------
		internal override string GetNodeName() {
            return NameUtility.ToDisplayName(memberInfo.Name);
		}
        // ----------------------------------------------------------------------		
		/// Retruns the library icon for a property get node.
		internal override Texture   GetLibraryIcon() {
            return iCS_BuiltinTextures.OutEndPortIcon;
		}
		
    }

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	/// Defines the class that represents a property set in the library.
    public class LibraryPropertySetter : LibraryProperty {
        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryPropertySetter(MethodInfo methodInfo) : base(methodInfo) {}

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------		
		internal override string GetDisplayString() {
			var result= new StringBuilder(64);
			result.Append(mainValueBegin);
			result.Append(GetNodeName());
			result.Append(mainValueEnd);
			result.Append(" <-- ");
			result.Append(firstPartBegin);
			result.Append(NameUtility.ToDisplayName(parameters[0].ParameterType));
			result.Append(firstPartEnd);
			return result.ToString();
		}
        // ----------------------------------------------------------------------
		internal override string GetNodeName() {
            return NameUtility.ToDisplayName(memberInfo.Name);
		}
        // ----------------------------------------------------------------------		
		/// Retruns the library icon for a property set node.
		internal override Texture   GetLibraryIcon() {
            return iCS_BuiltinTextures.InEndPortIcon;
		}
		
    }
    
}
