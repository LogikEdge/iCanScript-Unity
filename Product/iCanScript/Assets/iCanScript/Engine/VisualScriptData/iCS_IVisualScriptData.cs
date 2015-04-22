using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface iCS_IVisualScriptData {
	// Editor Interface
    string                  TypeName                { get; set; }
    bool                    OverrideDefaultBaseType { get; set; }
    string                  BaseTypeName            { get; set; }
    string                  SourceFileGUID          { get; set; }
    int                     DisplayRoot             { get; set; }
    int                     SelectedObject          { get; set; }
    Vector2                 SelectedObjectPosition  { get; set; }
    bool                    ShowDisplayRootNode     { get; set; }
    float                   GuiScale                { get; set; }
    Vector2					ScrollPosition		    { get; set; }
    iCS_NavigationHistory   NavigationHistory       { get; }

    // Engine Interface
    int                     MajorVersion        { get; set; }
    int                     MinorVersion        { get; set; }
    int                     BugFixVersion       { get; set; }
    List<iCS_EngineObject>  EngineObjects       { get; }
    int                     UndoRedoId          { get; set; } 
}
