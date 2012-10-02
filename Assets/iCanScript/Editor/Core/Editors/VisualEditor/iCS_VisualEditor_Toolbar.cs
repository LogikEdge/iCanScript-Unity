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

//        // Adjust toolbar styles
//		Vector2 test= EditorStyles.toolbar.contentOffset;
//		test.y=3f;
//		EditorStyles.toolbar.contentOffset= test;
//		test= EditorStyles.toolbarTextField.contentOffset;
//		test.y=2f;
//		EditorStyles.toolbarTextField.contentOffset= test;

		// Show zoom control at the end of the toolbar.
        float newScale= iCS_ToolbarUtility.Slider(ref r, 120f, Scale, 2f, 0.15f, spacer, spacer, true);
        iCS_ToolbarUtility.Label(ref r, new GUIContent("Zoom"), 0, 0, true);
		if(Math3D.IsNotEqual(newScale, Scale)) {
            Vector2 pivot= ViewportToGraph(ViewportCenter);
            CenterAtWithScale(pivot, newScale);
		}

		// Show current bookmark.
		string bookmarkString= "myBookmark: ";
		if(myBookmark == null) {
		    bookmarkString+= "(empty)";
		} else {
		    bookmarkString+= myBookmark.Name;
		}
		iCS_ToolbarUtility.Label(ref r, 150f, new GUIContent(bookmarkString),0,0,true);
	}
}
