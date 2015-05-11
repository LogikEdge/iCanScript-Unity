using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using P= iCanScript.Prelude;

namespace iCanScript.Editor {

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// Defines a Unity event handler library object.
    public class LibraryEventHandler : LibraryObject {
        // ======================================================================
        // FIELDS
        // ----------------------------------------------------------------------
        string 			myName       = null;
		public Type   	declaringType= null;
		public Type[] 	parameterTypes= null;
		public string[]	parameterNames= null;
		
        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal override string 	GetRawName()        { return myName; }
		internal override string 	GetNodeName()		{ return NameUtility.ToDisplayName(myName); }
        internal override string	GetDisplayString()	{ return GetNodeName(); }
        // ----------------------------------------------------------------------		
		/// Retruns the library icon for an event handler node.
		internal override Texture   GetLibraryIcon() {
            return TextureCache.GetIcon(Icons.kEventHandlerIcon);
		}				

        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryEventHandler(string name, Type declaringType,
								   Type[] parameterTypes, string[] parameterNames)
		: base() {
			myName= name;
			this.declaringType= declaringType;
			this.parameterTypes= parameterTypes;
			this.parameterNames= parameterNames;
		}
    }
    
}
