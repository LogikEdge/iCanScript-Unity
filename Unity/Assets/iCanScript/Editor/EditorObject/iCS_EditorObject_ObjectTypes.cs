using UnityEngine;
using System.Collections;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  OBJECT TYPE
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // Object Type Queries ---------------------------------------------------
    public bool IsNode                      { get { return EngineObject.IsNode; }}
    public bool IsKindOfPackage           { get { return EngineObject.IsKindOfPackage; }}
    public bool IsKindOfFunction                  { get { return EngineObject.IsKindOfFunction; }}
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

    // General Ports
    public bool IsPort                      { get { return EngineObject.IsPort; }}
    public bool IsInputPort                 { get { return EngineObject.IsInputPort; }}
    public bool IsOutputPort                { get { return EngineObject.IsOutputPort; }}
    public bool IsEndPort                   { get { return Storage.IsEndPort(EngineObject); }}
    public bool IsRelayPort                 { get { return Storage.IsRelayPort(EngineObject); }}
    public bool IsKindOfPackagePort         { get { return IsDataOrControlPort && ParentNode.IsKindOfPackage; }}
    // State Ports
    public bool IsStatePort                 { get { return EngineObject.IsStatePort; }}
    public bool IsInStatePort               { get { return EngineObject.IsInStatePort; }}
    public bool IsOutStatePort              { get { return EngineObject.IsOutStatePort; }}
    // Transition Ports
    public bool IsTransitionPort            { get { return EngineObject.IsTransitionPort; }}
    public bool IsInTransitionPort          { get { return EngineObject.IsInTransitionPort; }}
    public bool IsOutTransitionPort         { get { return EngineObject.IsOutTransitionPort; }}
    // Fix Data Flow Ports
    public bool IsFixDataPort               { get { return EngineObject.IsFixDataPort; }}
    public bool IsInFixDataPort             { get { return EngineObject.IsInFixDataPort; }}
    public bool IsOutFixDataPort            { get { return EngineObject.IsOutFixDataPort; }}
    // Dynamic Data Flow Ports
    public bool IsDynamicDataPort           { get { return EngineObject.IsDynamicDataPort; }}
    public bool IsInDynamicDataPort         { get { return EngineObject.IsInDynamicDataPort; }}
    public bool IsOutDynamicDataPort        { get { return EngineObject.IsOutDynamicDataPort; }}
    // Proposed Data Flow Ports
    public bool IsProposedDataPort          { get { return EngineObject.IsProposedDataPort; }}
    public bool IsInProposedDataPort        { get { return EngineObject.IsInProposedDataPort; }}
    public bool IsOutProposedDataPort       { get { return EngineObject.IsOutProposedDataPort; }}
    // Data Flow Ports
    public bool IsDataPort                  { get { return EngineObject.IsDataPort; }}
    public bool IsInDataPort                { get { return EngineObject.IsInDataPort; }}
    public bool IsOutDataPort               { get { return EngineObject.IsOutDataPort; }}
    // Control Ports
    public bool IsControlPort               { get { return EngineObject.IsControlPort; }}
    public bool IsEnablePort                { get { return EngineObject.IsEnablePort; }}
    public bool IsOutTriggerPort            { get { return EngineObject.IsOutTriggerPort; }}
    // Data Flow or Control Ports
    public bool IsDataOrControlPort         { get { return EngineObject.IsDataOrControlPort; }}
    public bool IsInDataOrControlPort       { get { return EngineObject.IsInDataOrControlPort; }}
    public bool IsOutDataOrControlPort      { get { return EngineObject.IsOutDataOrControlPort; }}
    // Mux Ports
    public bool IsOutMuxPort                { get { return EngineObject.IsOutMuxPort; }}
    public bool IsOutChildMuxPort           { get { return EngineObject.IsOutChildMuxPort; }}
    public bool IsOutParentMuxPort          { get { return EngineObject.IsOutParentMuxPort; }}
    public bool IsNestedPort                { get { var parent= Parent; return parent != null && parent.IsPort; }}
}
