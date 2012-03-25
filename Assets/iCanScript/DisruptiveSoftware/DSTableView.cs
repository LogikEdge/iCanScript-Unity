using UnityEngine;
using UnityEditor;
using System.Collections;

public class DSTableView : DSViewWithTitle {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSTableView(Rect viewArea, GUIContent title, bool titleSeperator= false)
        : base(viewArea, title, titleSeperator) {}
    public DSTableView(GUIContent title, bool titleSeperator= false)
        : this(new Rect(0,0,0,0), title, titleSeperator) {}
    public DSTableView()
        : this(new GUIContent("title")) {}
    
    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    public void Display(Rect viewArea) { base.Display(viewArea); }
    public void Display()              { base.Display(); }

    // ======================================================================
    // Display Utilities
    // ----------------------------------------------------------------------
}
