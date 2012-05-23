using UnityEngine;
using System.Collections;

public interface DSTreeViewDataSource {
	void	Reset();
	bool	MoveToNext();
	bool	MoveToNextSibling();
	bool	MoveToFirstChild();
	bool	MoveToParent();
	Vector2	CurrentObjectDisplaySize();
	bool	DisplayCurrentObject(Rect displayArea, bool foldout);
	object	CurrentObjectKey();
	void    MouseDownOn(object key, Rect screenPos);
}
