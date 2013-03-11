using UnityEngine;
using System.Collections;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  OBJECT TYPE
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // Object Type Queries ---------------------------------------------------
    public bool IsPort                      { get { return EngineObject.IsPort; }}
    public bool IsDataPort                  { get { return EngineObject.IsDataPort; }}
    public bool IsInDataPort                { get { return EngineObject.IsInDataPort; }}
    public bool IsOutDataPort               { get { return EngineObject.IsOutDataPort; }}
    public bool IsModulePort                { get { return EngineObject.IsModulePort; }}
    public bool IsDynamicModulePort         { get { return EngineObject.IsDynamicModulePort; }}
    public bool IsStatePort                 { get { return EngineObject.IsStatePort; }}
    public bool IsInStatePort               { get { return EngineObject.IsInStatePort; }}
    public bool IsOutStatePort              { get { return EngineObject.IsOutStatePort; }}
    public bool IsTransitionPort            { get { return EngineObject.IsTransitionPort; }}
    public bool IsInTransitionPort          { get { return EngineObject.IsInTransitionPort; }}
    public bool IsOutTransitionPort         { get { return EngineObject.IsOutTransitionPort; }}
    public bool IsInputPort                 { get { return EngineObject.IsInputPort; }}
    public bool IsOutputPort                { get { return EngineObject.IsOutputPort; }}
    public bool IsClassModule               { get { return EngineObject.IsClassModule; }}
    public bool IsTransitionModule          { get { return EngineObject.IsTransitionModule; }}
    public bool IsTransitionGuard           { get { return EngineObject.IsTransitionGuard; }}
    public bool IsTransitionAction          { get { return EngineObject.IsTransitionAction; }}
    public bool IsNode                      { get { return EngineObject.IsNode; }}
    public bool IsModule                    { get { return EngineObject.IsModule; }}
    public bool IsFunction                  { get { return EngineObject.IsFunction; }}
    public bool IsBehaviour                 { get { return EngineObject.IsBehaviour; }}
    public bool IsConstructor               { get { return EngineObject.IsConstructor; }}
    public bool IsTypeCast                  { get { return EngineObject.IsTypeCast; }}
    public bool IsStateChart                { get { return EngineObject.IsStateChart; }}
    public bool IsState                     { get { return EngineObject.IsState; }}
    public bool IsEnablePort                { get { return EngineObject.IsEnablePort; }}
    public bool IsMuxPort                   { get { return EngineObject.IsMuxPort; }}
    public bool IsInMuxPort                 { get { return EngineObject.IsInMuxPort; }}
    public bool IsOutMuxPort                { get { return EngineObject.IsOutMuxPort; }}
}
