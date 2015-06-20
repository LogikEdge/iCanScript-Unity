using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using iCanScript.Internal.Editor.CodeGeneration;

namespace iCanScript.Internal.Editor {
    using Prefs= PreferencesController;

    // ===========================================================================
    // Graph editor event processing.
    // ===========================================================================
    public partial class iCS_VisualEditor : iCS_EditorBase {
        // ======================================================================
        // Fields
    	// ----------------------------------------------------------------------
    	EditorWindow	mySubEditor = null;
        
        // ======================================================================
        // USER INTERACTIONS
    	// ----------------------------------------------------------------------
        void MouseMoveEvent() {
            // -- Update Help Information --
    		QueueOnGUICommand(UpdateHelp);
            // -- Update Hot Zones --
            var pickInfo= myGraphics.GetPickInfo(GraphMousePosition, IStorage);
            if(pickInfo == null || pickInfo.PickedObject.IsBehaviour) {
                HotZoneMouseOver(WindowMousePosition);
            }
            // -- Canvas Processing --
            switch(Event.current.button) {
                case 2: { // Middle mouse button
                    UpdateViewportPanning();
                    break;
                }
                default: {
                    ResetDrag();
                    break;
                }
            }        
        }
    	// ----------------------------------------------------------------------
        void MouseDragEvent() {
            switch(Event.current.button) {
                case 0: { // Left mouse button
                    // Drag the selected node or pan the viewport if no node is selected.
                    if(IsDragEnabled) {
                        if(EditionController.IsCommunityLimitReached) break;
                        ProcessDrag();                                                
                    } else {
                        UpdateViewportPanning();
                    }
                    break;
                }
                case 2: { // Middle mouse button
                    UpdateViewportPanning();
                    break;
                }
    			default: {
    				break;
    			}
            }        
        }
    	// ----------------------------------------------------------------------
        void MouseDownEvent() {
    		MouseDownPosition= ViewportMousePosition;
            // Update click count.
            myClickCount= Event.current.clickCount;
            // Keep a copy of the scroll position.
            MouseDragStartPosition= ViewportMousePosition;
            DragStartDisplayPosition= ScrollPosition;
            // Update the selected object.
            SelectedObjectBeforeMouseDown= SelectedObject;
            switch(Event.current.button) {
                case 0: { // Left mouse button
                    // -- Update Hot Zones --
                    var pickInfo= myGraphics.GetPickInfo(GraphMousePosition, IStorage);
                    if(HotZoneMouseClick(WindowMousePosition, pickInfo)) {
                        break;
                    }
                    // -- Update Selected Object --
                    DetermineSelectedObject();                    
                    if(SelectedObject != null && DisplayRoot.IsParentOf(SelectedObject)) {
    					if(SelectedObject.IsNode && IsDisplayRootKeyDown && IsDoubleClick &&
    					   !(SelectedObject.IsKindOfFunction || SelectedObject.IsInstanceNode)) {
                               if(SelectedObject == DisplayRoot) {
                                  if(IsShiftKeyDown && IStorage.HasBackwardNavigationHistory) {
                                      iCS_UserCommands.ReloadFromBackwardNavigationHistory(IStorage);
                                  }                               
                               }
                               else {
           					       iCS_UserCommands.SetAsDisplayRoot(SelectedObject);                               
                               }
    					}
                        IsDragEnabled= true;                                                    
                    }
                    CloseSubEditor();
                    break;
                }
                case 1: { // Right mouse button
                    if(!IsMultiSelectionActive) {
                        DetermineSelectedObject();                    
                    }
                    myShowDynamicMenu= true;
                    break;
                }
                case 2: { // Middle mouse button
                    // Mainly used for panning the viewport.
                    break;
                }
            }
        }
    	// ----------------------------------------------------------------------
        void MouseUpEvent() {
            if(IsDragStarted) {
                EndDrag();
            } else {
    			if(ShouldRotateMuxPort) RotateSelectedMuxPort();
                if(SelectedObject != null) {
                    // Process fold/unfold/minimize/maximize click.
                    Vector2 mouseGraphPos= GraphMousePosition;
                    if(myGraphics.IsFoldIconPicked(SelectedObject, mouseGraphPos)) {
                        if(SelectedObject.IsFoldedInLayout) {
                            iCS_UserCommands.Unfold(SelectedObject);
                        } else {
                            iCS_UserCommands.Fold(SelectedObject);
                        }
                    } else if(myGraphics.IsMinimizeIconPicked(SelectedObject, mouseGraphPos)) {
                        iCS_UserCommands.Iconize(SelectedObject);
                    } else {
                        // Fold/Unfold on double click.
                        if(SelectedObject == SelectedObjectBeforeMouseDown) {
                            if(IsShiftKeyDown) {
                                if(myClickCount >= 2) {
                                    ProcessNodeDisplayOptionEvent();
                                }
                            }
                            else {
                                iCS_PickInfo pickInfo= myGraphics.GetPickInfo(GraphMousePosition, IStorage);
    							if(pickInfo != null) {
    							    ProcessPicking(pickInfo);
    							}
                            }
                        }
                    }
                }                                                
            }        
        }
    	// ----------------------------------------------------------------------
        void ScrollWheelEvent() {
            Vector2 delta= Event.current.delta;
            if(IsScaleKeyDown) {
                Vector2 pivot= GraphMousePosition;
    			float zoomDirection= Prefs.InverseZoom ? -1f : 1f;
    			float scaleDelta= Scale*0.09f*Prefs.ZoomSpeed;
                var newScale= Scale+(delta.y > 0 ? -scaleDelta : scaleDelta)*zoomDirection;
                iCS_UserCommands.SetZoom(IStorage, newScale);
                Vector2 offset= pivot-GraphMousePosition;
                var newScrollPosition= ScrollPosition+offset;
                iCS_UserCommands.SetScrollPosition(IStorage, newScrollPosition);
            } else {
                delta*= Prefs.ScrollSpeed*(1f/Scale); 
                var newPosition= ScrollPosition+ delta;
                iCS_UserCommands.SetScrollPosition(IStorage, newPosition);                    
            }        
        }
        // ======================================================================
    	// ----------------------------------------------------------------------
        void ProcessNodeDisplayOptionEvent() {
            if(SelectedObject == null || !SelectedObject.IsNode) return;
            if(IsShiftKeyDown) {
                if(SelectedObject.IsUnfoldedInLayout) {
                    if(IsAltKeyDown) {
                        iCS_UserCommands.Iconize(SelectedObject);                                                
                    } else {
                        iCS_UserCommands.Fold(SelectedObject);                                                                    
                    }
                } else if(SelectedObject.IsFoldedInLayout) {
                    iCS_UserCommands.Iconize(SelectedObject);                        
                }
            } else {
                if(SelectedObject.IsIconizedOnDisplay) {
                    if(IsAltKeyDown) {
                        iCS_UserCommands.Unfold(SelectedObject);                                                                                                    
                    } else {
                        iCS_UserCommands.Fold(SelectedObject);                                            
                    }
                } else if(SelectedObject.IsFoldedInLayout) {
                    iCS_UserCommands.Unfold(SelectedObject);                                                                            
                }                    
            }
        }
    
    	// ----------------------------------------------------------------------
        void ShowDynamicMenu() {
            if(SelectedObject == null && DisplayRoot.IsBehaviour) {
                SelectedObject= DisplayRoot;
            }
            myContextualMenu.Update(iCS_ContextualMenu.MenuType.SelectedObject, SelectedObject, IStorage, GraphMousePosition);            
        }
    	// ----------------------------------------------------------------------
        void ProcessPicking(iCS_PickInfo pickInfo) {
    		if(myClickCount < 2) return;
    		iCS_EditorObject pickedObject= pickInfo.PickedObject;
            switch(pickInfo.PickedPart) {
                case iCS_PickPartEnum.Name:
                case iCS_PickPartEnum.Value:
                case iCS_PickPartEnum.EditorObject: {
                    if(pickedObject.IsPort) {
                        CloseSubEditor();
                        mySubEditor= PortEditor.Create(pickedObject, new Vector2(100,100));                    
                    }
                    else {
                        CloseSubEditor();
                        if(pickedObject.IsIconizedInLayout) {
                            iCS_UserCommands.Unfold(pickedObject);
                        }
                        else {
                            mySubEditor= NodeEditor.Create(pickedObject, new Vector2(100,100));
                        }
                    }
                    myClickCount= 0;
                    break;
                }
            }
        }
    	// ----------------------------------------------------------------------
        void UpdateViewportPanning() {
            Vector2 diff= ViewportMousePosition-MouseDragStartPosition;
            var newPosition= DragStartDisplayPosition-diff;        
            iCS_UserCommands.SetScrollPosition(IStorage, newPosition);
        }
    	// ----------------------------------------------------------------------
        public void PanViewportBy(Vector2 additionalPan) {
            ScrollPosition-= additionalPan;
        }
    
    	// ----------------------------------------------------------------------
        /// Closes the subeditor if it exists.
        void CloseSubEditor() {
            if(mySubEditor != null) {
                mySubEditor.Close();
                mySubEditor= null;
            }
        }
    }
}