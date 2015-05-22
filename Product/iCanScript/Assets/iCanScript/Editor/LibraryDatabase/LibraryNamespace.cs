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
    /// Defines the first level namespace in the library.
    public class LibraryRootNamespace : LibraryObject {
        // ======================================================================
        // FIELDS
        // ----------------------------------------------------------------------
        string myName= null;

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal override string 	GetRawName()        { return myName; }
		internal override string 	GetNodeName()		{ return NameUtility.ToDisplayName(myName); }
        internal override string	GetDisplayString()	{ return GetNodeName(); }
		internal override Texture	GetLibraryIcon() {
			if(myName.StartsWith("iCanScript")) {
				return TextureCache.GetIcon(Icons.kiCanScriptIcon);
			}
			if(myName.StartsWith("System")) {
				return TextureCache.GetIcon(Icons.kDotNetIcon);
			}
			if(myName.StartsWith("Unity")) {
				return TextureCache.GetIcon(Icons.kUnityIcon);
			}
			return TextureCache.GetIcon(Icons.kCompanyIcon);
		}

        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryRootNamespace(string name) : base() { myName= name; }

        // ======================================================================
        // UTILITIES
        // ----------------------------------------------------------------------
        /// Returns the child namespace library object with the given name.
        ///
        /// @param name The name of the child namespace to search for.
        /// @return The found or created child namespace library object.
        ///
        public LibraryChildNamespace GetChildNamespace(string name) {
            var node= GetChild<LibraryChildNamespace>(t=> t.GetRawName() == name);
            if(node == null) {
                node= new LibraryChildNamespace(name);
                AddChild(node);
            }
            return node;
        }

        // ----------------------------------------------------------------------
        /// Sorts the child namespaces and ask all children to perform sorting.
        public void Sort(float minScore) {
            // -- Sort our children --
            Sort<LibraryChildNamespace>(
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
                    return string.Compare(x.GetRawName(), y.GetRawName());
                }
            );
            // -- Ask our children to sort their children on so on... -- 
            foreach(var c in children) {
                var child= c as LibraryChildNamespace;
                if(child.score >= minScore) {
                    child.Sort(minScore);
                }
            }
        }

        // ----------------------------------------------------------------------
		/// Computes the visibility for the namespace root object.
        ///
        /// Updates the number of visible namespaces.
        ///
		public override void ComputeVisibility() {
            base.ComputeVisibility();
            if(this.libraryRoot.showUnityEditorMembers == false) {
                if(GetRawName() == "UnityEditor") {
                    myIsVisible= false;
                }
            }
            if(isVisible) this.libraryRoot.IncrementVisibleNamespaceCount();
		}
        
    }

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// Defines the nested namespaces in the library (2nd level).
    public class LibraryChildNamespace : LibraryObject {
        // ======================================================================
        // FIELDS
        // ----------------------------------------------------------------------
        string myName= null;

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal override string 	GetRawName()        { return myName; }
		internal override string 	GetNodeName()		{ return NameUtility.ToDisplayName(myName); }
        internal override string	GetDisplayString()	{ return GetNodeName(); }
		internal override Texture   GetLibraryIcon() {
            return TextureCache.GetIcon(Icons.kNamespaceIcon);
		}

        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryChildNamespace(string name) : base() { myName= name; }

        // ======================================================================
        // UTILITIES
        // ----------------------------------------------------------------------
        /// Returns the type library object for the given type.
        ///
        /// @param type The type to search for.
        /// @return The found library type object or _null_ if not found.
        ///
        public LibraryType GetLibraryType(Type type) {
            return GetChild<LibraryType>(t=> t.type == type);
        }

        // ----------------------------------------------------------------------
        /// Sorts the child types and ask all children to perform sorting.
        public void Sort(float minScore) {
            // -- Sort our children --
            Sort<LibraryType>(
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
					return string.Compare(x.GetRawName(), y.GetRawName());
				}
            );
            // -- Ask our children to sort their children on so on... -- 
            foreach(var c in children) {
                var child= c as LibraryType;
                if(child.isVisible) {
                    child.Sort(minScore);
                }
            }
        }
        
        // ----------------------------------------------------------------------
		/// Computes the visibility for the namespace library object.
        ///
        /// Updates the number of visible namespaces.
        ///
		public override void ComputeVisibility() {
            base.ComputeVisibility();
            if(isVisible) this.libraryRoot.IncrementVisibleNamespaceCount();
		}
        
    }
    
}
