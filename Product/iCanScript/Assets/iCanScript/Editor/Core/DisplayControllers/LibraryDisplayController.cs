using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using iCanScript.Internal.FuzzyLogic;

namespace iCanScript.Internal.Editor {
    using Prefs= PreferencesController;

    public class LibraryDisplayController : DSTreeViewDataSource {
        // =================================================================================
        // Constants
        // ---------------------------------------------------------------------------------
        const float kScoreWidth= 32f;
        
        // =================================================================================
        // Fields
        // ---------------------------------------------------------------------------------
    	DSTreeView      myTreeView       = null;
        LibraryObject   myCursor         = null;
    	float           myFoldOffset     = 16f;
        LibraryObject   mySelected       = null;
        GUIStyle        myLabelStyle     = null;
        
        // =================================================================================
        // Properties
        // ---------------------------------------------------------------------------------
    	public DSView   	 	View		 { get { return myTreeView; }}
		public LibraryObject	Selected	 { get { return mySelected; }}
		public int numberOfVisibleNamespaces { get { return database.numberOfVisibleNamespaces; }}
		public int numberOfVisibleTypes      { get { return database.numberOfVisibleTypes; }}
		public int numberOfVisibleMembers    { get { return database.numberOfVisibleMembers; }}
		public LibraryRoot database {
			get { return LibraryController.LibraryDatabase; }
		}
		public bool showInheritedMembers {
			get { return database.showInheritedMembers; }
			set {
                if(database.showInheritedMembers != value) {
                    Prefs.LibraryInheritedOption= value;
                    database.showInheritedMembers= value;
                    ShowVisible();                
                }
            }
		}
		public bool showProtectedMembers {
			get { return database.showProtectedMembers; }
			set {
                if(database.showProtectedMembers != value) {
                    Prefs.LibraryProtectedOption= value;
                    database.showProtectedMembers= value;
                    ShowVisible();
                }
            }
		}
		public bool showUnityEditorMembers {
			get { return database.showUnityEditorMembers; }
			set {
                if(database.showUnityEditorMembers != value) {
                    Prefs.LibraryUnityEditorOption= value;
                    database.showUnityEditorMembers= value;
                    ShowVisible();
                }
            }
		}
		public string namespaceFilter {
			get { return database.namespaceFilter; }
			set {
                if(database.namespaceFilter != value) {
                    database.namespaceFilter= value;
                    ShowVisible();
                }
            }
		}
		public string typeFilter {
			get { return database.typeFilter; }
			set {
                if(database.typeFilter != value) {
                    database.typeFilter= value;
                    ShowVisible();
                }
            }
		}
		public string memberFilter {
			get { return database.memberFilter; }
			set {
                if(database.memberFilter != value) {
                    database.memberFilter= value;
                    ShowVisible();
                }
            }
		}
        public string displayString {
            get {
                if(string.IsNullOrEmpty(myCursor.displayString)) {
                    if(myCursor is LibraryRootNamespace) {
                        return "-- no namespace --";
                    }
                    return "-- no child namespace --";
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
            // -- Initialize the cursor --
            Reset();
            // -- Load the user options. --
            showInheritedMembers= Prefs.LibraryInheritedOption;
            showProtectedMembers= Prefs.LibraryProtectedOption;
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
				size.x+= myFoldOffset+kIconWidth+kLabelSpacer+kScoreWidth;
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
            return libraryObject.isVisible;
        }
        
        // -------------------------------------------------------------------
        /// Determine what should be shown.
        void ShowVisible() {
			// -- Display all visible items if only few remain visible. --
			if(numberOfVisibleMembers < 50) {
				UnfoldAllVisible();
			}
			else if(numberOfVisibleMembers < 250) {
				UnfoldAllVisibleWithScore(database.score);
			}
            else if(numberOfVisibleNamespaces < 20) {
                UnfoldVisibleNamespaces();
                if(numberOfVisibleTypes < 20) {
                    UnfoldVisibleTypes();
                }
            }
            else {
                FoldAllVisible();
            }
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
		/// Folds all visible items.
		void FoldAllVisible() {
			database.ForEach(
				l=> {
					if(ShouldShow(l)) {
                        if(!(l is LibraryTypeMember)) {
							myTreeView.Fold(l);							
                        }
					}
				}
			);
		}

        // -------------------------------------------------------------------
		/// Unfolds all visible items.
		void UnfoldAllVisible() {
			database.ForEach(
				l=> {
					if(ShouldShow(l)) {
                        if(!(l is LibraryTypeMember)) {
							myTreeView.Unfold(l);							
                        }
					}
				}
			);
		}

        // -------------------------------------------------------------------
		/// Unfolds all visible items.
		void UnfoldVisibleNamespaces() {
			database.ForEach(
				l=> {
					if(ShouldShow(l)) {
                        if(l is LibraryRootNamespace) {
							myTreeView.Unfold(l);							
                        }
                        else {
                            myTreeView.Fold(l);
                        }
					}
				}
			);
		}

        // -------------------------------------------------------------------
		/// Unfolds all visible items.
		void UnfoldVisibleTypes() {
			database.ForEach(
				l=> {
					if(ShouldShow(l)) {
                        if(l is LibraryChildNamespace) {
							myTreeView.Unfold(l);							
                        }
					}
				}
			);
		}
        // -------------------------------------------------------------------
		/// Unfolds all visible items with the best search score.
		void UnfoldAllVisibleWithScore(float score) {
			database.ForEach(
				l=> {
					if(ShouldShow(l)) {
                        if(!(l is LibraryTypeMember)) {
                            if(Math3D.IsEqual(score, l.score)) {
        						myTreeView.Unfold(l);
        					}
                            else {
                                myTreeView.Fold(l);
                            }                            
                        }
					}
				}
			);
		}
    }
    
}
