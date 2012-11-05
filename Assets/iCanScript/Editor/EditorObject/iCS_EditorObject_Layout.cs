using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // Node layout attributes -----------------------------------------------
    public bool IsUnfolded                  { get { return EngineObject.IsUnfolded; }}
    public bool IsFolded                    { get { return EngineObject.IsFolded; }}
    public bool IsIconized                  { get { return EngineObject.IsIconized; }}
    public void Unfold()                    { EngineObject.Unfold(); }
    public void Fold()                      { EngineObject.Fold(); }
    public void Iconize()                   { EngineObject.Iconize(); }
    
}
