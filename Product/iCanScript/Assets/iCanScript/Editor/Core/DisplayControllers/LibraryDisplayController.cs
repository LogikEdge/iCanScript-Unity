﻿using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using iCanScript.FuzzyLogic;

namespace iCanScript.Editor {

    public class LibraryDisplayController : DSTreeViewDataSource {
        // =================================================================================
        // Fields
        // ---------------------------------------------------------------------------------
    	DSTreeView      myTreeView       = null;
        LibraryObject   myCursor         = null;
    	float           myFoldOffset     = 16f;
        LibraryObject   mySelected       = null;
        GUIStyle        myLabelStyle     = null;
		int				myNumberOfItems  = 0;
        bool            myShowInherited  = true;
		bool			myShowProtected  = false;
        
        // =================================================================================
        // Properties
        // ---------------------------------------------------------------------------------
    	public DSView   	 	View		{ get { return myTreeView; }}
		public LibraryObject	Selected	{ get { return mySelected; }}
        public string displayString {
            get {
                if(string.IsNullOrEmpty(myCursor.displayString)) {
                    return "<empty>";
                }
                return myCursor.displayString;
            }
        }
		public Texture libraryIcon {
			get { return myCursor.libraryIcon; }
		}
        public GUIStyle labelStyle {
            get {
                if(myLabelStyle == null) {
                    myLabelStyle= new GUIStyle(EditorStyles.label);
                    myLabelStyle.richText= true;
                }
                return myLabelStyle;
            }
        }
		public bool showInherited {
			get { return myShowInherited; }
			set {
				if(value != myShowInherited) {
					myShowInherited= value;
					ComputeNumberOfItems();
				}
			}
		}
		public bool showProtected {
			get { return myShowProtected; }
			set {
				if(value != myShowProtected) {
					myShowProtected= value;
					ComputeNumberOfItems();					
				}
			}
		}
		public int numberOfItems {
			get { return myNumberOfItems; }
			set { myNumberOfItems= value; }
		}
		public LibraryRoot database {
			get { return LibraryController.LibraryDatabase; }
		}
		
        // =================================================================================
        // Constants
        // ---------------------------------------------------------------------------------
		const float kMinScore= 0.5f;
        const int   kIconWidth  = 16;
        const int   kIconHeight = 16; 
        const float kLabelSpacer= 4f;
    
        // =================================================================================
        // Initialization
        // ---------------------------------------------------------------------------------
        /// Builds and iniotializes the library display controller.
    	public LibraryDisplayController() {
            // -- Initialize panel. --
    		myTreeView = new DSTreeView(new RectOffset(0,0,0,0), false, this, 16, 2);
			// -- Compute # of items --
			ComputeNumberOfItems();
            // -- Initialize the cursor --
            Reset();
    	}

        // ===================================================================
        // Tree View Data Source Extensions
        // -------------------------------------------------------------------
        /// Initializes the cursor position.
    	public void Reset() {
    	    myCursor= database;
    	}
    	public void BeginDisplay() {
            // -- Initialize some display constants. --
            if(myFoldOffset == 0) {
                myFoldOffset= EditorStyles.foldout.CalcSize(new GUIContent("")).x;
            }
    	}
    	public void EndDisplay() {}
        // -------------------------------------------------------------------
        /// Moves the cursor to the next element in the tree.
        ///
        /// @return _true_ if a next element exists. _false_ otherwise.
        ///
    	public bool MoveToNext() {
    		if(MoveToFirstChild()) return true;
    		if(MoveToNextSibling()) return true;
    		do {
    			if(!MoveToParent()) return false;			
    		} while(!MoveToNextSibling());
    		return true;
    	}
        // -------------------------------------------------------------------
        /// Moves the cursor to the next sibling under the same parent.
        ///
        /// @return _true_ if a next sibling exists.  _false_ otherwise.
        ///
    	public bool MoveToNextSibling() {
    	    var parent= myCursor.parent;
            if(parent == null) return false;
            var siblings= parent.children;
            if(siblings == null) return false;
            int idx= siblings.IndexOf(myCursor);
			if(idx < 0) return false;
			do {
	            if(idx >= siblings.Count-1) return false;
	            myCursor= siblings[idx+1] as LibraryObject;
				++idx;				
			} while(!ShouldShow(myCursor));
            return true;
    	}
        // -------------------------------------------------------------------
        /// Moves the cursor to the first child.
        ///
        /// @return _true_ if the cursor was moved. _false_ otherwise.
        ///
    	public bool MoveToFirstChild() {
            var siblings= myCursor.children;
            if(siblings == null || siblings.Count == 0) return false;
			int idx= 0;
			do {
	            if(idx >= siblings.Count-1) return false;
	            myCursor= siblings[idx] as LibraryObject;
				++idx;				
			} while(!ShouldShow(myCursor));
            return true;
    	}
        // -------------------------------------------------------------------
        /// Moves the cursor to the parent object.
        ///
        /// @return _true_ if the cursor was moved. _false_ otherwise.
        ///
    	public bool MoveToParent() {
    	    var parent= myCursor.parent;
            if(parent == null) return false;
            myCursor= parent as LibraryObject;
            return true;
    	}
        // -------------------------------------------------------------------
        /// Returns the area needed to display the library content.
        ///
        /// @return The area needed to display the libary content.
        ///
    	public Vector2 CurrentObjectLayoutSize() {
            var size= myCursor.displaySize;
            if(size == Vector2.zero) {
                size= labelStyle.CalcSize(new GUIContent(displayString));
				size.x+= myFoldOffset+kIconWidth+kLabelSpacer;
            }
    	    return size;
    	}
        // -------------------------------------------------------------------
        /// Displays the object pointed by the cursor.
        ///
        /// @param displayArea The area in which to display the object.
        /// @param foldout The current foldout state.
        /// @param frameArea The total area in which the display takes place.
        /// @return The new foldout state.
        ///
    	public bool DisplayCurrentObject(Rect displayArea, bool foldout, Rect frameArea) {
            // Show selected outline.
            GUIStyle labelStyle= this.labelStyle;
    		if(mySelected == myCursor) {
                Color selectionColor= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).settings.selectionColor;
                iCS_Graphics.DrawBox(frameArea, selectionColor, selectionColor, new Color(1.0f, 1.0f, 1.0f, 0.65f));
                labelStyle= EditorStyles.whiteLabel;
                labelStyle.richText= true;
    		}
            
			// -- Show search score --
			int score= (int)(myCursor.score * 100f);
			float kScoreWidth= 32f;
			var scoreRect= new Rect(displayArea.x, displayArea.y, kScoreWidth, displayArea.height);
			GUI.Label(scoreRect, score.ToString()+"%");
			
            // -- Show foldout (if needed) --
    		bool foldoutState= ShouldUseFoldout ? EditorGUI.Foldout(new Rect(displayArea.x+kScoreWidth, displayArea.y, myFoldOffset, displayArea.height), foldout, "") : false;

			// -- Show icon --
	        var pos= new Rect(myFoldOffset+kScoreWidth+displayArea.x, displayArea.y, displayArea.width-myFoldOffset, displayArea.height);
		    GUI.Label(pos, libraryIcon);
	        pos= new Rect(pos.x+kIconWidth+kLabelSpacer, pos.y-1f, pos.width-(kIconWidth+kLabelSpacer), pos.height);  // Move label up a bit.

            // -- Display string to user --
            displayArea.x= displayArea.x+myFoldOffset;
            GUI.Label(pos, displayString, labelStyle);
    	    return foldoutState;
    	}
        // -------------------------------------------------------------------
        /// Returns a uniqu key for the item under cursor.
        ///
        /// @return The cursor is returned.
        ///
    	public object CurrentObjectKey() {
    	    return myCursor;
    	}
        // -------------------------------------------------------------------
        /// Advises that a mouse down was performed on the given key.
        ///
        /// @param key The library object on which a mouse down occured.
        /// @param mouseInScreenPoint The position of the mouse.
        /// @param screenArea The screen area used to display the library object.
        ///
    	public void MouseDownOn(object key, Vector2 mouseInScreenPoint, Rect screenArea) {
            if(key == null) {
                return;
            }
            mySelected= key as LibraryObject;
    	}

        // -------------------------------------------------------------------
        /// Determines if type member should be shown.
        ///
        /// @param libraryObject Library object to test.
        /// @return _true_ if it should be shown. _false_ otherwise.
        ///
        bool ShouldShow(LibraryObject libraryObject) {
			var libraryMemberInfo= libraryObject as LibraryMemberInfo;
			if(libraryMemberInfo != null) {
				// -- Should we show inherited members? --
	            if(myShowInherited == false) {
	                if(libraryMemberInfo.isInherited) {
	            		return false;
	            	}
				}
				// -- Should we show protected members? --
				if(myShowProtected == false) {
					var methodBase= libraryMemberInfo.memberInfo as MethodBase;
					if(methodBase != null && methodBase.IsFamily) {
						return false;
					}
				}				
			}
			if(libraryObject.score < kMinScore) return false;
			return true;
        }
        
        // -------------------------------------------------------------------
        /// Determines the number of items to show.
        void ComputeNumberOfItems() {
			int nbItems= 0;
			database.ForEach(
				l=> {
					var libraryMemberInfo= l as LibraryMemberInfo;
					if(libraryMemberInfo != null && ShouldShow(libraryMemberInfo)) {
						++nbItems;
					}
				}
			);
			this.numberOfItems= nbItems;
        }

        // -------------------------------------------------------------------
		/// Computes the member score for the given string.
		public void ComputeMemberScoreFor(string searchString) {
			bool isEmpty= string.IsNullOrEmpty(searchString);
			database.ForEach(
				l=> {
					if(l is LibraryType) return;
					if(l is LibraryChildNamespace) return;
					if(l is LibraryRootNamespace) return;
					if(l is LibraryRoot) return;
					var libraryMember= l as LibraryObject;
					if(isEmpty) {
						libraryMember.rawScore= 1f;
					}
					else {
						libraryMember.rawScore= FuzzyString.GetScore(searchString, libraryMember.nodeName);						
					}
				}
			);
			database.ComputeScore();
            database.Sort();
			ComputeNumberOfItems();			
		}
        // -------------------------------------------------------------------
		/// Computes the type score for the given string.
		public void ComputeTypeScoreFor(string searchString) {
			bool isEmpty= string.IsNullOrEmpty(searchString);
			database.ForEach(
				l=> {
					if(l is LibraryType) {
						var libraryType= l as LibraryType;
						if(isEmpty) {
							libraryType.rawScore= 1f;
						}
						else {
							libraryType.rawScore= FuzzyString.GetScore(searchString, libraryType.nodeName);						
						}						
					}
				}
			);
			database.ComputeScore();
            database.Sort();
			ComputeNumberOfItems();			
		}
        // -------------------------------------------------------------------
		/// Computes the type score for the given string.
		public void ComputeNamespaceScoreFor(string searchString) {
			bool isEmpty= string.IsNullOrEmpty(searchString);
			database.ForEach(
				l=> {
					if(l is LibraryChildNamespace) {
						var libraryObject= l as LibraryObject;
						var rootNamespace= libraryObject.parent as LibraryObject;
						if(isEmpty) {
							libraryObject.rawScore= 1f;
						}
						else {
						 	var childScore= FuzzyString.GetScore(searchString, libraryObject.nodeName);						
							var rootScore = FuzzyString.GetScore(searchString, rootNamespace.nodeName);
							libraryObject.rawScore= Mathf.Max(childScore, rootScore);
						}						
					}
				}
			);
			database.ComputeScore();
            database.Sort();
			ComputeNumberOfItems();			
		}
		
        // ===================================================================
        // -------------------------------------------------------------------
        bool ShouldUseFoldout {
            get {
                if(myCursor == null) return false;
                if(myCursor is LibraryRootNamespace) return true;
                if(myCursor is LibraryChildNamespace) return true;
                if(myCursor is LibraryType) return true;
                return false;
            }
        }

    }
    
}
