#define DEBUG
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

public static class iCS_SceneController {
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
				var newGroup= new VSObjectReferenceGroup(objName);
				newGroup.Add(objRef);
				myGroups.Add(objName, newGroup);
			}
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
		string					myName    	 = null;
		List<VSObjectReference>	myDefinitions= new List<VSObjectReference>();
		List<VSObjectReference> myReferences = new List<VSObjectReference>();
		
		public VSObjectReferenceGroup(string name) {
			myName= name;
		}
		public List<VSObjectReference>	Definitions 	{ get { return myDefinitions; }}
		public List<VSObjectReference>	References		{ get { return myReferences; }}
		public int 						NbOfDefinitions	{ get { return myDefinitions.Count; }}
		public int 						NbOfReferences	{ get { return myReferences.Count; }}
		public bool						IsEmpty			{ get { return NbOfDefinitions+NbOfReferences == 0; }}
		public void Add(VSObjectReference objRef) {
			if(iCS_VisualScriptData.IsPublicVariable(objRef.EngineObject) ||
			   iCS_VisualScriptData.IsPublicFunction(objRef.EngineObject)) {
			   myDefinitions.Add(objRef);	
			}
			else {
				myReferences.Add(objRef);
			}
		}
		public void Remove(VSObjectReference objRef) {
			
		}
	};
    public class VSObjectReference {
        iCS_VisualScriptImp     myVisualScript= null;
        int                     myObjectId= -1;

        public VSObjectReference(iCS_VisualScriptImp visualScript, int objectId) {
            myVisualScript= visualScript;
            myObjectId= objectId;
        }
        public iCS_VisualScriptImp VisualScript { get { return myVisualScript; }}
        public int                 ObjectId     { get { return myObjectId; }}
        public iCS_EngineObject    EngineObject { get { return myVisualScript.EngineObjects[myObjectId]; }}
    };

    // ======================================================================
    // Scene Cache
    // ----------------------------------------------------------------------
    // Visual Scripts
    static iCS_VisualScriptImp[]    ourVisualScriptsInScene              = null;
    static iCS_VisualScriptImp[]    ourVisualScriptsReferencesByScene    = null;
    static iCS_VisualScriptImp[]    ourVisualScriptsInOrReferencesByScene= null;

    // Public Interfaces
    static VSObjectReference[]	ourPublicVariables   = null;
    static VSObjectReference[]	ourPublicFunctions   = null;
    static VSObjectReference[]	ourVariableReferences= null;
    static VSObjectReference[]	ourFunctionCalls     = null;
	static VSPublicGroups		ourPublicVariableGroups= null;
	static VSPublicGroups		ourPublicFunctionGroups= null;
	
    // ======================================================================
    // Scene properties
    // ----------------------------------------------------------------------
    public static iCS_VisualScriptImp[] VisualScriptsInScene {
        get { return ourVisualScriptsInScene; }
    }
    public static iCS_VisualScriptImp[] VisualScriptsReferencedByScene {
        get { return ourVisualScriptsReferencesByScene; }
    }
    public static iCS_VisualScriptImp[] VisualScriptsInOrReferencedByScene {
        get { return ourVisualScriptsInOrReferencesByScene; }
    }
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
    // Common Controller activation/deactivation
    // ----------------------------------------------------------------------
	static iCS_SceneController() {
        // Events to refresh scene content information.
        iCS_SystemEvents.OnSceneChanged    = OnSceneChanged;
        iCS_SystemEvents.OnHierarchyChanged= RefreshSceneInfo;
        iCS_SystemEvents.OnProjectChanged  = RefreshSceneInfo;
        // Events to refresh visual script information.
        iCS_SystemEvents.OnVisualScriptUndo                = OnVisualScriptUndo;                
        iCS_SystemEvents.OnVisualScriptElementAdded        = OnVisualScriptElementAdded;        
        iCS_SystemEvents.OnVisualScriptElementWillBeRemoved= OnVisualScriptElementWillBeRemoved;
        iCS_SystemEvents.OnVisualScriptElementNameChanged  = OnVisualScriptElementNameChanged;  
	}
    
    /// Start the application controller.
	public static void Start() {}
    /// Shutdowns the application controller.
    public static void Shutdown() {}

    // ======================================================================
    // Update scene content changed
    // ----------------------------------------------------------------------
    static void OnSceneChanged() {
        RefreshSceneInfo();
    }
    static void RefreshSceneInfo() {
        ourVisualScriptsInScene              = ScanForVisualScriptsInScene();
        ourVisualScriptsReferencesByScene    = ScanForVisualScriptsReferencedByScene();
        ourVisualScriptsInOrReferencesByScene= CombineVisualScriptsInOrReferencedByScene();

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
		
#if DEBUG
        Debug.Log("Scene as changed =>"+EditorApplication.currentScene);
		Debug.Log("NbOfPublicVariableGroups=> "+PublicVariableGroups.NbOfGroups);
		PublicVariableGroups.ForEach(
			(name, group)=> {
				Debug.Log("Group Name=> "+name+" #Definitions=> "+group.NbOfDefinitions+" #References=> "+group.NbOfReferences);
			}
		);
		Debug.Log("NbOfPublicFunctionGroups=> "+PublicFunctionGroups.NbOfGroups);
		PublicFunctionGroups.ForEach(
			(name, group)=> {
				Debug.Log("Group Name=> "+name+" #Definitions=> "+group.NbOfDefinitions+" #References=> "+group.NbOfReferences);
			}
		);
#endif
    }

    // ======================================================================
    // Update visual script content changed
    // ----------------------------------------------------------------------
    static void OnVisualScriptUndo(iCS_IStorage iStorage) {
#if DEBUG
        Debug.Log("Visual Script undo=> "+iStorage.VisualScript.name);
#endif
    }
    static void OnVisualScriptElementAdded(iCS_IStorage iStorage, iCS_EditorObject element) {
#if DEBUG
        Debug.Log("Visual Script element added=> "+iStorage.VisualScript.name+"."+element.Name);
#endif
    }
    static void OnVisualScriptElementWillBeRemoved(iCS_IStorage iStorage, iCS_EditorObject element) {
#if DEBUG
        Debug.Log("Visual Script element will be removed=> "+iStorage.VisualScript.name+"."+element.Name);
#endif
    }
    static void OnVisualScriptElementNameChanged(iCS_IStorage iStorage, iCS_EditorObject element) {
#if DEBUG
        Debug.Log("Visual Script element name changed=> "+iStorage.VisualScript.name+"."+element.Name);
#endif
    }

    // ======================================================================
    // VISUAL SCRIPTS
    // ----------------------------------------------------------------------
    /// Returns all active VisualScript included in the current scene.
    static iCS_VisualScriptImp[] ScanForVisualScriptsInScene() {
        var allVisualScripts= UnityEngine.Object.FindObjectsOfType(typeof(iCS_VisualScriptImp));
        return Array.ConvertAll( allVisualScripts, e=> e as iCS_VisualScriptImp );
    }

    // ----------------------------------------------------------------------
    /// Returns all Visual Scripts referenced in the current scene.
	///
	/// Note:	This function assumes that the visual script in the scene
	///			has already been fetched.
	///
    static iCS_VisualScriptImp[] ScanForVisualScriptsReferencedByScene() {
        var sceneVisualScripts= VisualScriptsInScene;
        var result= new iCS_VisualScriptImp[0];
        P.forEach(
            vs=> {
                result= P.append(result, ScanForVisualScriptsReferencedBy(vs));
            },
            sceneVisualScripts
        );
        result= P.removeDuplicates(result);
        return result;
    }
    
    // ----------------------------------------------------------------------
    /// Returns all Visual Scripts referenced in the current scene.
	///
	/// Note:	This function assumes that the visual script in and referenced
	///			by this scene have already been fetched.
	///
    static iCS_VisualScriptImp[] CombineVisualScriptsInOrReferencedByScene() {
        return P.removeDuplicates(P.append(VisualScriptsInScene, VisualScriptsReferencedByScene));
    }
    
    // ----------------------------------------------------------------------
    /// Returns Visual Scripts referenced by the given Visual Script.
    static iCS_VisualScriptImp[] ScanForVisualScriptsReferencedBy(iCS_VisualScriptImp vs) {
        var visualScripts= P.map(o=> o as iCS_VisualScriptImp, P.filter(o=> o is iCS_VisualScriptImp, vs.UnityObjects));
        var gameObjects  = P.map(o=> o as GameObject         , P.filter(o=> o is GameObject         , vs.UnityObjects));
        P.forEach(
            go=> {
                var vsFromGameObject= go.GetComponent(typeof(iCS_VisualScriptImp)) as iCS_VisualScriptImp;
                if(vsFromGameObject != null) {
                    visualScripts.Add(vsFromGameObject);
                }
            },
            gameObjects
        );
        visualScripts= P.removeDuplicates(visualScripts);
        return visualScripts.ToArray();
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
            VisualScriptsInOrReferencedByScene
        );
        return result.ToArray();
    }
    static VSObjectReference[] ScanForPublicFunctions() {
        var result= new List<VSObjectReference>();
        P.forEach(vs=> {
                var publicFunctions= iCS_VisualScriptData.FindPublicFunctions(vs);
                P.forEach(
                    pf=> {
                        result.Add(new VSObjectReference(vs, pf.InstanceId));
                    },
                    publicFunctions
                );
            },
            VisualScriptsInOrReferencedByScene
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
            VisualScriptsInOrReferencedByScene
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
            VisualScriptsInOrReferencedByScene
        );
        return result.ToArray();
    }
    // ----------------------------------------------------------------------
	static iCS_VisualScriptImp FindVisualScriptFromReferenceNode(VSObjectReference objRef) {
		return objRef.VisualScript.GetVisualScriptFromRefenceNode(objRef.EngineObject);
	}
	
    // ======================================================================
    // VALIDATE PUBLIC INTERFACES
    // ----------------------------------------------------------------------
	public static bool ValidatePublicGroups() {
		return true;
	}
}
