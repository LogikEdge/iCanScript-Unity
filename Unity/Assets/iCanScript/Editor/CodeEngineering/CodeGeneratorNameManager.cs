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
    string[] myNames;
    int[]    myParentCode;
        
    // -----------------------------------------------------------------------
    public CodeGeneratorNameManager(iCS_IStorage iStorage) {
        Init(iStorage);
    }
    // -----------------------------------------------------------------------
    public void Init(iCS_IStorage iStorage) {
        int len= iStorage.EditorObjects.Count;
        myNames= new string[len];
        myParentCode= new int[len];
        for(int i= 0; i < len; i++) {
            myNames[i]= null;
            var eObj= iStorage[i];
            myParentCode[i]= (eObj != null ? eObj.ParentId : -1);
        }
    }
    // -----------------------------------------------------------------------
    public string GetNameFor(iCS_EditorObject eObj) {
        int i= eObj.InstanceId;
        if(myNames[i] == null) {
            string name= null;
            if(eObj.IsPort) {
                name= GeneratePortName(eObj);
            }
            else if(eObj.IsConstructor) {
                name= MakeNameUnique(CSharpGenerator.ToVariableName(eObj), eObj);
            }
            else {
                name= MakeNameUnique(CSharpGenerator.ToGeneratedNodeName(eObj), eObj);
            }
            myNames[i]= name;
        }
        return myNames[i];
    }
    // -----------------------------------------------------------------------
    public void SetCodeParent(iCS_EditorObject eObj, iCS_EditorObject parent) {
        myParentCode[eObj.InstanceId]= parent.InstanceId;
    }
    // -----------------------------------------------------------------------
    public string MakeNameUnique(string proposedName, iCS_EditorObject eObj) {
        while(IsExisting(proposedName, eObj)) {
            proposedName= proposedName+"_"+eObj.InstanceId;
        }
        return proposedName;
    }
    // -----------------------------------------------------------------------
    public bool IsExisting(string name) {
        for(int i= 0; i < P.length(myNames); ++i) {
            var n= myNames[i];
            if(n != null && n == name) {
                return true;
            }
        }
        return false;
    }
    // -----------------------------------------------------------------------
    public bool IsExisting(string name, iCS_EditorObject eObj) {
        var id= eObj.InstanceId;
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
    public string GeneratePortName(iCS_EditorObject port) {
        var name= CSharpGenerator.ToGeneratedPortName(port);
        return MakeNameUnique(name, port);
    }
    // -----------------------------------------------------------------------
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