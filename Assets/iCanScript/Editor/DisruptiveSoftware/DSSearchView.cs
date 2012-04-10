using UnityEngine;
using System;
using System.Collections;

public class DSSearchView : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    Action<DSSearchView,string>	mySearchAction       = null;
    string 						mySearchString       = "";
	GUIStyle					mySearchFieldGUIStyle= null;
	
    // ======================================================================
    // Propreties
    // ----------------------------------------------------------------------
    public Action<DSSearchView,string> SearchAction {
        get { return mySearchAction; }
        set { mySearchAction= value; }
    }
	public string SearchString {
		get { return mySearchString; }
		set { mySearchString= value ?? ""; }
	}
	public GUIStyle SearchFieldGUIStyle {
		get { return mySearchFieldGUIStyle; }
		set { mySearchFieldGUIStyle= value; }
	}
	
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSSearchView(Action<DSSearchView,string> searchAction,
                        RectOffset margins, bool shouldDisplayFrame= false)
     : base(margins, shouldDisplayFrame, SearchViewDisplay, null, GetSearchViewSize) {
		mySearchAction= searchAction;
    }

    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    void SearchViewDisplay(DSView parent, Rect displayArea) {
		if(mySearchFieldGUIStyle == null) {
			mySearchString= GUI.TextField(DisplayArea, mySearchString);			
		} else {
			mySearchString= GUI.TextField(DisplayArea, mySearchString, mySearchFieldGUIStyle);
		}
		if(GUI.changed) {
			if(mySearchAction != null) mySearchAction(this, mySearchString);
		}
    }
    // ----------------------------------------------------------------------
    Vector2 GetSearchViewSize(DSView parent) {
        return mySearchFieldGUIStyle.CalcSize(mySearchString+"AA");
    }
}
