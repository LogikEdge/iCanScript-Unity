using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class iCS_EditorObject {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public iCS_ObjectTypeEnum    ObjectType    = iCS_ObjectTypeEnum.Unknown;
    public int                   InstanceId    = -1;
    public int                   ParentId      = -1;
    public string                QualifiedType = "";
    public string                RawName       = "";
    public Rect                  LocalPosition = new Rect(0,0,0,0);
    public iCS_DisplayOptionEnum DisplayOption = iCS_DisplayOptionEnum.Normal;
    public bool                  IsNameEditable= true;

	// Node specific attributes ---------------------------------------------
	public string				 MethodName= null;
	public int					 NbOfParams= 0;
    public string                IconGUID  = null;
    public string                RawToolTip= null;

    // Port specific attributes ---------------------------------------------
    public enum EdgeEnum { None, Top, Bottom, Right, Left };
    public EdgeEnum              Edge               = EdgeEnum.None;
    public int                   Source             = -1;
    public int                   PortIndex          = -1;
	public string				 InitialValueArchive= null;

    // State specific attributes ---------------------------------------------
    public bool                  IsRawEntryState= false;
    
    // Non-persistant properties --------------------------------------------
    [System.NonSerialized] public bool IsFloating= false;
    [System.NonSerialized] public bool IsDirty   = false;

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
		// Commmon
        instance.DisplayOption= toClone.DisplayOption;
        instance.IsNameEditable= toClone.IsNameEditable;
        instance.LocalPosition= toClone.LocalPosition;
		// Node
		instance.MethodName= toClone.MethodName;
		instance.NbOfParams= toClone.NbOfParams;
        instance.IconGUID= toClone.IconGUID;
        instance.RawToolTip= toClone.RawToolTip;
		// Port
        instance.Edge= toClone.Edge;
        instance.PortIndex= toClone.PortIndex;
        if(instance.IsInDataPort && toClone.Source == -1 && !iCS_Types.IsA<UnityEngine.Object>(toClone.RuntimeType)) {
            instance.InitialValueArchive= toClone.InitialValueArchive;
        }
        return instance;
    }
    // ----------------------------------------------------------------------
    public void Reset() {
		// Common
        ObjectType= iCS_ObjectTypeEnum.Unknown;
        InstanceId= -1;
        ParentId= -1;
        QualifiedType= "";
		RawName= "";
        LocalPosition= new Rect(0,0,0,0);
        DisplayOption= iCS_DisplayOptionEnum.Normal;
        IsNameEditable= true;
		// Node
		MethodName= null;
		NbOfParams= 0;
        IconGUID= null;
        RawToolTip = null;
		// Port
        Edge= EdgeEnum.None;
        Source= -1;
		PortIndex= -1;
		InitialValueArchive= null;
		// State
		IsRawEntryState= false;
		// Transient
        IsFloating= false;
        IsDirty= false;
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
	public bool IsDataNode				{ get { return WD.IsDataNode(this); }}
	public bool IsStateChartNode	    { get { return WD.IsStateChartNode(this); }}
    public bool IsBehaviour             { get { return WD.IsBehaviour(this); }}
    public bool IsStateChart            { get { return WD.IsStateChart(this); }}
    public bool IsState                 { get { return WD.IsState(this); }}
    public bool IsModule                { get { return WD.IsModule(this); }}
    public bool IsTransitionModule      { get { return WD.IsTransitionModule(this); }}
    public bool IsTransitionGuard       { get { return WD.IsTransitionGuard(this); }}
    public bool IsTransitionAction      { get { return WD.IsTransitionAction(this); }}
    public bool IsConstructor           { get { return WD.IsConstructor(this); }}
    public bool IsStaticMethod          { get { return WD.IsStaticMethod(this); }}
    public bool IsInstanceMethod        { get { return WD.IsInstanceMethod(this); }}
    public bool IsStaticField           { get { return WD.IsStaticField(this); }}
    public bool IsInstanceField         { get { return WD.IsInstanceField(this); }}
    public bool IsConversion            { get { return WD.IsConversion(this); }}
    public bool IsPort                  { get { return WD.IsPort(this); }}
    public bool IsDataPort              { get { return WD.IsDataPort(this); }}
    public bool IsFunctionPort          { get { return WD.IsFunctionPort(this); }}
    public bool IsModulePort            { get { return WD.IsModulePort(this); }}
    public bool IsDynamicModulePort     { get { return WD.IsDynamicModulePort(this); }}
    public bool IsStaticModulePort      { get { return WD.IsStaticModulePort(this); }}
    public bool IsStatePort             { get { return WD.IsStatePort(this); }}
    public bool IsTransitionPort        { get { return WD.IsTransitionPort(this); }}
    public bool IsEnablePort            { get { return WD.IsEnablePort(this); }}
    public bool IsOutputPort            { get { return WD.IsOutputPort(this); }}
    public bool IsInputPort             { get { return WD.IsInputPort(this); }}
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
    public bool IsEntryState            { get { return IsState && IsRawEntryState; } set { IsRawEntryState= value; }}
    public bool IsClassModule           { get { return IsModule && RuntimeType != typeof(iCS_Module); }}
    
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
            return iCS_Types.TypeName(RuntimeType);
        }
    }
    public string Name {
        get {
            return NameEmpty ? ":"+TypeName : (IsClassModule ? RawName+":"+TypeName : RawName);
        }
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

    // ----------------------------------------------------------------------
	public FieldInfo GetFieldInfo() {
        if(MethodName == null) return null;
		Type classType= RuntimeType;
        FieldInfo field= classType.GetField(MethodName);
        if(field == null) {
            Debug.LogWarning("iCanScript: Unable to extract FieldInfo from RuntimeDesc: "+MethodName);                
        }
        return field;		            
	}

    // ----------------------------------------------------------------------
	public MethodBase GetMethodBase(List<iCS_EditorObject> editorObjects) {
        // Extract MethodBase for constructor.
        MethodBase method= null;
		Type classType= RuntimeType;
        if(ObjectType == iCS_ObjectTypeEnum.Constructor) {
            method= classType.GetConstructor(GetParamTypes(editorObjects));
            if(method == null) {
                string signature="(";
                bool first= true;
                foreach(var param in GetParamTypes(editorObjects)) {
                    if(first) { first= false; } else { signature+=", "; }
                    signature+= param.Name;
                }
                signature+=")";
                Debug.LogWarning("Unable to extract constructor: "+classType.Name+signature);
            }
            return method;
        }
        // Extract MethodBase for class methods.
        if(MethodName == null) return null;
		Type[] paramTypes= GetParamTypes(editorObjects);
        method= classType.GetMethod(MethodName, paramTypes);            
        if(method == null) {
            string signature="(";
            bool first= true;
            foreach(var param in paramTypes) {
                if(first) { first= false; } else { signature+=", "; }
                signature+= param.Name;
            }
            signature+=")";
            Debug.LogWarning("iCanScript: Unable to extract MethodInfo from RuntimeDesc: "+MethodName+signature);
        }
        return method;		            
	}
	public Type[] GetParamTypes(List<iCS_EditorObject> editorObjects) {
		iCS_EditorObject[] ports= GetChildPorts(editorObjects);
		Type[] result= new Type[NbOfParams];
		for(int i= 0; i < result.Length; ++i) {
			result[i]= ports[i].RuntimeType;
		}
		return result;	        
	}
	iCS_EditorObject[] GetChildPorts(List<iCS_EditorObject> editorObjects) {
		List<iCS_EditorObject> ports= new List<iCS_EditorObject>();
		// Get all child data ports.
		int nodeId= InstanceId;
		foreach(var port in editorObjects) {
			if(port.ParentId != nodeId) continue;
			if(!port.IsDataPort) continue;
			if(port.IsEnablePort) continue;
			ports.Add(port);
		}
		// Sort child ports according to index.
		iCS_EditorObject[] result= ports.ToArray();
		Array.Sort(result, (x,y)=> x.PortIndex - y.PortIndex);
		return result;            
	}

}
