using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class WD_EditorObject {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public int              InstanceId   = -1;
    public int              ParentId     = -1;
    public string           QualifiedType= "";
    public string           Name         = "";
    public bool             IsDirty      = false;
    public bool             IsVisible    = false;
    public Rect             LocalPosition= new Rect(0,0,0,0);

    // Port specific attributes ---------------------------------------------
    public enum EdgeEnum { None, Top, Bottom, Right, Left };
    public EdgeEnum         Edge      = EdgeEnum.None;
    public int              Source= -1;

    // Non-persistant properties --------------------------------------------
    [System.NonSerialized] public bool IsBeingDragged= false;

    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public WD_EditorObject() { Init(); }
    public WD_EditorObject(int id, string name, Type type, int parentId, Rect localPosition) {
        Init();
        InstanceId= id;
        ParentId= parentId;
        Name= name;
        QualifiedType= type.AssemblyQualifiedName;
        IsDirty= true;
        IsVisible= true;
        LocalPosition= localPosition;
        Case<WD_RootNode, WD_Top, WD_Node, WD_Port>(
            (root) => { IsVisible= false; },
            (top)  => { IsVisible= false; },
            (node) => {},
            (port) => {
                port.ExecuteIf<WD_DataPort>(
                    (dataPort) => {
                        dataPort.Case<WD_InDataPort, WD_OutDataPort, WD_EnablePort>(
                            (inPort)     => { Edge= EdgeEnum.Left; },
                            (outPort)    => { Edge= EdgeEnum.Right; },
                            (enablePort) => { Edge= EdgeEnum.Top; }
                        );
                    }
                );                
            }
        );
    }
    // ----------------------------------------------------------------------
    public void Init() {
        InstanceId= -1;
        ParentId= -1;
        QualifiedType= "";
        Name= "";
        IsDirty= false;
        IsVisible= false;
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
    
    // ======================================================================
    // Object Serialization
    // ----------------------------------------------------------------------
    public void Serialize(WD_Object obj, int id) {
        IsDirty= true;
        InstanceId= obj.InstanceId= id;
        ParentId= obj.Parent != null ? obj.Parent.InstanceId : -1;
        Type t= obj.GetType();
        QualifiedType= t.AssemblyQualifiedName;
        Name= obj.name;
        obj.Case<WD_RootNode, WD_Top, WD_Node, WD_Port>(
            (root) => { IsVisible= false; },
            (top)  => { IsVisible= false; },
            (node) => { },
            (port) => {
                port.ExecuteIf<WD_DataPort>(
                    (dataPort) => {
                        if(dataPort.Source != null) Source= dataPort.Source.InstanceId;
                        dataPort.Case<WD_InDataPort, WD_OutDataPort, WD_EnablePort>(
                            (inPort)     => { Edge= EdgeEnum.Left; },
                            (outPort)    => { Edge= EdgeEnum.Right; },
                            (enablePort) => { Edge= EdgeEnum.Top; }
                        );
                    }
                );
            }
        );
    }
    // ----------------------------------------------------------------------
    public WD_Object Deserialize() {
        return null;
    }

    // ----------------------------------------------------------------------
    public bool IsRuntimeA(Type t) {
        for(Type rt= RuntimeType; rt != null; rt= rt.BaseType) {
            if(t == rt) return true;
        }
        return false;
    }
    public bool IsRuntimeA<T>() where T : class {
        return IsRuntimeA(typeof(T));
    }
    
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

}
