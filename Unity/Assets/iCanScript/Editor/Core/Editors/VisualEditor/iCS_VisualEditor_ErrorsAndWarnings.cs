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
				AnimateAlpha();
				var r= new Rect(kMargins, position.height-kMargins-48f, 48f, 48f);
				GUI.color= new Color(1f,1f,1f, ourAlphaAnimation.CurrentValue);
				GUI.DrawTexture(r, ourErrorIcon, ScaleMode.ScaleToFit);
				GUI.color= Color.white;
			}
		}
		//Show warning icon
		if(nbOfWarnings != 0 || nbOfErrors != 0) {
			// Load the warning icon
			if(ourWarningIcon == null) {
				if(!iCS_TextureCache.GetIcon(iCS_EditorStrings.WarningIcon, out ourWarningIcon)) {
					ourWarningIcon= null;
				}					
			}
			// Show the error icon
			if(ourWarningIcon != null) {
				AnimateAlpha();
				var r= new Rect(2*kMargins+48f, position.height-kMargins-48f, 48f, 48f);
				GUI.color= new Color(1f,1f,1f, ourAlphaAnimation.CurrentValue);
				GUI.DrawTexture(r, ourErrorIcon, ScaleMode.ScaleToFit);
				GUI.color= Color.white;
			}
			
		}
//		// Show error/warning counts
//		var errorCountAsStr= "Errors=> "+nbOfErrors;
//		var warningCountAsStr= "Warnings=> "+nbOfWarnings;
//		var errorWarningCountAsGUIContent= new GUIContent(errorCountAsStr+" "+warningCountAsStr);
//		var errorWarningSize= EditorStyles.label.CalcSize(errorWarningCountAsGUIContent);
//		var r= new Rect(0, position.height-errorWarningSize.y, errorWarningSize.x, errorWarningSize.y);
//		GUI.Label(r, errorWarningCountAsGUIContent);		
	}
	
	// -----------------------------------------------------------------------
	/// Cycle the alpha animation.
	void AnimateAlpha() {
		// Restart the alpha animation.
		if(ourAlphaAnimation.IsElapsed) {
			ourAlphaAnimation.Start(0f, 2f, 2f, (start,end,ratio)=> Mathf.Min(1f, 0.25f+Mathf.Abs(1f-Math3D.Lerp(start,end,ratio))));
		}
		// Update the alpha animation
		ourAlphaAnimation.Update();

		// Assure that we are being repainted at 10ms.
		if(ourRefreshAction == null) {
			ourRefreshAction= new TS.TimedAction(0.1f, Repaint);
			ourRefreshAction.Schedule();
		}
		if(ourRefreshAction.IsElapsed) {
			ourRefreshAction.Restart();
		}
	}
}
