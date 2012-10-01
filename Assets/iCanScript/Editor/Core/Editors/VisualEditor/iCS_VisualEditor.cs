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

	// ----------------------------------------------------------------------
	bool    HasKeyboardFocus    { get { return EditorWindow.focusedWindow == MyWindow; }}
    bool    IsFloatingKeyDown	{ get { return Event.current.control; }}
    bool    IsCopyKeyDown       { get { return Event.current.shift; }}
    bool    IsScaleKeyDown      { get { return Event.current.alt; }}
    bool    IsShiftKeyDown      { get { return Event.current.shift; }}
    
    // ======================================================================
    // INITIALIZATION
	// ----------------------------------------------------------------------
    // Prepares the editor for editing a graph.  Note that the graph to edit
    // is not configured at this point.  We must wait for an activate from
    // the graph inspector to know which graph to edit. 
	public new void OnEnable() {        
        base.OnEnable();
        
		// Tell Unity we want to be informed of move drag events
		MyWindow.wantsMouseMove= true;

        // Create worker objects.
        myGraphics   = new iCS_Graphics();
        myDynamicMenu= new iCS_DynamicMenu();
        
        // Inspect the assemblies for components.
        if(!ourAlreadyParsed) {
            ourAlreadyParsed= true;
            iCS_Reflection.ParseAppDomain();
        }
        
        // Get snapshot for realtime clock.
        myCurrentTime= Time.realtimeSinceStartup;	    
	}

	// ----------------------------------------------------------------------
    // Releases all resources used by the iCS_Behaviour editor.
    public new void OnDisable() {
        base.OnDisable();
        
        // Release all worker objects.
        myGraphics   = null;
        myDynamicMenu= null;
    }

	// ----------------------------------------------------------------------
    // Assures proper initialization and returns true if editor is ready
    // to execute.
	bool IsInitialized() {
        // Nothing to do if we don't have a Graph to edit...
		if(IStorage == null) {
            myDisplayRoot= null;
            myBookmark= null;
            DragType= DragTypeEnum.None;
		    return false;
		}
        if(IStorage != myPreviousIStorage) {
            myPreviousIStorage= IStorage;
            myDisplayRoot= StorageRoot;
            myBookmark= null;
            DragType= DragTypeEnum.None;
            return false;            
        }
        
		// Don't run if graphic sub-system did not initialise.
		if(iCS_Graphics.IsInitialized == false) {
            iCS_Graphics.Init(IStorage);
			return false;
		}
        iCS_InstallerMgr.InstallGizmo();
        return true;
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
    // EDITOR WINDOW MAIN LAYOUT
	// ----------------------------------------------------------------------
    float UsableWindowWidth() {
        return position.width-2*iCS_Config.EditorWindowGutterSize;
    }
    
	// ----------------------------------------------------------------------
    float UsableWindowHeight() {
        return position.height-2*iCS_Config.EditorWindowGutterSize+iCS_Config.EditorWindowToolbarHeight;
    }
    
	// ----------------------------------------------------------------------
    // Manages the object selection.
    iCS_EditorObject DetermineSelectedObject() {
        // Object selection is performed on left mouse button only.
        iCS_EditorObject newSelected= GetObjectAtMousePosition();
		if(SelectedObject != null && newSelected != null && newSelected.IsOutMuxPort && IStorage.GetOutMuxPort(SelectedObject) == newSelected) {
			ShouldRotateMuxPort= true;
			return SelectedObject;
		}
		ShouldRotateMuxPort= false;
        SelectedObject= newSelected;
        ShowInstanceEditor();
        return SelectedObject;
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    iCS_EditorObject GetObjectAtMousePosition() {
        return GetObjectAtScreenPosition(ViewportMousePosition);
    }

	// ----------------------------------------------------------------------
    // Returns the node at the given mouse position.
    iCS_EditorObject GetNodeAtMousePosition() {
        Vector2 graphPosition= ViewportToGraph(ViewportMousePosition);
        return IStorage.GetNodeAt(graphPosition);
    }

	// ----------------------------------------------------------------------
    // Returns the port at the given mouse position.
    iCS_EditorObject GetPortAtMousePosition() {
        Vector2 graphPosition= ViewportToGraph(ViewportMousePosition);
        return IStorage.GetPortAt(graphPosition);
    }

	// ----------------------------------------------------------------------
    // Returns the closest port at the given mouse position.
    iCS_EditorObject GetClosestPortAtMousePosition() {
        Vector2 graphPosition= ViewportToGraph(ViewportMousePosition);
        return IStorage.GetClosestPortAt(graphPosition);
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    iCS_EditorObject GetObjectAtScreenPosition(Vector2 _screenPos) {
        Vector2 graphPosition= ViewportToGraph(_screenPos);
        iCS_EditorObject port= IStorage.GetPortAt(graphPosition);
        if(port != null) {
            if(IStorage.IsMinimized(port)) return IStorage.GetParent(port);
            return port;
        }
        iCS_EditorObject node= IStorage.GetNodeAt(graphPosition);                
        if(node != null) return node;
        return null;
    }
	// ----------------------------------------------------------------------
    void RotateSelectedMuxPort() {
		if(SelectedObject == null || !SelectedObject.IsDataPort) return;
		if(SelectedObject.IsOutMuxPort) {
			IStorage.ForEachChild(SelectedObject, 
				c=> {
					if(c.IsDataPort) {
						SelectedObject= c;
						return true;
					}
					return false;
				}
			);
			return;
		}
		iCS_EditorObject muxPort= IStorage.GetParent(SelectedObject);
		if(!muxPort.IsDataPort) return;
		bool takeNext= false;
		bool found= IStorage.ForEachChild(muxPort,
			c=> {
				if(takeNext) {
					SelectedObject= c;
					return true;
				}
				if(c == SelectedObject) takeNext= true;
				return false;
			}
		);
		if(!found) SelectedObject= muxPort;
	}
	
	// ----------------------------------------------------------------------
    bool VerifyNewDragConnection(iCS_EditorObject fixPort, iCS_EditorObject dragPort) {
        // No new connection if no overlapping port found.
        iCS_EditorObject overlappingPort= IStorage.GetOverlappingPort(dragPort);
        if(overlappingPort == null) return false;

        // Only data ports can be connected together.
        if(!fixPort.IsDataPort || !overlappingPort.IsDataPort) return false;
        // Destroy drag port since it is not needed anymore.
        IStorage.DestroyInstance(dragPort);
        dragPort= null;
        return VerifyNewConnection(fixPort, overlappingPort);
    }
	// ----------------------------------------------------------------------
    bool VerifyNewConnection(iCS_EditorObject fixPort, iCS_EditorObject overlappingPort) {
        // Only data ports can be connected together.
        if(!fixPort.IsDataPort || !overlappingPort.IsDataPort) return false;
        iCS_EditorObject portParent= IStorage.GetParent(fixPort);
        iCS_EditorObject overlappingPortParent= IStorage.GetParent(overlappingPort);
        if(overlappingPort.IsOutputPort && (overlappingPortParent.IsState || overlappingPortParent.IsStateChart)) {
			CreateStateMux(fixPort, overlappingPort);
			return true;
		}
        
        // Connect function & modules ports together.
        iCS_EditorObject inPort = null;
        iCS_EditorObject outPort= null;

        bool portIsChildOfOverlapping= IStorage.IsChildOf(portParent, overlappingPortParent);
        bool overlappingIsChildOfPort= IStorage.IsChildOf(overlappingPortParent, portParent);
        if(portIsChildOfOverlapping || overlappingIsChildOfPort) {
            if(fixPort.IsInputPort && overlappingPort.IsInputPort) {
                if(portIsChildOfOverlapping) {
                    inPort= fixPort;
                    outPort= overlappingPort;
                } else {
                    inPort= overlappingPort;
                    outPort= fixPort;
                }
            } else if(fixPort.IsOutputPort && overlappingPort.IsOutputPort) {
                if(portIsChildOfOverlapping) {
                    inPort= overlappingPort;
                    outPort= fixPort;
                } else {
                    inPort= fixPort;
                    outPort= overlappingPort;
                }                    
            } else {
                MyWindow.ShowNotification(new GUIContent("Cannot connect nested node ports from input to output !!!"));
                return true;
            }
        } else {
            inPort = fixPort.IsInputPort          ? fixPort : overlappingPort;
            outPort= overlappingPort.IsOutputPort ? overlappingPort : fixPort;
        }
        if(inPort != outPort) {
            iCS_ReflectionInfo conversion= null;
            if(VerifyConnectionTypes(inPort, outPort, out conversion)) {
                SetNewDataConnection(inPort, outPort, conversion);                
            }
        } else {
            string direction= inPort.IsInputPort ? "input" : "output";
            MyWindow.ShowNotification(new GUIContent("Cannot connect an "+direction+" port to an "+direction+" port !!!"));
        }
        return true;
    }
	// ----------------------------------------------------------------------
    bool VerifyConnectionTypes(iCS_EditorObject inPort, iCS_EditorObject outPort, out iCS_ReflectionInfo typeCast) {
        typeCast= null;
		Type inType= inPort.RuntimeType;
		Type outType= outPort.RuntimeType;
        if(iCS_Types.CanBeConnectedWithoutConversion(outType, inType)) { // No conversion needed.
            return true;
        }
        // A conversion is required.
		if(iCS_Types.CanBeConnectedWithUpConversion(outType, inType)) {
			if(EditorUtility.DisplayDialog("Up Conversion Connection", "Are you sure you want to generate a conversion from "+iCS_Types.TypeName(outType)+" to "+iCS_Types.TypeName(inType)+"?", "Generate Conversion", "Abort")) {
                return true;
			}
            return false;
		}
        typeCast= iCS_DataBase.FindTypeCast(outType, inType);
        if(typeCast == null) {
			MyWindow.ShowNotification(new GUIContent("No automatic type conversion exists from "+iCS_Types.TypeName(outType)+" to "+iCS_Types.TypeName(inType)));
            return false;
        }
        return true;
    }
	// ----------------------------------------------------------------------
	void CreateStateMux(iCS_EditorObject fixPort, iCS_EditorObject stateMuxPort) {
        iCS_ReflectionInfo conversion= null;
        if(!VerifyConnectionTypes(stateMuxPort, fixPort, out conversion)) return;
		var source= IStorage.GetSource(stateMuxPort);
		// Simply connect a disconnected mux state port.
		if(source == null && IStorage.NbOfChildren(stateMuxPort, c=> c.IsDataPort) == 0) {
			SetNewDataConnection(stateMuxPort, fixPort, conversion);
			return;
		}
		// Convert source port to child port.
		if(source != null) {
			stateMuxPort.ObjectType= iCS_ObjectTypeEnum.OutMuxPort;
			var firstMuxInput= IStorage.CreatePort(fixPort.Name, stateMuxPort.InstanceId, stateMuxPort.RuntimeType, iCS_ObjectTypeEnum.InMuxPort);
			IStorage.SetSource(firstMuxInput, source);
			IStorage.SetSource(stateMuxPort, null);
		}
		// Create new mux input port.
		var inMuxPort= IStorage.CreatePort(fixPort.Name, stateMuxPort.InstanceId, stateMuxPort.RuntimeType, iCS_ObjectTypeEnum.InMuxPort);
		SetNewDataConnection(inMuxPort, fixPort, conversion);
	}
	// ----------------------------------------------------------------------
    void SetNewDataConnection(iCS_EditorObject inPort, iCS_EditorObject outPort, iCS_ReflectionInfo conversion= null) {
		iCS_EditorObject inNode= IStorage.GetParent(inPort);
        iCS_EditorObject outNode= IStorage.GetParent(outPort);
        iCS_EditorObject inParent= GetParentNode(inNode);
        iCS_EditorObject outParent= GetParentNode(outNode);
        // No need to create module ports if both connected nodes are under the same parent.
        if(inParent == outParent || inParent == outNode || inNode == outParent) {
            IStorage.SetSource(inPort, outPort, conversion);
            IStorage.OptimizeDataConnection(inPort, outPort);
            return;
        }
        // Create inPort if inParent is not part of the outParent hierarchy.
        bool inParentSeen= false;
        for(iCS_EditorObject op= GetParentNode(outParent); op != null; op= GetParentNode(op)) {
            if(inParent == op) {
                inParentSeen= true;
                break;
            }
        }
        if(!inParentSeen && inParent != null) {
            iCS_EditorObject newPort= IStorage.CreatePort(outPort.Name, inParent.InstanceId, outPort.RuntimeType, iCS_ObjectTypeEnum.InDynamicModulePort);
            IStorage.SetSource(inPort, newPort, conversion);
            SetNewDataConnection(newPort, outPort);
            IStorage.OptimizeDataConnection(inPort, outPort);
            return;                       
        }
        // Create outPort if outParent is not part of the inParent hierarchy.
        bool outParentSeen= false;
        for(iCS_EditorObject ip= GetParentNode(inParent); ip != null; ip= GetParentNode(ip)) {
            if(outParent == ip) {
                outParentSeen= true;
                break;
            }
        }
        if(!outParentSeen && outParent != null) {
            iCS_EditorObject newPort= IStorage.CreatePort(outPort.Name, outParent.InstanceId, outPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
            IStorage.SetSource(newPort, outPort, conversion);
            SetNewDataConnection(inPort, newPort);
            IStorage.OptimizeDataConnection(inPort, outPort);
            return;                       
        }
        // Should never happen ... just connect the ports.
        IStorage.SetSource(inPort, outPort, conversion);
        IStorage.OptimizeDataConnection(inPort, outPort);
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetStateAt(Vector2 point) {
        iCS_EditorObject node= IStorage.GetNodeAt(point);
        while(node != null && !node.IsState) {
            node= IStorage.GetNodeAt(point, node);
        }
        return node;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetStateChartAt(Vector2 point) {
        iCS_EditorObject node= IStorage.GetNodeAt(point);
        while(node != null && !node.IsStateChart) {
            node= IStorage.GetNodeAt(point, node);
        }
        return node;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetParentModule(iCS_EditorObject edObj) {
        iCS_EditorObject parentModule= IStorage.GetParent(edObj);
        for(; parentModule != null && !parentModule.IsModule; parentModule= IStorage.GetParent(parentModule));
        return parentModule;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetParentNode(iCS_EditorObject edObj) {
        iCS_EditorObject parentNode= IStorage.GetParent(edObj);
        for(; parentNode != null && !parentNode.IsNode; parentNode= IStorage.GetParent(parentNode));
        return parentNode;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetValidParentNodeUnder(Vector2 point, iCS_ObjectTypeEnum objType, string objName) {
        iCS_EditorObject newParent= IStorage.GetNodeAt(point);
        if(newParent != null && !iCS_AllowedChildren.CanAddChildNode(objName, objType, newParent, IStorage)) {
            newParent= null;
        }
        return newParent;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetValidParentNodeUnder(iCS_EditorObject node) {
        if(!node.IsNode) return null;
        Vector2 point= Math3D.Middle(IStorage.GetLayoutPosition(node));
        iCS_EditorObject newParent= IStorage.GetNodeAt(point, node);
        if(newParent == IStorage.GetParent(node)) return newParent;
        if(newParent != null && !iCS_AllowedChildren.CanAddChildNode(node.Name, node.ObjectType, newParent, IStorage)) {
            newParent= null;
        }
        return newParent;
    }
	// ----------------------------------------------------------------------
    void ChangeParent(iCS_EditorObject node, iCS_EditorObject newParent) {
        iCS_EditorObject oldParent= IStorage.GetParent(node);
        if(newParent == null || newParent == oldParent) return;
        IStorage.SetParent(node, newParent);
		if(node.IsState) CleanupEntryState(node, oldParent);
        CleanupConnections(node);
    }
	// ----------------------------------------------------------------------
	void CleanupEntryState(iCS_EditorObject state, iCS_EditorObject prevParent) {
		state.IsEntryState= false;
		iCS_EditorObject newParent= IStorage.GetParent(state);
		bool anEntryExists= false;
		IStorage.ForEachChild(newParent, child=> { if(child.IsEntryState) anEntryExists= true; });
		if(!anEntryExists) state.IsEntryState= true;
		anEntryExists= false;
		IStorage.ForEachChild(prevParent, child=> { if(child.IsEntryState) anEntryExists= true; });
		if(!anEntryExists) {
			IStorage.ForEachChild(prevParent,
				child=> {
					if(child.IsState) {
						child.IsEntryState= true;
						return true;
					}
					return false;
				}
			);
		}
	}
	// ----------------------------------------------------------------------
    void CleanupConnections(iCS_EditorObject node) {
        switch(node.ObjectType) {
            case iCS_ObjectTypeEnum.StateChart: {
                List<iCS_EditorObject> childNodes= new List<iCS_EditorObject>();
                IStorage.ForEachChild(node, c=> { if(c.IsNode) childNodes.Add(c);});
                foreach(var childNode in childNodes) { CleanupConnections(childNode); }
                break;                
            }
            case iCS_ObjectTypeEnum.State: {
                // Attempt to relocate transition modules.
                IStorage.ForEachChildPort(node,
                    p=> {
                        if(p.IsStatePort) {
                            iCS_EditorObject transitionModule= null;
                            if(p.IsInStatePort) {
                                transitionModule= IStorage.GetParent(IStorage.GetSource(p));
                            } else {
                                iCS_EditorObject[] connectedPorts= IStorage.FindConnectedPorts(p);
                                foreach(var cp in connectedPorts) {
                                    if(cp.IsInTransitionPort) {
                                        transitionModule= IStorage.GetParent(cp);
                                        break;
                                    }
                                }
                            }
                            iCS_EditorObject outState= IStorage.GetParent(IStorage.GetOutStatePort(transitionModule));
                            iCS_EditorObject inState= IStorage.GetParent(IStorage.GetInStatePort(transitionModule));
                            iCS_EditorObject newParent= IStorage.GetTransitionParent(inState, outState);
                            if(newParent != null && newParent != IStorage.GetParent(transitionModule)) {
                                ChangeParent(transitionModule, newParent);
                                IStorage.LayoutTransitionModule(transitionModule);
                                IStorage.SetDirty(IStorage.GetParent(node));
                            }
                        }
                    }
                );
                // Ask our children to cleanup their connections.
                List<iCS_EditorObject> childNodes= new List<iCS_EditorObject>();
                IStorage.ForEachChild(node, c=> { if(c.IsNode) childNodes.Add(c);});
                foreach(var childNode in childNodes) { CleanupConnections(childNode); }
                break;
            }
            case iCS_ObjectTypeEnum.TransitionModule:
            case iCS_ObjectTypeEnum.TransitionGuard:
            case iCS_ObjectTypeEnum.TransitionAction:
            case iCS_ObjectTypeEnum.Module: {
                List<iCS_EditorObject> childNodes= new List<iCS_EditorObject>();
                IStorage.ForEachChild(node, c=> { if(c.IsNode) childNodes.Add(c);});
                foreach(var childNode in childNodes) { CleanupConnections(childNode); }
                break;
            }
            case iCS_ObjectTypeEnum.InstanceMethod:
            case iCS_ObjectTypeEnum.StaticMethod:
            case iCS_ObjectTypeEnum.InstanceField:
            case iCS_ObjectTypeEnum.StaticField:
            case iCS_ObjectTypeEnum.TypeCast: {
                IStorage.ForEachChildPort(node,
                    port=> {
                        if(port.IsInDataPort) {
                            iCS_EditorObject sourcePort= RemoveConnection(port);
                            // Rebuild new connection.
                            if(sourcePort != port) {
                                SetNewDataConnection(port, sourcePort);
                            }
                        }
                        if(port.IsOutDataPort) {
                            iCS_EditorObject[] allInPorts= FindAllConnectedInDataPorts(port);
                            foreach(var inPort in allInPorts) {
                                RemoveConnection(inPort);
                            }
                            foreach(var inPort in allInPorts) {
                                if(inPort != port) {
                                    SetNewDataConnection(inPort, port);                                    
                                }
                            }
                        }
                    }
                );
                break;
            }
            default: {
                break;
            }
        }
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject RemoveConnection(iCS_EditorObject inPort) {
        iCS_EditorObject sourcePort= IStorage.GetDataConnectionSource(inPort);
        // Tear down previous connection.
        iCS_EditorObject tmpPort= IStorage.GetSource(inPort);
        List<iCS_EditorObject> toDestroy= new List<iCS_EditorObject>();
        while(tmpPort != null && tmpPort != sourcePort) {
            iCS_EditorObject[] connected= IStorage.FindConnectedPorts(tmpPort);
            if(connected.Length == 1) {
                iCS_EditorObject t= IStorage.GetSource(tmpPort);
                toDestroy.Add(tmpPort);
                tmpPort= t;
            } else {
                break;
            }
        }
        foreach(var byebye in toDestroy) {
            IStorage.DestroyInstance(byebye.InstanceId);
        }
        return sourcePort;        
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject[] FindAllConnectedInDataPorts(iCS_EditorObject outPort) {
        List<iCS_EditorObject> allInDataPorts= new List<iCS_EditorObject>();
        FillConnectedInDataPorts(outPort, allInDataPorts);
        return allInDataPorts.ToArray();
    }
    // ----------------------------------------------------------------------
    void FillConnectedInDataPorts(iCS_EditorObject outPort, List<iCS_EditorObject> result) {
        if(outPort == null) return;
        iCS_EditorObject[] connectedPorts= IStorage.FindConnectedPorts(outPort);
        foreach(var port in connectedPorts) {
            if(port.IsDataPort) {
                if(port.IsModulePort) {
                    FillConnectedInDataPorts(port, result);
                } else {
                    if(port.IsInputPort) {
                        result.Add(port);
                    }
                }
            }
        }
    }
	// ----------------------------------------------------------------------
    void PasteIntoGraph(Vector2 point, iCS_Storage sourceStorage, iCS_EditorObject sourceRoot) {
        if(sourceRoot == null) return;
        iCS_EditorObject validParent= GetValidParentNodeUnder(point, sourceRoot.ObjectType, sourceRoot.Name);
        if(validParent == null) {
			var node= IStorage.GetNodeAt(point);
			if(node == null && IStorage.IsEmptyBehaviour) {
				int option= EditorUtility.DisplayDialogComplex("Behaviour event required !", "Unity behaviour requires that nodes be added to a predefined event.  Use the buttons below to create the desired event type for your node.","Create Update", "More events...","Create OnGUI");
				switch(option) {
					case 0:
						validParent= AutoCreateBehaviourEvent(iCS_Strings.Update, point);
						break;
					case 1:
						MyWindow.ShowNotification(new GUIContent("Please use right mouse click on canvas to create behaviour event type before adding new object."));
						SelectedObject= IStorage.EditorObjects[0];
						myDynamicMenu.Update(SelectedObject, IStorage, point);
						IStorage.SetDirty(SelectedObject);
						return;
					case 2:
						validParent= AutoCreateBehaviourEvent(iCS_Strings.OnGUI, point);
						break;
				}
			} else {
	            EditorUtility.DisplayDialog("Operation Aborted", "Unable to find a suitable parent to paste into !!!", "Cancel");				
				return;
			}
        }
        iCS_EditorObject pasted= IStorage.CopyFrom(sourceRoot, new iCS_IStorage(sourceStorage), validParent, point);
        if(IStorage.IsMaximized(pasted)) {
            IStorage.Fold(pasted);            
        }
    }
    iCS_EditorObject AutoCreateBehaviourEvent(string eventName, Vector2 point) {
		var validParent= IStorage.CreateModule(0, point, iCS_Strings.Update);
		validParent.Tooltip= iCS_AllowedChildren.TooltipForBehaviourChild(iCS_Strings.Update);
        IStorage.Maximize(validParent);
        return validParent;
    }

    // ======================================================================
    // Graph Navigation
	// ----------------------------------------------------------------------
    public void CenterOnRoot() {
        CenterOn(myDisplayRoot);
    }
	// ----------------------------------------------------------------------
    public void CenterAndScaleOnRoot() {
        CenterAndScaleOn(myDisplayRoot);
    }
	// ----------------------------------------------------------------------
    public void CenterOnSelected() {
        CenterOn(SelectedObject ?? myDisplayRoot);
    }
	// ----------------------------------------------------------------------
    public void CenterAndScaleOnSelected() {
        CenterAndScaleOn(SelectedObject ?? myDisplayRoot);
    }
	// ----------------------------------------------------------------------
    public void CenterOn(iCS_EditorObject obj) {
        if(obj == null || IStorage == null) return;
        CenterAt(Math3D.Middle(IStorage.GetLayoutPosition(obj)));
    }
	// ----------------------------------------------------------------------
    public void CenterAndScaleOn(iCS_EditorObject obj) {
        if(obj == null || IStorage == null) return;
        while(obj != null && !IStorage.IsVisible(obj)) obj= IStorage.GetParent(obj);
        if(obj == null) return;
        Rect objectArea= IStorage.GetLayoutPosition(obj);
        float newScale= 1.0f;
        if(obj.IsNode) {
            float widthScale= position.width/(1.1f*objectArea.width);
            float heightScale= position.height/(1.1f*objectArea.height);
            newScale= Mathf.Min(1.0f, Mathf.Min(widthScale, heightScale));
        }
        CenterAtWithScale(Math3D.Middle(objectArea), newScale);
    }
	// ----------------------------------------------------------------------
    public void CenterAt(Vector2 point) {
        if(IStorage == null) return;
        Vector2 newScrollPosition= point-0.5f/Scale*new Vector2(position.width, position.height);
        float distance= Vector2.Distance(ScrollPosition, newScrollPosition);
        float deltaTime= distance/3500f;
        if(deltaTime < iCS_PreferencesEditor.AnimationTime) deltaTime= iCS_PreferencesEditor.AnimationTime;
        if(deltaTime > 0.5f) deltaTime= 0.5f+(0.5f*(deltaTime-0.5f));
        myAnimatedScrollPosition.Start(ScrollPosition, newScrollPosition, deltaTime, (start,end,ratio)=> Math3D.Lerp(start, end, ratio));
        ScrollPosition= newScrollPosition;
    }
	// ----------------------------------------------------------------------
    public void CenterAtWithScale(Vector2 point, float newScale) {
        if(IStorage == null) return;
        Vector2 newScrollPosition= point-0.5f/newScale*new Vector2(position.width, position.height);
        float distance= Vector2.Distance(ScrollPosition, newScrollPosition);
        float deltaTime= distance/3500f;
        if(deltaTime < iCS_PreferencesEditor.AnimationTime) deltaTime= iCS_PreferencesEditor.AnimationTime;
        if(deltaTime > 0.5f) deltaTime= 0.5f+(0.5f*(deltaTime-0.5f));
        myAnimatedScrollPosition.Start(ScrollPosition, newScrollPosition, deltaTime, (start,end,ratio)=> Math3D.Lerp(start, end, ratio));
        ScrollPosition= newScrollPosition;
        myAnimatedScale.Start(Scale, newScale, deltaTime, (start,end,ratio)=> Math3D.Lerp(start, end, ratio));
        Scale= newScale;
    }
    // ======================================================================
    // NODE GRAPH DISPLAY
	// ----------------------------------------------------------------------
    void DrawGrid() {
        myGraphics.DrawGrid(position,
                            iCS_PreferencesEditor.CanvasBackgroundColor,
                            iCS_PreferencesEditor.GridColor,
                            iCS_PreferencesEditor.GridSpacing);
    }                       
    
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
	// ----------------------------------------------------------------------
	void DrawGraph () {
        // Ask the storage to update itself.
        IStorage.Update();

		// Start graphics
        myGraphics.Begin(UpdateScrollPosition(), UpdateScale(), ClipingArea, SelectedObject, GraphMousePosition);
        
        // Draw editor grid.
        DrawGrid();
        
        // Draw nodes and their connections.
        DisplayGraphNodes();

        myGraphics.End(IStorage);

        // Show scroll zone (is applicable).
        if(IsDragStarted) DrawScrollZone();

		// Show header
		Heading();			
	}
	
	// ----------------------------------------------------------------------
	Vector2 UpdateScrollPosition() {
        Vector2 graphicScrollPosition= ScrollPosition;
        if(myAnimatedScrollPosition.IsActive) {
            myAnimatedScrollPosition.Update();
            graphicScrollPosition= myAnimatedScrollPosition.CurrentValue;
        }
		return graphicScrollPosition;
	}
	// ----------------------------------------------------------------------
    float UpdateScale() {
        float scale= Scale;
        if(myAnimatedScale.IsActive) {
            myAnimatedScale.Update();
            scale= myAnimatedScale.CurrentValue;
        }
        return scale;
    }

}
