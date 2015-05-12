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
using iCanScript.Internal.Engine;


namespace iCanScript.Internal.Editor {
    using Prefs= PreferencesController;

    public partial class iCS_VisualEditor : iCS_EditorBase {
        // ======================================================================
        // Constants
        // ----------------------------------------------------------------------
        const float kUpdateRate= 30f;   // time/seconds.
        
        // ======================================================================
        // Properties
        // ----------------------------------------------------------------------
        iCS_ContextualMenu  myContextualMenu = null;
        iCS_Graphics        myGraphics       = null;
    
        // ----------------------------------------------------------------------
        Vector2 GridOffset= Vector2.zero;
        
        // ----------------------------------------------------------------------
        bool  myShowDynamicMenu    = false;
        int   myUpdateCounter      = 0;
        float myCurrentTime        = 0;
        float myDeltaTime          = 0;
        bool  myNeedRepaint        = true;
    	
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
        }
        public void OnEndRelayoutOfTree() {
        }
        
        // ======================================================================
        // Force a repaint on selection change.
    	// ----------------------------------------------------------------------
        public new void OnSelectionChange() {
            base.OnSelectionChange();
            myNeedRepaint= true;
            CloseSubEditor();
        }
        // ======================================================================
        // Update all message ports when hierarchy has changed
    	// ----------------------------------------------------------------------
        public new void OnHierarchyChange() {
            base.OnHierarchyChange();
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
                if(IStorage.IsAnimationPlaying || myAnimatedScrollPosition.IsActive || myAnimatedScale.IsActive || mySubEditor != null) {
                    Repaint();
                    myNeedRepaint= true;
                }
                // Repaint on request.
                else if(myNeedRepaint) {
                    Repaint();
                    myNeedRepaint= false;                    
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
            // -- Draw common stuff for all editors --
            base.OnGUI();
    
           	// -- Update pending GUI commands --
            RunOnGUICommands();
            
            // -- Attempt to initialize environment (if not already done) --
            bool isInit= IsInitialized();
    
    		// Update mouse info.
    		UpdateMouse();
    		
    		// -- Nothing to be drawn until we are fully initialized --
            if(!isInit || IStorage == null) {
                // -- Show next step help --
                ShowWorkflowAssistant();
                return;            
            }
            // -- Assure that we have a library window opened --
            iCS_EditorController.OpenLibraryEditor();
            
            // -- Update GUI time --
            myDeltaTime= Time.realtimeSinceStartup-myCurrentTime;
            myCurrentTime= Time.realtimeSinceStartup;
    
            // -- Load Editor Skin --
            GUI.skin= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
            
    		// -- Update pending menu commands --
    		myContextualMenu.OnGUI();
    		
            // -- Process all visual editor events including Repaint --
            if(IsMouseInToolbar && Event.current.type != EventType.Repaint) {
                Toolbar();
            } else {
                 ProcessEvents();            
            }
            
            // -- Process scroll zone --
            ProcessScrollZone();
            
            // -- Debug information --
#if SHOW_FRAME_RATE || SHOW_FRAME_TIME
            FrameRateDebugInfo();
#endif
    		
    		// -- Simulate OnPostRender --
    		OnPostRender();
            
    //        // -- Test library panel --
    //        libraryEditor.position= new Rect(0,0, 350, position.height);
    //        var saveColor= GUI.color;
    //        GUI.color= new Color(0,0,0,0.9f);
    //        GUI.Box(libraryEditor.position, "");
    //        GUI.color= saveColor;
    //        libraryEditor.OnGUI();
    	}
    
    //    iCS_LibraryEditor2 libraryEditor= new iCS_LibraryEditor2();
    
    	// ----------------------------------------------------------------------
        // Processes all events.
    //    static int ourButton= -1;
        void ProcessEvents() {
    		var ev= Event.current;
            switch(ev.type) {
                case EventType.Repaint: {
                    // Draw Graph.				
                    DrawGraph();
    				DisplayHelp();
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
                    if(mySubEditor == null) {
                        KeyDownEvent();                    
                        myNeedRepaint= true;
                    }
                    else {
    //                    EditorGUI.FocusTextInControl("SubEditor");
                    }
        			break;
    			}
    			case EventType.ValidateCommand: {
                    // Accept undo/redo.
    			    if(ev.commandName == "UndoRedoPerformed") {
                        ev.Use();
    			    }
    			    break;
    			}
    			case EventType.ExecuteCommand: {
                    // Rebuild engine objects on undo/redo.
    			    if(ev.commandName == "UndoRedoPerformed") {
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
    		Debug.Log("VisualEditor: Frame Time: Average=> "+myAverageFrameTime+" Current=> "+frameTime+" Max=> "+myMaxFrameTime);
#endif			    
    	}
#endif
    }
}