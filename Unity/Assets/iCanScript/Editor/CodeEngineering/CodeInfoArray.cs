using UnityEngine;
using System.Collections;
using CodeType    = iCanScript.Editor.CodeEngineering.CodeInfo.CodeType;
using AccessType  = iCanScript.Editor.CodeEngineering.CodeInfo.AccessType;
using ScopeType   = iCanScript.Editor.CodeEngineering.CodeInfo.ScopeType;
using LocationType= iCanScript.Editor.CodeEngineering.CodeInfo.LocationType;

namespace iCanScript.Editor.CodeEngineering {

public class CodeInfoArray {
    // -----------------------------------------------------------------------
    CodeInfo[] myCodeInfos;
        
    // -----------------------------------------------------------------------
    public CodeInfoArray(iCS_IStorage iStorage) {
        Init(iStorage);
    }
    // -----------------------------------------------------------------------
    public void Init(iCS_IStorage iStorage) {
        myCodeInfos= new CodeInfo[iStorage.EditorObjects.Count];
    }
    // -----------------------------------------------------------------------
    public string GetNameFor(iCS_EditorObject eObj) {
        int i= eObj.InstanceId;
        if(myCodeInfos[i].name == null) {
            string name= null;
            if(eObj.IsPort) {
                name= CSharpGenerator.ToGeneratedPortName(eObj);
            }
            else {
                name= CSharpGenerator.ToGeneratedNodeName(eObj);
            }
            myCodeInfos[i].name= MakeNameUnique(name, eObj);
        }
        return myCodeInfos[i].name;
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
        for(int i= 0; i < myCodeInfos.Length; ++i) {
            var n= myCodeInfos[i].name;
            if(n != null && n == name) {
                return true;
            }
        }
        return false;
    }
}

}