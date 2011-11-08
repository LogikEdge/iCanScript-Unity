using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class WD_RuntimeDesc {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public WD_ObjectTypeEnum    ObjectType;
    public string               Company;
    public string               Package;
    public Type                 ClassType;
    public string               MethodName;
    public string[]             ParamNames;
    public Type[]               ParamTypes;
    public bool[]               ParamIsOuts;
    public object[]             ParamDefaultValues;
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public MethodInfo Method {
        get {
            return ClassType.GetMethod(MethodName, MethodParamTypes);            
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

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public WD_RuntimeDesc() {}
    // ----------------------------------------------------------------------
    // Decodes the string into its constituants.
    public WD_RuntimeDesc(string encoded) {
        // object type
        int end= encoded.IndexOf(':');
        string objectTypeStr= encoded.Substring(0, end);
        ObjectType= WD_Types.FromString<WD_ObjectTypeEnum>(objectTypeStr);
        encoded= encoded.Substring(end+1, encoded.Length-end-1);
        // company
        end= encoded.IndexOf(':');
        Company= encoded.Substring(0, end);
        encoded= encoded.Substring(end+1, encoded.Length-end-1);
        // package
        end= encoded.IndexOf(':');
        Package= encoded.Substring(0, end);
        encoded= encoded.Substring(end+1, encoded.Length-end-1);
        // class type
        end= encoded.IndexOf(':');
        string className= encoded.Substring(0, end);
        ClassType= WD_Types.FromString<Type>(className);
        encoded= encoded.Substring(end+1, encoded.Length-end-1);
        // name
        end= encoded.IndexOf('<');
        MethodName= encoded.Substring(0, end);
        encoded= encoded.Substring(end+1, encoded.Length-end-1);
        // parameters
        end= encoded.IndexOf('>');
        string parameterString= encoded.Substring(0, end);
        encoded= encoded.Substring(end+1, encoded.Length-end-1);
        ParseParameters(parameterString, out ParamIsOuts, out ParamTypes, out ParamNames, out ParamDefaultValues);
    }


    // ======================================================================
    // Conversion functions
    // ----------------------------------------------------------------------
    // Returns a string that uniquely describes the descriptor.
    public override string ToString() {
        string result= WD_Types.ToString(ObjectType)+":"+Company+":"+Package+":"+WD_Types.ToString(ClassType)+":"+MethodName+"<";
        for(int i= 0; i < ParamTypes.Length; ++i) {
            if(ParamIsOuts[i]) result+= "out ";
            result+= ParamNames[i];
            if(ParamDefaultValues[i] != null) {
                result+= ":="+WD_Types.ToString(ParamDefaultValues[i]);
            }
            result+= ":"+WD_Types.ToString(ParamTypes[i]);
            if(i != ParamTypes.Length-1) result+= ";";
        }
        result+=">{}";
        return result;
    }
    // ----------------------------------------------------------------------
    // Extracts the type of the parameters from the given string.
    static void ParseParameters(string paramStr, out bool[] isOut, out Type[] types, out string[] names, out object[] defaultValues) {
        List<bool>      paramIsOut   = new List<bool>();
        List<Type>      paramTypes   = new List<Type>();
        List<string>    paramNames   = new List<string>();
        List<object>    paramDefaults= new List<object>();
        while(paramStr.Length > 0) {
            // in/out parameter type
            if(paramStr.StartsWith("out ")) {
                paramIsOut.Add(true);
                paramStr= paramStr.Substring(4, paramStr.Length-4);
            } else {
                paramIsOut.Add(false);
            }                
            // parameter name
            int end= paramStr.IndexOf(':');
            paramNames.Add(paramStr.Substring(0, end));
            paramStr= paramStr.Substring(end+1, paramStr.Length-end-1);
            // parameter default value (part 1)
            string defaultValueStr= null;
            if(paramStr.StartsWith("=")) {
                end= paramStr.IndexOf(':');
                defaultValueStr= paramStr.Substring(1, end-1);
                paramStr= paramStr.Substring(end+1, paramStr.Length-end-1);                
            }
            // parameter type.
            end= paramStr.IndexOf(';');
            Type paramType= WD_Types.FromString<Type>(paramStr.Substring(0, end > 0 ? end : paramStr.Length));
            paramTypes.Add(paramType);
            paramStr= end > 0 ? paramStr.Substring(end+1, paramStr.Length-end-1) : "";
            // parameter default value (part 2)
            if(defaultValueStr != null) {
                paramDefaults.Add(WD_Types.FromString(defaultValueStr, paramType));
            } else {
                paramDefaults.Add(null);                
            }
        }
        isOut= paramIsOut.ToArray();
        types= paramTypes.ToArray();
        names= paramNames.ToArray();
        defaultValues= paramDefaults.ToArray();
    }
}
