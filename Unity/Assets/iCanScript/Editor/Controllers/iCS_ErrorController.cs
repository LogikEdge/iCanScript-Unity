using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;
using TS=iCS_TimerService;

public static class iCS_ErrorController {
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    static iCS_ErrorController()    {}
    public static void Start()      {}
    public static void Shutdown()   {}

    // ======================================================================
    // Types
    // ----------------------------------------------------------------------
	public class ErrorWarning {
		string				myServiceId   = null;
		string 				myMessage     = null;
		iCS_VisualScriptImp	myVisualScript= null;
		int					myObjectId    = -1;

		public string 				ServiceId 		{ get { return myServiceId; }}
		public string 				Message			{ get { return myMessage; }}
		public iCS_VisualScriptImp	VisualScript	{ get { return myVisualScript; }}
		public int					ObjectId		{ get { return myObjectId; }}
		
		public ErrorWarning(string serviceId, string msg, iCS_VisualScriptImp vs, int objectId) {
			myServiceId= serviceId;
			myMessage= msg;
			myVisualScript= vs;
			myObjectId= objectId;
		}
	};
	
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
	static List<ErrorWarning>	myErrors        = new List<ErrorWarning>();
	static List<ErrorWarning>	myWarnings      = new List<ErrorWarning>();
    static P.Animate<float>     myAnimatedAlpha = new P.Animate<float>();
    static TS.TimedAction       myAnimationTimer= TS.CreateTimedAction(0.05f, DoAnimation, /*isLooping=*/true);
    static Color                myBlendColor    = Color.white;
    static Texture2D            myErrorIcon       = null;
    static Texture2D            mySmallErrorIcon  = null;
    static Texture2D            myWarningIcon     = null;
    static Texture2D            mySmallWarningIcon= null;
	
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public static bool                  IsErrorOrWarning    { get { return NumberOfErrors != 0 || NumberOfWarnings != 0; }}
	public static int 					NumberOfErrors 		{ get { return myErrors.Count; }}
	public static int 					NumberOfWarnings	{ get { return myWarnings.Count; }}
	public static List<ErrorWarning>	Errors 				{ get { return myErrors; }}
	public static List<ErrorWarning>	Warnings 			{ get { return myWarnings; }}
    public static float                 DisplayAlpha        { get { return myAnimatedAlpha.CurrentValue; }}
    public static Color                 BlendColor          { get { return myBlendColor; }}
    public static Texture2D ErrorIcon {
        get {
            if(myErrorIcon == null) {
        		iCS_TextureCache.GetIcon(iCS_EditorStrings.ErrorIcon, out myErrorIcon);
            }
            return myErrorIcon;
        }
    }
    public static Texture2D SmallErrorIcon {
        get {
            if(mySmallErrorIcon == null) {
        		iCS_TextureCache.GetIcon(iCS_EditorStrings.ErrorSmallIcon, out mySmallErrorIcon);
            }
            return mySmallErrorIcon;
        }
    }
    public static Texture2D WarningIcon {
        get {
            if(myWarningIcon == null) {
        		iCS_TextureCache.GetIcon(iCS_EditorStrings.WarningIcon, out myWarningIcon);
            }
            return myWarningIcon;
        }
    }
    public static Texture2D SmallWarningIcon {
        get {
            if(mySmallWarningIcon == null) {
        		iCS_TextureCache.GetIcon(iCS_EditorStrings.WarningSmallIcon, out mySmallWarningIcon);
            }
            return mySmallWarningIcon;
        }
    }

    // ======================================================================
    // Operations
    // ----------------------------------------------------------------------
	public static void Clear(string serviceId) {
		myErrors  = P.filter(d=> d.ServiceId != serviceId, myErrors);
		myWarnings= P.filter(d=> d.ServiceId != serviceId, myWarnings);
        if(!IsErrorOrWarning) {
            myAnimationTimer.Stop();
        }
	}
	public static void AddError(string serviceId, string message, iCS_VisualScriptImp vs, int objectId) {
		myErrors.Add(new ErrorWarning(serviceId, message, vs, objectId));
        iCS_TimerService.Schedule(myAnimationTimer);
	}
	public static void AddWarning(string serviceId, string message, iCS_VisualScriptImp vs, int objectId) {
		myWarnings.Add(new ErrorWarning(serviceId, message, vs, objectId));
        iCS_TimerService.Schedule(myAnimationTimer);
	}
    public static List<ErrorWarning> GetErrorsFor(iCS_VisualScriptImp vs) {
        return P.filter(e=> e.VisualScript == vs, Errors);
    }
    public static List<ErrorWarning> GetWarningsFor(iCS_VisualScriptImp vs) {
        return P.filter(e=> e.VisualScript == vs, Warnings);
    }

    // ======================================================================
    // Utilities
    // ----------------------------------------------------------------------
    static void DoAnimation() {
		// Restart the alpha animation.
		if(myAnimatedAlpha.IsElapsed) {
			myAnimatedAlpha.Start(0f, 2f, 2f, (start,end,ratio)=> Mathf.Abs(1f-Math3D.Lerp(start,end,ratio)));
		}
		// Animate the error display alpha.
		myAnimatedAlpha.Update();
        myBlendColor.a= DisplayAlpha;
    }
}
