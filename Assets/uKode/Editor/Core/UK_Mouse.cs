using UnityEngine;
using UnityEditor;
using System.Collections;

public class UK_Mouse {

	// ----------------------------------------------------------------------
    public UK_Mouse(EditorWindow _window) {
        myEditorWindow= _window;
    }
	// ----------------------------------------------------------------------
    ~UK_Mouse() {
        myEditorWindow= null;
    }
    
	// ----------------------------------------------------------------------
    // Resets tye mouse informtion when we lose focus.
    public void Reset() {
        Position= Vector2.zero;
        IsRightButtonDown= false;
        RightButtonUpPosition= Vector2.zero;
        RightButtonDownPosition= Vector2.zero;
        IsLeftButtonDown= false;
        LeftButtonUpPosition= Vector2.zero;
        LeftButtonDownPosition= Vector2.zero;
        IsMiddleButtonDown= false;
        MiddleButtonUpPosition= Vector2.zero;
        MiddleButtonDownPosition= Vector2.zero;
        myRightButtonState= ButtonStateEnum.Idle;        
        myLeftButtonState= ButtonStateEnum.Idle;        
        myMiddleButtonState= ButtonStateEnum.Idle;        
        RightButtonDownTime= 0;
        LeftButtonDownTime= 0;
        MiddleButtonDownTime= 0;
        RightButtonUpTime= 0;
        LeftButtonUpTime= 0;
        MiddleButtonUpTime= 0;
    }
    
	// ----------------------------------------------------------------------
	// Returns true if mouse position is inside this Window
	public bool IsInWindow() {
		return EditorWindow.mouseOverWindow == myEditorWindow;
	}
	
	// ----------------------------------------------------------------------
	// Determines the current mouse position.
	public void Update() {
        // Only update if mouse is in our window.
		if(!IsInWindow()) {
            IsRightButtonDown= IsLeftButtonDown= IsMiddleButtonDown= false;
		    return;
	    }

        // Update mouse position.
		Position= Event.current.mousePosition;
		if(Event.current.type == EventType.MouseDrag)
			Position+= Event.current.delta;

        // Update button status.
        Event e = Event.current;
        if(e.type == EventType.MouseDown) {
            switch(e.button) {
                case 0: // Left button clik
                    if(!IsLeftButtonDown) {
                        LeftButtonDownPosition= Position;
                        LeftButtonDownTime= EditorApplication.timeSinceStartup;
                        LeftButtonUpTime= LeftButtonDownTime;
                    }
                    IsLeftButtonDown= true;
                    break;
                case 1: // Right button click
                    if(!IsRightButtonDown) {
                        RightButtonDownPosition= Position;
                        RightButtonDownTime= EditorApplication.timeSinceStartup;
                        RightButtonUpTime= RightButtonDownTime;
                    }
                    IsRightButtonDown= true;
                    break;
                case 2: // Middle button click
                    if(!IsMiddleButtonDown) {
                        MiddleButtonDownPosition= Position;
                        MiddleButtonDownTime= EditorApplication.timeSinceStartup;
                        MiddleButtonUpTime= MiddleButtonDownTime;
                    }
                    IsMiddleButtonDown= true;
                    break;
                default:
                    break;
            }
        }
        if(e.type == EventType.MouseUp) {
            switch(e.button) {
                case 0: // Left button clik
                    if(IsLeftButtonDown) {
                        LeftButtonUpPosition= Position;
                        LeftButtonUpTime= EditorApplication.timeSinceStartup;
                    }
                    if(myLeftButtonState != ButtonStateEnum.Dragging && LeftButtonUpTime-LeftButtonDownTime < ClickTime) {
                        myLeftButtonState= ButtonStateEnum.SingleClick;
                    }
                    else {
                        myLeftButtonState= ButtonStateEnum.Idle;
                    }
                    IsLeftButtonDown= false;
                    break;
                case 1: // Right button click
                    if(IsRightButtonDown) {
                        RightButtonUpPosition= Position;
                        RightButtonUpTime= EditorApplication.timeSinceStartup;
                    }
                    if(myRightButtonState != ButtonStateEnum.Dragging && RightButtonUpTime-RightButtonDownTime < ClickTime) {
                        myRightButtonState= ButtonStateEnum.SingleClick;
                    }
                    else {
                        myRightButtonState= ButtonStateEnum.Idle;
                    }
                    IsRightButtonDown= false;
                    break;
                case 2: // Middle button click
                    if(IsMiddleButtonDown) {
                        MiddleButtonUpPosition= Position;
                        MiddleButtonUpTime= EditorApplication.timeSinceStartup;
                    }
                    if(myMiddleButtonState != ButtonStateEnum.Dragging && MiddleButtonUpTime-MiddleButtonDownTime < ClickTime) {
                        myMiddleButtonState= ButtonStateEnum.SingleClick;
                    }
                    else {
                        myMiddleButtonState= ButtonStateEnum.Idle;
                    }
                    IsMiddleButtonDown= false;
                    break;
                default:
                    break;
            }
        }

        // Determine dragging state.
        if(IsLeftButtonDown) {
            if(EditorApplication.timeSinceStartup-LeftButtonDownTime > ClickTime) {
                myLeftButtonState= ButtonStateEnum.Dragging;
            }
            if(Vector2.Distance(Position,LeftButtonDownPosition) > DragDistance) {
                myLeftButtonState= ButtonStateEnum.Dragging;
            }
        }
        if(IsRightButtonDown) {
            if(EditorApplication.timeSinceStartup-RightButtonDownTime > ClickTime) {
                myRightButtonState= ButtonStateEnum.Dragging;
            }
            if(Vector2.Distance(Position,RightButtonDownPosition) > DragDistance) {
                myRightButtonState= ButtonStateEnum.Dragging;
            }            
        }
        if(IsMiddleButtonDown) {
            if(EditorApplication.timeSinceStartup-MiddleButtonDownTime > ClickTime) {
                myMiddleButtonState= ButtonStateEnum.Dragging;
            }
            if(Vector2.Distance(Position,MiddleButtonDownPosition) > DragDistance) {
                myMiddleButtonState= ButtonStateEnum.Dragging;
            }            
        }
	}

    
    // ======================================================================
    // PROPERTIES
    // ======================================================================
    public  enum ButtonStateEnum { Idle, Dragging, SingleClick, DoubleClick };

	public  Vector2         Position= Vector2.zero;
    public  bool            IsRightButtonDown= false;
    public  Vector2         RightButtonUpPosition= Vector2.zero;
    public  Vector2         RightButtonDownPosition= Vector2.zero;
    public  bool            IsLeftButtonDown= false;
    public  Vector2         LeftButtonUpPosition= Vector2.zero;
    public  Vector2         LeftButtonDownPosition= Vector2.zero;
    public  bool            IsMiddleButtonDown= false;
    public  Vector2         MiddleButtonUpPosition= Vector2.zero;
    public  Vector2         MiddleButtonDownPosition= Vector2.zero;

    private double          RightButtonDownTime= 0;
    private double          RightButtonUpTime= 0;
    private double          LeftButtonDownTime= 0;
    private double          LeftButtonUpTime= 0;
    private double          MiddleButtonDownTime= 0;
    private double          MiddleButtonUpTime= 0;

    private EditorWindow    myEditorWindow;
    private float           DragDistance= 10.0f;
    private float           ClickTime= 0.75f;    

    public  ButtonStateEnum RightButtonState {
        get {
            if(myRightButtonState == ButtonStateEnum.SingleClick) {
                myRightButtonState= ButtonStateEnum.Idle;
                return ButtonStateEnum.SingleClick;
            }
            return myRightButtonState;
        }
    }
    private ButtonStateEnum myRightButtonState= ButtonStateEnum.Idle;
    public  ButtonStateEnum LeftButtonState {
        get {
            if(myLeftButtonState == ButtonStateEnum.SingleClick) {
                myLeftButtonState= ButtonStateEnum.Idle;
                return ButtonStateEnum.SingleClick;
            }
            return myLeftButtonState;
        }
    }
    private ButtonStateEnum myLeftButtonState= ButtonStateEnum.Idle;
    public  ButtonStateEnum MiddleButtonState {
        get {
            if(myMiddleButtonState == ButtonStateEnum.SingleClick) {
                myMiddleButtonState= ButtonStateEnum.Idle;
                return ButtonStateEnum.SingleClick;
            }
            return myMiddleButtonState;
        }
    }
    private ButtonStateEnum myMiddleButtonState= ButtonStateEnum.Idle;
    
}
