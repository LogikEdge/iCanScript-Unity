using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class DSTableView : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    DSTitleView     		myMainView       = null;
	DSScrollView			myColumnTitleView= null;
	DSScrollView			myColumnDataView = null;
    DSTableViewDataSource	myDataSource     = null;
	List<DSTableColumn>		myColumns		 = new List<DSTableColumn>();
	float[]				    myRowHeights	 = new float[0];
	Vector2					myColumnTitleSize;
	Vector2					myColumnDataSize;
	GUIStyle				myColumnTitleGUIStyle= null;

    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public DSTableViewDataSource DataSource {
        get { return myDataSource; }
        set { myDataSource= value; RecomputeColumnAreas(); }
    }
	public GUIStyle ColumnTitleGUIStyle {
		get { return myColumnTitleGUIStyle ?? EditorStyles.label; }
		set { myColumnTitleGUIStyle= value; }
	}
	
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSTableView(RectOffset margins, bool shouldDisplayFrame,
                       GUIContent title, AnchorEnum titleAlignment, bool titleSeperator) {
        myMainView= new DSTitleView(margins, shouldDisplayFrame, title, titleAlignment, titleSeperator);
		myColumnTitleView= new DSScrollView(new RectOffset(0,0,0,0), true, DisplayColumnTitles, GetColumnTitleSize);
		myMainView.SetSubview(myColumnTitleView);
		myColumnDataView= new DSScrollView(new RectOffset(0,0,0,0), true, DisplayColumnData, GetColumnDataSize);
    }
    
    // ======================================================================
    // DSView implementation.
    // ----------------------------------------------------------------------
    public override void Display(Rect frameArea) {
		RecomputeColumnAreas();
        myMainView.Display(frameArea);
    }
    public override Vector2 GetSizeToDisplay(Rect frameArea) {
        return myMainView.GetSizeToDisplay(frameArea);
    }
    public override AnchorEnum GetAnchor() {
        return myMainView.Anchor;
    }
    public override void SetAnchor(AnchorEnum anchor) {
        myMainView.Anchor= anchor;
    }

    // ======================================================================
    // Column Title View implementation.
    // ----------------------------------------------------------------------
    void DisplayColumnTitles(DSScrollView view, Rect displayArea) {
		Rect titleArea= displayArea;
		titleArea.height= Mathf.Min(myColumnTitleSize.y, displayArea.height);
		foreach(var column in myColumns) {
			Rect columnTitleArea= titleArea;
			columnTitleArea.width= column.DataSize.x;
			if(column.Title != null) {
				Rect titleDisplayArea= DSCellView.PerformAlignment(columnTitleArea, ColumnTitleGUIStyle.CalcSize(column.Title), column.Anchor);
				GUI.Label(titleDisplayArea, column.Title, ColumnTitleGUIStyle);				
			}
			titleArea.x+= column.DataSize.x;
			titleArea.width-= column.DataSize.x;
		}
    }
    Vector2 GetColumnTitleSize(DSScrollView view, Rect displayArea) {
		float width= myColumnTitleSize.x;
		float height= myColumnTitleSize.y+myColumnDataSize.y;
		if(height > displayArea.height) height= displayArea.height;
		if(width > displayArea.width && height >= displayArea.height-kScrollerSize) {
			height= displayArea.height-kScrollerSize;
		}
        return new Vector2(width, height);
    }

    // ======================================================================
    // Column Data View implementation.
    // ----------------------------------------------------------------------
	void DisplayColumnData(DSScrollView view, Rect displayArea) {
	}
	Vector2 GetColumnDataSize(DSScrollView view, Rect displayArea) {
		return myColumnDataSize;
	}
	
    // ======================================================================
    // Column Methods
    // ----------------------------------------------------------------------
    public DSTableColumn FindTableColumn(string identifier) {
        DSTableColumn result= null;
		foreach(var tableColumn in myColumns) {
            if(string.Compare(tableColumn.Identifier, identifier) == 0) {
                result= tableColumn;
				break;
            }			
		}
        return result;
    }
	
//    // ======================================================================
//    // Display
//    // ----------------------------------------------------------------------
//    public override void Display(DSView parent, Rect frameArea) {
//        // Duisplay bounding box and title.
//        base.Display(parent, frameArea);
//        if(myDataSource == null) return;
//        
//        // Display frame and title of each column.
//        for(int i= 0; i < myColumns.Count; ++i) {
//            if(Math3D.IsNotZero(myColumns[i].DisplayArea.width)) {
//                myColumns[i].Display(myColumns[i].FrameArea);
//            }
//        }
//
//        // Show column data.
//		Rect scrollDisplayArea= myColumnsDisplayDataArea;
//		if(scrollDisplayArea.xMax < BodyArea.xMax) scrollDisplayArea.xMax= BodyArea.xMax;
//		if(scrollDisplayArea.yMax < BodyArea.yMax) scrollDisplayArea.yMax= BodyArea.yMax;
//        myScrollPosition= GUI.BeginScrollView(scrollDisplayArea, myScrollPosition, myColumnsTotalDataArea, false, false);
//		{
//	        float y= myColumnsTotalDataArea.y;
//	        for(int row= 0; row < myRowHeights.Length; ++row) {
//	            foreach(var column in myColumns) {
//	                Vector2 dataSize= myDataSource.DisplaySizeForObjectInTableView(this, column, row);
//	                Rect displayRect= new Rect(0, y, dataSize.x, dataSize.y);
//	                switch(column.TitleAlignment) {
//	                    case TextAlignment.Left: {
//	                        displayRect.x= column.DisplayArea.x;
//	                        break;
//	                    }
//	                    case TextAlignment.Right: {
//	                        displayRect.x= column.DisplayArea.xMax-dataSize.x;
//	                        break;
//	                    }
//	                    case TextAlignment.Center:
//	                    default: {
//	                        displayRect.x= column.DisplayArea.x+0.5f*(column.DisplayArea.width-dataSize.x);
//	                        break;
//	                    }
//	                }
//		            myDataSource.DisplayObjectInTableView(this, column, row, displayRect);					
//	            }
//	            y+= myRowHeights[row];
//	        }
//		}
//        GUI.EndScrollView();
//    }
//

    // ----------------------------------------------------------------------
	void RecomputeColumnAreas() {
		// Clear previous column information.
		myRowHeights= new float[0];
		if(myDataSource == null) return;
		
		// Recompute column data size.
		float maxTitleHeight= 0;
		float dataWidth= 0;
		int nbOfRows= myDataSource.NumberOfRowsInTableView(this);
		myRowHeights= new float[nbOfRows]; for(int row= 0; row < nbOfRows; ++row) myRowHeights[row]= 0f;
		foreach(var tableColumn in myColumns) {
			// Determine title area height.
			var titleSize= tableColumn.Title != null ? ColumnTitleGUIStyle.CalcSize(tableColumn.Title) : Vector2.zero;
			if(titleSize.y > maxTitleHeight) maxTitleHeight= titleSize.y;
			// Determine column data area.
			float maxCellWidth= titleSize.x;
			for(int row= 0; row < nbOfRows; ++row) {
				var cellSize= myDataSource.DisplaySizeForObjectInTableView(this, tableColumn, row);
				if(cellSize.x > maxCellWidth) maxCellWidth= cellSize.x;
				if(cellSize.y > myRowHeights[row]) myRowHeights[row]= cellSize.y;
			}
			maxCellWidth+= tableColumn.Margins.horizontal;
			dataWidth+= maxCellWidth;
			tableColumn.DataSize= new Vector2(maxCellWidth, 0);			
		}
		float dataHeight= 0; foreach(var height in myRowHeights) { dataHeight+= height; }
		foreach(var column in myColumns) { var tmp= column.DataSize; tmp.y= dataHeight; column.DataSize= tmp; }
		myColumnDataSize= new Vector2(dataWidth, dataHeight);
		myColumnTitleSize= new Vector2(dataWidth, maxTitleHeight);
	}
	
	// ======================================================================
    // Column management
    // ----------------------------------------------------------------------
    public void AddColumn(DSTableColumn column) {
		myColumns.Add(column);
    }
    public bool RemoveColumn(DSTableColumn column) {
		bool result= myColumns.Remove(column);
		return result;
    }
    
}
