﻿#define DEBUG
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

public static class iCS_PublicInterfaceController {
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    static iCS_PublicInterfaceController() {
        // Verify that the scene controller is ran before us.
        if(iCS_SceneController.VisualScriptsInScene == null) {
            Debug.LogError("iCanScript: Please move PublicInterfaceController after the SceneController in AppController");
        }
        // Events to refresh scene content information.
        iCS_SystemEvents.OnSceneChanged    = RefreshPublicInterfaceInfo;
        iCS_SystemEvents.OnHierarchyChanged= RefreshPublicInterfaceInfo;
        iCS_SystemEvents.OnProjectChanged  = RefreshPublicInterfaceInfo;
        // Events to refresh visual script information.
        iCS_SystemEvents.OnVisualScriptUndo                = OnVisualScriptUndo;                
        iCS_SystemEvents.OnVisualScriptElementAdded        = OnVisualScriptElementAdded;        
        iCS_SystemEvents.OnVisualScriptElementWillBeRemoved= OnVisualScriptElementWillBeRemoved;
        iCS_SystemEvents.OnVisualScriptElementNameChanged  = OnVisualScriptElementNameChanged;
        // Force an initial refresh of the scene info.
        RefreshPublicInterfaceInfo();
    }
    public static void Start() {}
    public static void Shutdown() {
    }
    

    // ======================================================================
    // Types
    // ----------------------------------------------------------------------
	public class VSPublicGroups {
		Dictionary<string, VSObjectReferenceGroup>	myGroups= null;
		
		public VSPublicGroups() {
			myGroups= new Dictionary<string, VSObjectReferenceGroup>();
		}
		public int	NbOfGroups { get { return myGroups.Count; }}
		public void Add(VSObjectReference objRef) {
			var objName= objRef.EngineObject.Name;
			VSObjectReferenceGroup	group= null;
			if(myGroups.TryGetValue(objName, out group)) {
				group.Add(objRef);
			}
			else {
				var newGroup= new VSObjectReferenceGroup();
				newGroup.Add(objRef);
				myGroups.Add(objName, newGroup);
			}
		}
        public void Add(iCS_EditorObject element) {
            Add(new VSObjectReference(element.IStorage.VisualScript, element.InstanceId));
        }
		public void Remove(VSObjectReference objRef) {
			var objName= objRef.EngineObject.Name;
			VSObjectReferenceGroup	group= null;
			if(myGroups.TryGetValue(objName, out group)) {
				group.Remove(objRef);
				if(group.IsEmpty) {
					myGroups.Remove(objName);
				}
			}
		}
        public void Remove(iCS_EditorObject element) {
            Remove(new VSObjectReference(element.IStorage.VisualScript, element.InstanceId));
        }
		public void ForEach(Action<string, VSObjectReferenceGroup> action) {
			foreach(var p in myGroups) {
				action(p.Key, p.Value);
			}
		}
		public VSObjectReferenceGroup Find(string name) {
			VSObjectReferenceGroup	group= null;
			myGroups.TryGetValue(name, out group);
			return group;
		}
	}
    // ----------------------------------------------------------------------	
	public class VSObjectReferenceGroup {
		List<VSObjectReference>	myDefinitions= new List<VSObjectReference>();
		List<VSObjectReference> myReferences = new List<VSObjectReference>();
		
		public VSObjectReferenceGroup() {}
		public List<VSObjectReference>	Definitions 	{ get { return myDefinitions; }}
		public List<VSObjectReference>	References		{ get { return myReferences; }}
		public int 						NbOfDefinitions	{ get { return myDefinitions.Count; }}
		public int 						NbOfReferences	{ get { return myReferences.Count; }}
		public bool						IsEmpty			{ get { return NbOfDefinitions+NbOfReferences == 0; }}
		public void Add(VSObjectReference objRef) {
			if(objRef.IsPublicVariable || objRef.IsPublicFunction) {
			   myDefinitions.Add(objRef);	
			}
			else {
				myReferences.Add(objRef);
			}
		}
		public void Remove(VSObjectReference objRef) {
			if(objRef.IsPublicVariable || objRef.IsPublicFunction) {
                   for(int i= 0; i < myDefinitions.Count; ++i) {
                       var definition= myDefinitions[i];
                       if(definition.ObjectId == objRef.ObjectId && definition.VisualScript == objRef.VisualScript) {
                           myDefinitions.RemoveAt(i);
                           break;
                       }
                   }
			}
			else {
                for(int i= 0; i < myReferences.Count; ++i) {
                    var reference= myReferences[i];
                    if(reference.ObjectId == objRef.ObjectId && reference.VisualScript == objRef.VisualScript) {
                        myReferences.RemoveAt(i);
                        break;
                    }
                }
			}			
		}
	};
    public class VSObjectReference {
        iCS_VisualScriptImp     myVisualScript= null;
        int                     myObjectId= -1;

        public VSObjectReference(iCS_VisualScriptImp visualScript, int objectId) {
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
                return VisualScript.name+"."+EngineObject.Name;
            }
        }
    };


    // ======================================================================
    // Fields 
    // ----------------------------------------------------------------------
    static VSObjectReference[]	ourPublicVariables   = null;
    static VSObjectReference[]	ourPublicFunctions   = null;
    static VSObjectReference[]	ourVariableReferences= null;
    static VSObjectReference[]	ourFunctionCalls     = null;
	static VSPublicGroups		ourPublicVariableGroups= null;
	static VSPublicGroups		ourPublicFunctionGroups= null;
	
    // ======================================================================
    // Scene properties
    // ----------------------------------------------------------------------
    public static VSObjectReference[] PublicVariables {
        get { return ourPublicVariables; }
    }
    public static VSObjectReference[] PublicFunctions {
        get { return ourPublicFunctions; }
    }
    public static VSObjectReference[] VariableReferences {
        get { return ourVariableReferences; }
    }
    public static VSObjectReference[] FunctionCalls {
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
    static void RefreshPublicInterfaceInfo() {
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
		
        // Validate variable & function groups
        ValidatePublicGroups();
    }

    // ======================================================================
    // Update visual script content changed
    // ----------------------------------------------------------------------
    static void OnVisualScriptUndo(iCS_IStorage iStorage) {
#if DEBUG
        Debug.Log("Visual Script undo=> "+iStorage.VisualScript.name);
#endif
        RefreshPublicInterfaceInfo();
    }
    static void OnVisualScriptElementAdded(iCS_IStorage iStorage, iCS_EditorObject element) {
#if DEBUG
        Debug.Log("Visual Script element added=> "+iStorage.VisualScript.name+"."+element.Name);
#endif
        if(element.IsPublicVariable || element.IsVariableReference) {
            PublicVariableGroups.Add(element);
        }
        if(element.IsPublicFunction || element.IsFunctionCall) {
            PublicFunctionGroups.Add(element);
        }
        ValidatePublicGroups();
    }
    static void OnVisualScriptElementWillBeRemoved(iCS_IStorage iStorage, iCS_EditorObject element) {
#if DEBUG
        Debug.Log("Visual Script element will be removed=> "+iStorage.VisualScript.name+"."+element.Name);
#endif
        if(element.IsPublicVariable || element.IsVariableReference) {
            PublicVariableGroups.Remove(element);
        }
        if(element.IsPublicFunction || element.IsFunctionCall) {
            PublicFunctionGroups.Remove(element);
        }
        ValidatePublicGroups();
    }
    static void OnVisualScriptElementNameChanged(iCS_IStorage iStorage, iCS_EditorObject element) {
#if DEBUG
        Debug.Log("Visual Script element name changed=> "+iStorage.VisualScript.name+"."+element.Name);
#endif
        ValidatePublicGroups();
    }

    // ======================================================================
    // PUBLIC INTERFACES
    // ----------------------------------------------------------------------
    static VSObjectReference[] ScanForPublicVariables() {
        var result= new List<VSObjectReference>();
        P.forEach(vs=> {
                var publicVariables= iCS_VisualScriptData.FindPublicVariables(vs);
                P.forEach(
                    pv=> {
                        result.Add(new VSObjectReference(vs, pv.InstanceId));
                    },
                    publicVariables
                );
            },
            iCS_SceneController.VisualScriptsInOrReferencedByScene
        );
        return result.ToArray();
    }
    static VSObjectReference[] ScanForPublicFunctions() {
        var result= new List<VSObjectReference>();
        P.forEach(vs=> {
                var publicFunctions= iCS_VisualScriptData.FindPublicFunctions(vs);
                P.forEach(
                    pf=> {
                        iCS_PortUtility.RepairPorts(vs, pf);
                        result.Add(new VSObjectReference(vs, pf.InstanceId));
                    },
                    publicFunctions
                );
            },
            iCS_SceneController.VisualScriptsInOrReferencedByScene
        );
        return result.ToArray();
    }
    static VSObjectReference[] ScanForVariableReferences() {
        var result= new List<VSObjectReference>();
        P.forEach(vs=> {
                var variableReferences= iCS_VisualScriptData.FindVariableReferences(vs);
                P.forEach(
                    vr=> {
                        result.Add(new VSObjectReference(vs, vr.InstanceId));
                    },
                    variableReferences
                );
            },
            iCS_SceneController.VisualScriptsInOrReferencedByScene
        );
        return result.ToArray();
    }
    static VSObjectReference[] ScanForFunctionCalls() {
        var result= new List<VSObjectReference>();
        P.forEach(vs=> {
                var functionCalls= iCS_VisualScriptData.FindFunctionCalls(vs);
                P.forEach(
                    fc=> {
                        result.Add(new VSObjectReference(vs, fc.InstanceId));
                    },
                    functionCalls
                );
            },
            iCS_SceneController.VisualScriptsInOrReferencedByScene
        );
        return result.ToArray();
    }
    // ----------------------------------------------------------------------
	static iCS_VisualScriptImp FindVisualScriptFromReferenceNode(VSObjectReference objRef) {
		return objRef.VisualScript.GetVisualScriptFromReferenceNode(objRef.EngineObject);
	}
	
	// =========================================
	// = Public Interface Validation Utilities =
	// =========================================
    // ----------------------------------------------------------------------
	public static P.Tuple<VSObjectReference,VSObjectReference>[] ValidateVariableDefinitions() {
		var result= new List<P.Tuple<VSObjectReference,VSObjectReference> >();
        PublicVariableGroups.ForEach(
            (name, group)=> {
				var definitions= group.Definitions;
				if(P.length(definitions) < 2) return;
                var definition= definitions[0];
				var qualifiedType= definition.EngineObject.QualifiedType;
				P.fold(
					(acc,o)=> {
						if(qualifiedType != o.EngineObject.QualifiedType) {
							acc.Add(new P.Tuple<VSObjectReference,VSObjectReference>(definitions[0], o));
#if DEBUG
                            Debug.LogWarning("iCanScript: public variables=> <color=orange><b>"+definition.VisualScript.name+"."+name+"</b></color> and=> <color=orange><b>"+o.VisualScript.name+"."+name+"</b></color> differ in terms of type! Please correct before continuing.");
#endif
						}
						return acc;
					}
					, result
					, definitions
				);
            }
        );
		return result.ToArray();
	}
    // ----------------------------------------------------------------------
	public static bool IsVariableDefinitionCompliant(iCS_EditorObject variable) {
		var group= PublicVariableGroups.Find(variable.Name);
		if(group == null) return true;
		var definitions= group.Definitions;
		if(P.length(definitions) == 0) return true;
		var type= variable.RuntimeType;
		return P.fold((acc,o)=> acc || type == o.EngineObject.RuntimeType, false, definitions);
	}
    // ----------------------------------------------------------------------
	public static P.Tuple<VSObjectReference,VSObjectReference>[] ValidateFunctionDefinitions() {
		var result= new List<P.Tuple<VSObjectReference,VSObjectReference> >();
        PublicFunctionGroups.ForEach(
            (name, group)=> {
				var definitions= group.Definitions;
				if(P.length(definitions) < 2) return;
				var definition= definitions[0];
				var vs = definition.VisualScript;
				var obj= definition.EngineObject;
				P.fold(
					(acc,o)=> {
						if(!IsSameFunction(vs, obj, o.VisualScript, o.EngineObject)) {
							acc.Add(new P.Tuple<VSObjectReference,VSObjectReference>(definition, o));
#if DEBUG
                            var definitionName= iCS_VisualScriptData.GetFullName(vs,vs,obj);
                            var otherName= iCS_VisualScriptData.GetFullName(o.VisualScript, o.VisualScript, o.EngineObject);
                            Debug.LogWarning("iCanScript: public functions=> <color=orange><b>"+definitionName+"</b></color> and=> <color=orange><b>"+otherName+"</b></color> are mismatched in terms of ports. Please correct before continuing.");
#endif
						}
						return acc;
					}
					, result
					, definitions
				);
            }
        );
		return result.ToArray();
	}
    // ----------------------------------------------------------------------
	/// Determines if the given functions have the same composition.
	static bool IsSameFunction(iCS_VisualScriptImp vs1, iCS_EngineObject f1,
							   iCS_VisualScriptImp vs2, iCS_EngineObject f2) {
		if(f1.Name != f2.Name) return false;
		var ps1= iCS_VisualScriptData.GetChildPorts(vs1, f1);
		var ps2= iCS_VisualScriptData.GetChildPorts(vs2, f2);
		if(P.length(ps1) != P.length(ps2)) return false;
        Array.Sort(ps1, (x,y)=> (int)x.PortIndex - (int)y.PortIndex);
        Array.Sort(ps2, (x,y)=> (int)x.PortIndex - (int)y.PortIndex);
		return P.and(P.zipWith( (p1,p2)=> IsSamePort(vs1,p1,vs2,p2), ps1, ps2));
	}
    // ----------------------------------------------------------------------
	/// Determines if the given ports have the same relative identification.
	static bool IsSamePort(iCS_VisualScriptImp vs1, iCS_EngineObject p1,
						   iCS_VisualScriptImp vs2, iCS_EngineObject p2) {
		if(p1.PortIndex != p2.PortIndex) return false;
		if(p1.RuntimeType != p2.RuntimeType) return false;
		return true;
	}
    // ----------------------------------------------------------------------
	public static P.Tuple<VSObjectReference,VSObjectReference>[] ValidateCallsToFunction(string name, VSObjectReferenceGroup group) {
		var result= new List<P.Tuple<VSObjectReference,VSObjectReference> >();
        var definitions= group.Definitions;
        var references = group.References;
        // No mismatch if no function call exists.
        if(references.Count == 0) return result.ToArray();
        // At least definition must exist is a function call exists.
        if(references.Count != 0 && definitions.Count == 0) {
            return P.map(
                o=> {
#if DEBUG
                    Debug.LogWarning("iCanScript: no definition exist for function call=> "+o.EngineObject.Name+" in=> "+o.VisualScript.name+".");
#endif
                    return new P.Tuple<VSObjectReference,VSObjectReference>(o, null);
                },
                references
            ).ToArray();
        }                
        var definition= definitions[0];
        var vs1= definition.VisualScript;
        var f1 = definition.EngineObject;
        return P.fold(
            (acc,o)=> {
                var vs2= o.VisualScript;
                var f2= o.EngineObject;
				if(!IsSameFunction(vs1, f1, vs2, f2)) {
					acc.Add(new P.Tuple<VSObjectReference,VSObjectReference>(definition, o));
#if DEBUG
                    var definitionName= iCS_VisualScriptData.GetFullName(vs1, vs1, f1);
                    var refName       = iCS_VisualScriptData.GetFullName(vs2, vs2, f2);
                    Debug.LogWarning("iCanScript: function call=> <color=orange><b>"+refName+"</b></color> differs for function definition=> <color=orange><b>"+definitionName+"</b></color>.  Please correct before continuing.");
#endif
				}
				return acc;
            },
            result,
            references
        ).ToArray();
    }
    // ----------------------------------------------------------------------
	public static P.Tuple<VSObjectReference,VSObjectReference>[] ValidateReferencesToVariable(string name, VSObjectReferenceGroup group) {
		var result= new List<P.Tuple<VSObjectReference,VSObjectReference> >();
        var definitions= group.Definitions;
        var references = group.References;
        // No mismatch if no function call exists.
        if(references.Count == 0) return result.ToArray();
        // At least definition must exist is a function call exists.
        if(references.Count != 0 && definitions.Count == 0) {
            return P.map(
                o=> {
#if DEBUG
                    Debug.LogWarning("iCanScript: no public variable exist for reference=> "+o.EngineObject.Name+" in=> "+o.VisualScript.name+".");
#endif
                    return new P.Tuple<VSObjectReference,VSObjectReference>(o, null);
                },
                references
            ).ToArray();
        }                
        var definition= definitions[0];
        var vs= definition.VisualScript;
        var obj= definition.EngineObject;
        var qualifiedType = obj.QualifiedType;
        return P.fold(
            (acc,o)=> {
                var outputPort= iCS_VisualScriptData.GetChildPortWithIndex(o.VisualScript, o.EngineObject, (int)iCS_PortIndex.OutInstance);
				if(outputPort == null || qualifiedType != outputPort.QualifiedType) {
					acc.Add(new P.Tuple<VSObjectReference,VSObjectReference>(definition, o));
#if DEBUG
                    var definitionName= iCS_VisualScriptData.GetFullName(vs,vs,obj);
                    var otherName= iCS_VisualScriptData.GetFullName(o.VisualScript, o.VisualScript, o.EngineObject);
                    Debug.LogWarning("iCanScript: variable reference=> <color=orange><b>"+otherName+"</b></color> differs in terms of type from definition=> <color=orange><b>"+definitionName+"</b></color>.  Please correct before continuing.");
#endif
				}
				return acc;
            },
            result,
            references
        ).ToArray();
    }

    // ----------------------------------------------------------------------
	public static bool ValidatePublicGroups() {
        // Validate Variables
		ValidateVariableDefinitions();
		ValidateFunctionDefinitions();
		
        PublicVariableGroups.ForEach(
            (name, group)=> {
                ValidateReferencesToVariable(name, group);
//                var definitions= group.Definitions;
//                var references = group.References;
//                if(references.Count != 0 && definitions.Count == 0) {
//                    foreach(var varRef in references) {
//                        Debug.LogWarning("iCanScript: No definition found for variable=> "+name+" in visual script=> "+varRef.VisualScript.name);                    
//                    }
//                }                
            }
        );
        
        // Validate Functions
        // TODO: Cumulate errors
        PublicFunctionGroups.ForEach(
            (name, group)=> {
                ValidateCallsToFunction(name, group);
//                var definitions= group.Definitions;
//                var references = group.References;
//                if(references.Count != 0 && definitions.Count == 0) {
//                    foreach(var varRef in references) {
//                        Debug.LogWarning("iCanScript: No definition found for variable=> "+name+" in visual script=> "+varRef.VisualScript.name);
//                    }
//                }
            }
        );
		return true;
	}


}
