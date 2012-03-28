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
    float 					myColumnContentHeight= 0f;

    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public DSTableViewDataSource DataSource {
        get { return myDataSource; }
        set { myDataSource= value; }
    }

    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const float   kScrollerSize = 16f;
    
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
        tableColumn.DataSize= new Vector2(contentWidth, tableColumn.DataSize.y);
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

        // Determine content width of each column.
		List<Rect> columnContentAreas= new List<Rect>();
		float columnX= BodyArea.x;
        for(int i= 0; i < columns.Count; ++i) {
            DSTableColumn tableColumn= columns[i];
            float columnWidth= tableColumn.DataSize.x+tableColumn.Margins.horizontal;
			columnContentAreas.Add(new Rect(columnX, BodyArea.y, columnWidth, myColumnContentHeight));
			columnX+= columnWidth;
        }

        // Determine display width of each column.
        List<Rect> columnDisplayAreas= new List<Rect>();
        float remainingWidth= BodyArea.width;
        for(int i= 0; i < columns.Count; ++i) {
            if(Math3D.IsGreater(remainingWidth,0f)) {
				Rect columnArea= Math3D.Intersection(BodyArea, columnContentAreas[i]);
				remainingWidth-= columnArea.width;
                columnDisplayAreas.Add(columnArea);
            } else {
                columnDisplayAreas.Add(new Rect(0,0,0,0));
            }
        }
        if(remainingWidth > kScrollerSize) {
            Rect tmp= columnDisplayAreas[columnDisplayAreas.Count-1];
            tmp.width+= remainingWidth-kScrollerSize;
            columnDisplayAreas[columnDisplayAreas.Count-1]= tmp;
        }
                
        // Show each column & compute combined columns data display area.
		Rect columnsDisplayPosition= BodyArea;
        for(int i= 0; i < columns.Count; ++i) {
            if(Math3D.IsNotZero(columnDisplayAreas[i].width)) {
                columns[i].Display(columnDisplayAreas[i]);
				if(columns[i].BodyArea.y > columnsDisplayPosition.y) {
					columnsDisplayPosition.y= columns[i].BodyArea.y;
					columnsDisplayPosition.yMax= BodyArea.yMax;
				}
            }
        }

        // Show column content.
        if(myDataSource == null) return;
		Rect columnsContentArea= columnsDisplayPosition;
		if(columnsContentArea.height < myColumnContentHeight) columnsContentArea.height= myColumnContentHeight;
		myColumnContentHeight= 0f;
        myScrollPosition= GUI.BeginScrollView(columnsDisplayPosition, myScrollPosition, columnsContentArea, false, false);
		{
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
		            myDataSource.DisplayObjectInTableView(this, column, row, displayRect);					
	                if(displayRect.height > maxHeight) maxHeight= displayRect.height;
	            }
	            y+= maxHeight;
				myColumnContentHeight+= maxHeight;
	        }
		}
        GUI.EndScrollView();
    }

    // ======================================================================
    // Display Utilities
    // ----------------------------------------------------------------------
}
