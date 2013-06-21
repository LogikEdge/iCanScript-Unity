using UnityEngine;
using System.Collections;

[System.Serializable]
public enum iCS_ObjectTypeEnum {
    // Structural nodes
    Behaviour= 0, Module, StateChart, State,

    // Function nodes
    Constructor=100,
    InstanceMethod, StaticMethod, 
    InstanceField, StaticField,
    TypeCast, Event,
    InstanceProperty, ClassProperty,

    // Transition nodes
    TransitionModule=200, TransitionGuard, TransitionAction,

    // Data ports
    InFunctionPort= 300, OutFunctionPort,
    InDynamicModulePort, OutDynamicModulePort,
    InStaticModulePort,  OutStaticModulePort,
    EnablePort,			 TriggerPort,

	// State ports
    InStatePort= 400,    OutStatePort,
    InTransitionPort,    OutTransitionPort,

	// Active ports.
	ChildMuxPort= 500,   ParentMuxPort,
	
    // Programatic
    Type= 900,
    
    // Undefined
    Unknown=1000
}

public static class iCS_ObjectType {
    // All nodes.
    public static bool IsNode                 (iCS_EngineObject obj) { return IsStructuralNode(obj) || IsFunction(obj); }

    // Structural nodes.
    public static bool IsStructuralNode       (iCS_EngineObject obj) { return IsBehaviour(obj) || IsModule(obj) || IsState(obj) || IsStateChart(obj); }
    public static bool IsBehaviour            (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Behaviour; }
    public static bool IsModule               (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Module || IsTransitionNode(obj); }
    public static bool IsStateChart           (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.StateChart; }
    public static bool IsState                (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.State; }

    // Function nodes.
    public static bool IsFunction             (iCS_EngineObject obj) { return IsConstructor(obj) || IsMethod(obj) || IsField(obj) || IsTypeCast(obj); } 
    public static bool IsMethod               (iCS_EngineObject obj) { return IsStaticMethod(obj) || IsInstanceMethod(obj); }
    public static bool IsField                (iCS_EngineObject obj) { return IsStaticField(obj) || IsInstanceField(obj); }
    public static bool IsConstructor          (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Constructor; }
    public static bool IsStaticMethod         (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.StaticMethod; }
    public static bool IsInstanceMethod       (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InstanceMethod; }
    public static bool IsStaticField          (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.StaticField; }
    public static bool IsInstanceField        (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InstanceField; }
    public static bool IsTypeCast             (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.TypeCast; }

    // Transition modules.
    public static bool IsTransitionNode       (iCS_EngineObject obj) { return IsTransitionModule(obj) || IsTransitionGuard(obj) || IsTransitionAction(obj); }
    public static bool IsTransitionModule     (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.TransitionModule; }
    public static bool IsTransitionGuard      (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.TransitionGuard; }
    public static bool IsTransitionAction     (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.TransitionAction; }

    // Ports.
    public static bool IsEnablePort           (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.EnablePort; }
    public static bool IsInFunctionPort       (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InFunctionPort; }
    public static bool IsOutFunctionPort      (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutFunctionPort; }
    public static bool IsInDynamicModulePort  (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InDynamicModulePort; }
    public static bool IsInStaticModulePort   (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InStaticModulePort; }
    public static bool IsOutDynamicModulePort (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutDynamicModulePort; }
    public static bool IsOutStaticModulePort  (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutStaticModulePort; }
    public static bool IsInStatePort          (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InStatePort; }
    public static bool IsOutStatePort         (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutStatePort; }
    public static bool IsInTransitionPort     (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InTransitionPort; }
    public static bool IsOutTransitionPort    (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutTransitionPort; }
    public static bool IsInDataPort           (iCS_EngineObject obj) { return IsInFunctionPort(obj) || IsInModulePort(obj) || IsEnablePort(obj); }
    public static bool IsOutDataPort          (iCS_EngineObject obj) { return IsOutFunctionPort(obj) || IsOutModulePort(obj) || IsMuxPort(obj); }
    public static bool IsInModulePort         (iCS_EngineObject obj) { return IsInDynamicModulePort(obj) || IsInStaticModulePort(obj); }
    public static bool IsOutModulePort        (iCS_EngineObject obj) { return IsOutDynamicModulePort(obj) || IsOutStaticModulePort(obj) || IsMuxPort(obj); }
	public static bool IsStateChartNode		  (iCS_EngineObject obj) { return IsStateChart(obj) || IsState(obj); }
    public static bool IsDataPort             (iCS_EngineObject obj) { return IsInDataPort(obj) || IsOutDataPort(obj); }
    public static bool IsDynamicModulePort    (iCS_EngineObject obj) { return IsInDynamicModulePort(obj) || IsOutDynamicModulePort(obj); }
    public static bool IsStaticModulePort     (iCS_EngineObject obj) { return IsInStaticModulePort(obj) || IsOutStaticModulePort(obj); }
    public static bool IsFunctionPort         (iCS_EngineObject obj) { return IsInFunctionPort(obj) || IsOutFunctionPort(obj); }
    public static bool IsModulePort           (iCS_EngineObject obj) { return IsInModulePort(obj) || IsOutModulePort(obj); }
    public static bool IsStatePort            (iCS_EngineObject obj) { return IsInStatePort(obj) || IsOutStatePort(obj); }
    public static bool IsTransitionPort       (iCS_EngineObject obj) { return IsInTransitionPort(obj) || IsOutTransitionPort(obj); }
    public static bool IsPort                 (iCS_EngineObject obj) { return IsDataPort(obj)|| IsStatePort(obj) || IsTransitionPort(obj); }
    public static bool IsOutputPort           (iCS_EngineObject obj) { return IsOutDataPort(obj) || IsOutStatePort(obj) || IsOutTransitionPort(obj); }
    public static bool IsInputPort            (iCS_EngineObject obj) { return IsInDataPort(obj)|| IsInStatePort(obj)|| IsInTransitionPort(obj); }
	public static bool IsMuxPort			  (iCS_EngineObject obj) { return IsChildMuxPort(obj) || IsParentMuxPort(obj); }
	public static bool IsParentMuxPort		  (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.ParentMuxPort; }
	public static bool IsChildMuxPort		  (iCS_EngineObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.ChildMuxPort; }
}