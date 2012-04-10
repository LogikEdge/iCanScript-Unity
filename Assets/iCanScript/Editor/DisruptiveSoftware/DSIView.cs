using UnityEngine;
using System.Collections;

public interface DSIView {
	Rect GetDisplayArea();
	void Display(Rect displayRect);
}
