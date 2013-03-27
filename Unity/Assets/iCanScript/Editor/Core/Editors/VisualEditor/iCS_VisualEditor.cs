//#define SHOW_FRAME_RATE
//#define SHOW_FRAME_TIME
//#define FORCE_REPAINT

using UnityEngine;
using UnityEditor;
using System;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
    const float kUpdateRate= 15f;   // time/seconds.
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    int             myDisplayRootId= 0;
    iCS_DynamicMenu myDynamicMenu  = null;
    iCS_Graphics    myGraphics     = null;
    
    // ----------------------------------------------------------------------
    bool  myShowDynamicMenu  = false;
    int   myUpdateCounter    = 0;
    int   myRefreshCounter   = 0;
    float myCurrentTime      = 0;
    float myDeltaTime        = 0;
    bool  myNeedRepaint      = true;
    bool  myNotificationShown= false; 
    
    // ----------------------------------------------------------------------
    static bool	ourAlreadyParsed  = false;

    // ----------------------------------------------------------------------
    // Debug properties.
#if SHOW_FRAME_RATE
	float	myAverageFrameRate= 0f;
	int     myFrameRateLastDisplay= 0;
#endif
#if SHOW_FRAME_TIME
	float myAverageFrameTime= 0f;
	float myMaxFrameTime= 0f;
#endif
     

    // ======================================================================
    // Properties
	// ----------------------------------------------------------------------
    iCS_EditorObject StorageRoot {
        get {
            if(IStorage == null || Prelude.length(IStorage.EditorObjects) == 0) return null;
            return IStorage.EditorObjects[0];
        }
    }
    iCS_EditorObject DisplayRoot {
        get {
            if(myDisplayRootId < 0 || IStorage == null || Prelude.length(IStorage.EditorObjects) <= myDisplayRootId) {
                return null;
            }
            return IStorage.EditorObjects[myDisplayRootId];
        }
        set {
            myDisplayRootId= value == null ? -1 : value.InstanceId;
        }
    }
    
    // ======================================================================
    // Force a repaint on selection change.
	// ----------------------------------------------------------------------
    public void OnSelectionChange() {
        myNeedRepaint= true;
        mySubEditor= null;
    }
    
    // ======================================================================
    // Repaint Proxy
	// ----------------------------------------------------------------------
    void Repaint() {
        MyWindow.Repaint();
    }
    
    // ======================================================================
    // Periodic Update
	// ----------------------------------------------------------------------
    // TODO: Simplify Update using event instead of continusouly scan all objects.
	public void Update() {
        // Don't run update faster then requested.
        float currentTime= Time.realtimeSinceStartup;
        int newUpdateCounter= (int)(currentTime*kUpdateRate);
        if(newUpdateCounter == myUpdateCounter) return;
        myUpdateCounter= newUpdateCounter;
        
        // Abort if our environment is not initialized.
        if(!IsInitialized()) return;
        
        // Determine repaint rate.
        if(IStorage != null) {
            // Update DisplayRoot
            if(DisplayRoot == null && IStorage.IsValid(0)) {
                DisplayRoot= IStorage[0];
            }            
            
            // Repaint visual editor if it has changed
            if(IStorage.IsDirty || IStorage.IsAnimationPlaying || myAnimatedScrollPosition.IsActive || myAnimatedScale.IsActive) {
                Repaint();
                myNeedRepaint= true;
            }
            // Repaint on request.
            else if(myNeedRepaint) {
                Repaint();
                myNeedRepaint= false;                    
            }
            // Repaint if game is running.
            else if(Application.isPlaying && iCS_PreferencesEditor.ShowRuntimePortValue) {
                float period= iCS_PreferencesEditor.PortValueRefreshPeriod;
                if(period < 0.1f) period= 0.1f;
                float refreshFactor= 1f/period;
                int newRefreshCounter= (int)(currentTime*refreshFactor);
                if(newRefreshCounter != myRefreshCounter) {
                    myRefreshCounter= newRefreshCounter;
                    Repaint();
                }
            }
#if FORCE_REPAINT
            else {
                Repaint();					
            }
#endif
        }

        // Cleanup memory pool.
        iCS_AutoReleasePool.Update();
	}
	
    // ======================================================================
	// Handles all event messages.
	// ----------------------------------------------------------------------
	public override void OnGUI() {
        // Attempt to initialize environment (if not already done).
        bool isInit= IsInitialized();

		// Nothing to be drawn until we are fully initialized.
        if(!isInit || IStorage == null) {
            // Tell the user that we can display without a behavior or library.
            MyWindow.ShowNotification(new GUIContent("No iCanScript component selected !!!"));
            myNotificationShown= true;
            return;            
        }

        // Remove any previously shown notification.
        if(myNotificationShown) {
            MyWindow.RemoveNotification();
            myNotificationShown= false;
        }
       	
        // Update GUI time.
        myDeltaTime= Time.realtimeSinceStartup-myCurrentTime;
        myCurrentTime= Time.realtimeSinceStartup;

		// Update mouse info.
		UpdateMouse();
		
        // Load Editor Skin.
        GUI.skin= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
        
		// Update pending menu commands
		myDynamicMenu.OnGUI();
		
		// Update sub editor if active.
		if(mySubEditor != null) {
			mySubEditor.Update();
		}

       // Process all visual editor events including Repaint.
       if(IsMouseInToolbar && Event.current.type != EventType.Repaint) {
           Toolbar();
       } else {
           ProcessEvents();           
       }

		// Process scroll zone.
		ProcessScrollZone();                        
	
        // Debug information.
#if SHOW_FRAME_RATE || SHOW_FRAME_TIME
        FrameRateDebugInfo();
#endif
	}

	// ----------------------------------------------------------------------
    // Processes all events.
    void ProcessEvents() {
		var ev= Event.current;
		// TODO: Should use undo/redo event to rebuild editor objects.
		//if(ev.commandName == "UndoRedoPerformed") {
        //    Debug.Log("EventType: "+ev.type);
		//	IStorage.SynchronizeAfterUndoRedo();
		//	Debug.Log("UndoRedo detected");
	    //}
        switch(ev.type) {
            case EventType.Repaint: {
                // Draw Graph.
                DrawGraph();
//                Event.current.Use();  // Unity will repeat this event if it is used ???                        
                break;                
            }
            case EventType.Layout: {
                if(myShowDynamicMenu) {
                    myShowDynamicMenu= false;
                    ShowDynamicMenu();
                }
                Event.current.Use();                        
                break;
            }
            case EventType.MouseMove: {
                MouseMoveEvent();
                Event.current.Use();                        
                break;
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
                myNeedRepaint= true;
                break;
            }
            case EventType.ScrollWheel: {
                ScrollWheelEvent();
                Event.current.Use();                
                break;
            }
            // Unity DragAndDrop events.
            case EventType.DragPerform: {
                DragAndDropPerformed();
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
                myNeedRepaint= true;
    			break;
			}
			case EventType.ValidateCommand: {
                // Force repaint on undo/redo.
			    if(ev.commandName == "UndoRedoPerformed") {
			        Repaint();
			        break;
			    }
			    break;
			}
        }
    }
    
    // ======================================================================
    // Debug information.
    // ----------------------------------------------------------------------
#if SHOW_FRAME_RATE || SHOW_FRAME_TIME
	void FrameRateDebugInfo() {
#if SHOW_FRAME_RATE
		myAverageFrameRate= (myAverageFrameRate*9f+myDeltaTime)/10f;
       	if(Math3D.IsNotZero(myAverageFrameRate) && myFrameRateLastDisplay != (int)myCurrentTime) {
            myFrameRateLastDisplay= (int)myCurrentTime;
       	    Debug.Log("VisualEditor: frame rate: "+1f/myAverageFrameRate);
       	}
#endif            
#if SHOW_FRAME_TIME
		float frameTime= Time.realtimeSinceStartup- myCurrentTime;
		if(frameTime > myMaxFrameTime) myMaxFrameTime= frameTime;
		myAverageFrameTime= (myAverageFrameTime*9f+frameTime)/10f;
		Debug.Log("VisualEditor: frame time: "+myAverageFrameTime+" max frame time: "+myMaxFrameTime);
#endif			    
	}
#endif
}
