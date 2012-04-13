using UnityEngine;

public interface DSIView {
    void        Display(Rect frameArea);
    Vector2     GetSizeToDisplay(Rect displayArea);
}
