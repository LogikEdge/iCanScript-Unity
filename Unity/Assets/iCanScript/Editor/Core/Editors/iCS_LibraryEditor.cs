using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

/*
	TODO : Should filter on name, input port type, and output port type.
*/
public class iCS_LibraryEditor : iCS_EditorBase {
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const int   kIconWidth  = 16;
    const int   kIconHeight = 16;
    const float kLabelSpacer= 4f;

    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    DSScrollView            myMainView;
    Rect                    myScrollViewArea;
	iCS_LibraryController   myController;
	Rect                    mySelectedAreaCache= new Rect(0,0,0,0);

	public new void OnEnable() { 
		// Tell Unity we want to be informed of mouse move events
		wantsMouseMove= true;
	}
	
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    bool IsInitialized() {
        if(myController == null || myMainView == null) {
            myController= new iCS_LibraryController();
            myMainView= new DSScrollView(new RectOffset(0,0,0,0), false, true, true, myController.View);
        }
        return true;
    }
    
	// =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    public new void OnGUI() {
        // Draw the base stuff for all windows.
        base.OnGUI();
    
        // Show library components.
        UpdateMgr();
		if(!IsInitialized()) return;
		var toolbarRect= ShowToolbar();
        myScrollViewArea= new Rect(0,toolbarRect.height,position.width,position.height-toolbarRect.height);
		myMainView.Display(myScrollViewArea);
		ProcessEvents(myScrollViewArea);
		// Make new selection visible
		if(mySelectedAreaCache != myController.SelectedArea) {
		    mySelectedAreaCache= myController.SelectedArea;
		    myMainView.MakeVisible(mySelectedAreaCache, myScrollViewArea);
		}            
	}
    // ---------------------------------------------------------------------------------
	Rect ShowToolbar() {
        // -- Display toolbar header --
		var headerRect= iCS_ToolbarUtility.BuildToolbar(position.width);
        var search1Rect= new Rect(headerRect.x, headerRect.yMax, headerRect.width, headerRect.height);
        // Display # of items found
        myController.ShowInherited= iCS_ToolbarUtility.Toggle(ref headerRect, myController.ShowInherited, 0, 0);
        iCS_ToolbarUtility.MiniLabel(ref headerRect, "Show Inherited", 0, 0);
        var numberOfItems= myController.NumberOfItems;
        iCS_ToolbarUtility.MiniLabel(ref headerRect, "# items: "+numberOfItems.ToString(), 0, 0, true);

        // -- Display toolbar search field #1 --
        var search= myController.SearchCriteria_1;
        iCS_ToolbarUtility.BuildToolbar(search1Rect);
        search.ShowClasses= iCS_ToolbarUtility.Toggle(ref search1Rect, search.ShowClasses, 0, 0);
		var icon= iCS_Icons.GetLibraryNodeIconFor(iCS_DefaultNodeIcons.ObjectInstance);
        iCS_ToolbarUtility.Texture(ref search1Rect, icon, 0, 4);            
        iCS_ToolbarUtility.Separator(ref search1Rect);
        iCS_ToolbarUtility.Separator(ref search1Rect);

        search.ShowFunctions= iCS_ToolbarUtility.Toggle(ref search1Rect, search.ShowFunctions, kLabelSpacer, 0);
        icon= iCS_Icons.GetLibraryNodeIconFor(iCS_DefaultNodeIcons.Function);            
        iCS_ToolbarUtility.Texture(ref search1Rect, icon, 0, kLabelSpacer);            
        iCS_ToolbarUtility.Separator(ref search1Rect);

        search.ShowVariables= iCS_ToolbarUtility.Toggle(ref search1Rect, search.ShowVariables, kLabelSpacer, 0);
        icon= iCS_BuiltinTextures.OutEndPortIcon;
        iCS_ToolbarUtility.Texture(ref search1Rect, icon, 0, 0);            
        icon= iCS_BuiltinTextures.InEndPortIcon;
        iCS_ToolbarUtility.Texture(ref search1Rect, icon, 0, kLabelSpacer);            
        iCS_ToolbarUtility.Separator(ref search1Rect);

		string searchString= myController.SearchString ?? "";
		myController.SearchString= iCS_ToolbarUtility.Search(ref search1Rect, 120.0f, searchString, 0, 0, true);
		return Math3D.Union(headerRect, search1Rect);
	}
	// =================================================================================
    // Event processing
    // ---------------------------------------------------------------------------------
    void ProcessEvents(Rect frameArea) {
     	Vector2 mousePosition= Event.current.mousePosition;
        var selected= myController.Selected;
		iCS_EditorController.FindVisualEditor().updateHelpFromLibrary();
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
            case EventType.ScrollWheel: {
                break;
            }
            case EventType.MouseDown: {
                var mouseInScreenPoint= GUIUtility.GUIToScreenPoint(mousePosition);
                var areaInScreenPoint= GUIUtility.GUIToScreenPoint(new Vector2(frameArea.x, frameArea.y));
                var areaInScreenPosition= new Rect(areaInScreenPoint.x, areaInScreenPoint.y, frameArea.width, frameArea.height);
                myController.MouseDownOn(null, mouseInScreenPoint, areaInScreenPosition);
                Event.current.Use();
                // Move keyboard focus to this window.			
                GUI.FocusControl("");   // Removes focus from the search field.
				break;
			}

            case EventType.MouseUp: {
				break;
			}
			case EventType.KeyDown: {
				var ev= Event.current;
				if(!ev.isKey) break;
                switch(ev.keyCode) {
                    // Tree navigation
                    case KeyCode.UpArrow: {
                        myController.SelectPrevious();
                        ev.Use();
                        break;
                    }
                    case KeyCode.DownArrow: {
                        myController.SelectNext();
                        ev.Use();
                        break;
                    }
                    // Fold/Unfold toggle
                    case KeyCode.Return: {
                        myController.ToggleFoldUnfoldSelected();
                        ev.Use();
                        break;
                    }
                }
                switch(ev.character) {
                    // Fold/Unfold.
                    case '+': {
                        myController.UnfoldSelected();
                        ev.Use();
                        break;
                    }
                    case '-': {
                        myController.FoldSelected();
                        ev.Use();
                        break;
                    }
					case 'h': {
                        myController.Help();
                        ev.Use();
                        break;
					}
                }
                break;
			}
        }   
    }

	// =================================================================================
    // Drag events.
    // ---------------------------------------------------------------------------------
    void StartDragAndDrop(iCS_LibraryController.Node node) {
		// Just return if nothing is selected.
        if(node == null) return;
		// Don't allow to drag & drop company, library, and packages
		switch(node.Type) {
			case iCS_LibraryController.NodeTypeEnum.Root:
			case iCS_LibraryController.NodeTypeEnum.Company:
			case iCS_LibraryController.NodeTypeEnum.Library:
			case iCS_LibraryController.NodeTypeEnum.Package:
				return;
			default:
				break;
		}
        // Build drag object.
        GameObject go= new GameObject(node.Name);
        go.hideFlags = HideFlags.HideAndDontSave;
        var library= go.AddComponent("iCS_Library") as iCS_LibraryImp;
        iCS_IStorage iStorage= new iCS_IStorage(library);
        CreateInstance(node, iStorage);
        iStorage.SaveStorage();
        // Fill drag info.
        DragAndDrop.PrepareStartDrag();
        DragAndDrop.objectReferences= new UnityEngine.Object[1]{go};
        DragAndDrop.StartDrag(node.Name);
        iCS_AutoReleasePool.AutoRelease(go, 60f);
    }
    // ---------------------------------------------------------------------------------
    void CreateInstance(iCS_LibraryController.Node node, iCS_IStorage iStorage) {
        if(node.Type == iCS_LibraryController.NodeTypeEnum.Company) {
            CreatePackage(node.Name, iStorage);        
            return;
        }
        if(node.Type == iCS_LibraryController.NodeTypeEnum.Library) {
            CreatePackage(node.Name, iStorage);        
            return;
        }
        if(node.Type == iCS_LibraryController.NodeTypeEnum.Class) {
            CreateObjectInstance(node.MemberInfo.ClassType, iStorage);        
            return;
        }
        if(node.Type == iCS_LibraryController.NodeTypeEnum.Field) {
            CreateMethod(node.MemberInfo, iStorage);        
            return;
        }
        if(node.Type == iCS_LibraryController.NodeTypeEnum.Property) {
            CreateMethod(node.MemberInfo, iStorage);        
            return;
        }
        if(node.Type == iCS_LibraryController.NodeTypeEnum.Constructor) {
            CreateMethod(node.MemberInfo, iStorage);        
            return;
        }
        if(node.Type == iCS_LibraryController.NodeTypeEnum.Method) {
            CreateMethod(node.MemberInfo, iStorage);        
            return;
        }
		if(node.Type == iCS_LibraryController.NodeTypeEnum.Message) {
            var module= CreateMessage(node.MemberInfo, iStorage);        
			if(node.MemberInfo.IconPath != null) {
				module.IconPath= node.MemberInfo.IconPath;				
			}
			return;
		}
		
    }
    // ======================================================================
    // Creation Utilities
    // ---------------------------------------------------------------------------------
    // FIXME: Should pass along the object type to the module instead of multiple creation.
    iCS_EditorObject CreatePackage(string name, iCS_IStorage iStorage) {
        return iStorage.CreatePackage(-1, name);
    }
    // ---------------------------------------------------------------------------------
    iCS_EditorObject CreateObjectInstance(Type classType, iCS_IStorage iStorage) {
        return iStorage.CreateObjectInstance(-1, null, classType);
    }
    // ---------------------------------------------------------------------------------
    iCS_EditorObject CreateMethod(iCS_MemberInfo desc, iCS_IStorage iStorage) {
        return iStorage.CreateFunction(-1, desc.ToMethodBaseInfo);            
    }    
    // ---------------------------------------------------------------------------------
    iCS_EditorObject CreateMessage(iCS_MemberInfo desc, iCS_IStorage iStorage) {
        return iStorage.CreateMessageHandler(-1, desc as iCS_MessageInfo);            
    }    
}
