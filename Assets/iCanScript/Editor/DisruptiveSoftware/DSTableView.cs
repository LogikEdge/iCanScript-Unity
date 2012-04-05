using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class DSTableView : DSViewWithTitle {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    Vector2                 myScrollPosition         = Vector2.zero;
    DSTableViewDataSource   myDataSource             = null;
	List<DSTableColumn>		myColumns				 = new List<DSTableColumn>();
	float[]				    myRowHeights			 = new float[0];
	Rect					myColumnsDisplayDataArea;
	Rect					myColumnsTotalDataArea;
	
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public DSTableViewDataSource DataSource {
        get { return myDataSource; }
        set { myDataSource= value; RecomputeColumnAreas(); }
    }
	
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSTableView(GUIContent title, TextAlignment titleAlignment, bool titleSeperator,
                       RectOffset margins, bool shouldDisplayFrame= true)
        : base(title, titleAlignment, titleSeperator, margins, shouldDisplayFrame) {}
    
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
    public override void ReloadData() {
		RecomputeColumnAreas();
	}
	
    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    public override void Display() {
        // Duisplay bounding box and title.
        base.Display();
        if(myDataSource == null) return;
        
        // Display frame and title of each column.
        for(int i= 0; i < myColumns.Count; ++i) {
            if(Math3D.IsNotZero(myColumns[i].DisplayArea.width)) {
                myColumns[i].Display(myColumns[i].FrameArea);
            }
        }

        // Show column data.
		Rect scrollDisplayArea= myColumnsDisplayDataArea;
		if(scrollDisplayArea.xMax < BodyArea.xMax) scrollDisplayArea.xMax= BodyArea.xMax;
		if(scrollDisplayArea.yMax < BodyArea.yMax) scrollDisplayArea.yMax= BodyArea.yMax;
        myScrollPosition= GUI.BeginScrollView(scrollDisplayArea, myScrollPosition, myColumnsTotalDataArea, false, false);
		{
	        float y= myColumnsTotalDataArea.y;
	        for(int row= 0; row < myRowHeights.Length; ++row) {
	            foreach(var column in myColumns) {
	                Vector2 dataSize= myDataSource.DisplaySizeForObjectInTableView(this, column, row);
	                Rect displayRect= new Rect(0, y, dataSize.x, dataSize.y);
	                switch(column.TitleAlignment) {
	                    case TextAlignment.Left: {
	                        displayRect.x= column.DisplayArea.x;
	                        break;
	                    }
	                    case TextAlignment.Right: {
	                        displayRect.x= column.DisplayArea.xMax-dataSize.x;
	                        break;
	                    }
	                    case TextAlignment.Center:
	                    default: {
	                        displayRect.x= column.DisplayArea.x+0.5f*(column.DisplayArea.width-dataSize.x);
	                        break;
	                    }
	                }
		            myDataSource.DisplayObjectInTableView(this, column, row, displayRect);					
	            }
	            y+= myRowHeights[row];
	        }
		}
        GUI.EndScrollView();
    }

    // ======================================================================
    // View dimension change notification.
    // ----------------------------------------------------------------------
    protected override void OnViewAreaChange() {
		base.OnViewAreaChange();
		RecomputeColumnAreas();
	}
    protected override Vector2 GetMinimumFrameSize() {
        var baseMinFameSize= base.GetMinimumFrameSize();
        float height= baseMinFameSize.y;
        if(myColumns.Count >= 1) {
            height+= myColumns[0].MinimumFrameSize.y;
        }
        if(myRowHeights.Length >= 1) {
            float maxRowHeight= 0;
            foreach(var rh in myRowHeights) {
                if(rh > maxRowHeight) maxRowHeight= rh;
            }
            height+= maxRowHeight;
        }
        if(myRowHeights.Length >= 2) {
            height+= kScrollerSize;
        }
        return new Vector2(baseMinFameSize.x, height);
    }
	
	void RecomputeColumnAreas() {
		// Clear previous column information.
		myColumns.Clear();
		myRowHeights= new float[0];
		if(myDataSource == null) return;
		
		// Collect table columns & recompute data size.
		int nbOfRows= myDataSource.NumberOfRowsInTableView(this);
		myRowHeights= new float[nbOfRows]; for(int row= 0; row < nbOfRows; ++row) myRowHeights[row]= 0f;
	    ForEachSubview(
	        subview=> {
	            DSTableColumn tableColumn= subview as DSTableColumn;
	            if(tableColumn != null) {
	                myColumns.Add(tableColumn);
					float maxCellWidth= 0f;
					for(int row= 0; row < nbOfRows; ++row) {
						var cellSize= myDataSource.DisplaySizeForObjectInTableView(this, tableColumn, row);
						if(cellSize.x > maxCellWidth) maxCellWidth= cellSize.x;
						if(cellSize.y > myRowHeights[row]) myRowHeights[row]= cellSize.y;
					}
					tableColumn.DataSize= new Vector2(maxCellWidth, 0);
	            }
	        }
	    );
		float dataHeight= 0; foreach(var height in myRowHeights) { dataHeight+= height; }
		foreach(var column in myColumns) { var tmp= column.DataSize; tmp.y= dataHeight; column.DataSize= tmp; }

		// Recompute column frame area.
		Rect remainingArea= BodyArea;
		for(int i= 0; i < myColumns.Count; ++i) {
			var columnFullFrameSize= myColumns[i].FullFrameSize;
			float width= Mathf.Min(columnFullFrameSize.x, remainingArea.width);
			float height= Mathf.Min(columnFullFrameSize.y, remainingArea.height);
			myColumns[i].FrameArea= new Rect(remainingArea.x, remainingArea.y, width, height);
			remainingArea.x+= width; remainingArea.width-= width;
		}
		if(Math3D.IsNotZero(remainingArea.width) && myColumns.Count > 0) {
			var lastColumnFrameArea= myColumns[myColumns.Count-1].FrameArea;
			lastColumnFrameArea.width+= remainingArea.width;
			myColumns[myColumns.Count-1].FrameArea= lastColumnFrameArea;
		}
		
		// Recompute total columns data area.
		myColumnsTotalDataArea= BodyArea;
		myColumnsTotalDataArea.width= 0f;
		myColumnsTotalDataArea.height= 0f;
        for(int i= 0; i < myColumns.Count; ++i) {
			var columnFullFrameSize= myColumns[i].FullFrameSize;
			myColumnsTotalDataArea.width+= columnFullFrameSize.x;
			if(myColumnsTotalDataArea.height < columnFullFrameSize.y) {
				myColumnsTotalDataArea.height= columnFullFrameSize.y;
			}
        }
		for(int i= 0; i < myColumns.Count; ++i) {
			if(myColumns[i].BodyArea.y > myColumnsTotalDataArea.y) {
				float tmp= myColumnsTotalDataArea.yMax;
				myColumnsTotalDataArea.y= myColumns[i].BodyArea.y;
				myColumnsTotalDataArea.yMax= tmp;
			}
		}

		// Recompute columns display area.
		myColumnsDisplayDataArea= Math3D.Intersection(BodyArea, myColumnsTotalDataArea);
	}
	
	// ======================================================================
    // View method overrides
    // ----------------------------------------------------------------------
	protected override bool GetHasHorizontalScroller() { return FrameArea.width < FullFrameSize.x; }
    protected override bool GetHasVerticalScroller()   { return FrameArea.height < FullFrameSize.y; }
    protected override Vector2 GetFullFrameSize() {
		Vector2 columnsSize= Vector2.zero;
		foreach(var c in myColumns) {
			Vector2 fullColumn= c.FullFrameSize;
			columnsSize.x+= fullColumn.x;
			if(fullColumn.y > columnsSize.y) columnsSize.y= fullColumn.y;
		}
		return new Vector2(Margins.horizontal+columnsSize.x,
			               Margins.vertical+TitleArea.height+columnsSize.y);        
    }
    
	// ======================================================================
    // Subview management
    // ----------------------------------------------------------------------
    public override void AddSubview(DSView subview) {
		base.AddSubview(subview);
		RecomputeColumnAreas();
    }
    public override bool RemoveSubview(DSView subview) {
		bool result= base.RemoveSubview(subview);
		RecomputeColumnAreas();
		return result;
    }
    
}
