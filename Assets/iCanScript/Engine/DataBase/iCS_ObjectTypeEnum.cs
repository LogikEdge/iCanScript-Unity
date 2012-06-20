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
    TypeCast,

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
	InMuxPort= 500,      OutMuxPort,
	
    // Undefined
    Unknown=1000
}

public static class iCS_ObjectType {
    // All nodes.
    public static bool IsNode                 (iCS_EditorObject obj) { return IsStructuralNode(obj) || IsFunction(obj); }

    // Structural nodes.
    public static bool IsStructuralNode       (iCS_EditorObject obj) { return IsBehaviour(obj) || IsModule(obj) || IsState(obj) || IsStateChart(obj); }
    public static bool IsBehaviour            (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Behaviour; }
    public static bool IsModule               (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Module || IsTransitionGuard(obj) || IsTransitionAction(obj) || IsTransitionModule(obj); }
    public static bool IsStateChart           (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.StateChart; }
    public static bool IsState                (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.State; }

    // Function nodes.
    public static bool IsFunction             (iCS_EditorObject obj) { return IsConstructor(obj) || IsMethod(obj) || IsField(obj) || IsTypeCast(obj); } 
    public static bool IsMethod               (iCS_EditorObject obj) { return IsStaticMethod(obj) || IsInstanceMethod(obj); }
    public static bool IsField                (iCS_EditorObject obj) { return IsStaticField(obj) || IsInstanceField(obj); }
    public static bool IsConstructor          (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Constructor; }
    public static bool IsStaticMethod         (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.StaticMethod; }
    public static bool IsInstanceMethod       (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InstanceMethod; }
    public static bool IsStaticField          (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.StaticField; }
    public static bool IsInstanceField        (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InstanceField; }
    public static bool IsTypeCast             (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.TypeCast; }

    // Transition modules.
    public static bool IsTransitionNode       (iCS_EditorObject obj) { return IsTransitionModule(obj) || IsTransitionGuard(obj) || IsTransitionAction(obj); }
    public static bool IsTransitionModule     (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.TransitionModule; }
    public static bool IsTransitionGuard      (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.TransitionGuard; }
    public static bool IsTransitionAction     (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.TransitionAction; }

    // Ports.
    public static bool IsEnablePort           (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.EnablePort; }
    public static bool IsInFunctionPort       (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InFunctionPort; }
    public static bool IsOutFunctionPort      (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutFunctionPort; }
    public static bool IsInDynamicModulePort  (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InDynamicModulePort; }
    public static bool IsInStaticModulePort   (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InStaticModulePort; }
    public static bool IsOutDynamicModulePort (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutDynamicModulePort; }
    public static bool IsOutStaticModulePort  (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutStaticModulePort; }
    public static bool IsInStatePort          (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InStatePort; }
    public static bool IsOutStatePort         (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutStatePort; }
    public static bool IsInTransitionPort     (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InTransitionPort; }
    public static bool IsOutTransitionPort    (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutTransitionPort; }
    public static bool IsInDataPort           (iCS_EditorObject obj) { return IsInFunctionPort(obj) || IsInModulePort(obj) || IsEnablePort(obj) || IsInMuxPort(obj); }
    public static bool IsOutDataPort          (iCS_EditorObject obj) { return IsOutFunctionPort(obj) || IsOutModulePort(obj) || IsOutMuxPort(obj); }
    public static bool IsInModulePort         (iCS_EditorObject obj) { return IsInDynamicModulePort(obj) || IsInStaticModulePort(obj); }
    public static bool IsOutModulePort        (iCS_EditorObject obj) { return IsOutDynamicModulePort(obj) || IsOutStaticModulePort(obj); }
	public static bool IsStateChartNode		  (iCS_EditorObject obj) { return IsStateChart(obj) || IsState(obj); }
    public static bool IsDataPort             (iCS_EditorObject obj) { return IsInDataPort(obj) || IsOutDataPort(obj); }
    public static bool IsDynamicModulePort    (iCS_EditorObject obj) { return IsInDynamicModulePort(obj) || IsOutDynamicModulePort(obj); }
    public static bool IsStaticModulePort     (iCS_EditorObject obj) { return IsInStaticModulePort(obj) || IsOutStaticModulePort(obj); }
    public static bool IsFunctionPort         (iCS_EditorObject obj) { return IsInFunctionPort(obj) || IsOutFunctionPort(obj); }
    public static bool IsModulePort           (iCS_EditorObject obj) { return IsInModulePort(obj) || IsOutModulePort(obj); }
    public static bool IsStatePort            (iCS_EditorObject obj) { return IsInStatePort(obj) || IsOutStatePort(obj); }
    public static bool IsTransitionPort       (iCS_EditorObject obj) { return IsInTransitionPort(obj) || IsOutTransitionPort(obj); }
    public static bool IsPort                 (iCS_EditorObject obj) { return IsDataPort(obj)|| IsStatePort(obj) || IsTransitionPort(obj); }
    public static bool IsOutputPort           (iCS_EditorObject obj) { return IsOutDataPort(obj) || IsOutStatePort(obj) || IsOutTransitionPort(obj); }
    public static bool IsInputPort            (iCS_EditorObject obj) { return IsInDataPort(obj)|| IsInStatePort(obj)|| IsInTransitionPort(obj); }
	public static bool IsMuxPort			  (iCS_EditorObject obj) { return IsInMuxPort(obj) || IsOutMuxPort(obj); }
	public static bool IsOutMuxPort			  (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutMuxPort; }
	public static bool IsInMuxPort			  (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InMuxPort; }
}