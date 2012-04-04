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
	Vector2						myMinimumContentSize;
	Vector2						myMaximumContentSize;
	
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
		set { mySearchFieldGUIStyle= value; CalculateContentSize(); }
	}
	
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSSearchView(Action<DSSearchView,string> searchAction,
                        RectOffset margins, bool shouldDisplayFrame= false)
     : base(margins, shouldDisplayFrame) {
		mySearchAction= searchAction;
		CalculateContentSize();
    }
    void CalculateContentSize() {
		myMinimumContentSize= (mySearchFieldGUIStyle ?? GUI.skin.textField).CalcSize(new GUIContent("1234567"));
		myMaximumContentSize= (mySearchFieldGUIStyle ?? GUI.skin.textField).CalcSize(new GUIContent("12345678901234"));
	}

    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    public override void Display() {
		if(mySearchFieldGUIStyle == null) {
			mySearchString= GUI.TextField(DisplayArea, mySearchString);			
		} else {
			mySearchString= GUI.TextField(DisplayArea, mySearchString, mySearchFieldGUIStyle);
		}
		if(GUI.changed) {
			if(mySearchAction != null) mySearchAction(this, mySearchString);
		}
    }
    protected override Vector2 GetMinimumFrameSize() {
        return MarginsSize + myMinimumContentSize;
    }
    protected override Vector2 GetFullFrameSize() {
        return MarginsSize + myMaximumContentSize;
    }
}
