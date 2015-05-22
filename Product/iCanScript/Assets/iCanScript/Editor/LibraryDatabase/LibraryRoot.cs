using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Internal.FuzzyLogic;
using P= iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// Defines the root node that contains all library objects.
    public class LibraryRoot : LibraryObject {
        // ======================================================================
        // Types
        // ----------------------------------------------------------------------
        /// Class used to store library filtering strings.
        internal class FilterString {
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
        int          myNumberOfVisibleNamespaces= 0;
        int          myNumberOfVisibleTypes     = 0;
        int          myNumberOfVisibleMembers   = 0;
        bool         myShowInheritedMembers     = true;
        bool         myShowProtectedMembers     = false;
        bool         myShowUnityEditorMembers   = true;
        FilterString myNamespaceFilter          = new FilterString("");
        FilterString myTypeFilter               = new FilterString("");
        FilterString myMemberFilter             = new FilterString("");

        // ======================================================================
        // Properties
        // ----------------------------------------------------------------------
        public int numberOfVisibleNamespaces {
            get { return myNumberOfVisibleNamespaces; }
        }
        public int numberOfVisibleTypes {
            get { return myNumberOfVisibleTypes; }
        }
        public int numberOfVisibleMembers {
            get { return myNumberOfVisibleMembers; }
        }
        public bool showInheritedMembers {
            get { return myShowInheritedMembers; }
            set {
                if(myShowInheritedMembers == value) return;
                myShowInheritedMembers= value;
                ComputeVisibility();
            }
        }
        public bool showProtectedMembers {
            get { return myShowProtectedMembers; }
            set {
                if(myShowProtectedMembers == value) return;
                myShowProtectedMembers= value;
                ComputeVisibility();
            }
        }
        public bool showUnityEditorMembers {
            get { return myShowUnityEditorMembers; }
            set {
                if(myShowUnityEditorMembers == value) return;
                myShowUnityEditorMembers= value;
                ComputeVisibility();
            }
        }
        public string namespaceFilter {
            get { return myNamespaceFilter.filter; }
            set {
                if(myNamespaceFilter.filter == value) return;
                myNamespaceFilter.Init(value);
                ComputeNamespaceRawScore();
                ComputeScore();
                ComputeVisibility();
                Sort();
            }
        }
        public string typeFilter {
            get { return myTypeFilter.filter; }
            set {
                if(myTypeFilter.filter == value) return;
                myTypeFilter.Init(value);
                ComputeTypeRawScore();
                ComputeScore();
                ComputeVisibility();
                Sort();
            }
        }
        public string memberFilter {
            get { return myMemberFilter.filter; }
            set {
                if(myMemberFilter.filter == value) return;
                myMemberFilter.Init(value);
//                iCS_Debug.Profile("RawScore", ()=> ComputeMemberRawScore());
                ComputeMemberRawScore();
                ComputeScore();
                ComputeVisibility();
                Sort();
//                iCS_Debug.Profile("Sort", ()=> Sort());
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
            var filterLen= myMemberFilter.filterLength;
			ForEach(
				l=> {
                    if(l is LibraryTypeMember) {
    					var libraryMember= l as LibraryObject;
    					if(filterLen == 0) {
    						libraryMember.rawScore= 1f;
                            libraryMember.searchLength= 0;
    					}
    					else {
    						libraryMember.rawScore= FuzzyString.GetScore(searchString, libraryMember.nodeName.ToUpper());	
                            libraryMember.searchLength= filterLen;
    					}
                        return false;                        
                    }
                    return true;
				}
			);
        }

        // ----------------------------------------------------------------------
		/// Computes the visibility for the entire library.
        ///
        /// We set the number of visible members to zero (0) before asking
        /// the entire tree to compute its visibility.  The type members will
        /// increment the visibility count as they progress.
        ///
		public override void ComputeVisibility() {
            myNumberOfVisibleNamespaces= 0;
            myNumberOfVisibleTypes     = 0;
            myNumberOfVisibleMembers   = 0;
            base.ComputeVisibility();
		}
        
        // ----------------------------------------------------------------------
        /// Used by the type members to increment the count of visible members.
        public void IncrementVisibleNamespaceCount() { ++myNumberOfVisibleNamespaces; } 
        public void IncrementVisibleTypeCount()      { ++myNumberOfVisibleTypes; } 
        public void IncrementVisibleMemberCount()    { ++myNumberOfVisibleMembers; } 
        
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
            if(type == null) return null;
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
                if(child.isVisible) {
                    child.Sort(minScore);
                }
            }
        }
    }
	
}
