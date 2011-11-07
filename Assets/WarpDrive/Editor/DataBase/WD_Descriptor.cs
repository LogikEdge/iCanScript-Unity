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
    public Type[]   ParameterTypes;
    public bool[]   ParameterIsOuts;
    public object[] ParameterDefaultValues;
    public string[] Children;
    
    public MethodInfo Method {
        get {
            return ClassType.GetMethod(Name, MethodParameterTypes);            
        }
    }
    public Type[] MethodParameterTypes {
        get {
            Type[] methodParameters= new Type[ParameterTypes.Length-1];
            Array.Copy(ParameterTypes, methodParameters, methodParameters.Length);
            return methodParameters;            
        }
    }
    public Type ReturnType {
        get {
            return ParameterTypes[ParameterTypes.Length-1];            
        }
    }

    public string ToString() {
        return WD_DataBase.ToString(this);
    }
}
