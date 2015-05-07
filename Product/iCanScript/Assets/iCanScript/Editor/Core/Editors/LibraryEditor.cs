using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using iCanScript.Engine;

namespace iCanScript.Editor {
    public class LibraryEditor : iCS_EditorBase {

        // =================================================================================
        // Constants
        // ---------------------------------------------------------------------------------
        const int   kIconWidth  = 16;
        const int   kIconHeight = 16;
        const float kLabelSpacer= 4f;

        // =================================================================================
        // Fields
        // ---------------------------------------------------------------------------------
    	LibraryDisplayController    myController;
        DSScrollView                myMainView;
        Rect                        myScrollViewArea;
    	Rect                        mySelectedAreaCache    = new Rect(0,0,0,0);
        bool                        myShowInherited        = true;
        bool                        myShowProtected        = false;
        string                      myNamespaceSearchString= "";
        string                      myTypeSearchString     = "";
        string                      myMemberSearchString   = "";

        // =================================================================================
        // PROPERTIES
        // ---------------------------------------------------------------------------------
		public int numberOfItems {
			get { return myController.numberOfItems; }
		}
		public bool showInherited {
			get { return myController.showInherited; }
			set { myController.showInherited= value; }
		}
		public bool showProtected {
			get { return myController.showProtected; }
			set { myController.showProtected= value; }
		}
		public string memberSearchString {
			get { return myMemberSearchString; }
			set { UpdateMemberSearchString(value); }
		}
		public string typeSearchString {
			get { return myTypeSearchString; }
			set { UpdateTypeSearchString(value); }
		}
		public string namespaceSearchString {
			get { return myNamespaceSearchString; }
			set { UpdateNamespaceSearchString(value); }
		}
		
        // =================================================================================
        // Activation/Deactivation.
        // ---------------------------------------------------------------------------------
        static void Init() {
            var editor= EditorWindow.CreateInstance<LibraryEditor>();
            editor.ShowUtility();
        }

        // ---------------------------------------------------------------------------------
        /// Assure a minimum size for the library window.
        public new void OnEnable() {
    		base.OnEnable();
    		minSize= new Vector2(270f, 270f);
        }

        // ---------------------------------------------------------------------------------
        /// Determines if the environement is properly initialize before we can start to
        /// use the library editor.
        ///
        /// @return _true_ if library window is ready. _false_ otherwise.
        ///
        bool IsInitialized() {
            if(myController == null || myMainView == null) {
                myController= new LibraryDisplayController();
                myMainView= new DSScrollView(new RectOffset(0,0,0,0), false, true, true, myController.View);                    
            }
            return true;
        }
	
    
    	// =================================================================================
        // Display.
        // ---------------------------------------------------------------------------------
        /// Displays the editor window and processes the user events.
        public new void OnGUI() {
//            // Wait until mouse is over our window.
//            if(mouseOverWindow != this) return;
    		if(!IsInitialized()) return;

            // -- Draw the base stuff for all windows --
            base.OnGUI();
    
            // -- Show toolbar --
    		var toolbarRect= ShowToolbar();
            myScrollViewArea= new Rect(0, toolbarRect.height, position.width, position.height-toolbarRect.height);
    		myMainView.Display(myScrollViewArea);
    		ProcessEvents(myScrollViewArea);
//    		// -- Make new selection visible --
//    		if(mySelectedAreaCache != myController.SelectedArea) {
//    		    mySelectedAreaCache= myController.SelectedArea;
//    		    myMainView.MakeVisible(mySelectedAreaCache, myScrollViewArea);
//    		}          
    	}
        // ---------------------------------------------------------------------------------
    	Rect ShowToolbar() {
			var r= new Rect(0, 0, position.width, 80);
			GUI.Box(r, "", EditorStyles.toolbar);
			
            // -- Build a 4 level toolbar --
    		var line1Rect= iCS_ToolbarUtility.BuildToolbar(position.width);
            var line2Rect= new Rect(line1Rect.x, line1Rect.yMax, line1Rect.width, line1Rect.height);
            var line3Rect= new Rect(line1Rect.x, line2Rect.yMax, line1Rect.width, line1Rect.height);
            var line4Rect= new Rect(line1Rect.x, line3Rect.yMax, line1Rect.width, line1Rect.height);
            iCS_ToolbarUtility.BuildToolbar(line2Rect);
            iCS_ToolbarUtility.BuildToolbar(line3Rect);
            iCS_ToolbarUtility.BuildToolbar(line4Rect);
            //  -- Add display options -- 
            iCS_ToolbarUtility.MiniLabel(ref line1Rect, "Show:", 0, 0);
            iCS_ToolbarUtility.Separator(ref line1Rect);
            showInherited= iCS_ToolbarUtility.Toggle(ref line1Rect, showInherited, 10, 0);
            iCS_ToolbarUtility.MiniLabel(ref line1Rect, "Inherited", 0, 0);
            iCS_ToolbarUtility.Separator(ref line1Rect);
            showProtected= iCS_ToolbarUtility.Toggle(ref line1Rect, showProtected, 10, 0);
            iCS_ToolbarUtility.MiniLabel(ref line1Rect, "Protected", 0, 0);
	        iCS_ToolbarUtility.MiniLabel(ref line1Rect, "# items: "+numberOfItems.ToString(), 0, 0, true);
			// -- Add search fields --
            iCS_ToolbarUtility.MiniLabel(ref line2Rect, "Search: ", 0, 0);
            iCS_ToolbarUtility.Separator(ref line2Rect);
    		this.namespaceSearchString= iCS_ToolbarUtility.Search(ref line2Rect, 120.0f, this.namespaceSearchString, 0, 0, true);
    		this.typeSearchString= iCS_ToolbarUtility.Search(ref line3Rect, 120.0f, this.typeSearchString, 0, 0, true);
    		this.memberSearchString= iCS_ToolbarUtility.Search(ref line4Rect, 120.0f, this.memberSearchString, 0, 0, true);
            iCS_ToolbarUtility.MiniLabel(ref line2Rect, "Namespace", 0, 20, true);
            iCS_ToolbarUtility.MiniLabel(ref line3Rect, "Type", 0, 20, true);
            iCS_ToolbarUtility.MiniLabel(ref line4Rect, "Field/Property/Function", 0, 20, true);
    		return Math3D.Union(line1Rect, line4Rect);
    	}
    	// =================================================================================
        // Event processing
        // ---------------------------------------------------------------------------------
        void ProcessEvents(Rect frameArea) {
         	Vector2 mousePosition= Event.current.mousePosition;
            var selected= myController.Selected;
    		switch(Event.current.type) {
                case EventType.MouseDrag: {
                    switch(Event.current.button) {
                        case 0: { // Left mouse button
                            StartDragAndDrop(selected);                            
                            Event.current.Use();
                            break;
                        }
                    }
                    break;
                }
//                case EventType.ScrollWheel: {
//                    break;
//                }
//                case EventType.MouseDown: {
//                    var mouseInScreenPoint= GUIUtility.GUIToScreenPoint(mousePosition);
//                    var areaInScreenPoint= GUIUtility.GUIToScreenPoint(new Vector2(frameArea.x, frameArea.y));
//                    var areaInScreenPosition= new Rect(areaInScreenPoint.x, areaInScreenPoint.y, frameArea.width, frameArea.height);
//                    myController.MouseDownOn(null, mouseInScreenPoint, areaInScreenPosition);
//                    Event.current.Use();
//                    // Move keyboard focus to this window.			
//                    GUI.FocusControl("");   // Removes focus from the search field.
//    				break;
//    			}
//
//                case EventType.MouseUp: {
//    				break;
//    			}
//    			case EventType.KeyDown: {
//    				var ev= Event.current;
//    				if(!ev.isKey) break;
//                    switch(ev.keyCode) {
//                        // Tree navigation
//                        case KeyCode.UpArrow: {
//                            myController.SelectPrevious();
//                            ev.Use();
//                            break;
//                        }
//                        case KeyCode.DownArrow: {
//                            myController.SelectNext();
//                            ev.Use();
//                            break;
//                        }
//                        // Fold/Unfold toggle
//                        case KeyCode.Return: {
//                            myController.ToggleFoldUnfoldSelected();
//                            ev.Use();
//                            break;
//                        }
//                    }
//                    switch(ev.character) {
//                        // Fold/Unfold.
//                        case '+': {
//                            myController.UnfoldSelected();
//                            ev.Use();
//                            break;
//                        }
//                        case '-': {
//                            myController.FoldSelected();
//                            ev.Use();
//                            break;
//                        }
//    					case 'h': {
//                            myController.Help();
//                            ev.Use();
//                            break;
//    					}
//                    }
//                    break;
//    			}
            }   
        }

    	// =================================================================================
        // Search functionality.
        // ---------------------------------------------------------------------------------
		/// Updates member search string and ask controller to adjust it filter.
		void UpdateMemberSearchString(string newValue) {
			if(newValue == myMemberSearchString) return;
			myMemberSearchString= newValue;
			myController.ComputeMemberScoreFor(myMemberSearchString);
		}
        // ---------------------------------------------------------------------------------
		/// Updates type search string and ask controller to adjust it filter.
		void UpdateTypeSearchString(string newValue) {
			if(newValue == myTypeSearchString) return;
			myTypeSearchString= newValue;
			myController.ComputeTypeScoreFor(myTypeSearchString);
		}
        // ---------------------------------------------------------------------------------
		/// Updates namespace search string and ask controller to adjust it filter.
		void UpdateNamespaceSearchString(string newValue) {
			if(newValue == myNamespaceSearchString) return;
			myNamespaceSearchString= newValue;
			myController.ComputeNamespaceScoreFor(myNamespaceSearchString);
		}
		
    	// =================================================================================
        // Drag events.
        // ---------------------------------------------------------------------------------
		/// Creates a drag and drop object with a node created from the selecct library
		/// object.
		///
		/// @param libraryObject The user selected library object.
		/// 
        void StartDragAndDrop(LibraryObject libraryObject) {
    		// -- Just return if nothing is selected. --
            if(libraryObject == null) return;
    		// -- Don't allow to drag & drop for libary root and namespace. --
			if(libraryObject is LibraryRoot) return;
			if(libraryObject is LibraryRootNamespace) return;
			if(libraryObject is LibraryChildNamespace) return;
            // -- Build drag object. --
            GameObject go= new GameObject(libraryObject.nodeName);
            go.hideFlags = HideFlags.HideAndDontSave;
            var visualScriptRoot= iCS_DynamicCall.AddLibrary(go);
            iCS_IStorage iStorage= new iCS_IStorage(visualScriptRoot);
            if(iStorage == null) {
                Debug.LogWarning("iCanScript: Cannot create iStorage. Contact support.");
            }
			// -- Create node for each known library type. --
            if(CreateInstance(libraryObject, iStorage)) {
    			// -- Commit changes to drag & drop storage. --
                iStorage.SaveStorage();
                // -- Complete the drag information. --
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences= new UnityEngine.Object[1]{go};
                DragAndDrop.StartDrag(libraryObject.nodeName);                
            }
            // -- Release temporary game object in 60 seconds. --
            iCS_AutoReleasePool.AutoRelease(go, 60f);
        }

        // ---------------------------------------------------------------------------------
		/// Creates a visual script node from the given library object.
		///
		/// @param libraryObject The library object from which to create a visual node.
		/// @param iStorage The storage in which to put the created node.
		/// @return _true_ if instance was created. _false_ otherwise.
		/// 
        bool CreateInstance(LibraryObject libraryObject, iCS_IStorage iStorage) {
			if(libraryObject is LibraryType) {
				var libraryType= libraryObject as LibraryType;
	            iStorage.CreatePropertyWizardNode(-1, libraryType.type);
				return true;
			}
			var node= iStorage.CreateNode(-1, libraryObject);
			return node != null;
        }
    }
    
}
