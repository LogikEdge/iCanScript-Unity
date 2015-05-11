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
    /// Defines the root node that contains all library objects.
    public class LibraryRoot : LibraryObject {
        // ======================================================================
        // INIT
        // ----------------------------------------------------------------------
        public LibraryRoot() {}

        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal override string	GetRawName()        { return "Library"; }
		internal override string	GetNodeName()		{ return GetRawName(); }
        internal override string	GetDisplayString()	{ return GetNodeName(); }

        // ======================================================================
        // UTILITIES
        // ----------------------------------------------------------------------
        /// Returns the root namespace library object with the given name.
        ///
        /// @param name The name of the root namespace to search for.
        /// @return The found or created root namespace library object.
        ///
        public LibraryRootNamespace GetRootNamespace(string name) {
            var node= GetChild<LibraryRootNamespace>(t=> t.GetRawName() == name);
            if(node == null) {
                node= new LibraryRootNamespace(name);
                AddChild(node);
            }
            return node;
        }

        // ----------------------------------------------------------------------
        /// Returns the library type object for the given type.
        ///
        /// @param type The type to search for.
        /// @return The library type if found. _null_ otherwise.
        ///
        public LibraryType GetLibraryType(Type type) {
            string nsRoot= "";
            string nsChildren= "";
			SplitNamespace(type.Namespace, out nsRoot, out nsChildren);
            var rootNamespace = GetRootNamespace(nsRoot);
            if(rootNamespace == null) return null;
            var childNamespace= rootNamespace.GetChildNamespace(nsChildren);
            if(childNamespace == null) return null;
			return childNamespace.GetLibraryType(type);
        }
        
        // ----------------------------------------------------------------------
        /// Sorts the root namespace and ask all children to perform sorting.
        public void Sort() {
            // -- Sort our children --
            float minScore= LibraryObject.kMinScoreFactor*this.score;
            Sort<LibraryRootNamespace>(
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
                var child= c as LibraryRootNamespace;
                if(child.score >= minScore) {
                    child.Sort(minScore);
                }
            }
        }
    }
	
}
