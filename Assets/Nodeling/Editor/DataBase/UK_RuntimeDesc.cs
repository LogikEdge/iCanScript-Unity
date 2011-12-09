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
    public string               DisplayName         = null;
    public Type                 ClassType           = null;
    public string               MethodName          = null;
    public string[]             PortNames           = new string[0];
    public Type[]               PortTypes           = new Type[0];
    public bool[]               PortIsOuts          = new bool[0];
    public object[]             PortDefaultValues   = new object[0];
    
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
    // ----------------------------------------------------------------------
    public object GetDefaultValue(int idx, UK_IStorage storage) {
        return storage.GetDefaultValue(this, idx);
    }
    public void SetDefaultValue(int idx, object obj, UK_IStorage storage) {
        storage.SetDefaultValue(this, idx, obj);
    }
    // ----------------------------------------------------------------------
    public string[] ParamNames {
        get {
            string[] result= null;
            switch(ObjectType) {
                case UK_ObjectTypeEnum.Module: {
                    result= PortNames;
                    break;
                }
                case UK_ObjectTypeEnum.InstanceMethod: {
                    result= new string[PortNames.Length-3];
                    Array.Copy(PortNames, result, result.Length);
                    break;
                }
                case UK_ObjectTypeEnum.Conversion:
                case UK_ObjectTypeEnum.StaticMethod: {
                    result= new string[PortNames.Length-1];
                    Array.Copy(PortNames, result, result.Length);
                    break;
                }
                default: {
                    result= new string[0]; 
                    break;
                }
            }
            return result;            
        }
    }
    // ----------------------------------------------------------------------
    public Type[] ParamTypes {
        get {
            Type[] result= null;
            switch(ObjectType) {
                case UK_ObjectTypeEnum.Module: {
                    result= PortTypes;
                    break;
                }
                case UK_ObjectTypeEnum.InstanceMethod: {
                    result= new Type[PortTypes.Length-3];
                    Array.Copy(PortTypes, result, result.Length);
                    break;
                }
                case UK_ObjectTypeEnum.Conversion:
                case UK_ObjectTypeEnum.StaticMethod: {
                    result= new Type[PortTypes.Length-1];
                    Array.Copy(PortTypes, result, result.Length);
                    break;
                }
                default: {
                    result= new Type[0]; 
                    break;
                }
            }
            return result;            
        }
    }
    // ----------------------------------------------------------------------
    public bool[] ParamIsOuts {
        get {
            bool[] result= null;
            switch(ObjectType) {
                case UK_ObjectTypeEnum.Module: {
                    result= PortIsOuts;
                    break;
                }
                case UK_ObjectTypeEnum.InstanceMethod: {
                    result= new bool[PortIsOuts.Length-3];
                    Array.Copy(PortIsOuts, result, result.Length);
                    break;
                }
                case UK_ObjectTypeEnum.Conversion:
                case UK_ObjectTypeEnum.StaticMethod: {
                    result= new bool[PortIsOuts.Length-1];
                    Array.Copy(PortIsOuts, result, result.Length);
                    break;
                }
                default: {
                    result= new bool[0]; 
                    break;
                }
            }
            return result;            
        }
    }
    // ----------------------------------------------------------------------
    public string ReturnName {
        get {
            string result= null;
            switch(ObjectType) {
                case UK_ObjectTypeEnum.InstanceMethod: {
                    result= PortNames[PortNames.Length-3];
                    break;
                }
                case UK_ObjectTypeEnum.Conversion:
                case UK_ObjectTypeEnum.StaticMethod: {
                    result= PortNames[PortNames.Length-1];
                    break;
                }
                default: {
                    result= null; 
                    break;
                }
            }
            return result;                    
        }
    }
    // ----------------------------------------------------------------------
    public Type ReturnType {
        get {
            Type result= null;
            switch(ObjectType) {
                case UK_ObjectTypeEnum.InstanceMethod: {
                    result= PortTypes[PortTypes.Length-3];
                    break;
                }
                case UK_ObjectTypeEnum.Conversion:
                case UK_ObjectTypeEnum.StaticMethod: {
                    result= PortTypes[PortTypes.Length-1];
                    break;
                }
                default: {
                    result= null; 
                    break;
                }
            }
            return result;                    
        }
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
    // ----------------------------------------------------------------------
    public UK_RuntimeDesc(UK_ObjectTypeEnum objectType, string company, string package, string name,
                          Type classType, string methodName,
                          string[] paramNames, Type[] paramTypes, bool[] paramIsOuts, object[] paramDefaultValues,
                          string returnName, Type returnType) {
        ObjectType= objectType;
        Company= company;
        Package= package;
        DisplayName= name;
        ClassType= classType;
        MethodName= methodName;
        switch(ObjectType) {
            case UK_ObjectTypeEnum.InstanceMethod: {
                PortNames= new string[paramNames.Length+3];
                Array.Copy(paramNames, PortNames, paramNames.Length);
                PortNames[paramNames.Length]= returnName;
                PortNames[paramNames.Length+1]= "this";
                PortNames[paramNames.Length+2]= "this";
                PortTypes= new Type[paramTypes.Length+3];
                Array.Copy(paramTypes, PortTypes, paramTypes.Length);
                PortTypes[paramTypes.Length]= returnType;
                PortTypes[paramTypes.Length+1]= classType;
                PortTypes[paramTypes.Length+2]= classType;
                PortIsOuts= new bool[paramIsOuts.Length+3];
                Array.Copy(paramIsOuts, PortIsOuts, paramIsOuts.Length);
                PortIsOuts[paramIsOuts.Length]= true;
                PortIsOuts[paramIsOuts.Length+1]= false;
                PortIsOuts[paramIsOuts.Length+2]= true;
                PortDefaultValues= new object[paramDefaultValues.Length+3];
                Array.Copy(paramDefaultValues, PortDefaultValues, paramDefaultValues.Length);
                PortDefaultValues[paramDefaultValues.Length]= UK_Types.DefaultValue(returnType);
                PortDefaultValues[paramDefaultValues.Length+1]= UK_Types.DefaultValue(classType);
                PortDefaultValues[paramDefaultValues.Length+2]= UK_Types.DefaultValue(classType);
                break;
            }
            case UK_ObjectTypeEnum.Conversion:
            case UK_ObjectTypeEnum.StaticMethod: {
                PortNames= new string[paramNames.Length+1];
                Array.Copy(paramNames, PortNames, paramNames.Length);
                PortNames[paramNames.Length]= returnName;
                PortTypes= new Type[paramTypes.Length+1];
                Array.Copy(paramTypes, PortTypes, paramTypes.Length);
                PortTypes[paramTypes.Length]= returnType;
                PortIsOuts= new bool[paramIsOuts.Length+1];
                Array.Copy(paramIsOuts, PortIsOuts, paramIsOuts.Length);
                PortIsOuts[paramIsOuts.Length]= true;
                PortDefaultValues= new object[paramDefaultValues.Length+1];
                Array.Copy(paramDefaultValues, PortDefaultValues, paramDefaultValues.Length);
                PortDefaultValues[paramDefaultValues.Length]= UK_Types.DefaultValue(returnType);
                break;
            }
            default: {
                PortNames= paramNames;
                PortTypes= paramTypes;
                PortIsOuts= paramIsOuts;
                PortDefaultValues= paramDefaultValues;
                break;
            }
        }        
    }

    // ======================================================================
    // Archiving
    // ----------------------------------------------------------------------
    // Encode the runtime descriptor into a string.
    // Format: ObjectType:company:package:classType:methodName<[out] paramName[:=defaultValue]:paramType; ...>
    public string Encode(int id) {
        string result= UK_Archive.Encode(id)+":"+UK_Archive.Encode(ObjectType)+":"+Company+":"+Package+":"+UK_Archive.Encode(DisplayName ?? "")+":"+UK_Archive.Encode(ClassType)+":"+MethodName+"<";
        for(int i= 0; i < PortTypes.Length; ++i) {
            if(PortIsOuts[i]) result+= "out ";
            result+= PortNames[i];
            if(PortDefaultValues[i] != null) {
                if(UK_Types.IsA<UnityEngine.Object>(PortTypes[i])) {
                    result+= ":="+UK_Archive.Encode((int)PortDefaultValues[i]);
                } else {
                    string defaultValueStr= UK_Archive.Encode(PortDefaultValues[i]);
                    if(defaultValueStr != null) {
                        result+= ":="+defaultValueStr;                                            
                    }
                }
            }
            result+= ":"+UK_Archive.Encode(PortTypes[i]);
            if(i != PortTypes.Length-1) result+= ";";
        }
        result+=">{}";
//        Debug.Log("Encode: "+result);
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
        // display name
//        Debug.Log("Decoding name=> "+encoded);
        end= encoded.IndexOf(':');
        DisplayName= UK_Archive.Decode<string>(encoded.Substring(0, end));
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
        ParsePorts(parameterString);
        return this;
    }
    // ----------------------------------------------------------------------
    // Extracts the type of the parameters from the given string.
    void ParsePorts(string paramStr) {
        List<bool>      portIsOut   = new List<bool>();
        List<Type>      portTypes   = new List<Type>();
        List<string>    portNames   = new List<string>();
        List<object>    portDefaults= new List<object>();
        while(paramStr.Length > 0) {
            // Return type
            int end= -1;
            // in/out parameter type
            if(paramStr.StartsWith("out ")) {
                portIsOut.Add(true);
                paramStr= paramStr.Substring(4, paramStr.Length-4);
            } else {
                portIsOut.Add(false);
            }                
            // parameter name
            end= paramStr.IndexOf(':');
            portNames.Add(paramStr.Substring(0, end));
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
            Type portType= UK_Archive.Decode<Type>(paramStr.Substring(0, end > 0 ? end : paramStr.Length));
            portTypes.Add(portType);
            paramStr= end > 0 ? paramStr.Substring(end+1, paramStr.Length-end-1) : "";
            // parameter default value (part 2)
            if(defaultValueStr != null) {
                if(UK_Types.IsA<UnityEngine.Object>(portType)) {
                    portDefaults.Add(UK_Archive.Decode(defaultValueStr, typeof(int)));
                } else {
                    portDefaults.Add(UK_Archive.Decode(defaultValueStr, portType));                    
                }
            } else {
                portDefaults.Add(UK_Types.DefaultValue(portType));                
            }
        }
        PortIsOuts= portIsOut.ToArray();
        PortTypes = portTypes.ToArray();
        PortNames = portNames.ToArray();
        PortDefaultValues= portDefaults.ToArray();
    }

}
