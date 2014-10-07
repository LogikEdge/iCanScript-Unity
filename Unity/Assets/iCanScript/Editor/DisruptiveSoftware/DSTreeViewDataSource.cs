using UnityEngine;
using System.Collections;

public interface DSTreeViewDataSource {
	void	Reset();
	void    BeginDisplay();
	void    EndDisplay();
	bool	MoveToNext();
	bool	MoveToNextSibling();
	bool	MoveToFirstChild();
	bool	MoveToParent();
	Vector2	CurrentObjectLayoutSize();
	bool	DisplayCurrentObject(Rect displayArea, bool foldout, Rect frameArea);
	object	CurrentObjectKey();
	void    MouseDownOn(object key, Vector2 mouseInScreenPoint, Rect screenArea);
	void    MouseMove(object key);
	void	KeyDown(object key, KeyCode keyCode);
}
