using UnityEngine;
using System.Collections;

// ===========================================================================
// Interface to the persisted visual editor data.
public interface iCS_IVisualScriptEditorData {

    int                     DisplayRoot         { get; set; }
    int                     SelectedObject      { get; set; }
    bool                    ShowDisplayRootNode { get; set; }
    float                   GuiScale            { get; set; }
    Vector2					ScrollPosition		{ get; set; }
    iCS_NavigationHistory   NavigationHistory   { get; }
}
