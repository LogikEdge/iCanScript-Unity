using UnityEngine;
using System.Collections;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  OBJECT TYPE
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // Object Type Queries ---------------------------------------------------
    public bool IsNode                      { get { return EngineObject.IsNode; }}
    public bool IsKindOfModule              { get { return EngineObject.IsKindOfModule; }}
    public bool IsFunction                  { get { return EngineObject.IsFunction; }}
    public bool IsMessage                   { get { return EngineObject.IsMessage; }}
    public bool IsBehaviour                 { get { return EngineObject.IsBehaviour; }}
    public bool IsConstructor               { get { return EngineObject.IsConstructor; }}
    public bool IsTypeCast                  { get { return EngineObject.IsTypeCast; }}
    public bool IsStateChart                { get { return EngineObject.IsStateChart; }}
    public bool IsState                     { get { return EngineObject.IsState; }}
    public bool IsObjectInstance            { get { return EngineObject.IsObjectInstance; }}
    public bool IsTransitionModule          { get { return EngineObject.IsTransitionModule; }}
    public bool IsTransitionGuard           { get { return EngineObject.IsTransitionGuard; }}
    public bool IsTransitionAction          { get { return EngineObject.IsTransitionAction; }}
	public bool IsBehaviourMessage			{ get { return IsMessage && IsParentValid && Parent.IsBehaviour;}}

    public bool IsPort                      { get { return EngineObject.IsPort; }}
    public bool IsFixPort                   { get { return EngineObject.IsFixPort; }}
    public bool IsDynamicPort               { get { return EngineObject.IsDynamicPort; }}
    
    public bool IsDataPort                  { get { return EngineObject.IsDataPort; }}
    public bool IsInDataPort                { get { return EngineObject.IsInDataPort; }}
    public bool IsOutDataPort               { get { return EngineObject.IsOutDataPort; }}

    public bool IsModulePort                { get { return IsDataPort && ParentNode.IsKindOfModule; }}

    public bool IsStatePort                 { get { return EngineObject.IsStatePort; }}
    public bool IsInStatePort               { get { return EngineObject.IsInStatePort; }}
    public bool IsOutStatePort              { get { return EngineObject.IsOutStatePort; }}

    public bool IsTransitionPort            { get { return EngineObject.IsTransitionPort; }}
    public bool IsInTransitionPort          { get { return EngineObject.IsInTransitionPort; }}
    public bool IsOutTransitionPort         { get { return EngineObject.IsOutTransitionPort; }}

    public bool IsInputPort                 { get { return EngineObject.IsInputPort; }}
    public bool IsOutputPort                { get { return EngineObject.IsOutputPort; }}
    public bool IsEnablePort                { get { return EngineObject.IsEnablePort; }}

    public bool IsMuxPort                   { get { return EngineObject.IsMuxPort; }}
    public bool IsChildMuxPort              { get { return EngineObject.IsChildMuxPort; }}
    public bool IsParentMuxPort             { get { return EngineObject.IsParentMuxPort; }}

    public bool IsNestedPort                { get { var parent= Parent; return parent != null && parent.IsPort; }}

	public bool IsDataRelayPort				    { get { return IsDataPort && !IsDataEndPort; }}
	public bool IsDataEndPort					{ get { return IsDataPort && (Parent.IsFunction ||
	                                                                      (IsInputPort && Source == null) ||
	                                                                      (IsOutputPort && !myIStorage.IsPortSourced(this))); }}
}
