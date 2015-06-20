using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using iCanScript;
using iCanScript.Internal.Engine;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Engine {
    [Serializable]
    public enum PortSpecification {
        Target= 0, Self, Parameter, ReturnValue,
        LocalVariable= 10,
        PublicVariable= 20, PrivateVariable,
        StaticPublicVariable= 30, StaticPrivateVariable,
        Constant= 40,
        Enable= 50, Trigger,
		Owner= 60,
        Default= 1000
    };
    [Serializable]
    public enum NodeSpecification {
        Class= 200, Struct, Enum, Interface,
        Constructor= 300, StaticConstructor,
        PublicFunction= 400, PublicStaticFunction,
        PublicVirtualFunction, PublicOverrideFunction,  
        PublicNewFunction, PublicNewStaticFunction,
        PrivateFunction= 420, PrivateStaticFunction,
        ProtectedFunction= 440,
        ProtectedVirtualFunction, ProtectedOverrideFunction,
        ProtectedNewFunction, ProtectedNewStaticFunction,
        FunctionCall= 600,
        EventHandler= 700,
        Package= 800,
        StateChart= 900, State,
        Default= 1000
    };

    [Serializable]
    public class iCS_EngineObject {
        // ======================================================================
        // Database Fields
        // ----------------------------------------------------------------------
        public string                RawName            = "";
        public VSObjectType          ObjectType         = VSObjectType.Unknown;
        public int                   InstanceId         = -1;
        public int                   ParentId           = -1;
        public string                QualifiedType      = "";
    	public Vector2				 LocalAnchorPosition= Vector2.zero;
        public iCS_DisplayOptionEnum DisplayOption      = iCS_DisplayOptionEnum.Unfolded;

    	// Node specific attributes ---------------------------------------------
        public NodeSpecification     NodeSpec         = NodeSpecification.Default;
    	public string				 MethodName       = null;
    	public int					 NbOfParams       = 0;     // Also used for port group
        public string                IconGUID         = null;
        public string                Description      = null;
        public int                   LayoutPriority   = 0;

        // Port specific attributes ---------------------------------------------
        public PortSpecification     PortSpec           = PortSpecification.Default;
        public int                   SourceId           = -1;
        public int                   PortIndex          = -1;
    	public string				 InitialValueArchive= null;

        // State specific attributes ---------------------------------------------
        public bool                  IsEntryState= false;

        // ======================================================================
        // Accessors
        // ----------------------------------------------------------------------
        public bool   IsValid         { get { return InstanceId != -1; }}
        public bool   IsParentValid   { get { return ParentId != -1; }}
        public bool   IsSourceValid   { get { return SourceId != -1; }}
        public bool   IsNameEmpty     { get { return string.IsNullOrEmpty(RawName); }}
        public string TypeName        { get { return iCS_Types.TypeName(RuntimeType);}} 
        public Type   RuntimeType     {
            get {
                bool conversionPerformed= false;
                var ty= iCS_Types.TypeFromAssemblyQualifiedName(QualifiedType, out conversionPerformed);
                if(conversionPerformed) {
                    QualifiedType= ty.AssemblyQualifiedName;
                }
                return ty;
            }
        }
        // Node Specific accesors ------------------------------------------------
        public int ProxyOriginalNodeId {
            get { return SourceId; }
            set { SourceId= value; }
        }
        public string ProxyOriginalVisualScriptTag {
            get { return InitialValueArchive; }
            set { InitialValueArchive= value; }
        }
    
        // Port Specific accesors ------------------------------------------------
        public int PortGroup {
            get { return NbOfParams; }
            set { NbOfParams= value; }
        }
        public iCS_EdgeEnum Edge {
            get {
                var edge= LocalAnchorPosition.x;
                if(Math3D.IsEqual(edge, 1f)) return iCS_EdgeEnum.Left;
                if(Math3D.IsEqual(edge, 2f)) return iCS_EdgeEnum.Right;
                if(Math3D.IsEqual(edge, 3f)) return iCS_EdgeEnum.Top;
                if(Math3D.IsEqual(edge, 4f)) return iCS_EdgeEnum.Bottom;
                return iCS_EdgeEnum.None;
            }
            set {
                switch(value) {
                    case iCS_EdgeEnum.Left  : LocalAnchorPosition.x= 1f; break;
                    case iCS_EdgeEnum.Right : LocalAnchorPosition.x= 2f; break;
                    case iCS_EdgeEnum.Top   : LocalAnchorPosition.x= 3f; break;
                    case iCS_EdgeEnum.Bottom: LocalAnchorPosition.x= 4f; break;
                    default                 : LocalAnchorPosition.x= 0f; break;
                }
            }
        }
        public float PortPositionRatio {
            get { return LocalAnchorPosition.y; }
            set { LocalAnchorPosition.y= value; }
        }

        // ======================================================================
        // Initialization
        // ----------------------------------------------------------------------
        public iCS_EngineObject(int id, string name, Type type, int parentId, VSObjectType objectType) {
            Reset();
            ObjectType= objectType;
            InstanceId= id;
            ParentId= parentId;
            RawName= name;
            QualifiedType= type.AssemblyQualifiedName;
            LocalAnchorPosition= Vector2.zero;
            if(IsDataOrControlPort) {
                Edge= IsInputPort ? (IsEnablePort ? iCS_EdgeEnum.Top : iCS_EdgeEnum.Left) : iCS_EdgeEnum.Right;
            }
        }
        // ----------------------------------------------------------------------
    	public iCS_EngineObject() {
    		Reset();
    	}
        // ----------------------------------------------------------------------
    	public static iCS_EngineObject CreateInvalidInstance() {
    		return new iCS_EngineObject();
    	}
        // ----------------------------------------------------------------------
    	public void DestroyInstance() {
    		Reset();
    	}
        // ----------------------------------------------------------------------
        public iCS_EngineObject Clone() {
            return CopyTo(new iCS_EngineObject());
        }
        // ----------------------------------------------------------------------
        public static iCS_EngineObject Clone(int id, iCS_EngineObject toClone, iCS_EngineObject parent) {
            iCS_EngineObject instance= new iCS_EngineObject(id, toClone.RawName, toClone.RuntimeType, parent != null ? parent.InstanceId : -1, toClone.ObjectType);
    		// Commmon
            instance.DisplayOption= toClone.DisplayOption;
    		instance.LocalAnchorPosition= toClone.LocalAnchorPosition;
    		// Node
            instance.NodeSpec   = toClone.NodeSpec;
    		instance.MethodName = toClone.MethodName;
    		instance.NbOfParams = toClone.NbOfParams;
            instance.IconGUID   = toClone.IconGUID;
            instance.Description= toClone.Description;
    		// Port
            instance.PortSpec= toClone.PortSpec;
            instance.Edge= toClone.Edge;
            instance.PortIndex= toClone.PortIndex;
            if(instance.IsInDataOrControlPort && toClone.SourceId == -1 && !iCS_Types.IsA<UnityEngine.Object>(toClone.RuntimeType)) {
                instance.InitialValueArchive= toClone.InitialValueArchive;
            }
            return instance;
        }
        // ----------------------------------------------------------------------
    	public iCS_EngineObject CopyTo(iCS_EngineObject to) {
            to.ObjectType= ObjectType;
            to.NodeSpec= NodeSpec;
            to.PortSpec= PortSpec;
            to.InstanceId= InstanceId;
            to.ParentId= ParentId;
            to.QualifiedType= QualifiedType;
            to.RawName= RawName;
            to.LocalAnchorPosition= LocalAnchorPosition;
            to.DisplayOption= DisplayOption;
            to.MethodName= MethodName;
            to.NbOfParams= NbOfParams;
            to.IconGUID= IconGUID;
            to.Description= Description;
            to.LayoutPriority= LayoutPriority;
            to.SourceId= SourceId;
            to.PortIndex= PortIndex;
            to.InitialValueArchive= InitialValueArchive;
            to.IsEntryState= IsEntryState;
    		return to;
    	}
        // ----------------------------------------------------------------------
        public void Reset() {
    		// Common
            ObjectType= VSObjectType.Unknown;
            NodeSpec= NodeSpecification.Default;
            PortSpec= PortSpecification.Default;
            InstanceId= -1;
            ParentId= -1;
            QualifiedType= "";
    		RawName= "";
            LocalAnchorPosition= Vector2.zero;
            LayoutPriority= 0;
            DisplayOption= iCS_DisplayOptionEnum.Unfolded;
    		// Node
    		MethodName= null;
    		NbOfParams= 0;
            IconGUID= null;
            Description = null;
    		// Port
            Edge= iCS_EdgeEnum.None;
            SourceId= -1;
    		PortIndex= -1;
    		InitialValueArchive= null;
    		// State
    		IsEntryState= false;
        }
    
        // ----------------------------------------------------------------------
        // Object Type Acessor
        public bool IsNode                     { get { return iCS_ObjectType.IsNode(this); }}
    	public bool IsKindOfState   	       { get { return iCS_ObjectType.IsKindOfState(this); }}
        public bool IsBehaviour                { get { return iCS_ObjectType.IsBehaviour(this); }}
        public bool IsInlineCode               { get { return iCS_ObjectType.IsInlineCode(this); }}
        public bool IsStateChart               { get { return iCS_ObjectType.IsStateChart(this); }}
        public bool IsState                    { get { return iCS_ObjectType.IsState(this); }}
        public bool IsPackage                  { get { return iCS_ObjectType.IsPackage(this); }}
        public bool IsMux                      { get { return iCS_ObjectType.IsMux(this); }}
        public bool IsKindOfPackage            { get { return iCS_ObjectType.IsKindOfPackage(this); }}
        public bool IsTransitionPackage        { get { return iCS_ObjectType.IsTransitionPackage(this); }}
        public bool IsKindOfFunction           { get { return iCS_ObjectType.IsKindOfFunction(this); }}
        public bool IsConstructor              { get { return iCS_ObjectType.IsConstructor(this); }}
        public bool IsFunction                 { get { return iCS_ObjectType.IsFunction(this); }}
        public bool IsStaticFunction           { get { return iCS_ObjectType.IsStaticFunction(this); }}
        public bool IsNonStaticFunction        { get { return iCS_ObjectType.IsNonStaticFunction(this); }}
    	public bool IsField					   { get { return iCS_ObjectType.IsField(this); }}
        public bool IsStaticField              { get { return iCS_ObjectType.IsStaticField(this); }}
        public bool IsNonStaticField           { get { return iCS_ObjectType.IsNonStaticField(this); }}
        public bool IsTypeCast                 { get { return iCS_ObjectType.IsTypeCast(this); }}
        public bool IsEventHandler             { get { return iCS_ObjectType.IsEventHandler(this); }}
        public bool IsInstanceNode             { get { return IsPackage && RuntimeType != typeof(iCS_Package); }}
    	public bool IsOnStatePackage           { get { return iCS_ObjectType.IsOnStatePackage(this); }}
        public bool IsOnStateEntryPackage      { get { return iCS_ObjectType.IsOnStateEntryPackage(this); }}
        public bool IsOnStateUpdatePackage     { get { return iCS_ObjectType.IsOnStateUpdatePackage(this); }}
        public bool IsOnStateExitPackage       { get { return iCS_ObjectType.IsOnStateExitPackage(this); }}
        // General Ports
        public bool IsPort                  { get { return iCS_ObjectType.IsPort(this); }}
        public bool IsOutputPort            { get { return iCS_ObjectType.IsOutputPort(this); }}
        public bool IsInputPort             { get { return iCS_ObjectType.IsInputPort(this); }}
        // State Ports
        public bool IsStatePort             { get { return iCS_ObjectType.IsStatePort(this); }}
        public bool IsInStatePort           { get { return iCS_ObjectType.IsInStatePort(this); }}
        public bool IsOutStatePort          { get { return iCS_ObjectType.IsOutStatePort(this); }}
        // Transition Ports
        public bool IsTransitionPort        { get { return iCS_ObjectType.IsTransitionPort(this); }}
        public bool IsInTransitionPort      { get { return iCS_ObjectType.IsInTransitionPort(this); }}
        public bool IsOutTransitionPort     { get { return iCS_ObjectType.IsOutTransitionPort(this); }}
        // Parameter Data Ports
        public bool IsParameterPort     	{ get { return iCS_ObjectType.IsParameterPort(this); }}
        public bool IsInParameterPort   	{ get { return iCS_ObjectType.IsInParameterPort(this); }}
        public bool IsOutParameterPort  	{ get { return iCS_ObjectType.IsOutParameterPort(this); }}
    	public bool IsReturnPort            { get { return iCS_ObjectType.IsReturnPort(this); }}
        // Fix Data Flow Ports
        public bool IsFixDataPort          	{ get { return iCS_ObjectType.IsFixDataPort(this); }}
        public bool IsInFixDataPort         { get { return iCS_ObjectType.IsInFixDataPort(this); }}
        public bool IsOutFixDataPort        { get { return iCS_ObjectType.IsOutFixDataPort(this); }}
        // Dynamic Data Flow Ports
        public bool IsDynamicDataPort       { get { return iCS_ObjectType.IsDynamicDataPort(this); }}
        public bool IsInDynamicDataPort     { get { return iCS_ObjectType.IsInDynamicDataPort(this); }}
        public bool IsOutDynamicDataPort    { get { return iCS_ObjectType.IsOutDynamicDataPort(this); }}
        // Proposed Data Flow Ports
        public bool IsProposedDataPort      { get { return iCS_ObjectType.IsProposedDataPort(this); }}
        public bool IsInProposedDataPort    { get { return iCS_ObjectType.IsInProposedDataPort(this); }}
        public bool IsOutProposedDataPort   { get { return iCS_ObjectType.IsOutProposedDataPort(this); }}
        // Data Flow Ports
        public bool IsDataPort              { get { return iCS_ObjectType.IsDataPort(this); }}
        public bool IsInDataPort            { get { return iCS_ObjectType.IsInDataPort(this); }}
        public bool IsOutDataPort           { get { return iCS_ObjectType.IsOutDataPort(this); }}
        // Control Ports
        public bool IsEnablePort            { get { return iCS_ObjectType.IsEnablePort(this); }}
        public bool IsTriggerPort           { get { return iCS_ObjectType.IsTriggerPort(this); }}
        // Data Flow or Control Ports
        public bool IsDataOrControlPort     { get { return iCS_ObjectType.IsDataOrControlPort(this); }}
        public bool IsInDataOrControlPort   { get { return iCS_ObjectType.IsInDataOrControlPort(this); }}
        public bool IsOutDataOrControlPort  { get { return iCS_ObjectType.IsOutDataOrControlPort(this); }}
        // Mux Ports
    	public bool	IsMuxPort			    { get { return iCS_ObjectType.IsMuxPort(this); }}
    	public bool IsParentMuxPort		    { get { return iCS_ObjectType.IsParentMuxPort(this); }}
    	public bool IsChildMuxPort		    { get { return iCS_ObjectType.IsChildMuxPort(this); }}
    	public bool	IsInMuxPort			    { get { return iCS_ObjectType.IsInMuxPort(this); }}
    	public bool IsInParentMuxPort		{ get { return iCS_ObjectType.IsInParentMuxPort(this); }}
    	public bool IsInChildMuxPort		{ get { return iCS_ObjectType.IsInChildMuxPort(this); }}
    	public bool	IsOutMuxPort			{ get { return iCS_ObjectType.IsOutMuxPort(this); }}
    	public bool IsOutParentMuxPort		{ get { return iCS_ObjectType.IsOutParentMuxPort(this); }}
    	public bool IsOutChildMuxPort		{ get { return iCS_ObjectType.IsOutChildMuxPort(this); }}
        // Instance Ports
    	public bool IsTargetOrSelfPort		{ get { return IsTargetPort || IsSelfPort; }}
    	public bool IsTargetPort		    { get { return iCS_ObjectType.IsTargetPort(this); }}
    	public bool IsSelfPort		        { get { return iCS_ObjectType.IsSelfPort(this); }}
	
        // ======================================================================
        // Feature support
        // ----------------------------------------------------------------------
        public bool SupportsAdditionOfPorts { get { return IsKindOfPackage; }}
        public bool SupportsNestedNodes     { get { return IsKindOfPackage; }}
    
        // ----------------------------------------------------------------------
    	public FieldInfo GetFieldInfo() {
            FieldInfo field= GetFieldInfoNoWarning();
            if(field == null) {
                Debug.LogWarning("iCanScript: Unable to extract FieldInfo from RuntimeDesc: "+MethodName);                
            }
            return field;		            
    	}
        public FieldInfo GetFieldInfoNoWarning() {
            if(MethodName == null) return null;
    		Type classType= RuntimeType;
            return GetFieldInfoRecursive(classType, MethodName);
        }
        public FieldInfo GetFieldInfoRecursive(Type classType, string fieldName) {
            if(classType == null) return null;
            var fieldInfo= classType.GetField(fieldName);
            if(fieldInfo != null) return fieldInfo;
            return GetFieldInfoRecursive(classType.BaseType, fieldName);
        }
    
        // ----------------------------------------------------------------------
    	public MethodBase GetMethodBase(List<iCS_EngineObject> parameters) {
            // Extract MethodBase for constructor.
    		Type classType= RuntimeType;
            if(classType == null) return null;
            MethodBase method= null;
            if(ObjectType == VSObjectType.Constructor) {
                method= classType.GetConstructor(GetParamTypes(parameters));
                if(method == null) {
                    string signature="(";
                    bool first= true;
                    foreach(var param in GetParamTypes(parameters)) {
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
    		Type[] paramTypes= GetParamTypes(parameters);
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
    	public Type[] GetParamTypes(List<iCS_EngineObject> engineObjects) {
    		iCS_EngineObject[] ports= GetChildPortsExcludingControlPorts(engineObjects);
    		Type[] result= new Type[NbOfParams];
    		for(int i= 0; i < P.length(result); ++i) {
    			result[i]= ports[i].RuntimeType;
    		}
    		return result;	        
    	}
    	iCS_EngineObject[] GetChildPortsExcludingControlPorts(List<iCS_EngineObject> engineObjects) {
    		List<iCS_EngineObject> ports= new List<iCS_EngineObject>();
    		// Get all child data ports.
    		int nodeId= InstanceId;
    		foreach(var port in engineObjects) {
    			if(port == null) continue;
    			if(port.ParentId != nodeId) continue;
    			if(!port.IsDataOrControlPort) continue;
    			if(port.IsEnablePort) continue;
    			ports.Add(port);
    		}
    		// Sort child ports according to index.
    		return SortPortsOnIndex(ports.ToArray());            
    	}

        // =======================================================================
        // Utilities
        // -----------------------------------------------------------------------
        public static iCS_EngineObject[] SortPortsOnIndex(iCS_EngineObject[] lst) {
    		Array.Sort(lst, (x,y)=> x.PortIndex - y.PortIndex);
            return lst;
        }
    }
}


