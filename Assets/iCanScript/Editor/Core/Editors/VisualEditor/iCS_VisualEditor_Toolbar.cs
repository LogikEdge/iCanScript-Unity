using UnityEngine;
using UnityEditor;
using System.Collections;

/*
    TODO: Should show frameId in header bar.
*/
public partial class iCS_VisualEditor : iCS_EditorBase {
	void Toolbar() {
		// Build standard toolbar at top of editor window.
		Rect r= iCS_ToolbarUtility.BuildToolbar(position.width, -1f);

		// Insert an initial spacer.
		float spacer= 8f;
		r.x+= spacer;
		r.width-= spacer;

        // Show Runtime values.
        bool showRuntime= iCS_PreferencesEditor.ShowRuntimePortValue;
        bool newShowRuntime= iCS_ToolbarUtility.Toggle(ref r, showRuntime, spacer, spacer);
        if(newShowRuntime != showRuntime) {
            Debug.Log("Changing runtime to: "+showRuntime);
            iCS_PreferencesEditor.ShowRuntimePortValue= newShowRuntime;
        }
        iCS_ToolbarUtility.MiniLabel(ref r, "Runtime Values", 0,0);
        float refreshSpeed= iCS_PreferencesEditor.PortValueRefreshPeriod;
        float newRefreshSpeed= iCS_ToolbarUtility.Slider(ref r, 120f, refreshSpeed, 0.1f, 2f, spacer, spacer);
        if(newRefreshSpeed != refreshSpeed) {
            iCS_PreferencesEditor.PortValueRefreshPeriod= newRefreshSpeed;
        }
        iCS_ToolbarUtility.Separator(ref r);
        
		// Show zoom control at the end of the toolbar.
        float newScale= iCS_ToolbarUtility.Slider(ref r, 120f, Scale, 2f, 0.15f, spacer, spacer, true);
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
	}
}
