using UnityEngine;
using System;
using System.Collections;

public class DSSearchView : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    Action<DSSearchView,string>	mySearchAction           = null;
    string 						mySearchString           = "";
	GUIStyle					mySearchFieldGUIStyle    = null;
	int                         myNbOfVisibleCharInSearch= 12;
    Vector2                     mySerachFieldSize        = Vector2.zero;
	
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
		get { return mySearchFieldGUIStyle ?? GUI.skin.textField; }
		set { mySearchFieldGUIStyle= value; ComputeSearchFieldSize(); }
	}
	
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSSearchView(RectOffset margins, bool shouldDisplayFrame,
                        int nbOfVisibleCharInSearch, Action<DSSearchView,string> searchAction)
     : base(margins, shouldDisplayFrame) {
		mySearchAction= searchAction;
        myNbOfVisibleCharInSearch= nbOfVisibleCharInSearch;
        ComputeSearchFieldSize();
    }
    void ComputeSearchFieldSize() {
        var oneLetterSize= SearchFieldGUIStyle.CalcSize(new GUIContent("A"));
        var twoLetterSize= SearchFieldGUIStyle.CalcSize(new GUIContent("AA"));
        float letterWidth= twoLetterSize.x-oneLetterSize.x;
        mySerachFieldSize= new Vector2(oneLetterSize.x-letterWidth+myNbOfVisibleCharInSearch*letterWidth, oneLetterSize.y);
    }
    
    // ======================================================================
    // DSView overrides.
    // ----------------------------------------------------------------------
    protected override void DoDisplay(Rect displayArea) {
		if(mySearchFieldGUIStyle == null) {
			mySearchString= GUI.TextField(displayArea, mySearchString);			
		} else {
			mySearchString= GUI.TextField(displayArea, mySearchString, mySearchFieldGUIStyle);
		}
		if(GUI.changed) {
			if(mySearchAction != null) mySearchAction(this, mySearchString);
		}
    }
    protected override Vector2 DoGetDisplaySize(Rect displayArea) {
        return mySerachFieldSize;
    }
}
