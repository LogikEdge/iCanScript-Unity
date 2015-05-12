using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Internal.Editor;
using P=iCanScript.Internal.Prelude;
using EC=iCanScript.Internal.Editor.ErrorController;
using TimedAction= iCanScript.Internal.Prelude.TimerService.TimedAction;

namespace iCanScript.Internal.Editor {
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
        // -- Get errors/warnings for this visual script --
        var errors  = ErrorController.GetErrorsFor(VisualScript);
        var warnings= ErrorController.GetWarningsFor(VisualScript);	
        // -- Filter out invalid objects --
        errors  = P.filter(e=> IStorage.IsValid(e.ObjectId), errors);
        warnings= P.filter(w=> IStorage.IsValid(w.ObjectId), warnings);	
		// -- Return if no errors or warnings --
		if((errors.Count + warnings.Count) == 0) return;
        // -- Show scene errors/warnings --
        DisplayVisualScriptErrorsAndWarnings(errors, warnings);
        // -- Show errors/warnings on the nodes of our visual script --
        DisplayObjectErrorsAndWarnings(errors, warnings);
    }
    
	// -----------------------------------------------------------------------
	void DisplayVisualScriptErrorsAndWarnings(List<ErrorWarning> errors, List<ErrorWarning> warnings) {
        // -- Get error/warning information --
		var nbOfErrors  = errors.Count;
        var nbOfWarnings= warnings.Count;

        // -- Display scene error/warning icon --
        var r= GetVisualScriptErrorWarningIconRect();
        var icon= nbOfErrors != 0 ? ErrorController.ErrorIcon : ErrorController.WarningIcon;
        ErrorController.DisplayErrorOrWarningIconWithAlpha(r, icon);
        
        // -- Initiate the display of the scene error/warning details when mouse is over the icon --
		if(r.Contains(WindowMousePosition)) {
			showErrorDetails= true;
			if(showErrorDetailTimer == null) {
				showErrorDetailTimer= TimerService.CreateTimedAction(1f, ()=> { showErrorDetails= false; });
				showErrorDetailTimer.Schedule();
			}
			else {
				showErrorDetailTimer.Restart();
			}
		}

        // -- Display scene errors/warnings --
		if(showErrorDetails) {
            var nbOfMessages= Mathf.Min(10, nbOfErrors+nbOfWarnings);
            r= ErrorController.DetermineErrorDetailRect(position, r, nbOfMessages, true);
			if(r.Contains(WindowMousePosition)) {
				showErrorDetailTimer.Restart();
			}
            ErrorController.DisplayErrorAndWarningDetails(r, errors, warnings);
		}
	}

	// -----------------------------------------------------------------------
    void DisplayObjectErrorsAndWarnings(List<ErrorWarning> errors, List<ErrorWarning> warnings) {
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
            DisplayErrorsAndWarningAt(r, nodeErrors, nodeWarnings);
        }
    }

	// -----------------------------------------------------------------------
    Rect GetVisualScriptErrorWarningIconRect() {
		var helpBoxWidth= iCS_EditorConfig.kHelpBoxWidth;
		return new Rect(helpBoxWidth+kMargins, position.height-kMargins-48f, 48f, 48f);
    }
	// -----------------------------------------------------------------------
    /// Displays an error/warning icon with the details inside a toolbox.
    ///
    /// @param rect The rectangle in which to display the error/warning icon.
    /// @param errors List of errors
    /// @param warnings List of warnings
    ///
    void DisplayErrorsAndWarningAt(Rect rect, List<ErrorWarning> errors, List<ErrorWarning> warnings) {
        // -- Display the appropriate error/warning icon --
        var icon= errors.Count != 0 ? ErrorController.ErrorIcon : ErrorController.WarningIcon;
        ErrorController.DisplayErrorOrWarningIconWithAlpha(rect, icon);
        // -- Display error/warning details --
        if(rect.Contains(WindowMousePosition)) {
            var nbOfMessages= Mathf.Min(5, errors.Count + warnings.Count);
            var detailRect= ErrorController.DetermineErrorDetailRect(position, rect, nbOfMessages);
            ErrorController.DisplayErrorAndWarningDetails(detailRect, errors, warnings);
        }
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