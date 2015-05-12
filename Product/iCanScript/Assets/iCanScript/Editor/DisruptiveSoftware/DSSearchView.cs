using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    public sealed class DSSearchView : DSView {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
        Action<DSSearchView,string>	mySearchAction           = null;
        string 						mySearchString           = "";
    	GUIStyle					mySearchFieldGUIStyle    = null;
    	int                         myNbOfVisibleCharInSearch= 12;
        Vector2                     mySearchFieldSize        = Vector2.zero;
        DSCellView                  myCellView               = null;
    	
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
                            int nbOfVisibleCharInSearch, Action<DSSearchView,string> searchAction) {
            myCellView= new DSCellView(margins, shouldDisplayFrame, DisplaySearchField, GetSearchFieldSize);
    		mySearchAction= searchAction;
            myNbOfVisibleCharInSearch= nbOfVisibleCharInSearch;
            ComputeSearchFieldSize();
        }
        void ComputeSearchFieldSize() {
            var oneLetterSize= SearchFieldGUIStyle.CalcSize(new GUIContent("A"));
            var twoLetterSize= SearchFieldGUIStyle.CalcSize(new GUIContent("AA"));
            float letterWidth= twoLetterSize.x-oneLetterSize.x;
            mySearchFieldSize= new Vector2(oneLetterSize.x-letterWidth+myNbOfVisibleCharInSearch*letterWidth, oneLetterSize.y);
        }
        
        // ======================================================================
        // DSView implementation.
        // ----------------------------------------------------------------------
        public override void Display(Rect frameArea) {
            myCellView.Display(frameArea);
        }
        public override Vector2 GetSizeToDisplay(Rect displayArea) {
            return myCellView.GetSizeToDisplay(displayArea);
        }
        public override AnchorEnum GetAnchor() {
            return myCellView.Anchor;
        }
        public override void SetAnchor(AnchorEnum anchor) {
            myCellView.Anchor= anchor;
        }
        
        // ======================================================================
        // DSSearchView implementation.
        // ----------------------------------------------------------------------
        void DisplaySearchField(DSView view, Rect displayArea) {
    		if(mySearchFieldGUIStyle == null) {
    			mySearchString= GUI.TextField(displayArea, mySearchString);			
    		} else {
    			mySearchString= GUI.TextField(displayArea, mySearchString, mySearchFieldGUIStyle);
    		}
    		if(GUI.changed) {
    			if(mySearchAction != null) mySearchAction(this, mySearchString);
    		}        
        }
        Vector2 GetSearchFieldSize(DSView view, Rect displayArea) {
            return mySearchFieldSize;
        }
    }
    
}
