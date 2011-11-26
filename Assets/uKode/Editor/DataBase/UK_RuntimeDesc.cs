using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class UK_RuntimeDesc {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public int                  Id                  = -1;
    public UK_ObjectTypeEnum    ObjectType          = UK_ObjectTypeEnum.Unknown;
    public string               Company             = "(no company)";
    public string               Package             = "DefaultPackage";
    public string               Name                = null;
    public Type                 ClassType           = null;
    public string               MethodName          = null;
    public string[]             ParamNames          = new string[0];
    public Type[]               ParamTypes          = new Type[0];
    public bool[]               ParamIsOuts         = new bool[0];
    public object[]             ParamDefaultValues  = new object[0];
    public string               ReturnName          = null;
    public Type                 ReturnType          = null;
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public MethodInfo Method {
        get {
            if(MethodName == null) return null;
            MethodInfo method= ClassType.GetMethod(MethodName, ParamTypes);            
            if(method == null) {
                Debug.LogWarning("Unable to extract MethodInfo from RuntimeDesc: "+MethodName);
            }
            return method;
        }
    }
    public FieldInfo Field {
        get {
            if(MethodName == null) return null;
            FieldInfo field= ClassType.GetField(MethodName);
            if(field == null) {
                Debug.LogWarning("Unable to extract FieldInfo from RuntimeDesc: "+MethodName);                
            }
            return field;
        }
    }
    public object GetDefaultValue(int idx, UK_IStorage storage) {
        return storage.GetDefaultValue(this, idx);
    }
    public void SetDefaultValue(int idx, object obj, UK_IStorage storage) {
        storage.SetDefaultValue(this, idx, obj);
    }
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_RuntimeDesc() {}
    // ----------------------------------------------------------------------
    // Decodes the string into its constituants.
    public UK_RuntimeDesc(string encoded) {
        Decode(encoded);
    }


    // ======================================================================
    // Archiving
    // ----------------------------------------------------------------------
    // Encode the runtime descriptor into a string.
    // Format: ObjectType:company:package:classType:methodName<[out] paramName[:=defaultValue]:paramType; ...>
    public string Encode(int id) {
        string result= UK_Archive.Encode(id)+":"+UK_Archive.Encode(ObjectType)+":"+Company+":"+Package+":"+UK_Archive.Encode(Name ?? "")+":"+UK_Archive.Encode(ClassType)+":"+MethodName+"<";
        for(int i= 0; i < ParamTypes.Length; ++i) {
            if(ParamIsOuts[i]) result+= "out ";
            result+= ParamNames[i];
            if(ParamDefaultValues[i] != null) {
                if(UK_Types.IsA<UnityEngine.Object>(ParamTypes[i])) {
                    result+= ":="+UK_Archive.Encode((int)ParamDefaultValues[i]);
                } else {
                    result+= ":="+UK_Archive.Encode(ParamDefaultValues[i]);                    
                }
            }
            result+= ":"+UK_Archive.Encode(ParamTypes[i]);
            if(i != ParamTypes.Length-1) result+= ";";
        }
        if(ReturnType != null) {
            if(ParamTypes.Length != 0) result+=";";
            result+= "ret "+(ReturnName != null ? ReturnName : "out")+":"+UK_Archive.Encode(ReturnType);
        }
        result+=">{}";
        return result;
    }
    // ----------------------------------------------------------------------
    // Fills the runtime descriptor from an encoded string.
    public UK_RuntimeDesc Decode(string encoded) {
        // object id
//        Debug.Log("Decoding object id=> "+encoded);
        int end= encoded.IndexOf(':');
        Id= UK_Archive.Decode<int>(encoded.Substring(0, end));
        encoded= encoded.Substring(end+1, encoded.Length-end-1);
        // object type
//        Debug.Log("Decoding object type=> "+encoded);
        end= encoded.IndexOf(':');
        string objectTypeStr= encoded.Substring(0, end);
        ObjectType= UK_Archive.Decode<UK_ObjectTypeEnum>(objectTypeStr);
        encoded= encoded.Substring(end+1, encoded.Length-end-1);
        // company
//        Debug.Log("Decoding company=> "+encoded);
        end= encoded.IndexOf(':');
        Company= encoded.Substring(0, end);
        encoded= encoded.Substring(end+1, encoded.Length-end-1);
        // package
//        Debug.Log("Decoding package=> "+encoded);
        end= encoded.IndexOf(':');
        Package= encoded.Substring(0, end);
        encoded= encoded.Substring(end+1, encoded.Length-end-1);
        // name
//        Debug.Log("Decoding name=> "+encoded);
        end= encoded.IndexOf(':');
        Name= UK_Archive.Decode<string>(encoded.Substring(0, end));
        encoded= encoded.Substring(end+1, encoded.Length-end-1);
        // class type
//        Debug.Log("Decoding class type=> "+encoded);
        end= encoded.IndexOf(':');
        string className= encoded.Substring(0, end);
        ClassType= UK_Archive.Decode<Type>(className);
        encoded= encoded.Substring(end+1, encoded.Length-end-1);
        // method name
//        Debug.Log("Decoding method name=> "+encoded);
        end= encoded.IndexOf('<');
        MethodName= encoded.Substring(0, end);
        encoded= encoded.Substring(end+1, encoded.Length-end-1);
        // parameters
//        Debug.Log("Decoding parameters=> "+encoded);
        end= encoded.IndexOf('>');
        string parameterString= encoded.Substring(0, end);
        encoded= encoded.Substring(end+1, encoded.Length-end-1);
        ParseParameters(parameterString);
        return this;
    }
    // ----------------------------------------------------------------------
    // Extracts the type of the parameters from the given string.
    void ParseParameters(string paramStr) {
        ReturnType= null;
        List<bool>      paramIsOut   = new List<bool>();
        List<Type>      paramTypes   = new List<Type>();
        List<string>    paramNames   = new List<string>();
        List<object>    paramDefaults= new List<object>();
        while(paramStr.Length > 0) {
            // Return type
            int end= -1;
            if(paramStr.StartsWith("ret ")) {
                end= paramStr.IndexOf(':');
                ReturnName= paramStr.Substring(4, end-4);
                paramStr= paramStr.Substring(end+1, paramStr.Length-end-1);
                end= paramStr.IndexOf(';');
                ReturnType= UK_Archive.Decode<Type>(paramStr.Substring(0, end > 0 ? end : paramStr.Length));
                paramStr= end > 0 ? paramStr.Substring(end+1, paramStr.Length-end-1) : "";
                continue;
            }
            // in/out parameter type
            if(paramStr.StartsWith("out ")) {
                paramIsOut.Add(true);
                paramStr= paramStr.Substring(4, paramStr.Length-4);
            } else {
                paramIsOut.Add(false);
            }                
            // parameter name
            end= paramStr.IndexOf(':');
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
            Type paramType= UK_Archive.Decode<Type>(paramStr.Substring(0, end > 0 ? end : paramStr.Length));
            paramTypes.Add(paramType);
            paramStr= end > 0 ? paramStr.Substring(end+1, paramStr.Length-end-1) : "";
            // parameter default value (part 2)
            if(defaultValueStr != null) {
                if(UK_Types.IsA<UnityEngine.Object>(paramType)) {
                    paramDefaults.Add(UK_Archive.Decode(defaultValueStr, typeof(int)));
                } else {
                    paramDefaults.Add(UK_Archive.Decode(defaultValueStr, paramType));                    
                }
            } else {
                paramDefaults.Add(null);                
            }
        }
        ParamIsOuts= paramIsOut.ToArray();
        ParamTypes = paramTypes.ToArray();
        ParamNames = paramNames.ToArray();
        ParamDefaultValues= paramDefaults.ToArray();
    }

}
