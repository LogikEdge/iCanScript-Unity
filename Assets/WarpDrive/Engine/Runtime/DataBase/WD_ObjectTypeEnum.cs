using UnityEngine;
using System.Collections;

[System.Serializable]
public enum WD_ObjectTypeEnum {
    Behaviour, Module, StateChart, State,
    Class, Function, Conversion, TransitionEntry, TransitionExit,
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
    public static bool IsBehaviour            (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.Behaviour; }
    public static bool IsModule               (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.Module || IsTransitionEntry(obj) || IsTransitionExit(obj); }
    public static bool IsStateChart           (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.StateChart; }
    public static bool IsState                (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.State; }
    public static bool IsClass                (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.Class; }
    public static bool IsFunction             (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.Function; }
    public static bool IsConversion           (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.Conversion; }
    public static bool IsTransitionEntry      (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.TransitionEntry; }
    public static bool IsTransitionExit       (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.TransitionExit; }
    public static bool IsEnablePort           (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.EnablePort; }
    public static bool IsInFieldPort          (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.InFieldPort; }
    public static bool IsOutFieldPort         (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.OutFieldPort; }
    public static bool IsInPropertyPort       (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.InPropertyPort; }
    public static bool IsOutPropertyPort      (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.OutPropertyPort; }
    public static bool IsInFunctionPort       (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.InFunctionPort; }
    public static bool IsOutFunctionPort      (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.OutFunctionPort; }
    public static bool IsInDynamicModulePort  (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.InDynamicModulePort; }
    public static bool IsInStaticModulePort   (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.InStaticModulePort; }
    public static bool IsOutDynamicModulePort (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.OutDynamicModulePort; }
    public static bool IsOutStaticModulePort  (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.OutStaticModulePort; }
    public static bool IsInStatePort          (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.InStatePort; }
    public static bool IsOutStatePort         (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.OutStatePort; }
    public static bool IsInDataPort           (WD_EditorObject obj) { return IsInFieldPort(obj) || IsInFunctionPort(obj) || IsInModulePort(obj) || IsInPropertyPort(obj) || IsEnablePort(obj); }
    public static bool IsOutDataPort          (WD_EditorObject obj) { return IsOutFieldPort(obj) || IsOutFunctionPort(obj) || IsOutModulePort(obj) || IsOutPropertyPort(obj); }
    public static bool IsInModulePort         (WD_EditorObject obj) { return IsInDynamicModulePort(obj) || IsInStaticModulePort(obj); }
    public static bool IsOutModulePort        (WD_EditorObject obj) { return IsOutDynamicModulePort(obj) || IsOutStaticModulePort(obj); }
    public static bool IsNode                 (WD_EditorObject obj) { return IsBehaviour(obj) || IsStateChart(obj) || IsState(obj) || IsModule(obj) || IsClass(obj) || IsFunction(obj) || IsConversion(obj); }
    public static bool IsDataPort             (WD_EditorObject obj) { return IsFieldPort(obj) || IsFunctionPort(obj) || IsModulePort(obj) || IsPropertyPort(obj) || IsEnablePort(obj); }
    public static bool IsDynamicModulePort    (WD_EditorObject obj) { return IsInDynamicModulePort(obj) || IsOutDynamicModulePort(obj); }
    public static bool IsStaticModulePort     (WD_EditorObject obj) { return IsInStaticModulePort(obj) || IsOutStaticModulePort(obj); }
    public static bool IsFieldPort            (WD_EditorObject obj) { return IsInFieldPort(obj) || IsOutFieldPort(obj); }
    public static bool IsPropertyPort         (WD_EditorObject obj) { return IsInPropertyPort(obj) || IsOutPropertyPort(obj); }
    public static bool IsFunctionPort         (WD_EditorObject obj) { return IsInFunctionPort(obj) || IsOutFunctionPort(obj); }
    public static bool IsModulePort           (WD_EditorObject obj) { return IsInModulePort(obj) || IsOutModulePort(obj); }
    public static bool IsStatePort            (WD_EditorObject obj) { return IsInStatePort(obj) || IsOutStatePort(obj); }
    public static bool IsPort                 (WD_EditorObject obj) { return IsFieldPort(obj) || IsFunctionPort(obj) || IsModulePort(obj) || IsPropertyPort(obj) || IsEnablePort(obj) || IsStatePort(obj); }
    public static bool IsOutputPort           (WD_EditorObject obj) { return IsOutFieldPort(obj) || IsOutPropertyPort(obj) || IsOutFunctionPort(obj) || IsOutModulePort(obj) || IsOutStatePort(obj); }
    public static bool IsInputPort            (WD_EditorObject obj) { return IsInFieldPort(obj) || IsInPropertyPort(obj) || IsInFunctionPort(obj) || IsInModulePort(obj) || IsInStatePort(obj) || IsEnablePort(obj); }
}