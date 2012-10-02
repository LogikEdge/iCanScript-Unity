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
    iCS_EditorObject    myDisplayRoot= null;
    iCS_DynamicMenu     myDynamicMenu= null;
    iCS_Graphics        myGraphics   = null;
    
    // ----------------------------------------------------------------------
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

    // ======================================================================
    // Force a repaint on selection change.
	// ----------------------------------------------------------------------
    public void OnSelectionChange() {
        myNeedRepaint= true;
    }
    
    // ======================================================================
    // Periodic Update
	// ----------------------------------------------------------------------
	public void Update() {
//        // Don't run update faster then requested.
//        float currentTime= Time.realtimeSinceStartup;
//        int newUpdateCounter= (int)(currentTime*kUpdateRate);
//        if(newUpdateCounter == myUpdateCounter) return;
//        myUpdateCounter= newUpdateCounter;
//        
//        // Abort if our environment is not initialized.
//        if(!IsInitialized()) return;
//        
//        // Determine repaint rate.
//        if(IStorage != null) {
//            // Update DisplayRoot
//            if(myDisplayRoot == null && IStorage.IsValid(0)) {
//                myDisplayRoot= IStorage[0];
//            }            
//            
//            // Repaint visual editor if it has changed
//            if(IStorage.IsDirty || IStorage.IsAnimationPlaying || myAnimatedScrollPosition.IsActive || myAnimatedScale.IsActive) {
//                MyWindow.Repaint();
//                myNeedRepaint= true;
//            }
//            // Repaint on request.
//            else if(myNeedRepaint) {
//                MyWindow.Repaint();
//                myNeedRepaint= false;                    
//            }
//            // Repaint if game is running.
//            else if(Application.isPlaying && iCS_PreferencesEditor.ShowRuntimePortValue) {
//                float period= iCS_PreferencesEditor.PortValueRefreshPeriod;
//                if(period < 0.1f) period= 0.1f;
//                float refreshFactor= 1f/period;
//                int newRefreshCounter= (int)(currentTime*refreshFactor);
//                if(newRefreshCounter != myRefreshCounter) {
//                    myRefreshCounter= newRefreshCounter;
//                    MyWindow.Repaint();
//                }
//            }
//#if FORCE_REPAINT
//            else {
//                MyWindow.Repaint();					
//            }
//#endif
//        }
//
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
        
		// Update sub editor if active.
		if(mySubEditor != null) {
			mySubEditor.Update();
		}

       // Process all visual editor events including Repaint.
       Debug.Log("Event: "+Event.current.type);
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
        switch(Event.current.type) {
            case EventType.Repaint: {
                // Draw Graph.
                DrawGraph();
                Event.current.Use();                        
                break;                
            }
            case EventType.Layout: {
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
                myNeedRepaint= true;
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
