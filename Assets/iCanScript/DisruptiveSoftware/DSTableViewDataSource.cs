using UnityEngine;
using System.Collections;

public interface DSTableViewDataSource {
    int     NumberOfRowsInTableView(DSTableView tableView);
    float   WidthOfColumnInTableView(DSTableView tableView, DSTableColumn tableColumn);
    Vector2 DisplaySizeForObjectInTableView(DSTableView tableView, DSTableColumn tableColumn, int row);
    void    DisplayObjectInTableView(DSTableView tableView, DSTableColumn tableColumn, int row, Rect postion);
}
