using UnityEngine;
using System.Collections;

public interface DSTreeViewDataSource {
	void	Reset();
	bool	MoveToNext();
	bool	MoveToNextSibling();
	bool	MoveToFirstChild();
	Vector2	CurrentObjectDisplaySize();
	void	DisplayCurrentObject();
}
