using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[Serializable()]
public class iCS_RuntimeDesc {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public iCS_ObjectTypeEnum   ObjectType          = iCS_ObjectTypeEnum.Unknown;
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
    public object GetDefaultValue(int portId, iCS_Storage storage) {
        if(iCS_Types.IsA<UnityEngine.Object>(PortTypes[portId])) {
            object id= PortDefaultValues[portId];
            if(id == null) return null;
            return storage.UnityObjects[(int)id];
        }
        return PortDefaultValues[portId];    
    }
    public void SetDefaultValue(int portId, object obj, iCS_Storage storage) {
        if(iCS_Types.IsA<UnityEngine.Object>(PortTypes[portId])) {
            object idObj= PortDefaultValues[portId];
            if(idObj == null) {
                PortDefaultValues[portId]= storage.AddUnityObject(obj as UnityEngine.Object);
                return;
            }
            int id= (int)idObj;
            if(storage.IsValidUnityObject(id)) {
                storage.SetUnityObject(id, obj as UnityEngine.Object);
            } else {
                PortDefaultValues[portId]= storage.AddUnityObject(obj as UnityEngine.Object);
            }
            return;
        }
        PortDefaultValues[portId]= obj;
    }
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_RuntimeDesc() {}
    // ----------------------------------------------------------------------
    // Decodes the string into its constituants.
    public iCS_RuntimeDesc(string encoded) {
        Decode(encoded);
    }
    // ----------------------------------------------------------------------
    public iCS_RuntimeDesc(iCS_ObjectTypeEnum objectType, string company, string package, string name,
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
            case iCS_ObjectTypeEnum.Constructor: {
                PortNames= new string[paramNames.Length+1];
                Array.Copy(paramNames, PortNames, paramNames.Length);
                PortNames[paramNames.Length]= "this";
                PortTypes= new Type[paramTypes.Length+1];
                Array.Copy(paramTypes, PortTypes, paramTypes.Length);
                PortTypes[paramTypes.Length]= classType;
                PortIsOuts= new bool[paramIsOuts.Length+1];
                Array.Copy(paramIsOuts, PortIsOuts, paramIsOuts.Length);
                PortIsOuts[paramIsOuts.Length]= true;
                PortDefaultValues= new object[paramDefaultValues.Length+1];
                Array.Copy(paramDefaultValues, PortDefaultValues, paramDefaultValues.Length);
                PortDefaultValues[paramDefaultValues.Length]= iCS_Types.DefaultValue(classType);
                break;
            }
            case iCS_ObjectTypeEnum.InstanceMethod: {
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
                PortDefaultValues[paramDefaultValues.Length]= iCS_Types.DefaultValue(returnType);
                PortDefaultValues[paramDefaultValues.Length+1]= iCS_Types.DefaultValue(classType);
                PortDefaultValues[paramDefaultValues.Length+2]= iCS_Types.DefaultValue(classType);
                break;
            }
            case iCS_ObjectTypeEnum.Conversion:
            case iCS_ObjectTypeEnum.StaticMethod: {
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
                PortDefaultValues[paramDefaultValues.Length]= iCS_Types.DefaultValue(returnType);
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

    // ----------------------------------------------------------------------
	public iCS_RuntimeDesc CopyFrom(iCS_RuntimeDesc rtDesc) {
	    ObjectType       = rtDesc.ObjectType;
	    Company          = rtDesc.Company;
	    Package          = rtDesc.Package;
	    DisplayName      = rtDesc.DisplayName;
	    ClassType        = rtDesc.ClassType;
	    MethodName       = rtDesc.MethodName;
	    PortNames        = rtDesc.PortNames;
	    PortTypes        = rtDesc.PortTypes;
	    PortIsOuts       = rtDesc.PortIsOuts;
	    PortDefaultValues= rtDesc.PortDefaultValues;
		return this;
	}
	
    // ======================================================================
    // Archiving
    // ----------------------------------------------------------------------
    // Fills the runtime descriptor from an encoded string.
    public iCS_RuntimeDesc Decode(string encoded) {
        iCS_Coder coder= new iCS_Coder();
        coder.Archive= encoded;
        return CopyFrom(coder.DecodeObjectForKey("rtDesc") as iCS_RuntimeDesc);
    }
}
