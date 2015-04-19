using UnityEngine;
using System.Collections;

[System.Serializable]
public enum iCS_ObjectTypeEnum {
    // --------------------------------------------------------------
    // Start of Node object types
    NodeStart= 0,
    
    // Structural nodes
    Behaviour= 0, Package, StateChart, State, Mux, Selector,

    // Function nodes
    Constructor=100,
    InstanceFunction, ClassFunction, 
    InstanceField, ClassField,
    TypeCast,
    InstanceMessage,  ClassMessage,
    InstanceProperty, ClassProperty,

    // Proxy nodes
    VariableReference= 150, FunctionCall,
            
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
    public static bool IsNode                 (iCS_EngineObject obj) { return obj.ObjectType >= iCS_ObjectTypeEnum.NodeStart &&
																			  obj.ObjectType <= iCS_ObjectTypeEnum.NodeEnd; }

    // Structural nodes.
    public static bool IsBehaviour            (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Behaviour; }
    public static bool IsStateChart           (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.StateChart; }
    public static bool IsState                (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.State; }
    public static bool IsPackage              (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Package ||
																			  IsOnStatePackage(obj) ||
																			  IsTransitionPackage(obj); }
    public static bool IsMux                  (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Mux; }
    public static bool IsSelector             (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Selector; }

    public static bool IsKindOfPackage	      (iCS_EngineObject obj) { return IsPackage(obj) ||
                                                                              IsBehaviour(obj) || IsMessage(obj); }
	public static bool IsKindOfState		  (iCS_EngineObject obj) { return IsStateChart(obj) || IsState(obj); }

    // Function nodes.
    public static bool IsKindOfFunction       (iCS_EngineObject obj) { return IsConstructor(obj) || IsFunction(obj) ||
																			  IsField(obj) || IsTypeCast(obj); } 
    public static bool IsFunction             (iCS_EngineObject obj) { return IsClassFunction(obj) || IsInstanceFunction(obj); }
    public static bool IsField                (iCS_EngineObject obj) { return IsClassField(obj) || IsInstanceField(obj); }
    public static bool IsMessage              (iCS_EngineObject obj) { return IsInstanceMessage(obj) || IsClassMessage(obj); }

    public static bool IsConstructor          (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Constructor; }
    public static bool IsClassFunction        (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.ClassFunction; }
    public static bool IsInstanceFunction     (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InstanceFunction; }
    public static bool IsClassField           (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.ClassField; }
    public static bool IsInstanceField        (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InstanceField; }
    public static bool IsTypeCast             (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.TypeCast; }
    public static bool IsInstanceMessage      (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InstanceMessage; }
    public static bool IsClassMessage         (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.ClassMessage; }

    // Transition modules.
    public static bool IsTransitionPackage    (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.TransitionPackage; }

	// State packages
	public static bool IsOnStatePackage       (iCS_EngineObject obj) { return IsOnStateEntryPackage(obj) || IsOnStateUpdatePackage(obj) || IsOnStateExitPackage(obj); }
    public static bool IsOnStateEntryPackage  (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OnStateEntry; }
    public static bool IsOnStateUpdatePackage (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OnStateUpdate; }
    public static bool IsOnStateExitPackage   (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OnStateExit; }
	
    // Proxy
    public static bool IsVariableReference    (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.VariableReference; }
    public static bool IsFunctionCall         (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.FunctionCall; }

    // General Ports
    public static bool IsPort                 (iCS_EngineObject obj) { return obj.ObjectType >= iCS_ObjectTypeEnum.PortStart &&
                                                                              obj.ObjectType <= iCS_ObjectTypeEnum.PortEnd; }
    public static bool IsOutputPort			  (iCS_EngineObject obj) { return IsOutDataOrControlPort(obj) || IsOutStatePort(obj) ||
	 																		  IsOutTransitionPort(obj); }
    public static bool IsInputPort			  (iCS_EngineObject obj) { return IsInDataOrControlPort(obj) || IsInStatePort(obj)||
																			  IsInTransitionPort(obj); }
    
    // State Ports.
	public static bool IsStatePort            (iCS_EngineObject obj) { return IsInStatePort(obj) || IsOutStatePort(obj); }
    public static bool IsInStatePort          (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InStatePort; }
    public static bool IsOutStatePort         (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutStatePort; }

    // Transition Ports
    public static bool IsTransitionPort       (iCS_EngineObject obj) { return IsInTransitionPort(obj) || IsOutTransitionPort(obj); }
    public static bool IsInTransitionPort     (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InTransitionPort; }
    public static bool IsOutTransitionPort    (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutTransitionPort; }

    // Fix Data Flow Ports
    public static bool IsFixDataPort		  (iCS_EngineObject obj) { return IsInFixDataPort(obj) || IsOutFixDataPort(obj); }
    public static bool IsInFixDataPort        (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InFixDataPort; }
    public static bool IsOutFixDataPort       (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutFixDataPort; }
    
    // Dynamic Data Flow Ports 
	public static bool IsDynamicDataPort	  (iCS_EngineObject obj) { return IsInDynamicDataPort(obj) || IsOutDynamicDataPort(obj); }
    public static bool IsInDynamicDataPort    (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InDynamicDataPort; }
    public static bool IsOutDynamicDataPort   (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutDynamicDataPort; }

    // Proposed Data Flow Ports
	public static bool IsProposedDataPort     (iCS_EngineObject obj) { return IsInProposedDataPort(obj) || IsOutProposedDataPort(obj); }
    public static bool IsInProposedDataPort   (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InProposedDataPort; }
    public static bool IsOutProposedDataPort  (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutProposedDataPort; }

    // Data Flow Ports                                                                          
    public static bool IsDataPort             (iCS_EngineObject obj) { return IsInDataPort(obj) || IsOutDataPort(obj); }
    public static bool IsInDataPort			  (iCS_EngineObject obj) { return IsInFixDataPort(obj) || IsInDynamicDataPort(obj) ||
																			  IsInProposedDataPort(obj) || IsInMuxPort(obj) ||
																			  IsInInstancePort(obj); }
    public static bool IsOutDataPort		  (iCS_EngineObject obj) { return IsOutFixDataPort(obj) || IsOutDynamicDataPort(obj) ||
		                                                                      IsOutProposedDataPort(obj) || IsOutMuxPort(obj) ||
																			  IsOutInstancePort(obj); }

	// Parameter Data Flow Ports
	public static bool IsParameterPort        (iCS_EngineObject obj) { return IsPort(obj) &&
	                                                                          obj.PortIndex >= (int)iCS_PortIndex.ParametersStart &&
	                                                                          obj.PortIndex <= (int)iCS_PortIndex.ParametersEnd; }
	public static bool IsInParameterPort	  (iCS_EngineObject obj) { return IsInputPort(obj) && IsParameterPort(obj); }
	public static bool IsOutParameterPort     (iCS_EngineObject obj) { return IsOutputPort(obj) && IsParameterPort(obj); }
	public static bool IsReturnPort			  (iCS_EngineObject obj) { return IsOutFixDataPort(obj) && obj.PortIndex == (int)iCS_PortIndex.Return; }

    // Control Ports
    public static bool IsControlPort          (iCS_EngineObject obj) { return IsEnablePort(obj) || IsTriggerPort(obj); }
    public static bool IsEnablePort           (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.EnablePort; }
	public static bool IsTriggerPort		  (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.TriggerPort; }

    // Data Flow or Control Ports
    public static bool IsDataOrControlPort    (iCS_EngineObject obj) { return IsDataPort(obj) || IsControlPort(obj); }
    public static bool IsInDataOrControlPort  (iCS_EngineObject obj) { return IsInDataPort(obj) || IsEnablePort(obj); }
    public static bool IsOutDataOrControlPort (iCS_EngineObject obj) { return IsOutDataPort(obj) || IsTriggerPort(obj); }
    
    // Mux Ports
	public static bool IsMuxPort			  (iCS_EngineObject obj) { return IsChildMuxPort(obj) || IsParentMuxPort(obj); }
	public static bool IsParentMuxPort	      (iCS_EngineObject obj) { return IsInParentMuxPort(obj) || IsOutParentMuxPort(obj); }
	public static bool IsChildMuxPort	      (iCS_EngineObject obj) { return IsInChildMuxPort(obj) || IsOutChildMuxPort(obj); }
	public static bool IsInMuxPort			  (iCS_EngineObject obj) { return IsInChildMuxPort(obj) || IsInParentMuxPort(obj); }
	public static bool IsInParentMuxPort	  (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InParentMuxPort; }
	public static bool IsInChildMuxPort	      (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InChildMuxPort; }
	public static bool IsOutMuxPort			  (iCS_EngineObject obj) { return IsOutChildMuxPort(obj) || IsOutParentMuxPort(obj); }
	public static bool IsOutParentMuxPort	  (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutParentMuxPort; }
	public static bool IsOutChildMuxPort	  (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutChildMuxPort; }

	// Instance Ports
	public static bool IsInstancePort		  (iCS_EngineObject obj) { return IsInInstancePort(obj) || IsOutInstancePort(obj); }
	public static bool IsInInstancePort		  (iCS_EngineObject obj) { return IsInFixDataPort(obj) &&
	                                                                          obj.PortIndex == (int)iCS_PortIndex.InInstance; }
	public static bool IsOutInstancePort	  (iCS_EngineObject obj) { return obj.PortIndex == (int)iCS_PortIndex.OutInstance; }
}