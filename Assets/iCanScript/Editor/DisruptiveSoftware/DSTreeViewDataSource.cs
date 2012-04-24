using UnityEngine;
using System.Collections;

public interface DSTreeViewDataSource {
    int     NumberOfRowsInTreeView(DSTreeView treeView);
	int		GetObjectParentInTreeView(DSTreeView treeView, int row);
    Vector2 DisplaySizeForObjectInTreeView(DSTreeView treeView, int row);
    void    DisplayObjectInTreeView(DSTableView tableView, int row, Rect postion);
	void	OnMouseDown(DSTreeView treeView, int row);
}
