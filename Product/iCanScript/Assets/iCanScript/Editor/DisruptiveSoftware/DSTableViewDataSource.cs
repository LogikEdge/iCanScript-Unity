using UnityEngine;

namespace iCanScript.Internal.Editor {

    public interface DSTableViewDataSource {
        int     NumberOfRowsInTableView(DSTableView tableView);
        Vector2 LayoutSizeForObjectInTableView(DSTableView tableView, DSTableColumn tableColumn, int row);
        void    DisplayObjectInTableView(DSTableView tableView, DSTableColumn tableColumn, int row, Rect postion);
    	void	OnMouseDown(DSTableView tableView, DSTableColumn tableColumn, int row);
    }
    
}
