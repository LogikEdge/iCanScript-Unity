using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class DSTableView : DSViewWithTitle {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    Vector2                 myScrollPosition;
    DSTableViewDataSource   myDataSource= null;
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public DSTableViewDataSource DataSource {
        get { return myDataSource; }
        set { myDataSource= value; }
    }
    
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
    // Column Methods
    // ----------------------------------------------------------------------
    public DSTableColumn FindTableColumn(string identifier) {
        DSTableColumn result= null;
        SearchInSubviews(
            subview=> {
                DSTableColumn tableColumn= subview as DSTableColumn;
                if(tableColumn != null && string.Compare(tableColumn.Identifier, identifier) == 0) {
                    result= tableColumn;
                    return true;
                }
                return false; 
            }
        );
        return result;
    }
    // ----------------------------------------------------------------------
    public void AdjustContentWidth(string columnIdentifier, float contentWidth) {
        DSTableColumn tableColumn= FindTableColumn(columnIdentifier);
        if(tableColumn == null) return;
        tableColumn.ContentWidth= contentWidth;
    }
    
    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    public override void Display() {
        // Duisplay bounding box and title.
        base.Display();
        
        // Collect all the table columns.
        List<DSTableColumn> columns= new List<DSTableColumn>();
        ForEachSubview(
            subview=> {
                DSTableColumn tableColumn= subview as DSTableColumn;
                if(tableColumn != null) {
                    columns.Add(tableColumn);
                }
            }
        );
        if(columns.Count == 0) return;

        // Determine width of each column.
        List<Rect> columnAreas= new List<Rect>();
        Rect remainingArea= BodyArea;
        for(int i= 0; i < columns.Count; ++i) {
            if(Math3D.IsGreater(remainingArea.width,0f)) {
                DSTableColumn tableColumn= columns[i];
                float columnWidth= tableColumn.ContentWidth+tableColumn.Margins.horizontal;
                columnWidth= Mathf.Min(remainingArea.width, columnWidth);
                Rect columnArea= remainingArea;
                columnArea.width= columnWidth;
                remainingArea.x+= columnWidth;
                remainingArea.width-= columnWidth;
                columnAreas.Add(columnArea);
            } else {
                columnAreas.Add(new Rect(0,0,0,0));
            }
        }
        if(remainingArea.width > 0f) {
            Rect tmp= columnAreas[columnAreas.Count-1];
            tmp.width+= remainingArea.width;
            columnAreas[columnAreas.Count-1]= tmp;
        }
                
        // Show each column.
        for(int i= 0; i < columns.Count; ++i) {
            if(Math3D.IsNotZero(columnAreas[i].width)) {
                columns[i].Display(columnAreas[i]);
            }
        }
        
        // Show column content.
        if(myDataSource == null) return;
        float y= columns[0].BodyArea.y;
        int nbOfRows= myDataSource.NumberOfRowsInTableView(this);
        for(int row= 0; row < nbOfRows; ++row) {
            float maxHeight= 0;
            foreach(var column in columns) {
                Vector2 dataSize= myDataSource.DisplaySizeForObjectInTableView(this, column, row);
                Rect displayRect= new Rect(0, y, dataSize.x, dataSize.y);
                switch(column.TitleAlignment) {
                    case TextAlignment.Left: {
                        displayRect.x= column.ContentArea.x;
                        break;
                    }
                    case TextAlignment.Right: {
                        displayRect.x= column.ContentArea.xMax-dataSize.x;
                        break;
                    }
                    case TextAlignment.Center:
                    default: {
                        displayRect.x= column.ContentArea.x+0.5f*(column.ContentArea.width-dataSize.x);
                        break;
                    }
                }
                displayRect= Math3D.Intersection(column.ContentArea, displayRect);
                myDataSource.DisplayObjectInTableView(this, column, row, displayRect);
                if(displayRect.height > maxHeight) maxHeight= displayRect.height;
            }
            y+= maxHeight;
        }
//        myScrollPosition= GUI.BeginScrollView(scrollViewVariableRect, myScrollPosition, contentVariableRect, false, true);
//        GUI.EndScrollView;
    }

    // ======================================================================
    // Display Utilities
    // ----------------------------------------------------------------------
}
