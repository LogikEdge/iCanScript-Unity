using UnityEngine;
using UnityEditor;
using System.Collections;
using P=Prelude;
using TS=iCS_TimerService;

public partial class iCS_VisualEditor {
	// =======================================================================
	// Fields
	// -----------------------------------------------------------------------
	static Texture2D		ourErrorIcon        = null;
	static Texture2D		ourWarningIcon      = null;
	static Texture2D		ourSmallErrorIcon	= null;
	static Texture2D		ourSmallWarningIcon = null;
	const  float			kMargins            = 4f;
	static P.Animate<float>	ourAlphaAnimation   = new P.Animate<float>();
		   TS.TimedAction	ourRefreshAction    = null;
	static bool				showErrorDetails    = false;
		   TS.TimedAction	showErrorDetailTimer= null;
	
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
				iCS_TextureCache.GetIcon(iCS_EditorStrings.ErrorIcon, out ourErrorIcon);
				iCS_TextureCache.GetIcon(iCS_EditorStrings.ErrorSmallIcon, out ourSmallErrorIcon);
			}
			// Show the error icon
			if(ourErrorIcon != null) {
				var r= new Rect(kMargins, position.height-kMargins-48f, 48f, 48f);
				GUI.color= new Color(1f,1f,1f, ourAlphaAnimation.CurrentValue);
				GUI.DrawTexture(r, ourErrorIcon, ScaleMode.ScaleToFit);
				GUI.color= Color.white;
				
				if(r.Contains(WindowMousePosition)) {
					showErrorDetails= true;
					if(showErrorDetailTimer == null) {
						showErrorDetailTimer= new TS.TimedAction(1f, ()=> showErrorDetails= false);
						showErrorDetailTimer.Schedule();
					}
					else {
						showErrorDetailTimer.Restart();
					}
				}
				if(showErrorDetails) {
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
						var content= new GUIContent(e.Message, ourSmallErrorIcon);
						GUI.Button(new Rect(0,y, r.width, r.height), content/*"-> "+e.Message*/, style);
						y+= 16;
					}
					foreach(var e in iCS_ErrorController.Warnings) {
						var content= new GUIContent(e.Message, ourSmallWarningIcon);
						GUI.Button(new Rect(0,y, r.width, r.height), content/*"-> "+e.Message*/, style);
						y+= 16;
					}
					GUI.EndScrollView();
					GUI.color= Color.white;
				}

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
				iCS_TextureCache.GetIcon(iCS_EditorStrings.WarningIcon, out ourWarningIcon);
				iCS_TextureCache.GetIcon(iCS_EditorStrings.WarningSmallIcon, out ourSmallWarningIcon);
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
