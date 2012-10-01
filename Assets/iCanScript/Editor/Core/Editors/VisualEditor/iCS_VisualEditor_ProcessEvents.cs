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
    // Processes all events.  Returns true if visual editor should be drawn
    // or false if processing should stop.
    bool ProcessEvents() {
		// Update sub editor if active.
		if(mySubEditor != null) {
			mySubEditor.Update();
		}
        
//        Debug.Log("EventType= "+Event.current.type);
        // Process window events.
        switch(Event.current.type) {
            case EventType.MouseMove: {
                MouseMoveEvent();
                Event.current.Use();                        
                return false;
            }
            case EventType.MouseDrag: {
                MouseDragEvent();
                Event.current.Use();
                break;
            }
            case EventType.MouseDown: {
                MouseDownEvent();
                Event.current.Use();
                break;
            }
            case EventType.MouseUp: {
                MouseUpEvent();
                Event.current.Use();
                break;
            }
            case EventType.ScrollWheel: {
                ScrollWheelEvent();
                Event.current.Use();                
                break;
            }
            // Unity DragAndDrop events.
            case EventType.DragPerform: {
                DragAndDropPerform();
                Event.current.Use();                
                break;
            }
            case EventType.DragUpdated: {
                DragAndDropUpdated();
                Event.current.Use();            
                break;
            }
            case EventType.DragExited: {
                if(EditorWindow.mouseOverWindow == MyWindow) {
                    DragAndDropExited();
                    Event.current.Use();
                }
                break;
            }
			case EventType.KeyDown: {
                KeyDownEvent();
    			return false;
			}
        }
        return true;
    }
	// ----------------------------------------------------------------------
    void MouseMoveEvent() {
        switch(Event.current.button) {
            case 2: { // Middle mouse button
                Vector2 diff= ViewportMousePosition-MouseDragStartPosition;
                ScrollPosition= DragStartPosition-diff;
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
                ProcessDrag();                            
                break;
            }
            case 2: { // Middle mouse button
                Vector2 diff= ViewportMousePosition-MouseDragStartPosition;
                ScrollPosition= DragStartPosition-diff;
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
                ShowDynamicMenu();
                break;
            }
            case 2: { // Middle mouse button
                MouseDragStartPosition= ViewportMousePosition;
                DragStartPosition= ScrollPosition;
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
                if(myGraphics.IsFoldIconPicked(SelectedObject, mouseGraphPos, IStorage)) {
                    if(IStorage.IsFolded(SelectedObject)) {
                        IStorage.RegisterUndo("Unfold");
                        IStorage.Maximize(SelectedObject);
                    } else {
                        IStorage.RegisterUndo("Fold");
                        IStorage.Fold(SelectedObject);
                    }
                } else if(myGraphics.IsMinimizeIconPicked(SelectedObject, mouseGraphPos, IStorage)) {
                    IStorage.RegisterUndo("Minimize");
                    IStorage.Minimize(SelectedObject);
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
        if(SelectedObject != null && SelectedObject.IsNode) {
            if(SelectedObject.IsMinimized) {
                IStorage.RegisterUndo("Unfold");
                IStorage.Fold(SelectedObject);
            } else if(SelectedObject.IsMaximized) {
                IStorage.RegisterUndo("Fold");
                IStorage.Fold(SelectedObject);
            } else {
                if(IsShiftKeyDown) {
                    IStorage.RegisterUndo("Minimize");
                    IStorage.Minimize(SelectedObject);
                } else {
                    IStorage.RegisterUndo("Maximize");
                    IStorage.Maximize(SelectedObject);                                                        
                }
            }
        }        
    }

	// ----------------------------------------------------------------------
    void ShowDynamicMenu() {
        if(SelectedObject == null && myDisplayRoot.IsBehaviour) {
            SelectedObject= myDisplayRoot;
        }
        ShowInstanceEditor();
        myDynamicMenu.Update(SelectedObject, IStorage, GraphMousePosition);
        IStorage.SetDirty(SelectedObject);                    
    }
	// ----------------------------------------------------------------------
    void ShowInstanceEditor() {
        if(SelectedObject != null && SelectedObject.IsClassModule) {
            bool hadKeyboardFocus= HasKeyboardFocus;
            iCS_EditorMgr.ShowInstanceEditor();
            // Keep keyboard focus.
            if(hadKeyboardFocus) MyWindow.Focus();
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
    					mySubEditor= new iCS_NodeNameEditor(pickedObject, pickInfo.IStorage, myGraphics);
    				}                                            
                } else {
                    MyWindow.ShowNotification(new GUIContent("The selected name cannot be changed !!!"));
                }
                break;
            }
            case iCS_PickPartEnum.Value: {
                if(!pickedObject.IsInDataPort || pickedObject.Source != -1) break;
                Debug.Log("Value is being picked");
                break;
            }
        }
    }
}
