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
    public bool             IsDirty      = true;
    public bool             IsVisible    = true;
    public Rect             LocalPosition= new Rect(0,0,0,0);

    // Port specific attributes ---------------------------------------------
    public enum EdgeEnum { None, Top, Bottom, Right, Left };
    public EdgeEnum         Edge      = EdgeEnum.None;
    public int              PortSource= -1;

    // Non-persistant properties --------------------------------------------
    [System.NonSerialized] public bool IsBeingDragged= false;

    
    // ======================================================================
    // Object Serialization
    // ----------------------------------------------------------------------
    public void Serialize(WD_Object obj, int id) {
        InstanceId= obj.InstanceId= id;
        ParentId= obj.Parent != null ? obj.Parent.InstanceId : -1;
        Type t= obj.GetType();
        QualifiedType= t.AssemblyQualifiedName;
        Name= obj.name;
        obj.Case<WD_Node, WD_Port>(
            (node) => { LocalPosition= node.LocalPosition; },
            (port) => {
                LocalPosition.x= port.LocalPosition.x;
                LocalPosition.y= port.LocalPosition.y;
                port.ExecuteIf<WD_DataPort>(
                    (dataPort) => { if(dataPort.Source != null) PortSource= dataPort.Source.InstanceId; }
                );
                switch(port.Edge) {
                    case WD_Port.EdgeEnum.Top   : Edge= EdgeEnum.Top; break;
                    case WD_Port.EdgeEnum.Bottom: Edge= EdgeEnum.Bottom; break;
                    case WD_Port.EdgeEnum.Right : Edge= EdgeEnum.Right; break;
                    case WD_Port.EdgeEnum.Left  : Edge= EdgeEnum.Left; break;                    
                }
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
    public bool IsRuntimeA<T>() where T : WD_Object {
        return IsRuntimeA(typeof(T));
    }
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
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
    public bool IsOnTopEdge     { get { return Edge == EdgeEnum.Top; }}
    public bool IsOnBottomEdge  { get { return Edge == EdgeEnum.Bottom; }}
    public bool IsOnRightEdge   { get { return Edge == EdgeEnum.Right; }}
    public bool IsOnLeftEdge    { get { return Edge == EdgeEnum.Left; }}
}
