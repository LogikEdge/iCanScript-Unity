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
    /// Defines the base class for all library objects that are based on a
    /// MemberInfo.
    public abstract class LibraryMemberInfo : LibraryTypeMember {
        // ======================================================================
        // FIELDS
        // ----------------------------------------------------------------------
        public MemberInfo   memberInfo= null;
        
        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
		public string		memberName	     { get { return memberInfo.Name; }}
        public MemberTypes  memberType       { get { return memberInfo.MemberType; }}
		public Type			declaringType    { get { return memberInfo.DeclaringType; }}
        public bool         isInherited   {
            get {
				var libraryType= parent as LibraryType;
            	return libraryType.type != memberInfo.DeclaringType;
            }
        }
        public string displayNameHeader {
			get { return (EditorGUIUtility.isProSkin ? "<color=cyan><b>" : "<color=blue><b>"); }
        }
        public string displayNameTrailer {
   			get { return "</b></color>"; }
        }

        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryMemberInfo(MemberInfo memberInfo) : base() {
            this.memberInfo= memberInfo;
        }

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal override string GetRawName()	{ return memberInfo.Name; }
		internal override string GetNodeName()	{ return NameUtility.ToDisplayName(GetRawName()); }
        internal override string GetDisplayString() {
			var displayString= new StringBuilder(mainValueBegin,32);
			displayString.Append(GetNodeName());
			displayString.Append(mainValueEnd);
			return displayString.ToString();
		}
    }
    
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	/// Defines the base class for all fields in the library.
    public class LibraryField : LibraryMemberInfo {
        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
		public FieldInfo		fieldInfo	{ get { return memberInfo as FieldInfo; }}
		public bool				isStatic	{ get { return fieldInfo.IsStatic; }}
		public bool				isConst     { get { return fieldInfo.IsLiteral; }}
        public string           fieldName   { get { return fieldInfo.Name; }}
        public Type             fieldType   { get { return fieldInfo.FieldType; }}
		
        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryField(MemberInfo memberInfo) : base(memberInfo) {}
    }

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	/// Defines the class that represents a programatic type field.
    public class LibraryFieldGetter : LibraryField {
        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryFieldGetter(MemberInfo memberInfo) : base(memberInfo) {}

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal override string GetRawName()	{ return "get_"+memberInfo.Name; }
        // ----------------------------------------------------------------------		
		/// Retruns the library icon for a field get node.
		internal override Texture   GetLibraryIcon() {
            return iCS_BuiltinTextures.OutEndPortIcon;
		}
        // ----------------------------------------------------------------------		
		internal override string GetDisplayString() {
			var result= new StringBuilder(64);
			result.Append(mainValueBegin);
			result.Append(GetNodeName());
			result.Append(mainValueEnd);
			result.Append(" --> ");
			result.Append(secondPartBegin);
			result.Append(NameUtility.ToDisplayName(fieldType));
			result.Append(secondPartEnd);
			return result.ToString();
		}
    }

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	/// Defines the class that represents a programatic type field.
    public class LibraryFieldSetter : LibraryField {
        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryFieldSetter(MemberInfo memberInfo) : base(memberInfo) {}

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal override string GetRawName()	{ return "set_"+memberInfo.Name; }
        // ----------------------------------------------------------------------		
		/// Retruns the library icon for a field set node.
		internal override Texture   GetLibraryIcon() {
            return iCS_BuiltinTextures.InEndPortIcon;
		}
        // ----------------------------------------------------------------------		
		internal override string GetDisplayString() {
			var result= new StringBuilder(64);
			result.Append(mainValueBegin);
			result.Append(GetNodeName());
			result.Append(mainValueEnd);
			result.Append(" <-- ");
			result.Append(firstPartBegin);
			result.Append(NameUtility.ToDisplayName(fieldType));
			result.Append(firstPartEnd);
			return result.ToString();
		}
    }

}
