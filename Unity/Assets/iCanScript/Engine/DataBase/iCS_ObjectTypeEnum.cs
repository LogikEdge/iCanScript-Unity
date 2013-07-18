using UnityEngine;
using System.Collections;

[System.Serializable]
public enum iCS_ObjectTypeEnum {
    // --------------------------------------------------------------
    // Start of Node object types
    NodeStart= 0,
    
    // Structural nodes
    Behaviour= 0, Package, StateChart, State,

    // Function nodes
    Constructor=100,
    InstanceFunction, ClassFunction, 
    InstanceField, ClassField,
    TypeCast,
    InstanceMessage,  ClassMessage,
    InstanceProperty, ClassProperty,

    // Transition nodes
    TransitionModule=200, TransitionGuard, TransitionAction,

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
    EnablePort,			 OutTriggerPort,

	// State ports
    InStatePort= 400,    OutStatePort,
    InTransitionPort,    OutTransitionPort,

	// Mux ports.
	ChildMuxPort= 500,   ParentMuxPort,
	
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
    public static bool IsPackage              (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Package; }

    public static bool IsKindOfPackage	      (iCS_EngineObject obj) { return IsPackage(obj) || IsTransitionNode(obj) ||
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
    public static bool IsTransitionNode       (iCS_EngineObject obj) { return IsTransitionModule(obj) || IsTransitionGuard(obj) ||
																			  IsTransitionAction(obj); }
    public static bool IsTransitionModule     (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.TransitionModule; }
    public static bool IsTransitionGuard      (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.TransitionGuard; }
    public static bool IsTransitionAction     (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.TransitionAction; }

    // General Ports
    public static bool IsPort                 (iCS_EngineObject obj) { return obj.ObjectType >= iCS_ObjectTypeEnum.PortStart &&
                                                                              obj.ObjectType <= iCS_ObjectTypeEnum.PortEnd; }
    public static bool IsOutputPort			  (iCS_EngineObject obj) { return IsOutDataOrControlPort(obj) || IsOutStatePort(obj) ||
	 																		  IsOutTransitionPort(obj); }
    public static bool IsInputPort			  (iCS_EngineObject obj) { return IsInDataOrControlPort(obj)|| IsInStatePort(obj)||
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
																			  IsInProposedDataPort(obj); }
    public static bool IsOutDataPort		  (iCS_EngineObject obj) { return IsOutFixDataPort(obj) || IsOutDynamicDataPort(obj) ||
		                                                                      IsOutProposedDataPort(obj) || IsMuxPort(obj); }

    // Control Ports
    public static bool IsControlPort          (iCS_EngineObject obj) { return IsEnablePort(obj) || IsOutTriggerPort(obj); }
    public static bool IsEnablePort           (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.EnablePort; }
	public static bool IsOutTriggerPort		  (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutTriggerPort; }

    // Data Flow or Control Ports
    public static bool IsDataOrControlPort    (iCS_EngineObject obj) { return IsDataPort(obj) || IsControlPort(obj); }
    public static bool IsInDataOrControlPort  (iCS_EngineObject obj) { return IsInDataPort(obj) || IsEnablePort(obj); }
    public static bool IsOutDataOrControlPort (iCS_EngineObject obj) { return IsOutDataPort(obj) || IsOutTriggerPort(obj); }
    
    // Mux Ports
	public static bool IsMuxPort			  (iCS_EngineObject obj) { return IsChildMuxPort(obj) || IsParentMuxPort(obj); }
	public static bool IsParentMuxPort		  (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.ParentMuxPort; }
	public static bool IsChildMuxPort		  (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.ChildMuxPort; }
}