//#define SHOW_FRAME_COUNT
//#define SHOW_FRAME_TIME

using UnityEngine;
using UnityEditor;
using System;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    iCS_EditorObject    myDisplayRoot= null;
    iCS_DynamicMenu     myDynamicMenu= null;
    iCS_Graphics        myGraphics   = null;
    
    // ----------------------------------------------------------------------
    int   myUpdateCounter = 0;
    int   myRefreshCounter= 0;
    float myCurrentTime   = 0;
    float myDeltaTime     = 0;
    bool  myNeedRepaint   = true; 
    
    // ----------------------------------------------------------------------
    static bool	ourAlreadyParsed  = false;

    // ----------------------------------------------------------------------
    // Debug properties.
#if SHOW_FRAME_COUNT
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
    // UPDATE FUNCTIONALITY
	// ----------------------------------------------------------------------
	public void Update() {
        // Perform 15 update per seconds.
        float currentTime= Time.realtimeSinceStartup;
        int newUpdateCounter= (int)(currentTime*15f);
        if(newUpdateCounter == myUpdateCounter) return;
        myUpdateCounter= newUpdateCounter;
        
        // Update storage selection.
        UpdateMgr();
        if(!IsInitialized()) return;
        // Determine repaint rate.
        if(IStorage != null) {
            // Repaint window
            if(IStorage.IsDirty || IStorage.IsAnimationPlaying || myAnimatedScrollPosition.IsActive || myAnimatedScale.IsActive) {
                MyWindow.Repaint();
                myNeedRepaint= true;
            } else if(myNeedRepaint) {
                MyWindow.Repaint();
                myNeedRepaint= false;                    
            } else if(Application.isPlaying && iCS_PreferencesEditor.ShowRuntimePortValue) {
                float period= iCS_PreferencesEditor.PortValueRefreshPeriod;
                if(period < 0.03f) period= 0.03f;
                float refreshFactor= 1f/period;
                int newRefreshCounter= (int)(currentTime*refreshFactor);
                if(newRefreshCounter != myRefreshCounter) {
                    myRefreshCounter= newRefreshCounter;
                    MyWindow.Repaint();
                }
            }
//			/*
//				CHANGED To be removed
//			*/                
//							else {
//			                    MyWindow.Repaint();					
//							}

            // Update DisplayRoot
            if(myDisplayRoot == null && IStorage.IsValid(0)) {
                myDisplayRoot= IStorage[0];
            }
        }
        // Cleanup objects.
        iCS_AutoReleasePool.Update();
	}
	
	// ----------------------------------------------------------------------
	// User GUI function.
	public override void OnGUI() {
       	if(Event.current.type == EventType.Layout) {
            // Show that we can display because we don't have a behavior or library.
            UpdateMgr();
            if(IStorage == null) {
                MyWindow.ShowNotification(new GUIContent("No iCanScript component selected !!!"));
                return;
            } else {
                MyWindow.RemoveNotification();
            }
            return;       	    
       	}
		// Don't do start editor if not properly initialized.
		if( !IsInitialized() ) return;
       	
        // Update GUI time.
        myDeltaTime= Time.realtimeSinceStartup-myCurrentTime;
        myCurrentTime= Time.realtimeSinceStartup;

        // Load Editor Skin.
        GUI.skin= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
        
		// Update mouse info.
		UpdateMouse();
		
        // Process user inputs.  False is return if graph should not be drawn.
        if(!ProcessEvents()) {
            myNeedRepaint= true;
            return;
        }

        if(Event.current.type == EventType.Repaint) {
            // Draw Graph.
            DrawGraph();
        }

		// Process scroll zone.
		ProcessScrollZone();                        
	
        // Debug information.
        FrameRateDebugInfo();
	}

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
    
    // ======================================================================
    // Debug information.
    // ----------------------------------------------------------------------
	void FrameRateDebugInfo() {
#if SHOW_FRAME_COUNT
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
}
