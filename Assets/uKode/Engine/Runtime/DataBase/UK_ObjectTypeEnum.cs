using UnityEngine;
using System.Collections;

[System.Serializable]
public enum UK_ObjectTypeEnum {
    Behaviour, Module, StateChart, State,
    InstanceMethod, StaticMethod, 
    InstanceField, StaticField,
    Conversion, TransitionGuard, TransitionAction,
//    TransitionTrigger, TransitionEntryAction, TransitionDataStream,
    InFieldPort,         OutFieldPort,
    InPropertyPort,      OutPropertyPort,
    InFunctionPort,      OutFunctionPort,
    InDynamicModulePort, OutDynamicModulePort,
    InStaticModulePort,  OutStaticModulePort,
    InStatePort,         OutStatePort,
    EnablePort,
    Unknown
}

public static partial class WD {
    public static bool IsBehaviour            (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.Behaviour; }
    public static bool IsModule               (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.Module || IsTransitionGuard(obj) || IsTransitionAction(obj); }
    public static bool IsStateChart           (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.StateChart; }
    public static bool IsState                (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.State; }
    public static bool IsStaticMethod         (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.StaticMethod; }
    public static bool IsInstanceMethod       (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.InstanceMethod; }
    public static bool IsStaticField          (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.StaticField; }
    public static bool IsInstanceField        (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.InstanceField; }
    public static bool IsConversion           (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.Conversion; }
    public static bool IsTransitionGuard      (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.TransitionGuard; }
    public static bool IsTransitionAction     (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.TransitionAction; }
    public static bool IsEnablePort           (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.EnablePort; }
    public static bool IsInFieldPort          (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.InFieldPort; }
    public static bool IsOutFieldPort         (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.OutFieldPort; }
    public static bool IsInPropertyPort       (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.InPropertyPort; }
    public static bool IsOutPropertyPort      (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.OutPropertyPort; }
    public static bool IsInFunctionPort       (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.InFunctionPort; }
    public static bool IsOutFunctionPort      (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.OutFunctionPort; }
    public static bool IsInDynamicModulePort  (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.InDynamicModulePort; }
    public static bool IsInStaticModulePort   (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.InStaticModulePort; }
    public static bool IsOutDynamicModulePort (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.OutDynamicModulePort; }
    public static bool IsOutStaticModulePort  (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.OutStaticModulePort; }
    public static bool IsInStatePort          (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.InStatePort; }
    public static bool IsOutStatePort         (UK_EditorObject obj) { return obj.ObjectType == UK_ObjectTypeEnum.OutStatePort; }
    public static bool IsMethod               (UK_EditorObject obj) { return IsStaticMethod(obj) || IsInstanceMethod(obj); }
    public static bool IsField                (UK_EditorObject obj) { return IsStaticField(obj) || IsInstanceField(obj); }
    public static bool IsInDataPort           (UK_EditorObject obj) { return IsInFieldPort(obj) || IsInFunctionPort(obj) || IsInModulePort(obj) || IsInPropertyPort(obj) || IsEnablePort(obj); }
    public static bool IsOutDataPort          (UK_EditorObject obj) { return IsOutFieldPort(obj) || IsOutFunctionPort(obj) || IsOutModulePort(obj) || IsOutPropertyPort(obj); }
    public static bool IsInModulePort         (UK_EditorObject obj) { return IsInDynamicModulePort(obj) || IsInStaticModulePort(obj); }
    public static bool IsOutModulePort        (UK_EditorObject obj) { return IsOutDynamicModulePort(obj) || IsOutStaticModulePort(obj); }
    public static bool IsNode                 (UK_EditorObject obj) { return IsBehaviour(obj) || IsStateChart(obj) || IsState(obj) || IsModule(obj) || IsMethod(obj) || IsConversion(obj) || IsField(obj); }
    public static bool IsDataPort             (UK_EditorObject obj) { return IsFieldPort(obj) || IsFunctionPort(obj) || IsModulePort(obj) || IsPropertyPort(obj) || IsEnablePort(obj); }
    public static bool IsDynamicModulePort    (UK_EditorObject obj) { return IsInDynamicModulePort(obj) || IsOutDynamicModulePort(obj); }
    public static bool IsStaticModulePort     (UK_EditorObject obj) { return IsInStaticModulePort(obj) || IsOutStaticModulePort(obj); }
    public static bool IsFieldPort            (UK_EditorObject obj) { return IsInFieldPort(obj) || IsOutFieldPort(obj); }
    public static bool IsPropertyPort         (UK_EditorObject obj) { return IsInPropertyPort(obj) || IsOutPropertyPort(obj); }
    public static bool IsFunctionPort         (UK_EditorObject obj) { return IsInFunctionPort(obj) || IsOutFunctionPort(obj); }
    public static bool IsModulePort           (UK_EditorObject obj) { return IsInModulePort(obj) || IsOutModulePort(obj); }
    public static bool IsStatePort            (UK_EditorObject obj) { return IsInStatePort(obj) || IsOutStatePort(obj); }
    public static bool IsPort                 (UK_EditorObject obj) { return IsFieldPort(obj) || IsFunctionPort(obj) || IsModulePort(obj) || IsPropertyPort(obj) || IsEnablePort(obj) || IsStatePort(obj); }
    public static bool IsOutputPort           (UK_EditorObject obj) { return IsOutFieldPort(obj) || IsOutPropertyPort(obj) || IsOutFunctionPort(obj) || IsOutModulePort(obj) || IsOutStatePort(obj); }
    public static bool IsInputPort            (UK_EditorObject obj) { return IsInFieldPort(obj) || IsInPropertyPort(obj) || IsInFunctionPort(obj) || IsInModulePort(obj) || IsInStatePort(obj) || IsEnablePort(obj); }
}