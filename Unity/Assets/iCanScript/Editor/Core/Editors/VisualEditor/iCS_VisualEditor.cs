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
    const float kUpdateRate= 30f;   // time/seconds.
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    iCS_ContextualMenu  myContextualMenu= null;
    iCS_Graphics        myGraphics      = null;

    // ----------------------------------------------------------------------
    Vector2 GridOffset= Vector2.zero;
    Vector2 SavedDisplayRootAnchorPosition= Vector2.zero;
    
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
    iCS_VisualScriptImp VisualScript {
        get {
            return IStorage.VisualScript;
        }
    }
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
    // Change in display Root.
	// ----------------------------------------------------------------------
    public void OnDisplayRootChange() {
        GridOffset= Vector2.zero;
        CenterAndScaleOn(DisplayRoot);
        Repaint();        
    }
    public void OnStartRelayoutOfTree() {
        // Keep a copy of the display root anchor position.
        SavedDisplayRootAnchorPosition= DisplayRoot.GlobalAnchorPosition;
    }
    public void OnEndRelayoutOfTree() {
        // Reset anchor position of display root to avoid movement in parent node.
        if(DisplayRoot != StorageRoot) {
            var anchorPosition= DisplayRoot.GlobalAnchorPosition;
            var deltaAnchor= anchorPosition-SavedDisplayRootAnchorPosition;
            if(Math3D.IsNotZero(deltaAnchor)) {
//                DisplayRoot.GlobalAnchorPosition= SavedDisplayRootAnchorPosition;
                ScrollPosition-= deltaAnchor;
                GridOffset+= deltaAnchor;                            
            }
        }  
    }
    
    // ======================================================================
    // Force a repaint on selection change.
	// ----------------------------------------------------------------------
    public new void OnSelectionChange() {
        base.OnSelectionChange();
        myNeedRepaint= true;
        mySubEditor= null;
    }
    // ======================================================================
    // Update all message ports when hierarchy has changed
	// ----------------------------------------------------------------------
    public new void OnHierarchyChange() {
        base.OnHierarchyChange();
		if(IStorage == null || IStorage.EditorObjects.Count == 0) return;
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
        if(iCS_DevToolsConfig.TakeVisualEditorSnapshot) {
            // Start countdown
            if(iCS_DevToolsConfig.SnapshotCountDown == -1) {
                iCS_DevToolsConfig.SnapshotCountDown= 3;
            }
            if(iCS_DevToolsConfig.SnapshotCountDown != 0) {
                --iCS_DevToolsConfig.SnapshotCountDown;
                return;
            }
            iCS_DevToolsConfig.SnapshotCountDown= -1;
			iCS_DevToolsConfig.TakeVisualEditorSnapshot= false;
            // Snapshot the asset store big image frame
            Rect pos;
            if(iCS_DevToolsConfig.ShowAssetStoreBigImageFrame) {
                Rect liveRect;
                pos= iCS_DevToolsConfig.GetAssetStoreBigImageRect(new Vector2(position.width, position.height), out liveRect);
            }
            // Snapshot the asset store small image frame
            else if(iCS_DevToolsConfig.ShowAssetStoreSmallImageFrame) {
                Rect liveRect;
                pos= iCS_DevToolsConfig.GetAssetStoreSmallImageRect(new Vector2(position.width, position.height), out liveRect);
            }
            // Snapshot the entire viewport
            else {
    			pos= new Rect(0, 0, position.width, position.height);
            }
			Debug.Log("iCanScript: Visual Editor Snapshot taken at "+DateTime.Now);
            Texture2D snapshot;
            if(iCS_DevToolsConfig.IsSnapshotWithoutBackground) {
    			snapshot= new Texture2D((int)pos.width, (int)pos.height, TextureFormat.ARGB32, false);                                
            }
            else {
                snapshot= new Texture2D((int)pos.width, (int)pos.height, TextureFormat.RGB24, false);                
            }
            pos.x+= 2;
            pos.y+= 3;
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
            if(IStorage.IsAnimationPlaying || myAnimatedScrollPosition.IsActive || myAnimatedScale.IsActive) {
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
//    static int ourButton= -1;
    void ProcessEvents() {
		var ev= Event.current;
		// TODO: Should use undo/redo event to rebuild editor objects.
		//if(ev.commandName == "UndoRedoPerformed") {
        //    Debug.Log("EventType: "+ev.type);
		//	IStorage.SynchronizeAfterUndoRedo();
		//	Debug.Log("UndoRedo detected");
	    //}

//        // %%%%%%%%%%%%
//        // UNITY 4.5 BUG: MouseUp / MouseDown event do not always work
//        //-------------
//        // This code directly reads the state of the mouse to reproduce
//        // the MouseUp / MouseDown events. 
//        if(ev.button > 0 || (ev.isMouse && ev.button == 0)) {
//            if(ourButton != ev.button) {
//                Debug.Log("Button down=> "+ev.button);
//                MouseDownEvent();
//            }
//            ourButton= ev.button;
//        }
//        else {
//            if(ourButton != -1) {
//                Debug.Log("Button up=> "+ev.button);
//                MouseUpEvent();
//                myNeedRepaint= true;                
//            }
//            ourButton= -1;
//        }
//        // %%%%%%%%%%%%
        
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
                if(ev.commandName == "ReloadStorage") {
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
