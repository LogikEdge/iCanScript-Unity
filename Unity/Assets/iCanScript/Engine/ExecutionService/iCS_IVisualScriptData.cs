using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ===========================================================================
// This interface is used to access the core data of a visual script.
public interface iCS_IVisualScriptData {
    string                  GetHostName();
    uint                    GetMajorVersion();
    uint                    GetMinorVersion();
    uint                    GetBugFixVersion();
    void                    SetMajorVersion(uint v);
    void                    SetMinorVersion(uint v);
    void                    SetBugFixVersion(uint v);
    List<iCS_EngineObject>  GetEngineObjects();
    List<Object>            GetUnityObjects();
}
