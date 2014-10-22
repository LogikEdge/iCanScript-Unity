using UnityEngine;
using UnityEditor;
using System.Collections;
using P=Prelude;
using TS=iCS_TimerService;

public partial class iCS_VisualEditor {
	// =======================================================================
	// Fields
	// -----------------------------------------------------------------------
	static Texture2D		ourErrorIcon     = null;
	static Texture2D		ourWarningIcon   = null;
	const  float			kMargins         = 4f;
	static P.Animate<float>	ourAlphaAnimation= new P.Animate<float>();
		   TS.TimedAction	ourRefreshAction = null;
	
	// -----------------------------------------------------------------------
	void ShowErrorsAndWarnings() {
		// Nothing to show if no errors/warnings detected.
		var nbOfErrors= iCS_ErrorController.NumberOfErrors;
		var nbOfWarnings= iCS_ErrorController.NumberOfWarnings;
		if(nbOfErrors == 0 && nbOfWarnings == 0) return;
		AnimateErrorAlpha();
		
		// Show errors icon
		if(nbOfErrors != 0) {
			// Load the error icon
			if(ourErrorIcon == null) {
				if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.ErrorIcon, out ourErrorIcon)) {
					ourErrorIcon= null;
				}					
			}
			// Show the error icon
			if(ourErrorIcon != null) {
//				// Build tooltip
//				var tooltip= "";
//				foreach(var e in iCS_ErrorController.Errors) {
//					tooltip+= e.Message+"\n";
//				}
//				var content= new GUIContent(ourErrorIcon, tooltip);
				var r= new Rect(kMargins, position.height-kMargins-48f, 48f, 48f);
				GUI.color= new Color(1f,1f,1f, ourAlphaAnimation.CurrentValue);
				GUI.DrawTexture(r, ourErrorIcon, ScaleMode.ScaleToFit);
				GUI.color= Color.white;

//				var nbErrorsAsStr= nbOfErrors.ToString();
//				var nbErrorsAsGUIContent= new GUIContent(nbErrorsAsStr);
//				var nbErrorsSize= EditorStyles.label.CalcSize(nbErrorsAsGUIContent);
//				r= Math3D.BuildRectCenteredAt(center, nbErrorsSize);
//				GUI.backgroundColor= Color.black;
//				GUI.Label(r, nbErrorsAsGUIContent);
			}
		}
		//Show warning icon
		if(nbOfWarnings != 0) {
			// Load the warning icon
			if(ourWarningIcon == null) {
				if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.WarningIcon, out ourWarningIcon)) {
					ourWarningIcon= null;
				}					
			}
			// Show the error icon
			if(ourWarningIcon != null) {
				var r= new Rect(2*kMargins+48f, position.height-kMargins-48f, 48f, 48f);
				GUI.color= new Color(1f,1f,1f, ourAlphaAnimation.CurrentValue);
				GUI.DrawTexture(r, ourWarningIcon, ScaleMode.ScaleToFit);
				GUI.color= Color.white;
			}
			
		}
	}
	
	// -----------------------------------------------------------------------
	/// Cycle the alpha animation.
	void AnimateErrorAlpha() {
		// Restart the alpha animation.
		if(ourAlphaAnimation.IsElapsed) {
			ourAlphaAnimation.Start(0f, 2f, 2f, (start,end,ratio)=> Mathf.Min(1f, Mathf.Abs(1f-Math3D.Lerp(start,end,ratio))));
		}
		// Update the alpha animation
		ourAlphaAnimation.Update();

		// Assure that we are being repainted at 10ms.
		if(ourRefreshAction == null) {
			ourRefreshAction= new TS.TimedAction(0.05f, ()=> SendEvent(EditorGUIUtility.CommandEvent("RepaintErrors")));
		}
		if(ourRefreshAction.IsElapsed) {
			ourRefreshAction.Restart();
		}
	}
}
