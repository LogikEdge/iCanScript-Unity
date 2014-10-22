using UnityEngine;
using UnityEditor;
using System.Collections;
using P=Prelude;
using TS=iCS_TimerService;

public partial class iCS_VisualEditor {
	// =======================================================================
	// Fields
	// -----------------------------------------------------------------------
	const  float			kMargins            = 4f;
		   TS.TimedAction	ourRefreshAction    = null;
	static bool				showErrorDetails    = false;
		   TS.TimedAction	showErrorDetailTimer= null;	
	
	// -----------------------------------------------------------------------
	void ShowErrorsAndWarnings() {
		// Nothing to show if no errors/warnings detected.
        if(!iCS_ErrorController.IsErrorOrWarning) return;
		var nbOfErrors= iCS_ErrorController.NumberOfErrors;
		var nbOfWarnings= iCS_ErrorController.NumberOfWarnings;
		AnimateErrorAlpha();
		
		// Show errors icon
		if(nbOfErrors != 0) {
			// Show the error icon
			var r= new Rect(kMargins, position.height-kMargins-48f, 48f, 48f);
			GUI.color= iCS_ErrorController.BlendColor;
			GUI.DrawTexture(r, iCS_ErrorController.ErrorIcon, ScaleMode.ScaleToFit);
			GUI.color= Color.white;
			
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
			if(showErrorDetails) {
				// Remove help viewport.
				IsHelpEnabled= false;
				
				r.x= kMargins+r.xMax;
				r.width= position.width-r.x-kMargins;
				if(r.Contains(WindowMousePosition)) {
					showErrorDetailTimer.Restart();
				}
				GUIStyle style= EditorStyles.whiteLabel;
				style.richText= true;
				Color bgColor= Color.black;
				bgColor.a= showErrorDetailTimer.RemainingTime;
				GUI.color= bgColor;					
				GUI.Box(r,"");
				Color fgColor= Color.white;
				fgColor.a= showErrorDetailTimer.RemainingTime;
				GUI.color= fgColor;
				GUI.BeginScrollView(r, Vector2.zero, new Rect(0,0,r.width,r.height));
				float y= 0;
				EditorGUIUtility.LookLikeControls();
				foreach(var e in iCS_ErrorController.Errors) {
					var content= new GUIContent(e.Message, iCS_ErrorController.SmallErrorIcon);
					GUI.Button(new Rect(0,y, r.width, r.height), content/*"-> "+e.Message*/, style);
					y+= 16;
				}
				foreach(var e in iCS_ErrorController.Warnings) {
					var content= new GUIContent(e.Message, iCS_ErrorController.SmallWarningIcon);
					GUI.Button(new Rect(0,y, r.width, r.height), content/*"-> "+e.Message*/, style);
					y+= 16;
				}
				GUI.EndScrollView();
				GUI.color= Color.white;
			}
		}
		//Show warning icon
		if(nbOfWarnings != 0) {
			// Show the error icon
			var r= new Rect(2*kMargins+48f, position.height-kMargins-48f, 48f, 48f);
			GUI.color= iCS_ErrorController.BlendColor;
			GUI.DrawTexture(r, iCS_ErrorController.WarningIcon, ScaleMode.ScaleToFit);
			GUI.color= Color.white;
		}
	}
	
	// -----------------------------------------------------------------------
	/// Cycle the alpha animation.
	void AnimateErrorAlpha() {
		// Assure that we are being repainted at 10ms.
		if(ourRefreshAction == null) {
			ourRefreshAction= new TS.TimedAction(0.05f, ()=> SendEvent(EditorGUIUtility.CommandEvent("RepaintErrors")));
		}
		if(ourRefreshAction.IsElapsed) {
			ourRefreshAction.Restart();
		}
	}
}
