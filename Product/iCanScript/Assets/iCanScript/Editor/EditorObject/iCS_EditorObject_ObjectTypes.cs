using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    //  OBJECT TYPE
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public partial class iCS_EditorObject {
        // Object Type Queries ---------------------------------------------------
        public bool IsRootObject                { get { return InstanceId == 0; }}
        public bool IsNode                      { get { return EngineObject.IsNode; }}
        public bool IsKindOfPackage             { get { return EngineObject.IsKindOfPackage; }}
        public bool IsKindOfFunction            { get { return EngineObject.IsKindOfFunction; }}
        public bool IsKindOfState               { get { return EngineObject.IsKindOfState; }}
        public bool IsBehaviour                 { get { return EngineObject.IsBehaviour; }}
        public bool IsPackage                   { get { return EngineObject.IsPackage; }}
        public bool IsEventHandler              { get { return EngineObject.IsEventHandler; }}
        public bool IsInlineCode                { get { return EngineObject.IsInlineCode; }}
        public bool IsMux                       { get { return EngineObject.IsMux; }}
        public bool IsConstructor               { get { return EngineObject.IsConstructor; }}
        public bool IsNonStaticFunction         { get { return EngineObject.IsNonStaticFunction; }}
        public bool IsStaticFunction            { get { return EngineObject.IsStaticFunction; }}
    	public bool IsField			            { get { return EngineObject.IsField; }}
    	public bool IsNonStaticField			{ get { return EngineObject.IsNonStaticField; }}
    	public bool IsStaticField               { get { return EngineObject.IsStaticField; }}
        public bool IsTypeCast                  { get { return EngineObject.IsTypeCast; }}
        public bool IsStateChart                { get { return EngineObject.IsStateChart; }}
        public bool IsState                     { get { return EngineObject.IsState; }}
    	public bool IsInstanceNode				{ get { return EngineObject.IsInstanceNode; }}
        public bool IsTransitionPackage         { get { return EngineObject.IsTransitionPackage; }}
    	public bool IsOnStatePackage        	{ get { return EngineObject.IsOnStatePackage; }}
        public bool IsOnStateEntryPackage   	{ get { return EngineObject.IsOnStateEntryPackage; }}
        public bool IsOnStateUpdatePackage  	{ get { return EngineObject.IsOnStateUpdatePackage; }}
        public bool IsOnStateExitPackage    	{ get { return EngineObject.IsOnStateExitPackage; }}
        public bool IsFunctionDefinition        { get { return IsPackage && IsParentValid && Parent.IsBehaviour; }}
        public bool IsVariableDefinition        { get { return IsConstructor && IsParentValid && Parent.IsBehaviour; }}
        public bool IsTypeDefinitionNode        { get { return IsRootObject; }}
        
        // General Ports
        public bool IsPort                      { get { return EngineObject.IsPort; }}
        public bool IsInputPort                 { get { return EngineObject.IsInputPort; }}
        public bool IsOutputPort                { get { return EngineObject.IsOutputPort; }}
        public bool IsEndPort                   { get { return Storage.IsEndPort(EngineObject); }}
        public bool IsRelayPort                 { get { return Storage.IsRelayPort(EngineObject); }}
        public bool IsKindOfPackagePort         { get { return IsDataOrControlPort && ParentNode.IsKindOfPackage; }}
        public bool IsVisualEndPort             { get { return IsEndPort || Parent.IsInstanceNode; }}
        public bool IsVisualRelayPort           { get { return IsRelayPort && !Parent.IsInstanceNode; }}
        public bool IsInstanceNodePort          { get { return IsPort && Parent.IsInstanceNode; }}
        // State Ports
        public bool IsStatePort                 { get { return EngineObject.IsStatePort; }}
        public bool IsInStatePort               { get { return EngineObject.IsInStatePort; }}
        public bool IsOutStatePort              { get { return EngineObject.IsOutStatePort; }}
        // Transition Ports
        public bool IsTransitionPort            { get { return EngineObject.IsTransitionPort; }}
        public bool IsInTransitionPort          { get { return EngineObject.IsInTransitionPort; }}
        public bool IsOutTransitionPort         { get { return EngineObject.IsOutTransitionPort; }}
        // Parameter Data Flow Ports
        public bool IsParameterPort         	{ get { return EngineObject.IsParameterPort; }}
        public bool IsInParameterPort       	{ get { return EngineObject.IsInParameterPort; }}
        public bool IsOutParameterPort      	{ get { return EngineObject.IsOutParameterPort; }}
    	public bool IsReturnPort                { get { return EngineObject.IsReturnPort; }}
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
        public bool IsControlPort               { get { return IsEnablePort || IsTriggerPort; }}
        public bool IsEnablePort                { get { return EngineObject.IsEnablePort; }}
        public bool IsTriggerPort               { get { return EngineObject.IsTriggerPort; }}
        // Data Flow or Control Ports
        public bool IsDataOrControlPort         { get { return EngineObject.IsDataOrControlPort; }}
        public bool IsInDataOrControlPort       { get { return EngineObject.IsInDataOrControlPort; }}
        public bool IsOutDataOrControlPort      { get { return EngineObject.IsOutDataOrControlPort; }}
        // Mux Ports
        public bool IsMuxPort                   { get { return EngineObject.IsMuxPort; }}
        public bool IsChildMuxPort              { get { return EngineObject.IsChildMuxPort; }}
        public bool IsParentMuxPort             { get { return EngineObject.IsParentMuxPort; }}
        public bool IsInMuxPort                 { get { return EngineObject.IsInMuxPort; }}
        public bool IsInChildMuxPort            { get { return EngineObject.IsInChildMuxPort; }}
        public bool IsInParentMuxPort           { get { return EngineObject.IsInParentMuxPort; }}
        public bool IsOutMuxPort                { get { return EngineObject.IsOutMuxPort; }}
        public bool IsOutChildMuxPort           { get { return EngineObject.IsOutChildMuxPort; }}
        public bool IsOutParentMuxPort          { get { return EngineObject.IsOutParentMuxPort; }}
        public bool IsNestedPort                { get { var parent= Parent; return parent != null && parent.IsPort; }}
    	// Instance Ports
    	public bool IsTargetOrSelfPort			{ get { return IsTargetPort || IsSelfPort; }}
    	public bool IsTargetPort			    { get { return EngineObject.IsTargetPort; }}
    	public bool IsSelfPort			        { get { return EngineObject.IsSelfPort; }}


        // -------------------------------------------------------------------
        // Special Cases
        public bool IsProgrammaticInstancePort  {
            get {
                if(IsTargetPort || IsSelfPort || (IsReturnPort && ParentNode.IsConstructor)) {
                       return true;
                }
                return false;
            }
        }
        public bool IsFieldGet {
            get {
                if(!IsField) return false;
                return GetReturnPort() != null && GetPortWithIndex(0) == null;                
            }
        }
        public bool IsFieldSet {
            get {
                if(!IsField) return false;
                return GetReturnPort() == null && GetPortWithIndex(0) != null;                
            }
        }
        public bool IsPropertyGet {
            get {
                if(!IsNonStaticFunction && !IsStaticFunction) return false;
                if(!CodeName.StartsWith("get_")) return false;
                return GetReturnPort() != null && GetPortWithIndex(0) == null;                
            }
        }
        public bool IsPropertySet {
            get {
                if(!IsNonStaticFunction && !IsStaticFunction) return false;
                if(!CodeName.StartsWith("set_")) return false;
                return GetReturnPort() == null && GetPortWithIndex(0) != null && GetPortWithIndex(1) == null;                
            }
        }
    }
}
