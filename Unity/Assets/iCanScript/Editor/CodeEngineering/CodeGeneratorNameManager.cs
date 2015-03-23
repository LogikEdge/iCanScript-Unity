using UnityEngine;
using System.Collections;
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
        while(IsExisting(proposedName)) {
            proposedName= proposedName+"_"+eObj.InstanceId;
        }
        return proposedName;
    }
    // -----------------------------------------------------------------------
    public bool IsExisting(string name) {
        for(int i= 0; i < myNames.Length; ++i) {
            var n= myNames[i];
            if(n != null && n == name) {
                return true;
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
    public bool IsClassScope(iCS_EditorObject eObj) {
        if(eObj.InstanceId == 0) return true;
        if(myParentCode[eObj.InstanceId] == 0) return true;
        return false;
    }
    // -----------------------------------------------------------------------
    public bool IsLocalScope(iCS_EditorObject eObj) {
        // Find parent node.
        var parentNode= eObj;
        while(myParentCode[parentNode.InstanceId] > 1) {
            parentNode= parentNode.ParentNode;
        }
        // Search in parent node for the given object.
        bool found= false;
        parentNode.ForEachChildRecursiveDepthLast(
            o=> {
                if(eObj == o) {
                    found= true;
                }
            }
        );
        return found;
    }
}

}