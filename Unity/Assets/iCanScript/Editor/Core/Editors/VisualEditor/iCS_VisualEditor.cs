//#define SHOW_FRAME_RATE
//#define SHOW_FRAME_TIME
//#define FORCE_REPAINT

using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Prefs= iCS_PreferencesController;


public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
    const float kUpdateRate= 15f;   // time/seconds.
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    iCS_ContextualMenu  myContextualMenu= null;
    iCS_Graphics        myGraphics      = null;
    
    // ----------------------------------------------------------------------
    bool  myShowDynamicMenu    = false;
    int   myUpdateCounter      = 0;
    int   myRefreshCounter     = 0;
    float myCurrentTime        = 0;
    float myDeltaTime          = 0;
    bool  myNeedRepaint        = true;
    bool  myNotificationShown  = false;
	
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
            if(IStorage == null) {
                return null;
            }
            return IStorage.DisplayRoot;
        }
        set {
            IStorage.DisplayRoot= value;
        }
    }
	public bool IsMultiSelectionActive {
		get { return IStorage.IsMultiSelectionActive; }
	}
	
    // ======================================================================
    // Force a repaint on selection change.
	// ----------------------------------------------------------------------
    public void OnSelectionChange() {
        myNeedRepaint= true;
        mySubEditor= null;
    }
    // ======================================================================
    // Update all message ports when hierarchy has changed
	// ----------------------------------------------------------------------
    public void OnHierarchyChange() {
		if(IStorage == null) return;
		iCS_EditorObject behaviour= IStorage.EditorObjects[0];
		if(behaviour == null || !behaviour.IsBehaviour) return;
		behaviour.ForEachChildNode(
			n=> {
				if(n.IsMessage) {
					IStorage.UpdateBehaviourMessagePorts(n);
				}
			}
		);
    }
    // ======================================================================
    // Update all message ports when hierarchy has changed
	// ----------------------------------------------------------------------
	public void OnPostRender()
	{
		if(iCS_DevToolsConfig.framesWithoutBackground != 0) {
			--iCS_DevToolsConfig.framesWithoutBackground;
			return;
		}
		if(iCS_DevToolsConfig.takeVisualEditorSnapshot) {
			iCS_DevToolsConfig.takeVisualEditorSnapshot= false;
			var pos= position;
			Debug.Log("iCanScript: Visual Editor Snapshot taken at "+DateTime.Now);
			var snapshot= new Texture2D((int)pos.width, (int)pos.height, TextureFormat.ARGB32, false);
			pos.x= 0; pos.y= 0;
			snapshot.ReadPixels(pos, 0, 0, false);
			snapshot.Apply();
			var PNGsnapshot= snapshot.EncodeToPNG();
			UnityEngine.Object.DestroyImmediate(snapshot);
			string fileName= iCS_DevToolsConfig.ScreenShotsFolder+"/"+iCS_DateTime.DateTimeAsString()+" VisualEditor.png";
			File.WriteAllBytes(Application.dataPath + fileName, PNGsnapshot);
		}
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
            else if(Application.isPlaying && Prefs.ShowRuntimePortValue) {
                float period= Prefs.PortValueRefreshPeriod;
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
	public new void OnGUI() {
        // Draw common stuff for all editors
        base.OnGUI();

        // Attempt to initialize environment (if not already done).
        bool isInit= IsInitialized();

		// Nothing to be drawn until we are fully initialized.
        if(!isInit || IStorage == null) {
            // Tell the user that we can display without a behavior or library.
            ShowNotification(new GUIContent("No iCanScript component selected !!!"));
            myNotificationShown= true;
            return;            
        }

        // Remove any previously shown notification.
        if(myNotificationShown) {
            RemoveNotification();
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
		myContextualMenu.OnGUI();
		
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

		// Simulate OnPostRender.
		OnPostRender();
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
                break;                
            }
            case EventType.Layout: {
                if(myShowDynamicMenu) {
                    myShowDynamicMenu= false;
                    ShowDynamicMenu();
                }
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
                if(EditorWindow.mouseOverWindow == this) {
                    DragAndDropExited();
                    ev.Use();
                }
                break;
            }
			case EventType.KeyDown: {
                KeyDownEvent();
                myNeedRepaint= true;
    			break;
			}
			case EventType.ValidateCommand: {
                // Accept undo/redo.
			    if(ev.commandName == "UndoRedoPerformed") {
//                    Debug.Log("iCanScript: Display Root before Undo => "+IStorage.iCSMonoBehaviour.Storage.DisplayRoot);
                    ev.Use();
			    }
			    break;
			}
			case EventType.ExecuteCommand: {
                // Rebuild engine objects on undo/redo.
			    if(ev.commandName == "UndoRedoPerformed") {
//                    Debug.Log("iCanScript: Display Root after Undo => "+IStorage.iCSMonoBehaviour.Storage.DisplayRoot);
                    IStorage.SynchronizeAfterUndoRedo();
                    ev.Use();
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
