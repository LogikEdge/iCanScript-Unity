using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Editor;
using P=Prelude;
using EC=iCanScript.Editor.ErrorController;
using TimedAction= Prelude.TimerService.TimedAction;

namespace iCanScript.Editor {
public partial class iCS_VisualEditor {
	// =======================================================================
	// Fields
	// -----------------------------------------------------------------------
	const  float			kMargins            = 4f;
		   TimedAction	    ourErrorRepaintTimer= null;
	static bool				showErrorDetails    = false;
		   TimedAction	    showErrorDetailTimer= null;	
	
	// =======================================================================
    // Errors/Warning display functionality
	// -----------------------------------------------------------------------
    void DisplayErrorsAndWarnings() {
        // -- Nothing to display if no error/warning exists --
        if(!ErrorController.IsErrorOrWarning) return;
        // -- Update the repaint timer --
        UpdateErrorRepaintTimer();
        // -- Show scene errors/warnings --
        DisplaySceneErrorsAndWarnings();
        // -- Show errors/warnings on the nodes of our visual script --
        DisplayVisualScriptErrorsAndWarnings();
    }
    
	// -----------------------------------------------------------------------
	void DisplaySceneErrorsAndWarnings() {
		// -- Nothing to show if no errors/warnings detected --
        if(!ErrorController.IsErrorOrWarning) return;
		
        // -- Display scene error/warning icon --
		var nbOfErrors  = ErrorController.NumberOfErrors;
        var nbOfWarnings= ErrorController.NumberOfWarnings;
        var r= GetSceneErrorWarningIconRect();
        var icon= nbOfErrors != 0 ? ErrorController.ErrorIcon : ErrorController.WarningIcon;
        DisplayErrorOrWarningIconWithAlpha(r, icon);
        
        // -- Initiate the display of the scene error/warning details when mouse is over the icon --
		if(r.Contains(WindowMousePosition)) {
			showErrorDetails= true;
			if(showErrorDetailTimer == null) {
				showErrorDetailTimer= TimerService.CreateTimedAction(1f, ()=> { showErrorDetails= false; IsHelpEnabled= true; });
				showErrorDetailTimer.Schedule();
			}
			else {
				showErrorDetailTimer.Restart();
			}
		}

        // -- Display scene errors/warnings --
		if(showErrorDetails) {
			// -- Remove help viewport --
			IsHelpEnabled= false;
			
            var nbOfMessages= Mathf.Min(10, nbOfErrors+nbOfWarnings);
            r= DetermineErrorDetailRect(r, nbOfMessages, true);
			if(r.Contains(WindowMousePosition)) {
				showErrorDetailTimer.Restart();
			}
            DisplayErrorAndWarningDetails(r, ErrorController.Errors, ErrorController.Warnings);
		}
	}

	// -----------------------------------------------------------------------
    void DisplayVisualScriptErrorsAndWarnings() {
        // -- Get errors/warnings for this visual script --
        var errors  = ErrorController.GetErrorsFor(VisualScript);
        var warnings= ErrorController.GetWarningsFor(VisualScript);

        // -- Filter out invalid objects --
        errors  = P.filter(e=> IStorage.IsValid(e.ObjectId), errors);
        warnings= P.filter(w=> IStorage.IsValid(w.ObjectId), warnings);

        // -- Determine wich objects have errors or warnings --
        var objectIds= P.append(P.map(e=> e.ObjectId, errors), P.map(w=> w.ObjectId, warnings));
        objectIds= P.removeDuplicates(objectIds);

        // -- Display the errors/warnings on the objects in the graph --
        foreach(var id in objectIds) {
            // -- Determine if node is visible --
            var node= IStorage[id];
            if(!DisplayRoot.IsParentOf(node)) continue;
            var pos= node.GlobalPosition;
            if(!VisibleGraphRect.Contains(pos)) continue;
            // -- Determine errors/warnings for this particular node --
            var nodeErrors  = P.filter(e=> e.ObjectId == id, errors);
            var nodeWarnings= P.filter(w=> w.ObjectId == id, warnings);
            // -- Display the appropriate error/warning icon --
            var r= Math3D.BuildRectCenteredAt(pos, 32f, 32f);
            r= myGraphics.TranslateAndScale(r);
            var icon= nodeErrors.Count != 0 ? ErrorController.ErrorIcon : ErrorController.WarningIcon;
            DisplayErrorOrWarningIconWithAlpha(r, icon);
            // -- Display error/warning details --
            if(r.Contains(WindowMousePosition)) {
                var nbOfMessages= Mathf.Min(5, nodeErrors.Count + nodeWarnings.Count);
                var detailRect= DetermineErrorDetailRect(r, nbOfMessages);
                DisplayErrorAndWarningDetails(detailRect, nodeErrors, nodeWarnings);
            }
        }
    }

	// -----------------------------------------------------------------------
    Rect GetSceneErrorWarningIconRect() {
		return new Rect(kMargins, position.height-kMargins-48f, 48f, 48f);
    }
	// -----------------------------------------------------------------------
    void DisplayErrorOrWarningIconWithAlpha(Rect r, Texture2D icon) {
        var savedColor= GUI.color;
		GUI.color= ErrorController.BlendColor;
		GUI.DrawTexture(r, icon, ScaleMode.ScaleToFit);
		GUI.color= savedColor;
    }
	// -----------------------------------------------------------------------
    void DisplayErrorAndWarningDetails(Rect r, List<ErrorController.ErrorWarning> errors, List<ErrorController.ErrorWarning> warnings) {
        // -- Draw background box --
        var savedColor= GUI.color;
        var outlineRect= new Rect(r.x-2, r.y-2, r.width+4, r.height+4);
        GUI.color= errors.Count != 0 ? Color.red : Color.yellow;
        GUI.Box(outlineRect,"");
		GUI.color= Color.black;
		GUI.Box(r,"");
		GUI.color= savedColor;

        // -- Define error/warning detail style --
		GUIStyle style= EditorStyles.whiteLabel;
		style.richText= true;
        
        // -- Show Error first than Warnings --
		float y= 0;
		GUI.BeginScrollView(r, Vector2.zero, new Rect(0,0,r.width,r.height));
        var content= new GUIContent("", ErrorController.SmallErrorIcon);
		foreach(var e in errors) {
			content.text= e.Message;
			GUI.Label(new Rect(0, y, r.width, r.height), content, style);
			y+= 16;
		}
        content.image= ErrorController.SmallWarningIcon;
		foreach(var w in warnings) {
			content.text= w.Message;
			GUI.Label(new Rect(0, y, r.width, r.height), content, style);
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
        if(!ErrorController.IsErrorOrWarning) {
            if(ourErrorRepaintTimer != null) {
                ourErrorRepaintTimer.Stop();                
            }
        }
        else {
            if(ourErrorRepaintTimer == null) {
                ourErrorRepaintTimer= TimerService.CreateTimedAction(0.06f, Repaint, /*isLooping=*/true);
            }
            else if(ourErrorRepaintTimer.IsElapsed) {
                ourErrorRepaintTimer.Restart();
            }
        }
    }
}
}