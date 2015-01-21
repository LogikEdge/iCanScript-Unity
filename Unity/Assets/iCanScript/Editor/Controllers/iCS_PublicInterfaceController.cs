//#define DEBUG
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

public static class iCS_PublicInterfaceController {
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    static iCS_PublicInterfaceController() {
        // -- Verify proper creation order --
        if(iCS_SceneController.VisualScriptsInScene == null) {
            Debug.LogError("iCanScript: Please move PublicInterfaceController after the SceneController in AppController");
        }
        // -- Events to refresh scene content information --
        iCS_SystemEvents.OnSceneChanged    += OnSceneChange;
        iCS_SystemEvents.OnHierarchyChanged+= OnSceneLoad;
        iCS_SystemEvents.OnProjectChanged  += OnSceneLoad;
        // -- Events to refresh visual script information --
		iCS_SystemEvents.OnVisualScriptSaved			   += OnVisualScriptSaved;
        iCS_SystemEvents.OnVisualScriptUndo                += OnVisualScriptUndo;
        iCS_SystemEvents.OnVisualScriptElementAdded        += OnVisualScriptElementAdded;
        iCS_SystemEvents.OnVisualScriptElementWillBeRemoved+= OnVisualScriptElementWillBeRemoved;
        iCS_SystemEvents.OnVisualScriptElementNameChanged  += OnVisualScriptElementNameChanged;

        // -- Force an initial refresh of the scene info --
        OnSceneLoad();
    }
    public static void Start() {}
    public static void Shutdown() {
        // -- Unregister events to refresh scene content information --
        iCS_SystemEvents.OnSceneChanged    -= OnSceneLoad;
        iCS_SystemEvents.OnHierarchyChanged-= OnSceneLoad;
        iCS_SystemEvents.OnProjectChanged  -= OnSceneLoad;
        // -- Unregister events to refresh visual script information --
		iCS_SystemEvents.OnVisualScriptSaved			   -= OnVisualScriptSaved;
        iCS_SystemEvents.OnVisualScriptUndo                -= OnVisualScriptUndo;
        iCS_SystemEvents.OnVisualScriptElementAdded        -= OnVisualScriptElementAdded;
        iCS_SystemEvents.OnVisualScriptElementWillBeRemoved-= OnVisualScriptElementWillBeRemoved;
        iCS_SystemEvents.OnVisualScriptElementNameChanged  -= OnVisualScriptElementNameChanged;
    }
    
    // ======================================================================
    // PROCESS EVENTS
    // ----------------------------------------------------------------------
    static void OnSceneChange() {
        RebuildAndValidatePublicInterfaceInfo();
        OnSceneLoad();
    }
    static void OnSceneLoad() {
        var timedAction= iCS_TimerService.CreateTimedAction(2f, RebuildAndValidatePublicInterfaceInfo);
        timedAction.Schedule();
    }
    // ----------------------------------------------------------------------
    static void OnVisualScriptSaved(iCS_IStorage iStorage) {
		switch(ourNextOperation) {
			case OperationEnum.RebuildInfo: {
				RebuildAndValidatePublicInterfaceInfo();
				break;
			}
			case OperationEnum.NodeNameChange: {
                ChangeNodeName(ourOperationObject);
				RebuildAndValidatePublicInterfaceInfo();
				break;
			}
			case OperationEnum.PortNameChange: {
				RebuildAndValidatePublicInterfaceInfo();
				break;
			}
			case OperationEnum.PortAdded: {
				RebuildAndValidatePublicInterfaceInfo();
				break;
			}
			case OperationEnum.PortRemoved: {
				RebuildAndValidatePublicInterfaceInfo();
				break;
			}
			case OperationEnum.OfNoInterrest: {
				RebuildAndValidatePublicInterfaceInfo();
				break;
			}
			default: {
				RebuildAndValidatePublicInterfaceInfo();
				break;
			}			
		}
		// -- Default to rebuild & validate the entire scene --
		SetNextOperation(OperationEnum.RebuildInfo);
    }
    // ----------------------------------------------------------------------
    static void OnVisualScriptUndo(iCS_IStorage iStorage) {
        RebuildAndValidatePublicInterfaceInfo();
    }
    // ----------------------------------------------------------------------
    static void OnVisualScriptElementAdded(iCS_EditorObject element) {
		// -- Processing port addition on public function --
		if(element.IsDataPort) {
			var parent= element.ParentNode;
			if(parent.IsPublicFunction) {
				SetNextOperation(OperationEnum.PortAdded, element);
			}
			else {
				SetNextOperation(OperationEnum.OfNoInterrest);
			}
			return;
		}
		// -- Process public interface node addition --
		if(IsPublicObject(element)) {
			SetNextOperation(OperationEnum.RebuildInfo);
		}
		else {
			SetNextOperation(OperationEnum.OfNoInterrest);
		}
    }
    // ----------------------------------------------------------------------
    static void OnVisualScriptElementWillBeRemoved(iCS_EditorObject element) {
		// -- Processing port addition on public function --
		if(element.IsDataPort) {
			var parent= element.ParentNode;
			if(parent.IsPublicFunction) {
				SetNextOperation(OperationEnum.PortRemoved, element);
			}
			else {
				SetNextOperation(OperationEnum.OfNoInterrest);
			}
			return;
		}
		// -- Process public interface node addition --
		if(IsPublicObject(element)) {
			SetNextOperation(OperationEnum.RebuildInfo);
		}
		else {
			SetNextOperation(OperationEnum.OfNoInterrest);
		}
    }
    // ----------------------------------------------------------------------
    static void OnVisualScriptElementNameChanged(iCS_EditorObject element) {
		// -- Processing port addition on public function --
		if(element.IsDataPort) {
			var parent= element.ParentNode;
			if(parent.IsPublicFunction) {
				SetNextOperation(OperationEnum.PortNameChange, element);
			}
			else {
				SetNextOperation(OperationEnum.OfNoInterrest);
			}
			return;
		}
		// -- Process public interface node addition --
		if(IsPublicObject(element)) {
			SetNextOperation(OperationEnum.NodeNameChange, element);
		}
		else {
			SetNextOperation(OperationEnum.OfNoInterrest);
		}
    }

    // ======================================================================
    // Types
    // ----------------------------------------------------------------------
	public class VSPublicGroups {
		Dictionary<string, LinkedGroup>	myGroups= null;
		
		public VSPublicGroups() {
			myGroups= new Dictionary<string, LinkedGroup>();
		}
		public int	NbOfGroups { get { return myGroups.Count; }}
		public void Add(ReferenceToDefinition definition) {
			var name= definition.Name;
			LinkedGroup	group= null;
			if(myGroups.TryGetValue(name, out group)) {
				group.Add(definition);
			}
			else {
				var newGroup= new LinkedGroup();
				newGroup.Add(definition);
				myGroups.Add(name, newGroup);
			}
		}
		public void Add(ReferenceToEngineObject reference) {
			var name= reference.Name;
			LinkedGroup	group= null;
			if(myGroups.TryGetValue(name, out group)) {
				group.Add(reference);
			}
			else {
				var newGroup= new LinkedGroup();
				newGroup.Add(reference);
				myGroups.Add(name, newGroup);
			}
		}
        public void Add(iCS_EditorObject element) {
            var vs= element.IStorage.VisualScript;
            if(element.IsPublicVariable || element.IsPublicFunction) {
                Add(new ReferenceToDefinition(vs, element.Name));
            }
            else {
                Add(new ReferenceToEngineObject(vs, element.InstanceId));                
            }
        }
		public void Remove(ReferenceToDefinition definition) {
			var name= definition.Name;
			LinkedGroup	group= null;
			if(myGroups.TryGetValue(name, out group)) {
				group.Remove(definition);
				if(group.IsEmpty) {
					myGroups.Remove(name);
				}
			}
		}
		public void Remove(ReferenceToEngineObject reference) {
			var name= reference.Name;
			LinkedGroup	group= null;
			if(myGroups.TryGetValue(name, out group)) {
				group.Remove(reference);
				if(group.IsEmpty) {
					myGroups.Remove(name);
				}
			}
		}
        public void Remove(iCS_EditorObject element) {
            var vs= element.IStorage.VisualScript;
            if(element.IsPublicVariable || element.IsPublicFunction) {
                Remove(new ReferenceToDefinition(vs, element.Name));
            }
            else {
                Remove(new ReferenceToEngineObject(vs, element.InstanceId));
            }
        }
		public void ForEach(Action<string, LinkedGroup> action) {
			foreach(var p in myGroups) {
				action(p.Key, p.Value);
			}
		}
		public LinkedGroup Find(string name) {
			LinkedGroup	group= null;
			myGroups.TryGetValue(name, out group);
			return group;
		}
	}
    // ----------------------------------------------------------------------	
	public class LinkedGroup {
        bool                          myIsDefinitionsErrorFree= true;
		List<ReferenceToDefinition>   myDefinitions= new List<ReferenceToDefinition>();
		List<ReferenceToEngineObject> myReferences = new List<ReferenceToEngineObject>();
		
		public LinkedGroup() {}
		public List<ReferenceToDefinition>	    Definitions 	{ get { return myDefinitions; }}
		public List<ReferenceToEngineObject>	References		{ get { return myReferences; }}
		public int 						        NbOfDefinitions	{ get { return myDefinitions.Count; }}
		public int 						        NbOfReferences	{ get { return myReferences.Count; }}
		public bool						        IsEmpty			{ get { return NbOfDefinitions+NbOfReferences == 0; }}
        public bool IsDefinitionsErrorFree {
            get { return myIsDefinitionsErrorFree; }
            set { myIsDefinitionsErrorFree= value; }
        }
		public void Add(ReferenceToDefinition definition) {
		    myDefinitions.Add(definition);	
		}
		public void Add(ReferenceToEngineObject reference) {
			myReferences.Add(reference);
		}
		public void Remove(ReferenceToDefinition definition) {
            var name= definition.Name;
            var vs  = definition.VisualScript;
            for(int i= 0; i < myDefinitions.Count; ++i) {
                var cursor= myDefinitions[i];
                if(cursor.Name == name && cursor.VisualScript == vs) {
                    myDefinitions.RemoveAt(i);
                    break;
                }
            }
		}
		public void Remove(ReferenceToEngineObject reference) {
            var id= reference.ObjectId;
            var vs= reference.VisualScript;
            for(int i= 0; i < myReferences.Count; ++i) {
                var cursor= myReferences[i];
                if(cursor.ObjectId == id && cursor.VisualScript == vs) {
                    myReferences.RemoveAt(i);
                    break;
                }
            }
        }
	};
    // ----------------------------------------------------------------------	
    public class ReferenceToDefinition {
        iCS_VisualScriptImp     myVisualScript = null;
        string                  myDefintionName= null;
		int						myObjectId     = -1;

        public ReferenceToDefinition(iCS_VisualScriptImp visualScript, string name) {
            myVisualScript = visualScript;
            myDefintionName= name;
			myObjectId     = EngineObject.InstanceId;
        }
        public iCS_VisualScriptImp VisualScript 		{ get { return myVisualScript; }}
        public iCS_EngineObject    EngineObject 		{ get { return iCS_VisualScriptData.FindDefinitionWithName(myVisualScript, myDefintionName); }}
        public bool                IsVariableDefinition { get { return EngineObject.IsConstructor; }}
        public bool                IsFunctionDefinition { get { return EngineObject.IsPackage; }}
        public string              Name                 { get { return myDefintionName; }}
        public string              FullName             { get { return iCS_VisualScriptData.GetFullName(myVisualScript, myVisualScript, EngineObject); }}
		public int				   InstanceId			{ get { return myObjectId; }}
    };
    // ----------------------------------------------------------------------	
    public class ReferenceToEngineObject {
        iCS_VisualScriptImp     myVisualScript= null;
        int                     myObjectId= -1;

        public ReferenceToEngineObject(iCS_VisualScriptImp visualScript, int objectId) {
            myVisualScript= visualScript;
            myObjectId= objectId;
        }
        public iCS_VisualScriptImp VisualScript 		{ get { return myVisualScript; }}
        public int                 ObjectId     		{ get { return myObjectId; }}
        public iCS_EngineObject    EngineObject 		{ get { return myVisualScript.EngineObjects[myObjectId]; }}
        public bool                IsVariableReference  { get { return EngineObject.IsVariableReference; }}
        public bool                IsFunctionCall       { get { return EngineObject.IsFunctionCall; }}
        public bool                IsPublicVariable
			{ get { return iCS_VisualScriptData.IsPublicVariable(VisualScript, EngineObject); }}
        public bool                IsPublicFunction
			{ get { return iCS_VisualScriptData.IsPublicFunction(VisualScript, EngineObject); }}
		public bool				   IsPublicInterface
			{ get { return IsPublicVariable || IsPublicFunction; }}
		public bool				   IsReferenceToPublicInterface
			{ get { return IsVariableReference || IsFunctionCall; }}
		public bool				   IsDynamicReference
			{ get { return VisualScript.IsReferenceNodeUsingDynamicBinding(EngineObject); }}
		public iCS_VisualScriptImp TargetVisualScript
			{ get { return VisualScript.GetVisualScriptFromReferenceNode(EngineObject); }}
		public iCS_EngineObject	   TargetEngineObject
			{ get { return VisualScript.GetEngineObjectFromReferenceNode(EngineObject); }}
        public string Name {
            get {
                return EngineObject.Name;
            }
        }
        public string RelativeName {
            get {
                return iCS_VisualScriptData.GetRelativeName(VisualScript, EngineObject);
            }
        }
        public string FullName {
            get {
                return iCS_VisualScriptData.GetFullName(VisualScript, VisualScript, EngineObject);
            }
        }
    };

    // ======================================================================
    // Enums
    // ----------------------------------------------------------------------
	enum OperationEnum { RebuildInfo, NodeNameChange, PortNameChange, PortAdded, PortRemoved, OfNoInterrest };

    // ======================================================================
    // Fields 
    // ----------------------------------------------------------------------
	static string						kServiceId			   = "Public Interface";
    static ReferenceToDefinition[]	    ourPublicVariables     = null;
    static ReferenceToDefinition[]	    ourPublicFunctions     = null;
    static ReferenceToEngineObject[]	ourVariableReferences  = null;
    static ReferenceToEngineObject[]	ourFunctionCalls       = null;
	static VSPublicGroups		        ourPublicVariableGroups= null;
	static VSPublicGroups		        ourPublicFunctionGroups= null;
	static OperationEnum				ourNextOperation	   = OperationEnum.RebuildInfo;
	static iCS_EditorObject				ourOperationObject     = null;
	
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	static void SetNextOperation(OperationEnum op, iCS_EditorObject obj= null) {
		ourNextOperation= op;
		ourOperationObject= obj;
	}
	
    // ======================================================================
    // Scene properties
    // ----------------------------------------------------------------------
    public static ReferenceToDefinition[] PublicVariables {
        get { return ourPublicVariables; }
    }
    public static ReferenceToDefinition[] PublicFunctions {
        get { return ourPublicFunctions; }
    }
    public static ReferenceToEngineObject[] VariableReferences {
        get { return ourVariableReferences; }
    }
    public static ReferenceToEngineObject[] FunctionCalls {
        get { return ourFunctionCalls; }
    }
	public static VSPublicGroups	PublicVariableGroups {
		get { return ourPublicVariableGroups; }
		set { ourPublicVariableGroups= value; }
	}
	public static VSPublicGroups	PublicFunctionGroups {
		get { return ourPublicFunctionGroups; }
		set { ourPublicFunctionGroups= value; }
	}


    // ======================================================================
    // Update scene content changed
    // ----------------------------------------------------------------------
    static void RebuildAndValidatePublicInterfaceInfo() {
		// -- Rebuild public interface information --
		RebuildPublicInterfaceInfo();
        
		// -- Remove all previous errors/warnings --
		iCS_ErrorController.Clear(kServiceId);
		
        // Validate variable & function groups
        ValidatePublicGroups();
		
		// -- Reset next operation to rebuild & validate all visual script in the scene --
		ourNextOperation  = OperationEnum.RebuildInfo;
		ourOperationObject= null;
    }
    // ----------------------------------------------------------------------
    static void RebuildPublicInterfaceInfo() {
		// Extract all public interface definitions and usages
        ourPublicVariables   = ScanForPublicVariables();
        ourPublicFunctions   = ScanForPublicFunctions();
        ourVariableReferences= ScanForVariableReferences();
        ourFunctionCalls     = ScanForFunctionCalls();
		
		// Build groups out of linked objects
		PublicVariableGroups= new VSPublicGroups();
		PublicFunctionGroups= new VSPublicGroups();
		foreach(var pv in ourPublicVariables)		{ PublicVariableGroups.Add(pv); }
		foreach(var vr in ourVariableReferences)	{ PublicVariableGroups.Add(vr); }
		foreach(var pf in ourPublicFunctions)		{ PublicFunctionGroups.Add(pf); }
		foreach(var fc in ourFunctionCalls)			{ PublicFunctionGroups.Add(fc); }
    }

    // ----------------------------------------------------------------------
	static bool IsPublicObject(iCS_EditorObject element) {
		return IsPublicDefinition(element) || IsReferenceToPublicObject(element);
	}
    // ----------------------------------------------------------------------
	static bool IsPublicDefinition(iCS_EditorObject element) {
		return element.IsPublicVariable || element.IsPublicFunction;
	}
    // ----------------------------------------------------------------------
	static bool IsReferenceToPublicObject(iCS_EditorObject element) {
		return element.IsVariableReference || element.IsFunctionCall;
	}

    // ======================================================================
    // PUBLIC INTERFACES
    // ----------------------------------------------------------------------
    static ReferenceToDefinition[] ScanForPublicVariables() {
        var result= new List<ReferenceToDefinition>();
        P.forEach(vs=> {
                var publicVariables= iCS_VisualScriptData.FindPublicVariableDefinitions(vs);
                P.forEach(
                    pv=> {
                        result.Add(new ReferenceToDefinition(vs, pv.Name));
                    },
                    publicVariables
                );
            },
            iCS_SceneController.VisualScriptsInOrReferencedByScene
        );
        return result.ToArray();
    }
    static ReferenceToDefinition[] ScanForPublicFunctions() {
        var result= new List<ReferenceToDefinition>();
        P.forEach(vs=> {
                var publicFunctions= iCS_VisualScriptData.FindFunctionDefinitions(vs);
                P.forEach(
                    pf=> {
                        iCS_PortUtility.RepairPorts(vs, pf);
                        result.Add(new ReferenceToDefinition(vs, pf.Name));
                    },
                    publicFunctions
                );
            },
            iCS_SceneController.VisualScriptsInOrReferencedByScene
        );
        return result.ToArray();
    }
    static ReferenceToEngineObject[] ScanForVariableReferences() {
        var result= new List<ReferenceToEngineObject>();
        P.forEach(vs=> {
                var variableReferences= iCS_VisualScriptData.FindVariableReferences(vs);
                P.forEach(
                    vr=> {
                        result.Add(new ReferenceToEngineObject(vs, vr.InstanceId));
                    },
                    variableReferences
                );
            },
            iCS_SceneController.VisualScriptsInOrReferencedByScene
        );
        return result.ToArray();
    }
    static ReferenceToEngineObject[] ScanForFunctionCalls() {
        var result= new List<ReferenceToEngineObject>();
        P.forEach(vs=> {
                var functionCalls= iCS_VisualScriptData.FindFunctionCalls(vs);
                P.forEach(
                    fc=> {
                        result.Add(new ReferenceToEngineObject(vs, fc.InstanceId));
                    },
                    functionCalls
                );
            },
            iCS_SceneController.VisualScriptsInOrReferencedByScene
        );
        return result.ToArray();
    }
    // ----------------------------------------------------------------------
	static iCS_VisualScriptImp FindVisualScriptFromReferenceNode(ReferenceToEngineObject objRef) {
		return objRef.VisualScript.GetVisualScriptFromReferenceNode(objRef.EngineObject);
	}
	
	// =========================================
	// = Public Interface Validation Utilities =
	// =========================================
    // ----------------------------------------------------------------------
	public static void ValidatePublicGroups() {
        // -- Validate Interface Compliancy for Definitions --
		ValidateVariableDefinitions();
		ValidateFunctionDefinitions();
		
        // -- Validate Interface Compliancy for References --
        ValidateVariableReferences();
		ValidateFunctionCalls();
	}
	
    // ----------------------------------------------------------------------
	public static void ValidateVariableDefinitions() {
        PublicVariableGroups.ForEach(
            (name, group)=> {
                group.IsDefinitionsErrorFree= true;
				var definitions= group.Definitions;
				if(P.length(definitions) < 2) return;
                var definition= definitions[0];
				var defVisualScript= definition.VisualScript;
				var defEngineObject= definition.EngineObject;
				var qualifiedType= defEngineObject.QualifiedType;
				definitions.ForEach(
					o=> {
						if(qualifiedType != o.EngineObject.QualifiedType) {
                            var errorMsg= "Public variables=> <color=orange><b>"+definition.FullName+"</b></color> and=> <color=orange><b>"+o.FullName+"</b></color> must have the same type.";
							iCS_ErrorController.AddError(kServiceId, errorMsg, defVisualScript, defEngineObject.InstanceId);
							iCS_ErrorController.AddError(kServiceId, errorMsg, o.VisualScript, o.EngineObject.InstanceId);
                            group.IsDefinitionsErrorFree= false;
						}
					}
				);
            }
        );
	}
    // ----------------------------------------------------------------------
    public static void ValidateVariableReferences() {
        PublicVariableGroups.ForEach(
            (name, group)=> {
                // -- Don't validate references if the definitions are not error free --
                if(!group.IsDefinitionsErrorFree) return;
                
                // -- Nothing to validate if no reference exists --
                var references = group.References;
                if(references.Count == 0) return;
                
                // -- Verify that all references comply to the definition interface.
				var definitions= group.Definitions;
                if(definitions.Count != 0) {
                    var definition= definitions[0];
                    var vs= definition.VisualScript;
                    var obj= definition.EngineObject;
                    var qualifiedType = obj.QualifiedType;
                    references.ForEach(
                        o=> {
                            var outputPort= iCS_VisualScriptData.GetChildPortWithIndex(o.VisualScript, o.EngineObject, (int)iCS_PortIndex.Return);
            				if(outputPort == null || qualifiedType != outputPort.QualifiedType) {
                                var definitionName= iCS_VisualScriptData.GetFullName(vs,vs,obj);
                                var otherName= iCS_VisualScriptData.GetFullName(o.VisualScript, o.VisualScript, o.EngineObject);
            					var errorMessage= "Variable reference=> <color=orange><b>"+otherName+"</b></color> has a different type then the definition=> <color=orange><b>"+definitionName+"</b></color>.";
            					iCS_ErrorController.AddError(kServiceId, errorMessage, o.VisualScript, o.EngineObject.InstanceId);
            				}
                        }
                    );
                }
                
                // -- Verify that the specific defintion exists --
                references.ForEach(
                    o=> {
                        var vs     = o.VisualScript;
                        var refNode= o.EngineObject;
                        if(vs.IsReferenceNodeUsingDynamicBinding(refNode)) return;
                        var defNode= vs.GetEngineObjectFromReferenceNode(refNode);
                        if(defNode == null || defNode.Name != refNode.Name) {
                            var refName= o.FullName;
                            var errorMessage= "Not able to find suitable variable defintion for variable reference=> <b><color=orange>"+refName+"</color></b>.";
        					iCS_ErrorController.AddError(kServiceId, errorMessage, o.VisualScript, o.EngineObject.InstanceId);
                        }
                    }
                );
            }
        );
    }
    // ----------------------------------------------------------------------
	public static void ValidateFunctionDefinitions() {
        PublicFunctionGroups.ForEach(
            (name, group)=> {
                group.IsDefinitionsErrorFree= true;
				var definitions= group.Definitions;
				if(P.length(definitions) < 2) return;
                var definition= definitions[0];
				var vs= definition.VisualScript;
				var engObj= definition.EngineObject;
				var ports= GetFunctionInterface(vs, engObj);
				definitions.ForEach(
					o=> {
						if(!CompareFunctionInterface(ports, o.VisualScript, o.EngineObject)) {
							UpdateFunctionInterfaceFrom(o.VisualScript, o.EngineObject, vs, engObj);
//                            var errorMsg= "Public functions=> <color=orange><b>"+definition.FullName+"</b></color> and=> <color=orange><b>"+o.FullName+"</b></color> must have the same interface.";
//							iCS_ErrorController.AddError(kServiceId, errorMsg, vs, engObj.InstanceId);
//							iCS_ErrorController.AddError(kServiceId, errorMsg, o.VisualScript, o.EngineObject.InstanceId);
//                            group.IsDefinitionsErrorFree= false;
						}
					}
				);
            }
        );
	}
    // ----------------------------------------------------------------------
    public static void ValidateFunctionCalls() {
        PublicFunctionGroups.ForEach(
            (name, group)=> {
                // -- Don't validate references if the definitions are not error free --
                if(!group.IsDefinitionsErrorFree) return;
                
                // -- Nothing to validate if no reference exists --
                var references = group.References;
                if(references.Count == 0) return;
                
                // -- Verify that all references comply to the definition interface.
				var definitions= group.Definitions;
                if(definitions.Count != 0) {
                    var definition= definitions[0];
                    var vs= definition.VisualScript;
                    var obj= definition.EngineObject;
					var ports= GetFunctionInterface(vs, obj);
                    references.ForEach(
                        o=> {
							if(!CompareFunctionInterface(ports, o.VisualScript, o.EngineObject)) {
                                UpdateFunctionCall(ports, o.VisualScript, o.EngineObject);
							}
                        }
                    );
                }
                
                // -- Verify that the specific defintion exists --
                references.ForEach(
                    o=> {
                        var vs     = o.VisualScript;
                        var refNode= o.EngineObject;
                        if(vs.IsReferenceNodeUsingDynamicBinding(refNode)) return;
                        var defNode= vs.GetEngineObjectFromReferenceNode(refNode);
                        if(defNode == null || defNode.Name != refNode.Name) {
                            var refName= o.FullName;
                            var errorMessage= "Not able to find suitable function defintion for function call=> <b><color=orange>"+refName+"</color></b>.";
        					iCS_ErrorController.AddError(kServiceId, errorMessage, o.VisualScript, o.EngineObject.InstanceId);
                        }
                    }
                );
            }
        );
    }
    // ----------------------------------------------------------------------
	/// Get function interface
	static iCS_EngineObject[] GetFunctionInterface(iCS_VisualScriptImp vs, iCS_EngineObject obj) {
		var ports= iCS_VisualScriptData.GetChildPorts(vs, obj);
		ports= P.filter(o=> o.IsDataPort && o.PortIndex != (int)iCS_PortIndex.InInstance, ports);
		P.sort(ports, (a,b)=> b.PortIndex - a.PortIndex);
		return ports;
	}
    // ----------------------------------------------------------------------
	/// Compare function interfaces
	static bool CompareFunctionInterface(iCS_EngineObject[] interface1, iCS_VisualScriptImp vs, iCS_EngineObject obj) {
		var interface2= GetFunctionInterface(vs, obj);
		var len= interface1.Length;
		if(len != interface2.Length) return false;
		for(int i= 0; i < len; ++i) {
			var a= interface1[i];
			var b= interface2[i];
			if(a.PortIndex != b.PortIndex) return false;
			if(a.Name != b.Name) return false;
			if(a.QualifiedType != b.QualifiedType) return false;
		}
		return true;
	}

	// ======================================================================
	// UPDATE FUNCTION CALL FROM FUNCTION DEFINITION
    // ----------------------------------------------------------------------
    /// Update function call from function definition.
    static void UpdateFunctionCall(iCS_EngineObject[] fncDefPorts, iCS_VisualScriptImp vs, iCS_EngineObject functionCall) {
        // -- Replicate interface ports --
        var fncCallPorts= GetFunctionInterface(vs, functionCall);
		ReplicatePortsOnFunctionCall(fncDefPorts, fncCallPorts, vs, functionCall);

        // -- Advise Unity that the visual script has changed --
        UpdateUnityAndEditors(vs);
    }
    // ----------------------------------------------------------------------
	static void ReplicatePortsOnFunctionCall(iCS_EngineObject[] portsToReplicate, iCS_EngineObject[] existingPorts,
						                     iCS_VisualScriptImp vs, iCS_EngineObject node) {

        // -- Update is completed if all ports are identical --
		var nonIdenticalPorts= KeepNonIdenticalPorts(portsToReplicate, existingPorts);
		var srcPorts= nonIdenticalPorts.Item1;
		var dstPorts= nonIdenticalPorts.Item2;
		var srcLen= P.length(srcPorts);
		var dstLen= P.length(dstPorts);
        if(srcLen == 0 && dstLen == 0) return;
		
        // -- Simple port addition --
		if(srcLen != 0 && dstLen == 0) {
	        foreach(var toClone in srcPorts) {
				AddPortOnFunctionCall(toClone, vs, node);
	        }	
			return;
		}

	    // -- Simple port removal --
		if(srcLen == 0 && dstLen != 0) {
	        foreach(var toRemove in dstPorts) {
				DestroyPortOnFunctionCall(toRemove, vs, node);
	        }
			return;
		}
		
		// -- Add port if no matching type --
		var srcPort= P.head(srcPorts);
		var dstPort= P.head(dstPorts);
		if(!P.or(p=> ArePortsTypeIdentical(p, srcPort), dstPorts)) {
			AddPortOnFunctionCall(srcPort, vs, node);
			ReplicatePortsOnFunctionCall(P.tail(srcPorts), dstPorts, vs, node);
			return;
		}

		// -- Remove port if no matching type --
		if(!P.or(p=> ArePortsTypeIdentical(p, dstPort), srcPorts)) {
			DestroyPortOnFunctionCall(dstPort, vs, node);
			ReplicatePortsOnFunctionCall(srcPorts, P.tail(dstPorts), vs, node);
			return;
		}

		// -- Relink the port index --
		var result= P.filter(p=> ArePortsIdenticalExceptIndex(p, srcPort), dstPorts);
		if(P.length(result) != 0) {
			result[0].PortIndex= srcPort.PortIndex;
			dstPorts= P.filter(p=> p != result[0], dstPorts);
			ReplicatePortsOnFunctionCall(P.tail(srcPorts), dstPorts, vs, node);
			return;
		}

		// -- Simple port rename --
		if(ArePortsIdenticalExceptName(srcPort, dstPort)) {
			dstPort.Name= srcPort.Name;
			ReplicatePortsOnFunctionCall(P.tail(srcPorts), P.tail(dstPorts), vs, node);
			return;
		}			
		
		// -- Could not find a valid match for source port; so just add it --
		AddPortOnFunctionCall(srcPort, vs, node);
		ReplicatePortsOnFunctionCall(P.tail(srcPorts), dstPorts, vs, node);
	}
    // ----------------------------------------------------------------------
	static P.Tuple<iCS_EngineObject[],iCS_EngineObject[]> KeepNonIdenticalPorts(iCS_EngineObject[] ps1, iCS_EngineObject[] ps2) {
        var identicalPorts= P.filter(p1=>  P.or(p2=> ArePortsIdentical(p1,p2), ps1), ps2);
        var nonIdentical1 = P.filter(p1=> !P.or(p2=> ArePortsIdentical(p1,p2), identicalPorts), ps1);
        var nonIdentical2 = P.filter(p1=> !P.or(p2=> ArePortsIdentical(p1,p2), identicalPorts), ps2);
		return P.Tuple.Create(nonIdentical1, nonIdentical2);
	}
    // ----------------------------------------------------------------------
    static bool ArePortsIdentical(iCS_EngineObject p1, iCS_EngineObject p2) {
		if(!ArePortsTypeIdentical(p1, p2)) return false;
        if(p1.PortIndex != p2.PortIndex) return false;
        return p1.Name == p2.Name;
    }
    // ----------------------------------------------------------------------
    static bool ArePortsIdenticalExceptName(iCS_EngineObject p1, iCS_EngineObject p2) {
		if(!ArePortsTypeIdentical(p1, p2)) return false;
        if(p1.PortIndex != p2.PortIndex) return false;
        return p1.Name != p2.Name;
    }
    // ----------------------------------------------------------------------
    static bool ArePortsIdenticalExceptIndex(iCS_EngineObject p1, iCS_EngineObject p2) {
		if(!ArePortsTypeIdentical(p1, p2)) return false;
        if(p1.Name != p2.Name) return false;
        return p1.PortIndex != p2.PortIndex;
    }
    // ----------------------------------------------------------------------
	static bool ArePortsTypeIdentical(iCS_EngineObject p1, iCS_EngineObject p2) {
		if(p1.QualifiedType != p2.QualifiedType) return false;
		var ot1= ConvertDynamicAndProposedPortToFixPort(p1);
		var ot2= ConvertDynamicAndProposedPortToFixPort(p2);
		return ot1 == ot2;
	}
    // ----------------------------------------------------------------------
	static iCS_ObjectTypeEnum ConvertDynamicAndProposedPortToFixPort(iCS_EngineObject p) {
        // -- Convert dynamic and proposed ports to fix ports --
        if(p.IsInDynamicDataPort || p.IsInProposedDataPort) {
            return iCS_ObjectTypeEnum.InFixDataPort;
        }
        if(p.IsOutDynamicDataPort || p.IsOutProposedDataPort) {
            return iCS_ObjectTypeEnum.OutFixDataPort;
        }
		return p.ObjectType;
	}
    // ----------------------------------------------------------------------
	static void AddPortOnFunctionCall(iCS_EngineObject portToClone, iCS_VisualScriptImp vs, iCS_EngineObject functionCallNode) {
        var newPort= portToClone.Clone();
        newPort.ParentId= functionCallNode.InstanceId;
        newPort.SourceId= -1;
        // -- Convert dynamic and proposed ports to fix ports --
		newPort.ObjectType= ConvertDynamicAndProposedPortToFixPort(newPort);
        newPort.IsNameEditable= false;
        // FIXME: Must update unity object reference in visual script data.
        iCS_VisualScriptData.AddEngineObject(vs, newPort);
	}
    // ----------------------------------------------------------------------
	static void DestroyPortOnFunctionCall(iCS_EngineObject portToDestroy, iCS_VisualScriptImp vs, iCS_EngineObject functionCallNode) {
        iCS_VisualScriptData.DestroyEngineObject(vs, portToDestroy);
	}

	// ======================================================================
	// UPDATE FUNCTION DEFINIION FROM ANOTHER FUNCTION DEFINITION
    // ----------------------------------------------------------------------
	static void UpdateFunctionInterfaceFrom(iCS_VisualScriptImp toVs  , iCS_EngineObject toFnc,
									 		iCS_VisualScriptImp fromVs, iCS_EngineObject fromFnc) {

		// -- Replcate interface ports --
		var fromPorts= GetFunctionInterface(fromVs, fromFnc);
		var toPorts  = GetFunctionInterface(toVs  , toFnc);
		ReplicatePortsOnFunction(fromPorts, toPorts, toVs, toFnc);
		
        // -- Advise Unity that the visual script has changed --
        UpdateUnityAndEditors(toVs);
	}
    // ----------------------------------------------------------------------
	static void ReplicatePortsOnFunction(iCS_EngineObject[] portsToReplicate, iCS_EngineObject[] existingPorts,
						                     iCS_VisualScriptImp vs, iCS_EngineObject node) {

        // -- Update is completed if all ports are identical --
		var nonIdenticalPorts= KeepNonIdenticalPorts(portsToReplicate, existingPorts);
		var srcPorts= nonIdenticalPorts.Item1;
		var dstPorts= nonIdenticalPorts.Item2;
		var srcLen= P.length(srcPorts);
		var dstLen= P.length(dstPorts);
        if(srcLen == 0 && dstLen == 0) return;
		
        // -- Simple port addition --
		if(srcLen != 0 && dstLen == 0) {
	        foreach(var toClone in srcPorts) {
				AddPortOnFunction(toClone, vs, node);
	        }	
			return;
		}

	    // -- Simple port removal --
		if(srcLen == 0 && dstLen != 0) {
	        foreach(var toRemove in dstPorts) {
				DestroyPortOnFunction(toRemove, vs, node);
	        }
			return;
		}
		
		// -- Add port if no matching type --
		var srcPort= P.head(srcPorts);
		var dstPort= P.head(dstPorts);
		if(!P.or(p=> ArePortsTypeIdentical(p, srcPort), dstPorts)) {
			AddPortOnFunction(srcPort, vs, node);
			ReplicatePortsOnFunction(P.tail(srcPorts), dstPorts, vs, node);
			return;
		}

		// -- Remove port if no matching type --
		if(!P.or(p=> ArePortsTypeIdentical(p, dstPort), srcPorts)) {
			DestroyPortOnFunction(dstPort, vs, node);
			ReplicatePortsOnFunction(srcPorts, P.tail(dstPorts), vs, node);
			return;
		}

		// -- Relink the port index --
		var result= P.filter(p=> ArePortsIdenticalExceptIndex(p, srcPort), dstPorts);
		if(P.length(result) != 0) {
			result[0].PortIndex= srcPort.PortIndex;
			dstPorts= P.filter(p=> p != result[0], dstPorts);
			ReplicatePortsOnFunction(P.tail(srcPorts), dstPorts, vs, node);
			return;
		}

		// -- Simple port rename --
		if(ArePortsIdenticalExceptName(srcPort, dstPort)) {
			dstPort.Name= srcPort.Name;
			ReplicatePortsOnFunction(P.tail(srcPorts), P.tail(dstPorts), vs, node);
			return;
		}			
		
		// -- Could not find a valid match for source port; so just add it --
		AddPortOnFunction(srcPort, vs, node);
		ReplicatePortsOnFunction(P.tail(srcPorts), dstPorts, vs, node);
	}
    // ----------------------------------------------------------------------
	static void AddPortOnFunction(iCS_EngineObject portToClone, iCS_VisualScriptImp vs, iCS_EngineObject functionNode) {
        var newPort= portToClone.Clone();
        newPort.ParentId= functionNode.InstanceId;
        newPort.SourceId= -1;
        newPort.IsNameEditable= false;
        // FIXME: Must update unity object reference in visual script data.
        iCS_VisualScriptData.AddEngineObject(vs, newPort);
	}
    // ----------------------------------------------------------------------
	static void DestroyPortOnFunction(iCS_EngineObject portToDestroy, iCS_VisualScriptImp vs, iCS_EngineObject functionNode) {
		DestroyPortOnFunctionCall(portToDestroy, vs, functionNode);
	}
    // ----------------------------------------------------------------------
    static void ChangeNodeName(iCS_EditorObject obj) {
		// -- Determine if this is a new node --
		var objVs= obj.IStorage.VisualScript;
		var engObj= obj.EngineObject;
        if(obj.IsPublicFunction) {
    		// -- New functions follow existing interface --
    		var isVirgin= obj.NbOfChildNodes == 0;
    		if(isVirgin) {
    			var group= ourPublicFunctionGroups.Find(obj.Name);
    			if(group != null) {
    				ReferenceToDefinition toFollow= null;
    				toFollow= P.find(def=> def.EngineObject != engObj, group.Definitions);
    				if(toFollow != null) {
    					UpdateFunctionInterfaceFrom(objVs, engObj, toFollow.VisualScript, toFollow.EngineObject);
    				}
    			}
    		}
            else {
                ChangeInterfaceName(ourPublicFunctionGroups, obj);
            }
        } else {
            ChangeInterfaceName(ourPublicVariableGroups, obj);
        }
    }
    // ----------------------------------------------------------------------
    static void ChangeInterfaceName(VSPublicGroups groups, iCS_EditorObject obj) {
        var objVs= obj.IStorage.PersistentStorage;
		// -- Try to find the original group --
		LinkedGroup myGroup= null;
		groups.ForEach(
			(_, group)=> {
				foreach(var def in group.Definitions) {
					if(def.VisualScript == objVs && obj.InstanceId == def.InstanceId) {
						myGroup= group;
						return;
					}
				}
			}
		);
        // -- Change name of all nodes in the found linked group --
		if(myGroup != null) {
			foreach(var def in myGroup.Definitions) {
				if(def != null) {
					var eng= def.EngineObject;
					if(eng != null) {
						eng.Name= obj.Name;
                        UpdateUnityAndEditors(def.VisualScript);
					}
				}
			}
			foreach(var r in myGroup.References) {
				if(r != null) {
					var eng= r.EngineObject;
					if(eng != null) {
						eng.Name= obj.Name;
                        UpdateUnityAndEditors(r.VisualScript);
					}
				}
			}
		}
    }
    // ----------------------------------------------------------------------
    static void UpdateUnityAndEditors(iCS_MonoBehaviourImp vs) {
        // -- Advise Unity that the visual script has changed --
        EditorUtility.SetDirty(vs);
        // -- Reload the visual editor --
        iCS_VisualEditor visualEditor= null;
        if(iCS_VisualScriptDataController.IsInUse(vs)) {
            visualEditor= iCS_EditorController.FindVisualEditor();
	        if(visualEditor != null) {
	            visualEditor.SendEvent(EditorGUIUtility.CommandEvent("ReloadStorage"));
	        }
        }        
    }
}
