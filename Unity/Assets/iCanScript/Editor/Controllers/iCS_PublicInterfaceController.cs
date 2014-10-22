//#define DEBUG
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
		List<ReferenceToDefinition>   myDefinitions= new List<ReferenceToDefinition>();
		List<ReferenceToEngineObject> myReferences = new List<ReferenceToEngineObject>();
		
		public LinkedGroup() {}
		public List<ReferenceToDefinition>	    Definitions 	{ get { return myDefinitions; }}
		public List<ReferenceToEngineObject>	References		{ get { return myReferences; }}
		public int 						        NbOfDefinitions	{ get { return myDefinitions.Count; }}
		public int 						        NbOfReferences	{ get { return myReferences.Count; }}
		public bool						        IsEmpty			{ get { return NbOfDefinitions+NbOfReferences == 0; }}
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

        public ReferenceToDefinition(iCS_VisualScriptImp visualScript, string name) {
            myVisualScript = visualScript;
            myDefintionName= name;
        }
        public iCS_VisualScriptImp VisualScript 		{ get { return myVisualScript; }}
        public iCS_EngineObject    EngineObject 		{ get { return iCS_VisualScriptData.FindDefinitionWithName(myVisualScript, myDefintionName); }}
        public bool                IsVariableDefinition { get { return EngineObject.IsConstructor; }}
        public bool                IsFunctionDefinition { get { return EngineObject.IsPackage; }}
        public string              Name                 { get { return myDefintionName; }}
        public string              FullName             { get { return iCS_VisualScriptData.GetFullName(myVisualScript, myVisualScript, EngineObject); }}
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
    // Fields 
    // ----------------------------------------------------------------------
	static string						kServiceId			 = "Public Interface";
    static ReferenceToDefinition[]	    ourPublicVariables   = null;
    static ReferenceToDefinition[]	    ourPublicFunctions   = null;
    static ReferenceToEngineObject[]	ourVariableReferences= null;
    static ReferenceToEngineObject[]	ourFunctionCalls     = null;
	static VSPublicGroups		        ourPublicVariableGroups= null;
	static VSPublicGroups		        ourPublicFunctionGroups= null;
	
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
    static void RefreshPublicInterfaceInfo() {
		// Remove all previous errors/warnings
		iCS_ErrorController.Clear(kServiceId);
		
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
        ValidatePublicGroups();
    }
    static void OnVisualScriptElementAdded(iCS_IStorage iStorage, iCS_EditorObject element) {
		// Don't process if the lement does not imply a public interface.
		if(!IsPublicObject(element)) return;

#if DEBUG
        Debug.Log("Visual Script element added=> "+iStorage.VisualScript.name+"."+element.Name);               
#endif
//        if(element.IsPublicVariable || element.IsVariableReference) {
//            PublicVariableGroups.Add(element);
//        }
//        if(element.IsPublicFunction || element.IsFunctionCall) {
//            PublicFunctionGroups.Add(element);
//        }
        RefreshPublicInterfaceInfo();
        ValidatePublicGroups();
    }
    static void OnVisualScriptElementWillBeRemoved(iCS_IStorage iStorage, iCS_EditorObject element) {
		// Don't process if the lement does not imply a public interface.
		if(!IsPublicObject(element)) return;		
#if DEBUG
        Debug.Log("Visual Script element will be removed=> "+iStorage.VisualScript.name+"."+element.Name);
#endif
//        if(element.IsPublicVariable || element.IsVariableReference) {
//            PublicVariableGroups.Remove(element);
//        }
//        if(element.IsPublicFunction || element.IsFunctionCall) {
//            PublicFunctionGroups.Remove(element);
//        }
		RefreshPublicInterfaceInfo();
        ValidatePublicGroups();
    }
    static void OnVisualScriptElementNameChanged(iCS_IStorage iStorage, iCS_EditorObject element) {
		// Don't process if the lement does not imply a public interface.
		if(!IsPublicObject(element)) return;
#if DEBUG
        Debug.Log("Visual Script element name changed=> "+iStorage.VisualScript.name+"."+element.Name);
#endif
		RefreshPublicInterfaceInfo();
        ValidatePublicGroups();
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
	public static void ValidateVariableDefinitions() {
        PublicVariableGroups.ForEach(
            (name, group)=> {
				var definitions= group.Definitions;
				if(P.length(definitions) < 2) return;
                var definition= definitions[0];
				var defVisualScript= definition.VisualScript;
				var defEngineObject= definition.EngineObject;
				var qualifiedType= defEngineObject.QualifiedType;
				definitions.ForEach(
					o=> {
						if(qualifiedType != o.EngineObject.QualifiedType) {
                            var errorMsg= "public variables=> <color=orange><b>"+definition.FullName+"</b></color> and=> <color=orange><b>"+o.FullName+"</b></color> differ in terms of type! Please correct before continuing.";
							iCS_ErrorController.AddError(kServiceId, errorMsg, defVisualScript, defEngineObject.InstanceId);
							iCS_ErrorController.AddError(kServiceId, errorMsg, o.VisualScript, o.EngineObject.InstanceId);
						}
					}
				);
            }
        );
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
	public static P.Tuple<ReferenceToDefinition,ReferenceToDefinition>[] ValidateFunctionDefinitions() {
		var result= new List<P.Tuple<ReferenceToDefinition,ReferenceToDefinition> >();
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
							acc.Add(new P.Tuple<ReferenceToDefinition,ReferenceToDefinition>(definition, o));
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
	public static P.Tuple<ReferenceToDefinition,ReferenceToEngineObject>[] ValidateCallsToFunction(string name, LinkedGroup group) {
		var result= new List<P.Tuple<ReferenceToDefinition,ReferenceToEngineObject> >();
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
                    return new P.Tuple<ReferenceToDefinition,ReferenceToEngineObject>(null, o);
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
					acc.Add(new P.Tuple<ReferenceToDefinition,ReferenceToEngineObject>(definition, o));
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
	public static void ValidateReferencesToVariable(string name, LinkedGroup group) {
        var definitions= group.Definitions;
        var references = group.References;
        // No mismatch if no function call exists.
        if(references.Count == 0) return;
        // At least definition must exist is a function call exists.
        if(definitions.Count == 0) {
            references.ForEach(
                o=> {
                    var fullName= o.FullName;
					var errorMessage= "no public variable exist for reference=> <color=orange><b>"+fullName+"</b></color>. Please correct before continuing.";
					iCS_ErrorController.AddError(kServiceId, errorMessage, o.VisualScript, o.EngineObject.InstanceId);
                }
            );
			return;
        }                
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
					var errorMessage= "variable reference=> <color=orange><b>"+otherName+"</b></color> differs in terms of type from definition=> <color=orange><b>"+definitionName+"</b></color>.  Please correct before continuing.";
					iCS_ErrorController.AddError(kServiceId, errorMessage, o.VisualScript, o.EngineObject.InstanceId);
				}
            }
        );
    }

    // ----------------------------------------------------------------------
	public static bool ValidatePublicGroups() {
        // Validate Variables
		ValidateVariableDefinitions();
//		ValidateFunctionDefinitions();
		
        PublicVariableGroups.ForEach(
            (name, group)=> {
                ValidateReferencesToVariable(name, group);
            }
        );
        
        // Validate Functions
        // TODO: Cumulate errors
//        PublicFunctionGroups.ForEach(
//            (name, group)=> {
//                ValidateCallsToFunction(name, group);
//                var definitions= group.Definitions;
//                var references = group.References;
//                if(references.Count != 0 && definitions.Count == 0) {
//                    foreach(var varRef in references) {
//                        Debug.LogWarning("iCanScript: No definition found for variable=> "+name+" in visual script=> "+varRef.VisualScript.name);
//                    }
//                }
//            }
//        );
		return true;
	}


}
