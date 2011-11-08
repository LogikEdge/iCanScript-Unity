using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class WD_Descriptor {
    // Identifiers
    public string   Company;
    public string   Package;
    public Type     ClassType;
    public string   Name;
    public string[] ParamNames;
    public Type[]   ParamTypes;
    public bool[]   ParamIsOuts;
    public object[] ParamDefaultValues;
    public string[] Children;
    
    public MethodInfo Method {
        get {
            return ClassType.GetMethod(Name, MethodParamTypes);            
        }
    }
    public Type[] MethodParamTypes {
        get {
            Type[] methodParameters= new Type[ParamTypes.Length-1];
            Array.Copy(ParamTypes, methodParameters, methodParameters.Length);
            return methodParameters;            
        }
    }
    public Type ReturnType {
        get {
            return ParamTypes[ParamTypes.Length-1];            
        }
    }

    public override string ToString() {
        return WD_DataBase.ToString(this);
    }
}
