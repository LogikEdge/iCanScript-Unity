using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class WD_EditorObject {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public WD_ObjectTypeEnum    ObjectType    = WD_ObjectTypeEnum.Unknown;
    public WD_DisplayOptionEnum DisplayOption = WD_DisplayOptionEnum.Normal;
    public string               Icon          = null;
    public int                  InstanceId    = -1;
    public int                  ParentId      = -1;
    public string               QualifiedType = "";
    public string               Name          = "";
    public bool                 IsNameEditable= true;
    public bool                 IsDirty       = false;
    public Rect                 LocalPosition = new Rect(0,0,0,0);

    // Port specific attributes ---------------------------------------------
    public enum EdgeEnum { None, Top, Bottom, Right, Left };
    public EdgeEnum         Edge      = EdgeEnum.None;
    public int              Source= -1;

    // Non-persistant properties --------------------------------------------
    [System.NonSerialized] public bool IsBeingDragged= false;

    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public WD_EditorObject(int id, string name, Type type, int parentId, WD_ObjectTypeEnum objectType, Rect localPosition, string icon= null) {
        Reset();
        ObjectType= objectType;
        Icon= icon;
        InstanceId= id;
        ParentId= parentId;
        Name= name;
        QualifiedType= type.AssemblyQualifiedName;
        IsDirty= true;
        LocalPosition= localPosition;
        if(IsDataPort) {
            Edge= IsInputPort ? (IsEnablePort ? EdgeEnum.Top : EdgeEnum.Left) : EdgeEnum.Right;
        }
    }
    // ----------------------------------------------------------------------
    public void Reset() {
        ObjectType= WD_ObjectTypeEnum.Unknown;
        DisplayOption= WD_DisplayOptionEnum.Normal;
        Icon= null;
        InstanceId= -1;
        ParentId= -1;
        QualifiedType= "";
        Name= "";
        IsDirty= false;
        LocalPosition= new Rect(0,0,0,0);
        Edge= EdgeEnum.None;
        Source= -1;
        IsBeingDragged= false;
        IsNameEditable= true;
    }
//    // ----------------------------------------------------------------------
//    public WD_Object CreateRuntimeObject() {
//        WD_Object rtObject;
//        if(IsRuntimeA<ScriptableObject>()) {
//            rtObject= ScriptableObject.CreateInstance(RuntimeType) as WD_Object;
//        }
//        else {
//            rtObject= Activator.CreateInstance(RuntimeType) as WD_Object;            
//        }
//        if(rtObject == null) {
//            Debug.LogError("Unable to create an instance of : "+QualifiedType);
//        }
//        rtObject.Name= Name;
//        rtObject.InstanceId= InstanceId;
//        return rtObject;
//    }
//    
//    // ----------------------------------------------------------------------
//    public bool IsRuntimeA(Type t) {
//        return Inf.IsA(RuntimeType, t);
//    }
//    public bool IsRuntimeA<T>() where T : class {
//        return IsRuntimeA(typeof(T));
//    }
    // ----------------------------------------------------------------------
    // Display Option Accessor
    public bool IsDisplayedNormally { get { return WD.IsDisplayedNormally(this); }}
    public bool IsHidden            { get { return WD.IsHidden(this); }}
    public bool IsMinimized         { get { return WD.IsMinimized(this); }}
    public bool IsFolded            { get { return WD.IsFolded(this); }}
    public void DisplayNormally()   { WD.DisplayNormally(this); }
    public void Fold()              { WD.Fold(this); }
    public void Unfold()            { DisplayNormally(); }
    public void Maximize()          { DisplayNormally(); }
    public void Minimize()          { WD.Minimize(this); }
    public void Hide()              { WD.Hide(this); }
    public void Unhide()            { DisplayNormally(); }
    // ----------------------------------------------------------------------
    // Object Type Acessor
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

}
