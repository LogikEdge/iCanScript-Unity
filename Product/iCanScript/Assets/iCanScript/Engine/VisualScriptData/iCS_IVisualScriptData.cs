using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Engine {
    
    public interface iCS_IVisualScriptData {
    	// Editor Interface
        bool                    IsEditorScript          { get; set; }
        string                  CSharpFileName          { get; set; }
        bool                    BaseTypeOverride        { get; set; }
        string                  BaseType                { get; set; }
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
}
