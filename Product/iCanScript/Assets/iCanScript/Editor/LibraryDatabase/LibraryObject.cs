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
    /// Defines the base class to create a tree container.
    public class TreeNode {
        public TreeNode        parent= null;
        public List<TreeNode>  children= null;
        
        public T GetChild<T>(Func<T,bool> cond) where T : TreeNode {
            if(children == null) return null;
            foreach(var c in children) {
                var childAsT= c as T;
                if(cond(childAsT)) {
                    return childAsT;
                }
            }                
            return null;
        }
        public void AddChild(TreeNode child) {
            if(children == null) children= new List<TreeNode>();
            child.parent= this;
            children.Add(child);
        }
        public void Sort<T>(Func<T,T,int> sortFnc) where T : TreeNode {
            if(children == null) return;
            children.Sort((x,y)=> sortFnc(x as T, y as T));
        }
    }
    
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// Defines the base class for all objects in the library.
    public abstract class LibraryObject : TreeNode {
        // ======================================================================
        // FIELDS
        // ----------------------------------------------------------------------
				Texture	myLibraryIcon  = null;
                string	myDisplayString= null;
        public  Vector2 displaySize    = Vector2.zero;
				float	myRawScore	   = 1f;
				float   myScore        = 1f;
                bool    myIsVisible    = true;

        public  int     searchLength   = 0;
        public  int     stringSortIndex= -1;

        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
        public string rawName       { get { return GetRawName(); }}
		public string nodeName 		{ get { return GetNodeName(); }}
        public string displayString {
            get {
                if(myDisplayString == null) {
                    myDisplayString= GetDisplayString();
                }
                return myDisplayString;
            }
        }
		public Texture libraryIcon {
			get {
				if(myLibraryIcon == null) {
					myLibraryIcon= GetLibraryIcon();
				}
				return myLibraryIcon;
			}
		}
		/// Returns the compounded score for this library object.
		public float score { get { return myScore; }}
		/// Get/Set the raw score for this library object.
		public float rawScore {
			get { return myRawScore; }
			set { myRawScore= value; }
		}

        // ======================================================================
        // FORMATTING HELPERS
        // ----------------------------------------------------------------------
        public string mainValueBegin {
			get { return (EditorGUIUtility.isProSkin ? "<color=cyan><b>" : "<color=blue><b>"); }
        }
        public string mainValueEnd { get { return "</b></color>"; }}
		public string firstPartBegin {
			get { return (EditorGUIUtility.isProSkin ? "<color=lime><i>" : "<color=green><i>");	}
		}
		public string firstPartEnd { get { return "</i></color>"; }}
		public string secondPartBegin {
			get { return (EditorGUIUtility.isProSkin ? "<color=yellow><i>" : "<color=brown><i>"); }
		}
		public string secondPartEnd { get { return "</i></color>"; }}
        
        // ======================================================================
        // INTERFACES
        // ----------------------------------------------------------------------
        internal abstract string 	GetRawName();
		internal abstract string 	GetNodeName();
        internal abstract string	GetDisplayString();
		internal virtual  Texture	GetLibraryIcon() {
			return TextureCache.GetIcon(Icons.kiCanScriptIcon);
		}

        // ======================================================================
        // INIT / SHUTDOWN
        // ----------------------------------------------------------------------
        public LibraryObject() {}
        
        // ======================================================================
        // UTILITIES
        // ----------------------------------------------------------------------
        /// Returns the library object with the given raw name.
        ///
        /// @param rawName The raw name to search for.
        /// @return The library object that matches. _null_ if not found.
        ///
        public T GetChild<T>(string rawName) where T : LibraryObject {
            return GetChild<T>(t=> t.GetRawName() == rawName);
        }
        
        // ----------------------------------------------------------------------
        /// Iterates the entire tree invoking the given action.
        ///
        /// @param fnc The action to invoke for each element in the tree.
        ///
        public void ForEach(Action<LibraryObject> fnc) {
            // -- First execute the action on ourself. --
            fnc(this);
            // -- ...then ask each child to do the same. --
            if(children != null) {
                foreach(var c in children) {
                    var libraryObject= c as LibraryObject;
                    if(libraryObject != null) {
                        libraryObject.ForEach(fnc);
                    }
                }
            }
        }

        // ----------------------------------------------------------------------
        /// Iterates the entire tree invoking the given conditional function.
        ///
        /// @param fnc The conditional fucntion to invoke for each element in the
        ///            tree. If the function return _false_, the iteration will not
        ///            include the children of the current library object.
        ///
        public void ForEach(Func<LibraryObject, bool> fnc) {
            // -- Execute the action on ourself. --
            if(fnc(this) && children != null) {
                foreach(var c in children) {
                    var libraryObject= c as LibraryObject;
                    if(libraryObject != null) {
                        libraryObject.ForEach(fnc);
                    }
                }
            }
        }

        // ----------------------------------------------------------------------
		/// Split the namespace into root and child namespace parts.
		///
		/// @param namespaceName The namespace to split.
		/// @param level1 The root namespace.
		/// @param level2 The child namesapces.
		///
		public static void SplitNamespace(string namespaceName, out string level1, out string level2) {
            level1= "";
            level2= "";
            if(!string.IsNullOrEmpty(namespaceName)) {
                var namespaceLen= namespaceName == null ? 0 : namespaceName.Length;
                var separator= namespaceName.IndexOf('.');
                if(separator >= 0 && separator < namespaceLen) {
                    level1= namespaceName.Substring(0, separator);
                    level2= namespaceName.Substring(separator+1, namespaceLen-separator-1);
                }
                else {
                    level1= namespaceName;
                }                    
            }	
		}

        // ----------------------------------------------------------------------
        /// Takes a snapshot of the string sort index.
        public void TakeSnapshotOfStringSortIndex() {
            if(children == null) return;
            var nbOfChild= children.Count;
            for(int i= 0; i < nbOfChild; ++i) {
                var child= children[i] as LibraryObject;
                child.stringSortIndex= i;
                child.TakeSnapshotOfStringSortIndex();
            }
        }
        
        // ----------------------------------------------------------------------
		/// Resets the score values for this tree branch.
		public void ResetScore() {
			myScore= myRawScore= 1f;
            searchLength= 0;
			if(children == null) return;
			foreach(var child in children) {
				var libraryChild= child as LibraryObject;
				libraryChild.ResetScore();
			}
		}
        // ----------------------------------------------------------------------
		/// Computes the score starting from this node going down to the tree leaf.
		public void ComputeScore(float scoreProduct= 1f, float scoreSum= 0f, int scoreSumLen= 0) {
            // -- Update score components. --
            if(searchLength != 0) {
                scoreProduct*= myRawScore;
                scoreSumLen+= searchLength;
                scoreSum+= myRawScore*searchLength;
            }
			// -- Update our score if we are a member of a type. --
			if(parent is LibraryType) {
                if(scoreSumLen == 0) {
                    myScore= 1f;
                }
                else {
                    myScore= scoreProduct * (scoreSum/scoreSumLen);
                }
				return;
			}
			// -- Ask each child to update their score. --
			myScore= 0f;
            if(children == null) return;
			foreach(var child in children) {
				var libraryChild= child as LibraryObject;
				if(libraryChild != null) {
					libraryChild.ComputeScore(scoreProduct, scoreSum, scoreSumLen);
                    // -- Update our score. --
					var libraryChildScore= libraryChild.score;
					if(libraryChildScore > myScore) {
						myScore= libraryChildScore;
					}
				}
			}
		}
     }
    
}
