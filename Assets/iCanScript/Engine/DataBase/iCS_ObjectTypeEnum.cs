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
    Conversion,

    // Transition nodes
    TransitionModule=200, TransitionGuard, TransitionAction,

    // Data ports
    InFunctionPort= 300, OutFunctionPort,
    InDynamicModulePort, OutDynamicModulePort,
    InStaticModulePort,  OutStaticModulePort,

	InStaticDataPort, OutStaticDataPort,
	InDynamicDataPort, OutDynamicDataPort,

	// Transition ports
    InStatePort= 400, OutStatePort,
    InTransitionPort, OutTransitionPort,

	// Control-plane ports
    EnablePort= 500, TriggerPort,

    // Undefined
    Unknown=1000
}

public static class iCS_ObjectType {
	// Nodes
	public static bool IsNode(iCS_ObjectTypeEnum ot)				{ return IsStructuralNode(ot) || IsFunctionNode(ot) || IsTransitionNode(ot); }
	
	// Structural nodes
	public static bool IsStructuralNode(iCS_ObjectTypeEnum ot)  	{ return IsBehaviour(ot) || IsModule(ot) || IsStateChart(ot) || IsState(ot); }
	public static bool IsBehaviour(iCS_ObjectTypeEnum ot)       	{ return ot == iCS_ObjectTypeEnum.Behaviour; }
	public static bool IsModule(iCS_ObjectTypeEnum ot)				{ return ot == iCS_ObjectTypeEnum.Module; }
	public static bool IsStateChart(iCS_ObjectTypeEnum ot)			{ return ot == iCS_ObjectTypeEnum.StateChart; }
	public static bool IsState(iCS_ObjectTypeEnum ot)				{ return ot == iCS_ObjectTypeEnum.State; }
	                                                            	
	// Function nodes                                           	
	public static bool IsFunctionNode(iCS_ObjectTypeEnum ot)		{ return IsConstructor(ot) || IsInstanceMethod(ot) || IsStaticMethod(ot) || IsInstanceField(ot) || IsStaticField(ot) || IsConversion(ot); }
	public static bool IsConstructor(iCS_ObjectTypeEnum ot)			{ return ot == iCS_ObjectTypeEnum.Constructor; }
	public static bool IsInstanceMethod(iCS_ObjectTypeEnum ot)		{ return ot == iCS_ObjectTypeEnum.InstanceMethod; }
	public static bool IsStaticMethod(iCS_ObjectTypeEnum ot)		{ return ot == iCS_ObjectTypeEnum.StaticMethod; }
	public static bool IsInstanceField(iCS_ObjectTypeEnum ot)		{ return ot == iCS_ObjectTypeEnum.InstanceField; }
	public static bool IsStaticField(iCS_ObjectTypeEnum ot)			{ return ot == iCS_ObjectTypeEnum.StaticField; }
	public static bool IsConversion(iCS_ObjectTypeEnum ot)			{ return ot == iCS_ObjectTypeEnum.Conversion; }
                                                                	
	// Transition nodes                                         	
	public static bool IsTransitionNode(iCS_ObjectTypeEnum ot)		{ return IsTransitionModule(ot) || IsTransitionGuard(ot) || IsTransitionAction(ot); }
	public static bool IsTransitionModule(iCS_ObjectTypeEnum ot)	{ return ot == iCS_ObjectTypeEnum.TransitionModule; }
	public static bool IsTransitionGuard(iCS_ObjectTypeEnum ot)		{ return ot == iCS_ObjectTypeEnum.TransitionGuard; }
	public static bool IsTransitionAction(iCS_ObjectTypeEnum ot)	{ return ot == iCS_ObjectTypeEnum.TransitionAction; }
	
	// Ports
	public static bool IsPort(iCS_ObjectTypeEnum ot)				{ return IsDataPort(ot); }
	public static bool IsInputPort(iCS_ObjectTypeEnum ot)			{ return IsInStaticDataPort(ot) || IsInDynamicDataPort(ot); }
	public static bool IsOutputPort(iCS_ObjectTypeEnum ot)			{ return IsOutStaticDataPort(ot) || IsOutDynamicDataPort(ot); }
	
	// Data ports
	public static bool IsDataPort(iCS_ObjectTypeEnum ot)			{ return IsStaticDataPort(ot) || IsDynamicDataPort(ot); }
	public static bool IsStaticDataPort(iCS_ObjectTypeEnum ot)		{ return IsInStaticDataPort(ot) || IsOutStaticDataPort(ot); }
	public static bool IsDynamicDataPort(iCS_ObjectTypeEnum ot)		{ return IsInDynamicDataPort(ot) || IsOutDynamicDataPort(ot); }
	public static bool IsInStaticDataPort(iCS_ObjectTypeEnum ot)	{ return ot == iCS_ObjectTypeEnum.InStaticDataPort || IsTriggerPort(ot); }
	public static bool IsOutStaticDataPort(iCS_ObjectTypeEnum ot)	{ return ot == iCS_ObjectTypeEnum.OutStaticDataPort; }
	public static bool IsInDynamicDataPort(iCS_ObjectTypeEnum ot)	{ return ot == iCS_ObjectTypeEnum.InDynamicDataPort || IsEnablePort(ot); }
	public static bool IsOutDynamicDataPort(iCS_ObjectTypeEnum ot)	{ return ot == iCS_ObjectTypeEnum.OutDynamicDataPort; }
	public static bool IsEnablePort(iCS_ObjectTypeEnum ot)			{ return ot == iCS_ObjectTypeEnum.EnablePort; }
	public static bool IsTriggerPort(iCS_ObjectTypeEnum ot)			{ return ot == iCS_ObjectTypeEnum.TriggerPort; }

	// Transition ports
	public static bool IsTransitionPort(iCS_ObjectTypeEnum ot)		{ return IsInTransitionPort(ot) || IsOutTransitionPort(ot); }
	public static bool IsInTransitionPort(iCS_ObjectTypeEnum ot)	{ return ot == iCS_ObjectTypeEnum.InTransitionPort; }
	public static bool IsOutTransitionPort(iCS_ObjectTypeEnum ot)	{ return ot == iCS_ObjectTypeEnum.OutTransitionPort; }
}

public static partial class WD {
    public static bool IsBehaviour            (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Behaviour; }
    public static bool IsModule               (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Module || IsTransitionGuard(obj) || IsTransitionAction(obj) || IsTransitionModule(obj); }
    public static bool IsStateChart           (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.StateChart; }
    public static bool IsState                (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.State; }
    public static bool IsConstructor          (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Constructor; }
    public static bool IsStaticMethod         (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.StaticMethod; }
    public static bool IsInstanceMethod       (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InstanceMethod; }
    public static bool IsStaticField          (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.StaticField; }
    public static bool IsInstanceField        (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InstanceField; }
    public static bool IsConversion           (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Conversion; }
    public static bool IsTransitionModule     (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.TransitionModule; }
    public static bool IsTransitionGuard      (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.TransitionGuard; }
    public static bool IsTransitionAction     (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.TransitionAction; }
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
    public static bool IsMethod               (iCS_EditorObject obj) { return IsStaticMethod(obj) || IsInstanceMethod(obj); }
    public static bool IsField                (iCS_EditorObject obj) { return IsStaticField(obj) || IsInstanceField(obj); }
    public static bool IsInDataPort           (iCS_EditorObject obj) { return IsInFunctionPort(obj) || IsInModulePort(obj) || IsEnablePort(obj); }
    public static bool IsOutDataPort          (iCS_EditorObject obj) { return IsOutFunctionPort(obj) || IsOutModulePort(obj); }
    public static bool IsInModulePort         (iCS_EditorObject obj) { return IsInDynamicModulePort(obj) || IsInStaticModulePort(obj); }
    public static bool IsOutModulePort        (iCS_EditorObject obj) { return IsOutDynamicModulePort(obj) || IsOutStaticModulePort(obj); }
    public static bool IsNode                 (iCS_EditorObject obj) { return IsBehaviour(obj) || IsStateChart(obj) || IsState(obj) || IsModule(obj) || IsConstructor(obj) || IsMethod(obj) || IsConversion(obj) || IsField(obj); }
	public static bool IsStateChartNode		  (iCS_EditorObject obj) { return IsStateChart(obj) || IsState(obj); }
	public static bool IsDataNode             (iCS_EditorObject obj) { return IsModule(obj) || IsConstructor(obj) || IsMethod(obj) || IsConversion(obj) || IsField(obj); }
    public static bool IsDataPort             (iCS_EditorObject obj) { return IsFunctionPort(obj) || IsModulePort(obj) || IsEnablePort(obj); }
    public static bool IsDynamicModulePort    (iCS_EditorObject obj) { return IsInDynamicModulePort(obj) || IsOutDynamicModulePort(obj); }
    public static bool IsStaticModulePort     (iCS_EditorObject obj) { return IsInStaticModulePort(obj) || IsOutStaticModulePort(obj); }
    public static bool IsFunctionPort         (iCS_EditorObject obj) { return IsInFunctionPort(obj) || IsOutFunctionPort(obj); }
    public static bool IsModulePort           (iCS_EditorObject obj) { return IsInModulePort(obj) || IsOutModulePort(obj); }
    public static bool IsStatePort            (iCS_EditorObject obj) { return IsInStatePort(obj) || IsOutStatePort(obj); }
    public static bool IsTransitionPort       (iCS_EditorObject obj) { return IsInTransitionPort(obj) || IsOutTransitionPort(obj); }
    public static bool IsPort                 (iCS_EditorObject obj) { return IsFunctionPort(obj) || IsModulePort(obj) || IsEnablePort(obj) || IsStatePort(obj) || IsTransitionPort(obj); }
    public static bool IsOutputPort           (iCS_EditorObject obj) { return IsOutFunctionPort(obj) || IsOutModulePort(obj) || IsOutStatePort(obj) || IsOutTransitionPort(obj); }
    public static bool IsInputPort            (iCS_EditorObject obj) { return IsInFunctionPort(obj) || IsInModulePort(obj) || IsInStatePort(obj) || IsEnablePort(obj) || IsInTransitionPort(obj); }
}