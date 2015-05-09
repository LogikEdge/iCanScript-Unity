using UnityEngine;
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
		public float bestSearchScore {
		    get { return database.score; }
		}
        
        // =================================================================================
        // Constants
        // ---------------------------------------------------------------------------------
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
	            if(idx >= siblings.Count) return false;
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
    		bool foldoutState= ShouldUseFoldout(myCursor) ? EditorGUI.Foldout(new Rect(displayArea.x+kScoreWidth, displayArea.y, myFoldOffset, displayArea.height), foldout, "") : false;

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
        /// Toggles the foldout on the selected library object.
        public void ToggleFoldUnfoldOnSelected() {
            if(mySelected == null) return;
            if(!ShouldUseFoldout(mySelected)) return;
            myTreeView.ToggleFoldUnfold(mySelected);
        }
        
        // -------------------------------------------------------------------
        /// Determines if type member should be shown.
        ///
        /// @param libraryObject Library object to test.
        /// @return _true_ if it should be shown. _false_ otherwise.
        ///
        bool ShouldShow(LibraryObject libraryObject) {
			// -- Don't show containers without visible members. --
			if(libraryObject is LibraryRootNamespace || libraryObject is LibraryChildNamespace || libraryObject is LibraryType) {
				if(libraryObject.children == null) return false;
				var showContainer= false;
				foreach(var c in libraryObject.children) {
					if(ShouldShow(c as LibraryObject)) {
						showContainer= true;
						break;
					}
				}
				if(showContainer == false) return false;
			}
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
            // -- Accept kMinScoreFactor of the best search score. --
			if(libraryObject.score < LibraryObject.kMinScoreFactor*bestSearchScore) {
                return false;
            }
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
			// -- Display all visible items if only few remain visible. --
			if(nbItems < 50) {
				OpenAllVisible();
			}
			else if(nbItems < 250) {
				OpenAllVisibleWithScore(database.score);
			}
        }

        // -------------------------------------------------------------------
		/// Computes the member score for the given string.
		public void ComputeMemberScoreFor(string searchString) {
			bool isEmpty= string.IsNullOrEmpty(searchString);
            var searchLength= isEmpty ? 0 : searchString.Length;
			if(!isEmpty) {
				searchString= searchString.ToUpper();
			}
			database.ForEach(
				l=> {
					if(l is LibraryType) return;
					if(l is LibraryChildNamespace) return;
					if(l is LibraryRootNamespace) return;
					if(l is LibraryRoot) return;
					var libraryMember= l as LibraryObject;
					if(isEmpty) {
						libraryMember.rawScore= 1f;
                        libraryMember.searchLength= 0;
					}
					else {
						libraryMember.rawScore= FuzzyString.GetScore(searchString, libraryMember.nodeName.ToUpper());	
                        libraryMember.searchLength= searchLength;
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
            var searchLength= isEmpty ? 0 : searchString.Length;
			if(!isEmpty) {
				searchString= searchString.ToUpper();
			}
			database.ForEach(
				l=> {
					if(l is LibraryType) {
						var libraryType= l as LibraryType;
						if(isEmpty) {
							libraryType.rawScore= 1f;
                            libraryType.searchLength= 0;
						}
						else {
							libraryType.rawScore= FuzzyString.GetScore(searchString, libraryType.nodeName.ToUpper());						
                            libraryType.searchLength= searchLength;
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
            var searchLength= isEmpty ? 0 : searchString.Length;
			if(!isEmpty) {
				searchString= searchString.ToUpper();
			}
			database.ForEach(
				l=> {
					if(l is LibraryChildNamespace) {
						var libraryObject= l as LibraryObject;
						var rootNamespace= libraryObject.parent as LibraryObject;
						if(isEmpty) {
							libraryObject.rawScore= 1f;
                            libraryObject.searchLength= 0;
						}
						else {
						 	var childScore= FuzzyString.GetScore(searchString, libraryObject.nodeName.ToUpper());						
							var rootScore = FuzzyString.GetScore(searchString, rootNamespace.nodeName.ToUpper());
							libraryObject.rawScore= Mathf.Max(childScore, rootScore);
                            libraryObject.searchLength= searchLength;
						}						
					}
				}
			);
			database.ComputeScore();
            database.Sort();
			ComputeNumberOfItems();			
		}
		
        // -------------------------------------------------------------------
        /// Determine if the curent library object requires a foldout.
        ///
        /// @return _true_ if current library object requires a foldout.
        ///         _false_ otherwise.
        ///
        bool ShouldUseFoldout(LibraryObject libraryObject) {
            if(libraryObject == null) return false;
            if(libraryObject is LibraryRootNamespace) return true;
            if(libraryObject is LibraryChildNamespace) return true;
            if(libraryObject is LibraryType) return true;
            return false;
        }

        // -------------------------------------------------------------------
		/// Opens all visible items.
		void OpenAllVisible() {
			database.ForEach(
				l=> {
					if(ShouldShow(l)) {
						var libraryParent= l.parent;
						if(libraryParent != null) {
							myTreeView.Unfold(libraryParent);							
						}
					}
				}
			);
		}

        // -------------------------------------------------------------------
		/// Opens all visible items with the best search score.
		void OpenAllVisibleWithScore(float score) {
			database.ForEach(
				l=> {
					if(ShouldShow(l) && Math3D.IsEqual(score, l.score)) {
						var libraryParent= l.parent;
						if(libraryParent != null) {
							myTreeView.Unfold(libraryParent);							
						}
					}
				}
			);
		}
    }
    
}
