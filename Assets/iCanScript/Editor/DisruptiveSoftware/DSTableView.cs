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
//    Vector2                 myScrollPosition         = Vector2.zero;
//	Rect					myColumnsDisplayDataArea;
//	Rect					myColumnsTotalDataArea;
//	
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public DSTableViewDataSource DataSource {
        get { return myDataSource; }
        set { myDataSource= value; RecomputeColumnAreas(); }
    }
	public GUIStyle ColumnTitleGUIStyle {
		get { return myColumnTitleGUIStyle; }
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
    }
    Vector2 GetColumnTitleSize(DSScrollView view, Rect displayArea) {
        return Vector2.zero;
    }

    // ======================================================================
    // Column Data View implementation.
    // ----------------------------------------------------------------------
	void DisplayColumnData(DSScrollView view, Rect displayArea) {
	}
	Vector2 GetColumnDataSize(DSScrollView view, Rect displayArea) {
		return Vector2.zero;
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
			var titleSize= ColumnTitleGUIStyle.CalcSize(tableColumn.Title);
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
		
//		// Recompute column frame area.
//		Rect remainingArea= BodyArea;
//		for(int i= 0; i < myColumns.Count; ++i) {
//			var columnFullFrameSize= myColumns[i].FullFrameSize;
//			float width= Mathf.Min(columnFullFrameSize.x, remainingArea.width);
//			float height= Mathf.Min(columnFullFrameSize.y, remainingArea.height);
//			myColumns[i].FrameArea= new Rect(remainingArea.x, remainingArea.y, width, height);
//			remainingArea.x+= width; remainingArea.width-= width;
//		}
//		if(Math3D.IsNotZero(remainingArea.width) && myColumns.Count > 0) {
//			var lastColumnFrameArea= myColumns[myColumns.Count-1].FrameArea;
//			lastColumnFrameArea.width+= remainingArea.width;
//			myColumns[myColumns.Count-1].FrameArea= lastColumnFrameArea;
//		}
//		
//		// Recompute total columns data area.
//		myColumnsTotalDataArea= BodyArea;
//		myColumnsTotalDataArea.width= 0f;
//		myColumnsTotalDataArea.height= 0f;
//        for(int i= 0; i < myColumns.Count; ++i) {
//			var columnFullFrameSize= myColumns[i].FullFrameSize;
//			myColumnsTotalDataArea.width+= columnFullFrameSize.x;
//			if(myColumnsTotalDataArea.height < columnFullFrameSize.y) {
//				myColumnsTotalDataArea.height= columnFullFrameSize.y;
//			}
//        }
//		for(int i= 0; i < myColumns.Count; ++i) {
//			if(myColumns[i].BodyArea.y > myColumnsTotalDataArea.y) {
//				float tmp= myColumnsTotalDataArea.yMax;
//				myColumnsTotalDataArea.y= myColumns[i].BodyArea.y;
//				myColumnsTotalDataArea.yMax= tmp;
//			}
//		}
//
//		// Recompute columns display area.
//		myColumnsDisplayDataArea= Math3D.Intersection(BodyArea, myColumnsTotalDataArea);
	}
	
//	// ======================================================================
//    // View method overrides
//    // ----------------------------------------------------------------------
//	protected bool GetHasHorizontalScroller() { return FrameArea.width < FullFrameSize.x; }
//    protected bool GetHasVerticalScroller()   { return FrameArea.height < FullFrameSize.y; }
//    protected Vector2 GetFullFrameSize() {
//		Vector2 columnsSize= Vector2.zero;
//		foreach(var c in myColumns) {
//			Vector2 fullColumn= c.FullFrameSize;
//			columnsSize.x+= fullColumn.x;
//			if(fullColumn.y > columnsSize.y) columnsSize.y= fullColumn.y;
//		}
//		return new Vector2(Margins.horizontal+columnsSize.x,
//			               Margins.vertical+TitleArea.height+columnsSize.y);        
//    }
//    
//	// ======================================================================
//    // Subview management
//    // ----------------------------------------------------------------------
//    public override void AddSubview(DSView subview) {
//		base.AddSubview(subview);
//		RecomputeColumnAreas();
//    }
//    public override bool RemoveSubview(DSView subview) {
//		bool result= base.RemoveSubview(subview);
//		RecomputeColumnAreas();
//		return result;
//    }
//    
}
