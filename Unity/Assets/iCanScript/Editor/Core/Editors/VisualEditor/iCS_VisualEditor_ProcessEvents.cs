using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

// ===========================================================================
// Graph editor event processing.
// ===========================================================================
public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Fields
	// ----------------------------------------------------------------------
	iCS_ISubEditor		mySubEditor       = null;
    
    // ======================================================================
    // USER INTERACTIONS
	// ----------------------------------------------------------------------
    void MouseMoveEvent() {
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
        // Keep a copy of the scroll position.
        MouseDragStartPosition= ViewportMousePosition;
        DragStartDisplayPosition= ScrollPosition;
        // Update the selected object.
        SelectedObjectBeforeMouseDown= SelectedObject;
        DetermineSelectedObject();
        switch(Event.current.button) {
            case 0: { // Left mouse button
                if(SelectedObject != null && !SelectedObject.IsBehaviour) {
                    IsDragEnabled= true;                            
                }
                mySubEditor= null;
                break;
            }
            case 1: { // Right mouse button
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
        float mouseUpDeltaTime= myCurrentTime-MouseUpTime;
        MouseUpTime= myCurrentTime;
        if(IsDragStarted) {
            EndDrag();
        } else {
			if(ShouldRotateMuxPort) RotateSelectedMuxPort();
            if(SelectedObject != null) {
                // Process fold/unfold/minimize/maximize click.
                Vector2 mouseGraphPos= GraphMousePosition;
                if(myGraphics.IsFoldIconPicked(SelectedObject, mouseGraphPos)) {
                    if(SelectedObject.IsFoldedOnDisplay) {
                        IStorage.RegisterUndo("Unfold");
                        IStorage.Unfold(SelectedObject);
                    } else {
                        IStorage.RegisterUndo("Fold");
                        IStorage.Fold(SelectedObject);
                    }
                } else if(myGraphics.IsMinimizeIconPicked(SelectedObject, mouseGraphPos)) {
                    IStorage.RegisterUndo("Minimize");
                    IStorage.Iconize(SelectedObject);
                } else {
                    // Fold/Unfold on double click.
                    if(SelectedObject == SelectedObjectBeforeMouseDown) {
                        if(mouseUpDeltaTime < 0.25f) {
                            ProcessNodeDisplayOptionEvent();
                        }
                        else {
                            Event.current.Use();
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
			float zoomDirection= iCS_PreferencesEditor.InverseZoom ? -1f : 1f;
			float scaleDelta= Scale*0.09f*iCS_PreferencesEditor.ZoomSpeed;
            Scale= Scale+(delta.y > 0 ? -scaleDelta : scaleDelta)*zoomDirection;
            Vector2 offset= pivot-GraphMousePosition;
            ScrollPosition+= offset;
        } else {
            delta*= iCS_PreferencesEditor.ScrollSpeed*(1f/Scale); 
            ScrollPosition+= delta;                    
        }        
    }
    // ======================================================================
	// ----------------------------------------------------------------------
    void ProcessNodeDisplayOptionEvent() {
        if(SelectedObject == null || !SelectedObject.IsNode) return;
        if(SelectedObject.IsKindOfFunction || SelectedObject.IsObjectInstance) {
            if(SelectedObject.IsIconizedOnDisplay && !IsShiftKeyDown) {
                IStorage.RegisterUndo("Unfold "+SelectedObject.Name);
                IStorage.Unfold(SelectedObject);                                                                            
            } else if(SelectedObject.IsUnfoldedOnDisplay && IsShiftKeyDown) {
                IStorage.RegisterUndo("Iconize "+SelectedObject.Name);
                IStorage.Iconize(SelectedObject);                    
            }
        } else {
            if(IsShiftKeyDown) {
                if(SelectedObject.IsUnfoldedOnDisplay) {
                    if(IsControlKeyDown) {
                        IStorage.RegisterUndo("Iconize "+SelectedObject.Name);
                        IStorage.Iconize(SelectedObject);                                                
                    } else {
                        IStorage.RegisterUndo("Fold "+SelectedObject.Name);
                        IStorage.Fold(SelectedObject);                                                                    
                    }
                } else if(SelectedObject.IsFoldedOnDisplay) {
                    IStorage.RegisterUndo("Iconize "+SelectedObject.Name);
                    IStorage.Iconize(SelectedObject);                        
                }
            } else {
                if(SelectedObject.IsIconizedOnDisplay) {
                    if(IsControlKeyDown) {
                        IStorage.RegisterUndo("Unfold "+SelectedObject.Name);
                        IStorage.Unfold(SelectedObject);                                                                                                    
                    } else {
                        IStorage.RegisterUndo("Fold "+SelectedObject.Name);
                        IStorage.Fold(SelectedObject);                                            
                    }
                } else if(SelectedObject.IsFoldedOnDisplay) {
                    IStorage.RegisterUndo("Unfold "+SelectedObject.Name);
                    IStorage.Unfold(SelectedObject);                                                                            
                }                    
            }
        }
    }

	// ----------------------------------------------------------------------
    void ShowDynamicMenu() {
        if(SelectedObject == null && DisplayRoot.IsBehaviour) {
            SelectedObject= DisplayRoot;
        }
        ShowInstanceEditor();
        myDynamicMenu.Update(SelectedObject, IStorage, GraphMousePosition);
    }
	// ----------------------------------------------------------------------
    void ShowInstanceEditor() {
        if(SelectedObject != null && (SelectedObject.IsObjectInstance || SelectedObject.IsBehaviour)) {
            bool hadKeyboardFocus= HasKeyboardFocus;
            iCS_EditorMgr.ShowInstanceEditor();
            // Keep keyboard focus.
            if(hadKeyboardFocus) Focus();
        }        
    }
	// ----------------------------------------------------------------------
    void ProcessPicking(iCS_PickInfo pickInfo) {
		iCS_EditorObject pickedObject= pickInfo.PickedObject;
        switch(pickInfo.PickedPart) {
            case iCS_PickPartEnum.Name: {
                if(pickedObject.IsNameEditable) {
    				if(pickedObject.IsPort) {
    					mySubEditor= new iCS_PortNameEditor(pickedObject, pickInfo.IStorage, myGraphics);											
    				}
    				if(pickedObject.IsNode) {
    					mySubEditor= new iCS_NodeNameEditor(pickedObject, myGraphics);
    				}                                            
                } else {
                    ShowNotification(new GUIContent("The selected name cannot be changed !!!"));
                }
                break;
            }
            case iCS_PickPartEnum.Value: {
                if(!pickedObject.IsInDataOrControlPort || pickedObject.SourceId != -1) break;
                break;
            }
        }
    }
	// ----------------------------------------------------------------------
    void UpdateViewportPanning() {
        Vector2 diff= ViewportMousePosition-MouseDragStartPosition;
        ScrollPosition= DragStartDisplayPosition-diff;        
    }
}
