using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface iCS_IVisualScriptData {
	// Editor Interface
    int                     DisplayRoot            { get; set; }
    int                     SelectedObject         { get; set; }
    Vector2                 SelectedObjectPosition { get; set; }
    bool                    ShowDisplayRootNode    { get; set; }
    float                   GuiScale               { get; set; }
    Vector2					ScrollPosition		   { get; set; }
    iCS_NavigationHistory   NavigationHistory      { get; }

    // Engine Interface
    int                     MajorVersion        { get; set; }
    int                     MinorVersion        { get; set; }
    int                     BugFixVersion       { get; set; }
    List<iCS_EngineObject>  EngineObjects       { get; }
    List<Object>            UnityObjects        { get; }
    int                     UndoRedoId          { get; set; } 
}
