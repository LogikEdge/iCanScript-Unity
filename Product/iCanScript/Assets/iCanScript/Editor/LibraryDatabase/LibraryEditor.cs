using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

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
        string                      myNamespaceSearchString= null;
        string                      myTypeSearchString     = null;
        string                      myMemberSearchString   = null;

        // =================================================================================
        // Activation/Deactivation.
        // ---------------------------------------------------------------------------------
    	[MenuItem("iCanScript/Editors/Special Editor",false,910)]
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
            // -- Display toolbar header --
    		var line1Rect= iCS_ToolbarUtility.BuildToolbar(position.width);
            var line2Rect= new Rect(line1Rect.x, line1Rect.yMax, line1Rect.width, line1Rect.height);
            var line3Rect= new Rect(line1Rect.x, line2Rect.yMax, line1Rect.width, line1Rect.height);
            // Display # of items found
            var newShowInherited= iCS_ToolbarUtility.Toggle(ref line1Rect, myShowInherited, 0, 0);
            if(newShowInherited != myShowInherited) {
                // TODO: change controller ShowInherited.
                myShowInherited= newShowInherited;                
            }
            iCS_ToolbarUtility.MiniLabel(ref line1Rect, "Show Inherited", 0, 0);
            var newShowProtected= iCS_ToolbarUtility.Toggle(ref line1Rect, myShowProtected, 0, 0);
            if(newShowProtected != myShowProtected) {
                // TODO: change controller ShowProtected.
                myShowProtected= newShowProtected;                
            }
            iCS_ToolbarUtility.MiniLabel(ref line1Rect, "Show Protected", 0, 0);
//            var numberOfItems= myController.NumberOfItems;
//            iCS_ToolbarUtility.MiniLabel(ref line3Rect, "# items: "+numberOfItems.ToString(), 0, 0, true);
//
//            // -- Display toolbar search field #1 --
//            var search= myController.SearchCriteria_1;
//            iCS_ToolbarUtility.BuildToolbar(search1Rect);
//            search.ShowClasses= iCS_ToolbarUtility.Toggle(ref search1Rect, search.ShowClasses, 0, 0);
//    		var icon= iCS_Icons.GetLibraryNodeIconFor(iCS_DefaultNodeIcons.ObjectInstance);
//            iCS_ToolbarUtility.Texture(ref search1Rect, icon, 0, 4);            
//            iCS_ToolbarUtility.Separator(ref search1Rect);
//            iCS_ToolbarUtility.Separator(ref search1Rect);
//
//            search.ShowFunctions= iCS_ToolbarUtility.Toggle(ref search1Rect, search.ShowFunctions, kLabelSpacer, 0);
//            icon= iCS_Icons.GetLibraryNodeIconFor(iCS_DefaultNodeIcons.Function);            
//            iCS_ToolbarUtility.Texture(ref search1Rect, icon, 0, kLabelSpacer);            
//            iCS_ToolbarUtility.Separator(ref search1Rect);
//
//            search.ShowVariables= iCS_ToolbarUtility.Toggle(ref search1Rect, search.ShowVariables, kLabelSpacer, 0);
//            icon= iCS_BuiltinTextures.OutEndPortIcon;
//            iCS_ToolbarUtility.Texture(ref search1Rect, icon, 0, 0);            
//            icon= iCS_BuiltinTextures.InEndPortIcon;
//            iCS_ToolbarUtility.Texture(ref search1Rect, icon, 0, kLabelSpacer);            
//            iCS_ToolbarUtility.Separator(ref search1Rect);
//
//    		string searchString= myController.SearchString ?? "";
//    		myController.SearchString= iCS_ToolbarUtility.Search(ref search1Rect, 120.0f, searchString, 0, 0, true);
    		return Math3D.Union(line1Rect, line3Rect);
    	}
    	// =================================================================================
        // Event processing
        // ---------------------------------------------------------------------------------
        void ProcessEvents(Rect frameArea) {
//         	Vector2 mousePosition= Event.current.mousePosition;
//            var selected= myController.Selected;
//    		switch(Event.current.type) {
//                case EventType.MouseDrag: {
//                    switch(Event.current.button) {
//                        case 0: { // Left mouse button
//                            StartDragAndDrop(selected);                            
//                            Event.current.Use();
//                            break;
//                        }
//                    }
//                    break;
//                }
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
//            }   
        }

//    	// =================================================================================
//        // Drag events.
//        // ---------------------------------------------------------------------------------
//        void StartDragAndDrop(iCS_LibraryController.Node node) {
//    		// Just return if nothing is selected.
//            if(node == null) return;
//    		// Don't allow to drag & drop company, library, and packages
//    		switch(node.Type) {
//    			case iCS_LibraryController.NodeTypeEnum.Root:
//    			case iCS_LibraryController.NodeTypeEnum.Company:
//    			case iCS_LibraryController.NodeTypeEnum.Library:
//    			case iCS_LibraryController.NodeTypeEnum.Package:
//    				return;
//    			default:
//    				break;
//    		}
//            // Build drag object.
//            GameObject go= new GameObject(node.Name);
//            go.hideFlags = HideFlags.HideAndDontSave;
//    //        var library= go.AddComponent("iCS_Library") as iCS_LibraryImp;
//            var library= iCS_DynamicCall.AddLibrary(go);
//            iCS_IStorage iStorage= new iCS_IStorage(library);
//            if(iStorage == null) {
//                Debug.LogWarning("iCanScript: Cannot create iStorage.");
//            }
//            CreateInstance(node, iStorage);
//            iStorage.SaveStorage();
//            // Fill drag info.
//            DragAndDrop.PrepareStartDrag();
//            DragAndDrop.objectReferences= new UnityEngine.Object[1]{go};
//            DragAndDrop.StartDrag(node.Name);
//            iCS_AutoReleasePool.AutoRelease(go, 60f);
//        }
//        // ---------------------------------------------------------------------------------
//        void CreateInstance(iCS_LibraryController.Node node, iCS_IStorage iStorage) {
//            if(node.Type == iCS_LibraryController.NodeTypeEnum.Company) {
//                CreatePackage(node.Name, iStorage);        
//                return;
//            }
//            if(node.Type == iCS_LibraryController.NodeTypeEnum.Library) {
//                CreatePackage(node.Name, iStorage);        
//                return;
//            }
//            if(node.Type == iCS_LibraryController.NodeTypeEnum.Class) {
//                CreateObjectInstance(node.MemberInfo.ClassType, iStorage);        
//                return;
//            }
//            if(node.Type == iCS_LibraryController.NodeTypeEnum.Field) {
//                CreateMethod(node.MemberInfo, iStorage);        
//                return;
//            }
//            if(node.Type == iCS_LibraryController.NodeTypeEnum.Property) {
//                CreateMethod(node.MemberInfo, iStorage);        
//                return;
//            }
//            if(node.Type == iCS_LibraryController.NodeTypeEnum.Constructor) {
//                CreateMethod(node.MemberInfo, iStorage);        
//                return;
//            }
//            if(node.Type == iCS_LibraryController.NodeTypeEnum.Method) {
//                CreateMethod(node.MemberInfo, iStorage);        
//                return;
//            }
//    		if(node.Type == iCS_LibraryController.NodeTypeEnum.Message) {
//                var module= CreateMessage(node.MemberInfo, iStorage);        
//    			if(node.MemberInfo.IconPath != null) {
//    				module.IconPath= node.MemberInfo.IconPath;				
//    			}
//    			return;
//    		}
//		
//        }
//	
//        // ======================================================================
//        // Creation Utilities
//        // ---------------------------------------------------------------------------------
//        // FIXME: Should pass along the object type to the module instead of multiple creation.
//        iCS_EditorObject CreatePackage(string name, iCS_IStorage iStorage) {
//            return iStorage.CreatePackage(-1, name);
//        }
//        // ---------------------------------------------------------------------------------
//        iCS_EditorObject CreateObjectInstance(Type classType, iCS_IStorage iStorage) {
//            return iStorage.CreateObjectInstance(-1, null, classType);
//        }
//        // ---------------------------------------------------------------------------------
//        iCS_EditorObject CreateMethod(iCS_MemberInfo desc, iCS_IStorage iStorage) {
//            return iStorage.CreateFunction(-1, desc.ToFunctionPrototypeInfo);            
//        }    
//        // ---------------------------------------------------------------------------------
//        iCS_EditorObject CreateMessage(iCS_MemberInfo desc, iCS_IStorage iStorage) {
//            return iStorage.CreateMessageHandler(-1, desc as iCS_MessageInfo);            
//        }    
//
    }
    
}
