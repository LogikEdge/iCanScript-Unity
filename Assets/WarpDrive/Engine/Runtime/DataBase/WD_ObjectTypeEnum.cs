using UnityEngine;
using System.Collections;

[System.Serializable]
public enum WD_ObjectTypeEnum {
    Behaviour, Module, StateChart, State,
    Class, Function, Conversion,
    InFieldPort,    OutFieldPort,
    InPropertyPort, OutPropertyPort,
    InFunctionPort, OutFunctionPort,
    InModulePort,   OutModulePort,
    InStatePort,    OutStatePort,
    EnablePort,
    Unknown
}

public static partial class WD {
    public static bool IsBehaviour        (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.Behaviour; }
    public static bool IsModule           (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.Module; }
    public static bool IsStateChart       (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.StateChart; }
    public static bool IsState            (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.State; }
    public static bool IsClass            (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.Class; }
    public static bool IsFunction         (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.Function; }
    public static bool IsConversion       (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.Conversion; }
    public static bool IsEnablePort       (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.EnablePort; }
    public static bool IsInFieldPort      (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.InFieldPort; }
    public static bool IsOutFieldPort     (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.OutFieldPort; }
    public static bool IsInPropertyPort   (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.InPropertyPort; }
    public static bool IsOutPropertyPort  (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.OutPropertyPort; }
    public static bool IsInFunctionPort   (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.InFunctionPort; }
    public static bool IsOutFunctionPort  (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.OutFunctionPort; }
    public static bool IsInModulePort     (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.InModulePort; }
    public static bool IsOutModulePort    (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.OutModulePort; }
    public static bool IsInStatePort      (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.InStatePort; }
    public static bool IsOutStatePort     (WD_EditorObject obj) { return obj.ObjectType == WD_ObjectTypeEnum.OutStatePort; }
    public static bool IsNode             (WD_EditorObject obj) { return IsBehaviour(obj) || IsStateChart(obj) || IsState(obj) || IsModule(obj) || IsClass(obj) || IsFunction(obj) || IsConversion(obj); }
    public static bool IsDataPort         (WD_EditorObject obj) { return IsFieldPort(obj) || IsFunctionPort(obj) || IsModulePort(obj) || IsPropertyPort(obj) || IsEnablePort(obj); }
    public static bool IsFieldPort        (WD_EditorObject obj) { return IsInFieldPort(obj) || IsOutFieldPort(obj); }
    public static bool IsPropertyPort     (WD_EditorObject obj) { return IsInPropertyPort(obj) || IsOutPropertyPort(obj); }
    public static bool IsFunctionPort     (WD_EditorObject obj) { return IsInFunctionPort(obj) || IsOutFunctionPort(obj); }
    public static bool IsModulePort       (WD_EditorObject obj) { return IsInModulePort(obj) || IsOutModulePort(obj); }
    public static bool IsStatePort        (WD_EditorObject obj) { return IsInStatePort(obj) || IsOutStatePort(obj); }
    public static bool IsPort             (WD_EditorObject obj) { return IsFieldPort(obj) || IsFunctionPort(obj) || IsModulePort(obj) || IsPropertyPort(obj) || IsEnablePort(obj) || IsStatePort(obj); }
    public static bool IsOutputPort       (WD_EditorObject obj) { return IsOutFieldPort(obj) || IsOutPropertyPort(obj) || IsOutFunctionPort(obj) || IsOutModulePort(obj) || IsOutStatePort(obj); }
    public static bool IsInputPort        (WD_EditorObject obj) { return IsInFieldPort(obj) || IsInPropertyPort(obj) || IsInFunctionPort(obj) || IsInModulePort(obj) || IsInStatePort(obj) || IsEnablePort(obj); }
}