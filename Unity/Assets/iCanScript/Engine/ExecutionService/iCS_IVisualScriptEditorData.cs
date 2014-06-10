using UnityEngine;
using System.Collections;

// ===========================================================================
// Interface to the persisted visual editor data.
public interface iCS_IVisualScriptEditorData {

    int     GetDisplayRoot();
    void    SetDisplayRoot(int id);
    int     GetSelectedObject();
    void    SetSelectedObject(int id);
    bool    GetShowDisplayRootNode();
    void    SetShowDisplayRootNode(bool show);
    float   GetGuiScale();
    void    SetGuiScale(float scale);
    Vector2 GetScrollPosition();
    void    SetSrcollPosition(Vector2 pos);
}
