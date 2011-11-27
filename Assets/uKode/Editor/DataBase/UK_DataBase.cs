using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class UK_DataBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public static bool                      IsDirty  = true;
    public static List<UK_ReflectionDesc>   Functions= new List<UK_ReflectionDesc>();
    public static string[]                  FunctionMenu= null;
    
    // ======================================================================
    // DataBase functionality
    // ----------------------------------------------------------------------
    public static void Sort(int min= 0, int max= -1) {
        int len= Functions.Count;
        if(min > len) return;
        if(max == -1 || max > len) max= len-1;
        int minRestart= min;
        while(min != max) {
            if(CompareFunctionNames(Functions[min], Functions[min+1]) > 0) {
                UK_ReflectionDesc tmp= Functions[min];
                Functions[min]= Functions[min+1];
                Functions[min+1]= tmp;
                if(min != 0) --min;
            } else {
                ++min;
                if(min < minRestart) {
                    min= minRestart;
                } else {
                    minRestart= min;
                }
            }
        }
    }
    // ----------------------------------------------------------------------
    // Returns all the company names for which a uCode component exists.
    public static string[] GetCompanies() {
        List<string> companies= new List<string>();
        foreach(var func in Functions) {
            uCode.AddUniqu<string>(func.Company, companies);
        }
        return companies.ToArray();
    }
    // ----------------------------------------------------------------------
    // Returns all available packages of the given company.
    public static string[] GetPackages(string company) {
        List<string> packages= new List<string>();
        foreach(var func in Functions) {
            if(func.Company == company) {
                uCode.AddUniqu<string>(func.Package, packages);                
            }
        }
        return packages.ToArray();
    }
    // ----------------------------------------------------------------------
    // Returns all available functions of the given company/package.
    public static string[] GetFunctions(string company, string package) {
        List<string> functions= new List<string>();
        foreach(var func in Functions) {
            if(func.Company == company && func.Package == package) {
                uCode.AddUniqu<string>(func.DisplayName, functions);                
            }
        }
        return functions.ToArray();
    }
    // ----------------------------------------------------------------------
    // Returns all available functions parameters for the given
    // company/package/function.
    public static string[] GetFunctionSignatures(string company, string package, string functionName) {
        List<string> parameters= new List<string>();
        foreach(var func in Functions) {
            if(func.Company == company && func.Package == package && func.DisplayName == functionName) {
                parameters.Add(GetFunctionSignature(func));
            }
        }
        return parameters.ToArray();
    }

    // ----------------------------------------------------------------------
    public static string GetFunctionSignature(UK_ReflectionDesc desc) {
        string signature= TypeName(desc.ReturnType);
        signature+= " "+desc.DisplayName+"(";
        if(desc.ObjectType == UK_ObjectTypeEnum.InstanceMethod) {
            signature+= TypeName(desc.ClassType)+" this";
            if(desc.ParamNames.Length != 0) signature+=", ";
        }
        for(int i= 0; i < desc.ParamNames.Length; ++i) {
            signature+= TypeName(desc.RuntimeDesc.ParamTypes[i])+" "+desc.ParamNames[i];
            if(i != desc.ParamNames.Length-1) signature+=", ";
        }
        return signature+")";
    }
    // ----------------------------------------------------------------------
    // Returns the function name in the form of "company/package/displayName".
    public static string GetFunctionName(UK_ReflectionDesc desc) {
        return desc.Company+"/"+desc.Package+"/"+desc.DisplayName;
    }
    // ----------------------------------------------------------------------
    // Returns 0 if equal, negative if first is smaller and
    // positive if first is greather.
    public static int CompareFunctionNames(UK_ReflectionDesc d1, UK_ReflectionDesc d2) {
        int result= d1.Company.CompareTo(d2.Company);
        if(result != 0) return result;
        result= d1.Package.CompareTo(d2.Package);
        if(result != 0) return result;
        return d1.DisplayName.CompareTo(d2.DisplayName);
    }
    // ----------------------------------------------------------------------
    public static string[] BuildMenu() {
        if(!IsDirty) return FunctionMenu;
        Sort();
        List<string> menu= new List<string>();
        string previousName= "";
        bool needsSignature= false;
        for(int i= 0; i < Functions.Count; ++i) {
            string newName= GetFunctionName(Functions[i]);
            if(previousName == newName) needsSignature= true;
            if(previousName != "") {
                if(needsSignature) { menu.Add(previousName+"/"+GetFunctionSignature(Functions[i-1])); }
                else               { menu.Add(previousName); }
            }
            if(previousName != newName) {
                needsSignature= false;
                previousName= newName;
            }
        }
        if(previousName != "") {
            if(needsSignature) { menu.Add(previousName+"/"+GetFunctionSignature(Functions[Functions.Count-1])); }
            else               { menu.Add(previousName); }
        }
        FunctionMenu= menu.ToArray();
        IsDirty= false;
        return FunctionMenu;
    }
    // ----------------------------------------------------------------------
    static string TypeName(Type type) {
        if(type == null) return "void";
        if(type == typeof(float)) return "float";
        return type.Name;
    }
    // ----------------------------------------------------------------------
    // Returns the descriptor associated with the given company/package/function.
    public static UK_ReflectionDesc GetDescriptor(string company, string package, string functionName, string signature) {
        foreach(var desc in Functions) {
            if(desc.Company == company &&
               desc.Package == package &&
               desc.DisplayName == functionName) {
                   if(signature == null) return desc;
                   if(signature == GetFunctionSignature(desc)) return desc;
               }
        }
        return null;
    }
    // ----------------------------------------------------------------------
    // Finds a conversion that matches the given from/to types.
    public static UK_ReflectionDesc FindConversion(Type fromType, Type toType) {
        foreach(var desc in Functions) {
            if(IsConversion(desc)) {
                UK_RuntimeDesc conv= desc.RuntimeDesc;
                if(UK_Types.CanBeConnectedWithoutConversion(fromType, conv.ParamTypes[0]) &&
                   UK_Types.CanBeConnectedWithoutConversion(conv.ReturnType, toType)) return desc;
            }
        }
        return null;
    }
    // ----------------------------------------------------------------------
    // Returns true if the given desc is a conversion function.
    public static bool IsConversion(UK_ReflectionDesc desc) {
        return desc.RuntimeDesc.ObjectType == UK_ObjectTypeEnum.Conversion;
    }
    
    // ======================================================================
    // Container management functions
    // ----------------------------------------------------------------------
    // Removes all previously recorded functions.
    public static void Clear() {
        Functions.Clear();
    }
    // ----------------------------------------------------------------------
    public static void AddStaticField(string company, string package, string displayName, string toolTip, string iconPath,
                                      Type classType, FieldInfo field, UK_ParamDirectionEnum direction) {
        
    }
    // ----------------------------------------------------------------------
    public static void AddInstanceField(string company, string package, string displayName, string toolTip, string iconPath,
                                        Type classType, FieldInfo field, UK_ParamDirectionEnum direction) {
        Add(company, package, displayName, toolTip, iconPath,
            UK_ObjectTypeEnum.InstanceField, classType, null,
            new bool[3]{false, direction==UK_ParamDirectionEnum.Out, true},
            new string[3]{"this", field.Name, "this"},
            new Type[3]{classType, field.FieldType, classType},
            new object[3]{UK_Types.DefaultValue(classType), UK_Types.DefaultValue(field.FieldType),UK_Types.DefaultValue(classType)},
            null, null);
    }
    // ----------------------------------------------------------------------
    public static void AddInstanceMethod(string company, string package, string displayName, string toolTip, string iconPath,
                                       Type classType, MethodInfo methodInfo,
                                       bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaults,
                                       string retName, Type retType) {
        Add(company, package, displayName, toolTip, iconPath,
            UK_ObjectTypeEnum.InstanceMethod, classType, methodInfo,
            paramIsOuts, paramNames, paramTypes, paramDefaults,
            retName, retType);
    }
    // ----------------------------------------------------------------------
    // Adds an execution function (no context).
    public static void AddStaticMethod(string company, string package, string displayName, string toolTip, string iconPath,
                                       Type classType, MethodInfo methodInfo,
                                       bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaults,
                                       string retName, Type retType) {
        Add(company, package, displayName, toolTip, iconPath,
            UK_ObjectTypeEnum.StaticMethod, classType, methodInfo,
            paramIsOuts, paramNames, paramTypes, paramDefaults,
            retName, retType);
    }
    // ----------------------------------------------------------------------
    // Adds a conversion function
    public static void AddConversion(string company, string package, string iconPath, Type classType, MethodInfo methodInfo, Type fromType, Type toType) {
        // Don't accept automatic conversion if it already exist.
        foreach(var desc in Functions) {
            if(IsConversion(desc)) {
                UK_RuntimeDesc conv= desc.RuntimeDesc;
                if(conv.ParamTypes[0] == fromType && conv.ReturnType == toType) {
                    Debug.LogWarning("Duplicate conversion function from "+fromType+" to "+toType+" exists in classes "+conv.Method.DeclaringType+" and "+methodInfo.DeclaringType);
                    return;
                }                
            }
        }
        Add(company, package, fromType.Name+"->"+toType.Name, "Converts from "+fromType.Name+" to "+toType.Name, iconPath,
            UK_ObjectTypeEnum.Conversion, classType, methodInfo,
            new bool[1]{false}, new string[1]{fromType.Name}, new Type[1]{fromType}, new object[1]{null},
            toType.Name, toType);        
    }
    // ----------------------------------------------------------------------
    // Adds a new database record.
    public static UK_ReflectionDesc Add(string company, string package, string displayName, string toolTip, string iconPath,
                                        UK_ObjectTypeEnum objType, Type classType, MethodInfo methodInfo,
                                        bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaults,
                                        string retName, Type retType) {
        UK_ReflectionDesc fd= new UK_ReflectionDesc(company, package, displayName, toolTip, iconPath,
                                                    objType, classType, methodInfo,
                                                    paramIsOuts, paramNames, paramTypes, paramDefaults,
                                                    retName, retType);
        Functions.Add(fd);
        if(Functions.Count >= 2) { Sort(Functions.Count-2, Functions.Count-1); }
        IsDirty= true;
        return fd;
    }
    
}
