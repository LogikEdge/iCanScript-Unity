using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class WD_EditorObject {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public WD_ObjectTypeEnum    ObjectType  = WD_ObjectTypeEnum.Unknown;
    public int                  InstanceId   = -1;
    public int                  ParentId     = -1;
    public string               QualifiedType= "";
    public string               Name         = "";
    public bool                 IsDirty      = false;
    public Rect                 LocalPosition= new Rect(0,0,0,0);

    // Port specific attributes ---------------------------------------------
    public enum EdgeEnum { None, Top, Bottom, Right, Left };
    public EdgeEnum         Edge      = EdgeEnum.None;
    public int              Source= -1;

    // Non-persistant properties --------------------------------------------
    [System.NonSerialized] public bool IsBeingDragged= false;


    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public bool IsVisible { get { return ObjectType != WD_ObjectTypeEnum.Unknown; }}
    
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public WD_EditorObject() { Reset(); }
    public WD_EditorObject(int id, string name, Type type, int parentId, WD_ObjectTypeEnum objectType, Rect localPosition) {
        Reset();
        InstanceId= id;
        ParentId= parentId;
        Name= name;
        QualifiedType= type.AssemblyQualifiedName;
        IsDirty= true;
        LocalPosition= localPosition;
        ObjectType= objectType;
        if(IsDataPort) {
            Edge= IsInputPort ? (IsEnablePort ? EdgeEnum.Top : EdgeEnum.Left) : EdgeEnum.Right;
        }
    }
    public WD_EditorObject(int id, string name, Type type, int parentId, Rect localPosition) {
        Reset();
        InstanceId= id;
        ParentId= parentId;
        Name= name;
        QualifiedType= type.AssemblyQualifiedName;
        IsDirty= true;
        LocalPosition= localPosition;
        Case<WD_RootNode, WD_Top, WD_StateChart, WD_State, WD_Module, WD_Function, WD_Node, WD_Port>(
            (root)  => { },
            (top)   => { },
            (chart) => { ObjectType= WD_ObjectTypeEnum.State; },
            (state) => { ObjectType= WD_ObjectTypeEnum.State; },
            (mod)   => { ObjectType= WD_ObjectTypeEnum.Module; },
            (func)  => { ObjectType= WD_ObjectTypeEnum.Function; },
            (node)  => { ObjectType= WD_ObjectTypeEnum.Class; },
            (port)  => {
                port.ExecuteIf<WD_FieldPort>(
                    (dataPort) => {
                        dataPort.Case<WD_InFieldPort, WD_OutFieldPort, WD_EnablePort>(
                            (inPort)     => { Edge= EdgeEnum.Left;  ObjectType= WD_ObjectTypeEnum.InFieldPort; },
                            (outPort)    => { Edge= EdgeEnum.Right; ObjectType= WD_ObjectTypeEnum.OutFieldPort; },
                            (enablePort) => { Edge= EdgeEnum.Top;   ObjectType= WD_ObjectTypeEnum.EnablePort; }
                        );
                    }
                );                
            }
        );
    }
    // ----------------------------------------------------------------------
    public void Reset() {
        ObjectType= WD_ObjectTypeEnum.Unknown;
        InstanceId= -1;
        ParentId= -1;
        QualifiedType= "";
        Name= "";
        IsDirty= false;
        LocalPosition= new Rect(0,0,0,0);
        Edge= EdgeEnum.None;
        Source= -1;
        IsBeingDragged= false;
    }
    // ----------------------------------------------------------------------
    public WD_Object CreateRuntimeObject() {
        WD_Object rtObject;
        if(IsRuntimeA<ScriptableObject>()) {
            rtObject= ScriptableObject.CreateInstance(RuntimeType) as WD_Object;
        }
        else {
            rtObject= Activator.CreateInstance(RuntimeType) as WD_Object;            
        }
        if(rtObject == null) {
            Debug.LogError("Unable to create an instance of : "+QualifiedType);
        }
        rtObject.Name= Name;
        rtObject.InstanceId= InstanceId;
        return rtObject;
    }
    
    // ----------------------------------------------------------------------
    public bool IsRuntimeA(Type t) {
        return Inf.IsA(RuntimeType, t);
    }
    public bool IsRuntimeA<T>() where T : class {
        return IsRuntimeA(typeof(T));
    }
    // ----------------------------------------------------------------------
    public bool IsNode             { get { return WD.IsNode(this); }}
    public bool IsBehaviour        { get { return WD.IsBehaviour(this); }}
    public bool IsStateChart       { get { return WD.IsStateChart(this); }}
    public bool IsState            { get { return WD.IsState(this); }}
    public bool IsModule           { get { return WD.IsModule(this);; }}
    public bool IsClass            { get { return WD.IsClass(this); }}
    public bool IsFunction         { get { return WD.IsFunction(this); }}
    public bool IsConversion       { get { return WD.IsConversion(this); }}
    public bool IsPort             { get { return WD.IsPort(this); }}
    public bool IsDataPort         { get { return WD.IsDataPort(this); }}
    public bool IsFieldPort        { get { return WD.IsFieldPort(this); }}
    public bool IsPropertyPort     { get { return WD.IsPropertyPort(this); }}
    public bool IsFunctionPort     { get { return WD.IsFunctionPort(this); }}
    public bool IsModulePort       { get { return WD.IsModulePort(this); }}
    public bool IsStatePort        { get { return WD.IsStatePort(this); }}
    public bool IsEnablePort       { get { return WD.IsEnablePort(this); }}
    public bool IsOutputPort       { get { return WD.IsOutputPort(this); }}
    public bool IsInputPort        { get { return WD.IsInputPort(this); }}
    public bool IsInFieldPort      { get { return WD.IsInFieldPort(this); }}
    public bool IsOutFieldPort     { get { return WD.IsOutFieldPort(this); }}
    public bool IsInPropertyPort   { get { return WD.IsInPropertyPort(this); }}
    public bool IsOutPropertyPort  { get { return WD.IsOutPropertyPort(this); }}
    public bool IsInFunctionPort   { get { return WD.IsInFunctionPort(this); }}
    public bool IsOutFunctionPort  { get { return WD.IsOutFunctionPort(this); }}
    public bool IsInModulePort     { get { return WD.IsInModulePort(this); }}
    public bool IsOutModulePort    { get { return WD.IsOutModulePort(this); }}
    public bool IsInStatePort      { get { return WD.IsInStatePort(this); }}
    public bool IsOutStatePort     { get { return WD.IsOutStatePort(this); }}
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public bool IsValid { get { return InstanceId != -1; }}
    public Type RuntimeType { get { return Type.GetType(QualifiedType); }}
    public string TypeName {
        get {
            int end= QualifiedType.IndexOf(',');
            if(QualifiedType.StartsWith(WD_EditorConfig.TypePrefix)) {
                int prefixLen= WD_EditorConfig.TypePrefix.Length;
                return QualifiedType.Substring(prefixLen, end-prefixLen);
            }
            return QualifiedType.Substring(0, end);
        }
    }
    public string NameOrTypeName {
        get { return (Name == null || Name == "") ? TypeName : Name; }
    }
    public bool IsOnTopEdge         { get { return Edge == EdgeEnum.Top; }}
    public bool IsOnBottomEdge      { get { return Edge == EdgeEnum.Bottom; }}
    public bool IsOnRightEdge       { get { return Edge == EdgeEnum.Right; }}
    public bool IsOnLeftEdge        { get { return Edge == EdgeEnum.Left; }}
    public bool IsOnHorizontalEdge  { get { return IsOnTopEdge   || IsOnBottomEdge; }}
    public bool IsOnVerticalEdge    { get { return IsOnRightEdge || IsOnLeftEdge; }}

    // ======================================================================
    // Editor Object Iteration Utilities
    // ----------------------------------------------------------------------
    // Executes the given action if the given object matches the T type.
    public void ExecuteIf<T>(Action<WD_EditorObject> fnc) where T : WD_Object {
        if(IsRuntimeA<T>()) fnc(this);
    }
    public void Case<T1,T2>(Action<WD_EditorObject> fnc1,
                            Action<WD_EditorObject> fnc2,
                            Action<WD_EditorObject> defaultFnc= null) where T1 : WD_Object
                                                                      where T2 : WD_Object {
        if(IsRuntimeA<T1>())         { fnc1(this); }
        else if(IsRuntimeA<T2>())    { fnc2(this); }
        else if(defaultFnc != null)  { defaultFnc(this); }                                    
    }
    public void Case<T1,T2,T3>(Action<WD_EditorObject> fnc1,
                               Action<WD_EditorObject> fnc2,
                               Action<WD_EditorObject> fnc3,
                               Action<WD_EditorObject> defaultFnc= null) where T1 : WD_Object
                                                                         where T2 : WD_Object
                                                                         where T3 : WD_Object {
        if(IsRuntimeA<T1>())         { fnc1(this); }
        else if(IsRuntimeA<T2>())    { fnc2(this); }
        else if(IsRuntimeA<T3>())    { fnc3(this); }
        else if(defaultFnc != null)  { defaultFnc(this); }                                    
    }
    public void Case<T1,T2,T3,T4>(Action<WD_EditorObject> fnc1,
                                  Action<WD_EditorObject> fnc2,
                                  Action<WD_EditorObject> fnc3,
                                  Action<WD_EditorObject> fnc4,
                                  Action<WD_EditorObject> defaultFnc= null) where T1 : WD_Object
                                                                            where T2 : WD_Object
                                                                            where T3 : WD_Object
                                                                            where T4 : WD_Object {
        if(IsRuntimeA<T1>())         { fnc1(this); }
        else if(IsRuntimeA<T2>())    { fnc2(this); }
        else if(IsRuntimeA<T3>())    { fnc3(this); }
        else if(IsRuntimeA<T4>())    { fnc4(this); }
        else if(defaultFnc != null)  { defaultFnc(this); }                                    
    }
    public void Case<T1,T2,T3,T4,T5>(Action<WD_EditorObject> fnc1,
                                     Action<WD_EditorObject> fnc2,
                                     Action<WD_EditorObject> fnc3,
                                     Action<WD_EditorObject> fnc4,
                                     Action<WD_EditorObject> fnc5,
                                     Action<WD_EditorObject> defaultFnc= null) where T1 : WD_Object
                                                                               where T2 : WD_Object
                                                                               where T3 : WD_Object
                                                                               where T4 : WD_Object
                                                                               where T5 : WD_Object {
        if(IsRuntimeA<T1>())         { fnc1(this); }
        else if(IsRuntimeA<T2>())    { fnc2(this); }
        else if(IsRuntimeA<T3>())    { fnc3(this); }
        else if(IsRuntimeA<T4>())    { fnc4(this); }
        else if(IsRuntimeA<T5>())    { fnc5(this); }
        else if(defaultFnc != null)  { defaultFnc(this); }                                    
    }
    public void Case<T1,T2,T3,T4,T5,T6>(Action<WD_EditorObject> fnc1,
                                        Action<WD_EditorObject> fnc2,
                                        Action<WD_EditorObject> fnc3,
                                        Action<WD_EditorObject> fnc4,
                                        Action<WD_EditorObject> fnc5,
                                        Action<WD_EditorObject> fnc6,
                                        Action<WD_EditorObject> defaultFnc= null) where T1 : WD_Object
                                                                                  where T2 : WD_Object
                                                                                  where T3 : WD_Object
                                                                                  where T4 : WD_Object
                                                                                  where T5 : WD_Object
                                                                                  where T6 : WD_Object {
        if(IsRuntimeA<T1>())         { fnc1(this); }
        else if(IsRuntimeA<T2>())    { fnc2(this); }
        else if(IsRuntimeA<T3>())    { fnc3(this); }
        else if(IsRuntimeA<T4>())    { fnc4(this); }
        else if(IsRuntimeA<T5>())    { fnc5(this); }
        else if(IsRuntimeA<T6>())    { fnc6(this); }
        else if(defaultFnc != null)  { defaultFnc(this); }                                    
    }
    public void Case<T1,T2,T3,T4,T5,T6,T7>(Action<WD_EditorObject> fnc1,
                                           Action<WD_EditorObject> fnc2,
                                           Action<WD_EditorObject> fnc3,
                                           Action<WD_EditorObject> fnc4,
                                           Action<WD_EditorObject> fnc5,
                                           Action<WD_EditorObject> fnc6,
                                           Action<WD_EditorObject> fnc7,
                                           Action<WD_EditorObject> defaultFnc= null) where T1 : WD_Object
                                                                                     where T2 : WD_Object
                                                                                     where T3 : WD_Object
                                                                                     where T4 : WD_Object
                                                                                     where T5 : WD_Object
                                                                                     where T6 : WD_Object
                                                                                     where T7 : WD_Object {
        if(IsRuntimeA<T1>())         { fnc1(this); }
        else if(IsRuntimeA<T2>())    { fnc2(this); }
        else if(IsRuntimeA<T3>())    { fnc3(this); }
        else if(IsRuntimeA<T4>())    { fnc4(this); }
        else if(IsRuntimeA<T5>())    { fnc5(this); }
        else if(IsRuntimeA<T6>())    { fnc6(this); }
        else if(IsRuntimeA<T7>())    { fnc7(this); }
        else if(defaultFnc != null)  { defaultFnc(this); }                                    
    }
    public void Case<T1,T2,T3,T4,T5,T6,T7,T8>(Action<WD_EditorObject> fnc1,
                                              Action<WD_EditorObject> fnc2,
                                              Action<WD_EditorObject> fnc3,
                                              Action<WD_EditorObject> fnc4,
                                              Action<WD_EditorObject> fnc5,
                                              Action<WD_EditorObject> fnc6,
                                              Action<WD_EditorObject> fnc7,
                                              Action<WD_EditorObject> fnc8,
                                              Action<WD_EditorObject> defaultFnc= null) where T1 : WD_Object
                                                                                        where T2 : WD_Object
                                                                                        where T3 : WD_Object
                                                                                        where T4 : WD_Object
                                                                                        where T5 : WD_Object
                                                                                        where T6 : WD_Object
                                                                                        where T7 : WD_Object
                                                                                        where T8 : WD_Object {
        if(IsRuntimeA<T1>())         { fnc1(this); }
        else if(IsRuntimeA<T2>())    { fnc2(this); }
        else if(IsRuntimeA<T3>())    { fnc3(this); }
        else if(IsRuntimeA<T4>())    { fnc4(this); }
        else if(IsRuntimeA<T5>())    { fnc5(this); }
        else if(IsRuntimeA<T6>())    { fnc6(this); }
        else if(IsRuntimeA<T7>())    { fnc7(this); }
        else if(IsRuntimeA<T8>())    { fnc8(this); }
        else if(defaultFnc != null)  { defaultFnc(this); }                                    
    }

}
