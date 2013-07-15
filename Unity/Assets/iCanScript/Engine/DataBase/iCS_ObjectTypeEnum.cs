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

    // Data ports
    InFixPort= 300,      OutFixPort,
    InDynamicPort,       OutDynamicPort,
    InStaticModulePort_obsolete,  OutStaticModulePort_obsolete,
    EnablePort,			 TriggerPort,
    InProposedPort,      OutProposedPort,

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
    public static bool IsPort                 (iCS_EngineObject obj) { return obj.ObjectType >= iCS_ObjectTypeEnum.PortStart &&
                                                                              obj.ObjectType <= iCS_ObjectTypeEnum.PortEnd; }

    // Structural nodes.
    public static bool IsBehaviour            (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Behaviour; }
    public static bool IsStateChart           (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.StateChart; }
    public static bool IsState                (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.State; }
    public static bool IsAggregate            (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Package; }

    public static bool IsKindOfAggregate	  (iCS_EngineObject obj) { return IsAggregate(obj) || IsTransitionNode(obj) ||
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

    // Ports.
    public static bool IsEnablePort           (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.EnablePort; }
    public static bool IsInFixPort            (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InFixPort; }
    public static bool IsOutFixPort           (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutFixPort; }
    public static bool IsInDynamicPort        (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InDynamicPort; }
    public static bool IsOutDynamicPort       (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutDynamicPort; }
    public static bool IsInProposedPort       (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InProposedPort; }
    public static bool IsOutProposedPort      (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutProposedPort; }
    public static bool IsInStatePort          (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InStatePort; }
    public static bool IsOutStatePort         (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutStatePort; }
    public static bool IsInTransitionPort     (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InTransitionPort; }
    public static bool IsOutTransitionPort    (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutTransitionPort; }
	public static bool IsParentMuxPort		  (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.ParentMuxPort; }
	public static bool IsChildMuxPort		  (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.ChildMuxPort; }

	public static bool IsFixPort			  (iCS_EngineObject obj) { return IsInFixPort(obj) || IsOutFixPort(obj); }
	public static bool IsDynamicPort		  (iCS_EngineObject obj) { return IsInDynamicPort(obj) || IsOutDynamicPort(obj); }
	public static bool IsProposedPort         (iCS_EngineObject obj) { return IsInProposedPort(obj) || IsOutProposedPort(obj); }
    public static bool IsInDataPort			  (iCS_EngineObject obj) { return IsInFixPort(obj) || IsInDynamicPort(obj) ||
																			  IsInProposedPort(obj) || IsEnablePort(obj); }
    public static bool IsOutDataPort		  (iCS_EngineObject obj) { return IsOutFixPort(obj) || IsOutDynamicPort(obj) ||
																			  IsOutProposedPort(obj) || IsMuxPort(obj); }
    public static bool IsOutputPort			  (iCS_EngineObject obj) { return IsOutDataPort(obj) || IsOutStatePort(obj) ||
	 																		  IsOutTransitionPort(obj); }
    public static bool IsInputPort			  (iCS_EngineObject obj) { return IsInDataPort(obj)|| IsInStatePort(obj)||
																			  IsInTransitionPort(obj); }

    public static bool IsDataPort             (iCS_EngineObject obj) { return IsInDataPort(obj) || IsOutDataPort(obj); }
    public static bool IsStatePort            (iCS_EngineObject obj) { return IsInStatePort(obj) || IsOutStatePort(obj); }
    public static bool IsTransitionPort       (iCS_EngineObject obj) { return IsInTransitionPort(obj) || IsOutTransitionPort(obj); }

	public static bool IsMuxPort			  (iCS_EngineObject obj) { return IsChildMuxPort(obj) || IsParentMuxPort(obj); }
}