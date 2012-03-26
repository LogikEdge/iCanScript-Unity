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
    public DSTableView(GUIContent title, TextAlignment titleAlignment, bool titleSeperator,
                       RectOffset margins, Rect viewArea)
        : base(title, titleAlignment, titleSeperator, margins, viewArea) {}
    public DSTableView(GUIContent title, TextAlignment titleAlignment, bool titleSeperator,
                       RectOffset margins)
        : this(title, titleAlignment, titleSeperator, margins, new Rect(0,0,0,0)) {}
    
    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    public override void Display() {
        // Duisplay bounding box and title.
        base.Display();
        
        // Determine width of each column.
        ForEachSubview(
            subview=> {
                
            }
        );
        
    }

    // ======================================================================
    // Display Utilities
    // ----------------------------------------------------------------------
}
