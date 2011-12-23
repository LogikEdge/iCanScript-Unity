using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class iCS_EditorObject {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public iCS_ObjectTypeEnum   ObjectType    = iCS_ObjectTypeEnum.Unknown;
    public int                  InstanceId    = -1;
    public int                  ParentId      = -1;
    public string               QualifiedType = "";
    public string               RuntimeArchive= null;
    public iCS_DisplayOptionEnum DisplayOption = iCS_DisplayOptionEnum.Normal;
    public string               IconGUID      = null;
    public string               RawName       = "";
    public bool                 IsNameEditable= true;
    public string               RawToolTip    = null;
    public bool                 IsDirty       = false;
    public Rect                 LocalPosition = new Rect(0,0,0,0);

    // Port specific attributes ---------------------------------------------
    public enum EdgeEnum { None, Top, Bottom, Right, Left };
    public EdgeEnum     Edge     = EdgeEnum.None;
    public int          Source   = -1;
    public int          PortIndex= -1;

    // State specific attributes ---------------------------------------------
    public bool     IsRawEntryState= false;
    
    // Non-persistant properties --------------------------------------------
    [System.NonSerialized] public bool IsFloating= false;

    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public iCS_EditorObject(int id, string name, Type type, int parentId, iCS_ObjectTypeEnum objectType, Rect localPosition) {
        Reset();
        ObjectType= objectType;
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
    public static iCS_EditorObject Clone(int id, iCS_EditorObject toClone, iCS_EditorObject parent, Rect localPosition) {
        iCS_EditorObject instance= new iCS_EditorObject(id, toClone.Name, toClone.RuntimeType, parent != null ? parent.InstanceId : -1, toClone.ObjectType, localPosition);
        instance.RuntimeArchive= toClone.RuntimeArchive;
        instance.DisplayOption= toClone.DisplayOption;
        instance.IconGUID= toClone.IconGUID;
        instance.IsNameEditable= toClone.IsNameEditable;
        instance.RawToolTip= toClone.RawToolTip;
        instance.Edge= toClone.Edge;
        instance.PortIndex= toClone.PortIndex;
        return instance;
    }
    // ----------------------------------------------------------------------
    public void Reset() {
        ObjectType= iCS_ObjectTypeEnum.Unknown;
        InstanceId= -1;
        ParentId= -1;
        QualifiedType= "";
        RuntimeArchive= null;
        DisplayOption= iCS_DisplayOptionEnum.Normal;
        IconGUID= null;
        Name= "";
        IsDirty= false;
        LocalPosition= new Rect(0,0,0,0);
        Edge= EdgeEnum.None;
        Source= -1;
        IsFloating= false;
        IsNameEditable= true;
        ToolTip = null;
    }
    
    // ----------------------------------------------------------------------
    // Display Option Accessor
    public bool IsDisplayedNormally { get { return WD.IsDisplayedNormally(this); }}
    public bool IsMinimized         { get { return WD.IsMinimized(this); }}
    public bool IsFolded            { get { return WD.IsFolded(this); }}
    public void DisplayNormally()   { WD.DisplayNormally(this); }
    public void Fold()              { WD.Fold(this); }
    public void Unfold()            { DisplayNormally(); }
    public void Maximize()          { DisplayNormally(); }
    public void Minimize()          { WD.Minimize(this); }
    // ----------------------------------------------------------------------
    // Object Type Acessor
    public bool IsNode                  { get { return WD.IsNode(this); }}
    public bool IsBehaviour             { get { return WD.IsBehaviour(this); }}
    public bool IsStateChart            { get { return WD.IsStateChart(this); }}
    public bool IsState                 { get { return WD.IsState(this); }}
    public bool IsModule                { get { return WD.IsModule(this); }}
    public bool IsTransitionModule      { get { return WD.IsTransitionModule(this); }}
    public bool IsTransitionGuard       { get { return WD.IsTransitionGuard(this); }}
    public bool IsTransitionAction      { get { return WD.IsTransitionAction(this); }}
    public bool IsStaticMethod          { get { return WD.IsStaticMethod(this); }}
    public bool IsInstanceMethod        { get { return WD.IsInstanceMethod(this); }}
    public bool IsConversion            { get { return WD.IsConversion(this); }}
    public bool IsPort                  { get { return WD.IsPort(this); }}
    public bool IsDataPort              { get { return WD.IsDataPort(this); }}
    public bool IsFieldPort             { get { return WD.IsFieldPort(this); }}
    public bool IsPropertyPort          { get { return WD.IsPropertyPort(this); }}
    public bool IsFunctionPort          { get { return WD.IsFunctionPort(this); }}
    public bool IsModulePort            { get { return WD.IsModulePort(this); }}
    public bool IsDynamicModulePort     { get { return WD.IsDynamicModulePort(this); }}
    public bool IsStaticModulePort      { get { return WD.IsStaticModulePort(this); }}
    public bool IsStatePort             { get { return WD.IsStatePort(this); }}
    public bool IsTransitionPort        { get { return WD.IsTransitionPort(this); }}
    public bool IsEnablePort            { get { return WD.IsEnablePort(this); }}
    public bool IsOutputPort            { get { return WD.IsOutputPort(this); }}
    public bool IsInputPort             { get { return WD.IsInputPort(this); }}
    public bool IsInFieldPort           { get { return WD.IsInFieldPort(this); }}
    public bool IsOutFieldPort          { get { return WD.IsOutFieldPort(this); }}
    public bool IsInPropertyPort        { get { return WD.IsInPropertyPort(this); }}
    public bool IsOutPropertyPort       { get { return WD.IsOutPropertyPort(this); }}
    public bool IsInFunctionPort        { get { return WD.IsInFunctionPort(this); }}
    public bool IsOutFunctionPort       { get { return WD.IsOutFunctionPort(this); }}
    public bool IsInModulePort          { get { return WD.IsInModulePort(this); }}
    public bool IsOutModulePort         { get { return WD.IsOutModulePort(this); }}
    public bool IsInDynamicModulePort   { get { return WD.IsInDynamicModulePort(this); }}
    public bool IsOutDynamicModulePort  { get { return WD.IsOutDynamicModulePort(this); }}
    public bool IsInStaticModulePort    { get { return WD.IsInStaticModulePort(this); }}
    public bool IsOutStaticModulePort   { get { return WD.IsOutStaticModulePort(this); }}
    public bool IsInStatePort           { get { return WD.IsInStatePort(this); }}
    public bool IsOutStatePort          { get { return WD.IsOutStatePort(this); }}
    public bool IsInDataPort            { get { return WD.IsInDataPort(this); }}
    public bool IsOutDataPort           { get { return WD.IsOutDataPort(this); }}
    public bool IsInTransitionPort      { get { return WD.IsInTransitionPort(this); }}
    public bool IsOutTransitionPort     { get { return WD.IsOutTransitionPort(this); }}
    public bool IsEntryState            { get { return IsState && IsRawEntryState; }}
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public bool IsValid         { get { return InstanceId != -1; }}
    public bool IsParentValid   { get { return ParentId != -1; }}
    public bool IsSourceValid   { get { return Source != -1; }}
    public bool NameEmpty       { get { return RawName == null || RawName == ""; }}
    public bool ToolTipEmpty    { get { return RawToolTip == null || RawToolTip == ""; }}
    public Type RuntimeType     { get { return Type.GetType(QualifiedType); }}
    public string TypeName {
        get {
            Type type= RuntimeType;
            if(type == null) return "void";
            if(type == typeof(float)) return "float";
            return type.Name;
        }
    }
    public string Name {
        get { return NameEmpty ? TypeName : RawName; }
        set { RawName= value; }
    }
    public string ToolTip {
        get { return ToolTipEmpty ? (NameEmpty ? TypeName : Name+":"+TypeName) : RawToolTip; }
        set { RawToolTip= value; }
    }
    public bool IsOnTopEdge         { get { return Edge == EdgeEnum.Top; }}
    public bool IsOnBottomEdge      { get { return Edge == EdgeEnum.Bottom; }}
    public bool IsOnRightEdge       { get { return Edge == EdgeEnum.Right; }}
    public bool IsOnLeftEdge        { get { return Edge == EdgeEnum.Left; }}
    public bool IsOnHorizontalEdge  { get { return IsOnTopEdge   || IsOnBottomEdge; }}
    public bool IsOnVerticalEdge    { get { return IsOnRightEdge || IsOnLeftEdge; }}

}
