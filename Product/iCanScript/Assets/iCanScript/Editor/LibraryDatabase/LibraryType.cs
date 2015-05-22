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
    /// Defines the class that represents a programming type in the library.
    public class LibraryType : LibraryObject {
        // ======================================================================
        // FIELDS
        // ----------------------------------------------------------------------
        public Type    type= null;

        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
		public Type		baseType			{ get { return type.BaseType; }}
		public bool 	isGeneric 			{ get { return type.IsGenericType; }}
		public Type[]	genericArguments	{ get { return type.GetGenericArguments(); }}
		
        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryType(Type type) : base()    { this.type= type; }

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal override string GetRawName()   { return type.Name; }
		internal override string GetNodeName()	{ return NameUtility.ToDisplayName(type); }
        internal override string GetDisplayString() {
			// -- Start with the base name --
			var displayName= new StringBuilder(mainValueBegin, 64);
			displayName.Append(NameUtility.ToDisplayNameNoGenericArguments(type));
			// -- Add generic arguments --
			if(isGeneric) {
				displayName.Append("<i>");
				displayName.Append(NameUtility.ToDisplayGenericArguments(type));
				displayName.Append("</i>");
			}
			displayName.Append(mainValueEnd);
			// -- Add inheritance information --
			if(baseType != null && baseType != typeof(void)) {
				displayName.Append(firstPartBegin);
				displayName.Append(" : ");
				displayName.Append(NameUtility.ToDisplayName(baseType));
				displayName.Append(firstPartEnd);
			}
			return displayName.ToString();
		}
        // ----------------------------------------------------------------------		
		/// Retruns the library icon for a type node.
		internal override Texture GetLibraryIcon() {
            return TextureCache.GetIcon(Icons.kTypeIcon);
		}		

        // ======================================================================
        // UTILITIES
        // ----------------------------------------------------------------------
        /// Sorts the child members according to their type.
        public void Sort(float minScore) {
            // -- Sort our children --
            Sort<LibraryTypeMember>(
                (x,y)=> {
                    // -- Handler null parameters. --
                    if (x == null && y == null) return 0;
                    else if (x == null) return -1;
                    else if (y == null) return 1;

                    // -- Sort according to score first --
                    float scoreDiff= y.score-x.score;
                    if(Math3D.IsNotZero(scoreDiff)) {
                        return scoreDiff < 0 ? -1 : 1;
                    }

                    // -- If score equal, then sort alphabetically. --
                    if(x.stringSortIndex != -1 && y.stringSortIndex != -1) {
                        return x.stringSortIndex-y.stringSortIndex;
                    }
                    if(x.isField && !(y.isField)) return -1;
                    if(y.isField && !(x.isField)) return 1;
                    if(x.isProperty && !(y.isProperty)) return -1;
                    if(y.isProperty && !(x.isProperty)) return 1;
                    if(x.isConstructor && !(y.isConstructor)) return -1;
                    if(y.isConstructor && !(x.isConstructor)) return 1;
                    return string.Compare(x.GetNodeName(), y.GetNodeName());
                }
            );
        }

        // ----------------------------------------------------------------------
        /// Returns the members of type T installed on this type.
        ///
        /// @return The array of member <T> installed on this type.
        ///
        public T[] GetMembers<T>() where T : LibraryObject {
            var events= P.filter(p=> p is T, children);
            return P.map(p=> p as T, events).ToArray();
        }

        // ----------------------------------------------------------------------
		/// Computes the visibility for the namespace library object.
        ///
        /// Updates the number of visible namespaces.
        ///
		public override void ComputeVisibility() {
            base.ComputeVisibility();
            if(isVisible) this.libraryRoot.IncrementVisibleTypeCount();
		}
        

    }
    
}
