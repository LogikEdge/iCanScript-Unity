using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;
using TS=iCS_TimerService;
using EC=iCS_ErrorController;

public partial class iCS_VisualEditor {
	// =======================================================================
	// Fields
	// -----------------------------------------------------------------------
	const  float			kMargins            = 4f;
		   TS.TimedAction	ourErrorRepaintTimer= null;
	static bool				showErrorDetails    = false;
		   TS.TimedAction	showErrorDetailTimer= null;	
	
	// =======================================================================
    // Errors/Warning display functionality
	// -----------------------------------------------------------------------
    void DisplayErrorsAndWarnings() {
        // Nothing to display if no error/warning exists.
        if(!iCS_ErrorController.IsErrorOrWarning) return;
        // Update the repaint timer
        UpdateErrorRepaintTimer();
        // Show scene errors/warnings
        DisplaySceneErrorsAndWarnings();
        // Show errors/warnings on the nodes of our visual script
        DisplayVisualScriptErrorsAndWarnings();
    }
    
	// -----------------------------------------------------------------------
	void DisplaySceneErrorsAndWarnings() {
		// Nothing to show if no errors/warnings detected.
        if(!iCS_ErrorController.IsErrorOrWarning) return;
		
        // Display scene error/warning icon
		var nbOfErrors  = iCS_ErrorController.NumberOfErrors;
        var nbOfWarnings= iCS_ErrorController.NumberOfWarnings;
        var r= GetSceneErrorWarningIconRect();
        var icon= nbOfErrors != 0 ? iCS_ErrorController.ErrorIcon : iCS_ErrorController.WarningIcon;
        DisplayErrorOrWarningIconWithAlpha(r, icon);
        
        // Initiate the display of the scene error/warning details when mouse is over the icon
		if(r.Contains(WindowMousePosition)) {
			showErrorDetails= true;
			if(showErrorDetailTimer == null) {
				showErrorDetailTimer= new TS.TimedAction(1f, ()=> { showErrorDetails= false; IsHelpEnabled= true; });
				showErrorDetailTimer.Schedule();
			}
			else {
				showErrorDetailTimer.Restart();
			}
		}

        // Display scene errors/warnings
		if(showErrorDetails) {
			// Remove help viewport.
			IsHelpEnabled= false;
			
            var nbOfMessages= Mathf.Min(10, nbOfErrors+nbOfWarnings);
            r= DetermineErrorDetailRect(r, nbOfMessages, true);
			if(r.Contains(WindowMousePosition)) {
				showErrorDetailTimer.Restart();
			}
            DisplayErrorAndWarningDetails(r, iCS_ErrorController.Errors, iCS_ErrorController.Warnings);
		}
	}

	// -----------------------------------------------------------------------
    void DisplayVisualScriptErrorsAndWarnings() {
        var visibleRect= VisibleGraphRect;
        var errors= iCS_ErrorController.GetErrorsFor(VisualScript);
        var warnings= iCS_ErrorController.GetWarningsFor(VisualScript);
        var nbOfErrors= P.length(errors);
        var nbOfMessages= Mathf.Min(10,nbOfErrors+P.length(warnings));
        foreach(var w in warnings) {
            if(!IStorage.IsValid(w.ObjectId)) continue;
            var node= IStorage[w.ObjectId];
            var pos= node.GlobalPosition;
            if(!visibleRect.Contains(pos)) continue;
            var r= Math3D.BuildRectCenteredAt(pos, 32f, 32f);
            r= myGraphics.TranslateAndScale(r);
            DisplayErrorOrWarningIconWithAlpha(r, iCS_ErrorController.ErrorIcon);
        }
        foreach(var e in errors) {
            if(!IStorage.IsValid(e.ObjectId)) continue;
            var node= IStorage[e.ObjectId];
            var pos= node.GlobalPosition;
            if(!visibleRect.Contains(pos)) continue;
            var r= Math3D.BuildRectCenteredAt(pos, 32f, 32f);
            r= myGraphics.TranslateAndScale(r);
            if(r.Contains(WindowMousePosition)) {
                var detailRect= DetermineErrorDetailRect(r, nbOfMessages);
                DisplayErrorAndWarningDetails(detailRect, P.filter(er=> er.ObjectId == e.ObjectId, errors), P.filter(wa=> wa.ObjectId == e.ObjectId, warnings));
            }
            DisplayErrorOrWarningIconWithAlpha(r, iCS_ErrorController.ErrorIcon);
        }
    }

	// -----------------------------------------------------------------------
    Rect GetSceneErrorWarningIconRect() {
		return new Rect(kMargins, position.height-kMargins-48f, 48f, 48f);
    }
	// -----------------------------------------------------------------------
    void DisplayErrorOrWarningIconWithAlpha(Rect r, Texture2D icon) {
        var savedColor= GUI.color;
		GUI.color= iCS_ErrorController.BlendColor;
		GUI.DrawTexture(r, icon, ScaleMode.ScaleToFit);
		GUI.color= savedColor;
    }
	// -----------------------------------------------------------------------
    void DisplayErrorAndWarningDetails(Rect r, List<iCS_ErrorController.ErrorWarning> errors, List<iCS_ErrorController.ErrorWarning> warnings) {
        // Draw background box
        var outlineRect= new Rect(r.x-2, r.y-2, r.width+4, r.height+4);
        GUI.color= errors.Count != 0 ? Color.red : Color.yellow;
        GUI.Box(outlineRect,"");
		GUI.color= Color.black;
		GUI.Box(r,"");
		GUI.color= Color.white;

        // Define error/warning detail style.
		GUIStyle style= EditorStyles.whiteLabel;
		style.richText= true;
        
        // Show Error first than Warnings.
		float y= 0;
		GUI.BeginScrollView(r, Vector2.zero, new Rect(0,0,r.width,r.height));
        var content= new GUIContent("", iCS_ErrorController.SmallErrorIcon);
		foreach(var e in errors) {
			content.text= e.Message;
			GUI.Label(new Rect(0,y, r.width, r.height), content, style);
			y+= 16;
		}
        content.image= iCS_ErrorController.SmallWarningIcon;
		foreach(var w in warnings) {
			content.text= w.Message;
			GUI.Label(new Rect(0,y, r.width, r.height), content, style);
			y+= 16;
		}
		GUI.EndScrollView();        
    }
	// -----------------------------------------------------------------------
    Rect DetermineErrorDetailRect(Rect iconRect, int nbOfLines, bool growUpward= false) {
        var r= iconRect;
		r.x= kMargins+iconRect.xMax;
		r.width= position.width-r.x-kMargins;
        var height= 16*nbOfLines;
        if(growUpward) {
            r.y= r.yMax-height;
        }
        r.height= height;
        return r;
    }
    
    // =======================================================================
    // Utilities
	// -----------------------------------------------------------------------
    void UpdateErrorRepaintTimer() {
        if(!iCS_ErrorController.IsErrorOrWarning) {
            if(ourErrorRepaintTimer != null) {
                ourErrorRepaintTimer.Stop();                
            }
        }
        else {
            if(ourErrorRepaintTimer == null) {
                ourErrorRepaintTimer= TS.CreateTimedAction(0.06f, Repaint, /*isLooping=*/true);
            }
            else if(ourErrorRepaintTimer.IsElapsed) {
                ourErrorRepaintTimer.Restart();
            }
        }
    }
}
