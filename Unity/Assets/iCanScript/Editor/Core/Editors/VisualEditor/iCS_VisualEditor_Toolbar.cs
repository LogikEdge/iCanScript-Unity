using UnityEngine;
using UnityEditor;
using System.Collections;
using Prefs= iCS_PreferencesController;

/*
    TODO: Should show frameId in header bar.
*/
public partial class iCS_VisualEditor : iCS_EditorBase {
    // =======================================================================
    // Toolbar Constants
    // -----------------------------------------------------------------------
    const float kSliderSize= 60f;
    
    // -----------------------------------------------------------------------
	void Toolbar() {
		// No toolbar if editor snapshot without background requested.
		if(iCS_DevToolsConfig.framesWithoutBackground != 0) return;
		
		// Build standard toolbar at top of editor window.
		Rect r= iCS_ToolbarUtility.BuildToolbar(position.width, -1f);

		// Insert an initial spacer.
		float spacer= 8f;
		r.x+= spacer;
		r.width-= spacer;

		// --------------
		// LEFT TOOLBAR
		// Show Runtime frame id.
        bool showFrameId= Prefs.ShowRuntimeFrameId;
        bool newShowFrameId= iCS_ToolbarUtility.Toggle(ref r, showFrameId, spacer, spacer);
        if(newShowFrameId != showFrameId) {
            Prefs.ShowRuntimeFrameId= newShowFrameId;
        }
        iCS_ToolbarUtility.MiniLabel(ref r, "Frame Id", 0,0);
        iCS_ToolbarUtility.Separator(ref r);
		
        // Show Runtime values.
        bool showRuntime= Prefs.ShowRuntimePortValue;
        bool newShowRuntime= iCS_ToolbarUtility.Toggle(ref r, showRuntime, spacer, spacer);
        if(newShowRuntime != showRuntime) {
            Prefs.ShowRuntimePortValue= newShowRuntime;
        }
        iCS_ToolbarUtility.MiniLabel(ref r, "Runtime Values", 0,0);
        float refreshSpeed= Mathf.Sqrt(Prefs.PortValueRefreshPeriod);
        float newRefreshSpeed= iCS_ToolbarUtility.Slider(ref r, kSliderSize, refreshSpeed, 0.3162f, 1.414f, spacer, spacer);
        if(newRefreshSpeed != refreshSpeed) {
            Prefs.PortValueRefreshPeriod= newRefreshSpeed*newRefreshSpeed;
        }
        iCS_ToolbarUtility.Separator(ref r);
		
		// --------------
		// RIGHT TOOLBAR
		// Show zoom control at the end of the toolbar.
        float newScale= iCS_ToolbarUtility.Slider(ref r, kSliderSize, Scale, 2f, 0.15f, spacer, spacer, true);
        iCS_ToolbarUtility.MiniLabel(ref r, "Zoom", 10f, 0, true);
		if(Math3D.IsNotEqual(newScale, Scale)) {
            Vector2 pivot= ViewportToGraph(ViewportCenter);
            CenterAtWithScale(pivot, newScale);
		}
		iCS_ToolbarUtility.Separator(ref r, true);

		// Show current bookmark.
		string bookmarkString= "Bookmark: ";
		if(myBookmark == null) {
		    bookmarkString+= "(empty)";
		} else {
		    bookmarkString+= myBookmark.Name;
		}
		iCS_ToolbarUtility.MiniLabel(ref r, 150f, bookmarkString, spacer, 0, true);
        iCS_ToolbarUtility.Separator(ref r, true);

		// --------------
		// CENTER TOOLBAR
        // Show game object name in middle of toolbar.
		var name= IStorage.Storage.name;
		if(Application.isPlaying) {
			var visualScript= IStorage.Storage as iCS_VisualScriptImp;
			if(visualScript != null) {
				name+= " (id= "+visualScript.UpdateFrameId;
				if(Math3D.IsNotZero(Time.smoothDeltaTime)) {
					int frameRate= (int)(1f/Time.smoothDeltaTime);
					name+="; fr= "+frameRate;
				}
				name+=")";				
			}
		}
		iCS_ToolbarUtility.CenteredTitle(ref r, name);
	}
}
