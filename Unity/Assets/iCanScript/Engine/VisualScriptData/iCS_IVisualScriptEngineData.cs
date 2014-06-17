using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ===========================================================================
// This interface is used to access the core data of a visual script.
public interface iCS_IVisualScriptEngineData {
    string                  HostName      { get; set; }
    uint                    MajorVersion  { get; set; }
    uint                    MinorVersion  { get; set; }
    uint                    BugFixVersion { get; set; }
    List<iCS_EngineObject>  EngineObjects { get; }
    List<Object>            UnityObjects  { get; }
    int                     UndoRedoId    { get; set; }    
}
