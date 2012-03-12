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
    public iCS_DisplayOptionEnum DisplayOption = iCS_DisplayOptionEnum.Maximized;
    public bool                  IsNameEditable= true;

	// Node specific attributes ---------------------------------------------
	public string				 MethodName= null;
	public int					 NbOfParams= 0;     // Also used as port group
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
        DisplayOption= iCS_DisplayOptionEnum.Maximized;
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
    public bool IsMaximized         { get { return iCS_DisplayOption.IsMaximized(this); }}
    public bool IsMinimized         { get { return iCS_DisplayOption.IsMinimized(this); }}
    public bool IsFolded            { get { return iCS_DisplayOption.IsFolded(this); }}
    public void Fold()              { iCS_DisplayOption.Fold(this); }
    public void Unfold()            { Maximize(); }
    public void Maximize()          { iCS_DisplayOption.Maximize(this); }
    public void Minimize()          { iCS_DisplayOption.Minimize(this); }
    // ----------------------------------------------------------------------
    // Object Type Acessor
    public bool IsNode                  { get { return iCS_ObjectType.IsNode(this); }}
	public bool IsStateChartNode	    { get { return iCS_ObjectType.IsStateChartNode(this); }}
    public bool IsBehaviour             { get { return iCS_ObjectType.IsBehaviour(this); }}
    public bool IsStateChart            { get { return iCS_ObjectType.IsStateChart(this); }}
    public bool IsState                 { get { return iCS_ObjectType.IsState(this); }}
    public bool IsModule                { get { return iCS_ObjectType.IsModule(this); }}
    public bool IsTransitionModule      { get { return iCS_ObjectType.IsTransitionModule(this); }}
    public bool IsTransitionGuard       { get { return iCS_ObjectType.IsTransitionGuard(this); }}
    public bool IsTransitionAction      { get { return iCS_ObjectType.IsTransitionAction(this); }}
    public bool IsFunction              { get { return iCS_ObjectType.IsFunction(this); }}
    public bool IsConstructor           { get { return iCS_ObjectType.IsConstructor(this); }}
    public bool IsStaticMethod          { get { return iCS_ObjectType.IsStaticMethod(this); }}
    public bool IsInstanceMethod        { get { return iCS_ObjectType.IsInstanceMethod(this); }}
    public bool IsStaticField           { get { return iCS_ObjectType.IsStaticField(this); }}
    public bool IsInstanceField         { get { return iCS_ObjectType.IsInstanceField(this); }}
    public bool IsTypeCast              { get { return iCS_ObjectType.IsTypeCast(this); }}
    public bool IsPort                  { get { return iCS_ObjectType.IsPort(this); }}
    public bool IsDataPort              { get { return iCS_ObjectType.IsDataPort(this); }}
    public bool IsFunctionPort          { get { return iCS_ObjectType.IsFunctionPort(this); }}
    public bool IsModulePort            { get { return iCS_ObjectType.IsModulePort(this); }}
    public bool IsDynamicModulePort     { get { return iCS_ObjectType.IsDynamicModulePort(this); }}
    public bool IsStaticModulePort      { get { return iCS_ObjectType.IsStaticModulePort(this); }}
    public bool IsStatePort             { get { return iCS_ObjectType.IsStatePort(this); }}
    public bool IsTransitionPort        { get { return iCS_ObjectType.IsTransitionPort(this); }}
    public bool IsEnablePort            { get { return iCS_ObjectType.IsEnablePort(this); }}
    public bool IsOutputPort            { get { return iCS_ObjectType.IsOutputPort(this); }}
    public bool IsInputPort             { get { return iCS_ObjectType.IsInputPort(this); }}
    public bool IsInFunctionPort        { get { return iCS_ObjectType.IsInFunctionPort(this); }}
    public bool IsOutFunctionPort       { get { return iCS_ObjectType.IsOutFunctionPort(this); }}
    public bool IsInModulePort          { get { return iCS_ObjectType.IsInModulePort(this); }}
    public bool IsOutModulePort         { get { return iCS_ObjectType.IsOutModulePort(this); }}
    public bool IsInDynamicModulePort   { get { return iCS_ObjectType.IsInDynamicModulePort(this); }}
    public bool IsOutDynamicModulePort  { get { return iCS_ObjectType.IsOutDynamicModulePort(this); }}
    public bool IsInStaticModulePort    { get { return iCS_ObjectType.IsInStaticModulePort(this); }}
    public bool IsOutStaticModulePort   { get { return iCS_ObjectType.IsOutStaticModulePort(this); }}
    public bool IsInStatePort           { get { return iCS_ObjectType.IsInStatePort(this); }}
    public bool IsOutStatePort          { get { return iCS_ObjectType.IsOutStatePort(this); }}
    public bool IsInDataPort            { get { return iCS_ObjectType.IsInDataPort(this); }}
    public bool IsOutDataPort           { get { return iCS_ObjectType.IsOutDataPort(this); }}
    public bool IsInTransitionPort      { get { return iCS_ObjectType.IsInTransitionPort(this); }}
    public bool IsOutTransitionPort     { get { return iCS_ObjectType.IsOutTransitionPort(this); }}
    public bool IsEntryState            { get { return IsState && IsRawEntryState; } set { IsRawEntryState= value; }}
    public bool IsClassModule           { get { return IsModule && RuntimeType != typeof(iCS_Module); }}
    
    // ======================================================================
    // Feature support
    // ----------------------------------------------------------------------
    public bool SupportsAdditionOfPorts { get { return IsModule; }}
    public bool SupportsNestedNodes     { get { return IsModule; }}
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public int  PortGroup       { get { return NbOfParams; } set { NbOfParams= value; }}
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
