//#define SHOW_FRAME_COUNT
//#define SHOW_FRAME_TIME

using UnityEngine;
using UnityEditor;
using System;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/*
    TODO: Should show frameId in header bar.
*/
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This non-persistante class is used to edit the iCS_Behaviour.
public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    iCS_EditorObject    myDisplayRoot= null;
    iCS_DynamicMenu     myDynamicMenu= null;
    iCS_Graphics        myGraphics   = null;
    iCS_IStorage        myPreviousIStorage= null;
    
    // ----------------------------------------------------------------------
    int   myUpdateCounter = 0;
    int   myRefreshCounter= 0;
    float myCurrentTime   = 0;
    float myDeltaTime     = 0;
    bool  myNeedRepaint   = true; 
    
    // ----------------------------------------------------------------------
    Prelude.Animate<Vector2>    myAnimatedScrollPosition= new Prelude.Animate<Vector2>();
    Prelude.Animate<float>      myAnimatedScale         = new Prelude.Animate<float>();
    
    // ----------------------------------------------------------------------
    DragTypeEnum     DragType              = DragTypeEnum.None;
    iCS_EditorObject DragObject            = null;
    iCS_EditorObject DragFixPort           = null;
    iCS_EditorObject DragOriginalPort      = null;
    Vector2          MouseDragStartPosition= Vector2.zero;
    Vector2          DragStartPosition     = Vector2.zero;
    bool             IsDragEnabled         = false;
    bool             IsDragStarted         { get { return IsDragEnabled && DragObject != null; }}

    // ----------------------------------------------------------------------
    iCS_EditorObject SelectedObjectBeforeMouseDown= null;
    iCS_EditorObject myBookmark= null;
	bool			 ShouldRotateMuxPort= false;
    
    // ----------------------------------------------------------------------
    Rect    ClipingArea    { get { return new Rect(ScrollPosition.x, ScrollPosition.y, Viewport.width, Viewport.height); }}
    Vector2 ViewportCenter { get { return new Vector2(0.5f/Scale*position.width, 0.5f/Scale*position.height); } }
    Rect    Viewport       { get { return new Rect(0,0,position.width/Scale, position.height/Scale); }}
    Vector2 ViewportToGraph(Vector2 v) { return v+ScrollPosition; }
    Rect    GraphArea {
        get {
            float headerHeight= iCS_ToolbarUtility.GetHeight();
            return new Rect(position.x, position.y+headerHeight, position.width, position.height-headerHeight);
            }
    }
    Rect    HeaderArea {
        get {
            float headerHeight= iCS_ToolbarUtility.GetHeight();
            return new Rect(position.x, position.y, position.width, headerHeight);
            }    
    }
    
    // ----------------------------------------------------------------------
    static bool	ourAlreadyParsed  = false;
     
    // ======================================================================
    // Properties
	// ----------------------------------------------------------------------
    iCS_EditorObject StorageRoot {
        get {
            if(IStorage == null || Prelude.length(IStorage.EditorObjects) == 0) return null;
            return IStorage.EditorObjects[0];
        }
    }
	// ----------------------------------------------------------------------
    Vector2     ScrollPosition {
        get { return IStorage != null ? IStorage.ScrollPosition : Vector2.zero; }
        set { if(IStorage != null) IStorage.ScrollPosition= value; }
    }
    float       Scale {
        get { return IStorage != null ? IStorage.GuiScale : 1.0f; }
        set {
            if(value > 2f) value= 2f;
            if(value < 0.15f) value= 0.15f;
            if(IStorage != null) IStorage.GuiScale= value;
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
#if SHOW_FRAME_COUNT
	float	myAverageFrameRate= 0f;
	int     myFrameRateLastDisplay= 0;
#endif
#if SHOW_FRAME_TIME
	float myAverageFrameTime= 0f;
	float myMaxFrameTime= 0f;
#endif
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

    // ======================================================================
    // Graph Navigation
	// ----------------------------------------------------------------------
	void Heading() {
		// Build standard toolbar at top of editor window.
		Rect r= iCS_ToolbarUtility.BuildToolbar(position.width, -1f);

		// Insert an initial spacer.
		float spacer= 8f;
		r.x+= spacer;
		r.width-= spacer;
		
//        // Adjust toolbar styles
//		Vector2 test= EditorStyles.toolbar.contentOffset;
//		test.y=3f;
//		EditorStyles.toolbar.contentOffset= test;
//		test= EditorStyles.toolbarTextField.contentOffset;
//		test.y=2f;
//		EditorStyles.toolbarTextField.contentOffset= test;
		
		// Show zoom control at the end of the toolbar.
        float newScale= iCS_ToolbarUtility.Slider(ref r, 120f, Scale, 2f, 0.15f, spacer, spacer, true);
        iCS_ToolbarUtility.Label(ref r, new GUIContent("Zoom"), 0, 0, true);
		if(Math3D.IsNotEqual(newScale, Scale)) {
            Vector2 pivot= ViewportToGraph(ViewportCenter);
            CenterAtWithScale(pivot, newScale);
		}
		
		// Show current bookmark.
		string bookmarkString= "myBookmark: ";
		if(myBookmark == null) {
		    bookmarkString+= "(empty)";
		} else {
		    bookmarkString+= myBookmark.Name;
		}
		iCS_ToolbarUtility.Label(ref r, 150f, new GUIContent(bookmarkString),0,0,true);
	}
}
