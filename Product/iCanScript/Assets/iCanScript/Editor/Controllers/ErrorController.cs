using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Internal.Engine;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {
    
    // ======================================================================
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
    public static class ErrorController {
        // ======================================================================
        // INIT / SHUTDOWN
        // ----------------------------------------------------------------------
        static ErrorController()    	{}
        public static void Start()      {}
        public static void Shutdown()   {}
    
        // ======================================================================
        // TYPES
    	
        // ======================================================================
        // FIELDS
        // ----------------------------------------------------------------------
    	static List<ErrorWarning>	myErrors        = new List<ErrorWarning>();
    	static List<ErrorWarning>	myWarnings      = new List<ErrorWarning>();
        static Texture2D            myErrorIcon       = null;
        static Texture2D            mySmallErrorIcon  = null;
        static Texture2D            myWarningIcon     = null;
        static Texture2D            mySmallWarningIcon= null;
    	
        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
        public static bool                  IsErrorOrWarning    { get { return NumberOfErrors != 0 || NumberOfWarnings != 0; }}
    	public static int 					NumberOfErrors 		{ get { return myErrors.Count; }}
    	public static int 					NumberOfWarnings	{ get { return myWarnings.Count; }}
    	public static List<ErrorWarning>	Errors 				{ get { return myErrors; }}
    	public static List<ErrorWarning>	Warnings 			{ get { return myWarnings; }}
        public static float                 DisplayAlpha        { get { return BlinkController.NormalBlinkRatio; }}
        public static Color                 BlendColor          { get { return BlinkController.NormalBlinkColor; }}
        public static Texture2D ErrorIcon {
            get {
                if(myErrorIcon == null) {
            		TextureCache.GetIcon(iCS_EditorStrings.ErrorIcon, out myErrorIcon);
                }
                return myErrorIcon;
            }
        }
        public static Texture2D SmallErrorIcon {
            get {
                if(mySmallErrorIcon == null) {
            		TextureCache.GetIcon(iCS_EditorStrings.ErrorSmallIcon, out mySmallErrorIcon);
                }
                return mySmallErrorIcon;
            }
        }
        public static Texture2D WarningIcon {
            get {
                if(myWarningIcon == null) {
            		TextureCache.GetIcon(iCS_EditorStrings.WarningIcon, out myWarningIcon);
                }
                return myWarningIcon;
            }
        }
        public static Texture2D SmallWarningIcon {
            get {
                if(mySmallWarningIcon == null) {
            		TextureCache.GetIcon(iCS_EditorStrings.WarningSmallIcon, out mySmallWarningIcon);
                }
                return mySmallWarningIcon;
            }
        }
    
        // ======================================================================
        // OPERATIONS
        // ----------------------------------------------------------------------
    	public static void Clear(string serviceId) {
    		myErrors  = P.filter(d=> d.ServiceId != serviceId, myErrors);
    		myWarnings= P.filter(d=> d.ServiceId != serviceId, myWarnings);
    	}
    	public static void AddError(string serviceId, string message, iCS_VisualScriptImp vs, int objectId) {
    		myErrors.Add(new ErrorWarning(serviceId, message, vs, objectId));
    	}
    	public static void AddWarning(string serviceId, string message, iCS_VisualScriptImp vs, int objectId) {
    		myWarnings.Add(new ErrorWarning(serviceId, message, vs, objectId));
    	}
        public static List<ErrorWarning> GetErrorsFor(iCS_VisualScriptImp vs) {
            return P.filter(e=> e.VisualScript == vs, Errors);
        }
        public static List<ErrorWarning> GetWarningsFor(iCS_VisualScriptImp vs) {
            return P.filter(e=> e.VisualScript == vs, Warnings);
        }
		public static List<ErrorWarning> GetErrorsFor(iCS_VisualScriptImp vs, int objectId) {
            return P.filter(e=> e.VisualScript == vs && e.ObjectId == objectId, Errors);			
		}
		public static List<ErrorWarning> GetWarningsFor(iCS_VisualScriptImp vs, int objectId) {
            return P.filter(e=> e.VisualScript == vs && e.ObjectId == objectId, Warnings);			
		}
		public static List<ErrorWarning> GetErrorsFor(string serviceKey, iCS_VisualScriptImp vs, int objectId) {
            return P.filter(e=> e.ServiceId == serviceKey && e.VisualScript == vs && e.ObjectId == objectId, Errors);			
		}
		public static List<ErrorWarning> GetWarningsFor(string serviceKey, iCS_VisualScriptImp vs, int objectId) {
            return P.filter(e=> e.ServiceId == serviceKey && e.VisualScript == vs && e.ObjectId == objectId, Warnings);			
		}

        // ======================================================================
        // DISPLAY UTILITIES
    	// -----------------------------------------------------------------------
        /// Dsiplays the error/warning icon in the given rectangle with a blinking
        /// alpha channel.
        ///
        /// @param r The rectangle in which the display the error/warning icon.
        /// @param icon The icon to be displayed with the blinking alpha channel.
        ///
        public static void DisplayErrorOrWarningIconWithAlpha(Rect r, Texture2D icon) {
            var savedColor= GUI.color;
    		GUI.color= BlendColor;
    		GUI.DrawTexture(r, icon, ScaleMode.ScaleToFit);
    		GUI.color= savedColor;
        }
    	// -----------------------------------------------------------------------
        /// Determines the size of the error detail rectangle.
        ///
        /// @param position The parent window rectangle.
        /// @param iconRect The rectangle used to display the error/warning icon.
        /// @param nbOfLines The number of lines to be displayed.
        /// @param growUpward Set to _true_ for the detail box to grow upwards.
        /// @return The rectangle in which to display of the details.
        ///  
    	const float kMargins= 4f;
        public static Rect DetermineErrorDetailRect(Rect position, Rect iconRect, int nbOfLines, bool growUpward= false) {
            var r= iconRect;
            var rightWidth= position.width-iconRect.xMax-kMargins;
            var leftWidth= iconRect.x-kMargins;
            if(leftWidth > rightWidth) {
                r.x= kMargins;
                r.width= leftWidth-2*kMargins;
            }
            else {
                r.x= r.xMax+kMargins;
                r.width= rightWidth-2*kMargins;
            }
            var lineFactor= 1+((int)(640f/r.width));
            if(r.width < 500) nbOfLines*= lineFactor;
            var height= 16*nbOfLines;
            if(growUpward) {
                r.y= r.yMax-height;
            }
            r.height= height;
            return r;
        }

    	// -----------------------------------------------------------------------
        /// Display the error/warning details inside the given area.
        ///
        /// @param r The area in which to display the error/warning details.
        /// @param errors The error details.
        /// @param warnings The warning details.
        ///
        public static void DisplayErrorAndWarningDetails(Rect r, List<ErrorWarning> errors, List<ErrorWarning> warnings) {
            // -- We must have at least one item to display --
            var nbErrors  = errors   != null ? errors.Count : 0;
            var nbWarnings= warnings != null ? warnings.Count : 0;
            if(nbErrors == 0 && nbWarnings == 0) return;
            // -- convert ErrorWarning to their string representation --
            string[] errorsStrings = null;
            string[] warningStrings= null;
            if(nbErrors != 0) {
                errorsStrings= P.map(e=> e.Message, errors).ToArray();
            }
            if(nbWarnings != 0) {
                warningStrings= P.map(w=> w.Message, warnings).ToArray();
            }
            DisplayErrorAndWarningDetails(r, errorsStrings, warningStrings);
        }    

    	// -----------------------------------------------------------------------
        /// Display the error/warning details inside the given area.
        ///
        /// @param r The area in which to display the error/warning details.
        /// @param errors The error details.
        /// @param warnings The warning details.
        ///
        public static void DisplayErrorAndWarningDetails(Rect r, string[] errors, string[] warnings) {
            // -- We must have at least one item to display --
            var nbErrors  = errors   != null ? errors.Length : 0;
            var nbWarnings= warnings != null ? warnings.Length : 0;
            if(nbErrors == 0 && nbWarnings == 0) return;
            // -- Draw background box --
            var borderColor= nbErrors != 0 ? Color.red : Color.yellow;
            Draw2D.FilledBoxWithOutline(r, Color.black, borderColor, 2);

            // -- Define error/warning detail style --
    		GUIStyle style= EditorStyles.whiteLabel;
    		style.richText= true;
            style.wordWrap= true;
        
            // -- Show Error first than Warnings --
    		float y= 0;
    		GUI.BeginScrollView(r, Vector2.zero, new Rect(0,0,r.width,r.height));
            var content= new GUIContent("", ErrorController.SmallErrorIcon);
            if(nbErrors != 0) {
        		foreach(var e in errors) {
        			content.text= e;
        			GUI.Label(new Rect(0, y, r.width, r.height), content, style);
        			y+= 16;
        		}                
            }
            if(nbWarnings != 0) {
                content.image= ErrorController.SmallWarningIcon;
        		foreach(var w in warnings) {
        			content.text= w;
        			GUI.Label(new Rect(0, y, r.width, r.height), content, style);
        			y+= 16;
        		}
            }
    		GUI.EndScrollView();
        }    

    	// -----------------------------------------------------------------------
        /// Displays an error/warning icon with the details inside a toolbox.
        ///
        /// @param rect The rectangle in which to display the error/warning icon.
        /// @param errors List of errors
        /// @param warnings List of warnings
        ///
        public static void DisplayErrorsAndWarningsAt(Rect position, Rect rect, List<ErrorWarning> errors, List<ErrorWarning> warnings) {
            // -- Display the appropriate error/warning icon --
            var icon= errors.Count != 0 ? ErrorController.ErrorIcon : ErrorController.WarningIcon;
            DisplayErrorOrWarningIconWithAlpha(rect, icon);
            // -- Display error/warning details --
            var mousePosition= Event.current.mousePosition;
            if(rect.Contains(mousePosition)) {
                var nbOfMessages= Mathf.Min(5, errors.Count + warnings.Count);
                var detailRect= DetermineErrorDetailRect(position, rect, nbOfMessages);
                DisplayErrorAndWarningDetails(detailRect, errors, warnings);
            }
        }

    	// -----------------------------------------------------------------------
        /// Displays an error message.
        ///
        /// @param rect The rectangle in which to display the error/warning icon.
        /// @param errorMessage The error message.
        ///
        public static void DisplayErrorMessage(Rect position, Rect rect, string errorMessage) {
            // -- Display the appropriate error/warning icon --
            var icon= ErrorController.ErrorIcon;
            var errors= new string[1]{errorMessage};
            DisplayIconAndMessages(position, rect, icon, errors, null);
        }

    	// -----------------------------------------------------------------------
        /// Displays a warning message.
        ///
        /// @param rect The rectangle in which to display the error/warning icon.
        /// @param errorMessage The warning message.
        ///
        public static void DisplayWarningMessage(Rect position, Rect rect, string errorMessage) {
            // -- Display the appropriate error/warning icon --
            var icon= ErrorController.WarningIcon;
            var warnings= new string[1]{errorMessage};
            DisplayIconAndMessages(position, rect, icon, null, warnings);
        }

    	// -----------------------------------------------------------------------
        /// Displays an icon and message when mouse is over icon.
        ///
        /// @param position The parent window rectangle.
        /// @param iconRect The rectangle used to display the error/warning icon.
        /// @param icon The icon to be displayed.
        /// @param errors List of errors
        /// @param warnings List of warnings
        ///
        public static void DisplayIconAndMessages(Rect position, Rect rect, Texture2D icon, string[] errors, string[] warnings) {
            // -- Display the appropriate error/warning icon --
            ErrorController.DisplayErrorOrWarningIconWithAlpha(rect, icon);
            // -- Display error/warning details --
            var mousePosition= Event.current.mousePosition;
            if(rect.Contains(mousePosition)) {
                var nbOfMessages= 1;
                var detailRect= ErrorController.DetermineErrorDetailRect(position, rect, nbOfMessages);
                ErrorController.DisplayErrorAndWarningDetails(detailRect, errors, null);
            }
        }
    
    }
    
}