using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using iCanScript.FuzzyLogic;
using P= iCanScript.Prelude;

namespace iCanScript.Editor {

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// Defines the root node that contains all library objects.
    public class LibraryRoot : LibraryObject {
        // ======================================================================
        // Types
        // ----------------------------------------------------------------------
        class FilterString {
            public string  filter      = null;
            public string  filterUpper = null;
            public int     filterLength= 0;
            public FilterString(string filter) {
                Init(filter);
            }
            public void Init(string filter) {
                if(filter == null) { filter= ""; }
                this.filter      = filter;
                this.filterUpper = filter.ToUpper();
                this.filterLength= filter.Length;
            }
        }
        
        // ======================================================================
        // Constants
        // ----------------------------------------------------------------------
        public const float kMinScoreFactor= 0.7f;
        
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
        bool         myShowInherited  = true;
        bool         myShowProtected  = false;
        FilterString myNamespaceFilter= new FilterString("");
        FilterString myTypeFilter     = new FilterString("");
        FilterString myMemberFilter   = new FilterString("");

        // ======================================================================
        // Properties
        // ----------------------------------------------------------------------
        public bool showInherited {
            get { return myShowInherited; }
            set {
                if(myShowInherited == value) return;
                myShowInherited= value;
            }
        }
        public bool showProtected {
            get { return myShowProtected; }
            set {
                if(myShowProtected == value) return;
                myShowProtected= value;
            }
        }
        public string namespaceFilter {
            get { return myNamespaceFilter.filter; }
            set {
                if(myNamespaceFilter.filter == value) return;
                myNamespaceFilter.Init(value);
            }
        }
        public string typeFilter {
            get { return myTypeFilter.filter; }
            set {
                if(myTypeFilter.filter == value) return;
                myTypeFilter.Init(value);
            }
        }
        public string memberFilter {
            get { return myMemberFilter.filter; }
            set {
                if(myMemberFilter.filter == value) return;
                myMemberFilter.Init(value);
            }
        }

        // ======================================================================
        // Init
        // ----------------------------------------------------------------------
        public LibraryRoot() {}

        // ======================================================================
        // Interfaces
        // ----------------------------------------------------------------------
        internal override string	GetRawName()        { return "Library"; }
		internal override string	GetNodeName()		{ return GetRawName(); }
        internal override string	GetDisplayString()	{ return GetNodeName(); }

        // ======================================================================
        // Filtering
        // ----------------------------------------------------------------------
		/// Computes the type score for the given string.
		public void ComputeNamespaceRawScore() {
            var searchString= myNamespaceFilter.filterUpper;
			ForEach(
				l=> {
					if(l is LibraryChildNamespace) {
						var libraryObject= l as LibraryObject;
						var rootNamespace= libraryObject.parent as LibraryObject;
						if(myNamespaceFilter.filterLength == 0) {
							libraryObject.rawScore= 1f;
                            libraryObject.searchLength= 0;
						}
						else {
						 	var childScore= FuzzyString.GetScore(searchString, libraryObject.nodeName.ToUpper());						
							var rootScore = FuzzyString.GetScore(searchString, rootNamespace.nodeName.ToUpper());
							libraryObject.rawScore= Mathf.Max(childScore, rootScore);
                            libraryObject.searchLength= myNamespaceFilter.filterLength;
						}						
                        return false;
					}
                    return true;
				}
			);
        }
        // ----------------------------------------------------------------------
		/// Computes the type score for the given string.
		public void ComputeTypeRawScore() {
            var searchString= myTypeFilter.filterUpper;
			ForEach(
				l=> {
					if(l is LibraryType) {
						var libraryType= l as LibraryType;
						if(myTypeFilter.filterLength == 0) {
							libraryType.rawScore= 1f;
                            libraryType.searchLength= 0;
						}
						else {
							libraryType.rawScore= FuzzyString.GetScore(searchString, libraryType.nodeName.ToUpper());						
                            libraryType.searchLength= myTypeFilter.filterLength;
						}
                        return false;						
					}
                    return true;
				}
			);
        }
        // ----------------------------------------------------------------------
		/// Computes the member score for the given string.
		public void ComputeMemberRawScore() {
            var searchString= myMemberFilter.filterUpper;
			ForEach(
				l=> {
					if(l is LibraryType) return true;
					if(l is LibraryChildNamespace) return true;
					if(l is LibraryRootNamespace) return true;
					if(l is LibraryRoot) return true;
					var libraryMember= l as LibraryObject;
					if(myMemberFilter.filterLength == 0) {
						libraryMember.rawScore= 1f;
                        libraryMember.searchLength= 0;
					}
					else {
						libraryMember.rawScore= FuzzyString.GetScore(searchString, libraryMember.nodeName.ToUpper());	
                        libraryMember.searchLength= myMemberFilter.filterLength;
					}
                    return false;
				}
			);
        }
        
        // ======================================================================
        // Utilities
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
            float minScore= kMinScoreFactor * this.score;
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
