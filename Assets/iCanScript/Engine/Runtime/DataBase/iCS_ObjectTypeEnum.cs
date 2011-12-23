using UnityEngine;
using System.Collections;

[System.Serializable]
public enum iCS_ObjectTypeEnum {
    Behaviour, Module, StateChart, State,
    InstanceMethod, StaticMethod, 
    InstanceField, StaticField,
    Conversion,
    TransitionModule, TransitionGuard, TransitionAction,
    InFieldPort,         OutFieldPort,
    InPropertyPort,      OutPropertyPort,
    InFunctionPort,      OutFunctionPort,
    InDynamicModulePort, OutDynamicModulePort,
    InStaticModulePort,  OutStaticModulePort,
    InStatePort,         OutStatePort,
    InTransitionPort,    OutTransitionPort,
    EnablePort,
    Unknown
}

public static partial class WD {
    public static bool IsBehaviour            (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Behaviour; }
    public static bool IsModule               (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Module || IsTransitionGuard(obj) || IsTransitionAction(obj) || IsTransitionModule(obj); }
    public static bool IsStateChart           (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.StateChart; }
    public static bool IsState                (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.State; }
    public static bool IsStaticMethod         (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.StaticMethod; }
    public static bool IsInstanceMethod       (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InstanceMethod; }
    public static bool IsStaticField          (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.StaticField; }
    public static bool IsInstanceField        (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InstanceField; }
    public static bool IsConversion           (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.Conversion; }
    public static bool IsTransitionModule     (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.TransitionModule; }
    public static bool IsTransitionGuard      (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.TransitionGuard; }
    public static bool IsTransitionAction     (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.TransitionAction; }
    public static bool IsEnablePort           (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.EnablePort; }
    public static bool IsInFieldPort          (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InFieldPort; }
    public static bool IsOutFieldPort         (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutFieldPort; }
    public static bool IsInPropertyPort       (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.InPropertyPort; }
    public static bool IsOutPropertyPort      (iCS_EditorObject obj) { return obj.ObjectType == iCS_ObjectTypeEnum.OutPropertyPort; }
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
    public static bool IsInDataPort           (iCS_EditorObject obj) { return IsInFieldPort(obj) || IsInFunctionPort(obj) || IsInModulePort(obj) || IsInPropertyPort(obj) || IsEnablePort(obj); }
    public static bool IsOutDataPort          (iCS_EditorObject obj) { return IsOutFieldPort(obj) || IsOutFunctionPort(obj) || IsOutModulePort(obj) || IsOutPropertyPort(obj); }
    public static bool IsInModulePort         (iCS_EditorObject obj) { return IsInDynamicModulePort(obj) || IsInStaticModulePort(obj); }
    public static bool IsOutModulePort        (iCS_EditorObject obj) { return IsOutDynamicModulePort(obj) || IsOutStaticModulePort(obj); }
    public static bool IsNode                 (iCS_EditorObject obj) { return IsBehaviour(obj) || IsStateChart(obj) || IsState(obj) || IsModule(obj) || IsMethod(obj) || IsConversion(obj) || IsField(obj); }
    public static bool IsDataPort             (iCS_EditorObject obj) { return IsFieldPort(obj) || IsFunctionPort(obj) || IsModulePort(obj) || IsPropertyPort(obj) || IsEnablePort(obj); }
    public static bool IsDynamicModulePort    (iCS_EditorObject obj) { return IsInDynamicModulePort(obj) || IsOutDynamicModulePort(obj); }
    public static bool IsStaticModulePort     (iCS_EditorObject obj) { return IsInStaticModulePort(obj) || IsOutStaticModulePort(obj); }
    public static bool IsFieldPort            (iCS_EditorObject obj) { return IsInFieldPort(obj) || IsOutFieldPort(obj); }
    public static bool IsPropertyPort         (iCS_EditorObject obj) { return IsInPropertyPort(obj) || IsOutPropertyPort(obj); }
    public static bool IsFunctionPort         (iCS_EditorObject obj) { return IsInFunctionPort(obj) || IsOutFunctionPort(obj); }
    public static bool IsModulePort           (iCS_EditorObject obj) { return IsInModulePort(obj) || IsOutModulePort(obj); }
    public static bool IsStatePort            (iCS_EditorObject obj) { return IsInStatePort(obj) || IsOutStatePort(obj); }
    public static bool IsTransitionPort       (iCS_EditorObject obj) { return IsInTransitionPort(obj) || IsOutTransitionPort(obj); }
    public static bool IsPort                 (iCS_EditorObject obj) { return IsFieldPort(obj) || IsFunctionPort(obj) || IsModulePort(obj) || IsPropertyPort(obj) || IsEnablePort(obj) || IsStatePort(obj) || IsTransitionPort(obj); }
    public static bool IsOutputPort           (iCS_EditorObject obj) { return IsOutFieldPort(obj) || IsOutPropertyPort(obj) || IsOutFunctionPort(obj) || IsOutModulePort(obj) || IsOutStatePort(obj) || IsOutTransitionPort(obj); }
    public static bool IsInputPort            (iCS_EditorObject obj) { return IsInFieldPort(obj) || IsInPropertyPort(obj) || IsInFunctionPort(obj) || IsInModulePort(obj) || IsInStatePort(obj) || IsEnablePort(obj) || IsInTransitionPort(obj); }
}