using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;
using CodeType    = iCanScript.Editor.CodeEngineering.CodeGenerator.CodeType;
using AccessType  = iCanScript.Editor.CodeEngineering.CodeGenerator.AccessType;
using ScopeType   = iCanScript.Editor.CodeEngineering.CodeGenerator.ScopeType;
using LocationType= iCanScript.Editor.CodeEngineering.CodeGenerator.LocationType;

namespace iCanScript.Editor.CodeEngineering {

public class CodeGeneratorNameManager {
    // -----------------------------------------------------------------------
    string[] myNames;           ///< Array of generated code fragment names
    int[]    myParentCode;      ///< Array of parent code fragment IDs
        
    // -----------------------------------------------------------------------
    /// Builds an instance of a name manager for the given visual script.
    ///
    /// @param iStorage Storage of the visual script for which to generate names
    ///
    public CodeGeneratorNameManager(iCS_IStorage iStorage) {
        Init(iStorage);
    }
    // -----------------------------------------------------------------------
    /// Initializes the internal structures of the name manager.
    ///
    /// @param iStorage Storage of the visual script for which to generate names
    ///
    public void Init(iCS_IStorage iStorage) {
        int len= iStorage.EditorObjects.Count;
        myNames= new string[len];
        myParentCode= new int[len];
        for(int i= 0; i < len; i++) {
            myNames[i]= null;
            var vsObj= iStorage[i];
            var parentCode= FindDefaultCodeParent(vsObj);
            myParentCode[i]= (parentCode != null ? parentCode.InstanceId : -1);
        }
    }
    // -----------------------------------------------------------------------
    /// Returns the default code parent for the given vidual script object.
    ///
    /// @param vsObj    The visual object for whom to find the code parent.
    /// @return The code parent if found.  _null_ is returned otherwise.
    ///
    iCS_EditorObject FindDefaultCodeParent(iCS_EditorObject vsObj) {
        if(vsObj == null) return null;
        var iStorage= vsObj.IStorage;
        var rootObj= iStorage.EditorObjects[0];
        var parentNode= vsObj.ParentNode;
        while(parentNode != null && parentNode != rootObj && !parentNode.IsPublicFunction) {
            parentNode= parentNode.ParentNode;
        }
        return parentNode;
    }
    // -----------------------------------------------------------------------
    /// Builds or returns the pre-built code name for the given viusal script
    /// object.
    ///
    /// @param vsObj Visual Script object for which to return a code name.
    ///
    public string GetNameFor(iCS_EditorObject vsObj) {
        int i= vsObj.InstanceId;
        if(myNames[i] == null) {
            string name= null;
            if(vsObj.IsPort) {
                name= GeneratePortName(vsObj);
            }
            else if(vsObj.IsConstructor) {
                name= MakeNameUnique(CSharpGenerator.ToVariableName(vsObj), vsObj);
            }
            else {
                name= MakeNameUnique(CSharpGenerator.ToGeneratedNodeName(vsObj), vsObj);
            }
            myNames[i]= name;
        }
        return myNames[i];
    }
    // -----------------------------------------------------------------------
    /// Builds or returns the pre-built local variable name for the given
    /// viusal script object.
    ///
    /// @param vsObj Visual Script object for which to return a code name.
    ///
    public string ToLocalVariableName(iCS_EditorObject vsObj) {
        int i= vsObj.InstanceId;
        if(myNames[i] == null) {
            string name= vsObj.CodeName;
            name= iCS_ObjectNames.ToLocalVariableName(name);
            myNames[i]= MakeNameUnique(name, vsObj);
        }
        return myNames[i];
    }
    // -----------------------------------------------------------------------
    /// Builds or returns the pre-built parameter name for the given
    /// viusal script object.
    ///
    /// @param vsObj Visual Script object for which to return a code name.
    ///
    public string ToFunctionParameterName(iCS_EditorObject vsObj) {
        int i= vsObj.InstanceId;
        if(myNames[i] == null) {
            string name= vsObj.CodeName;
            name= iCS_ObjectNames.ToFunctionParameterName(name);
            myNames[i]= MakeNameUnique(name, vsObj);
        }
        return myNames[i];
    }
    // -----------------------------------------------------------------------
    /// Changes the code parent for the given visual script object.
    ///
    /// The code parent is initializes to be equivalent to the visual script
    /// parent object.  However, when generating the code, it is possible that
    /// the code would be generated in a different execution scope.  Use this
    /// method to change the code generation parent when needed.
    ///
    /// @param vsObj    Visual Script object on which to change the code parent
    /// @param parent   The new code parent for the given visual script object
    ///
    public void SetCodeParent(iCS_EditorObject vsObj, iCS_EditorObject parent) {
        myParentCode[vsObj.InstanceId]= parent.InstanceId;
    }
    // -----------------------------------------------------------------------
    /// Creates a unique code name from the proposedName.
    ///
    /// @param proposedName The desired name for the code fragment associated
    ///                     with the visual script object.
    /// @param vsObj    Visual Script object of the code fragment.
    ///
    public string MakeNameUnique(string proposedName, iCS_EditorObject vsObj) {
        while(IsExisting(proposedName, vsObj)) {
            proposedName= proposedName+"_"+vsObj.InstanceId;
        }
        return proposedName;
    }
    // -----------------------------------------------------------------------
    /// Determines if the given _'name'_ already exists in the code scope.
    ///
    /// This algorithm first seeks for a name collison in the code scope.
    /// The name collision is detected if the names are the same and:
    /// - the parent code is the same or;
    /// - the name exists in one of the code parent scope.
    ///
    /// @param name The proposed name of the code fragment
    /// @param vsObj The visual script object of the cod efragment
    ///
    public bool IsExisting(string name, iCS_EditorObject vsObj) {
        var id= vsObj.InstanceId;
        var parents= GetCodeParents(id);
        for(int i= 0; i < P.length(myNames); ++i) {
            var compareName= myParentCode[id] == myParentCode[i];
            if(compareName == false) {
                compareName= IsIncludedIn(i, parents);
            }
            if(compareName) {
                var n= myNames[i];
                if(n != null && n == name) {
                    return true;
                }                
            }
        }
        return false;
    }
    // -----------------------------------------------------------------------
    /// Generates a code unique name for the given _'port'_.
    ///
    /// @param port The visual script port for which to generate a unique name.
    ///
    public string GeneratePortName(iCS_EditorObject port) {
        var name= CSharpGenerator.ToGeneratedPortName(port);
        return MakeNameUnique(name, port);
    }
    // -----------------------------------------------------------------------
    /// Determines if the given instance ID exists a list of instance IDs.
    ///
    /// @param id   The ID to serach for
    /// @param lst  The array of instance IDs being searched
    ///
    public bool IsIncludedIn(int id, int[] lst) {
        foreach(var i in lst) {
            if(id == i) return true;
        }
        return false;
    }
    // -----------------------------------------------------------------------
    /// Returns a list of the common code parents.
    ///
    /// @param instanceId1 Instance ID of the first code object
    /// @param instanceId2 Instance ID of the second code object
    /// 
    public int[] GetCommonCodeParents(int instanceId1, int instanceId2) {
        var parents2= GetCodeParents(instanceId2);
        return GetCommonCodeParents(instanceId1, parents2);
    }
    // -----------------------------------------------------------------------
    /// Returns a list of the common code parents.
    ///
    /// @param instanceId1 Instance ID of the first code object
    /// @param parents List of code parent of the second code object
    /// 
    public int[] GetCommonCodeParents(int instanceId1, int[] parents2) {
        var parents1= GetCodeParents(instanceId1);
        var maxLen= Mathf.Min(P.length(parents1), P.length(parents2));
        int len= 0;
        for(; len < maxLen; ++len) {
            if(parents1[len] != parents2[len]) {
                break;
            }
        }
        return P.take(len, parents1);
    }
    // -----------------------------------------------------------------------
    /// Returns the list of code parents starting from the class.
    ///
    /// @param id   The starting instance ID from which to extract parent list
    ///
    public int[] GetCodeParents(int instanceId) {
        /// Cummulate parent including the given instance ID.
        var result= new List<int>();
        while(instanceId > 0) {
            result.Add(instanceId);
            instanceId= myParentCode[instanceId];
        }
        // Always add the class instance ID.
        result.Add(0);
        result.Reverse();
        return result.ToArray();
    }
}

}