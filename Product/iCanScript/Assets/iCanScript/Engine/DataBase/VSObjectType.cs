using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Engine {
    
    [System.Serializable]
    public enum VSObjectType {
        // --------------------------------------------------------------
        // Start of Node object types
        NodeStart= 0,
    
        // Structural nodes
        Behaviour= 0, Package, StateChart, State, Mux, InlineCode,

        // Function nodes
        Constructor=100,
        NonStaticFunction, StaticFunction, 
        NonStaticField, StaticField,
        TypeCast,
        InstanceMessage,  StaticMessage,
        NonStaticProperty, StaticProperty,
    	StaticConstructor,

        // Transition nodes
        TransitionPackage=200,

    	// State specific nodes
    	OnStateEntry=210, OnStateUpdate, OnStateExit,

        // Programatic
        Type= 250,
    
        // End of Node object types
        NodeEnd= 299,
    
        // --------------------------------------------------------------
        // Start of Port object type
        PortStart= 300,

        // Data Flow ports
        InFixDataPort= 300,  OutFixDataPort,
        InDynamicDataPort,   OutDynamicDataPort,
        InStaticModulePort_obsolete,  OutStaticModulePort_obsolete,
        InProposedDataPort,  OutProposedDataPort,

        // Control ports
        EnablePort,			 TriggerPort,

    	// State ports
        InStatePort= 400,    OutStatePort,
        InTransitionPort,    OutTransitionPort,

    	// Mux ports.
    	OutChildMuxPort= 500,   OutParentMuxPort,
    	InChildMuxPort,         InParentMuxPort,
	
        // End of Port object type
        PortEnd= 999,
    
        // --------------------------------------------------------------
        // Undefined
        Unknown=10000
    }

    public static class iCS_ObjectType {
        // Type Groups
        public static bool IsNode                 (iCS_EngineObject obj) { return obj.ObjectType >= VSObjectType.NodeStart &&
    																			  obj.ObjectType <= VSObjectType.NodeEnd; }

        // Structural nodes.
        public static bool IsBehaviour            (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.Behaviour; }
        public static bool IsStateChart           (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.StateChart; }
        public static bool IsState                (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.State; }
        public static bool IsInlineCode           (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.InlineCode; }
        public static bool IsPackage              (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.Package ||
    																			  IsOnStatePackage(obj) ||
    																			  IsTransitionPackage(obj); }
        public static bool IsMux                  (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.Mux; }

        public static bool IsKindOfPackage	      (iCS_EngineObject obj) { return IsPackage(obj) ||
                                                                                  IsBehaviour(obj) || IsEventHandler(obj); }
    	public static bool IsKindOfState		  (iCS_EngineObject obj) { return IsStateChart(obj) || IsState(obj); }

        // Function nodes.
        public static bool IsKindOfFunction       (iCS_EngineObject obj) { return IsConstructor(obj) || IsFunction(obj) ||
    																			  IsField(obj) || IsTypeCast(obj); } 
        public static bool IsFunction             (iCS_EngineObject obj) { return IsStaticFunction(obj) || IsNonStaticFunction(obj); }
        public static bool IsField                (iCS_EngineObject obj) { return IsStaticField(obj) || IsNonStaticField(obj); }
        public static bool IsEventHandler         (iCS_EngineObject obj) { return IsInstanceMessage(obj) || IsStaticMessage(obj); }

        public static bool IsConstructor          (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.Constructor; }
        public static bool IsStaticFunction       (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.StaticFunction; }
        public static bool IsNonStaticFunction    (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.NonStaticFunction; }
        public static bool IsStaticField          (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.StaticField; }
        public static bool IsNonStaticField       (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.NonStaticField; }
        public static bool IsTypeCast             (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.TypeCast; }
        public static bool IsInstanceMessage      (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.InstanceMessage; }
        public static bool IsStaticMessage        (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.StaticMessage; }

        // Transition modules.
        public static bool IsTransitionPackage    (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.TransitionPackage; }

    	// State packages
    	public static bool IsOnStatePackage       (iCS_EngineObject obj) { return IsOnStateEntryPackage(obj) || IsOnStateUpdatePackage(obj) || IsOnStateExitPackage(obj); }
        public static bool IsOnStateEntryPackage  (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.OnStateEntry; }
        public static bool IsOnStateUpdatePackage (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.OnStateUpdate; }
        public static bool IsOnStateExitPackage   (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.OnStateExit; }
	
        // General Ports
        public static bool IsPort                 (iCS_EngineObject obj) { return obj.ObjectType >= VSObjectType.PortStart &&
                                                                                  obj.ObjectType <= VSObjectType.PortEnd; }
        public static bool IsOutputPort			  (iCS_EngineObject obj) { return IsOutDataOrControlPort(obj) || IsOutStatePort(obj) ||
    	 																		  IsOutTransitionPort(obj); }
        public static bool IsInputPort			  (iCS_EngineObject obj) { return IsInDataOrControlPort(obj) || IsInStatePort(obj)||
    																			  IsInTransitionPort(obj); }
    
        // State Ports.
    	public static bool IsStatePort            (iCS_EngineObject obj) { return IsInStatePort(obj) || IsOutStatePort(obj); }
        public static bool IsInStatePort          (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.InStatePort; }
        public static bool IsOutStatePort         (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.OutStatePort; }

        // Transition Ports
        public static bool IsTransitionPort       (iCS_EngineObject obj) { return IsInTransitionPort(obj) || IsOutTransitionPort(obj); }
        public static bool IsInTransitionPort     (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.InTransitionPort; }
        public static bool IsOutTransitionPort    (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.OutTransitionPort; }

        // Fix Data Flow Ports
        public static bool IsFixDataPort		  (iCS_EngineObject obj) { return IsInFixDataPort(obj) || IsOutFixDataPort(obj); }
        public static bool IsInFixDataPort        (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.InFixDataPort; }
        public static bool IsOutFixDataPort       (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.OutFixDataPort; }
    
        // Dynamic Data Flow Ports 
    	public static bool IsDynamicDataPort	  (iCS_EngineObject obj) { return IsInDynamicDataPort(obj) || IsOutDynamicDataPort(obj); }
        public static bool IsInDynamicDataPort    (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.InDynamicDataPort; }
        public static bool IsOutDynamicDataPort   (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.OutDynamicDataPort; }

        // Proposed Data Flow Ports
    	public static bool IsProposedDataPort     (iCS_EngineObject obj) { return IsInProposedDataPort(obj) || IsOutProposedDataPort(obj); }
        public static bool IsInProposedDataPort   (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.InProposedDataPort; }
        public static bool IsOutProposedDataPort  (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.OutProposedDataPort; }

        // Data Flow Ports                                                                          
        public static bool IsDataPort             (iCS_EngineObject obj) { return IsInDataPort(obj) || IsOutDataPort(obj); }
        public static bool IsInDataPort			  (iCS_EngineObject obj) { return IsInFixDataPort(obj) || IsInDynamicDataPort(obj) ||
    																			  IsInProposedDataPort(obj) || IsInMuxPort(obj) ||
    																			  IsTargetPort(obj); }
        public static bool IsOutDataPort		  (iCS_EngineObject obj) { return IsOutFixDataPort(obj) || IsOutDynamicDataPort(obj) ||
    		                                                                      IsOutProposedDataPort(obj) || IsOutMuxPort(obj) ||
    																			  IsSelfPort(obj); }

    	// Parameter Data Flow Ports
    	public static bool IsParameterPort        (iCS_EngineObject obj) { return IsPort(obj) &&
    	                                                                          obj.PortIndex >= (int)iCS_PortIndex.ParametersStart &&
    	                                                                          obj.PortIndex <= (int)iCS_PortIndex.ParametersEnd; }
    	public static bool IsInParameterPort	  (iCS_EngineObject obj) { return IsInputPort(obj) && IsParameterPort(obj); }
    	public static bool IsOutParameterPort     (iCS_EngineObject obj) { return IsOutputPort(obj) && IsParameterPort(obj); }
    	public static bool IsReturnPort			  (iCS_EngineObject obj) { return IsOutFixDataPort(obj) && obj.PortIndex == (int)iCS_PortIndex.Return; }

        // Control Ports
        public static bool IsControlPort          (iCS_EngineObject obj) { return IsEnablePort(obj) || IsTriggerPort(obj); }
        public static bool IsEnablePort           (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.EnablePort; }
    	public static bool IsTriggerPort		  (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.TriggerPort; }

        // Data Flow or Control Ports
        public static bool IsDataOrControlPort    (iCS_EngineObject obj) { return IsDataPort(obj) || IsControlPort(obj); }
        public static bool IsInDataOrControlPort  (iCS_EngineObject obj) { return IsInDataPort(obj) || IsEnablePort(obj); }
        public static bool IsOutDataOrControlPort (iCS_EngineObject obj) { return IsOutDataPort(obj) || IsTriggerPort(obj); }
    
        // Mux Ports
    	public static bool IsMuxPort			  (iCS_EngineObject obj) { return IsChildMuxPort(obj) || IsParentMuxPort(obj); }
    	public static bool IsParentMuxPort	      (iCS_EngineObject obj) { return IsInParentMuxPort(obj) || IsOutParentMuxPort(obj); }
    	public static bool IsChildMuxPort	      (iCS_EngineObject obj) { return IsInChildMuxPort(obj) || IsOutChildMuxPort(obj); }
    	public static bool IsInMuxPort			  (iCS_EngineObject obj) { return IsInChildMuxPort(obj) || IsInParentMuxPort(obj); }
    	public static bool IsInParentMuxPort	  (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.InParentMuxPort; }
    	public static bool IsInChildMuxPort	      (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.InChildMuxPort; }
    	public static bool IsOutMuxPort			  (iCS_EngineObject obj) { return IsOutChildMuxPort(obj) || IsOutParentMuxPort(obj); }
    	public static bool IsOutParentMuxPort	  (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.OutParentMuxPort; }
    	public static bool IsOutChildMuxPort	  (iCS_EngineObject obj) { return obj.ObjectType == VSObjectType.OutChildMuxPort; }

    	// Instance Ports
    	public static bool IsTargetPort		      (iCS_EngineObject obj) { return obj.PortIndex == (int)iCS_PortIndex.Target; }
    	public static bool IsSelfPort	          (iCS_EngineObject obj) { return obj.PortIndex == (int)iCS_PortIndex.Self; }
    }

}

