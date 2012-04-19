using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class DSTableView : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    DSTitleView     		myMainView         = null;
    Vector2                 myScrollbarPosition= Vector2.zero;
    DSTableViewDataSource	myDataSource       = null;
	List<DSTableColumn>		myColumns		   = new List<DSTableColumn>();
	float[]				    myRowHeights	   = new float[0];
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
        myMainView= new DSTitleView(margins, shouldDisplayFrame,
                                    title, titleAlignment, titleSeperator,
                                    DisplayMainView, GetMainViewDisplaySize);
    }
    
    // ======================================================================
    // DSView implementation.
    // ----------------------------------------------------------------------
    public override void Display(Rect frameArea) {
        myMainView.Display(frameArea);
    }
    public override Vector2 GetSizeToDisplay(Rect frameArea) {
		RecomputeColumnAreas();
        return myMainView.GetSizeToDisplay(frameArea);
    }
    public override AnchorEnum GetAnchor() {
        return myMainView.Anchor;
    }
    public override void SetAnchor(AnchorEnum anchor) {
        myMainView.Anchor= anchor;
    }

    // ======================================================================
    // Main View implementation.
    // ----------------------------------------------------------------------
    void DisplayMainView(DSTitleView view, Rect displayArea) {
        // Compute piece-part display areas.
        Rect titleDisplayArea= new Rect(displayArea.x, displayArea.y,
                                        Mathf.Min(displayArea.width, myColumnTitleSize.x),
                                        Mathf.Min(displayArea.height, myColumnTitleSize.y));
        Rect dataDisplayArea= new Rect(displayArea.x, titleDisplayArea.yMax,
                                       Mathf.Min(displayArea.width, myColumnDataSize.x),
                                       Mathf.Min(displayArea.height-titleDisplayArea.height, myColumnDataSize.y));
        if(dataDisplayArea.height < 0) dataDisplayArea.height= 0;
        
        // Compute scrollbar information.
        bool needHorizontalScrollbar= false;
        bool needVerticalScrollbar= false;
        float horizontalScrollbarSize= 0;
        float verticalScrollbarSize= 0;
        
        float displayWidth= displayArea.width;
        float displayHeight= displayArea.height;
        if(myColumnTitleSize.x > displayWidth) {
            needHorizontalScrollbar= true;
            horizontalScrollbarSize= (displayWidth*displayWidth)/myColumnTitleSize.x;
            dataDisplayArea.height-= kScrollbarSize;
            if(dataDisplayArea.height < 0) dataDisplayArea.height= 0;
        }
        float dataHeight= displayHeight-myColumnTitleSize.y-(needHorizontalScrollbar ? kScrollbarSize : 0);
        if(myColumnDataSize.y > dataHeight) {
            needVerticalScrollbar= true;
            verticalScrollbarSize= (dataHeight*dataHeight)/myColumnDataSize.y;
            dataDisplayArea.width-= kScrollbarSize;
            if(dataDisplayArea.width < 0) dataDisplayArea.width= 0;
        }

        // Display column titles.
        GUI.BeginGroup(titleDisplayArea);
            DisplayColumnTitles();              
        GUI.EndGroup();
        // Display column data.
        GUI.BeginGroup(dataDisplayArea);
            DisplayColumnData();
        GUI.EndGroup();

        // Display scrollbar if needed.
        if(needHorizontalScrollbar) {
            Rect scrollbarPos= new Rect(displayArea.x, displayArea.yMax-kScrollbarSize, displayArea.width, kScrollbarSize);
            myScrollbarPosition.x= GUI.HorizontalScrollbar(scrollbarPos, myScrollbarPosition.x, horizontalScrollbarSize, 0, displayWidth);
        }
        if(needVerticalScrollbar) {
            Rect scrollbarPos= new Rect(displayArea.xMax-kScrollbarSize, displayArea.y, kScrollbarSize, displayArea.height-myColumnTitleSize.y);
            myScrollbarPosition.y= GUI.VerticalScrollbar(scrollbarPos, myScrollbarPosition.y, verticalScrollbarSize, 0, dataHeight);
        }
    }
    Vector2 GetMainViewDisplaySize(DSTitleView view, Rect displayArea) {
        float width= Mathf.Max(myColumnTitleSize.x, myColumnDataSize.x);
        float height= myColumnTitleSize.y+myColumnDataSize.y;
        return new Vector2(width, height);
    }
    // ----------------------------------------------------------------------
    void DisplayColumnTitles() {
        Rect titleArea= new Rect(-myScrollbarPosition.x, 0, myColumnTitleSize.x, myColumnTitleSize.y);
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
    // ----------------------------------------------------------------------
    void DisplayColumnData() {
        Rect dataArea= new Rect(-myScrollbarPosition.x, -myScrollbarPosition.y, myColumnDataSize.x, myColumnDataSize.y);
        float y= dataArea.y;
        for(int row= 0; row < myRowHeights.Length; ++row) {
            float x= dataArea.x;
            foreach(var column in myColumns) {
                Rect displayRect= new Rect(x, y, column.DataSize.x, myRowHeights[row]);
                Vector2 dataSize= myDataSource.DisplaySizeForObjectInTableView(this, column, row);
                displayRect= DSCellView.PerformAlignment(displayRect, dataSize, column.Anchor);
	            myDataSource.DisplayObjectInTableView(this, column, row, displayRect);					
                x+= column.DataSize.x;
            }
            y+= myRowHeights[row];
        }
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
